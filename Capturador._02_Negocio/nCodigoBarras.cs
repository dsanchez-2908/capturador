using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Capturador._04_Entidades;
using Ghostscript.NET.Rasterizer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ZXing;

namespace Capturador._02_Negocio;

public class nCodigoBarras
{
	public static void ProcesarPDF(string pRutaCarpeta, eDespacho pDespacho, string outputFolder, eUsuario pUsuarioLogueado)
	{
		string pdfPath = Path.Combine(pRutaCarpeta, pDespacho.dsDespacho + ".pdf");
		PdfDocument inputPdf = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
		List<int> paginasAExtraer = new List<int>();
		int codigoActual = 1;
		bool primeraPagina = true;
		for (int i = 0; i < inputPdf.PageCount; i++)
		{
			string codigoBarra = DetectarCodigoDeBarras(pdfPath, i + 1);
			if (!string.IsNullOrEmpty(codigoBarra) && int.TryParse(codigoBarra, out var numeroCodigo) && numeroCodigo >= 1 && numeroCodigo <= 8)
			{
				if (paginasAExtraer.Count > 0)
				{
					GuardarPDF(inputPdf, paginasAExtraer, Path.Combine(outputFolder, pDespacho.dsDespacho + " - " + $"{codigoActual}.pdf"));
				}
				paginasAExtraer.Clear();
				codigoActual = numeroCodigo;
				i++;
			}
			else
			{
				if (primeraPagina)
				{
					codigoActual = 1;
					primeraPagina = false;
				}
				paginasAExtraer.Add(i);
			}
		}
		if (paginasAExtraer.Count > 0)
		{
			GuardarPDF(inputPdf, paginasAExtraer, Path.Combine(outputFolder, pDespacho.dsDespacho + " - " + $"{codigoActual}.pdf"));
		}
		inputPdf.Close();
	}

	private static string DetectarCodigoDeBarras(string pdfPath, int pageNumber)
	{
		using GhostscriptRasterizer rasterizer = new GhostscriptRasterizer();
		rasterizer.Open(pdfPath);
		using Bitmap img = new Bitmap(rasterizer.GetPage(150, 150, pageNumber));
		BarcodeReader barcodeReader = new BarcodeReader();
		return barcodeReader.Decode(img)?.Text;
	}

	private static void GuardarPDF(PdfDocument inputPdf, List<int> paginas, string outputPath)
	{
		if (paginas.Count == 0)
		{
			return;
		}
		PdfDocument nuevoPdf = new PdfDocument();
		foreach (int pagina in paginas)
		{
			nuevoPdf.AddPage(inputPdf.Pages[pagina]);
		}
		nuevoPdf.Save(outputPath);
		nuevoPdf.Close();
	}
}
