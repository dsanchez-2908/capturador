using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using PdfiumViewer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ZXing;
using ZXing.Common;

namespace Capturador._02_Negocio;

internal class nCodigoBarras_v7_Despachos
{
	private static readonly Dictionary<string, SemaphoreSlim> _fileLocks = new Dictionary<string, SemaphoreSlim>();

	private static readonly object _lockDictionary = new object();

	private static SemaphoreSlim GetFileLock(string rutaArchivo)
	{
		lock (_lockDictionary)
		{
			if (!_fileLocks.ContainsKey(rutaArchivo))
			{
				_fileLocks[rutaArchivo] = new SemaphoreSlim(1, 1);
			}
			return _fileLocks[rutaArchivo];
		}
	}

	public static List<eCodigosBarrasEncontrados_v2> buscarCodigoBarras(eUsuario pUsuarioLogueado, eDespacho pDespacho)
	{
		return buscarCodigoBarrasAsync(pUsuarioLogueado, pDespacho).GetAwaiter().GetResult();
	}

	public static async Task<List<eCodigosBarrasEncontrados_v2>> buscarCodigoBarrasAsync(eUsuario pUsuarioLogueado, eDespacho pDespacho)
	{
		List<eCodigosBarrasEncontrados_v2> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v2>();
		string carpeta = Path.GetDirectoryName(pDespacho.dsRutaArchivoPDF);
		string nombreArchivoTxt = pDespacho.dsNombreLote + ".txt";
		string rutaArchivoTxt = Path.Combine(carpeta, nombreArchivoTxt);
		SemaphoreSlim fileLock = GetFileLock(rutaArchivoTxt);
		await fileLock.WaitAsync();
		try
		{
			if (File.Exists(rutaArchivoTxt))
			{
				string[] array = await Task.Run(() => File.ReadAllLines(rutaArchivoTxt));
				foreach (string linea in array)
				{
					if (string.IsNullOrWhiteSpace(linea))
					{
						continue;
					}
					string[] campos = linea.Split(';');
					if (campos.Length >= 5)
					{
						string nombreLoteArchivo = campos[1].Trim();
						string nombreArchivo = campos[2].Trim();
						if (nombreLoteArchivo.Equals(pDespacho.dsNombreLote, StringComparison.OrdinalIgnoreCase) && nombreArchivo.Equals(pDespacho.dsDespacho, StringComparison.OrdinalIgnoreCase))
						{
							int numPag;
							int valor;
							eCodigosBarrasEncontrados_v2 item = new eCodigosBarrasEncontrados_v2
							{
								CarpetaInicial = campos[0].Trim(),
								NombreLote = campos[1].Trim(),
								Despacho = campos[2].Trim(),
								NumeroPagina = (int.TryParse(campos[3].Trim(), out numPag) ? numPag : 0),
								ValorEncontrado = (int.TryParse(campos[4].Trim(), out valor) ? valor : 0)
							};
							oListaCodigosBarrasEncontrados.Add(item);
						}
					}
				}
				if (oListaCodigosBarrasEncontrados.Count > 0)
				{
					return oListaCodigosBarrasEncontrados;
				}
			}
			oListaCodigosBarrasEncontrados = await Task.Run(() => extraerCodigoBarras(pDespacho));
			if (oListaCodigosBarrasEncontrados.Count > 0)
			{
				await guardarCodigosBarrasEncontradosAsync(oListaCodigosBarrasEncontrados, rutaArchivoTxt);
			}
		}
		finally
		{
			fileLock.Release();
		}
		return oListaCodigosBarrasEncontrados;
	}

	private static async Task guardarCodigosBarrasEncontradosAsync(List<eCodigosBarrasEncontrados_v2> pListaCodigosBarrasEncontrados, string rutaArchivoTxt)
	{
		if (pListaCodigosBarrasEncontrados == null || pListaCodigosBarrasEncontrados.Count == 0)
		{
			return;
		}
		string carpeta = Path.GetDirectoryName(rutaArchivoTxt);
		if (!Directory.Exists(carpeta))
		{
			Directory.CreateDirectory(carpeta);
		}
		List<string> lineas = new List<string>();
		foreach (eCodigosBarrasEncontrados_v2 item in pListaCodigosBarrasEncontrados)
		{
			string linea = $"{item.CarpetaInicial};{item.NombreLote};{item.Despacho};{item.NumeroPagina};{item.ValorEncontrado}";
			lineas.Add(linea);
		}
		await Task.Run(delegate
		{
			File.AppendAllLines(rutaArchivoTxt, lineas);
		});
	}

    // ARCHIVO: nCodigoBarras_v7_Despachos.cs

