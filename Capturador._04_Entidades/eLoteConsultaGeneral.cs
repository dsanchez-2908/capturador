using System;

namespace Capturador._04_Entidades;

public class eLoteConsultaGeneral
{
	private int CdProyecto;

	private int CdLote;

	private string DsNombreLote;

	private int CdUsuario;

	private int CdEstado;

	private DateTime FeAltaDesde;

	private DateTime FeAltaHasta;

	private DateTime FeControlCalidadDesde;

	private DateTime FeControlCalidadHasta;

	private DateTime FeIndexacionDesde;

	private DateTime FeIndexacionHasta;

	private DateTime FeSeparacionDesde;

	private DateTime FeSeparacionHasta;

	private DateTime FeOCRDesde;

	private DateTime FeOCRHasta;

	private DateTime FeFinalizacionDesde;

	private DateTime FeFinalizacionHasta;

	public DateTime feOCRHasta
	{
		get
		{
			return FeOCRHasta;
		}
		set
		{
			FeOCRHasta = value;
		}
	}

	public DateTime feOCRDesde
	{
		get
		{
			return FeOCRDesde;
		}
		set
		{
			FeOCRDesde = value;
		}
	}

	public DateTime feSeparacionHasta
	{
		get
		{
			return FeSeparacionHasta;
		}
		set
		{
			FeSeparacionHasta = value;
		}
	}

	public DateTime feSeparacionDesde
	{
		get
		{
			return FeSeparacionDesde;
		}
		set
		{
			FeSeparacionDesde = value;
		}
	}

	public DateTime feIndexacionHasta
	{
		get
		{
			return FeIndexacionHasta;
		}
		set
		{
			FeIndexacionHasta = value;
		}
	}

	public DateTime feIndexacionDesde
	{
		get
		{
			return FeIndexacionDesde;
		}
		set
		{
			FeIndexacionDesde = value;
		}
	}

	public DateTime feControlCalidadHasta
	{
		get
		{
			return FeControlCalidadHasta;
		}
		set
		{
			FeControlCalidadHasta = value;
		}
	}

	public DateTime feControlCalidadDesde
	{
		get
		{
			return FeControlCalidadDesde;
		}
		set
		{
			FeControlCalidadDesde = value;
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
