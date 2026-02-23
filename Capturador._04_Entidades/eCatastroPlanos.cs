using System;

namespace Capturador._04_Entidades;

public class eCatastroPlanos
{
	private int Id;

	private DateTime FeAlta;

	private int CdUsuarioAlta;

	private string DsUsuarioAlta;

	private string CdDocumento;

	private string DsNombreArchivo;

	private DateTime FeCopiado;

	private int CdUsuarioCopiado;

	private string DsUsuarioCopiado;

	private int CdEstado;

	private string DsEstado;

	private int NuSecuencia;

	public int nuSecuencia
	{
		get
		{
			return NuSecuencia;
		}
		set
		{
			NuSecuencia = value;
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

	public string dsUsuarioCopiado
	{
		get
		{
			return DsUsuarioCopiado;
		}
		set
		{
			DsUsuarioCopiado = value;
		}
	}

	public int cdUsuarioCopiado
	{
		get
		{
			return CdUsuarioCopiado;
		}
		set
		{
			CdUsuarioCopiado = value;
		}
	}

	public DateTime feCopiado
	{
		get
		{
			return FeCopiado;
		}
		set
		{
			FeCopiado = value;
		}
	}

	public string dsNombreArchivo
	{
		get
		{
			return DsNombreArchivo;
		}
		set
		{
			DsNombreArchivo = value;
		}
	}

	public string cdDocumento
	{
		get
		{
			return CdDocumento;
		}
		set
		{
			CdDocumento = value;
		}
	}

	public string dsUsuarioAlta
	{
		get
		{
			return DsUsuarioAlta;
		}
		set
		{
			DsUsuarioAlta = value;
		}
	}

	public int cdUsuarioAlta
	{
		get
		{
			return CdUsuarioAlta;
		}
		set
		{
			CdUsuarioAlta = value;
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
