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

public class frmLotesDisponiblesIDX : Form
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

	private Panel pnlLotesDisponibles;

	private DataGridView dgvLotesDisponibles;

	private Button btnCerrar;

	private Button btnCancelar;

	private Button btnCargarLote;

	public frmLotesDisponiblesIDX(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
	}

	private void frmLotesDisponiblesIDX_Load(object sender, EventArgs e)
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
		//cbxUsuarioDigitalizacion.DataSource = nIndexacion.ObtenerLista(oUsuarioLogueado, 1, 1);
		//cbxUsuarioDigitalizacion.DisplayMember = "dsValorLista";
		//cbxUsuarioDigitalizacion.ValueMember = "cdValor";
		//cbxUsuarioDigitalizacion.SelectedIndex = -1;
		//cbxOrigen.DataSource = nIndexacion.ObtenerLista(oUsuarioLogueado, 1, 2);
		//cbxOrigen.DisplayMember = "dsValorLista";
		//cbxOrigen.ValueMember = "cdValor";
		//cbxOrigen.SelectedIndex = -1;
	}

	private void actualizarLotesDisponibles()
	{
		dgvLotesDisponibles.DataSource = nLotes.obtenerLotesDisponibleIndexacion(oUsuarioLogueado, 9);
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
		//pnlLoteSeleccionado.Controls.Add(labelTituloLoteSeleccionado);
	}

	private void btnCerrar_Click_1(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click_1(object sender, EventArgs e)
	{
		//deshabilitarFormulario();
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
		//btnCargarLote.BackColor = Color.DarkGray;
		//btnCargarLote.Enabled = false;
		//dgvLotesDisponibles.Enabled = false;
		//btnCancelar.Enabled = true;
		//btnCancelar.BackColor = Color.Salmon;
		//base.MaximizeBox = true;

		int cdLoteSeleccionado = Convert.ToInt32(dgvLotesDisponibles.SelectedRows[0].Cells["cdLote"].Value.ToString());

		oLoteSeleccionado = nLotes.obtenerUnLote(oUsuarioLogueado, cdLoteSeleccionado);


		Capturador._01_Pantallas._03_Indexacion.frmHistoriasClinicas oFrmHistoriasClinicas = new frmHistoriasClinicas(oUsuarioLogueado, oLoteSeleccionado);
		DialogResult resultado = oFrmHistoriasClinicas.ShowDialog();

		// Si el formulario hijo se cerró después de procesar exitosamente, actualizar el grid
		if (resultado == DialogResult.OK)
		{
			actualizarLotesDisponibles();
			deshabilitarFormulario();
		}

		//oLoteSeleccionado.cdLote = Convert.ToInt32(dgvLotesDisponibles.SelectedRows[0].Cells["cdLote"].Value.ToString());


        //txtProyectoSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsProyecto"].Value.ToString();
        //txtLoteSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsNombreLote"].Value.ToString();
        //txtTotalArchivosSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["nuCantidadArchivos"].Value.ToString();
        //txtRutaLote.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsRutaLote"].Value.ToString();
        //rutaLote = txtRutaLote.Text;
  //      List<string> listaArchivosEncontrados = null;
		//try
		//{
		//	//listaArchivosEncontrados = nIndexacion.ObtenerNombreArchivos(txtRutaLote.Text);
		//	//lboxArchivosEncontrados.DataSource = listaArchivosEncontrados;
		//	//totalArchivosEncontrados = lboxArchivosEncontrados.Items.Count;
		//	procesarArchivosEncontrados();
		//}
		//catch (Exception ex)
		//{
		//	MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		//	deshabilitarFormulario();
		//	actualizarLotesDisponibles();
		//}
	}

	private void procesarArchivosEncontrados()
	{
		//foreach (object archivoEncontrado in lboxArchivosEncontrados.Items)
		//{
		//	string archivo = Path.GetFileNameWithoutExtension(Convert.ToString(archivoEncontrado));
		//	AgregarResultado(archivo);
		//}
		//cargarGrillasResultadoBusqueda();
		//ajustarFormularioCompleto();
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
		//pnlDespachosEncontrados.Visible = true;
		//if (ListaFinalDespachosEncontrados.Count > 0)
		//{
		//	dgvDespachosEncontrados.DataSource = ListaFinalDespachosEncontrados;
		//}
		//else
		//{
		//	List<eDespacho> ListaDespachosVacios = new List<eDespacho>();
		//	dgvDespachosEncontrados.DataSource = ListaDespachosVacios;
		//}
		//dgvDespachosEncontrados.Columns["id"].HeaderText = "id";
		//dgvDespachosEncontrados.Columns["dsDespacho"].HeaderText = "Despacho";
		//dgvDespachosEncontrados.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		//dgvDespachosEncontrados.Columns["nuSIGEA"].HeaderText = "SIGEA";
		//dgvDespachosEncontrados.Columns["nuGuia"].HeaderText = "Nro. Guia";
		//dgvDespachosEncontrados.Columns["id"].DisplayIndex = 0;
		//dgvDespachosEncontrados.Columns["dsDespacho"].DisplayIndex = 1;
		//dgvDespachosEncontrados.Columns["cdSerieDocumental"].DisplayIndex = 2;
		//dgvDespachosEncontrados.Columns["nuSIGEA"].DisplayIndex = 3;
		//dgvDespachosEncontrados.Columns["nuGuia"].DisplayIndex = 4;
		//dgvDespachosEncontrados.Columns["cdEstado"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsEstado"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuCodigoBarras"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuCantidadImagenes"].Visible = false;
		//dgvDespachosEncontrados.Columns["feGeneracionIndice"].Visible = false;
		//dgvDespachosEncontrados.Columns["cdUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontrados.Columns["feFinalizacionProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuEstacionTrabajo"].Visible = false;
		//dgvDespachosEncontrados.Columns["cdUsuarioProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsUsuarioProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuImagen"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsNombreLote"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsRutaArchivoPDF"].Visible = false;
		//Label labelTituloDespachosEncontrados = new Label();
		//labelTituloDespachosEncontrados.Text = "Despachos Encontrados";
		//labelTituloDespachosEncontrados.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		//labelTituloDespachosEncontrados.BackColor = Color.FromArgb(26, 32, 40);
		//labelTituloDespachosEncontrados.ForeColor = Color.White;
		//labelTituloDespachosEncontrados.Dock = DockStyle.Top;
		//labelTituloDespachosEncontrados.Height = 30;
		//labelTituloDespachosEncontrados.TextAlign = ContentAlignment.MiddleCenter;
		//int totalRegistros = dgvDespachosEncontrados.Rows.Count;
		//Label labelTotal = new Label();
		//labelTotal.Text = $"Total de Despachos: {totalRegistros}";
		//labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		//labelTotal.Dock = DockStyle.Bottom;
		//labelTotal.Height = 25;
		//labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		//labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		//labelTotal.ForeColor = Color.White;
		//dgvDespachosEncontrados.Dock = DockStyle.Fill;
		//pnlDespachosEncontrados.Controls.Clear();
		//pnlDespachosEncontrados.Controls.Add(dgvDespachosEncontrados);
		//pnlDespachosEncontrados.Controls.Add(labelTituloDespachosEncontrados);
		//pnlDespachosEncontrados.Controls.Add(labelTotal);
		//if (ListaFinalDespachosEncontradosMultiples.Count > 0)
		//{
		//	pnlDespachosEncontradosMultiple.Visible = true;
		//	dgvDespachosEncontradosMultiple.DataSource = ListaFinalDespachosEncontradosMultiples;
		//	dgvDespachosEncontradosMultiple.Columns["id"].HeaderText = "id";
		//	dgvDespachosEncontradosMultiple.Columns["dsDespacho"].HeaderText = "Despacho";
		//	dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		//	dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].HeaderText = "SIGEA";
		//	dgvDespachosEncontradosMultiple.Columns["nuGuia"].HeaderText = "Nro. Guia";
		//	dgvDespachosEncontradosMultiple.Columns["id"].DisplayIndex = 0;
		//	dgvDespachosEncontradosMultiple.Columns["dsDespacho"].DisplayIndex = 1;
		//	dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].DisplayIndex = 2;
		//	dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].DisplayIndex = 3;
		//	dgvDespachosEncontradosMultiple.Columns["nuGuia"].DisplayIndex = 4;
		//	dgvDespachosEncontradosMultiple.Columns["cdEstado"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["dsEstado"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["nuCodigoBarras"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["nuCantidadImagenes"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["feGeneracionIndice"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["cdUsuarioDigitalizacion"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["dsUsuarioDigitalizacion"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["feFinalizacionProceso"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["nuEstacionTrabajo"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["cdUsuarioProceso"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["dsUsuarioProceso"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["nuImagen"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["dsNombreLote"].Visible = false;
		//	dgvDespachosEncontradosMultiple.Columns["dsRutaArchivoPDF"].Visible = false;
		//	Label labelTituloDespachosEncontradosMultiples = new Label();
		//	labelTituloDespachosEncontradosMultiples.Text = "Despachos Encontrados (Repetidos)";
		//	labelTituloDespachosEncontradosMultiples.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		//	labelTituloDespachosEncontradosMultiples.BackColor = Color.FromArgb(26, 32, 40);
		//	labelTituloDespachosEncontradosMultiples.ForeColor = Color.White;
		//	labelTituloDespachosEncontradosMultiples.Dock = DockStyle.Top;
		//	labelTituloDespachosEncontradosMultiples.Height = 30;
		//	labelTituloDespachosEncontradosMultiples.TextAlign = ContentAlignment.MiddleCenter;
		//	dgvDespachosEncontradosMultiple.Dock = DockStyle.Fill;
		//	pnlDespachosEncontradosMultiple.Controls.Clear();
		//	pnlDespachosEncontradosMultiple.Controls.Add(dgvDespachosEncontradosMultiple);
		//	pnlDespachosEncontradosMultiple.Controls.Add(labelTituloDespachosEncontradosMultiples);
		//}
		//if (ListaFinalDespachosNoEncontrados.Count > 0)
		//{
		//	pnlDespachosNoEncontrados.Visible = true;
		//	dgvDespachosNoEncontrados.AutoGenerateColumns = false;
		//	DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
		//	column1.HeaderText = "Despachos";
		//	column1.Name = "Despachos";
		//	column1.DataPropertyName = "Valor";
		//	dgvDespachosNoEncontrados.Columns.Add(column1);
		//	dgvDespachosNoEncontrados.DataSource = ListaFinalDespachosNoEncontrados.Select((string x) => new
		//	{
		//		Valor = x
		//	}).ToList();
		//	Label labelTituloDespachosNoEncontrados = new Label();
		//	labelTituloDespachosNoEncontrados.Text = "Despachos No Encontrados";
		//	labelTituloDespachosNoEncontrados.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		//	labelTituloDespachosNoEncontrados.BackColor = Color.FromArgb(26, 32, 40);
		//	labelTituloDespachosNoEncontrados.ForeColor = Color.White;
		//	labelTituloDespachosNoEncontrados.Dock = DockStyle.Top;
		//	labelTituloDespachosNoEncontrados.Height = 30;
		//	labelTituloDespachosNoEncontrados.TextAlign = ContentAlignment.MiddleCenter;
		//	dgvDespachosNoEncontrados.Dock = DockStyle.Fill;
		//	pnlDespachosNoEncontrados.Controls.Clear();
		//	pnlDespachosNoEncontrados.Controls.Add(dgvDespachosNoEncontrados);
		//	pnlDespachosNoEncontrados.Controls.Add(labelTituloDespachosNoEncontrados);
		//}
	}

	private void actualizarGrillaEncontrados()
	{
		//pnlDespachosEncontrados.Visible = true;
		//List<eDespacho> oDespachosEncontradosActualizar = (List<eDespacho>)dgvDespachosEncontrados.DataSource;
		//dgvDespachosEncontrados.DataSource = oDespachosEncontradosActualizar;
		//dgvDespachosEncontrados.Columns["id"].HeaderText = "id";
		//dgvDespachosEncontrados.Columns["dsDespacho"].HeaderText = "Despacho";
		//dgvDespachosEncontrados.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		//dgvDespachosEncontrados.Columns["nuSIGEA"].HeaderText = "SIGEA";
		//dgvDespachosEncontrados.Columns["nuGuia"].HeaderText = "Nro. Guia";
		//dgvDespachosEncontrados.Columns["id"].DisplayIndex = 0;
		//dgvDespachosEncontrados.Columns["dsDespacho"].DisplayIndex = 1;
		//dgvDespachosEncontrados.Columns["cdSerieDocumental"].DisplayIndex = 2;
		//dgvDespachosEncontrados.Columns["nuSIGEA"].DisplayIndex = 3;
		//dgvDespachosEncontrados.Columns["nuGuia"].DisplayIndex = 4;
		//dgvDespachosEncontrados.Columns["cdEstado"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsEstado"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuCodigoBarras"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuCantidadImagenes"].Visible = false;
		//dgvDespachosEncontrados.Columns["feGeneracionIndice"].Visible = false;
		//dgvDespachosEncontrados.Columns["cdUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontrados.Columns["feFinalizacionProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuEstacionTrabajo"].Visible = false;
		//dgvDespachosEncontrados.Columns["cdUsuarioProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsUsuarioProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuImagen"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsNombreLote"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsRutaArchivoPDF"].Visible = false;
		//Label labelTituloDespachosEncontrados = new Label();
		//labelTituloDespachosEncontrados.Text = "Despachos Encontrados";
		//labelTituloDespachosEncontrados.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		//labelTituloDespachosEncontrados.BackColor = Color.FromArgb(26, 32, 40);
		//labelTituloDespachosEncontrados.ForeColor = Color.White;
		//labelTituloDespachosEncontrados.Dock = DockStyle.Top;
		//labelTituloDespachosEncontrados.Height = 30;
		//labelTituloDespachosEncontrados.TextAlign = ContentAlignment.MiddleCenter;
		//int totalRegistros = dgvDespachosEncontrados.Rows.Count;
		//Label labelTotal = new Label();
		//labelTotal.Text = $"Total de Despachos: {totalRegistros}";
		//labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		//labelTotal.Dock = DockStyle.Bottom;
		//labelTotal.Height = 25;
		//labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		//labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		//labelTotal.ForeColor = Color.White;
		//dgvDespachosEncontrados.Dock = DockStyle.Fill;
		//pnlDespachosEncontrados.Controls.Clear();
		//pnlDespachosEncontrados.Controls.Add(dgvDespachosEncontrados);
		//pnlDespachosEncontrados.Controls.Add(labelTituloDespachosEncontrados);
		//pnlDespachosEncontrados.Controls.Add(labelTotal);
	}

	private void dgvDespachosEncontradosMultiple_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		//string id = dgvDespachosEncontradosMultiple.CurrentRow.Cells["id"].Value.ToString();
		//string dsDespacho = dgvDespachosEncontradosMultiple.CurrentRow.Cells["dsDespacho"].Value.ToString();
		//string nuSerieDocumental = dgvDespachosEncontradosMultiple.CurrentRow.Cells["cdSerieDocumental"].Value.ToString();
		//string nuSIGEA = dgvDespachosEncontradosMultiple.CurrentRow.Cells["nuSIGEA"].Value.ToString();
		//string nuGuia = dgvDespachosEncontradosMultiple.CurrentRow.Cells["nuGuia"].Value.ToString();
		//eDespacho despachoAgregar = new eDespacho();
		//despachoAgregar.id = Convert.ToInt32(id);
		//despachoAgregar.dsDespacho = dsDespacho;
		//despachoAgregar.cdSerieDocumental = nuSerieDocumental;
		//despachoAgregar.nuSIGEA = nuSIGEA;
		//despachoAgregar.nuGuia = nuGuia;
		//List<eDespacho> oDespachosEncontradosActual = (List<eDespacho>)dgvDespachosEncontrados.DataSource;
		//oDespachosEncontradosActual.Add(despachoAgregar);
		//dgvDespachosEncontrados.DataSource = null;
		//dgvDespachosEncontrados.DataSource = oDespachosEncontradosActual;
		//dgvDespachosEncontrados.Columns["id"].HeaderText = "id";
		//dgvDespachosEncontrados.Columns["dsDespacho"].HeaderText = "Despacho";
		//dgvDespachosEncontrados.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		//dgvDespachosEncontrados.Columns["nuSIGEA"].HeaderText = "SIGEA";
		//dgvDespachosEncontrados.Columns["nuGuia"].HeaderText = "Nro. Guia";
		//dgvDespachosEncontrados.Columns["id"].DisplayIndex = 0;
		//dgvDespachosEncontrados.Columns["dsDespacho"].DisplayIndex = 1;
		//dgvDespachosEncontrados.Columns["cdSerieDocumental"].DisplayIndex = 2;
		//dgvDespachosEncontrados.Columns["nuSIGEA"].DisplayIndex = 3;
		//dgvDespachosEncontrados.Columns["nuGuia"].DisplayIndex = 4;
		//dgvDespachosEncontrados.Columns["cdEstado"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsEstado"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuCodigoBarras"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuCantidadImagenes"].Visible = false;
		//dgvDespachosEncontrados.Columns["feGeneracionIndice"].Visible = false;
		//dgvDespachosEncontrados.Columns["cdUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontrados.Columns["feFinalizacionProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuEstacionTrabajo"].Visible = false;
		//dgvDespachosEncontrados.Columns["cdUsuarioProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsUsuarioProceso"].Visible = false;
		//dgvDespachosEncontrados.Columns["nuImagen"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsNombreLote"].Visible = false;
		//dgvDespachosEncontrados.Columns["dsRutaArchivoPDF"].Visible = false;
		//dgvDespachosEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		//List<eDespacho> oDespachosEncontradosMultiplesActual = (List<eDespacho>)dgvDespachosEncontradosMultiple.DataSource;
		//for (int i = oDespachosEncontradosMultiplesActual.Count - 1; i >= 0; i--)
		//{
		//	if (oDespachosEncontradosMultiplesActual[i].dsDespacho == dsDespacho)
		//	{
		//		oDespachosEncontradosMultiplesActual.RemoveAt(i);
		//	}
		//}
		//dgvDespachosEncontradosMultiple.DataSource = null;
		//dgvDespachosEncontradosMultiple.DataSource = oDespachosEncontradosMultiplesActual;
		//dgvDespachosEncontradosMultiple.Columns["id"].HeaderText = "id";
		//dgvDespachosEncontradosMultiple.Columns["dsDespacho"].HeaderText = "Despacho";
		//dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].HeaderText = "Serie Documental";
		//dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].HeaderText = "SIGEA";
		//dgvDespachosEncontradosMultiple.Columns["nuGuia"].HeaderText = "Nro. Guia";
		//dgvDespachosEncontradosMultiple.Columns["id"].DisplayIndex = 0;
		//dgvDespachosEncontradosMultiple.Columns["dsDespacho"].DisplayIndex = 1;
		//dgvDespachosEncontradosMultiple.Columns["cdSerieDocumental"].DisplayIndex = 2;
		//dgvDespachosEncontradosMultiple.Columns["nuSIGEA"].DisplayIndex = 3;
		//dgvDespachosEncontradosMultiple.Columns["nuGuia"].DisplayIndex = 4;
		//dgvDespachosEncontradosMultiple.Columns["cdEstado"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["dsEstado"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["nuCodigoBarras"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["nuCantidadImagenes"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["feGeneracionIndice"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["cdUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["dsUsuarioDigitalizacion"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["feFinalizacionProceso"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["nuEstacionTrabajo"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["cdUsuarioProceso"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["dsUsuarioProceso"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["nuImagen"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["dsNombreLote"].Visible = false;
		//dgvDespachosEncontradosMultiple.Columns["dsRutaArchivoPDF"].Visible = false;
		//actualizarGrillaEncontrados();
	}

	private void btnGenerarIndice_Click_1(object sender, EventArgs e)
	{
		//int totalRegistosEncontrados = dgvDespachosEncontrados.Rows.Count;
		//string valorUsuarioDigitalizacion = cbxUsuarioDigitalizacion.Text;
		//string origen = cbxOrigen.Text;
		//if (totalArchivosEncontrados != totalRegistosEncontrados)
		//{
		//	MessageBox.Show("No coincide la cantidad de archivos con los despachos encontrados", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//}
		//else if (string.IsNullOrEmpty(valorUsuarioDigitalizacion))
		//{
		//	MessageBox.Show("Seleccione un usuario de digitalización de la lista antes de continuar.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//}
		//else if (string.IsNullOrEmpty(origen))
		//{
		//	MessageBox.Show("Seleccione un origen de la lista antes de continuar.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//}
		//else
		//{
		//	generarIndice(valorUsuarioDigitalizacion, origen);
		//}
	}

	private void generarIndice(string pValorUsuarioDigitalizacion, string pOrigen)
	{
		//int cdUsuarioDigitalizacion = Convert.ToInt32(cbxUsuarioDigitalizacion.SelectedValue);
		//int cdOrigen = Convert.ToInt32(cbxOrigen.SelectedValue);
		//DataTable oDespachosEncontradosFinalExportar = ConvertirDGVaDataTable(dgvDespachosEncontrados);
		//oDespachosEncontradosFinalExportar.Columns.Add("usuarioDigitalizacion", typeof(string));
		//oDespachosEncontradosFinalExportar.Columns.Add("origen", typeof(string));
		//foreach (DataRow row in oDespachosEncontradosFinalExportar.Rows)
		//{
		//	row["usuarioDigitalizacion"] = pValorUsuarioDigitalizacion;
		//	row["origen"] = pOrigen;
		//}
		//nArchivos.GenerarArchivoIndice(rutaLote, "INDEX_TERMINADO.DAT", oDespachosEncontradosFinalExportar);
		//agregarIndexacion(oDespachosEncontradosFinalExportar);
		//finalizarLote();
	}

	private void agregarIndexacion(DataTable pDespachosIndexados)
	{
		//try
		//{
		//	int cdUsuarioDigitalizacion = Convert.ToInt32(cbxUsuarioDigitalizacion.SelectedValue);
		//	int cdOrigen = Convert.ToInt32(cbxOrigen.SelectedValue);
		//	DataTable oDespachosBaseDatos = ConvertirDGVaDataTable(dgvDespachosEncontrados);
		//	oDespachosBaseDatos.Columns.Add("usuarioDigitalizacion", typeof(string));
		//	oDespachosBaseDatos.Columns.Add("origen", typeof(string));
		//	foreach (DataRow row in oDespachosBaseDatos.Rows)
		//	{
		//		row["usuarioDigitalizacion"] = cdUsuarioDigitalizacion;
		//		row["origen"] = cdOrigen;
		//	}
		//	nIndexacion.agregarIndexacion(oUsuarioLogueado, 1, oLoteSeleccionado.cdLote, oDespachosBaseDatos);
		//}
		//catch (Exception ex)
		//{
		//	MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		//}
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
		//if (string.IsNullOrEmpty(txtDespachoSeleccionado.Text))
		//{
		//	MessageBox.Show("Debe seleccionar un despacho", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//	return;
		//}
		//frmVerArchivoPDF ofrmVerArchivoPDF = new frmVerArchivoPDF("Despacho: " + txtDespachoSeleccionado.Text, rutaLote, txtDespachoSeleccionado.Text + ".pdf");
		//ofrmVerArchivoPDF.Show();
	}

	private void dgvDespachosEncontrados_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
	{
		//txtDespachoSeleccionado.Text = dgvDespachosEncontrados.CurrentRow.Cells["dsDespacho"].Value.ToString();
	}

	private void dgvDespachosEncontradosMultiple_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		//txtDespachoSeleccionado.Text = dgvDespachosEncontradosMultiple.CurrentRow.Cells["dsDespacho"].Value.ToString();
	}

	private void dgvDespachosNoEncontrados_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
	{
		//txtDespachoSeleccionado.Text = dgvDespachosNoEncontrados.CurrentRow.Cells[0].Value.ToString();
	}

	private void ajustarFormularioInicial()
	{
		base.MaximizeBox = false;
		base.Size = new Size(900, 500);
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
		//base.WindowState = FormWindowState.Normal;
		//dgvLotesDisponibles.Enabled = true;
		//btnCargarLote.BackColor = Color.SeaGreen;
		//btnCargarLote.Enabled = true;
		//btnCancelar.BackColor = Color.DarkGray;
		//btnCancelar.Enabled = false;
		//txtProyectoSeleccionado.Clear();
		//txtLoteSeleccionado.Clear();
		//txtTotalArchivosSeleccionado.Clear();
		//txtRutaLote.Clear();
		//lboxArchivosEncontrados.DataSource = null;
		//pnlDespachosEncontrados.Visible = false;
		//dgvDespachosEncontrados.DataSource = null;
		//pnlDespachosEncontradosMultiple.Visible = false;
		//dgvDespachosEncontradosMultiple.DataSource = null;
		//pnlDespachosNoEncontrados.Visible = false;
		//dgvDespachosNoEncontrados.DataSource = null;
		//txtDespachoSeleccionado.Clear();
		//rutaLote = "";
		//totalArchivosEncontrados = 0;
		//rutaCarpetaInicial = "";
		//ListaFinalDespachosEncontrados.Clear();
		//ListaFinalDespachosEncontradosMultiples.Clear();
		//ListaFinalDespachosNoEncontrados.Clear();
		//ajustarFormularioInicial();
		//GC.Collect();
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlLotesDisponibles = new System.Windows.Forms.Panel();
            this.dgvLotesDisponibles = new System.Windows.Forms.DataGridView();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnCargarLote = new System.Windows.Forms.Button();
            this.pnlLotesDisponibles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotesDisponibles)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlLotesDisponibles
            // 
            this.pnlLotesDisponibles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.pnlLotesDisponibles.Controls.Add(this.dgvLotesDisponibles);
            this.pnlLotesDisponibles.Location = new System.Drawing.Point(12, 12);
            this.pnlLotesDisponibles.Name = "pnlLotesDisponibles";
            this.pnlLotesDisponibles.Size = new System.Drawing.Size(860, 382);
            this.pnlLotesDisponibles.TabIndex = 58;
            // 
            // dgvLotesDisponibles
            // 
            this.dgvLotesDisponibles.AllowUserToAddRows = false;
            this.dgvLotesDisponibles.AllowUserToDeleteRows = false;
            this.dgvLotesDisponibles.AllowUserToResizeRows = false;
            this.dgvLotesDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLotesDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.dgvLotesDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLotesDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLotesDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(96)))), ((int)(((byte)(130)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(96)))), ((int)(((byte)(130)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLotesDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLotesDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLotesDisponibles.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLotesDisponibles.EnableHeadersVisualStyles = false;
            this.dgvLotesDisponibles.GridColor = System.Drawing.Color.SteelBlue;
            this.dgvLotesDisponibles.Location = new System.Drawing.Point(40, 55);
            this.dgvLotesDisponibles.Name = "dgvLotesDisponibles";
            this.dgvLotesDisponibles.ReadOnly = true;
            this.dgvLotesDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLotesDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLotesDisponibles.RowHeadersVisible = false;
            this.dgvLotesDisponibles.RowHeadersWidth = 15;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            this.dgvLotesDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvLotesDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLotesDisponibles.Size = new System.Drawing.Size(420, 140);
            this.dgvLotesDisponibles.TabIndex = 18;
            this.dgvLotesDisponibles.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLotesDisponibles_CellContentDoubleClick);
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(693, 401);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(130, 24);
            this.btnCerrar.TabIndex = 63;
            this.btnCerrar.Text = "   Cerrar";
            this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click_1);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.DarkGray;
            this.btnCancelar.Enabled = false;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(233, 400);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 25);
            this.btnCancelar.TabIndex = 62;
            this.btnCancelar.Text = "   Cancelar";
            this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click_1);
            // 
            // btnCargarLote
            // 
            this.btnCargarLote.BackColor = System.Drawing.Color.SeaGreen;
            this.btnCargarLote.FlatAppearance.BorderSize = 0;
            this.btnCargarLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarLote.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarLote.ForeColor = System.Drawing.Color.White;
            this.btnCargarLote.Location = new System.Drawing.Point(52, 400);
            this.btnCargarLote.Name = "btnCargarLote";
            this.btnCargarLote.Size = new System.Drawing.Size(130, 25);
            this.btnCargarLote.TabIndex = 61;
            this.btnCargarLote.Text = "   Cargar Lote";
            this.btnCargarLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCargarLote.UseVisualStyleBackColor = false;
            this.btnCargarLote.Click += new System.EventHandler(this.btnCargarLote_Click_1);
            // 
            // frmLotesDisponiblesIDX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCargarLote);
            this.Controls.Add(this.pnlLotesDisponibles);
            this.MinimizeBox = false;
            this.Name = "frmLotesDisponiblesIDX";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Indexación de Historias Clinicas";
            this.Load += new System.EventHandler(this.frmLotesDisponiblesIDX_Load);
            this.pnlLotesDisponibles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotesDisponibles)).EndInit();
            this.ResumeLayout(false);

	}
}
