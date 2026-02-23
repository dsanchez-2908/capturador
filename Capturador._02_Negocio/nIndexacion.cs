using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Capturador._03_Datos;
using Capturador._04_Entidades;

namespace Capturador._02_Negocio;

public class nIndexacion
{
	public static List<string> ObtenerNombreArchivos(string pCarpeta)
	{
		if (!Directory.Exists(pCarpeta))
		{
			throw new Exception("La carpeta " + pCarpeta + " del lote no existe");
		}
		List<string> oListaNombreArchivosEncontradas = new List<string>();
		oListaNombreArchivosEncontradas = dArchivos.BuscarArchivos(pCarpeta, "pdf");
		if (oListaNombreArchivosEncontradas.Count == 0)
		{
			throw new Exception("No se encontraron archivos PDF en la " + pCarpeta);
		}
		return oListaNombreArchivosEncontradas;
	}

	public static DataTable BuscarDespacho(string pDespacho)
	{
		return dIndexacion.BuscarDespacho(pDespacho);
	}

	public static List<eDespacho> BuscarDespacho2(eUsuario pUsuarioLogueado, string pDespacho)
	{
		return dIndexacion.obtenerDespacho(pUsuarioLogueado, pDespacho);
	}

	public static List<eLista> ObtenerLista(eUsuario pUsuario, int pProyecto, int pCampo)
	{
		return dListas.ObtenerListaCampos(pUsuario, pProyecto, pCampo);
	}

	public static void agregarIndexacion(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdLote, DataTable pTablaIndexacion)
	{
		foreach (DataRow row in pTablaIndexacion.Rows)
		{
			eIDX_Despacho oIDX_Despacho = new eIDX_Despacho();
			oIDX_Despacho.cdLote = pCdLote;
			oIDX_Despacho.idACK = Convert.ToInt32(row[0].ToString());
			oIDX_Despacho.despacho = row[1].ToString();
			oIDX_Despacho.serieDocumental = row[2].ToString();
			oIDX_Despacho.SIGEA = row[3].ToString();
			oIDX_Despacho.nroGuia = row[4].ToString();
			oIDX_Despacho.cdUsuarioDigitalizacion = Convert.ToInt32(row[5].ToString());
			oIDX_Despacho.cdOrigen = Convert.ToInt32(row[6].ToString());
			dIndexacion.agregarIndexacionDespacho(pUsuarioLogueado, pCdProyecto, oIDX_Despacho);
		}
	}

	public static void agregarCatastroPlanos(eUsuario pUsuarioLogueado, eCatastroPlanos pCatastroLote)
	{
		dIndexacion.agregarCatastroPlanos(pUsuarioLogueado, pCatastroLote);
	}

	public static void validarExistePlano(eUsuario pUsuarioLogueado, eCatastroPlanos pCatastroLote)
	{
		dIndexacion.validarExistePlano(pUsuarioLogueado, pCatastroLote);
	}

	public static int obtenerProximaSecuencia(eUsuario pUsuarioLogueado)
	{
		int ultimaNuSecuencia = dIndexacion.obtenerUltimaSecuencia(pUsuarioLogueado);
		return ultimaNuSecuencia + 1;
	}

	public static DataTable buscarPlanosDisponibles(eUsuario pUsuarioLogueado)
	{
		return dIndexacion.obtenerPlanosDisponibles(pUsuarioLogueado);
	}

	public static DataTable buscarLotesPlanos(eUsuario pUsuarioLogueado, string pRutaPlanos, string pRutaBase, DataTable pTablaPlanos)
	{
		pTablaPlanos.Columns.Add("Ruta de Lote", typeof(string));
		DataTable oTablaResultado = new DataTable();
		oTablaResultado = pTablaPlanos.Clone();
		foreach (DataRow fila in pTablaPlanos.Rows)
		{
			try
			{
				string nombreCarpeta = fila[3].ToString();
				string nombreArchivoPlano = fila[4].ToString();
				if (File.Exists(Path.Combine(pRutaPlanos, nombreArchivoPlano)))
				{
					string[] directoriosEncontrados = Directory.GetDirectories(pRutaBase, nombreCarpeta, SearchOption.AllDirectories);
					if (directoriosEncontrados.Length != 0)
					{
						fila["Ruta de Lote"] = directoriosEncontrados[0];
						oTablaResultado.ImportRow(fila);
					}
					else
					{
						fila["Ruta de Lote"] = "Carpeta no encontrada";
					}
				}
			}
			catch (DirectoryNotFoundException)
			{
				throw new Exception("La ruta especificada en el TextBox no existe: " + pRutaBase);
			}
			catch (Exception ex2)
			{
				throw new Exception("Ocurrió un error inesperado: " + ex2.Message);
			}
		}
		oTablaResultado.Columns.RemoveAt(1);
		oTablaResultado.Columns.RemoveAt(1);
		return oTablaResultado;
	}

