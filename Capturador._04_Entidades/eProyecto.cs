namespace Capturador._04_Entidades;

public class eProyecto
{
	private int CdProyecto;

	private string DsProyecto;

	private string CdCliente;

	private string DsCliente;

	private string DsOperatoria;

	private string SnHabilitado;

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

	public string dsOperatoria
	{
		get
		{
			return DsOperatoria;
		}
		set
		{
			DsOperatoria = value;
		}
	}

	public string dsCliente
	{
		get
		{
			return DsCliente;
		}
		set
		{
			DsCliente = value;
		}
	}

	public string cdCliente
	{
		get
		{
			return CdCliente;
		}
		set
		{
			CdCliente = value;
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
