using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Capturador._02_Negocio;

public class nSeparador
{
	public static string crearCarpeta(string pRutaCarpeta, eDespacho pDespacho)
	{
		string nombreCarpeta = pDespacho.dsDespacho + " - " + pDespacho.cdSerieDocumental + " - " + pDespacho.nuSIGEA + " - " + DateTime.Now.ToString("yyyy-MM-dd");
		return dArchivos.crearCarpeta(pRutaCarpeta, nombreCarpeta);
	}

	public static void separarArchivosLote(eUsuario pUsuarioLogueado, List<eLote> pListaLotes, List<eCodigosBarrasEncontrados_v3> oListaSeparadoresEncontrados)
	{
		foreach (eLote lote in pListaLotes)
		{
			int contador = 1;
			string carpetaLoteSeparado = Path.Combine(lote.dsRutaLoteFinal, lote.dsNombreLote);
			if (!Directory.Exists(carpetaLoteSeparado))
			{
				Directory.CreateDirectory(carpetaLoteSeparado);
			}
			List<string> listaArchivos = dArchivos.BuscarArchivosPDF(lote.dsRutaLote);
			foreach (string archivoRuta in listaArchivos)
			{
				if (!File.Exists(archivoRuta))
				{
					throw new FileNotFoundException("No se encontró el archivo: " + archivoRuta);
				}
				using (PdfDocument inputDocument = PdfReader.Open(archivoRuta, PdfDocumentOpenMode.Import))
				{
					int totalPaginas = inputDocument.PageCount;
					string archivo = Path.GetFileName(archivoRuta);
					List<eCodigosBarrasEncontrados_v3> separadores = (from s in oListaSeparadoresEncontrados
						where s.NombreLote == lote.dsNombreLote && s.Despacho == archivo
						orderby s.NumeroPagina
						select s).ToList();
					if (separadores.Count == 0)
					{
						string nombreArchivoNuevo = contador.ToString("D8") + ".pdf";
						string rutaSalidaArchivo = Path.Combine(carpetaLoteSeparado, nombreArchivoNuevo);
						File.Copy(archivoRuta, rutaSalidaArchivo, overwrite: true);
						contador++;
						continue;
					}
					for (int i = 0; i < separadores.Count; i++)
					{
						int paginaInicio = separadores[i].NumeroPagina + 1;
						int paginaFin = ((i + 1 < separadores.Count) ? (separadores[i + 1].NumeroPagina - 1) : totalPaginas);
						int idxInicio = paginaInicio - 1;
						int idxFin = paginaFin - 1;
						if (idxInicio <= idxFin && idxInicio < totalPaginas)
						{
							PdfDocument newDocument = new PdfDocument();
							for (int j = idxInicio; j <= idxFin && j < totalPaginas; j++)
							{
								newDocument.AddPage(inputDocument.Pages[j]);
							}
							string nombreArchivoNuevo2 = contador.ToString("D8") + ".pdf";
							string rutaSalidaArchivo2 = Path.Combine(carpetaLoteSeparado, nombreArchivoNuevo2);
							newDocument.Save(rutaSalidaArchivo2);
							contador++;
						}
					}
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}
	}

	public static void separarArchivosLoteCatastro(eUsuario pUsuarioLogueado, List<eLote> pListaLotes, List<eCodigosBarrasEncontrados_v3> oListaSeparadoresEncontrados)
	{
		foreach (eLote lote in pListaLotes)
		{
			int contador = 1;
			string carpetaLoteSeparado = Path.Combine(lote.dsRutaLoteFinal, lote.dsNombreLote);
			if (!Directory.Exists(carpetaLoteSeparado))
			{
				Directory.CreateDirectory(carpetaLoteSeparado);
			}
			List<string> listaArchivosSeperar = dArchivos.BuscarArchivosPDFxEmpiezaCero(lote.dsRutaLote);
			List<string> listaArchivosPlanos = dArchivos.BuscarArchivosPDFxEmpiezaConscan(lote.dsRutaLote);
			foreach (string archivoRuta in listaArchivosSeperar)
			{
				if (!File.Exists(archivoRuta))
				{
					throw new FileNotFoundException("No se encontró el archivo: " + archivoRuta);
				}
				using (PdfDocument inputDocument = PdfReader.Open(archivoRuta, PdfDocumentOpenMode.Import))
				{
					int totalPaginas = inputDocument.PageCount;
					string archivo = Path.GetFileName(archivoRuta);
					string archivoSinExtension = Path.GetFileNameWithoutExtension(archivoRuta);
					List<eCodigosBarrasEncontrados_v3> separadores = (from s in oListaSeparadoresEncontrados
						where s.NombreLote == lote.dsNombreLote && s.Despacho == archivo
						orderby s.NumeroPagina
						select s).ToList();
					if (separadores.Count == 0)
					{
						string nombreArchivoNuevo = archivoSinExtension + "-" + contador.ToString("D2") + ".pdf";
						string rutaSalidaArchivo = Path.Combine(carpetaLoteSeparado, nombreArchivoNuevo);
						File.Copy(archivoRuta, rutaSalidaArchivo, overwrite: true);
						contador++;
						continue;
					}
					if (separadores[0].NumeroPagina > 1)
					{
						int paginaInicio = 1;
						int paginaFin = separadores[0].NumeroPagina - 1;
						PdfDocument newDocument = new PdfDocument();
						for (int j = paginaInicio - 1; j < paginaFin; j++)
						{
							newDocument.AddPage(inputDocument.Pages[j]);
						}
						string nombreArchivoNuevo2 = archivoSinExtension + "-" + contador.ToString("D2") + ".pdf";
						string rutaSalidaArchivo2 = Path.Combine(carpetaLoteSeparado, nombreArchivoNuevo2);
						newDocument.Save(rutaSalidaArchivo2);
						contador++;
					}
					for (int i = 0; i < separadores.Count; i++)
					{
						int paginaInicio2 = separadores[i].NumeroPagina + 1;
						int paginaFin2 = ((i + 1 < separadores.Count) ? (separadores[i + 1].NumeroPagina - 1) : totalPaginas);
						int idxInicio = paginaInicio2 - 1;
						int idxFin = paginaFin2 - 1;
						if (idxInicio <= idxFin && idxInicio < totalPaginas)
						{
							PdfDocument newDocument2 = new PdfDocument();
							for (int j2 = idxInicio; j2 <= idxFin && j2 < totalPaginas; j2++)
							{
								newDocument2.AddPage(inputDocument.Pages[j2]);
							}
							string nombreArchivoNuevo3 = archivoSinExtension + "-" + contador.ToString("D2") + ".pdf";
							string rutaSalidaArchivo3 = Path.Combine(carpetaLoteSeparado, nombreArchivoNuevo3);
							newDocument2.Save(rutaSalidaArchivo3);
							contador++;
						}
					}
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			foreach (string archivoRuta2 in listaArchivosPlanos)
			{
				string directorio = Path.GetDirectoryName(archivoRuta2);
				string nombreArchivoCarpeta = Path.GetFileName(directorio);
				string nuevoNombreArchivo = nombreArchivoCarpeta + "_" + contador.ToString("D2") + ".pdf";
				string rutaSalidaArchivo4 = Path.Combine(carpetaLoteSeparado, nuevoNombreArchivo);
				dArchivos.copiarArchivo(archivoRuta2, rutaSalidaArchivo4);
				contador++;
			}
		}
	}
}
