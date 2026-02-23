namespace Capturador._04_Entidades;

public class eCodigosBarrasEncontrados_v3
{
	private string carpetaInicial;

	private string nombreLote;

	private string despacho;

	private int numeroPagina;

	private string valorEncontrado;

	public string ValorEncontrado
	{
		get
		{
			return valorEncontrado;
		}
		set
		{
			valorEncontrado = value;
		}
	}

	public int NumeroPagina
	{
		get
		{
			return numeroPagina;
		}
		set
		{
			numeroPagina = value;
		}
	}

	public string Despacho
	{
		get
		{
			return despacho;
		}
		set
		{
			despacho = value;
		}
	}

	public string NombreLote
	{
		get
		{
			return nombreLote;
		}
		set
		{
			nombreLote = value;
		}
	}

	public string CarpetaInicial
	{
		get
		{
			return carpetaInicial;
		}
		set
		{
			carpetaInicial = value;
		}
	}
}
