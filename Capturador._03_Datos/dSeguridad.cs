using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Capturador._04_Entidades;

namespace Capturador._03_Datos;

public class dSeguridad
{
	public static string GetSHA256(string str)
	{
		SHA256 sha256 = SHA256.Create();
		ASCIIEncoding encoding = new ASCIIEncoding();
		byte[] stream = null;
		StringBuilder sb = new StringBuilder();
		stream = sha256.ComputeHash(encoding.GetBytes(str));
		for (int i = 0; i < stream.Length; i++)
		{
			sb.AppendFormat("{0:x2}", stream[i]);
		}
		return sb.ToString();
	}

	public static eUsuario ValidarLoginUsuario(string pUsuario, string pClave)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		string Clave = GetSHA256(pClave);
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "1";
		oCom.Parameters.Add("@p_dsUsuario", SqlDbType.VarChar).Value = pUsuario;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			eUsuario oUsuario = new eUsuario();
			while (oLeer.Read())
			{
				oUsuario.cdUsuario = Convert.ToInt32(oLeer["cdUsuario"].ToString());
				oUsuario.dsUsuario = oLeer["dsUsuario"].ToString();
				oUsuario.dsUsuarioNombre = oLeer["dsUsuarioNombre"].ToString();
				oUsuario.dsNombre = oLeer["dsNombre"].ToString();
				oUsuario.dsApellido = oLeer["dsApellido"].ToString();
				oUsuario.dsClave = oLeer["dsClave"].ToString();
				oUsuario.nuDNI = oLeer["nuDNI"].ToString();
				oUsuario.dsLegajo = oLeer["dsLegajo"].ToString();
				oUsuario.dsMail = oLeer["dsMail"].ToString();
				oUsuario.nuIntentosFallidos = Convert.ToInt32(oLeer["nuIntentosFallidos"].ToString());
				oUsuario.feAlta = Convert.ToDateTime(oLeer["feAlta"].ToString());
				oUsuario.dsRoles = oLeer["dsRoles"].ToString();
				oUsuario.cdEstado = Convert.ToInt32(oLeer["cdEstado"].ToString());
				oUsuario.dsEstado = oLeer["dsEstado"].ToString();
			}
			return oUsuario;
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

