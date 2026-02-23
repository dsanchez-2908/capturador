namespace Capturador._04_Entidades;

public class eEstado
{
	private int IdEstado;

	private string DsProceso;

	private int CdEstado;

	private string DsEstado;

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

	public string dsProceso
	{
		get
		{
			return DsProceso;
		}
		set
		{
			DsProceso = value;
		}
	}

	public int idEstado
	{
		get
		{
			return IdEstado;
		}
		set
		{
			IdEstado = value;
		}
	}
}
