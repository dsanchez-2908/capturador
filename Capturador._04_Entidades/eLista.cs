namespace Capturador._04_Entidades;

public class eLista
{
	private int CdValor;

	private int CdProyecto;

	private string DsProyecto;

	private int CdCampo;

	private string DsCampo;

	private string CdExterno;

	private string DsValorLista;

	private string SnHabilitado;

	public int cdValor
	{
		get
		{
			return CdValor;
		}
		set
		{
			CdValor = value;
		}
	}

	public string snHabilitado
	{
		get
		{
			return SnHabilitado;
		}
		set
		{
			SnHabilitado = value;
		}
	}

	public string dsValorLista
	{
		get
		{
			return DsValorLista;
		}
		set
		{
			DsValorLista = value;
		}
	}

	public string cdExterno
	{
		get
		{
			return CdExterno;
		}
		set
		{
			CdExterno = value;
		}
	}

	public string dsCampo
	{
		get
		{
			return DsCampo;
		}
		set
		{
			DsCampo = value;
		}
	}

	public int cdCampo
	{
		get
		{
			return CdCampo;
		}
		set
		{
			CdCampo = value;
		}
	}

	public string dsProyecto
	{
		get
		{
			return DsProyecto;
		}
		set
		{
			DsProyecto = value;
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
