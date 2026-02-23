using System;

namespace Capturador._04_Entidades;

public class eLote
{
	private int CDLote;

	private int CDproyecto;

	private string DSnombreLote;

	private string DSrutaLote;

	private int NUcantidadArchivos;

	private DateTime FEalta;

	private int CDusuarioAlta;

	private DateTime FEpreparado;

	private int CDusuarioPreparado;

	private DateTime FEcontrolCalidad;

	private int CDusuarioControlCalidad;

	private DateTime FEindexacion;

	private int CDusuarioIndexacion;

	private DateTime FErestaurado;

	private int CDusuarioRestaurado;

	private DateTime FEsalida;

	private int CDusuarioSalida;

	private int CDestado;

	private string DSusuarioAlta;

	private string DSestado;

	private string DSproyecto;

	private string DsRutaLoteFinal;

	public string dsRutaLoteFinal
	{
		get
		{
			return DsRutaLoteFinal;
		}
		set
		{
			DsRutaLoteFinal = value;
		}
	}

	public string dsProyecto
	{
		get
		{
			return DSproyecto;
		}
		set
		{
			DSproyecto = value;
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

	public string dsUsuarioAlta
	{
		get
		{
			return DSusuarioAlta;
		}
		set
		{
			DSusuarioAlta = value;
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

	public int cdUsuarioSalida
	{
		get
		{
			return CDusuarioSalida;
		}
		set
		{
			CDusuarioSalida = value;
		}
	}

	public DateTime feSalida
	{
		get
		{
			return FEsalida;
		}
		set
		{
			FEsalida = value;
		}
	}

	public int cdUsuarioRestaurado
	{
		get
		{
			return CDusuarioRestaurado;
		}
		set
		{
			CDusuarioRestaurado = value;
		}
	}

	public DateTime feRestaurado
	{
		get
		{
			return FErestaurado;
		}
		set
		{
			FErestaurado = value;
		}
	}

	public int cdUsuarioIndexacion
	{
		get
		{
			return CDusuarioIndexacion;
		}
		set
		{
			CDusuarioIndexacion = value;
		}
	}

	public DateTime feIndexacion
	{
		get
		{
			return FEindexacion;
		}
		set
		{
			FEindexacion = value;
		}
	}

	public int cdUsuarioControlCalidad
	{
		get
		{
			return CDusuarioControlCalidad;
		}
		set
		{
			CDusuarioControlCalidad = value;
		}
	}

	public DateTime feControlCalidad
	{
		get
		{
			return FEcontrolCalidad;
		}
		set
		{
			FEcontrolCalidad = value;
		}
	}

	public int cdUsuarioPreparado
	{
		get
		{
			return CDusuarioPreparado;
		}
		set
		{
			CDusuarioPreparado = value;
		}
	}

	public DateTime fePreparado
	{
		get
		{
			return FEpreparado;
		}
		set
		{
			FEpreparado = value;
		}
	}

	public int cdUsuarioAlta
	{
		get
		{
			return CDusuarioAlta;
		}
		set
		{
			CDusuarioAlta = value;
		}
	}

	public DateTime feAlta
	{
		get
		{
			return FEalta;
		}
		set
		{
			FEalta = value;
		}
	}

	public int nuCantidadArchivos
	{
		get
		{
			return NUcantidadArchivos;
		}
		set
		{
			NUcantidadArchivos = value;
		}
	}

	public string dsRutaLote
	{
		get
		{
			return DSrutaLote;
		}
		set
		{
			DSrutaLote = value;
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

	public int cdProyecto
	{
		get
		{
			return CDproyecto;
		}
		set
		{
			CDproyecto = value;
		}
	}

	public int cdLote
	{
		get
		{
			return CDLote;
		}
		set
		{
			CDLote = value;
		}
	}
}
