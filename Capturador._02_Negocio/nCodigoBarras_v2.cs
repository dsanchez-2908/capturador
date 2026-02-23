using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using Ghostscript.NET.Rasterizer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ZXing;
using ZXing.Common;

namespace Capturador._02_Negocio;

public class nCodigoBarras_v2
{
	public static void ProcesarPDF(string pRutaCarpeta, eDespacho pDespacho, string outputFolder, eUsuario pUsuarioLogueado)
	{
		string pdfPath = Path.Combine(pRutaCarpeta, pDespacho.dsDespacho + ".pdf");
		Directory.CreateDirectory(outputFolder);
		ConcurrentBag<(int PageIndex, string OutputPath)> paginasProcesadas = new ConcurrentBag<(int, string)>();
		ConcurrentBag<(int PageIndex, string Linea)> lineasIndice = new ConcurrentBag<(int, string)>();
		PdfDocument inputPdf = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
		try
		{
			int codigoActual = 1;
			object lockObj = new object();
			Parallel.For(0, inputPdf.PageCount, delegate(int i)
			{
				using Bitmap bitmap = ConvertirPaginaAImagen(pdfPath, i + 1);
				string text = DetectarCodigoDeBarras(bitmap);
				bitmap.Dispose();
				if (!string.IsNullOrEmpty(text) && int.TryParse(text, out var result) && result >= 1 && result <= 8)
				{
					lock (lockObj)
					{
						codigoActual = result;
						return;
					}
				}
				string path = $"{pDespacho.dsDespacho}-{codigoActual}_{i + 1}.pdf";
				string item = Path.Combine(outputFolder, path);
				paginasProcesadas.Add((i, item));
				string item2 = $"{pDespacho.nuGuia}|{pDespacho.dsDespacho}|{codigoActual}|{inputPdf.PageCount}|{DateTime.Now:yyyy.MM.dd HH:mm:ss}|{pDespacho.dsUsuarioDigitalizacion}|{DateTime.Now:yyyy.MM.dd HH:mm:ss}|1|{pUsuarioLogueado.dsUsuario}|{i + 1}|{pDespacho.cdSerieDocumental}|";
				lineasIndice.Add((i, item2));
			});
		}
		finally
		{
			if (inputPdf != null)
			{
				((IDisposable)inputPdf).Dispose();
			}
		}
		using (PdfDocument inputPdf2 = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import))
		{
			foreach (var (pageIndex, outputPath) in paginasProcesadas.OrderBy(((int PageIndex, string OutputPath) p) => p.PageIndex))
			{
				using PdfDocument outputPdf = new PdfDocument();
				outputPdf.AddPage(inputPdf2.Pages[pageIndex]);
				outputPdf.Save(outputPath);
			}
		}
		string nombreArchivo = pDespacho.dsDespacho + "-" + pDespacho.cdSerieDocumental + "-" + pDespacho.nuSIGEA + ".TXT";
		IEnumerable<string> lineasOrdenadas = from l in lineasIndice
			orderby l.PageIndex
			select l.Linea;
		dArchivos.agregarLineaIndice(outputFolder, nombreArchivo, string.Join("\n", lineasOrdenadas));
		UnificarPDFs(paginasProcesadas.Select(((int PageIndex, string OutputPath) p) => p.OutputPath).ToList(), Path.Combine(outputFolder, pDespacho.dsDespacho + "-000-.pdf"));
	}

	private static Bitmap ConvertirPaginaAImagen(string pdfPath, int pageNumber)
	{
		using GhostscriptRasterizer rasterizer = new GhostscriptRasterizer();
		rasterizer.Open(pdfPath);
		return new Bitmap(rasterizer.GetPage(100, 100, pageNumber));
	}

	private static string DetectarCodigoDeBarras(Bitmap img)
	{
		try
		{
			using Bitmap imgRedimensionada = new Bitmap(img, new Size(img.Width * 2, img.Height * 2));
			using Bitmap imgRGB = ConvertirABitmapRGB(imgRedimensionada);
			BarcodeReader barcodeReader = new BarcodeReader
			{
				AutoRotate = true,
				Options = new DecodingOptions
				{
					PossibleFormats = new List<BarcodeFormat>
					{
						BarcodeFormat.CODE_128,
						BarcodeFormat.CODE_39,
						BarcodeFormat.EAN_13,
						BarcodeFormat.EAN_8,
						BarcodeFormat.UPC_A,
						BarcodeFormat.UPC_E
					},
					TryHarder = true
				}
			};
			return barcodeReader.Decode(imgRGB)?.Text;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error detectando código de barras: " + ex.Message);
			return null;
		}
	}

	private static void UnificarPDFs(List<string> archivos, string finalPath)
	{
		using PdfDocument finalPdf = new PdfDocument();
		foreach (string pdfFile in archivos.OrderBy((string f) => f))
		{
			using PdfDocument tempPdf = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import);
			foreach (PdfPage page in tempPdf.Pages)
			{
				finalPdf.AddPage(page);
			}
		}
		finalPdf.Save(finalPath);
	}

	private static Bitmap ConvertirABitmapRGB(Bitmap img)
	{
		Bitmap imgRGB = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
		using (Graphics g = Graphics.FromImage(imgRGB))
		{
			g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
		}
		return imgRGB;
	}
}
