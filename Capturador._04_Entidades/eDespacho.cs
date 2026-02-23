using System;

namespace Capturador._04_Entidades;

public class eDespacho
{
	private int ID;

	private int CDestado;

	private string DSestado;

	private string DSdespacho;

	private string NUguia;

	private string NUcodigoBarras;

	private int NUcantidadImagenes;

	private DateTime FEgeneracionIndice;

	private int CDusuarioDigitalizacion;

	private string DSusuarioDigitalizacion;

	private DateTime FEfinalizacionProceso;

	private int NUestacionTrabajo;

	private int CDusuarioProceso;

	private string DSusuarioProceso;

	private int NUimagen;

	private string CDserieDocumental;

	private string NUsigea;

	private string DSnombreLote;

	private string DSrutaArchivoPDF;

	public string dsRutaArchivoPDF
	{
		get
		{
			return DSrutaArchivoPDF;
		}
		set
		{
			DSrutaArchivoPDF = value;
		}
	}

	public string dsNombreLote
	{
		get
		{
			return DSnombreLote;
		}
		set
		{
			DSnombreLote = value;
		}
	}

	public string cdSerieDocumental
	{
		get
		{
			return CDserieDocumental;
		}
		set
		{
			CDserieDocumental = value;
		}
	}

	public string nuSIGEA
	{
		get
		{
			return NUsigea;
		}
		set
		{
			NUsigea = value;
		}
	}

	public int nuImagen
	{
		get
		{
			return NUimagen;
		}
		set
		{
			NUimagen = value;
		}
	}

	public string dsUsuarioProceso
	{
		get
		{
			return DSusuarioProceso;
		}
		set
		{
			DSusuarioProceso = value;
		}
	}

	public int cdUsuarioProceso
	{
		get
		{
			return CDusuarioProceso;
		}
		set
		{
			CDusuarioProceso = value;
		}
	}

	public int nuEstacionTrabajo
	{
		get
		{
			return NUestacionTrabajo;
		}
		set
		{
			NUestacionTrabajo = value;
		}
	}

	public DateTime feFinalizacionProceso
	{
		get
		{
			return FEfinalizacionProceso;
		}
		set
		{
			FEfinalizacionProceso = value;
		}
	}

	public string dsUsuarioDigitalizacion
	{
		get
		{
			return DSusuarioDigitalizacion;
		}
		set
		{
			DSusuarioDigitalizacion = value;
		}
	}

	public int cdUsuarioDigitalizacion
	{
		get
		{
			return CDusuarioDigitalizacion;
		}
		set
		{
			CDusuarioDigitalizacion = value;
		}
	}

	public DateTime feGeneracionIndice
	{
		get
		{
			return FEgeneracionIndice;
		}
		set
		{
			FEgeneracionIndice = value;
		}
	}

	public int nuCantidadImagenes
	{
		get
		{
			return NUcantidadImagenes;
		}
		set
		{
			NUcantidadImagenes = value;
		}
	}

	public string nuCodigoBarras
	{
		get
		{
			return NUcodigoBarras;
		}
		set
		{
			NUcodigoBarras = value;
		}
	}

	public string nuGuia
	{
		get
		{
			return NUguia;
		}
		set
		{
			NUguia = value;
		}
	}

	public string dsDespacho
	{
		get
		{
			return DSdespacho;
		}
		set
		{
			DSdespacho = value;
		}
	}

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

	public int id
	{
		get
		{
			return ID;
		}
		set
		{
			ID = value;
		}
	}
}
