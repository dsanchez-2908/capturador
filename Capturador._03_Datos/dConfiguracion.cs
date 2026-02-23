using System;
using System.Data;
using System.Data.SqlClient;
using Capturador._04_Entidades;

namespace Capturador._03_Datos;

public class dConfiguracion
{
	public static eProyectoConfiguracion ObtenerConfiguracionProyecto(eUsuario pUsuarioLogueado, int pProyecto)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_PROYECTO_CONFIGURACION_Obtener";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.Int).Value = "1";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pProyecto;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			eProyectoConfiguracion oProyectoConfiguracion = new eProyectoConfiguracion();
			while (oLeer.Read())
			{
				oProyectoConfiguracion.cdProyecto = Convert.ToInt32(oLeer["cdProyecto"].ToString());
				oProyectoConfiguracion.snControlCalidad = oLeer["snControlCalidad"].ToString();
				oProyectoConfiguracion.snIndexacion = oLeer["snIndexacion"].ToString();
				oProyectoConfiguracion.snSeparacion = oLeer["snSeparador"].ToString();
				oProyectoConfiguracion.snOCR = oLeer["snOCR"].ToString();
				oProyectoConfiguracion.dsRSrutaUltimaCarpeta = oLeer["dsRutaUltimaCarpeta"].ToString();
				oProyectoConfiguracion.dsRutaSalida = oLeer["dsRutaSalida"].ToString();
			}
			return oProyectoConfiguracion;
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

	public static void actualizarUltimaCarpetaOrigen(eUsuario pUsuarioLogueado, int pCdProyecto, string pUltimaCarpeta)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_PROYECTO_CONFIGURACION_Obtener", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			cmd.Parameters.AddWithValue("@p_cdOperacion", "2");
			cmd.Parameters.AddWithValue("@p_cdProyecto", pCdProyecto);
			cmd.Parameters.AddWithValue("@p_dsRutaUltimaCarpeta", pUltimaCarpeta);
			cmd.ExecuteNonQuery();
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

	public static void actualizarUltimaCarpetaSalida(eUsuario pUsuarioLogueado, int pCdProyecto, string pUltimaCarpeta)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_PROYECTO_CONFIGURACION_Obtener", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			cmd.Parameters.AddWithValue("@p_cdOperacion", "3");
			cmd.Parameters.AddWithValue("@p_cdProyecto", pCdProyecto);
			cmd.Parameters.AddWithValue("@p_dsRutaUltimaCarpeta", pUltimaCarpeta);
			cmd.ExecuteNonQuery();
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
