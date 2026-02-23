using System;
using System.Data.SqlClient;
using Capturador.Properties;

namespace Capturador._03_Datos;

public class ConexionSQL
{
	private static string ObtenerCadena()
	{
		return Settings.Default.CapturadorConnectionString;
	}

	public static SqlConnection ObtenerConexion()
	{
		SqlConnection oCon = new SqlConnection(ObtenerCadena());
		try
		{
			oCon.Open();
		}
		catch (Exception ex)
		{
			throw new Exception("No se pudo conectar con la Base de datos: " + ex.Message);
		}
		return oCon;
	}

	public static string RecuperarNombreBD()
	{
		SqlConnection oCon = ObtenerConexion();
		return oCon.Database;
	}
}
