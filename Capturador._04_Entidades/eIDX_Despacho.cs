using System;

namespace Capturador._04_Entidades;

public class eIDX_Despacho
{
	private int Id;

	private int CdLote;

	private int CdLoteDetalle;

	private int IdACK;

	private string Despacho;

	private string SerieDocumental;

	private string sigea;

	private string NroGuia;

	private int CdUsuarioDigitalizacion;

	private string DsUsuarioDigitalizacion;

	private int CdOrigen;

	private string dsOrigen;

	private DateTime FeIndexacion;

	private int CdUsuarioIndexacion;

	private string DsUsuarioIndexacion;

	private string DsUsuarioNombreIndexacion;

	public string dsUsuarioNombreIndexacion
	{
		get
		{
			return DsUsuarioNombreIndexacion;
		}
		set
		{
			DsUsuarioNombreIndexacion = value;
		}
	}

	public string dsUsuarioIndexacion
	{
		get
		{
			return DsUsuarioIndexacion;
		}
		set
		{
			DsUsuarioIndexacion = value;
		}
	}

	public int cdUsuarioIndexacion
	{
		get
		{
			return CdUsuarioIndexacion;
		}
		set
		{
			CdUsuarioIndexacion = value;
		}
	}

	public DateTime feIndexacion
	{
		get
		{
			return FeIndexacion;
		}
		set
		{
			FeIndexacion = value;
		}
	}

	public string MyProperty
	{
		get
		{
			return dsOrigen;
		}
		set
		{
			dsOrigen = value;
		}
	}

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

	public string dsUsuarioDigitalizacion
	{
		get
		{
			return DsUsuarioDigitalizacion;
		}
		set
		{
			DsUsuarioDigitalizacion = value;
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

	public string nroGuia
	{
		get
		{
			return NroGuia;
		}
		set
		{
			NroGuia = value;
		}
	}

	public string SIGEA
	{
		get
		{
			return sigea;
		}
		set
		{
			sigea = value;
		}
	}

	public string serieDocumental
	{
		get
		{
			return SerieDocumental;
		}
		set
		{
			SerieDocumental = value;
		}
	}

	public string despacho
	{
		get
		{
			return Despacho;
		}
		set
		{
			Despacho = value;
		}
	}

	public int idACK
	{
		get
		{
			return IdACK;
		}
		set
		{
			IdACK = value;
		}
	}

	public int cdLoteDetalle
	{
		get
		{
			return CdLoteDetalle;
		}
		set
		{
			CdLoteDetalle = value;
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

	public int id
	{
		get
		{
			return Id;
		}
		set
		{
			Id = value;
		}
	}
}