    private static List<eCodigosBarrasEncontrados_v2> extraerCodigoBarras(eDespacho pDespacho)
    {
        List<eCodigosBarrasEncontrados_v2> oListaEncontrado = new List<eCodigosBarrasEncontrados_v2>();
        string pdfPath = pDespacho.dsRutaArchivoPDF;

        if (!File.Exists(pdfPath))
        {
            throw new Exception("Advertencia: El archivo PDF no existe: " + pdfPath);
        }

        try
        {
            using (PdfiumViewer.PdfDocument document = PdfiumViewer.PdfDocument.Load(pdfPath))
            {
                DecodingOptions decodingOptions = new DecodingOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.CODE_128,
                    BarcodeFormat.CODE_39,
                    BarcodeFormat.ITF
                },
                    TryHarder = false // CAMBIO: Desactivar TryHarder para mayor velocidad
                };

                BarcodeReader reader = new BarcodeReader
                {
                    Options = decodingOptions,
                    AutoRotate = false // CAMBIO: Desactivar AutoRotate innecesario
                };

                // CAMBIO: Procesar en bloques de 10 páginas para liberar memoria frecuentemente
                const int BLOQUE_PAGINAS = 10;
                int totalPaginas = document.PageCount;

                for (int bloqueInicio = 0; bloqueInicio < totalPaginas; bloqueInicio += BLOQUE_PAGINAS)
                {
                    int bloqueFin = Math.Min(bloqueInicio + BLOQUE_PAGINAS, totalPaginas);

                    for (int i = bloqueInicio; i < bloqueFin; i++)
                    {
                        Image pageImage = null;
                        try
                        {
                            // CAMBIO: Reducir a 150 DPI (suficiente para códigos de barras)
                            pageImage = document.Render(i, 150f, 150f, PdfRenderFlags.None);

                            Result result = reader.Decode((Bitmap)pageImage);

                            if (result != null &&
                                int.TryParse(result.Text, out var valorEncontrado) &&
                                valorEncontrado >= 1 &&
                                valorEncontrado <= 5)
                            {
                                eCodigosBarrasEncontrados_v2 encontrado = new eCodigosBarrasEncontrados_v2
                                {
                                    CarpetaInicial = pDespacho.dsRutaArchivoPDF,
                                    NombreLote = pDespacho.dsNombreLote,
                                    Despacho = pDespacho.dsDespacho,
                                    NumeroPagina = i + 1,
                                    ValorEncontrado = valorEncontrado
                                };

                                if (!oListaEncontrado.Any(c =>
                                    c.CarpetaInicial == encontrado.CarpetaInicial &&
                                    c.NombreLote == encontrado.NombreLote &&
                                    c.Despacho == encontrado.Despacho &&
                                    c.NumeroPagina == encontrado.NumeroPagina &&
                                    c.ValorEncontrado == encontrado.ValorEncontrado))
                                {
                                    oListaEncontrado.Add(encontrado);
                                }
                            }
                        }
                        finally
                        {
                            // CAMBIO: Liberación explícita e inmediata
                            if (pageImage != null)
                            {
                                pageImage.Dispose();
                                pageImage = null;
                            }
                        }
                    }

                    // CAMBIO CORREGIDO: Sintaxis correcta para .NET Framework 4.8
                    GC.Collect(2, GCCollectionMode.Forced, true);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(2, GCCollectionMode.Forced, true);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al extraer códigos de barras del PDF " + pdfPath + ": " + ex.Message);
        }

        return oListaEncontrado;
    }

    //   private static List<eCodigosBarrasEncontrados_v2> extraerCodigoBarras(eDespacho pDespacho)
    //{
    //	List<eCodigosBarrasEncontrados_v2> oListaEncontrado = new List<eCodigosBarrasEncontrados_v2>();
    //	string pdfPath = pDespacho.dsRutaArchivoPDF;
    //	if (!File.Exists(pdfPath))
    //	{
    //		throw new Exception("Advertencia: El archivo PDF no existe: " + pdfPath);
    //	}
    //	try
    //	{
    //		using PdfiumViewer.PdfDocument document = PdfiumViewer.PdfDocument.Load(pdfPath);
    //		DecodingOptions decodingOptions = new DecodingOptions
    //		{
    //			PossibleFormats = new List<BarcodeFormat>
    //			{
    //				BarcodeFormat.CODE_128,
    //				BarcodeFormat.CODE_39,
    //				BarcodeFormat.ITF
    //			}
    //		};
    //		BarcodeReader reader = new BarcodeReader
    //		{
    //			Options = decodingOptions,
    //			AutoRotate = true
    //		};
    //		for (int i = 0; i < document.PageCount; i++)
    //		{
    //			using Image pageImage = document.Render(i, 600f, 600f, PdfRenderFlags.None);
    //			Result result = reader.Decode((Bitmap)pageImage);
    //			if (result != null && int.TryParse(result.Text, out var valorEncontrado) && valorEncontrado >= 1 && valorEncontrado <= 5)
    //			{
    //				eCodigosBarrasEncontrados_v2 encontrado = new eCodigosBarrasEncontrados_v2
    //				{
    //					CarpetaInicial = pDespacho.dsRutaArchivoPDF,
    //					NombreLote = pDespacho.dsNombreLote,
    //					Despacho = pDespacho.dsDespacho,
    //					NumeroPagina = i + 1,
    //					ValorEncontrado = valorEncontrado
    //				};
    //				if (!oListaEncontrado.Any((eCodigosBarrasEncontrados_v2 c) => c.CarpetaInicial == encontrado.CarpetaInicial && c.NombreLote == encontrado.NombreLote && c.Despacho == encontrado.Despacho && c.NumeroPagina == encontrado.NumeroPagina && c.ValorEncontrado == encontrado.ValorEncontrado))
    //				{
    //					oListaEncontrado.Add(encontrado);
    //				}
    //			}
    //		}
    //	}
    //	catch (Exception ex)
    //	{
    //		throw new Exception("Error al extraer códigos de barras del PDF " + pdfPath + ": " + ex.Message);
    //	}
    //	return oListaEncontrado;
    //}

