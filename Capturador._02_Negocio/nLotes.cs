using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Capturador._03_Datos;
using Capturador._04_Entidades;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Capturador._02_Negocio;

public class nLotes
{
	public static List<eLote> buscarLotes(eUsuario pUsuarioLogueado, int pCdProyecto, string pCarpetaBuscar)
	{
		List<string> listaArchivosEncontrados = new List<string>();
		List<eLote> listaLoteEncontrados = new List<eLote>();
		List<eLote> listaLotes = new List<eLote>();
		listaArchivosEncontrados = nArchivos.buscarArchivos(pCarpetaBuscar, "*.PDF");
		foreach (string item in listaArchivosEncontrados)
		{
			string nombreLote = dArchivos.obtenerNombreLote(item);
			string rutaLote = Path.GetDirectoryName(item);
			if (!listaLotes.Any((eLote l) => l.dsRutaLote.Equals(rutaLote, StringComparison.OrdinalIgnoreCase)))
			{
				eLote oLote = new eLote();
				oLote.cdProyecto = pCdProyecto;
				oLote.cdUsuarioAlta = pUsuarioLogueado.cdUsuario;
				oLote.dsNombreLote = nombreLote;
				oLote.dsRutaLote = rutaLote;
				oLote.cdEstado = 1;
				oLote.nuCantidadArchivos = dArchivos.obtenerCantidadArchivos(rutaLote, "*.PDF");
				listaLotes.Add(oLote);
			}
		}
		return listaLotes;
	}

	public static List<eLote> validarSiExiste(eUsuario pUsuarioLogueado, List<eLote> pListaLote)
	{
		List<eLote> oListaLotesExistente = new List<eLote>();
		foreach (eLote item in pListaLote)
		{
			oListaLotesExistente.AddRange(dLote.validarSiExisteLote(pUsuarioLogueado, item));
		}
		return oListaLotesExistente;
	}

	public static void ingresarLote(eUsuario pUsuarioLogueado, List<eLote> pListaLote)
	{
		int cdLote = 0;
		foreach (eLote item in pListaLote)
		{
			cdLote = dLote.agregarLote(pUsuarioLogueado, item);
			ingresarLoteDetalle(pUsuarioLogueado, cdLote, item);
			if (item.cdProyecto == 1)
			{
                dLote.agregarPreIndexacionDespacho(pUsuarioLogueado, 1, cdLote);
            }
            if (item.cdProyecto == 9)
            {
                dLote.agregarPreIndexacionDespacho(pUsuarioLogueado, 9, cdLote);
            }
        }
	}

	private static void ingresarLoteDetalle(eUsuario pUsuarioLogueado, int pCdLote, eLote pLote)
	{
		List<string> listaArchivos = dArchivos.BuscarArchivosIndices(pLote.dsRutaLote, "*.PDF");
		foreach (string item in listaArchivos)
		{
			eLoteDetalle oLoteDetalle = new eLoteDetalle();
			oLoteDetalle.cdLote = pCdLote;
			oLoteDetalle.dsNombreArchivo = Path.GetFileName(item);
			oLoteDetalle.nuCantidadPaginasInicial = obtenerCantidadPaginas(pLote.dsRutaLote, item);
			oLoteDetalle.cdEstado = 1;
			dLote.agregarLoteDetalleInicial(pUsuarioLogueado, pLote.cdProyecto, oLoteDetalle);
		}
	}

	private static void ingresarPreIndexacionDespacho(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdLote)
	{
		dLote.agregarPreIndexacionDespacho(pUsuarioLogueado, pCdProyecto, pCdLote);
	}

	private static int obtenerCantidadPaginas(string pRutaLote, string pArchivo)
	{
		string archivo = Path.Combine(pRutaLote, pArchivo);
		if (!File.Exists(archivo))
		{
			throw new FileNotFoundException("El archivo PDF no fue encontrado.", archivo);
		}
		using PdfDocument document = PdfReader.Open(archivo, PdfDocumentOpenMode.ReadOnly);
		int totalPaginas = document.PageCount;
		document.Dispose();
		return totalPaginas;
	}

	public static List<eLote> obtenerLotesDisponibleControlCalidad(eUsuario pUsuarioLogueado, int pCdProyecto)
	{
		return dLote.obtenerLotesDisponibles(pUsuarioLogueado, "3", pCdProyecto);
	}

