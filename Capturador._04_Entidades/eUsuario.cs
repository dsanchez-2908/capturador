using System;

namespace Capturador._04_Entidades;

public class eUsuario
{
	private int CdUsuario;

	private string DsUsuario;

	private string DsNombre;

	private string DsApellido;

	private string DsClave;

	private string NuDNI;

	private string DsLegajo;

	private string DsMail;

	private int NuIntentosFallidos;

	private DateTime FeAlta;

	private DateTime FeAltaDesde;

	private DateTime FeAltaHasta;

	private int CdEstado;

	private string DsEstado;

	private int CdRol;

	private string DsRoles;

	private string DsUsuarioNombre;

	public string dsUsuarioNombre
	{
		get
		{
			return DsUsuarioNombre;
		}
		set
		{
			DsUsuarioNombre = value;
		}
	}

	public string dsRoles
	{
		get
		{
			return DsRoles;
		}
		set
		{
			DsRoles = value;
		}
	}

	public int cdRol
	{
		get
		{
			return CdRol;
		}
		set
		{
			CdRol = value;
		}
	}

	public string dsEstado
	{
		get
		{
			return DsEstado;
		}
		set
		{
			DsEstado = value;
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

	public DateTime feAlta
	{
		get
		{
			return FeAlta;
		}
		set
		{
			FeAlta = value;
		}
	}

	public int nuIntentosFallidos
	{
		get
		{
			return NuIntentosFallidos;
		}
		set
		{
			NuIntentosFallidos = value;
		}
	}

	public string dsMail
	{
		get
		{
			return DsMail;
		}
		set
		{
			DsMail = value;
		}
	}

	public string dsLegajo
	{
		get
		{
			return DsLegajo;
		}
		set
		{
			DsLegajo = value;
		}
	}

	public string nuDNI
	{
		get
		{
			return NuDNI;
		}
		set
		{
			NuDNI = value;
		}
	}

	public string dsClave
	{
		get
		{
			return DsClave;
		}
		set
		{
			DsClave = value;
		}
	}

	public string dsApellido
	{
		get
		{
			return DsApellido;
		}
		set
		{
			DsApellido = value;
		}
	}

	public string dsNombre
	{
		get
		{
			return DsNombre;
		}
		set
		{
			DsNombre = value;
		}
	}

	public string dsUsuario
	{
		get
		{
			return DsUsuario;
		}
		set
		{
			DsUsuario = value;
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
}
