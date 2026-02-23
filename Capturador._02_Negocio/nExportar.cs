using System;
using System.Data;
using System.IO;
using Capturador._03_Datos;

namespace Capturador._02_Negocio;

public class nExportar
{
	public static void exportarTabla(DataTable pTabla, string pRutaArchivo)
	{
		if (pTabla.Rows.Count == 0)
		{
			throw new Exception("La tabla está vacía, no hay datos para exportar.");
		}
		string tipoArchivo = Path.GetExtension(pRutaArchivo).ToUpper();
		if (tipoArchivo == ".CSV")
		{
			dExportacion.GenerarArchivoCSV(pTabla, pRutaArchivo);
		}
		if (tipoArchivo == ".XLSX")
		{
			dExportacion.GenerarArchivoXLXS(pTabla, pRutaArchivo);
		}
	}
}
