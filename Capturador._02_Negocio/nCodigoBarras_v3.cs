using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Capturador._04_Entidades;
using IronBarCode;
using PdfiumViewer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Capturador._02_Negocio;

public class nCodigoBarras_v3
{
	public void ProcesarPDF(string pRutaCarpetaOrigen, eDespacho pDespacho, string pNombreCarpetaNueva, eUsuario pUsuarioLogueado)
	{
		string pdfPath = Path.Combine(pRutaCarpetaOrigen, pDespacho.dsDespacho + ".pdf");
		int codigoActual = 1;
		List<string> oListaLineasIndice = new List<string>();
		int contador = 1;
		if (!File.Exists(pdfPath))
		{
			throw new FileNotFoundException("El archivo PDF no existe", pdfPath);
		}
		using PdfSharp.Pdf.PdfDocument inputPdf = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
		List<string> generatedPdfs = new List<string>();
		for (int i = 0; i < inputPdf.PageCount; i++)
		{
			int? barcodeValue = ExtraerCodigoBarras(pdfPath, i);
			if (barcodeValue.HasValue && barcodeValue.Value >= 1 && barcodeValue.Value <= 8)
			{
				codigoActual = barcodeValue.Value;
				continue;
			}
			PdfPage page = inputPdf.Pages[i];
			string outputFilePath = Path.Combine(pNombreCarpetaNueva, $"{pDespacho.dsDespacho}-{codigoActual}_{contador}.pdf");
			using (PdfSharp.Pdf.PdfDocument outputPdf = new PdfSharp.Pdf.PdfDocument())
			{
				outputPdf.AddPage(page);
				outputPdf.Save(outputFilePath);
				generatedPdfs.Add(outputFilePath);
			}
			string lineaIndice = $"{pDespacho.nuGuia}|{pDespacho.dsDespacho}|{codigoActual}|{inputPdf.PageCount}|{DateTime.Now:yyyy.MM.dd HH:mm:ss}|{pDespacho.dsUsuarioDigitalizacion}|{DateTime.Now:yyyy.MM.dd HH:mm:ss}|1|{pUsuarioLogueado.dsUsuario}|{contador}|{pDespacho.cdSerieDocumental}|{pDespacho.nuSIGEA}";
			oListaLineasIndice.Add(lineaIndice);
			contador++;
		}
		string mergedPdfPath = Path.Combine(pNombreCarpetaNueva, pDespacho.dsDespacho + "-" + pDespacho.cdSerieDocumental + "-" + pDespacho.nuSIGEA + ".pdf");
		UnirPDFs(generatedPdfs, mergedPdfPath);
		string txtFilePath = Path.Combine(pNombreCarpetaNueva, pDespacho.dsDespacho + "-" + pDespacho.cdSerieDocumental + "-" + pDespacho.nuSIGEA + ".TXT");
		File.WriteAllLines(txtFilePath, oListaLineasIndice);
	}

	private bool EsPaginaBlanca(string pdfPath, int pageIndex)
	{
		using PdfiumViewer.PdfDocument pdfDocument = PdfiumViewer.PdfDocument.Load(pdfPath);
		using Image image = pdfDocument.Render(pageIndex, 72f, 72f, forPrinting: true);
		using MemoryStream stream = new MemoryStream();
		image.Save(stream, ImageFormat.Png);
		return stream.Length < 256000;
	}

	private int? ExtraerCodigoBarras(string pdfPath, int pageIndex)
	{
		BarcodeResults results = BarcodeReader.ReadPdf(pdfPath);
		foreach (BarcodeResult pageResult in results)
		{
			string Value = pageResult.Value;
			int PageNum = pageResult.PageNumber;
		}
		return null;
	}

	private void UnirPDFs(List<string> pdfFiles, string outputFilePath)
	{
		using PdfSharp.Pdf.PdfDocument outputPdf = new PdfSharp.Pdf.PdfDocument();
		foreach (string file in pdfFiles)
		{
			using PdfSharp.Pdf.PdfDocument inputPdf = PdfReader.Open(file, PdfDocumentOpenMode.Import);
			foreach (PdfPage page in inputPdf.Pages)
			{
				outputPdf.AddPage(page);
			}
		}
		outputPdf.Save(outputFilePath);
	}
}