	public static void ActualizarIntentosFallidos(eUsuario pUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuario.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "8";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pUsuario.cdUsuario;
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

	public static void ActualizarCeroIntentosFallidos(eUsuario pUsuarioLogueado, eUsuario pUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "9";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.BigInt).Value = pUsuario.cdUsuario;
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

	public static void ActualizarClaveUsuario(eUsuario pUsuarioLogueado, eUsuario pUsuario, string pNuevaClave)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		string Clave = GetSHA256(pNuevaClave);
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "10";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pUsuario.cdUsuario;
		oCom.Parameters.Add("@p_dsClave", SqlDbType.VarChar).Value = Clave;
		oCom.Parameters.Add("@p_cdEstado", SqlDbType.BigInt).Value = 2;
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

	public static DataTable obtenerUsuarios(eUsuario pUsuarioLogueado, eUsuario pUsuario)
	{
		using SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();
		DataTable dt = new DataTable();
		try
		{
			cmd = new SqlCommand("SP_Seguridad_Usuario", oCon);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			cmd.Parameters.AddWithValue("@p_operacion", 3);
			if (pUsuario.cdUsuario != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdUsuario", pUsuario.cdUsuario);
			}
			if (!string.IsNullOrEmpty(pUsuario.dsUsuario))
			{
				cmd.Parameters.AddWithValue("@p_dsUsuario", pUsuario.dsUsuario);
			}
			if (!string.IsNullOrEmpty(pUsuario.dsNombre))
			{
				cmd.Parameters.AddWithValue("@p_dsNombre", pUsuario.dsNombre);
			}
			if (!string.IsNullOrEmpty(pUsuario.dsApellido))
			{
				cmd.Parameters.AddWithValue("@p_dsApellido", pUsuario.dsApellido);
			}
			if (!string.IsNullOrEmpty(pUsuario.dsMail))
			{
				cmd.Parameters.AddWithValue("@p_dsMail", pUsuario.dsMail);
			}
			if (pUsuario.cdEstado != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdEstado", pUsuario.cdEstado);
			}
			if (pUsuario.feAltaDesde != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feAltaDesde", pUsuario.feAltaDesde);
			}
			if (pUsuario.feAltaHasta != DateTime.MinValue)
			{
				cmd.Parameters.AddWithValue("@p_feAltaHasta", pUsuario.feAltaHasta);
			}
			if (pUsuario.cdRol != 0)
			{
				cmd.Parameters.AddWithValue("@p_cdRol", pUsuario.cdRol);
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

	public static void agregarUsuario(eUsuario pUsuarioLogueado, eUsuario pUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		string Clave = GetSHA256(pUsuario.dsClave);
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "5";
		oCom.Parameters.Add("@p_dsUsuario", SqlDbType.VarChar).Value = pUsuario.dsUsuario;
		oCom.Parameters.Add("@p_dsNombre", SqlDbType.VarChar).Value = pUsuario.dsNombre;
		oCom.Parameters.Add("@p_dsApellido", SqlDbType.VarChar).Value = pUsuario.dsApellido;
		oCom.Parameters.Add("@p_dsMail", SqlDbType.VarChar).Value = pUsuario.dsMail;
		oCom.Parameters.Add("@p_dsClave", SqlDbType.VarChar).Value = Clave;
		oCom.Parameters.Add("@p_dsDNI", SqlDbType.VarChar).Value = pUsuario.nuDNI;
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

	public static void asociarRolUsuario(eUsuario pUsuarioLogueado, eUsuario pUsuario, int pCdRol)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		string Clave = GetSHA256(pUsuario.dsClave);
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "6";
		oCom.Parameters.Add("@p_dsUsuario", SqlDbType.VarChar).Value = pUsuario.dsUsuario;
		oCom.Parameters.Add("@p_cdRol", SqlDbType.Int).Value = pCdRol;
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

	public static void asociarProyectoUsuario(eUsuario pUsuarioLogueado, eUsuario pUsuario, int pCdProyecto)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		string Clave = GetSHA256(pUsuario.dsClave);
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "7";
		oCom.Parameters.Add("@p_dsUsuario", SqlDbType.VarChar).Value = pUsuario.dsUsuario;
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
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

	public static eUsuario obtenerUnUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "11";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			eUsuario oUsuario = new eUsuario();
			while (oLeer.Read())
			{
				oUsuario.cdUsuario = Convert.ToInt32(oLeer["cdUsuario"].ToString());
				oUsuario.dsUsuario = oLeer["dsUsuario"].ToString();
				oUsuario.dsUsuarioNombre = oLeer["dsUsuarioNombre"].ToString();
				oUsuario.dsNombre = oLeer["dsNombre"].ToString();
				oUsuario.dsApellido = oLeer["dsApellido"].ToString();
				oUsuario.dsClave = oLeer["dsClave"].ToString();
				oUsuario.nuDNI = oLeer["nuDNI"].ToString();
				oUsuario.dsLegajo = oLeer["dsLegajo"].ToString();
				oUsuario.dsMail = oLeer["dsMail"].ToString();
				oUsuario.nuIntentosFallidos = Convert.ToInt32(oLeer["nuIntentosFallidos"].ToString());
				oUsuario.feAlta = Convert.ToDateTime(oLeer["feAlta"].ToString());
				oUsuario.dsRoles = oLeer["dsRoles"].ToString();
				oUsuario.cdEstado = Convert.ToInt32(oLeer["cdEstado"].ToString());
				oUsuario.dsEstado = oLeer["dsEstado"].ToString();
			}
			return oUsuario;
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

	public static List<string> obtenerRolesUnUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "12";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			List<string> oListaRoles = new List<string>();
			while (oLeer.Read())
			{
				oListaRoles.Add(oLeer["dsRol"].ToString());
			}
			return oListaRoles;
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

	public static List<string> obtenerProyetosUnUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = 0;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "13";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			List<string> oListaProyectos = new List<string>();
			while (oLeer.Read())
			{
				oListaProyectos.Add(oLeer["dsProyecto"].ToString());
			}
			return oListaProyectos;
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

	public static void desbloquearUsuario(eUsuario pUsuarioLogueado, int pCdUsuario, string pDsClave)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		string Clave = GetSHA256(pDsClave);
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "14";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
		oCom.Parameters.Add("@p_dsClave", SqlDbType.VarChar).Value = Clave;
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

	public static void borrarTodosRolesUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "15";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
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

	public static void asociarNuevosRolesUsuario(eUsuario pUsuarioLogueado, int pCdUsuario, int pCdRol)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "16";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
		oCom.Parameters.Add("@p_cdRol", SqlDbType.Int).Value = pCdRol;
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

	public static void borrarTodosProyectoUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "17";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
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

	public static void asociarNuevosProyectosUsuario(eUsuario pUsuarioLogueado, int pCdUsuario, int pCdProyecto)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "18";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
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

	public static void eliminarUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_Seguridad_Usuario";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_operacion", SqlDbType.VarChar).Value = "19";
		oCom.Parameters.Add("@p_cdUsuario", SqlDbType.Int).Value = pCdUsuario;
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

	public static List<eMenuItem> ObtenerMenuDesdeBD(eUsuario pUsuarioLogueado)
	{
		List<eMenuItem> resultado = new List<eMenuItem>();
		using (SqlConnection conn = ConexionSQL.ObtenerConexion())
		{
			using SqlCommand cmd = new SqlCommand("SP_MENU_Obtener", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@p_cdUsuarioLogueado", pUsuarioLogueado.cdUsuario);
			using SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				resultado.Add(new eMenuItem
				{
					cdMenu = reader.GetInt32(0),
					nuNivel = reader.GetInt32(1),
					nuOrden = reader.GetInt32(2),
					cdMenuSuperior = reader.GetInt32(3),
					dsNombre = reader.GetString(4),
					dsFormulario = (reader.IsDBNull(5) ? null : reader.GetString(5)),
					dsRutaFormulario = (reader.IsDBNull(6) ? null : reader.GetString(6))
				});
			}
		}
		return resultado;
	}
}
