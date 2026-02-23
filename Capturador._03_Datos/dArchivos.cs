using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Capturador._03_Datos;

public class dArchivos
{
	public static List<string> BuscarArchivosIndices(string pCarpeta, string pArchivoIndice)
	{
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, pArchivoIndice, SearchOption.AllDirectories);
		if (Resultado.Length == 0)
		{
			throw new Exception("No se encontraró el archivo " + pArchivoIndice + " en la carpeta " + pCarpeta);
		}
		string[] array = Resultado;
		foreach (string item in array)
		{
			oListaArchivos.Add(item);
		}
		return oListaArchivos;
	}

	public static List<string> BuscarArchivosPDF(string pCarpeta)
	{
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, "*.PDF", SearchOption.AllDirectories);
		string[] array = Resultado;
		foreach (string item in array)
		{
			oListaArchivos.Add(item);
		}
		return oListaArchivos;
	}

	public static List<string> BuscarArchivosPDFxCantidadCaracteres(string pCarpeta, int pCantidadCaracteres)
	{
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, "*.PDF", SearchOption.AllDirectories);
		string[] array = Resultado;
		foreach (string item in array)
		{
			string nombreArchivo = Path.GetFileNameWithoutExtension(item);
			int cantidadCaracteres = nombreArchivo.Length;
			if (cantidadCaracteres == pCantidadCaracteres)
			{
				oListaArchivos.Add(item);
			}
		}
		return oListaArchivos;
	}

	public static List<string> BuscarArchivosPDFxEmpiezaCero(string pCarpeta)
	{
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, "0*.PDF", SearchOption.AllDirectories);
		string[] array = Resultado;
		foreach (string item in array)
		{
			oListaArchivos.Add(item);
		}
		return oListaArchivos;
	}

	public static List<string> BuscarArchivosPDFxEmpiezaConscan(string pCarpeta)
	{
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, "scan*.PDF", SearchOption.AllDirectories);
		string[] array = Resultado;
		foreach (string item in array)
		{
			oListaArchivos.Add(item);
		}
		return oListaArchivos;
	}

	public static List<string> BuscarArchivosPDFmayoraCantidadCaracteres(string pCarpeta, int pCantidadCaracteres)
	{
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, "*.PDF", SearchOption.AllDirectories);
		string[] array = Resultado;
		foreach (string item in array)
		{
			string nombreArchivo = Path.GetFileNameWithoutExtension(item);
			int cantidadCaracteres = nombreArchivo.Length;
			if (cantidadCaracteres > pCantidadCaracteres)
			{
				oListaArchivos.Add(item);
			}
		}
		return oListaArchivos;
	}

	public static int obtenerCantidadArchivos(string pCarpeta, string pArchivo)
	{
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, pArchivo, SearchOption.AllDirectories);
		return Resultado.Length;
	}

	public static DataTable CargarArchivoIndice(string pRutaArchivoIndice)
	{
		char caracter = ';';
		DataTable oDT = new DataTable();
		oDT.Columns.Add("id", typeof(string));
		oDT.Columns.Add("Despacho", typeof(string));
		oDT.Columns.Add("Serie Documental", typeof(string));
		oDT.Columns.Add("SIGEA", typeof(string));
		oDT.Columns.Add("Guia", typeof(string));
		oDT.Columns.Add("Usuario Digitalización", typeof(string));
		oDT.Columns.Add("Nombre Lote", typeof(string));
		oDT.Columns.Add("Ruta Archivo", typeof(string));
		try
		{
			StreamReader oReader = new StreamReader(pRutaArchivoIndice, Encoding.Default);
			string sLine = "";
			int fila = 0;
			oDT.Rows.Clear();
			do
			{
				sLine = oReader.ReadLine();
				if (sLine != null)
				{
					AgregarFilaDataTable(oDT, sLine, caracter, fila, pRutaArchivoIndice);
				}
			}
			while (sLine != null);
			oReader.Close();
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message + ": Verifique que no este abierto por otra aplicación");
		}
		return oDT;
	}

	private static void AgregarFilaDataTable(DataTable pTabla, string pLinea, char pCaracter, int pFila, string pRutaArchivoIndice)
	{
		string[] arreglo = pLinea.Split(pCaracter);
		string nombreLote = obtenerNombreLote(pRutaArchivoIndice);
		string rutaLote = Path.GetDirectoryName(pRutaArchivoIndice);
		string rutaArchivoPDF = rutaLote + "\\" + arreglo[1] + ".PDF";
		pTabla.Rows.Add(Convert.ToString(arreglo[0]), Convert.ToString(arreglo[1]), Convert.ToString(arreglo[2]), Convert.ToString(arreglo[3]), Convert.ToString(arreglo[4]), Convert.ToString(arreglo[5]), nombreLote, rutaArchivoPDF);
	}

	public static string obtenerNombreLote(string pCarpeta)
	{
		string[] parts = pCarpeta.Split(new string[1] { "\\" }, StringSplitOptions.None);
		return parts[parts.Length - 2];
	}

	public static void modificarNombreArchivo(string pRutaArchivo, string pNombreAnterior, string pNombreNuevo)
	{
		try
		{
			string archivoAnterior = Path.Combine(pRutaArchivo, pNombreAnterior);
			string archivoNuevo = Path.Combine(pRutaArchivo, pNombreNuevo);
			if (File.Exists(archivoAnterior))
			{
				if (File.Exists(archivoNuevo))
				{
					File.Delete(archivoNuevo);
				}
				File.Move(archivoAnterior, archivoNuevo);
				File.Delete(archivoAnterior);
				return;
			}
			throw new FileNotFoundException("El archivo '" + archivoAnterior + "' no existe.");
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error al renombrar el archivo: " + ex.Message);
			throw;
		}
	}

	public static void copiarArchivo(string pRutaOrigen, string pRutaDestino)
	{
		File.Copy(pRutaOrigen, pRutaDestino);
	}

	public static bool existeArchivo(string pArchivo)
	{
		bool existeArchivo = false;
		if (File.Exists(pArchivo))
		{
			existeArchivo = true;
		}
		return existeArchivo;
	}

	public static List<string> BuscarArchivos(string pCarpeta, string pExtension)
	{
		string ArchivoBuscar = "*." + pExtension;
		List<string> oListaArchivos = new List<string>();
		string[] Resultado = Directory.GetFiles(pCarpeta, ArchivoBuscar);
		string[] array = Resultado;
		foreach (string item in array)
		{
			string NombreArchivo = Path.GetFileName(item);
			oListaArchivos.Add(NombreArchivo);
		}
		return oListaArchivos;
	}

	public static string crearCarpeta(string pRutaCarpeta, string pNombreCarpeta)
	{
		pNombreCarpeta = pNombreCarpeta.Replace("/", "-");
		string carpetaCrear = Path.Combine(pRutaCarpeta, pNombreCarpeta);
		if (!Directory.Exists(carpetaCrear))
		{
			DirectoryInfo di = Directory.CreateDirectory(carpetaCrear);
		}
		return carpetaCrear;
	}

	public static void agregarLineaIndice(string pRutaCarpeta, string pNombreArchivo, string pMedicamento)
	{
		string archivo = Path.Combine(pRutaCarpeta, pNombreArchivo);
		StreamWriter oEscribirArchivo = new StreamWriter(archivo, append: true);
		oEscribirArchivo.WriteLine(pMedicamento);
		oEscribirArchivo.Close();
	}
}
