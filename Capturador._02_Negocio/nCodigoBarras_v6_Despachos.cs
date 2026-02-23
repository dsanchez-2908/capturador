using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using PdfiumViewer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ZXing;
using ZXing.Common;

namespace Capturador._02_Negocio;

public class nCodigoBarras_v6_Despachos
{
	public static List<eCodigosBarrasEncontrados_v2> buscarCodigoBarras(eUsuario pUsuarioLogueado, eDespacho pDespacho)
	{
		List<eCodigosBarrasEncontrados_v2> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v2>();
		string carpeta = Path.GetDirectoryName(pDespacho.dsRutaArchivoPDF);
		string nombreArchivoTxt = pDespacho.dsNombreLote + ".txt";
		string rutaArchivoTxt = Path.Combine(carpeta, nombreArchivoTxt);
		if (File.Exists(rutaArchivoTxt))
		{
			string[] lineas = File.ReadAllLines(rutaArchivoTxt);
			string[] array = lineas;
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
						eCodigosBarrasEncontrados_v2 item = new eCodigosBarrasEncontrados_v2
						{
							CarpetaInicial = campos[0].Trim(),
							NombreLote = campos[1].Trim(),
							Despacho = campos[2].Trim(),
							NumeroPagina = (int.TryParse(campos[3].Trim(), out numPag) ? numPag : 0),
							ValorEncontrado = Convert.ToInt32(campos[4].Trim())
						};
						oListaCodigosBarrasEncontrados.Add(item);
					}
				}
			}
		}
		else
		{
			oListaCodigosBarrasEncontrados = extraerCodigoBarras(pDespacho);
			guardarCodigosBarrasEncontrados(oListaCodigosBarrasEncontrados);
		}
		return oListaCodigosBarrasEncontrados;
	}

	private static void guardarCodigosBarrasEncontrados(List<eCodigosBarrasEncontrados_v2> pListaCodigosBarrasEncontrados)
	{
		if (pListaCodigosBarrasEncontrados == null || pListaCodigosBarrasEncontrados.Count == 0)
		{
			return;
		}
		eCodigosBarrasEncontrados_v2 primerElemento = pListaCodigosBarrasEncontrados[0];
		string carpeta = Path.GetDirectoryName(primerElemento.CarpetaInicial);
		string nombreArchivo = primerElemento.NombreLote;
		if (!nombreArchivo.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
		{
			nombreArchivo += ".txt";
		}
		string rutaCompleta = Path.Combine(carpeta, nombreArchivo);
		List<string> lineas = new List<string>();
		foreach (eCodigosBarrasEncontrados_v2 item in pListaCodigosBarrasEncontrados)
		{
			string linea = $"{item.CarpetaInicial};{item.NombreLote};{item.Despacho};{item.NumeroPagina};{item.ValorEncontrado}";
			lineas.Add(linea);
		}
		File.AppendAllLines(rutaCompleta, lineas);
	}

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
			using PdfiumViewer.PdfDocument document = PdfiumViewer.PdfDocument.Load(pdfPath);
			DecodingOptions decodingOptions = new DecodingOptions
			{
				PossibleFormats = new List<BarcodeFormat>
				{
					BarcodeFormat.CODE_128,
					BarcodeFormat.CODE_39,
					BarcodeFormat.ITF
				}
			};
			BarcodeReader reader = new BarcodeReader
			{
				Options = decodingOptions,
				AutoRotate = true
			};
			for (int i = 0; i < document.PageCount; i++)
			{
				using Image pageImage = document.Render(i, 600f, 600f, PdfRenderFlags.None);
				Result result = reader.Decode((Bitmap)pageImage);
				if (result != null && int.TryParse(result.Text, out var valorEncontrado) && valorEncontrado >= 1 && valorEncontrado <= 8)
				{
					eCodigosBarrasEncontrados_v2 encontrado = new eCodigosBarrasEncontrados_v2
					{
						CarpetaInicial = pDespacho.dsRutaArchivoPDF,
						NombreLote = pDespacho.dsNombreLote,
						Despacho = pDespacho.dsDespacho,
						NumeroPagina = i + 1,
						ValorEncontrado = valorEncontrado
					};
					if (!oListaEncontrado.Any((eCodigosBarrasEncontrados_v2 c) => c.CarpetaInicial == encontrado.CarpetaInicial && c.NombreLote == encontrado.NombreLote && c.Despacho == encontrado.Despacho && c.NumeroPagina == encontrado.NumeroPagina && c.ValorEncontrado == encontrado.ValorEncontrado))
					{
						oListaEncontrado.Add(encontrado);
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al extraer códigos de barras del PDF " + pdfPath + ": " + ex.Message);
		}
		return oListaEncontrado;
	}

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
