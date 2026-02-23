namespace Capturador._04_Entidades;

public class eListaCampos
{
	private int CdProyecto;

	private int CdCampo;

	private string DsCampo;

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
