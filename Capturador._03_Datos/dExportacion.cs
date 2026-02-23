using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;

namespace Capturador._03_Datos;

public class dExportacion
{
	public static void GenerarArchivoCSV(string pRutaArchivo, string pNombreArchivo, DataTable pTabla)
	{
		if (pTabla.Rows.Count > 0)
		{
			bool fileError = false;
			string archivo = Path.Combine(pRutaArchivo, pNombreArchivo);
			if (File.Exists(archivo))
			{
				try
				{
					File.Delete(archivo);
				}
				catch (IOException ex)
				{
					fileError = true;
					throw new Exception("No fue posible escribir los datos en el disco: " + ex.Message);
				}
			}
			if (fileError)
			{
				return;
			}
			try
			{
				int columnCount = pTabla.Columns.Count;
				string columnNames = string.Join(";", from DataColumn col in pTabla.Columns
					select col.ColumnName);
				string[] outputCsv = new string[pTabla.Rows.Count];
				for (int i = 0; i < pTabla.Rows.Count; i++)
				{
					outputCsv[i] = string.Join(";", pTabla.Rows[i].ItemArray.Select((object item) => item?.ToString() ?? ""));
				}
				File.WriteAllLines(archivo, outputCsv, Encoding.UTF8);
				return;
			}
			catch (Exception ex2)
			{
				throw new Exception("Error: " + ex2.Message);
			}
		}
		throw new Exception("No hay ningún registro para exportar!");
	}

	public static void GenerarArchivoCSV(DataTable pTabla, string pRutaArchivo)
	{
		if (pTabla.Rows.Count > 0)
		{
			try
			{
				List<string> lineas = new List<string>();
				string encabezado = string.Join(";", from DataColumn col in pTabla.Columns
					select col.ColumnName);
				lineas.Add(encabezado);
				foreach (DataRow fila in pTabla.Rows)
				{
					string linea = string.Join(";", fila.ItemArray.Select((object campo) => campo.ToString()));
					lineas.Add(linea);
				}
				File.WriteAllLines(pRutaArchivo, lineas, Encoding.UTF8);
				return;
			}
			catch (Exception ex)
			{
				throw new Exception("Error al generar el archivo CSV: " + ex.Message);
			}
		}
		throw new Exception("La tabla está vacía, no hay datos para exportar.");
	}

	public static void GenerarArchivoXLXS(DataTable pTabla, string pRutaArchivo)
	{
		if (pTabla.Rows.Count > 0)
		{
			try
			{
				using XLWorkbook wb = new XLWorkbook();
				wb.Worksheets.Add(pTabla, "Datos");
				wb.SaveAs(pRutaArchivo);
				return;
			}
			catch (Exception ex)
			{
				throw new Exception("Error al generar el archivo Excel: " + ex.Message);
			}
		}
		throw new Exception("La tabla está vacía, no hay datos para exportar.");
	}
}
