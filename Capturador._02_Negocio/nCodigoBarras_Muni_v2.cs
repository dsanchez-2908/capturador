using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capturador._04_Entidades;
using IronBarCode;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Capturador._02_Negocio;

internal class nCodigoBarras_Muni_v2
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

	public static List<eCodigosBarrasEncontrados_v3> buscarCodigoBarras(eUsuario pUsuarioLogueado, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		return buscarCodigoBarrasAsync(pUsuarioLogueado, pNombreLote, pNombreArchivo, pRutaArchivoPDF).GetAwaiter().GetResult();
	}

	public static async Task<List<eCodigosBarrasEncontrados_v3>> buscarCodigoBarrasAsync(eUsuario pUsuarioLogueado, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		List<eCodigosBarrasEncontrados_v3> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v3>();
		string rutaArchivoTxt = Path.Combine(Path.GetDirectoryName(pRutaArchivoPDF), path3: pNombreLote + ".txt", path2: pNombreLote);
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
				if (oListaCodigosBarrasEncontrados.Count > 0)
				{
					return oListaCodigosBarrasEncontrados;
				}
			}
			oListaCodigosBarrasEncontrados = await Task.Run(() => extraerCodigoBarras(pNombreLote, pNombreArchivo, pRutaArchivoPDF));
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

	private static async Task guardarCodigosBarrasEncontradosAsync(List<eCodigosBarrasEncontrados_v3> pListaCodigosBarrasEncontrados, string rutaArchivoTxt)
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
		foreach (eCodigosBarrasEncontrados_v3 item in pListaCodigosBarrasEncontrados)
		{
			string linea = $"{item.CarpetaInicial};{item.NombreLote};{item.Despacho};{item.NumeroPagina};{item.ValorEncontrado}";
			lineas.Add(linea);
		}
		await Task.Run(delegate
		{
			File.AppendAllLines(rutaArchivoTxt, lineas);
		});
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

	private static List<string> DividirPdfEnBloques(string rutaPdfOriginal, int paginasPorBloque)
	{
		List<string> rutasBloques = new List<string>();
		using (PdfDocument documentoOriginal = PdfReader.Open(rutaPdfOriginal, PdfDocumentOpenMode.Import))
		{
			int totalPaginas = documentoOriginal.PageCount;
			int numeroBloque = 1;
			for (int i = 0; i < totalPaginas; i += paginasPorBloque)
			{
				PdfDocument nuevoBloque = new PdfDocument();
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

	public static List<eCodigosBarrasEncontrados_v3> buscarCodigoBarrasPATCH(eUsuario pUsuarioLogueado, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		return buscarCodigoBarrasPATCHAsync(pUsuarioLogueado, pNombreLote, pNombreArchivo, pRutaArchivoPDF).GetAwaiter().GetResult();
	}

	public static async Task<List<eCodigosBarrasEncontrados_v3>> buscarCodigoBarrasPATCHAsync(eUsuario pUsuarioLogueado, string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF)
	{
		List<eCodigosBarrasEncontrados_v3> oListaCodigosBarrasEncontrados = new List<eCodigosBarrasEncontrados_v3>();
		string rutaArchivoTxt = Path.Combine(Path.GetDirectoryName(pRutaArchivoPDF), path3: pNombreLote + ".txt", path2: pNombreLote);
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
				if (oListaCodigosBarrasEncontrados.Count > 0)
				{
					return oListaCodigosBarrasEncontrados;
				}
			}
			oListaCodigosBarrasEncontrados = await Task.Run(() => extraerCodigoBarrasPATCH(pNombreLote, pNombreArchivo, pRutaArchivoPDF));
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
}
