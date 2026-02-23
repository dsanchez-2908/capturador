using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using PdfiumViewer;
using Tesseract;
using ZXing;
using ZXing.Common;

namespace Capturador._02_Negocio;

public class nCodigoBarras_Municipalidad_v2
{
	public static List<eCodigosBarrasEncontrados_v3> buscarCodigoBarras(eUsuario pUsuarioLogueado, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		List<eCodigosBarrasEncontrados_v3> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v3>();
		return extraerCodigoBarras(pNombreLote, pRutaArchivoPDF, pNombreArchivo);
	}

	private static List<eCodigosBarrasEncontrados_v3> extraerCodigoBarras(string pNombreLote, string pRutaArchivoPDF, string pNombreArchivo)
	{
		List<eCodigosBarrasEncontrados_v3> oListaEncontrado = new List<eCodigosBarrasEncontrados_v3>();
		string pdfPath = Path.Combine(pRutaArchivoPDF, pNombreArchivo);
		if (!File.Exists(pdfPath))
		{
			throw new Exception("Advertencia: El archivo PDF no existe: " + pdfPath);
		}
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
		string tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
		if (!Directory.Exists(tessDataPath))
		{
			throw new DirectoryNotFoundException("Error: La carpeta 'tessdata' para Tesseract OCR no se encontró en: " + tessDataPath + ". Asegúrate de que los archivos .traineddata (ej. spa.traineddata, eng.traineddata) estén ahí.");
		}
		using (TesseractEngine ocrEngine = new TesseractEngine(tessDataPath, "eng+spa", EngineMode.Default))
		{
			try
			{
				using PdfDocument document = PdfDocument.Load(pdfPath);
				for (int i = 0; i < document.PageCount; i++)
				{
					bool foundOnPage = false;
					using Bitmap pageImage = (Bitmap)document.Render(i, 900f, 900f, PdfRenderFlags.None);
					using (Bitmap binarizedImage = BinarizeImage(pageImage))
					{
						int roiHeight = Math.Min(300, binarizedImage.Height);
						Rect rect = new Rect(0, 0, binarizedImage.Width, roiHeight);
						using Pix pix = PixConverter.ToPix(binarizedImage);
						using Page page = ocrEngine.Process(pix, rect);
						string text = page.GetText();
						string normalizedText = text.Replace(" ", "").ToUpperInvariant();
						if (normalizedText.Contains("PATCH2"))
						{
							eCodigosBarrasEncontrados_v3 encontrado = new eCodigosBarrasEncontrados_v3
							{
								CarpetaInicial = pRutaArchivoPDF,
								NombreLote = pNombreLote,
								Despacho = pNombreArchivo,
								NumeroPagina = i + 1,
								ValorEncontrado = "PATCH2 (OCR)"
							};
							if (!oListaEncontrado.Any((eCodigosBarrasEncontrados_v3 c) => c.CarpetaInicial == encontrado.CarpetaInicial && c.NombreLote == encontrado.NombreLote && c.Despacho == encontrado.Despacho && c.NumeroPagina == encontrado.NumeroPagina && c.ValorEncontrado == encontrado.ValorEncontrado))
							{
								oListaEncontrado.Add(encontrado);
								foundOnPage = true;
							}
						}
					}
					if (foundOnPage)
					{
						continue;
					}
					Result barcodeResult = reader.Decode(pageImage);
					if (barcodeResult != null && barcodeResult.Text.Replace(" ", "").ToUpperInvariant().Contains("PATCH2"))
					{
						eCodigosBarrasEncontrados_v3 encontrado2 = new eCodigosBarrasEncontrados_v3
						{
							CarpetaInicial = pRutaArchivoPDF,
							NombreLote = pNombreLote,
							Despacho = pNombreArchivo,
							NumeroPagina = i + 1,
							ValorEncontrado = barcodeResult.Text
						};
						if (!oListaEncontrado.Any((eCodigosBarrasEncontrados_v3 c) => c.CarpetaInicial == encontrado2.CarpetaInicial && c.NombreLote == encontrado2.NombreLote && c.Despacho == encontrado2.Despacho && c.NumeroPagina == encontrado2.NumeroPagina && c.ValorEncontrado == encontrado2.ValorEncontrado))
						{
							oListaEncontrado.Add(encontrado2);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error al extraer información del PDF " + pdfPath + ": " + ex.Message);
				throw new Exception("Error al extraer información del PDF " + pdfPath + ": " + ex.Message, ex);
			}
		}
		return oListaEncontrado;
	}

	private static Bitmap BinarizeImage(Bitmap original, int threshold = 128)
	{
		Bitmap binarized = new Bitmap(original.Width, original.Height, PixelFormat.Format1bppIndexed);
		BitmapData originalData = null;
		BitmapData binarizedData = null;
		try
		{
			originalData = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			binarizedData = binarized.LockBits(new Rectangle(0, 0, binarized.Width, binarized.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
			int originalStride = originalData.Stride;
			int binarizedStride = binarizedData.Stride;
			IntPtr originalPtr = originalData.Scan0;
			IntPtr binarizedPtr = binarizedData.Scan0;
			byte[] originalBytes = new byte[originalStride * original.Height];
			byte[] binarizedBytes = new byte[binarizedStride * binarized.Height];
			Marshal.Copy(originalPtr, originalBytes, 0, originalBytes.Length);
			for (int y = 0; y < original.Height; y++)
			{
				for (int x = 0; x < original.Width; x++)
				{
					int originalPixelIndex = y * originalStride + x * 4;
					byte blue = originalBytes[originalPixelIndex];
					byte green = originalBytes[originalPixelIndex + 1];
					byte red = originalBytes[originalPixelIndex + 2];
					byte grey = (byte)((double)(int)red * 0.299 + (double)(int)green * 0.587 + (double)(int)blue * 0.114);
					byte bit = (byte)((grey >= threshold) ? 1u : 0u);
					int binarizedByteIndex = y * binarizedStride + x / 8;
					int bitPosition = 7 - x % 8;
					if (bit == 0)
					{
						binarizedBytes[binarizedByteIndex] &= (byte)(~(1 << bitPosition));
					}
					else
					{
						binarizedBytes[binarizedByteIndex] |= (byte)(1 << bitPosition);
					}
				}
			}
			Marshal.Copy(binarizedBytes, 0, binarizedPtr, binarizedBytes.Length);
			ColorPalette palette = binarized.Palette;
			palette.Entries[0] = Color.Black;
			palette.Entries[1] = Color.White;
			binarized.Palette = palette;
		}
		finally
		{
			if (originalData != null)
			{
				original.UnlockBits(originalData);
			}
			if (binarizedData != null)
			{
				binarized.UnlockBits(binarizedData);
			}
		}
		return binarized;
	}

	public static string crearCarpeta(string pRutaCarpeta, eDespacho pDespacho)
	{
		string nombreCarpeta = pDespacho.dsDespacho + " - " + pDespacho.cdSerieDocumental + " - " + pDespacho.nuSIGEA + " - " + DateTime.Now.ToString("yyyy-MM-dd");
		return dArchivos.crearCarpeta(pRutaCarpeta, nombreCarpeta);
	}
}
