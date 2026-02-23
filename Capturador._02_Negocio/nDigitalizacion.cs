using Capturador._03_Datos;

namespace Capturador._02_Negocio;

public class nDigitalizacion
{
	public static void crearLote(string pCarpetaRepositorio, string pNombreLote)
	{
		dArchivos.crearCarpeta(pCarpetaRepositorio, pNombreLote);
	}
}
