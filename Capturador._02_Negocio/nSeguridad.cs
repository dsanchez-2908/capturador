using System;
using System.Collections.Generic;
using System.Data;
using Capturador._03_Datos;
using Capturador._04_Entidades;

namespace Capturador._02_Negocio;

public class nSeguridad
{
	public static eUsuario ValidarLoginUsuario(eUsuario pUsuarioLogueado, string pUsuario, string pClave)
	{
		eUsuario oUsuario = new eUsuario();
		oUsuario = dSeguridad.ValidarLoginUsuario(pUsuario, pClave);
		if (oUsuario.cdUsuario == 0)
		{
			throw new Exception("El usuario o clave son incorrectos");
		}
		try
		{
			ValidarBloqueado(oUsuario);
			ValidarClave(oUsuario, pClave);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		dSeguridad.ActualizarCeroIntentosFallidos(pUsuarioLogueado, oUsuario);
		return oUsuario;
	}

	private static void ValidarBloqueado(eUsuario pUsuario)
	{
		if (pUsuario.cdEstado == 3)
		{
			throw new Exception("El usuario " + pUsuario.dsUsuario + " esta bloqueado");
		}
	}

	private static void ValidarClave(eUsuario pUsuario, string pClave)
	{
		string Clave = dSeguridad.GetSHA256(pClave);
		if (pUsuario.dsClave != Clave)
		{
			dSeguridad.ActualizarIntentosFallidos(pUsuario);
			throw new Exception("El usuario o clave son incorrectos");
		}
	}

	public static void ActualizarClaveNueva(eUsuario pUsuarioLogueado, eUsuario pUsuario, string pNuevaClave)
	{
		dSeguridad.ActualizarClaveUsuario(pUsuarioLogueado, pUsuario, pNuevaClave);
	}

	public static DataTable ObtenerTablaUsuarios(eUsuario pUsuarioLogueado, eUsuario pUsuario)
	{
		return dSeguridad.obtenerUsuarios(pUsuarioLogueado, pUsuario);
	}

	public static void agregarUsuario(eUsuario pUsuarioLogueado, eUsuario pUsuario, List<int> pListaCdRolesSeleccionados, List<int> pListaCdProyectosSeleccionados)
	{
		validarAgregarUsuario(pUsuario, pListaCdRolesSeleccionados, pListaCdProyectosSeleccionados);
		dSeguridad.agregarUsuario(pUsuarioLogueado, pUsuario);
		asociarRolUsuario(pUsuarioLogueado, pUsuario, pListaCdRolesSeleccionados);
		asociarProyectoUsuario(pUsuarioLogueado, pUsuario, pListaCdProyectosSeleccionados);
	}

	private static void validarAgregarUsuario(eUsuario pUsuario, List<int> pListaCdRolesSeleccionados, List<int> pListaCdProyectosSeleccionados)
	{
		if (pListaCdRolesSeleccionados.Count == 0)
		{
			throw new Exception("Debe seleccionar por lo menos un rol");
		}
		if (pListaCdProyectosSeleccionados.Count == 0)
		{
			throw new Exception("Debe seleccionar por lo menos un Proyecto");
		}
		if (string.IsNullOrEmpty(pUsuario.dsUsuario))
		{
			throw new Exception("Debe ingresar un usuario");
		}
		if (string.IsNullOrEmpty(pUsuario.dsNombre))
		{
			throw new Exception("Debe ingresar un Nombre");
		}
		if (string.IsNullOrEmpty(pUsuario.dsApellido))
		{
			throw new Exception("Debe ingresar un Apellido");
		}
		if (string.IsNullOrEmpty(pUsuario.dsClave))
		{
			throw new Exception("Debe ingresar una clave provisoria");
		}
	}

	private static void asociarRolUsuario(eUsuario pUsuarioLogueado, eUsuario pUsuario, List<int> pListaCdRol)
	{
		foreach (int item in pListaCdRol)
		{
			dSeguridad.asociarRolUsuario(pUsuarioLogueado, pUsuario, item);
		}
	}

	private static void asociarProyectoUsuario(eUsuario pUsuarioLogueado, eUsuario pUsuario, List<int> pListaCdProyecto)
	{
		foreach (int item in pListaCdProyecto)
		{
			dSeguridad.asociarProyectoUsuario(pUsuarioLogueado, pUsuario, item);
		}
	}

	public static eUsuario obtenerUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		return dSeguridad.obtenerUnUsuario(pUsuarioLogueado, pCdUsuario);
	}

	public static List<string> obtenerListaRolesUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		return dSeguridad.obtenerRolesUnUsuario(pUsuarioLogueado, pCdUsuario);
	}

	public static List<string> obtenerListaProyectosUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		return dSeguridad.obtenerProyetosUnUsuario(pUsuarioLogueado, pCdUsuario);
	}

	public static void desbloquearUsuario(eUsuario pUsuarioLogueado, int pCdUsuario, string pDsClave)
	{
		dSeguridad.desbloquearUsuario(pUsuarioLogueado, pCdUsuario, pDsClave);
	}

	public static void ActualizarRolesUsuario(eUsuario pUsuarioLogueado, int pCdUsuario, List<int> pListaCdRol)
	{
		if (pListaCdRol.Count == 0)
		{
			throw new Exception("Debe seleccionar al menos un rol");
		}
		dSeguridad.borrarTodosRolesUsuario(pUsuarioLogueado, pCdUsuario);
		foreach (int item in pListaCdRol)
		{
			dSeguridad.asociarNuevosRolesUsuario(pUsuarioLogueado, pCdUsuario, item);
		}
	}

	public static void ActualizarProyectoUsuario(eUsuario pUsuarioLogueado, int pCdUsuario, List<int> pListaCdProyecto)
	{
		if (pListaCdProyecto.Count == 0)
		{
			throw new Exception("Debe seleccionar al menos un proyecto");
		}
		dSeguridad.borrarTodosProyectoUsuario(pUsuarioLogueado, pCdUsuario);
		foreach (int item in pListaCdProyecto)
		{
			dSeguridad.asociarNuevosProyectosUsuario(pUsuarioLogueado, pCdUsuario, item);
		}
	}

	public static void eliminarUsuario(eUsuario pUsuarioLogueado, int pCdUsuario)
	{
		dSeguridad.eliminarUsuario(pUsuarioLogueado, pCdUsuario);
	}

	public static string obtenerNombreBaseDatos()
	{
		return ConexionSQL.RecuperarNombreBD();
	}
}
