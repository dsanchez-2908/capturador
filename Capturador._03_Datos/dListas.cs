using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Capturador._04_Entidades;

namespace Capturador._03_Datos;

public class dListas
{
	public static void agregarLista(eUsuario pUsuarioLogueado, eLista pListaValores)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "A";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pListaValores.cdProyecto;
		oCom.Parameters.Add("@p_cdCampo", SqlDbType.Int).Value = pListaValores.cdCampo;
		oCom.Parameters.Add("@p_cdExterno", SqlDbType.VarChar).Value = pListaValores.cdExterno;
		oCom.Parameters.Add("@p_dsValorLista", SqlDbType.VarChar).Value = pListaValores.dsValorLista;
		oCom.Parameters.Add("@p_snHabilitado", SqlDbType.VarChar).Value = pListaValores.snHabilitado;
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

	public static void modificarLista(eUsuario pUsuarioLogueado, eLista pListaValores)
	{
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "M";
		oCom.Parameters.Add("@p_cdValorLista", SqlDbType.Int).Value = pListaValores.cdValor;
		oCom.Parameters.Add("@p_cdExterno", SqlDbType.VarChar).Value = pListaValores.cdExterno;
		oCom.Parameters.Add("@p_dsValorLista", SqlDbType.VarChar).Value = pListaValores.dsValorLista;
		oCom.Parameters.Add("@p_snHabilitado", SqlDbType.VarChar).Value = pListaValores.snHabilitado;
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

	public static List<eLista> ObtenerListaCampos(eUsuario pUsuarioLogueado, int pProyecto, int pCampo)
	{
		List<eLista> oListaCampos = new List<eLista>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.Int).Value = "3";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pProyecto;
		oCom.Parameters.Add("@p_cdCampo", SqlDbType.Int).Value = pCampo;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eLista oLista = new eLista();
				oLista.cdValor = Convert.ToInt32(oLeer["cdValor"].ToString());
				oLista.cdProyecto = Convert.ToInt32(oLeer["cdProyecto"].ToString());
				oLista.dsProyecto = oLeer["dsProyecto"].ToString();
				oLista.cdCampo = Convert.ToInt32(oLeer["cdCampo"].ToString());
				oLista.dsCampo = oLeer["dsCampo"].ToString();
				oLista.cdExterno = oLeer["cdExterno"].ToString();
				oLista.dsValorLista = oLeer["dsValor"].ToString();
				oLista.snHabilitado = oLeer["snHabilitado"].ToString();
				oListaCampos.Add(oLista);
			}
			return oListaCampos;
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

	public static List<eProyecto> ObtenerListaProyectos(eUsuario pUsuarioLogueado, string pOperacion)
	{
		List<eProyecto> oListaProyectos = new List<eProyecto>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = pOperacion;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eProyecto oProyecto = new eProyecto();
				oProyecto.cdProyecto = Convert.ToInt32(oLeer["cdProyecto"].ToString());
				oProyecto.dsProyecto = oLeer["dsProyecto"].ToString();
				oProyecto.snHabilitado = oLeer["snHabilitado"].ToString();
				oListaProyectos.Add(oProyecto);
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

	public static List<eUsuario> ObtenerListaUsuarios(eUsuario pUsuarioLogueado)
	{
		List<eUsuario> oListaUsuarios = new List<eUsuario>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "4";
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eUsuario oUsuario = new eUsuario();
				oUsuario.cdUsuario = Convert.ToInt32(oLeer["cdUsuario"].ToString());
				oUsuario.dsUsuario = oLeer["dsUsuario"].ToString();
				oUsuario.dsUsuarioNombre = oLeer["dsUsuarioNombre"].ToString();
				oListaUsuarios.Add(oUsuario);
			}
			return oListaUsuarios;
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

	public static List<eEstado> ObtenerListaEstados(eUsuario pUsuarioLogueado)
	{
		List<eEstado> oListaEstados = new List<eEstado>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "5";
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eEstado oEstado = new eEstado();
				oEstado.cdEstado = Convert.ToInt32(oLeer["cdEstado"].ToString());
				oEstado.dsEstado = oLeer["dsEstado"].ToString();
				oListaEstados.Add(oEstado);
			}
			return oListaEstados;
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

	public static List<eLista> ObtenerListaValores(eUsuario pUsuarioLogueado, string pOperacion)
	{
		List<eLista> oListaValores = new List<eLista>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = pOperacion;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eLista oLista = new eLista();
				oLista.cdValor = Convert.ToInt32(oLeer["cdValor"].ToString());
				oLista.dsValorLista = oLeer["dsValor"].ToString();
				oListaValores.Add(oLista);
			}
			return oListaValores;
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

	public static List<eRol> ObtenerListaRoles(eUsuario pUsuarioLogueado)
	{
		List<eRol> oListaRoles = new List<eRol>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = 9;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eRol oRol = new eRol();
				oRol.cdRol = Convert.ToInt32(oLeer["cdRol"].ToString());
				oRol.dsRol = oLeer["dsRol"].ToString();
				oListaRoles.Add(oRol);
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

	public static List<eEstado> ObtenerListaEstadoUsuarios(eUsuario pUsuarioLogueado)
	{
		List<eEstado> oListaEstados = new List<eEstado>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = 8;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eEstado oEstado = new eEstado();
				oEstado.cdEstado = Convert.ToInt32(oLeer["cdEstado"].ToString());
				oEstado.dsEstado = oLeer["dsEstado"].ToString();
				oListaEstados.Add(oEstado);
			}
			return oListaEstados;
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

	public static List<eListaCampos> ObtenerListaNombreCampos(eUsuario pUsuarioLogueado, int pProyecto)
	{
		List<eListaCampos> oListaCampos = new List<eListaCampos>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.Int).Value = "10";
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pProyecto;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eListaCampos oCampo = new eListaCampos();
				oCampo.cdCampo = Convert.ToInt32(oLeer["cdCampo"].ToString());
				oCampo.dsCampo = oLeer["dsCampo"].ToString();
				oCampo.snHabilitado = oLeer["snHabilitado"].ToString();
				oListaCampos.Add(oCampo);
			}
			return oListaCampos;
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

	public static List<eLista> ObtenerListaValores(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdCampos)
	{
		List<eLista> oListaValores = new List<eLista>();
		SqlConnection oCon = ConexionSQL.ObtenerConexion();
		SqlCommand oCom = oCon.CreateCommand();
		oCom.CommandType = CommandType.StoredProcedure;
		oCom.CommandText = "SP_LISTAS";
		oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
		oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = 11;
		oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
		oCom.Parameters.Add("@p_cdCampo", SqlDbType.Int).Value = pCdCampos;
		try
		{
			SqlDataReader oLeer = oCom.ExecuteReader();
			while (oLeer.Read())
			{
				eLista oLista = new eLista();
				oLista.cdValor = Convert.ToInt32(oLeer["cdValor"].ToString());
				oLista.cdExterno = oLeer["cdExterno"].ToString();
				oLista.dsValorLista = oLeer["dsValor"].ToString();
				oLista.snHabilitado = oLeer["snHabilitado"].ToString();
				oLista.cdProyecto = Convert.ToInt32(oLeer["cdProyecto"].ToString());
				oLista.cdCampo = Convert.ToInt32(oLeer["cdCampo"].ToString());
				oListaValores.Add(oLista);
			}
			return oListaValores;
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
}
