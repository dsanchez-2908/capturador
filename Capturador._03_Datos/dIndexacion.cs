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

	public static List<eIDX_Historia_Clinica> obtenerIndexacionHistoriasClinicas(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdLote)
	{
        SqlConnection oCon = ConexionSQL.ObtenerConexion();
        SqlCommand oCom = oCon.CreateCommand();
        oCom.CommandType = CommandType.StoredProcedure;
        oCom.CommandText = "SP_IDX_HISTORIAS_CLINICAS";
        oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
        oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "1";
        oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
        oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pCdLote;
        try
        {
			SqlDataReader oLeer = oCom.ExecuteReader();
			List<eIDX_Historia_Clinica> oListaHistoriasClinicas = new List<eIDX_Historia_Clinica>();
			while (oLeer.Read())
			{
				eIDX_Historia_Clinica oHistoriaClinica = new eIDX_Historia_Clinica();

				// Mapear campos obligatorios
				oHistoriaClinica.cdLoteDetalle = Convert.ToInt32(oLeer["cdLoteDetalle"]);

				// Mapear campos que pueden ser NULL
				if (oLeer["cdProyecto"] != DBNull.Value)
					oHistoriaClinica.cdProyecto = Convert.ToInt32(oLeer["cdProyecto"]);

				if (oLeer["dsProyecto"] != DBNull.Value)
					oHistoriaClinica.dsProyecto = oLeer["dsProyecto"].ToString();

				if (oLeer["cdLote"] != DBNull.Value)
					oHistoriaClinica.cdLote = Convert.ToInt32(oLeer["cdLote"]);

				if (oLeer["dsNombreLote"] != DBNull.Value)
					oHistoriaClinica.dsNombreLote = oLeer["dsNombreLote"].ToString();

				if (oLeer["nuCantidadArchivos"] != DBNull.Value)
					oHistoriaClinica.nuCantidadArchivos = Convert.ToInt32(oLeer["nuCantidadArchivos"]);

				//if (oLeer["dsRutaLoteFinal"] != DBNull.Value)
				//	oHistoriaClinica.dsRutaLoteFinal = oLeer["dsRutaLoteFinal"].ToString();

				if (oLeer["dsTipoDetalle"] != DBNull.Value)
					oHistoriaClinica.dsTipoDetalle = oLeer["dsTipoDetalle"].ToString();

				if (oLeer["dsNombreArchivo"] != DBNull.Value)
					oHistoriaClinica.dsNombreArchivo = oLeer["dsNombreArchivo"].ToString();

				if (oLeer["nuCantidadPaginasInicial"] != DBNull.Value)
					oHistoriaClinica.nuCantidadPaginaInicial = Convert.ToInt32(oLeer["nuCantidadPaginasInicial"]);

				if (oLeer["nuCantidadPaginasFinal"] != DBNull.Value)
					oHistoriaClinica.nuCantidadPaginasFinal = Convert.ToInt32(oLeer["nuCantidadPaginasFinal"]);

				if (oLeer["nuHistoriaClinica"] != DBNull.Value)
					oHistoriaClinica.nuHistoriaClinica = oLeer["nuHistoriaClinica"].ToString();

				if (oLeer["nuDNI"] != DBNull.Value)
					oHistoriaClinica.nuDNI = oLeer["nuDNI"].ToString();

                if (oLeer["dsNombreApellido"] != DBNull.Value)
                    oHistoriaClinica.dsNombreApellido = oLeer["dsNombreApellido"].ToString();

                if (oLeer["feNacimiento"] != DBNull.Value)
					oHistoriaClinica.feNacimiento = Convert.ToDateTime(oLeer["feNacimiento"]);

				//if (oLeer["feAlta"] != DBNull.Value)
				//	oHistoriaClinica.feAlta = Convert.ToDateTime(oLeer["feAlta"]);

				if (oLeer["cdUsuarioIndexacion"] != DBNull.Value)
					oHistoriaClinica.cdUsuarioIndexacion = Convert.ToInt32(oLeer["cdUsuarioIndexacion"]);

				if (oLeer["dsUsuarioIndexacion"] != DBNull.Value)
					oHistoriaClinica.dsUsuarioIndexacion = oLeer["dsUsuarioIndexacion"].ToString();

				oListaHistoriasClinicas.Add(oHistoriaClinica);
			}
			return oListaHistoriasClinicas;
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

    public static eIDX_Historia_Clinica obtenerArchivoIndexar(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdLote)
    {
        SqlConnection oCon = ConexionSQL.ObtenerConexion();
        SqlCommand oCom = oCon.CreateCommand();
        oCom.CommandType = CommandType.StoredProcedure;
        oCom.CommandText = "SP_IDX_HISTORIAS_CLINICAS";
        oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
        oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "2";
        oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
        oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pCdLote;
        try
        {
            SqlDataReader oLeer = oCom.ExecuteReader();
            //List<eIDX_Historia_Clinica> oListaHistoriasClinicas = new List<eIDX_Historia_Clinica>();
            eIDX_Historia_Clinica oHistoriaClinica = new eIDX_Historia_Clinica();
            while (oLeer.Read())
            {
                

                // Mapear campos obligatorios
                oHistoriaClinica.cdLoteDetalle = Convert.ToInt32(oLeer["cdLoteDetalle"]);

                // Mapear campos que pueden ser NULL
                if (oLeer["cdProyecto"] != DBNull.Value)
                    oHistoriaClinica.cdProyecto = Convert.ToInt32(oLeer["cdProyecto"]);

                if (oLeer["dsProyecto"] != DBNull.Value)
                    oHistoriaClinica.dsProyecto = oLeer["dsProyecto"].ToString();

                if (oLeer["cdLote"] != DBNull.Value)
                    oHistoriaClinica.cdLote = Convert.ToInt32(oLeer["cdLote"]);

                if (oLeer["dsNombreLote"] != DBNull.Value)
                    oHistoriaClinica.dsNombreLote = oLeer["dsNombreLote"].ToString();

                if (oLeer["nuCantidadArchivos"] != DBNull.Value)
                    oHistoriaClinica.nuCantidadArchivos = Convert.ToInt32(oLeer["nuCantidadArchivos"]);

                //if (oLeer["dsRutaLoteFinal"] != DBNull.Value)
                //	oHistoriaClinica.dsRutaLoteFinal = oLeer["dsRutaLoteFinal"].ToString();

                if (oLeer["dsTipoDetalle"] != DBNull.Value)
                    oHistoriaClinica.dsTipoDetalle = oLeer["dsTipoDetalle"].ToString();

                if (oLeer["dsNombreArchivo"] != DBNull.Value)
                    oHistoriaClinica.dsNombreArchivo = oLeer["dsNombreArchivo"].ToString();

                if (oLeer["nuCantidadPaginasInicial"] != DBNull.Value)
                    oHistoriaClinica.nuCantidadPaginaInicial = Convert.ToInt32(oLeer["nuCantidadPaginasInicial"]);

                if (oLeer["nuCantidadPaginasFinal"] != DBNull.Value)
                    oHistoriaClinica.nuCantidadPaginasFinal = Convert.ToInt32(oLeer["nuCantidadPaginasFinal"]);

                if (oLeer["nuHistoriaClinica"] != DBNull.Value)
                    oHistoriaClinica.nuHistoriaClinica = oLeer["nuHistoriaClinica"].ToString();

                if (oLeer["nuDNI"] != DBNull.Value)
                    oHistoriaClinica.nuDNI = oLeer["nuDNI"].ToString();

                if (oLeer["dsNombreApellido"] != DBNull.Value)
                    oHistoriaClinica.dsNombreApellido = oLeer["dsNombreApellido"].ToString();

                if (oLeer["feNacimiento"] != DBNull.Value)
                    oHistoriaClinica.feNacimiento = Convert.ToDateTime(oLeer["feNacimiento"]);

                //if (oLeer["feAlta"] != DBNull.Value)
                //	oHistoriaClinica.feAlta = Convert.ToDateTime(oLeer["feAlta"]);

                if (oLeer["cdUsuarioIndexacion"] != DBNull.Value)
                    oHistoriaClinica.cdUsuarioIndexacion = Convert.ToInt32(oLeer["cdUsuarioIndexacion"]);

                if (oLeer["dsUsuarioIndexacion"] != DBNull.Value)
                    oHistoriaClinica.dsUsuarioIndexacion = oLeer["dsUsuarioIndexacion"].ToString();

                //oListaHistoriasClinicas.Add(oHistoriaClinica);
            }
            return oHistoriaClinica;
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

    public static void guardarHistoriaClinica(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdLote, eIDX_Historia_Clinica pIDX_Historia_Clinica)
    {
        SqlConnection oCon = ConexionSQL.ObtenerConexion();
        SqlCommand oCom = oCon.CreateCommand();
        oCom.CommandType = CommandType.StoredProcedure;
        oCom.CommandText = "SP_IDX_HISTORIAS_CLINICAS";
        oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
        oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "3";

        oCom.Parameters.Add("@p_cdProyecto", SqlDbType.Int).Value = pCdProyecto;
        oCom.Parameters.Add("@p_cdLote", SqlDbType.Int).Value = pCdLote;

        oCom.Parameters.Add("@p_cdLoteDetalle", SqlDbType.Int).Value = pIDX_Historia_Clinica.cdLoteDetalle;
        
		oCom.Parameters.Add("@p_nuHistoriaClinica", SqlDbType.VarChar).Value = pIDX_Historia_Clinica.nuHistoriaClinica;
        oCom.Parameters.Add("@p_nuDNI", SqlDbType.VarChar).Value = pIDX_Historia_Clinica.nuDNI;
        oCom.Parameters.Add("@p_dsNombreApellido", SqlDbType.VarChar).Value = pIDX_Historia_Clinica.dsNombreApellido;
        oCom.Parameters.Add("@p_feNacimiento", SqlDbType.Date).Value = pIDX_Historia_Clinica.feNacimiento;

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

    public static void finalizarIndexacionHistoriaClinica(eUsuario pUsuarioLogueado, int pCdProyecto, int pCdLote)
    {
        SqlConnection oCon = ConexionSQL.ObtenerConexion();
        SqlCommand oCom = oCon.CreateCommand();
        oCom.CommandType = CommandType.StoredProcedure;
        oCom.CommandText = "SP_IDX_HISTORIAS_CLINICAS";
        oCom.Parameters.Add("@p_cdUsuarioLogueado", SqlDbType.Int).Value = pUsuarioLogueado.cdUsuario;
        oCom.Parameters.Add("@p_cdOperacion", SqlDbType.VarChar).Value = "4";

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

}
