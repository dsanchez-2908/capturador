using System;

namespace Capturador._04_Entidades;

public class eIDX_Historia_Clinica
{
    private int CDproyecto;

    private string DSproyecto;

    private int CDlote;

    private string DSnombreLote;

    private int NUcantidadArchivos;

    private string DSrutaLoteFinal;

    private int CDloteDetalle;

    private string DStipoDetalle;

    private string DSnombreArchivo;

    private int NUcantidadPaginaInicial;

    private int NUcantidadPaginasFinal;

    private string NUhistoriaClinica;

    private string NUdni;

    private string DSnombreApellido;

    private DateTime FEnacimiento;

    private DateTime FEalta;

    private int CDusuarioIndexacion;

    private string DSusuarioIndexacion;

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

    public int cdLote
    {
        get
        {
            return CDlote;
        }
        set
        {
            CDlote = value;
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

    public string dsRutaLoteFinal
    {
        get
        {
            return DSrutaLoteFinal;
        }
        set
        {
            DSrutaLoteFinal = value;
        }
    }

    public int cdLoteDetalle
    {
        get
        {
            return CDloteDetalle;
        }
        set
        {
            CDloteDetalle = value;
        }
    }

    public string dsTipoDetalle
    {
        get
        {
            return DStipoDetalle;
        }
        set
        {
            DStipoDetalle = value;
        }
    }

    public string dsNombreArchivo
    {
        get
        {
            return DSnombreArchivo;
        }
        set
        {
            DSnombreArchivo = value;
        }
    }

    public int nuCantidadPaginaInicial
    {
        get
        {
            return NUcantidadPaginaInicial;
        }
        set
        {
            NUcantidadPaginaInicial = value;
        }
    }

    public int nuCantidadPaginasFinal
    {
        get
        {
            return NUcantidadPaginasFinal;
        }
        set
        {
            NUcantidadPaginasFinal = value;
        }
    }

    public string nuHistoriaClinica
    {
        get
        {
            return NUhistoriaClinica;
        }
        set
        {
            NUhistoriaClinica = value;
        }
    }

    public string nuDNI
    {
        get
        {
            return NUdni;
        }
        set
        {
            NUdni = value;
        }
    }

    public string dsNombreApellido
    {
        get
        {
            return DSnombreApellido;
        }
        set
        {
            DSnombreApellido = value;
        }
    }

    public DateTime feNacimiento
    {
        get
        {
            return FEnacimiento;
        }
        set
        {
            FEnacimiento = value;
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

    public string dsUsuarioIndexacion
    {
        get
        {
            return DSusuarioIndexacion;
        }
        set
        {
            DSusuarioIndexacion = value;
        }
    }
}
