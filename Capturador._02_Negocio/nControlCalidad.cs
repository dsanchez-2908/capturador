using Capturador._03_Datos;
using Capturador._04_Entidades;

namespace Capturador._02_Negocio;

public class nControlCalidad
{
	public static eProyectoConfiguracion ObtenerUltimaCarpeta(eUsuario pUsuarioLogueado, int pCdProyecto)
	{
		return dControlCalidad.ObtenerConfiguracionProyecto(pUsuarioLogueado, pCdProyecto);
	}

	public static void actualizarUltimaCarpeta(eUsuario pUsuarioLogueado, int pCdProyecto, string pCarpetaActual, string pCarpetaSeleccionada)
	{
		if (pCarpetaActual != pCarpetaSeleccionada)
		{
			dControlCalidad.actualizarUltimaCarpeta(pUsuarioLogueado, pCdProyecto, pCarpetaSeleccionada);
		}
	}

	public static void actualizarNombreArchivo(eUsuario pUsuarioLogueado, int pCdLote, string pArchivos)
	{
		dControlCalidad.actualizarNombreArchivo(pUsuarioLogueado, pCdLote, pArchivos);
	}
}
