namespace Capturador._04_Entidades;

public class eProyectoConfiguracion
{
	private int CdProyecto;

	private string DsRutaUltimaCarpeta;

	private string DsRutaSalida;

	private string SnControlCalidad;

	private string SnIndexacion;

	private string SnSeparacion;

	private string SnOCR;

	public string snOCR
	{
		get
		{
			return SnOCR;
		}
		set
		{
			SnOCR = value;
		}
	}

	public string snSeparacion
	{
		get
		{
			return SnSeparacion;
		}
		set
		{
			SnSeparacion = value;
		}
	}

	public string snIndexacion
	{
		get
		{
			return SnIndexacion;
		}
		set
		{
			SnIndexacion = value;
		}
	}

	public string snControlCalidad
	{
		get
		{
			return SnControlCalidad;
		}
		set
		{
			SnControlCalidad = value;
		}
	}

	public string dsRutaSalida
	{
		get
		{
			return DsRutaSalida;
		}
		set
		{
			DsRutaSalida = value;
		}
	}

	public string dsRSrutaUltimaCarpeta
	{
		get
		{
			return DsRutaUltimaCarpeta;
		}
		set
		{
			DsRutaUltimaCarpeta = value;
		}
	}

	public int cdProyecto
	{
		get
		{
			return CdProyecto;
		}
		set
		{
			CdProyecto = value;
		}
	}
}
