namespace Capturador._04_Entidades;

public class eLoteDetalle
{
	private int CDloteDetalle;

	private int CDlote;

	private string DSnombreArchivo;

	private int NUcantidadPaginasInicial;

	private int NUcantidadPaginasFinal;

	private int CDestado;

	private string DSestado;

	public string dsEstado
	{
		get
		{
			return DSestado;
		}
		set
		{
			DSestado = value;
		}
	}

	public int cdEstado
	{
		get
		{
			return CDestado;
		}
		set
		{
			CDestado = value;
		}
	}

	public int nuCantidadPaginasFinal
	{
		get
		{
			return NUcantidadPaginasFinal;
		}
		set
		{
			NUcantidadPaginasFinal = value;
		}
	}

	public int nuCantidadPaginasInicial
	{
		get
		{
			return NUcantidadPaginasInicial;
		}
		set
		{
			NUcantidadPaginasInicial = value;
		}
	}

	public string dsNombreArchivo
	{
		get
		{
			return DSnombreArchivo;
		}
		set
		{
			DSnombreArchivo = value;
		}
	}

	public int cdLote
	{
		get
		{
			return CDlote;
		}
		set
		{
			CDlote = value;
		}
	}

	public int cdLoteDetalle
	{
		get
		{
			return CDloteDetalle;
		}
		set
		{
			CDloteDetalle = value;
		}
	}
}