	public static void copiarPlanos(eUsuario pUsuarioLogueado, string pRutaPlano, DataTable pTablaPlanosCopiar)
	{
		foreach (DataRow fila in pTablaPlanosCopiar.Rows)
		{
			int idPlano = Convert.ToInt32(fila[0].ToString());
			string nombreArchivoPlano = fila[2].ToString();
			string carpetaLote = fila[3].ToString();
			string origen = Path.Combine(pRutaPlano, nombreArchivoPlano);
			string destino = Path.Combine(carpetaLote, nombreArchivoPlano);
			File.Copy(origen, destino, overwrite: true);
			dIndexacion.pasarPlanoCopiado(pUsuarioLogueado, idPlano);
		}
	}

	public static DataTable buscarPlanosNoEncontrados(DataTable pTablaPlanosCargados, DataTable pTablaPlanosEncontrados)
	{
		DataTable oTablaPlanosNoEncontrados = new DataTable();
		oTablaPlanosNoEncontrados = pTablaPlanosCargados.Clone();
		List<string> oListaCodigoDocumento = new List<string>();
		if (pTablaPlanosEncontrados != null && pTablaPlanosEncontrados.Rows.Count > 0)
		{
			oListaCodigoDocumento = (from row in pTablaPlanosEncontrados.AsEnumerable()
				select row.Field<string>(1)).Distinct().ToList();
		}
		foreach (DataRow fila in pTablaPlanosCargados.Rows)
		{
			string codigoDocumento = fila[3].ToString();
			string nombreArchivoPlano = fila[4].ToString();
			if (oListaCodigoDocumento.Contains(codigoDocumento) && !pTablaPlanosEncontrados.AsEnumerable().Any((DataRow row) => row.Field<string>(1) == codigoDocumento && row.Field<string>(2) == nombreArchivoPlano))
			{
				oTablaPlanosNoEncontrados.ImportRow(fila);
			}
		}
		return oTablaPlanosNoEncontrados;
	}

	public static DataTable buscarCarpetaLoteNoCargado(eUsuario pUsuarioLogueado, DataTable pTablaPlanosCargados, string pRutaBase)
	{
		DataTable oTablaLotesNoCargados = new DataTable();
		oTablaLotesNoCargados.Columns.Add("Código Documento");
		oTablaLotesNoCargados.Columns.Add("Ruta Lote");
		try
		{
			string[] carpetasEncontradas = Directory.GetDirectories(pRutaBase, "000*", SearchOption.AllDirectories);
			HashSet<string> nombresCargados = new HashSet<string>(from row in pTablaPlanosCargados.AsEnumerable()
				select row.Field<string>(3));
			string[] array = carpetasEncontradas;
			foreach (string rutaCompleta in array)
			{
				string nombreCarpeta = Path.GetFileName(rutaCompleta);
				if (!nombresCargados.Contains(nombreCarpeta) && !dIndexacion.existeCodigoDocumentoCopiado(pUsuarioLogueado, nombreCarpeta))
				{
					DataRow nuevaFila = oTablaLotesNoCargados.NewRow();
					nuevaFila["Código Documento"] = nombreCarpeta;
					nuevaFila["Ruta Lote"] = rutaCompleta;
					oTablaLotesNoCargados.Rows.Add(nuevaFila);
				}
			}
		}
		catch (DirectoryNotFoundException)
		{
		}
		catch (Exception)
		{
		}
		return oTablaLotesNoCargados;
	}
}
