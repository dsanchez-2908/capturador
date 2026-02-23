using System;

namespace Capturador._04_Entidades;

public class eLoteConsultaDespacho
{
	private int CdLote;

	private string DsNombreLote;

	private int CdUsuario;

	private int CdEstado;

	private DateTime FeAltaDesde;

	private DateTime FeAltaHasta;

	private DateTime FeFinalizacionDesde;

	private DateTime FeFinalizacionHasta;

	private string DsDespacho;

	private string NuGuia;

	private string DsSerieDocumental;

	private string DsSIGEA;

	private int CdUsuarioDigitalizacion;

	private int CdOrigen;

	public int cdOrigen
	{
		get
		{
			return CdOrigen;
		}
		set
		{
			CdOrigen = value;
		}
	}

	public int cdUsuarioDigitalizacion
	{
		get
		{
			return CdUsuarioDigitalizacion;
		}
		set
		{
			CdUsuarioDigitalizacion = value;
		}
	}

	public string dsSIGEA
	{
		get
		{
			return DsSIGEA;
		}
		set
		{
			DsSIGEA = value;
		}
	}

	public string dsSerieDocumental
	{
		get
		{
			return DsSerieDocumental;
		}
		set
		{
			DsSerieDocumental = value;
		}
	}

	public string nuGuia
	{
		get
		{
			return NuGuia;
		}
		set
		{
			NuGuia = value;
		}
	}

	public string dsDespacho
	{
		get
		{
			return DsDespacho;
		}
		set
		{
			DsDespacho = value;
		}
	}

	public DateTime feFinalizacionHasta
	{
		get
		{
			return FeFinalizacionHasta;
		}
		set
		{
			FeFinalizacionHasta = value;
		}
	}

	public DateTime feFinalizacionDesde
	{
		get
		{
			return FeFinalizacionDesde;
		}
		set
		{
			FeFinalizacionDesde = value;
		}
	}

	public DateTime feAltaHasta
	{
		get
		{
			return FeAltaHasta;
		}
		set
		{
			FeAltaHasta = value;
		}
	}

	public DateTime feAltaDesde
	{
		get
		{
			return FeAltaDesde;
		}
		set
		{
			FeAltaDesde = value;
		}
	}

	public int cdEstado
	{
		get
		{
			return CdEstado;
		}
		set
		{
			CdEstado = value;
		}
	}

	public int cdUsuario
	{
		get
		{
			return CdUsuario;
		}
		set
		{
			CdUsuario = value;
		}
	}

	public string dsNombreLote
	{
		get
		{
			return DsNombreLote;
		}
		set
		{
			DsNombreLote = value;
		}
	}

	public int cdLote
	{
		get
		{
			return CdLote;
		}
		set
		{
			CdLote = value;
		}
	}
}
