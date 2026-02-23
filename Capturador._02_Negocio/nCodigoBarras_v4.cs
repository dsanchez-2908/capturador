using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Capturador._04_Entidades;
using IronBarCode;
using PdfiumViewer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Capturador._02_Negocio;

public class nCodigoBarras_v4
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
		List<eCodigosBarraEncontrados> oListaEncontado = ExtraerCodigoBarras(pdfPath);
		using PdfSharp.Pdf.PdfDocument inputPdf = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
		List<string> generatedPdfs = new List<string>();
		int i;
		for (i = 0; i < inputPdf.PageCount; i++)
		{
			int y = 0;
			if (y >= inputPdf.PageCount)
			{
				continue;
			}
			bool paginaConCodigo = false;
			eCodigosBarraEncontrados codigoEncontrado = oListaEncontado.FirstOrDefault((eCodigosBarraEncontrados x) => x.NumeroPagina == i);
			if (codigoEncontrado != null)
			{
				codigoActual = codigoEncontrado.ValorEncontrado;
				paginaConCodigo = true;
			}
			if (!paginaConCodigo)
			{
				PdfPage page = inputPdf.Pages[i];
				string outputFilePath = Path.Combine(pNombreCarpetaNueva, $"{pDespacho.dsDespacho}-{codigoActual}_{contador}.pdf");
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
		string mergedPdfPath = Path.Combine(pNombreCarpetaNueva, pDespacho.dsDespacho + "-" + pDespacho.cdSerieDocumental + "-" + pDespacho.nuSIGEA.Replace("/", "-") + ".pdf");
		UnirPDFs(generatedPdfs, mergedPdfPath);
		string txtFilePath = Path.Combine(pNombreCarpetaNueva, pDespacho.dsDespacho + "-" + pDespacho.cdSerieDocumental + "-" + pDespacho.nuSIGEA.Replace("/", "-") + ".TXT");
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

	private List<eCodigosBarraEncontrados> ExtraerCodigoBarras(string pdfPath)
	{
		List<eCodigosBarraEncontrados> oListaEncontado = new List<eCodigosBarraEncontrados>();
		BarcodeResults results = BarcodeReader.ReadPdf(pdfPath);
		foreach (BarcodeResult pageResult in results)
		{
			int resultado = 0;
			string Value = pageResult.Value;
			int PageNum = pageResult.PageNumber;
			if (int.TryParse(Value, out resultado))
			{
				int? valorEncontrado = Convert.ToInt32(Value);
				if (valorEncontrado.HasValue && valorEncontrado.Value >= 1 && valorEncontrado.Value <= 8)
				{
					eCodigosBarraEncontrados oCodigosBarraEncontrados = new eCodigosBarraEncontrados();
					oCodigosBarraEncontrados.NumeroPagina = PageNum - 1;
					oCodigosBarraEncontrados.ValorEncontrado = Convert.ToInt32(valorEncontrado);
					oListaEncontado.Add(oCodigosBarraEncontrados);
				}
			}
		}
		return oListaEncontado;
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
