using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using IronBarCode;
using PdfiumViewer;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Tesseract;

namespace Capturador._02_Negocio;

public class nCodigoBarras_v5_Muni
{
	public static List<eCodigosBarrasEncontrados_v3> buscarCodigoBarras(eUsuario pUsuarioLogueado, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		List<eCodigosBarrasEncontrados_v3> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v3>();
		string carpeta = Path.GetDirectoryName(pRutaArchivoPDF);
		string nombreArchivoTxt = pNombreLote + ".txt";
		string rutaArchivoTxt = Path.Combine(carpeta, pNombreLote, nombreArchivoTxt);
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
					string numeroPagina = campos[3].Trim();
					if (nombreLoteArchivo.Equals(pNombreLote, StringComparison.OrdinalIgnoreCase) && nombreArchivo.Equals(pNombreArchivo, StringComparison.OrdinalIgnoreCase))
					{
						int numPag;
						eCodigosBarrasEncontrados_v3 item = new eCodigosBarrasEncontrados_v3
						{
							CarpetaInicial = campos[0].Trim(),
							NombreLote = campos[1].Trim(),
							Despacho = campos[2].Trim(),
							NumeroPagina = (int.TryParse(campos[3].Trim(), out numPag) ? numPag : 0),
							ValorEncontrado = campos[4].Trim()
						};
						oListaCodigosBarrasEncontrados.Add(item);
					}
				}
			}
		}
		else
		{
			oListaCodigosBarrasEncontrados = extraerCodigoBarras(pNombreLote, pNombreArchivo, pRutaArchivoPDF);
			guardarCodigosBarrasEncontrados(oListaCodigosBarrasEncontrados);
		}
		return oListaCodigosBarrasEncontrados;
	}

	private static void guardarCodigosBarrasEncontrados(List<eCodigosBarrasEncontrados_v3> pListaCodigosBarrasEncontrados)
	{
		if (pListaCodigosBarrasEncontrados == null || pListaCodigosBarrasEncontrados.Count == 0)
		{
			return;
		}
		eCodigosBarrasEncontrados_v3 primerElemento = pListaCodigosBarrasEncontrados[0];
		string carpeta = primerElemento.CarpetaInicial;
		string nombreArchivo = primerElemento.NombreLote;
		if (!nombreArchivo.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
		{
			nombreArchivo += ".txt";
		}
		string rutaCompleta = Path.Combine(carpeta, nombreArchivo);
		if (!Directory.Exists(carpeta))
		{
			Directory.CreateDirectory(carpeta);
		}
		List<string> lineas = new List<string>();
		foreach (eCodigosBarrasEncontrados_v3 item in pListaCodigosBarrasEncontrados)
		{
			string linea = $"{item.CarpetaInicial};{item.NombreLote};{item.Despacho};{item.NumeroPagina};{item.ValorEncontrado}";
			lineas.Add(linea);
		}
		File.AppendAllLines(rutaCompleta, lineas);
	}

	public static List<eCodigosBarrasEncontrados_v3> buscarCodigoBarrasPATCH(eUsuario pUsuarioLogueado, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		List<eCodigosBarrasEncontrados_v3> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v3>();
		string carpeta = Path.GetDirectoryName(pRutaArchivoPDF);
		string nombreArchivoTxt = pNombreLote + ".txt";
		string rutaArchivoTxt = Path.Combine(carpeta, pNombreLote, nombreArchivoTxt);
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
					if (nombreLoteArchivo.Equals(pNombreLote, StringComparison.OrdinalIgnoreCase) && nombreArchivo.Equals(pNombreArchivo, StringComparison.OrdinalIgnoreCase))
					{
						int numPag;
						eCodigosBarrasEncontrados_v3 item = new eCodigosBarrasEncontrados_v3
						{
							CarpetaInicial = campos[0].Trim(),
							NombreLote = campos[1].Trim(),
							Despacho = campos[2].Trim(),
							NumeroPagina = (int.TryParse(campos[3].Trim(), out numPag) ? numPag : 0),
							ValorEncontrado = campos[4].Trim()
						};
						oListaCodigosBarrasEncontrados.Add(item);
					}
				}
			}
		}
		else
		{
			oListaCodigosBarrasEncontrados = extraerCodigoBarrasPATCH(pNombreLote, pNombreArchivo, pRutaArchivoPDF);
			guardarCodigosBarrasEncontrados(oListaCodigosBarrasEncontrados);
		}
		return oListaCodigosBarrasEncontrados;
	}

	private static List<eCodigosBarrasEncontrados_v3> extraerCodigoBarras(string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		List<eCodigosBarrasEncontrados_v3> oListaEncontado = new List<eCodigosBarrasEncontrados_v3>();
		string pdfPath = Path.Combine(pRutaArchivoPDF, pNombreArchivo);
		int paginasPorBloque = 100;
		List<string> bloques = DividirPdfEnBloques(pdfPath, paginasPorBloque);
		int paginaOffset = 0;
		foreach (string bloquePath in bloques)
		{
			try
			{
				using FileStream stream = new FileStream(bloquePath, FileMode.Open, FileAccess.Read);
				BarcodeResults results = BarcodeReader.ReadPdf(stream);
				foreach (BarcodeResult pageResult in results)
				{
					int resultado = 0;
					string Value = pageResult.Value;
					int PageNum = pageResult.PageNumber + paginaOffset;
					if (Value.Contains("PATCH") || Value == "2")
					{
						eCodigosBarrasEncontrados_v3 oCodigosBarraEncontrados = new eCodigosBarrasEncontrados_v3
						{
							CarpetaInicial = pRutaArchivoPDF,
							NombreLote = pNombreLote,
							Despacho = pNombreArchivo,
							NumeroPagina = PageNum,
							ValorEncontrado = Value
						};
						if (!oListaEncontado.Any((eCodigosBarrasEncontrados_v3 c) => c.CarpetaInicial == oCodigosBarraEncontrados.CarpetaInicial && c.NombreLote == oCodigosBarraEncontrados.NombreLote && c.Despacho == oCodigosBarraEncontrados.Despacho && c.NumeroPagina == oCodigosBarraEncontrados.NumeroPagina && c.ValorEncontrado == oCodigosBarraEncontrados.ValorEncontrado))
						{
							oListaEncontado.Add(oCodigosBarraEncontrados);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error al procesar bloque: " + bloquePath + "\n" + ex.Message);
			}
			finally
			{
				try
				{
					File.Delete(bloquePath);
				}
				catch
				{
				}
				paginaOffset += paginasPorBloque;
			}
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
		return oListaEncontado;
	}

	private static List<eCodigosBarrasEncontrados_v3> extraerCodigoBarrasPATCH(string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		List<eCodigosBarrasEncontrados_v3> oListaEncontado = new List<eCodigosBarrasEncontrados_v3>();
		string pdfPath = Path.Combine(pRutaArchivoPDF, pNombreArchivo);
		int paginasPorBloque = 100;
		List<string> bloques = DividirPdfEnBloques(pdfPath, paginasPorBloque);
		int paginaOffset = 0;
		foreach (string bloquePath in bloques)
		{
			try
			{
				using FileStream stream = new FileStream(bloquePath, FileMode.Open, FileAccess.Read);
				BarcodeResults results = BarcodeReader.ReadPdf(stream);
				foreach (BarcodeResult pageResult in results)
				{
					int resultado = 0;
					string Value = pageResult.Value;
					int PageNum = pageResult.PageNumber + paginaOffset;
					if (Value.Contains("PATCH"))
					{
						eCodigosBarrasEncontrados_v3 oCodigosBarraEncontrados = new eCodigosBarrasEncontrados_v3
						{
							CarpetaInicial = pRutaArchivoPDF,
							NombreLote = pNombreLote,
							Despacho = pNombreArchivo,
							NumeroPagina = PageNum,
							ValorEncontrado = Value
						};
						if (!oListaEncontado.Any((eCodigosBarrasEncontrados_v3 c) => c.CarpetaInicial == oCodigosBarraEncontrados.CarpetaInicial && c.NombreLote == oCodigosBarraEncontrados.NombreLote && c.Despacho == oCodigosBarraEncontrados.Despacho && c.NumeroPagina == oCodigosBarraEncontrados.NumeroPagina && c.ValorEncontrado == oCodigosBarraEncontrados.ValorEncontrado))
						{
							oListaEncontado.Add(oCodigosBarraEncontrados);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error al procesar bloque: " + bloquePath + "\n" + ex.Message);
			}
			finally
			{
				try
				{
					File.Delete(bloquePath);
				}
				catch
				{
				}
				paginaOffset += paginasPorBloque;
			}
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
		return oListaEncontado;
	}

	private static List<string> DividirPdfEnBloques(string rutaPdfOriginal, int paginasPorBloque)
	{
		List<string> rutasBloques = new List<string>();
		using (PdfSharp.Pdf.PdfDocument documentoOriginal = PdfReader.Open(rutaPdfOriginal, PdfDocumentOpenMode.Import))
		{
			int totalPaginas = documentoOriginal.PageCount;
			int numeroBloque = 1;
			for (int i = 0; i < totalPaginas; i += paginasPorBloque)
			{
				PdfSharp.Pdf.PdfDocument nuevoBloque = new PdfSharp.Pdf.PdfDocument();
				nuevoBloque.Version = documentoOriginal.Version;
				int limite = Math.Min(paginasPorBloque, totalPaginas - i);
				for (int j = 0; j < limite; j++)
				{
					nuevoBloque.AddPage(documentoOriginal.Pages[i + j]);
				}
				string nombreBase = Path.GetFileNameWithoutExtension(rutaPdfOriginal);
				string carpetaTemporal = Path.Combine(Path.GetTempPath(), "PDF_Bloques");
				Directory.CreateDirectory(carpetaTemporal);
				string rutaSalida = Path.Combine(carpetaTemporal, $"{nombreBase}_Parte{numeroBloque}.pdf");
				nuevoBloque.Save(rutaSalida);
				rutasBloques.Add(rutaSalida);
				numeroBloque++;
			}
		}
		return rutasBloques;
	}

	public static string crearCarpeta(string pRutaCarpeta, eDespacho pDespacho)
	{
		string nombreCarpeta = pDespacho.dsDespacho + " - " + pDespacho.cdSerieDocumental + " - " + pDespacho.nuSIGEA + " - " + DateTime.Now.ToString("yyyy-MM-dd");
		return dArchivos.crearCarpeta(pRutaCarpeta, nombreCarpeta);
	}

	public static void ProcesarPDF(eUsuario pUsuarioLogueado, string pRutaSalida, List<eCodigosBarrasEncontrados_v3> pListaSeparadores, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		string carpetaLote = Path.Combine(pRutaSalida, pNombreLote);
		if (!Directory.Exists(carpetaLote))
		{
			Directory.CreateDirectory(carpetaLote);
		}
		string pdfPath = Path.Combine(pRutaArchivoPDF, pNombreArchivo);
		if (!File.Exists(pdfPath))
		{
			throw new FileNotFoundException("No se encontró el archivo: " + pdfPath);
		}
		using (PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import))
		{
			int totalPaginas = inputDocument.PageCount;
			List<eCodigosBarrasEncontrados_v3> separadores = (from s in pListaSeparadores
				where s.NombreLote == pNombreLote && s.Despacho == pNombreArchivo
				orderby s.NumeroPagina
				select s).ToList();
			if (separadores.Count == 0)
			{
				throw new Exception("No se encontraron separadores para este archivo.");
			}
			int contador = 1;
			for (int i = 0; i < separadores.Count; i++)
			{
				int paginaInicio = separadores[i].NumeroPagina + 1;
				int paginaFin = ((i + 1 < separadores.Count) ? (separadores[i + 1].NumeroPagina - 1) : totalPaginas);
				int idxInicio = paginaInicio - 1;
				int idxFin = paginaFin - 1;
				if (idxInicio <= idxFin && idxInicio < totalPaginas)
				{
					PdfSharp.Pdf.PdfDocument newDocument = new PdfSharp.Pdf.PdfDocument();
					for (int j = idxInicio; j <= idxFin && j < totalPaginas; j++)
					{
						newDocument.AddPage(inputDocument.Pages[j]);
					}
					string nombreArchivo = contador.ToString("D8") + ".pdf";
					string rutaSalidaArchivo = Path.Combine(carpetaLote, nombreArchivo);
					newDocument.Save(rutaSalidaArchivo);
					contador++;
				}
			}
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	public static void AplicarOCREnCarpeta(string carpetaPDFs, string idioma = "spa")
	{
		string[] archivos = Directory.GetFiles(carpetaPDFs, "*.pdf");
		string[] array = archivos;
		foreach (string archivoPDF in array)
		{
			string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoPDF);
			string rutaTemporalOCR = Path.Combine(carpetaPDFs, nombreSinExtension + "_OCR.pdf");
			using (PdfiumViewer.PdfDocument originalPdf = PdfiumViewer.PdfDocument.Load(archivoPDF))
			{
				using PdfSharp.Pdf.PdfDocument documentoOCR = new PdfSharp.Pdf.PdfDocument();
				for (int j = 0; j < originalPdf.PageCount; j++)
				{
					using Image image = originalPdf.Render(j, 300f, 300f, forPrinting: true);
					string textoReconocido = ObtenerTextoOCR(image, idioma);
					PdfPage page = documentoOCR.AddPage();
					using XGraphics gfx = XGraphics.FromPdfPage(page);
					XImage img;
					using (MemoryStream ms = new MemoryStream())
					{
						image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						ms.Position = 0L;
						img = XImage.FromStream(ms);
					}
					gfx.DrawImage(img, 0.0, 0.0, page.Width, page.Height);
					gfx.DrawString(textoReconocido, new XFont("Arial", 8.0), XBrushes.Transparent, new XRect(10.0, 10.0, page.Width, page.Height));
				}
				documentoOCR.Save(rutaTemporalOCR);
			}
			File.Delete(archivoPDF);
			File.Move(rutaTemporalOCR, archivoPDF);
		}
	}

	public static string ObtenerTextoOCR(Image img, string idioma = "spa")
	{
		string texto = "";
		using (TesseractEngine engine = new TesseractEngine("./tessdata", idioma, EngineMode.Default))
		{
			using Pix pix = PixConverter.ToPix((Bitmap)img);
			using Page page = engine.Process(pix);
			texto = page.GetText();
		}
		return texto;
	}

	public static void UnirPDFs(List<string> archivos, string salidaFinal)
	{
		using PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
		foreach (string file in archivos)
		{
			using PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
			foreach (PdfPage page in inputDocument.Pages)
			{
				outputDocument.AddPage(page);
			}
		}
		outputDocument.Save(salidaFinal);
	}
}
