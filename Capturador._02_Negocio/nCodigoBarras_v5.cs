using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using IronBarCode;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Capturador._02_Negocio;

public class nCodigoBarras_v5
{
	public static List<eCodigosBarrasEncontrados_v2> buscarCodigoBarras(eUsuario pUsuarioLogueado, eDespacho pDespacho)
	{
		List<eCodigosBarrasEncontrados_v2> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v2>();
		return extraerCodigoBarras(pDespacho);
	}

	private static List<eCodigosBarrasEncontrados_v2> extraerCodigoBarras(eDespacho pDespacho)
	{
		List<eCodigosBarrasEncontrados_v2> oListaEncontrado = new List<eCodigosBarrasEncontrados_v2>();
		string pdfPath = pDespacho.dsRutaArchivoPDF;
		int paginasPorBloque = 100;
		using (PdfDocument documentoOriginal = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import))
		{
			int totalPaginas = documentoOriginal.PageCount;
			for (int paginaOffset = 0; paginaOffset < totalPaginas; paginaOffset += paginasPorBloque)
			{
				int limite = Math.Min(paginasPorBloque, totalPaginas - paginaOffset);
				using MemoryStream ms = new MemoryStream();
				PdfDocument bloque = new PdfDocument();
				bloque.Version = documentoOriginal.Version;
				for (int i = 0; i < limite; i++)
				{
					bloque.AddPage(documentoOriginal.Pages[paginaOffset + i]);
				}
				bloque.Save(ms);
				ms.Position = 0L;
				try
				{
					BarcodeResults results = BarcodeReader.ReadPdf(ms);
					foreach (BarcodeResult pageResult in results)
					{
						string Value = pageResult.Value;
						int PageNum = pageResult.PageNumber + paginaOffset;
						if (int.TryParse(Value, out var resultado) && resultado >= 1 && resultado <= 8)
						{
							eCodigosBarrasEncontrados_v2 encontrado = new eCodigosBarrasEncontrados_v2
							{
								CarpetaInicial = pDespacho.dsRutaArchivoPDF,
								NombreLote = pDespacho.dsNombreLote,
								Despacho = pDespacho.dsDespacho,
								NumeroPagina = PageNum,
								ValorEncontrado = resultado
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
					throw new Exception($"Error procesando bloque: {paginaOffset}-{paginaOffset + limite - 1} :: {ex.Message}");
				}
				finally
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
			}
		}
		return oListaEncontrado;
	}

	private static List<(int paginaOffset, MemoryStream bloqueStream)> DividirPdfEnBloquesEnMemoria(string rutaPdfOriginal, int paginasPorBloque)
	{
		List<(int, MemoryStream)> bloques = new List<(int, MemoryStream)>();
		using (PdfDocument documentoOriginal = PdfReader.Open(rutaPdfOriginal, PdfDocumentOpenMode.Import))
		{
			int totalPaginas = documentoOriginal.PageCount;
			for (int i = 0; i < totalPaginas; i += paginasPorBloque)
			{
				MemoryStream ms = new MemoryStream();
				PdfDocument nuevoBloque = new PdfDocument();
				nuevoBloque.Version = documentoOriginal.Version;
				int limite = Math.Min(paginasPorBloque, totalPaginas - i);
				for (int j = 0; j < limite; j++)
				{
					nuevoBloque.AddPage(documentoOriginal.Pages[i + j]);
				}
				nuevoBloque.Save(ms);
				ms.Position = 0L;
				bloques.Add((i, ms));
			}
		}
		return bloques;
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
			throw new Exception("El archivo PDF " + pRutaArchivoPDF + " no existe");
		}
		using PdfDocument inputPdf = PdfReader.Open(pRutaArchivoPDF, PdfDocumentOpenMode.Import);
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
				using (PdfDocument outputPdf = new PdfDocument())
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
		using PdfDocument outputPdf = new PdfDocument();
		foreach (string file in pdfFiles)
		{
			using PdfDocument inputPdf = PdfReader.Open(file, PdfDocumentOpenMode.Import);
			foreach (PdfPage page in inputPdf.Pages)
			{
				outputPdf.AddPage(page);
			}
		}
		outputPdf.Save(outputFilePath);
	}
}
