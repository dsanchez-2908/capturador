using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Capturador._04_Entidades;

namespace Capturador._03_Datos;

public class dLote
{
	public static List<eLote> validarSiExisteLote(eUsuario pUsuarioLogueado, eLote pLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.Int).Value = "1";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pLote.cdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = pLote.dsNombreLote;
		oCom.Parameters.Add("@p_cdEstadoNuevo", SqlDbType.VarChar).Value = 0;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			List<eLote> oListaLote = new List<eLote>();
			while (oLeer.Read())
			{
				eLote oLote = new eLote();
				oLote.cdLote = Convert.ToInt32(oLeer["cdLote"].ToString());
				oLote.dsNombreLote = oLeer["dsNombreLote"].ToString();
				oLote.feAlta = Convert.ToDateTime(oLeer["feAlta"].ToString());
				oLote.cdUsuarioAlta = Convert.ToInt32(oLeer["cdUsuarioAlta"].ToString());
				oLote.dsUsuarioAlta = oLeer["dsUsuario"].ToString();
				oLote.cdEstado = Convert.ToInt32(oLeer["cdEstado"].ToString());
				oLote.dsEstado = oLeer["dsEstado"].ToString();
				oListaLote.Add(oLote);
			}
			return oListaLote;
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static int agregarLote(eUsuario pUsuarioLogueado, eLote pLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "2";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pLote.cdProyecto;
		SqlParameter outputIdParam = new SqlParameter("@p_cdLote", SqlDbType.Int)
		{
			Direction = ParameterDirection.Output
		};
		oCom.Parameters.Add(outputIdParam);
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = pLote.dsNombreLote;
		oCom.Parameters.Add("@p_dsRutaLote", SqlDbType.VarChar).Value = pLote.dsRutaLote;
		oCom.Parameters.Add("@p_nuCantidadArchivos", SqlDbType.Int).Value = pLote.nuCantidadArchivos;
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdEstadoNuevo", SqlDbType.VarChar).Value = 0;
		try
		{
			oCom.ExecuteNonQuery();
			return (int)outputIdParam.Value;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static void agregarLoteDetalleInicial(eUsuario pUsuarioLogueado, int pCdProyecto, eLoteDetalle pLoteDetalle)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE_DETALLE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "1";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pLoteDetalle.cdLote;
		oCom.Parameters.Add("@p_dsNombreArchivo", SqlDbType.VarChar).Value = pLoteDetalle.dsNombreArchivo;
		oCom.Parameters.Add("@p_nuCantidadPaginasInicial", SqlDbType.Int).Value = pLoteDetalle.nuCantidadPaginasInicial;
		try
		{
			oCom.ExecuteNonQuery();
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static void agregarLoteDetalleFinal(eUsuario pUsuarioLogueado, int pCdProyecto, eLoteDetalle pLoteDetalle)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE_DETALLE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "4";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pLoteDetalle.cdLote;
		oCom.Parameters.Add("@p_dsNombreArchivo", SqlDbType.VarChar).Value = pLoteDetalle.dsNombreArchivo;
		oCom.Parameters.Add("@p_nuCantidadPaginasInicial", SqlDbType.Int).Value = pLoteDetalle.nuCantidadPaginasInicial;
		try
		{
			oCom.ExecuteNonQuery();
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static void agregarPreIndexacionDespacho(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE_DETALLE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "3";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pCdLote;
		try
		{
			oCom.ExecuteNonQuery();
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static List<eLote> obtenerLotesDisponibles(eUsuario pUsuarioLogueado, string pOperacion, int pCdProyecto)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.Int).Value = pOperacion;
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = "";
		oCom.Parameters.Add("@p_cdEstadoNuevo", SqlDbType.VarChar).Value = 0;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			List<eLote> oListaLote = new List<eLote>();
			while (oLeer.Read())
			{
				eLote oLote = new eLote();
				oLote.cdLote = Convert.ToInt32(oLeer["cdLote"].ToString());
				oLote.cdProyecto = Convert.ToInt32(oLeer["cdProyecto"].ToString());
				oLote.dsProyecto = oLeer["dsProyecto"].ToString();
				oLote.dsNombreLote = oLeer["dsNombreLote"].ToString();
				oLote.dsRutaLote = oLeer["dsRutaLote"].ToString();
				oLote.dsRutaLoteFinal = oLeer["dsRutaLoteFinal"].ToString();
				oLote.nuCantidadArchivos = Convert.ToInt32(oLeer["nuCantidadArchivos"].ToString());
				oLote.feAlta = Convert.ToDateTime(oLeer["feAlta"].ToString());
				oLote.cdUsuarioAlta = Convert.ToInt32(oLeer["cdUsuarioAlta"].ToString());
				oLote.dsUsuarioAlta = oLeer["dsUsuario"].ToString();
				oLote.cdEstado = Convert.ToInt32(oLeer["cdEstado"].ToString());
				oLote.dsEstado = oLeer["dsEstado"].ToString();
				oListaLote.Add(oLote);
			}
			return oListaLote;
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static int actualizarEstadoControlCalidad(eUsuario pUsuarioLogueado, eLote pLote)
	{
		int cdEstadoNuevo = 0;
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "4";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pLote.cdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pLote.cdLote;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = pLote.dsNombreLote;
		oCom.Parameters.Add("@p_dsRutaLote", SqlDbType.VarChar).Value = pLote.dsRutaLote;
		oCom.Parameters.Add("@p_nuCantidadArchivos", SqlDbType.Int).Value = pLote.nuCantidadArchivos;
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = 0;
		SqlParameter outputIdParam = new SqlParameter("@p_cdEstadoNuevo", SqlDbType.Int)
		{
			Direction = ParameterDirection.Output
		};
		oCom.Parameters.Add(outputIdParam).Value = cdEstadoNuevo;
		try
		{
			oCom.ExecuteNonQuery();
			return (int)outputIdParam.Value;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static int actualizarEstadoIndexacion(eUsuario pUsuarioLogueado, eLote pLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "6";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pLote.cdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pLote.cdLote;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = pLote.dsNombreLote;
		oCom.Parameters.Add("@p_dsRutaLote", SqlDbType.VarChar).Value = pLote.dsRutaLote;
		oCom.Parameters.Add("@p_nuCantidadArchivos", SqlDbType.Int).Value = pLote.nuCantidadArchivos;
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = 0;
		SqlParameter outputIdParam = new SqlParameter("@p_cdEstadoNuevo", SqlDbType.Int)
		{
			Direction = ParameterDirection.Output
		};
		oCom.Parameters.Add(outputIdParam).Value = 0;
		try
		{
			oCom.ExecuteNonQuery();
			return (int)outputIdParam.Value;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static int actualizarEstadoSeperador(eUsuario pUsuarioLogueado, eLote pLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "8";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pLote.cdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pLote.cdLote;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = pLote.dsNombreLote;
		oCom.Parameters.Add("@p_dsRutaLote", SqlDbType.VarChar).Value = pLote.dsRutaLote;
		oCom.Parameters.Add("@p_dsRutaLoteFinal", SqlDbType.VarChar).Value = pLote.dsRutaLoteFinal;
		oCom.Parameters.Add("@p_nuCantidadArchivos", SqlDbType.Int).Value = pLote.nuCantidadArchivos;
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = 0;
		SqlParameter outputIdParam = new SqlParameter("@p_cdEstadoNuevo", SqlDbType.Int)
		{
			Direction = ParameterDirection.Output
		};
		oCom.Parameters.Add(outputIdParam).Value = 0;
		try
		{
			oCom.ExecuteNonQuery();
			return (int)outputIdParam.Value;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static int actualizarEstadoOCR(eUsuario pUsuarioLogueado, eLote pLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "15";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pLote.cdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pLote.cdLote;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = pLote.dsNombreLote;
		oCom.Parameters.Add("@p_dsRutaLote", SqlDbType.VarChar).Value = pLote.dsRutaLote;
		oCom.Parameters.Add("@p_dsRutaLoteFinal", SqlDbType.VarChar).Value = pLote.dsRutaLoteFinal;
		oCom.Parameters.Add("@p_nuCantidadArchivos", SqlDbType.Int).Value = pLote.nuCantidadArchivos;
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = 0;
		SqlParameter outputIdParam = new SqlParameter("@p_cdEstadoNuevo", SqlDbType.Int)
		{
			Direction = ParameterDirection.Output
		};
		oCom.Parameters.Add(outputIdParam).Value = 0;
		try
		{
			oCom.ExecuteNonQuery();
			return (int)outputIdParam.Value;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static DataTable obtenerTablaLote(eUsuario pUsuarioLogueado, string pOperacion)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_LOTE", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			cmd.Parameters.AddWithValue("@p_cdOperacion", pOperacion);
			cmd.Parameters.AddWithValue("@p_cdProyecto", 0);
			cmd.Parameters.AddWithValue("@p_cdLote", 0);
			cmd.Parameters.AddWithValue("@p_dsNombreLote", "");
			cmd.Parameters.AddWithValue("@p_dsRutaLote", "");
			cmd.Parameters.AddWithValue("@p_nuCantidadArchivos", 0);
			cmd.Parameters.AddWithValue("@p_cdEstado", 0);
			cmd.Parameters.AddWithValue("@p_cdUsuario", 0);
			cmd.Parameters.AddWithValue("@p_cdEstadoNuevo", 0);
			da.SelectCommand = cmd;
			da.Fill(dt);
			return dt;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static void eliminarLote(eUsuario pUsuarioLogueado, int pCdLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "12";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pCdLote;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = "";
		oCom.Parameters.Add("@p_dsRutaLote", SqlDbType.VarChar).Value = "";
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_cdEstadoNuevo", SqlDbType.VarChar).Value = 0;
		try
		{
			oCom.ExecuteNonQuery();
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static void modificarLote(eUsuario pUsuarioLogueado, int pCdLote, int pCdEstado, int pCdUsuario, int pCdProyecto, string pDsRutaLote)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LOTE";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "13";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pCdLote;
		oCom.Parameters.Add("@p_dsNombreLote", SqlDbType.VarChar).Value = "";
		oCom.Parameters.Add("@p_dsRutaLote", SqlDbType.VarChar).Value = pDsRutaLote;
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.Int).Value = pCdEstado;
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
		oCom.Parameters.Add("@p_cdEstadoNuevo", SqlDbType.VarChar).Value = 0;
		try
		{
			oCom.ExecuteNonQuery();
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static DataTable obtenerConsultaLotes(eUsuario pUsuarioLogueado, eLoteConsultaDespacho pLoteConsultaDespacho)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_LOTE_CONSULTA", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			if (pLoteConsultaDespacho.cdLote != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdLote", pLoteConsultaDespacho.cdLote);
			}
			if (!string.IsNullOrEmpty(pLoteConsultaDespacho.dsNombreLote))
			{
				cmd.Parameters.AddWithValue("@p_dsNombreLote", pLoteConsultaDespacho.dsNombreLote);
			}
			if (pLoteConsultaDespacho.cdUsuario != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdUsuario", pLoteConsultaDespacho.cdUsuario);
			}
			if (pLoteConsultaDespacho.cdEstado != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdEstado", pLoteConsultaDespacho.cdEstado);
			}
			if (pLoteConsultaDespacho.feAltaDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feAltaDesde", pLoteConsultaDespacho.feAltaDesde);
			}
			if (pLoteConsultaDespacho.feAltaHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feAltaHasta", pLoteConsultaDespacho.feAltaHasta);
			}
			if (pLoteConsultaDespacho.feFinalizacionDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feFinalizacionDesde", pLoteConsultaDespacho.feFinalizacionDesde);
			}
			if (pLoteConsultaDespacho.feFinalizacionHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feFinalizacionHasta", pLoteConsultaDespacho.feFinalizacionHasta);
			}
			if (!string.IsNullOrEmpty(pLoteConsultaDespacho.dsDespacho))
			{
				cmd.Parameters.AddWithValue("@p_dsDespacho", pLoteConsultaDespacho.dsDespacho);
			}
			if (!string.IsNullOrEmpty(pLoteConsultaDespacho.nuGuia))
			{
				cmd.Parameters.AddWithValue("@p_nuGuia", pLoteConsultaDespacho.nuGuia);
			}
			if (!string.IsNullOrEmpty(pLoteConsultaDespacho.dsSerieDocumental))
			{
				cmd.Parameters.AddWithValue("@p_dsSerieDocumental", pLoteConsultaDespacho.dsSerieDocumental);
			}
			if (!string.IsNullOrEmpty(pLoteConsultaDespacho.dsSIGEA))
			{
				cmd.Parameters.AddWithValue("@p_dsSIGEA", pLoteConsultaDespacho.dsSIGEA);
			}
			if (pLoteConsultaDespacho.cdUsuarioDigitalizacion != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdUsuarioDigitalizacion", pLoteConsultaDespacho.cdUsuarioDigitalizacion);
			}
			if (pLoteConsultaDespacho.cdOrigen != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdOrigen", pLoteConsultaDespacho.cdOrigen);
			}
			da.SelectCommand = cmd;
			da.Fill(dt);
			return dt;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static DataTable obtenerConsultaLotesGeneral(eUsuario pUsuarioLogueado, eLoteConsultaGeneral pLoteConsultaGeneral)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_LOTE_CONSULTA_GENERAL", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			if (pLoteConsultaGeneral.cdProyecto != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdProyecto", pLoteConsultaGeneral.cdProyecto);
			}
			if (pLoteConsultaGeneral.cdLote != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdLote", pLoteConsultaGeneral.cdLote);
			}
			if (!string.IsNullOrEmpty(pLoteConsultaGeneral.dsNombreLote))
			{
				cmd.Parameters.AddWithValue("@p_dsNombreLote", pLoteConsultaGeneral.dsNombreLote);
			}
			if (pLoteConsultaGeneral.cdUsuario != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdUsuario", pLoteConsultaGeneral.cdUsuario);
			}
			if (pLoteConsultaGeneral.cdEstado != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdEstado", pLoteConsultaGeneral.cdEstado);
			}
			if (pLoteConsultaGeneral.feAltaDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feAltaDesde", pLoteConsultaGeneral.feAltaDesde);
			}
			if (pLoteConsultaGeneral.feAltaHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feAltaHasta", pLoteConsultaGeneral.feAltaHasta);
			}
			if (pLoteConsultaGeneral.feControlCalidadDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feControlCalidadDesde", pLoteConsultaGeneral.feControlCalidadDesde);
			}
			if (pLoteConsultaGeneral.feControlCalidadHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feControlCalidadHasta", pLoteConsultaGeneral.feControlCalidadHasta);
			}
			if (pLoteConsultaGeneral.feIndexacionDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feIndexacionDesde", pLoteConsultaGeneral.feIndexacionDesde);
			}
			if (pLoteConsultaGeneral.feIndexacionHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feIndexacionHasta", pLoteConsultaGeneral.feIndexacionHasta);
			}
			if (pLoteConsultaGeneral.feSeparacionDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feSeparacionDesde", pLoteConsultaGeneral.feSeparacionDesde);
			}
			if (pLoteConsultaGeneral.feSeparacionHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feSeparacionHasta", pLoteConsultaGeneral.feSeparacionHasta);
			}
			if (pLoteConsultaGeneral.feOCRDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feOCRDesde", pLoteConsultaGeneral.feOCRDesde);
			}
			if (pLoteConsultaGeneral.feOCRHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feOCRHasta", pLoteConsultaGeneral.feOCRHasta);
			}
			if (pLoteConsultaGeneral.feFinalizacionDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feFinalizacionDesde", pLoteConsultaGeneral.feFinalizacionDesde);
			}
			if (pLoteConsultaGeneral.feFinalizacionHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feFinalizacionHasta", pLoteConsultaGeneral.feFinalizacionHasta);
			}
			da.SelectCommand = cmd;
			da.Fill(dt);
			return dt;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}

	public static DataTable obtenerLote(eUsuario pUsuarioLogueado, string pOperacion, int pCdLote)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_LOTE_ObtenerLote", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			cmd.Parameters.AddWithValue("@p_operacion", pOperacion);
			cmd.Parameters.AddWithValue("@p_cdLote", pCdLote);
			da.SelectCommand = cmd;
			da.Fill(dt);
			return dt;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			oCon?.Close();
		}
	}


    public static eLote obtenerUnLote(eUsuario pUsuarioLogueado, string pOperacion, int pCdLote)
    {
        eLote oLoteEncontrado = new eLote();

        using SqlConnection oCon = ConexionSQL.ObtenerConexion();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt = new DataTable();
        try
        {
            cmd = new SqlCommand("SP_LOTE_ObtenerLote", oCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
            cmd.Parameters.AddWithValue("@p_operacion", pOperacion);
            cmd.Parameters.AddWithValue("@p_cdLote", pCdLote);
            da.SelectCommand = cmd;
            da.Fill(dt);

            // Mapear datos del DataTable al objeto eLote
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                oLoteEncontrado.cdLote = Convert.ToInt32(row["cdLote"]);
                oLoteEncontrado.cdProyecto = Convert.ToInt32(row["cdProyecto"]);
                oLoteEncontrado.dsProyecto = row["dsProyecto"].ToString();
                oLoteEncontrado.dsNombreLote = row["dsNombreLote"].ToString();
                oLoteEncontrado.dsRutaLote = row["dsRutaLote"].ToString();
                oLoteEncontrado.nuCantidadArchivos = Convert.ToInt32(row["nuCantidadArchivos"]);
                oLoteEncontrado.feAlta = Convert.ToDateTime(row["feAlta"]);
                oLoteEncontrado.cdUsuarioAlta = Convert.ToInt32(row["cdUsuarioAlta"]);
                oLoteEncontrado.dsUsuarioAlta = row["dsUsuarioAlta"].ToString();

                // Campos que pueden ser nulos - validar antes de convertir
                if (row["fePreparado"] != DBNull.Value)
                    oLoteEncontrado.fePreparado = Convert.ToDateTime(row["fePreparado"]);

                if (row["cdUsuarioPreparado"] != DBNull.Value)
                    oLoteEncontrado.cdUsuarioPreparado = Convert.ToInt32(row["cdUsuarioPreparado"]);

                if (row["feControlCalidad"] != DBNull.Value)
                    oLoteEncontrado.feControlCalidad = Convert.ToDateTime(row["feControlCalidad"]);

                if (row["cdUsuarioControlCalidad"] != DBNull.Value)
                    oLoteEncontrado.cdUsuarioControlCalidad = Convert.ToInt32(row["cdUsuarioControlCalidad"]);

                if (row["feIndexacion"] != DBNull.Value)
                    oLoteEncontrado.feIndexacion = Convert.ToDateTime(row["feIndexacion"]);

                if (row["cdUsuarioIndexacion"] != DBNull.Value)
                    oLoteEncontrado.cdUsuarioIndexacion = Convert.ToInt32(row["cdUsuarioIndexacion"]);

                if (row["feSalida"] != DBNull.Value)
                    oLoteEncontrado.feSalida = Convert.ToDateTime(row["feSalida"]);

                if (row["cdUsuarioSalida"] != DBNull.Value)
                    oLoteEncontrado.cdUsuarioSalida = Convert.ToInt32(row["cdUsuarioSalida"]);

                oLoteEncontrado.cdEstado = Convert.ToInt32(row["cdEstado"]);
                oLoteEncontrado.dsEstado = row["dsEstado"].ToString();
            }

            return oLoteEncontrado;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            oCon?.Close();
        }
    }

    public static DataTable obtenerConsultaHistoiaClinica(eUsuario pUsuarioLogueado, int pCdLote, string pNombreLote, int pCdEstadoLote, DateTime pFeAltaDesde, DateTime pFeAltaHasta, int pCdUsuarioIndexado, DateTime pFeIndexadoDesde, DateTime pFeIndexadoHasta, string pHistoriaClinica, string pDNI, string pNombreApellido, DateTime feNacimiento)
    {
        using SqlConnection oCon = ConexionSQL.ObtenerConexion();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt = new DataTable();
        try
        {
            cmd = new SqlCommand("SP_CONSULTA_HISTORIA_CLINICA", oCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
            
			if (pCdLote != 0)
            {
                cmd.Parameters.AddWithValue("@p_cdLote", pCdLote);
            }
            if (!string.IsNullOrEmpty(pNombreLote))
            {
                cmd.Parameters.AddWithValue("@p_dsNombreLote", pNombreLote);
            }            
            if (pCdEstadoLote != 0)
            {
                cmd.Parameters.AddWithValue("@p_cdEstado", pCdEstadoLote);
            }
            if (pFeAltaDesde != DateTime.MinValue)
            {
                cmd.Parameters.AddWithValue("@p_feAltaLoteDesde", pFeAltaDesde);
            }
            if (pFeAltaHasta != DateTime.MinValue)
            {
                cmd.Parameters.AddWithValue("@p_feAltaLoteHasta", pFeAltaHasta);
            }
            if (pCdUsuarioIndexado != 0)
            {
                cmd.Parameters.AddWithValue("@p_cdUsuarioIndexado", pCdUsuarioIndexado);
            }
            if (pFeIndexadoDesde != DateTime.MinValue)
            {
                cmd.Parameters.AddWithValue("@p_feIndexadoDesde", pFeIndexadoDesde);
            }
            if (pFeIndexadoHasta != DateTime.MinValue)
            {
                cmd.Parameters.AddWithValue("@p_feIndexadoHasta", pFeIndexadoHasta);
            }
            if (!string.IsNullOrEmpty(pHistoriaClinica))
            {
                cmd.Parameters.AddWithValue("@p_dsHistoriaClinica", pHistoriaClinica);
            }
            if (!string.IsNullOrEmpty(pDNI))
            {
                cmd.Parameters.AddWithValue("@p_dsDNI", pDNI);
            }
            if (!string.IsNullOrEmpty(pNombreApellido))
            {
                cmd.Parameters.AddWithValue("@p_dsNombreApellido", pNombreApellido);
            }
            if (feNacimiento != DateTime.MinValue)
            {
                cmd.Parameters.AddWithValue("@p_feNacimiento", feNacimiento);
            }


            
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            oCon?.Close();
        }
    }



}
