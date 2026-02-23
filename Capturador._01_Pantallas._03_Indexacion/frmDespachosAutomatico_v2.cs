using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmDespachosAutomatico_v2 : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string rutaLote;

	private int totalArchivosEncontrados;

	private string rutaCarpetaInicial;

	private List<eDespacho> ListaFinalDespachosEncontrados = new List<eDespacho>();

	private List<eDespacho> ListaFinalDespachosEncontradosMultiples = new List<eDespacho>();

	private List<string> ListaFinalDespachosNoEncontrados = new List<string>();

	private eLote oLoteSeleccionado = new eLote();

	private IContainer components = null;

	private GroupBox groupBox6;

	private TextBox txtDespachoSeleccionado;

	private Label label1;

	private ComboBox cbxUsuarioDigitalizacion;

	private Label label2;

	private ComboBox cbxOrigen;

	private Panel pnlLotesDisponibles;

	private DataGridView dgvLotesDisponibles;

	private Panel pnlLoteSeleccionado;

	private ListBox lboxArchivosEncontrados;

	private TextBox txtRutaLote;

	private TextBox txtLoteSeleccionado;

	private TextBox txtTotalArchivosSeleccionado;

	private TextBox txtProyectoSeleccionado;

	private Label lblNombreLote;

	private Label lblCarpetaLote;

	private Label lblProyeto;

	private Label lblTotalArchivos;

	private Button btnCerrar;

	private Button btnCancelar;

	private Button btnCargarLote;

	private Panel pnlDespachosEncontrados;

	private DataGridView dgvDespachosEncontrados;

	private Panel pnlDespachosEncontradosMultiple;

	private DataGridView dgvDespachosEncontradosMultiple;

	private Panel pnlDespachosNoEncontrados;

	private DataGridView dgvDespachosNoEncontrados;

	private Button btnGenerarIndice;

	private Button btnVerDespacho;

	public frmDespachosAutomatico_v2(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
	}

	private void frmDespachosAutomatico_Load(object sender, EventArgs e)
	{
		actualizarLotesDisponibles();
		ajustarPanelLoteSeleccionado();
		llenarListas();
		eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		ajustarFormularioInicial();
	}

	private void llenarListas()
	{
		cbxUsuarioDigitalizacion.DataSource = nIndexacion.ObtenerLista(oUsuarioLogueado, 1, 1);
		cbxUsuarioDigitalizacion.DisplayMember = "dsValorLista";
		cbxUsuarioDigitalizacion.ValueMember = "cdValor";
		cbxUsuarioDigitalizacion.SelectedIndex = -1;
		cbxOrigen.DataSource = nIndexacion.ObtenerLista(oUsuarioLogueado, 1, 2);
		cbxOrigen.DisplayMember = "dsValorLista";
		cbxOrigen.ValueMember = "cdValor";
		cbxOrigen.SelectedIndex = -1;
	}

	private void actualizarLotesDisponibles()
	{
		dgvLotesDisponibles.DataSource = nLotes.obtenerLotesDisponibleIndexacion(oUsuarioLogueado, 1);
		dgvLotesDisponibles.Columns["dsProyecto"].HeaderText = "Proyecto";
		dgvLotesDisponibles.Columns["dsNombreLote"].HeaderText = "Lote";
		dgvLotesDisponibles.Columns["nuCantidadArchivos"].HeaderText = "Cantidad Archivos";
		dgvLotesDisponibles.Columns["feAlta"].HeaderText = "Fecha de Alta";
		dgvLotesDisponibles.Columns["dsProyecto"].DisplayIndex = 0;
		dgvLotesDisponibles.Columns["dsNombreLote"].DisplayIndex = 1;
		dgvLotesDisponibles.Columns["nuCantidadArchivos"].DisplayIndex = 2;
		dgvLotesDisponibles.Columns["feAlta"].DisplayIndex = 3;
		dgvLotesDisponibles.Columns["dsRutaLote"].Visible = false;
		dgvLotesDisponibles.Columns["cdEstado"].Visible = false;
		dgvLotesDisponibles.Columns["cdUsuarioSalida"].Visible = false;
		dgvLotesDisponibles.Columns["feSalida"].Visible = false;
		dgvLotesDisponibles.Columns["cdUsuarioRestaurado"].Visible = false;
		dgvLotesDisponibles.Columns["feRestaurado"].Visible = false;
		dgvLotesDisponibles.Columns["cdUsuarioIndexacion"].Visible = false;
		dgvLotesDisponibles.Columns["feIndexacion"].Visible = false;
		dgvLotesDisponibles.Columns["cdUsuarioControlCalidad"].Visible = false;
		dgvLotesDisponibles.Columns["feControlCalidad"].Visible = false;
		dgvLotesDisponibles.Columns["cdUsuarioPreparado"].Visible = false;
		dgvLotesDisponibles.Columns["fePreparado"].Visible = false;
		dgvLotesDisponibles.Columns["cdUsuarioAlta"].Visible = false;
		dgvLotesDisponibles.Columns["cdProyecto"].Visible = false;
		dgvLotesDisponibles.Columns["cdLote"].Visible = false;
		dgvLotesDisponibles.Columns["dsEstado"].Visible = false;
		dgvLotesDisponibles.Columns["dsUsuarioAlta"].Visible = false;
		dgvLotesDisponibles.Columns["dsRutaLoteFinal"].Visible = false;
		dgvLotesDisponibles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Lotes Disponibles";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvLotesDisponibles.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Lotes: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvLotesDisponibles.Dock = DockStyle.Fill;
		pnlLotesDisponibles.Controls.Clear();
		pnlLotesDisponibles.Controls.Add(dgvLotesDisponibles);
		pnlLotesDisponibles.Controls.Add(labelTitulo);
		pnlLotesDisponibles.Controls.Add(labelTotal);
	}

	private void ajustarPanelLoteSeleccionado()
	{
		Label labelTituloLoteSeleccionado = new Label();
		labelTituloLoteSeleccionado.Text = "Lote Seleccionado";
		labelTituloLoteSeleccionado.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTituloLoteSeleccionado.BackColor = Color.FromArgb(26, 32, 40);
		labelTituloLoteSeleccionado.ForeColor = Color.White;
		labelTituloLoteSeleccionado.Dock = DockStyle.Top;
		labelTituloLoteSeleccionado.Height = 30;
		labelTituloLoteSeleccionado.TextAlign = ContentAlignment.MiddleCenter;
		pnlLoteSeleccionado.Controls.Add(labelTituloLoteSeleccionado);
	}

	private void btnCerrar_Click_1(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click_1(object sender, EventArgs e)
	{
		deshabilitarFormulario();
	}

	private void btnCargarLote_Click_1(object sender, EventArgs e)
	{
		cargarLoteSeleccionado();
	}

	private void dgvLotesDisponibles_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		cargarLoteSeleccionado();
	}

	private void cargarLoteSeleccionado()
	{
		if (dgvLotesDisponibles.SelectedRows.Count == 0)
		{
			MessageBox.Show("Debe seleccionar un registros de los lotes disponibles", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		btnCargarLote.BackColor = Color.DarkGray;
		btnCargarLote.Enabled = false;
		dgvLotesDisponibles.Enabled = false;
		btnCancelar.Enabled = true;
		btnCancelar.BackColor = Color.Salmon;
		base.MaximizeBox = true;
		oLoteSeleccionado.cdLote = Convert.ToInt32(dgvLotesDisponibles.SelectedRows[0].Cells["cdLote"].Value.ToString());
		txtProyectoSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsProyecto"].Value.ToString();
		txtLoteSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsNombreLote"].Value.ToString();
		txtTotalArchivosSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["nuCantidadArchivos"].Value.ToString();
		txtRutaLote.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsRutaLote"].Value.ToString();
		rutaLote = txtRutaLote.Text;
		List<string> listaArchivosEncontrados = null;
		try
		{
			listaArchivosEncontrados = nIndexacion.ObtenerNombreArchivos(txtRutaLote.Text);
			lboxArchivosEncontrados.DataSource = listaArchivosEncontrados;
			totalArchivosEncontrados = lboxArchivosEncontrados.Items.Count;
			procesarArchivosEncontrados();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			deshabilitarFormulario();
			actualizarLotesDisponibles();
		}
	}

	private void procesarArchivosEncontrados()
	{
		foreach (object archivoEncontrado in lboxArchivosEncontrados.Items)
		{
			string archivo = Path.GetFileNameWithoutExtension(Convert.ToString(archivoEncontrado));
			AgregarResultado(archivo);
		}
		cargarGrillasResultadoBusqueda();
		ajustarFormularioCompleto();
	}

	private void AgregarResultado(string pDespacho)
	{
		List<eDespacho> oListaDespachoResultadoBuscada = nIndexacion.BuscarDespacho2(oUsuarioLogueado, pDespacho);
		if (oListaDespachoResultadoBuscada.Count == 0)
		{
			ListaFinalDespachosNoEncontrados.Add(pDespacho);
		}
		else if (oListaDespachoResultadoBuscada.Count == 1)
		{
			ListaFinalDespachosEncontrados.AddRange(oListaDespachoResultadoBuscada);
		}
		else
		{
			ListaFinalDespachosEncontradosMultiples.AddRange(oListaDespachoResultadoBuscada);
		}
	}

	private void cargarGrillasResultadoBusqueda()
	{
		pnlDespachosEncontrados.Visible = true;
		if (ListaFinalDespachosEncontrados.Count > 0)
		{
			dgvDespachosEncontrados.DataSource = ListaFinalDespachosEncontrados;
		}
		else
		{
			List<eDespacho> ListaDespachosVacios = new List<eDespacho>();
			dgvDespachosEncontrados.DataSource = ListaDespachosVacios;
		}
		dgvDespachosEncontrados.Columns["id"].HeaderText = "id";
		dgvDespachosEncontrados.Columns["dsDespacho"].HeaderText = "Despacho";
		dgvDespachosEncontrados.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		dgvDespachosEncontrados.Columns["nuSIGEA"].HeaderText = "SIGEA";
		dgvDespachosEncontrados.Columns["nuGuia"].HeaderText = "Nro. Guia";
		dgvDespachosEncontrados.Columns["id"].DisplayIndex = 0;
		dgvDespachosEncontrados.Columns["dsDespacho"].DisplayIndex = 1;
		dgvDespachosEncontrados.Columns["cdSerieDocumental"].DisplayIndex = 2;
		dgvDespachosEncontrados.Columns["nuSIGEA"].DisplayIndex = 3;
		dgvDespachosEncontrados.Columns["nuGuia"].DisplayIndex = 4;
		dgvDespachosEncontrados.Columns["cdEstado"].Visible = false;
		dgvDespachosEncontrados.Columns["dsEstado"].Visible = false;
		dgvDespachosEncontrados.Columns["nuCodigoBarras"].Visible = false;
		dgvDespachosEncontrados.Columns["nuCantidadImagenes"].Visible = false;
		dgvDespachosEncontrados.Columns["feGeneracionIndice"].Visible = false;
		dgvDespachosEncontrados.Columns["cdUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontrados.Columns["dsUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontrados.Columns["feFinalizacionProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["nuEstacionTrabajo"].Visible = false;
		dgvDespachosEncontrados.Columns["cdUsuarioProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["dsUsuarioProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["nuImagen"].Visible = false;
		dgvDespachosEncontrados.Columns["dsNombreLote"].Visible = false;
		dgvDespachosEncontrados.Columns["dsRutaArchivoPDF"].Visible = false;
		Label labelTituloDespachosEncontrados = new Label();
		labelTituloDespachosEncontrados.Text = "Despachos Encontrados";
		labelTituloDespachosEncontrados.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTituloDespachosEncontrados.BackColor = Color.FromArgb(26, 32, 40);
		labelTituloDespachosEncontrados.ForeColor = Color.White;
		labelTituloDespachosEncontrados.Dock = DockStyle.Top;
		labelTituloDespachosEncontrados.Height = 30;
		labelTituloDespachosEncontrados.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvDespachosEncontrados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Despachos: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvDespachosEncontrados.Dock = DockStyle.Fill;
		pnlDespachosEncontrados.Controls.Clear();
		pnlDespachosEncontrados.Controls.Add(dgvDespachosEncontrados);
		pnlDespachosEncontrados.Controls.Add(labelTituloDespachosEncontrados);
		pnlDespachosEncontrados.Controls.Add(labelTotal);
		if (ListaFinalDespachosEncontradosMultiples.Count > 0)
		{
			pnlDespachosEncontradosMultiple.Visible = true;
			dgvDespachosEncontradosMultiple.DataSource = ListaFinalDespachosEncontradosMultiples;
			dgvDespachosEncontradosMultiple.Columns["id"].HeaderText = "id";
			dgvDespachosEncontradosMultiple.Columns["dsDespacho"].HeaderText = "Despacho";
			dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
			dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].HeaderText = "SIGEA";
			dgvDespachosEncontradosMultiple.Columns["nuGuia"].HeaderText = "Nro. Guia";
			dgvDespachosEncontradosMultiple.Columns["id"].DisplayIndex = 0;
			dgvDespachosEncontradosMultiple.Columns["dsDespacho"].DisplayIndex = 1;
			dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].DisplayIndex = 2;
			dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].DisplayIndex = 3;
			dgvDespachosEncontradosMultiple.Columns["nuGuia"].DisplayIndex = 4;
			dgvDespachosEncontradosMultiple.Columns["cdEstado"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["dsEstado"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["nuCodigoBarras"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["nuCantidadImagenes"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["feGeneracionIndice"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["cdUsuarioDigitalizacion"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["dsUsuarioDigitalizacion"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["feFinalizacionProceso"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["nuEstacionTrabajo"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["cdUsuarioProceso"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["dsUsuarioProceso"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["nuImagen"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["dsNombreLote"].Visible = false;
			dgvDespachosEncontradosMultiple.Columns["dsRutaArchivoPDF"].Visible = false;
			Label labelTituloDespachosEncontradosMultiples = new Label();
			labelTituloDespachosEncontradosMultiples.Text = "Despachos Encontrados (Repetidos)";
			labelTituloDespachosEncontradosMultiples.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
			labelTituloDespachosEncontradosMultiples.BackColor = Color.FromArgb(26, 32, 40);
			labelTituloDespachosEncontradosMultiples.ForeColor = Color.White;
			labelTituloDespachosEncontradosMultiples.Dock = DockStyle.Top;
			labelTituloDespachosEncontradosMultiples.Height = 30;
			labelTituloDespachosEncontradosMultiples.TextAlign = ContentAlignment.MiddleCenter;
			dgvDespachosEncontradosMultiple.Dock = DockStyle.Fill;
			pnlDespachosEncontradosMultiple.Controls.Clear();
			pnlDespachosEncontradosMultiple.Controls.Add(dgvDespachosEncontradosMultiple);
			pnlDespachosEncontradosMultiple.Controls.Add(labelTituloDespachosEncontradosMultiples);
		}
		if (ListaFinalDespachosNoEncontrados.Count > 0)
		{
			pnlDespachosNoEncontrados.Visible = true;
			dgvDespachosNoEncontrados.AutoGenerateColumns = false;
			DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
			column1.HeaderText = "Despachos";
			column1.Name = "Despachos";
			column1.DataPropertyName = "Valor";
			dgvDespachosNoEncontrados.Columns.Add(column1);
			dgvDespachosNoEncontrados.DataSource = ListaFinalDespachosNoEncontrados.Select((string x) => new
			{
				Valor = x
			}).ToList();
			Label labelTituloDespachosNoEncontrados = new Label();
			labelTituloDespachosNoEncontrados.Text = "Despachos No Encontrados";
			labelTituloDespachosNoEncontrados.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
			labelTituloDespachosNoEncontrados.BackColor = Color.FromArgb(26, 32, 40);
			labelTituloDespachosNoEncontrados.ForeColor = Color.White;
			labelTituloDespachosNoEncontrados.Dock = DockStyle.Top;
			labelTituloDespachosNoEncontrados.Height = 30;
			labelTituloDespachosNoEncontrados.TextAlign = ContentAlignment.MiddleCenter;
			dgvDespachosNoEncontrados.Dock = DockStyle.Fill;
			pnlDespachosNoEncontrados.Controls.Clear();
			pnlDespachosNoEncontrados.Controls.Add(dgvDespachosNoEncontrados);
			pnlDespachosNoEncontrados.Controls.Add(labelTituloDespachosNoEncontrados);
		}
	}

	private void actualizarGrillaEncontrados()
	{
		pnlDespachosEncontrados.Visible = true;
		List<eDespacho> oDespachosEncontradosActualizar = (List<eDespacho>)dgvDespachosEncontrados.DataSource;
		dgvDespachosEncontrados.DataSource = oDespachosEncontradosActualizar;
		dgvDespachosEncontrados.Columns["id"].HeaderText = "id";
		dgvDespachosEncontrados.Columns["dsDespacho"].HeaderText = "Despacho";
		dgvDespachosEncontrados.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		dgvDespachosEncontrados.Columns["nuSIGEA"].HeaderText = "SIGEA";
		dgvDespachosEncontrados.Columns["nuGuia"].HeaderText = "Nro. Guia";
		dgvDespachosEncontrados.Columns["id"].DisplayIndex = 0;
		dgvDespachosEncontrados.Columns["dsDespacho"].DisplayIndex = 1;
		dgvDespachosEncontrados.Columns["cdSerieDocumental"].DisplayIndex = 2;
		dgvDespachosEncontrados.Columns["nuSIGEA"].DisplayIndex = 3;
		dgvDespachosEncontrados.Columns["nuGuia"].DisplayIndex = 4;
		dgvDespachosEncontrados.Columns["cdEstado"].Visible = false;
		dgvDespachosEncontrados.Columns["dsEstado"].Visible = false;
		dgvDespachosEncontrados.Columns["nuCodigoBarras"].Visible = false;
		dgvDespachosEncontrados.Columns["nuCantidadImagenes"].Visible = false;
		dgvDespachosEncontrados.Columns["feGeneracionIndice"].Visible = false;
		dgvDespachosEncontrados.Columns["cdUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontrados.Columns["dsUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontrados.Columns["feFinalizacionProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["nuEstacionTrabajo"].Visible = false;
		dgvDespachosEncontrados.Columns["cdUsuarioProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["dsUsuarioProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["nuImagen"].Visible = false;
		dgvDespachosEncontrados.Columns["dsNombreLote"].Visible = false;
		dgvDespachosEncontrados.Columns["dsRutaArchivoPDF"].Visible = false;
		Label labelTituloDespachosEncontrados = new Label();
		labelTituloDespachosEncontrados.Text = "Despachos Encontrados";
		labelTituloDespachosEncontrados.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTituloDespachosEncontrados.BackColor = Color.FromArgb(26, 32, 40);
		labelTituloDespachosEncontrados.ForeColor = Color.White;
		labelTituloDespachosEncontrados.Dock = DockStyle.Top;
		labelTituloDespachosEncontrados.Height = 30;
		labelTituloDespachosEncontrados.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvDespachosEncontrados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Despachos: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvDespachosEncontrados.Dock = DockStyle.Fill;
		pnlDespachosEncontrados.Controls.Clear();
		pnlDespachosEncontrados.Controls.Add(dgvDespachosEncontrados);
		pnlDespachosEncontrados.Controls.Add(labelTituloDespachosEncontrados);
		pnlDespachosEncontrados.Controls.Add(labelTotal);
	}

	private void dgvDespachosEncontradosMultiple_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		string id = dgvDespachosEncontradosMultiple.CurrentRow.Cells["id"].Value.ToString();
		string dsDespacho = dgvDespachosEncontradosMultiple.CurrentRow.Cells["dsDespacho"].Value.ToString();
		string nuSerieDocumental = dgvDespachosEncontradosMultiple.CurrentRow.Cells["cdSerieDocumental"].Value.ToString();
		string nuSIGEA = dgvDespachosEncontradosMultiple.CurrentRow.Cells["nuSIGEA"].Value.ToString();
		string nuGuia = dgvDespachosEncontradosMultiple.CurrentRow.Cells["nuGuia"].Value.ToString();
		eDespacho despachoAgregar = new eDespacho();
		despachoAgregar.id = Convert.ToInt32(id);
		despachoAgregar.dsDespacho = dsDespacho;
		despachoAgregar.cdSerieDocumental = nuSerieDocumental;
		despachoAgregar.nuSIGEA = nuSIGEA;
		despachoAgregar.nuGuia = nuGuia;
		List<eDespacho> oDespachosEncontradosActual = (List<eDespacho>)dgvDespachosEncontrados.DataSource;
		oDespachosEncontradosActual.Add(despachoAgregar);
		dgvDespachosEncontrados.DataSource = null;
		dgvDespachosEncontrados.DataSource = oDespachosEncontradosActual;
		dgvDespachosEncontrados.Columns["id"].HeaderText = "id";
		dgvDespachosEncontrados.Columns["dsDespacho"].HeaderText = "Despacho";
		dgvDespachosEncontrados.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		dgvDespachosEncontrados.Columns["nuSIGEA"].HeaderText = "SIGEA";
		dgvDespachosEncontrados.Columns["nuGuia"].HeaderText = "Nro. Guia";
		dgvDespachosEncontrados.Columns["id"].DisplayIndex = 0;
		dgvDespachosEncontrados.Columns["dsDespacho"].DisplayIndex = 1;
		dgvDespachosEncontrados.Columns["cdSerieDocumental"].DisplayIndex = 2;
		dgvDespachosEncontrados.Columns["nuSIGEA"].DisplayIndex = 3;
		dgvDespachosEncontrados.Columns["nuGuia"].DisplayIndex = 4;
		dgvDespachosEncontrados.Columns["cdEstado"].Visible = false;
		dgvDespachosEncontrados.Columns["dsEstado"].Visible = false;
		dgvDespachosEncontrados.Columns["nuCodigoBarras"].Visible = false;
		dgvDespachosEncontrados.Columns["nuCantidadImagenes"].Visible = false;
		dgvDespachosEncontrados.Columns["feGeneracionIndice"].Visible = false;
		dgvDespachosEncontrados.Columns["cdUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontrados.Columns["dsUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontrados.Columns["feFinalizacionProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["nuEstacionTrabajo"].Visible = false;
		dgvDespachosEncontrados.Columns["cdUsuarioProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["dsUsuarioProceso"].Visible = false;
		dgvDespachosEncontrados.Columns["nuImagen"].Visible = false;
		dgvDespachosEncontrados.Columns["dsNombreLote"].Visible = false;
		dgvDespachosEncontrados.Columns["dsRutaArchivoPDF"].Visible = false;
		dgvDespachosEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		List<eDespacho> oDespachosEncontradosMultiplesActual = (List<eDespacho>)dgvDespachosEncontradosMultiple.DataSource;
		for (int i = oDespachosEncontradosMultiplesActual.Count - 1; i >= 0; i--)
		{
			if (oDespachosEncontradosMultiplesActual[i].dsDespacho == dsDespacho)
			{
				oDespachosEncontradosMultiplesActual.RemoveAt(i);
			}
		}
		dgvDespachosEncontradosMultiple.DataSource = null;
		dgvDespachosEncontradosMultiple.DataSource = oDespachosEncontradosMultiplesActual;
		dgvDespachosEncontradosMultiple.Columns["id"].HeaderText = "id";
		dgvDespachosEncontradosMultiple.Columns["dsDespacho"].HeaderText = "Despacho";
		dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].HeaderText = "SIGEA";
		dgvDespachosEncontradosMultiple.Columns["nuGuia"].HeaderText = "Nro. Guia";
		dgvDespachosEncontradosMultiple.Columns["id"].DisplayIndex = 0;
		dgvDespachosEncontradosMultiple.Columns["dsDespacho"].DisplayIndex = 1;
		dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].DisplayIndex = 2;
		dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].DisplayIndex = 3;
		dgvDespachosEncontradosMultiple.Columns["nuGuia"].DisplayIndex = 4;
		dgvDespachosEncontradosMultiple.Columns["cdEstado"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["dsEstado"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["nuCodigoBarras"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["nuCantidadImagenes"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["feGeneracionIndice"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["cdUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["dsUsuarioDigitalizacion"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["feFinalizacionProceso"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["nuEstacionTrabajo"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["cdUsuarioProceso"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["dsUsuarioProceso"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["nuImagen"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["dsNombreLote"].Visible = false;
		dgvDespachosEncontradosMultiple.Columns["dsRutaArchivoPDF"].Visible = false;
		actualizarGrillaEncontrados();
	}

	private void btnGenerarIndice_Click_1(object sender, EventArgs e)
	{
		int totalRegistosEncontrados = dgvDespachosEncontrados.Rows.Count;
		string valorUsuarioDigitalizacion = cbxUsuarioDigitalizacion.Text;
		string origen = cbxOrigen.Text;
		if (totalArchivosEncontrados != totalRegistosEncontrados)
		{
			MessageBox.Show("No coincide la cantidad de archivos con los despachos encontrados", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else if (string.IsNullOrEmpty(valorUsuarioDigitalizacion))
		{
			MessageBox.Show("Seleccione un usuario de digitalización de la lista antes de continuar.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else if (string.IsNullOrEmpty(origen))
		{
			MessageBox.Show("Seleccione un origen de la lista antes de continuar.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else
		{
			generarIndice(valorUsuarioDigitalizacion, origen);
		}
	}

	private void generarIndice(string pValorUsuarioDigitalizacion, string pOrigen)
	{
		int cdUsuarioDigitalizacion = Convert.ToInt32(cbxUsuarioDigitalizacion.SelectedValue);
		int cdOrigen = Convert.ToInt32(cbxOrigen.SelectedValue);
		DataTable oDespachosEncontradosFinalExportar = ConvertirDGVaDataTable(dgvDespachosEncontrados);
		oDespachosEncontradosFinalExportar.Columns.Add("usuarioDigitalizacion", typeof(string));
		oDespachosEncontradosFinalExportar.Columns.Add("origen", typeof(string));
		foreach (DataRow row in oDespachosEncontradosFinalExportar.Rows)
		{
			row["usuarioDigitalizacion"] = pValorUsuarioDigitalizacion;
			row["origen"] = pOrigen;
		}
		nArchivos.GenerarArchivoIndice(rutaLote, "INDEX_TERMINADO.DAT", oDespachosEncontradosFinalExportar);
		agregarIndexacion(oDespachosEncontradosFinalExportar);
		finalizarLote();
	}

	private void agregarIndexacion(DataTable pDespachosIndexados)
	{
		try
		{
			int cdUsuarioDigitalizacion = Convert.ToInt32(cbxUsuarioDigitalizacion.SelectedValue);
			int cdOrigen = Convert.ToInt32(cbxOrigen.SelectedValue);
			DataTable oDespachosBaseDatos = ConvertirDGVaDataTable(dgvDespachosEncontrados);
			oDespachosBaseDatos.Columns.Add("usuarioDigitalizacion", typeof(string));
			oDespachosBaseDatos.Columns.Add("origen", typeof(string));
			foreach (DataRow row in oDespachosBaseDatos.Rows)
			{
				row["usuarioDigitalizacion"] = cdUsuarioDigitalizacion;
				row["origen"] = cdOrigen;
			}
			nIndexacion.agregarIndexacion(oUsuarioLogueado, 1, oLoteSeleccionado.cdLote, oDespachosBaseDatos);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private DataTable ConvertirDGVaDataTable(DataGridView dgv)
	{
		DataTable dt = new DataTable();
		List<DataGridViewColumn> columnasVisibles = (from DataGridViewColumn c in dgv.Columns
			where c.Visible
			orderby c.DisplayIndex
			select c).ToList();
		foreach (DataGridViewColumn col in columnasVisibles)
		{
			dt.Columns.Add(col.Name, col.ValueType ?? typeof(string));
		}
		foreach (DataGridViewRow row in (IEnumerable)dgv.Rows)
		{
			if (row.IsNewRow)
			{
				continue;
			}
			DataRow dr = dt.NewRow();
			foreach (DataGridViewColumn col2 in columnasVisibles)
			{
				dr[col2.Name] = row.Cells[col2.Name].Value ?? DBNull.Value;
			}
			dt.Rows.Add(dr);
		}
		return dt;
	}

	private void finalizarLote()
	{
		nLotes.finalizarIndexacion(oUsuarioLogueado, oLoteSeleccionado);
		MessageBox.Show("Se finalizo la Indexación del Lote", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		deshabilitarFormulario();
		actualizarLotesDisponibles();
	}

	private void btnVerDespacho_Click_1(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtDespachoSeleccionado.Text))
		{
			MessageBox.Show("Debe seleccionar un despacho", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		frmVerArchivoPDF ofrmVerArchivoPDF = new frmVerArchivoPDF("Despacho: " + txtDespachoSeleccionado.Text, rutaLote, txtDespachoSeleccionado.Text + ".pdf");
		ofrmVerArchivoPDF.Show();
	}

	private void dgvDespachosEncontrados_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
	{
		txtDespachoSeleccionado.Text = dgvDespachosEncontrados.CurrentRow.Cells["dsDespacho"].Value.ToString();
	}

	private void dgvDespachosEncontradosMultiple_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		txtDespachoSeleccionado.Text = dgvDespachosEncontradosMultiple.CurrentRow.Cells["dsDespacho"].Value.ToString();
	}

	private void dgvDespachosNoEncontrados_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
	{
		txtDespachoSeleccionado.Text = dgvDespachosNoEncontrados.CurrentRow.Cells[0].Value.ToString();
	}

	private void ajustarFormularioInicial()
	{
		base.MaximizeBox = false;
		base.Size = new Size(628, 234);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormularioCompleto()
	{
		base.MaximizeBox = true;
		base.Size = new Size(1300, 675);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void deshabilitarFormulario()
	{
		base.WindowState = FormWindowState.Normal;
		dgvLotesDisponibles.Enabled = true;
		btnCargarLote.BackColor = Color.SeaGreen;
		btnCargarLote.Enabled = true;
		btnCancelar.BackColor = Color.DarkGray;
		btnCancelar.Enabled = false;
		txtProyectoSeleccionado.Clear();
		txtLoteSeleccionado.Clear();
		txtTotalArchivosSeleccionado.Clear();
		txtRutaLote.Clear();
		lboxArchivosEncontrados.DataSource = null;
		pnlDespachosEncontrados.Visible = false;
		dgvDespachosEncontrados.DataSource = null;
		pnlDespachosEncontradosMultiple.Visible = false;
		dgvDespachosEncontradosMultiple.DataSource = null;
		pnlDespachosNoEncontrados.Visible = false;
		dgvDespachosNoEncontrados.DataSource = null;
		txtDespachoSeleccionado.Clear();
		rutaLote = "";
		totalArchivosEncontrados = 0;
		rutaCarpetaInicial = "";
		ListaFinalDespachosEncontrados.Clear();
		ListaFinalDespachosEncontradosMultiples.Clear();
		ListaFinalDespachosNoEncontrados.Clear();
		ajustarFormularioInicial();
		GC.Collect();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._03_Indexacion.frmDespachosAutomatico_v2));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
		this.groupBox6 = new System.Windows.Forms.GroupBox();
		this.btnVerDespacho = new System.Windows.Forms.Button();
		this.txtDespachoSeleccionado = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.cbxUsuarioDigitalizacion = new System.Windows.Forms.ComboBox();
		this.label2 = new System.Windows.Forms.Label();
		this.cbxOrigen = new System.Windows.Forms.ComboBox();
		this.pnlLotesDisponibles = new System.Windows.Forms.Panel();
		this.dgvLotesDisponibles = new System.Windows.Forms.DataGridView();
		this.pnlLoteSeleccionado = new System.Windows.Forms.Panel();
		this.lboxArchivosEncontrados = new System.Windows.Forms.ListBox();
		this.txtRutaLote = new System.Windows.Forms.TextBox();
		this.txtLoteSeleccionado = new System.Windows.Forms.TextBox();
		this.txtTotalArchivosSeleccionado = new System.Windows.Forms.TextBox();
		this.txtProyectoSeleccionado = new System.Windows.Forms.TextBox();
		this.lblNombreLote = new System.Windows.Forms.Label();
		this.lblCarpetaLote = new System.Windows.Forms.Label();
		this.lblProyeto = new System.Windows.Forms.Label();
		this.lblTotalArchivos = new System.Windows.Forms.Label();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCargarLote = new System.Windows.Forms.Button();
		this.pnlDespachosEncontrados = new System.Windows.Forms.Panel();
		this.dgvDespachosEncontrados = new System.Windows.Forms.DataGridView();
		this.pnlDespachosEncontradosMultiple = new System.Windows.Forms.Panel();
		this.dgvDespachosEncontradosMultiple = new System.Windows.Forms.DataGridView();
		this.pnlDespachosNoEncontrados = new System.Windows.Forms.Panel();
		this.dgvDespachosNoEncontrados = new System.Windows.Forms.DataGridView();
		this.btnGenerarIndice = new System.Windows.Forms.Button();
		this.groupBox6.SuspendLayout();
		this.pnlLotesDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).BeginInit();
		this.pnlLoteSeleccionado.SuspendLayout();
		this.pnlDespachosEncontrados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontrados).BeginInit();
		this.pnlDespachosEncontradosMultiple.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontradosMultiple).BeginInit();
		this.pnlDespachosNoEncontrados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosNoEncontrados).BeginInit();
		base.SuspendLayout();
		this.groupBox6.Controls.Add(this.btnVerDespacho);
		this.groupBox6.Controls.Add(this.txtDespachoSeleccionado);
		this.groupBox6.ForeColor = System.Drawing.Color.White;
		this.groupBox6.Location = new System.Drawing.Point(12, 424);
		this.groupBox6.Name = "groupBox6";
		this.groupBox6.Size = new System.Drawing.Size(599, 51);
		this.groupBox6.TabIndex = 7;
		this.groupBox6.TabStop = false;
		this.groupBox6.Text = "Despacho Seleccionado";
		this.btnVerDespacho.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnVerDespacho.BackColor = System.Drawing.Color.SeaGreen;
		this.btnVerDespacho.FlatAppearance.BorderSize = 0;
		this.btnVerDespacho.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnVerDespacho.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnVerDespacho.ForeColor = System.Drawing.Color.White;
		this.btnVerDespacho.Image = (System.Drawing.Image)resources.GetObject("btnVerDespacho.Image");
		this.btnVerDespacho.Location = new System.Drawing.Point(348, 17);
		this.btnVerDespacho.Name = "btnVerDespacho";
		this.btnVerDespacho.Size = new System.Drawing.Size(237, 25);
		this.btnVerDespacho.TabIndex = 68;
		this.btnVerDespacho.Text = "   Ver Despacho";
		this.btnVerDespacho.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnVerDespacho.UseVisualStyleBackColor = false;
		this.btnVerDespacho.Click += new System.EventHandler(btnVerDespacho_Click_1);
		this.txtDespachoSeleccionado.Enabled = false;
		this.txtDespachoSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDespachoSeleccionado.Location = new System.Drawing.Point(10, 20);
		this.txtDespachoSeleccionado.Name = "txtDespachoSeleccionado";
		this.txtDespachoSeleccionado.Size = new System.Drawing.Size(332, 22);
		this.txtDespachoSeleccionado.TabIndex = 0;
		this.txtDespachoSeleccionado.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(26, 495);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(160, 16);
		this.label1.TabIndex = 8;
		this.label1.Text = "Usuario de Digitalización:";
		this.cbxUsuarioDigitalizacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxUsuarioDigitalizacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxUsuarioDigitalizacion.FormattingEnabled = true;
		this.cbxUsuarioDigitalizacion.Location = new System.Drawing.Point(304, 492);
		this.cbxUsuarioDigitalizacion.Name = "cbxUsuarioDigitalizacion";
		this.cbxUsuarioDigitalizacion.Size = new System.Drawing.Size(308, 24);
		this.cbxUsuarioDigitalizacion.TabIndex = 9;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(26, 535);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(51, 16);
		this.label2.TabIndex = 13;
		this.label2.Text = "Origen:";
		this.cbxOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxOrigen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxOrigen.FormattingEnabled = true;
		this.cbxOrigen.Location = new System.Drawing.Point(304, 532);
		this.cbxOrigen.Name = "cbxOrigen";
		this.cbxOrigen.Size = new System.Drawing.Size(308, 24);
		this.cbxOrigen.TabIndex = 14;
		this.pnlLotesDisponibles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLotesDisponibles.Controls.Add(this.dgvLotesDisponibles);
		this.pnlLotesDisponibles.Location = new System.Drawing.Point(12, 12);
		this.pnlLotesDisponibles.Name = "pnlLotesDisponibles";
		this.pnlLotesDisponibles.Size = new System.Drawing.Size(450, 175);
		this.pnlLotesDisponibles.TabIndex = 58;
		this.dgvLotesDisponibles.AllowUserToAddRows = false;
		this.dgvLotesDisponibles.AllowUserToDeleteRows = false;
		this.dgvLotesDisponibles.AllowUserToResizeRows = false;
		this.dgvLotesDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dgvLotesDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesDisponibles.DefaultCellStyle = dataGridViewCellStyle2;
		this.dgvLotesDisponibles.EnableHeadersVisualStyles = false;
		this.dgvLotesDisponibles.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesDisponibles.Location = new System.Drawing.Point(17, 13);
		this.dgvLotesDisponibles.Name = "dgvLotesDisponibles";
		this.dgvLotesDisponibles.ReadOnly = true;
		this.dgvLotesDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dgvLotesDisponibles.RowHeadersVisible = false;
		this.dgvLotesDisponibles.RowHeadersWidth = 15;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dgvLotesDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesDisponibles.Size = new System.Drawing.Size(420, 140);
		this.dgvLotesDisponibles.TabIndex = 18;
		this.dgvLotesDisponibles.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvLotesDisponibles_CellContentDoubleClick);
		this.pnlLoteSeleccionado.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLoteSeleccionado.Controls.Add(this.lboxArchivosEncontrados);
		this.pnlLoteSeleccionado.Controls.Add(this.txtRutaLote);
		this.pnlLoteSeleccionado.Controls.Add(this.txtLoteSeleccionado);
		this.pnlLoteSeleccionado.Controls.Add(this.txtTotalArchivosSeleccionado);
		this.pnlLoteSeleccionado.Controls.Add(this.txtProyectoSeleccionado);
		this.pnlLoteSeleccionado.Controls.Add(this.lblNombreLote);
		this.pnlLoteSeleccionado.Controls.Add(this.lblCarpetaLote);
		this.pnlLoteSeleccionado.Controls.Add(this.lblProyeto);
		this.pnlLoteSeleccionado.Controls.Add(this.lblTotalArchivos);
		this.pnlLoteSeleccionado.Location = new System.Drawing.Point(672, 14);
		this.pnlLoteSeleccionado.Name = "pnlLoteSeleccionado";
		this.pnlLoteSeleccionado.Size = new System.Drawing.Size(600, 173);
		this.pnlLoteSeleccionado.TabIndex = 59;
		this.lboxArchivosEncontrados.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lboxArchivosEncontrados.FormattingEnabled = true;
		this.lboxArchivosEncontrados.ItemHeight = 16;
		this.lboxArchivosEncontrados.Location = new System.Drawing.Point(386, 35);
		this.lboxArchivosEncontrados.Name = "lboxArchivosEncontrados";
		this.lboxArchivosEncontrados.Size = new System.Drawing.Size(200, 132);
		this.lboxArchivosEncontrados.TabIndex = 2;
		this.txtRutaLote.Enabled = false;
		this.txtRutaLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtRutaLote.Location = new System.Drawing.Point(151, 121);
		this.txtRutaLote.Multiline = true;
		this.txtRutaLote.Name = "txtRutaLote";
		this.txtRutaLote.Size = new System.Drawing.Size(229, 45);
		this.txtRutaLote.TabIndex = 20;
		this.txtLoteSeleccionado.Enabled = false;
		this.txtLoteSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtLoteSeleccionado.Location = new System.Drawing.Point(151, 65);
		this.txtLoteSeleccionado.Name = "txtLoteSeleccionado";
		this.txtLoteSeleccionado.Size = new System.Drawing.Size(229, 22);
		this.txtLoteSeleccionado.TabIndex = 18;
		this.txtTotalArchivosSeleccionado.Enabled = false;
		this.txtTotalArchivosSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtTotalArchivosSeleccionado.Location = new System.Drawing.Point(151, 93);
		this.txtTotalArchivosSeleccionado.Name = "txtTotalArchivosSeleccionado";
		this.txtTotalArchivosSeleccionado.Size = new System.Drawing.Size(229, 22);
		this.txtTotalArchivosSeleccionado.TabIndex = 17;
		this.txtProyectoSeleccionado.Enabled = false;
		this.txtProyectoSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtProyectoSeleccionado.Location = new System.Drawing.Point(151, 37);
		this.txtProyectoSeleccionado.Name = "txtProyectoSeleccionado";
		this.txtProyectoSeleccionado.Size = new System.Drawing.Size(229, 22);
		this.txtProyectoSeleccionado.TabIndex = 16;
		this.lblNombreLote.AutoSize = true;
		this.lblNombreLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNombreLote.ForeColor = System.Drawing.Color.White;
		this.lblNombreLote.Location = new System.Drawing.Point(14, 68);
		this.lblNombreLote.Name = "lblNombreLote";
		this.lblNombreLote.Size = new System.Drawing.Size(40, 17);
		this.lblNombreLote.TabIndex = 15;
		this.lblNombreLote.Text = "Lote:";
		this.lblCarpetaLote.AutoSize = true;
		this.lblCarpetaLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblCarpetaLote.ForeColor = System.Drawing.Color.White;
		this.lblCarpetaLote.Location = new System.Drawing.Point(14, 124);
		this.lblCarpetaLote.Name = "lblCarpetaLote";
		this.lblCarpetaLote.Size = new System.Drawing.Size(99, 17);
		this.lblCarpetaLote.TabIndex = 14;
		this.lblCarpetaLote.Text = "Carpeta Lote:";
		this.lblProyeto.AutoSize = true;
		this.lblProyeto.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblProyeto.ForeColor = System.Drawing.Color.White;
		this.lblProyeto.Location = new System.Drawing.Point(14, 40);
		this.lblProyeto.Name = "lblProyeto";
		this.lblProyeto.Size = new System.Drawing.Size(69, 17);
		this.lblProyeto.TabIndex = 11;
		this.lblProyeto.Text = "Proyecto:";
		this.lblTotalArchivos.AutoSize = true;
		this.lblTotalArchivos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblTotalArchivos.ForeColor = System.Drawing.Color.White;
		this.lblTotalArchivos.Location = new System.Drawing.Point(14, 96);
		this.lblTotalArchivos.Name = "lblTotalArchivos";
		this.lblTotalArchivos.Size = new System.Drawing.Size(122, 17);
		this.lblTotalArchivos.TabIndex = 12;
		this.lblTotalArchivos.Text = "Total de Archivos:";
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(468, 161);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(130, 24);
		this.btnCerrar.TabIndex = 63;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click_1);
		this.btnCancelar.BackColor = System.Drawing.Color.DarkGray;
		this.btnCancelar.Enabled = false;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(468, 129);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(130, 25);
		this.btnCancelar.TabIndex = 62;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click_1);
		this.btnCargarLote.BackColor = System.Drawing.Color.SeaGreen;
		this.btnCargarLote.FlatAppearance.BorderSize = 0;
		this.btnCargarLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCargarLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCargarLote.ForeColor = System.Drawing.Color.White;
		this.btnCargarLote.Image = (System.Drawing.Image)resources.GetObject("btnCargarLote.Image");
		this.btnCargarLote.Location = new System.Drawing.Point(468, 25);
		this.btnCargarLote.Name = "btnCargarLote";
		this.btnCargarLote.Size = new System.Drawing.Size(130, 25);
		this.btnCargarLote.TabIndex = 61;
		this.btnCargarLote.Text = "   Cargar Lote";
		this.btnCargarLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCargarLote.UseVisualStyleBackColor = false;
		this.btnCargarLote.Click += new System.EventHandler(btnCargarLote_Click_1);
		this.pnlDespachosEncontrados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlDespachosEncontrados.Controls.Add(this.dgvDespachosEncontrados);
		this.pnlDespachosEncontrados.Location = new System.Drawing.Point(12, 208);
		this.pnlDespachosEncontrados.Name = "pnlDespachosEncontrados";
		this.pnlDespachosEncontrados.Size = new System.Drawing.Size(600, 200);
		this.pnlDespachosEncontrados.TabIndex = 64;
		this.pnlDespachosEncontrados.Visible = false;
		this.dgvDespachosEncontrados.AllowUserToAddRows = false;
		this.dgvDespachosEncontrados.AllowUserToDeleteRows = false;
		this.dgvDespachosEncontrados.AllowUserToResizeRows = false;
		this.dgvDespachosEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvDespachosEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvDespachosEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvDespachosEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvDespachosEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDespachosEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
		this.dgvDespachosEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvDespachosEncontrados.DefaultCellStyle = dataGridViewCellStyle6;
		this.dgvDespachosEncontrados.EnableHeadersVisualStyles = false;
		this.dgvDespachosEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvDespachosEncontrados.Location = new System.Drawing.Point(11, 49);
		this.dgvDespachosEncontrados.Name = "dgvDespachosEncontrados";
		this.dgvDespachosEncontrados.ReadOnly = true;
		this.dgvDespachosEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDespachosEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dgvDespachosEncontrados.RowHeadersVisible = false;
		this.dgvDespachosEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		this.dgvDespachosEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle8;
		this.dgvDespachosEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvDespachosEncontrados.Size = new System.Drawing.Size(575, 139);
		this.dgvDespachosEncontrados.TabIndex = 18;
		this.dgvDespachosEncontrados.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosEncontrados_CellContentClick_1);
		this.pnlDespachosEncontradosMultiple.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlDespachosEncontradosMultiple.Controls.Add(this.dgvDespachosEncontradosMultiple);
		this.pnlDespachosEncontradosMultiple.Location = new System.Drawing.Point(672, 208);
		this.pnlDespachosEncontradosMultiple.Name = "pnlDespachosEncontradosMultiple";
		this.pnlDespachosEncontradosMultiple.Size = new System.Drawing.Size(600, 200);
		this.pnlDespachosEncontradosMultiple.TabIndex = 65;
		this.pnlDespachosEncontradosMultiple.Visible = false;
		this.dgvDespachosEncontradosMultiple.AllowUserToAddRows = false;
		this.dgvDespachosEncontradosMultiple.AllowUserToDeleteRows = false;
		this.dgvDespachosEncontradosMultiple.AllowUserToResizeRows = false;
		this.dgvDespachosEncontradosMultiple.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvDespachosEncontradosMultiple.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvDespachosEncontradosMultiple.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvDespachosEncontradosMultiple.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvDespachosEncontradosMultiple.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDespachosEncontradosMultiple.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.dgvDespachosEncontradosMultiple.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvDespachosEncontradosMultiple.DefaultCellStyle = dataGridViewCellStyle10;
		this.dgvDespachosEncontradosMultiple.EnableHeadersVisualStyles = false;
		this.dgvDespachosEncontradosMultiple.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvDespachosEncontradosMultiple.Location = new System.Drawing.Point(17, 49);
		this.dgvDespachosEncontradosMultiple.Name = "dgvDespachosEncontradosMultiple";
		this.dgvDespachosEncontradosMultiple.ReadOnly = true;
		this.dgvDespachosEncontradosMultiple.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDespachosEncontradosMultiple.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
		this.dgvDespachosEncontradosMultiple.RowHeadersVisible = false;
		this.dgvDespachosEncontradosMultiple.RowHeadersWidth = 15;
		dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle12.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
		this.dgvDespachosEncontradosMultiple.RowsDefaultCellStyle = dataGridViewCellStyle12;
		this.dgvDespachosEncontradosMultiple.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvDespachosEncontradosMultiple.Size = new System.Drawing.Size(569, 139);
		this.dgvDespachosEncontradosMultiple.TabIndex = 18;
		this.dgvDespachosEncontradosMultiple.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosEncontradosMultiple_CellContentClick);
		this.dgvDespachosEncontradosMultiple.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosEncontradosMultiple_CellContentDoubleClick);
		this.pnlDespachosNoEncontrados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlDespachosNoEncontrados.Controls.Add(this.dgvDespachosNoEncontrados);
		this.pnlDespachosNoEncontrados.Location = new System.Drawing.Point(672, 424);
		this.pnlDespachosNoEncontrados.Name = "pnlDespachosNoEncontrados";
		this.pnlDespachosNoEncontrados.Size = new System.Drawing.Size(600, 200);
		this.pnlDespachosNoEncontrados.TabIndex = 66;
		this.pnlDespachosNoEncontrados.Visible = false;
		this.dgvDespachosNoEncontrados.AllowUserToAddRows = false;
		this.dgvDespachosNoEncontrados.AllowUserToDeleteRows = false;
		this.dgvDespachosNoEncontrados.AllowUserToResizeRows = false;
		this.dgvDespachosNoEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvDespachosNoEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvDespachosNoEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvDespachosNoEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvDespachosNoEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle13.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDespachosNoEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
		this.dgvDespachosNoEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvDespachosNoEncontrados.DefaultCellStyle = dataGridViewCellStyle14;
		this.dgvDespachosNoEncontrados.EnableHeadersVisualStyles = false;
		this.dgvDespachosNoEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvDespachosNoEncontrados.Location = new System.Drawing.Point(17, 30);
		this.dgvDespachosNoEncontrados.Name = "dgvDespachosNoEncontrados";
		this.dgvDespachosNoEncontrados.ReadOnly = true;
		this.dgvDespachosNoEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDespachosNoEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
		this.dgvDespachosNoEncontrados.RowHeadersVisible = false;
		this.dgvDespachosNoEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
		this.dgvDespachosNoEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle16;
		this.dgvDespachosNoEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvDespachosNoEncontrados.Size = new System.Drawing.Size(569, 165);
		this.dgvDespachosNoEncontrados.TabIndex = 18;
		this.dgvDespachosNoEncontrados.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosNoEncontrados_CellContentClick_1);
		this.btnGenerarIndice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnGenerarIndice.BackColor = System.Drawing.Color.SeaGreen;
		this.btnGenerarIndice.FlatAppearance.BorderSize = 0;
		this.btnGenerarIndice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnGenerarIndice.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnGenerarIndice.ForeColor = System.Drawing.Color.White;
		this.btnGenerarIndice.Image = (System.Drawing.Image)resources.GetObject("btnGenerarIndice.Image");
		this.btnGenerarIndice.Location = new System.Drawing.Point(13, 594);
		this.btnGenerarIndice.Name = "btnGenerarIndice";
		this.btnGenerarIndice.Size = new System.Drawing.Size(599, 25);
		this.btnGenerarIndice.TabIndex = 67;
		this.btnGenerarIndice.Text = "   Generar Indice";
		this.btnGenerarIndice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnGenerarIndice.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnGenerarIndice.UseVisualStyleBackColor = false;
		this.btnGenerarIndice.Click += new System.EventHandler(btnGenerarIndice_Click_1);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1284, 636);
		base.Controls.Add(this.btnGenerarIndice);
		base.Controls.Add(this.pnlDespachosNoEncontrados);
		base.Controls.Add(this.pnlDespachosEncontradosMultiple);
		base.Controls.Add(this.pnlDespachosEncontrados);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnCargarLote);
		base.Controls.Add(this.pnlLoteSeleccionado);
		base.Controls.Add(this.pnlLotesDisponibles);
		base.Controls.Add(this.cbxOrigen);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.groupBox6);
		base.Controls.Add(this.cbxUsuarioDigitalizacion);
		base.Controls.Add(this.label1);
		base.MinimizeBox = false;
		base.Name = "frmDespachosAutomatico_v2";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Indexación de Despachos Auduaneros";
		base.Load += new System.EventHandler(frmDespachosAutomatico_Load);
		this.groupBox6.ResumeLayout(false);
		this.groupBox6.PerformLayout();
		this.pnlLotesDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).EndInit();
		this.pnlLoteSeleccionado.ResumeLayout(false);
		this.pnlLoteSeleccionado.PerformLayout();
		this.pnlDespachosEncontrados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontrados).EndInit();
		this.pnlDespachosEncontradosMultiple.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontradosMultiple).EndInit();
		this.pnlDespachosNoEncontrados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosNoEncontrados).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
