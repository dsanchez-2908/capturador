namespace Capturador._04_Entidades;

public class eCodigosBarrasEncontrados_v2
{
	private string carpetaInicial;

	private string nombreLote;

	private string despacho;

	private int numeroPagina;

	private int valorEncontrado;

	public int ValorEncontrado
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
