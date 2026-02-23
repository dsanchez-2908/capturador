using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Capturador._04_Entidades;

namespace Capturador._03_Datos;

public class dIndexacion
{
	public static DataTable BuscarDespacho(string pDespacho)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_Despachos_Buscar", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_dsDespacho", pDespacho);
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

	public static List<eDespacho> obtenerDespacho(eUsuario pUsuarioLogueado, string pDespacho)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Despachos_Buscar";
		oCom.Parameters.Add("@p_dsDespacho", SqlDbType.VarChar).Value = pDespacho;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			List<eDespacho> oListaDespachos = new List<eDespacho>();
			while (oLeer.Read())
			{
				eDespacho oDespacho = new eDespacho();
				oDespacho.id = Convert.ToInt32(oLeer["id"].ToString());
				oDespacho.dsDespacho = oLeer["dsDespacho"].ToString();
				oDespacho.cdSerieDocumental = oLeer["cdSerieDocumental"].ToString();
				oDespacho.nuSIGEA = oLeer["nuSIGEA"].ToString();
				oDespacho.nuGuia = oLeer["nuGuia"].ToString();
				oListaDespachos.Add(oDespacho);
			}
			return oListaDespachos;
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

	public static void agregarIndexacionDespacho(eUsuario pUsuarioLogueado, int pCdProyecto, eIDX_Despacho pIDX_Despacho)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_IDX_DESPACHO";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "1";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
		oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pIDX_Despacho.cdLote;
		oCom.Parameters.Add("@p_idACK", SqlDbType.Int).Value = pIDX_Despacho.idACK;
		oCom.Parameters.Add("@p_despacho", SqlDbType.VarChar).Value = pIDX_Despacho.despacho;
		oCom.Parameters.Add("@p_serieDocumental", SqlDbType.VarChar).Value = pIDX_Despacho.serieDocumental;
		oCom.Parameters.Add("@p_SIGEA", SqlDbType.VarChar).Value = pIDX_Despacho.SIGEA;
		oCom.Parameters.Add("@p_nroGuia", SqlDbType.VarChar).Value = pIDX_Despacho.nroGuia;
		oCom.Parameters.Add("@p_cdUsuarioDigitalizacion", SqlDbType.Int).Value = pIDX_Despacho.cdUsuarioDigitalizacion;
		oCom.Parameters.Add("@p_cdOrigen", SqlDbType.Int).Value = pIDX_Despacho.cdOrigen;
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

	public static void agregarCatastroPlanos(eUsuario pUsuarioLogueado, eCatastroPlanos pCatastroPlanos)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_CATASTRO_PLANOS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "1";
		oCom.Parameters.Add("@p_cdDocumento", SqlDbType.VarChar).Value = pCatastroPlanos.cdDocumento;
		oCom.Parameters.Add("@p_nuSecuencia", SqlDbType.Int).Value = pCatastroPlanos.nuSecuencia;
		oCom.Parameters.Add("@p_dsNombreArchivo", SqlDbType.VarChar).Value = pCatastroPlanos.dsNombreArchivo;
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

	public static int obtenerUltimaSecuencia(eUsuario pUsuarioLogueado)
	{
		int nuUltimaSecuencia = 0;
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_CATASTRO_PLANOS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "2";
		SqlParameter outputIdParam = new SqlParameter("@p_nuSecuencia", SqlDbType.Int)
		{
			Direction = ParameterDirection.Output
		};
		oCom.Parameters.Add(outputIdParam);
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

	public static void validarExistePlano(eUsuario pUsuarioLogueado, eCatastroPlanos pCatastroPlanos)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_CATASTRO_PLANOS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "3";
		oCom.Parameters.Add("@p_cdDocumento", SqlDbType.VarChar).Value = pCatastroPlanos.cdDocumento;
		oCom.Parameters.Add("@p_nuSecuencia", SqlDbType.Int).Value = pCatastroPlanos.nuSecuencia;
		oCom.Parameters.Add("@p_dsNombreArchivo", SqlDbType.VarChar).Value = pCatastroPlanos.dsNombreArchivo;
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

	public static DataTable obtenerPlanosDisponibles(eUsuario pUsuarioLogueado)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_CATASTRO_PLANOS", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			cmd.Parameters.AddWithValue("@p_cdOperacion", "4");
			cmd.Parameters.AddWithValue("@p_nuSecuencia", 0);
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

	public static void pasarPlanoCopiado(eUsuario pUsuarioLogueado, int pIdPlano)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_CATASTRO_PLANOS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "5";
		oCom.Parameters.Add("@p_id", SqlDbType.Int).Value = pIdPlano;
		oCom.Parameters.Add("@p_nuSecuencia", SqlDbType.Int).Value = 0;
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

	public static bool existeCodigoDocumentoCopiado(eUsuario pUsuarioLogueado, string pCdDocumento)
	{
		int resultado = 0;
		bool existe = false;
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_CATASTRO_PLANOS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "6";
		oCom.Parameters.Add("@p_cdDocumento", SqlDbType.VarChar).Value = pCdDocumento;
		SqlParameter outputIdParam = new SqlParameter("@p_nuSecuencia", SqlDbType.Int)
		{
			Direction = ParameterDirection.Output
		};
		oCom.Parameters.Add(outputIdParam);
		try
		{
			oCom.ExecuteNonQuery();
			if ((int)outputIdParam.Value == 0)
			{
				return false;
			}
			return true;
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