    public static string crearCarpeta(string pRutaCarpeta, eDespacho pDespacho)
	{
		string nombreCarpeta = pDespacho.dsDespacho + " - " + pDespacho.cdSerieDocumental + " - " + pDespacho.nuSIGEA + " - " + DateTime.Now.ToString("yyyy-MM-dd");
		return dArchivos.crearCarpeta(pRutaCarpeta, nombreCarpeta);
	}

	public static void ProcesarPDF(eUsuario pUsuarioLogueado, string pRutaSalida, string pRutaArchivoPDF, List<eCodigosBarrasEncontrados_v2> pListaSeparadores, eDespacho pDespacho)
	{
		string outputFolder = crearCarpeta(pRutaSalida, pDespacho);
		int codigoActual = 1;
		List<string> oListaLineasIndice = new List<string>();
		int contador = 1;
		if (!File.Exists(pRutaArchivoPDF))
		{
			throw new Exception("El archivo PDF no existe: " + pRutaArchivoPDF);
		}
		using PdfSharp.Pdf.PdfDocument inputPdf = PdfReader.Open(pRutaArchivoPDF, PdfDocumentOpenMode.Import);
		List<string> generatedPdfs = new List<string>();
		int i;
		for (i = 0; i < inputPdf.PageCount; i++)
		{
			bool paginaConCodigo = false;
			eCodigosBarrasEncontrados_v2 codigoEncontrado = pListaSeparadores.FirstOrDefault((eCodigosBarrasEncontrados_v2 x) => x.Despacho == pDespacho.dsDespacho && x.NumeroPagina == i + 1);
			if (codigoEncontrado != null)
			{
				codigoActual = codigoEncontrado.ValorEncontrado;
				paginaConCodigo = true;
			}
			if (!paginaConCodigo)
			{
				PdfPage page = inputPdf.Pages[i];
				string outputFilePath = Path.Combine(outputFolder, $"{pDespacho.dsDespacho}-{codigoActual}_{contador}.pdf");
				using (PdfSharp.Pdf.PdfDocument outputPdf = new PdfSharp.Pdf.PdfDocument())
				{
					outputPdf.AddPage(page);
					outputPdf.Save(outputFilePath);
					generatedPdfs.Add(outputFilePath);
				}
				string lineaIndice = string.Format("{0}|{1}|{2}|{3}|{4:yyyy.MM.dd HH:mm:ss}|{5}|{6:yyyy.MM.dd HH:mm:ss}|1|{7}|{8}|{9}|{10}", pDespacho.nuGuia, pDespacho.dsDespacho, codigoActual, inputPdf.PageCount, DateTime.Now, pDespacho.dsUsuarioDigitalizacion, DateTime.Now, pUsuarioLogueado.dsUsuario, contador, pDespacho.cdSerieDocumental, pDespacho.nuSIGEA.Replace("/", "-"));
				oListaLineasIndice.Add(lineaIndice);
				contador++;
			}
		}
		string mergedPdfPath = Path.Combine(outputFolder, pDespacho.dsDespacho + "-" + pDespacho.cdSerieDocumental + "-" + pDespacho.nuSIGEA.Replace("/", "-") + ".pdf");
		UnirPDFs(generatedPdfs, mergedPdfPath);
		string txtFilePath = Path.Combine(outputFolder, pDespacho.dsDespacho + "-" + pDespacho.cdSerieDocumental + "-" + pDespacho.nuSIGEA.Replace("/", "-") + ".TXT");
		File.WriteAllLines(txtFilePath, oListaLineasIndice);
	}

	private static void UnirPDFs(List<string> pdfFiles, string outputFilePath)
	{
		using PdfSharp.Pdf.PdfDocument outputPdf = new PdfSharp.Pdf.PdfDocument();
		foreach (string file in pdfFiles)
		{
			if (File.Exists(file))
			{
				try
				{
					using PdfSharp.Pdf.PdfDocument inputPdf = PdfReader.Open(file, PdfDocumentOpenMode.Import);
					foreach (PdfPage page in inputPdf.Pages)
					{
						outputPdf.AddPage(page);
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error al unir PDF " + file + ": " + ex.Message);
				}
				continue;
			}
			throw new Exception("Advertencia: Archivo PDF no encontrado para unir: " + file);
		}
		outputPdf.Save(outputFilePath);
	}
}
