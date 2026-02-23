using Capturador._03_Datos;
using Capturador._04_Entidades;

namespace Capturador._02_Negocio;

public class nConfiguracion
{
	public static eProyectoConfiguracion ObtenerUltimaCarpeta(eUsuario pUsuarioLogueado, int pCdProyecto)
	{
		return dConfiguracion.ObtenerConfiguracionProyecto(pUsuarioLogueado, pCdProyecto);
	}

	public static void actualizarUltimaCarpetaOrigen(eUsuario pUsuarioLogueado, int pCdProyecto, string pCarpetaActual, string pCarpetaSeleccionada)
	{
		if (pCarpetaActual != pCarpetaSeleccionada)
		{
			dConfiguracion.actualizarUltimaCarpetaOrigen(pUsuarioLogueado, pCdProyecto, pCarpetaSeleccionada);
		}
	}

	public static void actualizarUltimaCarpetaSalida(eUsuario pUsuarioLogueado, int pCdProyecto, string pCarpetaActual, string pCarpetaSeleccionada)
	{
		if (pCarpetaActual != pCarpetaSeleccionada)
		{
			dConfiguracion.actualizarUltimaCarpetaSalida(pUsuarioLogueado, pCdProyecto, pCarpetaSeleccionada);
		}
	}
}