	public static void finalizarControlCalidad(eUsuario pUsuarioLogueado, eLote pLote)
	{
		int cdEstadoNuevo = 0;
		cdEstadoNuevo = dLote.actualizarEstadoControlCalidad(pUsuarioLogueado, pLote);
		if (cdEstadoNuevo != 4)
		{
			return;
		}
		List<string> listaArchivos = dArchivos.BuscarArchivosIndices(pLote.dsRutaLote, "*.PDF");
		foreach (string item in listaArchivos)
		{
			eLoteDetalle oLoteDetalle = new eLoteDetalle();
			oLoteDetalle.cdLote = pLote.cdLote;
			oLoteDetalle.dsNombreArchivo = Path.GetFileName(item);
			oLoteDetalle.nuCantidadPaginasInicial = obtenerCantidadPaginas(pLote.dsRutaLote, item);
			oLoteDetalle.cdEstado = 1;
			dLote.agregarLoteDetalleFinal(pUsuarioLogueado, pLote.cdProyecto, oLoteDetalle);
		}
	}

	public static List<eLote> obtenerLotesDisponibleIndexacion(eUsuario pUsuarioLogueado, int pCdProyecto)
	{
		return dLote.obtenerLotesDisponibles(pUsuarioLogueado, "5", pCdProyecto);
	}

	public static void finalizarIndexacion(eUsuario pUsuarioLogueado, eLote pLote)
	{
		int cdEstadoNuevo = 0;
		cdEstadoNuevo = dLote.actualizarEstadoIndexacion(pUsuarioLogueado, pLote);
		if (cdEstadoNuevo != 4)
		{
			return;
		}
		List<string> listaArchivos = dArchivos.BuscarArchivosIndices(pLote.dsRutaLote, "*.PDF");
		foreach (string item in listaArchivos)
		{
			eLoteDetalle oLoteDetalle = new eLoteDetalle();
			oLoteDetalle.cdLote = pLote.cdLote;
			oLoteDetalle.dsNombreArchivo = Path.GetFileName(item);
			oLoteDetalle.nuCantidadPaginasInicial = obtenerCantidadPaginas(pLote.dsRutaLote, item);
			oLoteDetalle.cdEstado = 1;
			dLote.agregarLoteDetalleFinal(pUsuarioLogueado, pLote.cdProyecto, oLoteDetalle);
		}
	}

	public static List<eLote> obtenerLotesDisponibleSeparacion(eUsuario pUsuarioLogueado, int pCdProyecto)
	{
		return dLote.obtenerLotesDisponibles(pUsuarioLogueado, "7", pCdProyecto);
	}

	public static void finalizarSeparacion(eUsuario pUsuarioLogueado, eLote pLote)
	{
		int cdEstadoNuevo = 0;
		cdEstadoNuevo = dLote.actualizarEstadoSeperador(pUsuarioLogueado, pLote);
	}

	public static void separarArchivosLote(eUsuario pUsuarioLogueado, List<eLote> pListaLotes, List<eCodigosBarrasEncontrados_v3> oListaSeparadoresEncontrados)
	{
		nSeparador.separarArchivosLote(pUsuarioLogueado, pListaLotes, oListaSeparadoresEncontrados);
		finalizarSeparacion(pUsuarioLogueado, pListaLotes);
	}

	public static void separarArchivosLoteCatastro(eUsuario pUsuarioLogueado, List<eLote> pListaLotes, List<eCodigosBarrasEncontrados_v3> oListaSeparadoresEncontrados)
	{
		nSeparador.separarArchivosLoteCatastro(pUsuarioLogueado, pListaLotes, oListaSeparadoresEncontrados);
		finalizarSeparacion(pUsuarioLogueado, pListaLotes);
	}

