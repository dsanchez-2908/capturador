using System;
using System.Collections.Generic;
using Capturador._03_Datos;
using Capturador._04_Entidades;

namespace Capturador._02_Negocio;

public class nListas
{
	public static void agregarLista(eUsuario pUsuarioLogueado, eLista pListaValores)
	{
		validarAgregarModificar(pListaValores);
		dListas.agregarLista(pUsuarioLogueado, pListaValores);
	}

	private static void validarAgregarModificar(eLista pListaValores)
	{
		if (string.IsNullOrEmpty(pListaValores.dsValorLista))
		{
			throw new Exception("Debe ingresar una valor para la lista");
		}
		if (string.IsNullOrEmpty(pListaValores.snHabilitado))
		{
			throw new Exception("Debe indicar si esta habilitado o no");
		}
	}

	public static void modificarLista(eUsuario pUsuarioLogueado, eLista pListaValores)
	{
		validarAgregarModificar(pListaValores);
		dListas.modificarLista(pUsuarioLogueado, pListaValores);
	}

	public static List<eProyecto> ObtenerListaProyectosActivos(eUsuario pUsuario)
	{
		return dListas.ObtenerListaProyectos(pUsuario, "1");
	}

	public static List<eProyecto> ObtenerListaProyectosTodos(eUsuario pUsuario)
	{
		return dListas.ObtenerListaProyectos(pUsuario, "2");
	}

	public static List<eEstado> ObtenerListaEstados(eUsuario pUsuario)
	{
		return dListas.ObtenerListaEstados(pUsuario);
	}

	public static List<eUsuario> ObtenerListaUsuarios(eUsuario pUsuario)
	{
		return dListas.ObtenerListaUsuarios(pUsuario);
	}

	public static List<eLista> ObtenerListaDigitalizadores(eUsuario pUsuario)
	{
		return dListas.ObtenerListaValores(pUsuario, "6");
	}

	public static List<eLista> ObtenerListaOrigen(eUsuario pUsuario)
	{
		return dListas.ObtenerListaValores(pUsuario, "7");
	}

	public static List<eLista> ObtenerListaValor(eUsuario pUsuario, int pCdProyecto, int pCdCampo)
	{
		return dListas.ObtenerListaValores(pUsuario, pCdProyecto, pCdCampo);
	}

	public static List<eRol> ObtenerListaRoles(eUsuario pUsuario)
	{
		return dListas.ObtenerListaRoles(pUsuario);
	}

	public static List<eEstado> ObtenerListaEstadosUsuarios(eUsuario pUsuarioLogueado)
	{
		return dListas.ObtenerListaEstadoUsuarios(pUsuarioLogueado);
	}

	public static List<eListaCampos> ObtenerListaNombreCampos(eUsuario pUsuarioLogueado, int pCdProyecto)
	{
		return dListas.ObtenerListaNombreCampos(pUsuarioLogueado, pCdProyecto);
	}
}
