using System.Collections.Generic;
using System.Data;
using System.IO;
using Capturador._03_Datos;

namespace Capturador._02_Negocio;

public class nArchivos
{
	public static void GenerarArchivoIndice(string pRutaArchivo, string pNombreArchivo, DataTable pTabla)
	{
		dExportacion.GenerarArchivoCSV(pRutaArchivo, pNombreArchivo, pTabla);
	}

	public static DataTable cargarArchivoIndice(string pRutaCarpeta, string pNombreArchivo)
	{
		string archivo = Path.Combine(pRutaCarpeta, pNombreArchivo);
		return dArchivos.CargarArchivoIndice(archivo);
	}

	public static bool existeArchivoIndice(string pRutaCarpeta, string pNombreArchivoIndice)
	{
		string archivo = Path.Combine(pRutaCarpeta, pNombreArchivoIndice);
		return dArchivos.existeArchivo(archivo);
	}

	public static List<string> buscarArchivos(string pRutaCarpetaBuscar, string pNombreArchivoIndice)
	{
		return buscarArchivosIndices(pRutaCarpetaBuscar, pNombreArchivoIndice);
	}

	public static DataTable cargarArchivoIndice2(string pRutaCarpetaBuscar, string pNombreArchivoIndice)
	{
		List<string> listaIndicesEncontrados = buscarArchivosIndices(pRutaCarpetaBuscar, pNombreArchivoIndice);
		DataTable oTablaIndice = new DataTable();
		oTablaIndice.Columns.Add("id", typeof(string));
		oTablaIndice.Columns.Add("Despacho", typeof(string));
		oTablaIndice.Columns.Add("Serie Documental", typeof(string));
		oTablaIndice.Columns.Add("SIGEA", typeof(string));
		oTablaIndice.Columns.Add("Guia", typeof(string));
		oTablaIndice.Columns.Add("Usuario Digitalización", typeof(string));
		oTablaIndice.Columns.Add("Nombre Lote", typeof(string));
		oTablaIndice.Columns.Add("Ruta Archivo", typeof(string));
		foreach (string item in listaIndicesEncontrados)
		{
			DataTable tablaTemporal = dArchivos.CargarArchivoIndice(item);
			foreach (DataRow fila in tablaTemporal.Rows)
			{
				oTablaIndice.ImportRow(fila);
			}
		}
		DataView vistaOrdenada = oTablaIndice.DefaultView;
		vistaOrdenada.Sort = "[Nombre Lote] ASC, [Despacho] ASC";
		return vistaOrdenada.ToTable();
	}

	public static DataTable buscarArchivosPDF(string pNombreLote, string pRutaCarpetaBuscar)
	{
		List<string> listaArchivosEncontrados = dArchivos.BuscarArchivos(pRutaCarpetaBuscar, "PDF");
		DataTable oTablaLotesArchivos = new DataTable();
		oTablaLotesArchivos.Columns.Add("Nombre Lote", typeof(string));
		oTablaLotesArchivos.Columns.Add("Nombre Archivo", typeof(string));
		oTablaLotesArchivos.Columns.Add("Ruta Archivo", typeof(string));
		foreach (string item in listaArchivosEncontrados)
		{
			oTablaLotesArchivos.Rows.Add(pNombreLote, item, pRutaCarpetaBuscar);
		}
		DataView vistaOrdenada = oTablaLotesArchivos.DefaultView;
		vistaOrdenada.Sort = "[Nombre Lote] ASC, [Nombre Archivo] ASC";
		return vistaOrdenada.ToTable();
	}

	private static List<string> buscarArchivosIndices(string pRutaCarpetaBuscar, string pNombreArchivoIndice)
	{
		return dArchivos.BuscarArchivosIndices(pRutaCarpetaBuscar, pNombreArchivoIndice);
	}

	private static List<string> buscarArchivosPDF(string pRutaCarpetaBuscar)
	{
		return dArchivos.BuscarArchivos(pRutaCarpetaBuscar, "PDF");
	}

	public static void modificarNombreArchivo(string pRutaArchivo, string pNombreAnterior, string pNombreNuevo)
	{
		dArchivos.modificarNombreArchivo(pRutaArchivo, pNombreAnterior, pNombreNuevo);
	}
}