	private static void finalizarSeparacion(eUsuario pUsuarioLogueado, List<eLote> pListaLotes)
	{
		foreach (eLote lote in pListaLotes)
		{
			int cdEstadoNuevo = 0;
			cdEstadoNuevo = dLote.actualizarEstadoSeperador(pUsuarioLogueado, lote);
			if (cdEstadoNuevo != 4)
			{
				continue;
			}
			string rutaFinalLote = Path.Combine(lote.dsRutaLoteFinal, lote.dsNombreLote);
			List<string> listaArchivos = dArchivos.BuscarArchivosPDF(rutaFinalLote);
			foreach (string archivo in listaArchivos)
			{
				eLoteDetalle oLoteDetalle = new eLoteDetalle();
				oLoteDetalle.cdLote = lote.cdLote;
				oLoteDetalle.dsNombreArchivo = Path.GetFileName(archivo);
				oLoteDetalle.nuCantidadPaginasInicial = obtenerCantidadPaginas(rutaFinalLote, Path.GetFileName(archivo));
				oLoteDetalle.cdEstado = 1;
				dLote.agregarLoteDetalleFinal(pUsuarioLogueado, lote.cdProyecto, oLoteDetalle);
			}
		}
	}

	public static DataTable obtenerTablaLotesEstado(eUsuario pUsuarioLogueado)
	{
		return dLote.obtenerTablaLote(pUsuarioLogueado, "9");
	}

	public static DataTable obtenerTablaLotesUsuario(eUsuario pUsuarioLogueado)
	{
		return dLote.obtenerTablaLote(pUsuarioLogueado, "10");
	}

	public static DataTable obtenerTablaLotesDetalle(eUsuario pUsuarioLogueado)
	{
		return dLote.obtenerTablaLote(pUsuarioLogueado, "11");
	}

	public static void eliminarLote(eUsuario pUsuarioLogueado, int pCdLote)
	{
		dLote.eliminarLote(pUsuarioLogueado, pCdLote);
	}

	public static void modificarLote(eUsuario pUsuarioLogueado, int pCdLote, int pCdEstado, int pCdUsuario, int pCdProyecto, string pDsRutaLote)
	{
		dLote.modificarLote(pUsuarioLogueado, pCdLote, pCdEstado, pCdUsuario, pCdProyecto, pDsRutaLote);
	}

	public static DataTable obtenerLoteConsultaDespachos(eUsuario pUsuarioLogueado, eLoteConsultaDespacho pLoteConsultaDespacho)
	{
		return dLote.obtenerConsultaLotes(pUsuarioLogueado, pLoteConsultaDespacho);
	}

	public static DataTable obtenerLoteConsultaGeneral(eUsuario pUsuarioLogueado, eLoteConsultaGeneral pLoteConsultaGeneral)
	{
		return dLote.obtenerConsultaLotesGeneral(pUsuarioLogueado, pLoteConsultaGeneral);
	}

	public static DataTable obtenerLote(eUsuario pUsuarioLogueado, int pCdLote)
	{
		return dLote.obtenerLote(pUsuarioLogueado, "1", pCdLote);
	}

    public static eLote obtenerUnLote(eUsuario pUsuarioLogueado, int pCdLote)
    {
        return dLote.obtenerUnLote(pUsuarioLogueado, "1", pCdLote);
    }

    public static DataTable obtenerLoteDetalle(eUsuario pUsuarioLogueado, int pCdLote)
	{
		return dLote.obtenerLote(pUsuarioLogueado, "2", pCdLote);
	}

	public static List<eLote> obtenerLotesDisponibleOCR(eUsuario pUsuarioLogueado, int pCdProyecto)
	{
		return dLote.obtenerLotesDisponibles(pUsuarioLogueado, "14", pCdProyecto);
	}

	public static void finalizarOCR(eUsuario pUsuarioLogueado, eLote pLote)
	{
		int cdEstadoNuevo = 0;
		cdEstadoNuevo = dLote.actualizarEstadoOCR(pUsuarioLogueado, pLote);
		if (cdEstadoNuevo != 4)
		{
			return;
		}
		List<string> listaArchivos = dArchivos.BuscarArchivosIndices(pLote.dsRutaLoteFinal, "*.PDF");
		foreach (string item in listaArchivos)
		{
			eLoteDetalle oLoteDetalle = new eLoteDetalle();
			oLoteDetalle.cdLote = pLote.cdLote;
			oLoteDetalle.dsNombreArchivo = Path.GetFileName(item);
			oLoteDetalle.nuCantidadPaginasInicial = obtenerCantidadPaginas(pLote.dsRutaLoteFinal, item);
			oLoteDetalle.cdEstado = 1;
			dLote.agregarLoteDetalleFinal(pUsuarioLogueado, pLote.cdProyecto, oLoteDetalle);
		}
	}
}
