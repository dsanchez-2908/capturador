using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._05_Consultas;

public class frmLoteConsultaDespachos : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private IContainer components = null;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

	private Label label8;

	private Label label9;

	private Label label10;

	private Label label11;

	private Label label12;

	private Label label13;

	private TextBox txtCodigoLote;

	private TextBox txtNombreLote;

	private ComboBox cbxListaUsuarios;

	private ComboBox cbxListaEstados;

	private MaskedTextBox txtFechaAltaDesde;

	private MaskedTextBox txtFechaAltaHasta;

	private TextBox txtDespacho;

	private TextBox txtNumeroGuia;

	private TextBox txtSerieDocumental;

	private TextBox txtSIGEA;

	private MaskedTextBox txtFechaFinalizacionDesde;

	private MaskedTextBox txtFechaFinalizacionHasta;

	private Panel pnlLotesEncontrados;

	private DataGridView dgvLotesEncontrados;

	private Button btnVerLote;

	private Button btnCerrar;

	private Button btnCancelar;

	private Button btnExportar;

	private Button btnBuscar;

	private ComboBox cbxUsuarioDigitalizacion;

	private Label label1;

	private ComboBox cbxOrigen;

	private Label label14;

	private Panel pnlDetalleLote;

	private TextBox txtDetalleCdLote;

	private ComboBox cbxDetalleEstado;

	private Label label23;

	private ComboBox cbxDetalleUsuario;

	private Label label15;

	private TextBox txtDetalleNombreLote;

	private Label label22;

	private Label label16;

	private Label label21;

	private Label label17;

	private Label label20;

	private Label label18;

	private Label label19;

	private ComboBox cbxDetalleOrigen;

	private ComboBox cbxDetalleUsuarioDigitalizacion;

	private Label label24;

	private TextBox txtDetalleFechaFinalizacion;

	private TextBox txtDetalleFechaAlta;

	private DataGridView dgvDetalleLote;

	private TextBox txtDetalleCantidadDespachos;

	private TextBox txtDetalleRutaLote;

	private Label label26;

	private TextBox txtDetalleFechaIndexacion;

	private Label label25;

	private TextBox txtDetalleFechaControlCalidad;

	private Button btnCerrarDetalle;

	public frmLoteConsultaDespachos(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
	}

	private void frmLoteConsultaDespachos_Load(object sender, EventArgs e)
	{
		cargarListas();
		llenarGrillaResultadoConsulta();
	}

	private void cargarListas()
	{
		cbxListaUsuarios.DataSource = nListas.ObtenerListaUsuarios(oUsuarioLogueado);
		cbxListaUsuarios.DisplayMember = "dsUsuarioNombre";
		cbxListaUsuarios.ValueMember = "cdUsuario";
		cbxListaUsuarios.SelectedIndex = -1;
		cbxListaEstados.DataSource = nListas.ObtenerListaEstados(oUsuarioLogueado);
		cbxListaEstados.DisplayMember = "dsEstado";
		cbxListaEstados.ValueMember = "cdEstado";
		cbxListaEstados.SelectedIndex = -1;
		cbxUsuarioDigitalizacion.DataSource = nListas.ObtenerListaDigitalizadores(oUsuarioLogueado);
		cbxUsuarioDigitalizacion.DisplayMember = "dsValorLista";
		cbxUsuarioDigitalizacion.ValueMember = "cdValor";
		cbxUsuarioDigitalizacion.SelectedIndex = -1;
		cbxOrigen.DataSource = nListas.ObtenerListaOrigen(oUsuarioLogueado);
		cbxOrigen.DisplayMember = "dsValorLista";
		cbxOrigen.ValueMember = "cdValor";
		cbxOrigen.SelectedIndex = -1;
		cbxDetalleUsuario.DataSource = nListas.ObtenerListaUsuarios(oUsuarioLogueado);
		cbxDetalleUsuario.DisplayMember = "dsUsuarioNombre";
		cbxDetalleUsuario.ValueMember = "cdUsuario";
		cbxDetalleUsuario.SelectedIndex = -1;
		cbxDetalleEstado.DataSource = nListas.ObtenerListaEstados(oUsuarioLogueado);
		cbxDetalleEstado.DisplayMember = "dsEstado";
		cbxDetalleEstado.ValueMember = "cdEstado";
		cbxDetalleEstado.SelectedIndex = -1;
		cbxDetalleUsuarioDigitalizacion.DataSource = nListas.ObtenerListaDigitalizadores(oUsuarioLogueado);
		cbxDetalleUsuarioDigitalizacion.DisplayMember = "dsValorLista";
		cbxDetalleUsuarioDigitalizacion.ValueMember = "cdValor";
		cbxDetalleUsuarioDigitalizacion.SelectedIndex = -1;
		cbxDetalleOrigen.DataSource = nListas.ObtenerListaOrigen(oUsuarioLogueado);
		cbxDetalleOrigen.DisplayMember = "dsValorLista";
		cbxDetalleOrigen.ValueMember = "cdValor";
		cbxDetalleOrigen.SelectedIndex = -1;
	}

	private void llenarGrillaResultadoConsulta()
	{
		dgvLotesEncontrados.DataSource = ObtenerTablaConsultaLote();
		dgvLotesEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Detalle de Valores";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvLotesEncontrados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Cantidad de Registros: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvLotesEncontrados.Dock = DockStyle.Fill;
		pnlLotesEncontrados.Controls.Clear();
		pnlLotesEncontrados.Controls.Add(dgvLotesEncontrados);
		pnlLotesEncontrados.Controls.Add(labelTitulo);
		pnlLotesEncontrados.Controls.Add(labelTotal);
	}

	private DataTable ObtenerTablaConsultaLote()
	{
		DataTable oTableResultadoConsulta = new DataTable();
		eLoteConsultaDespacho oLoteConsultaDespacho = new eLoteConsultaDespacho();
		if (!string.IsNullOrEmpty(txtCodigoLote.Text))
		{
			oLoteConsultaDespacho.cdLote = Convert.ToInt32(txtCodigoLote.Text);
		}
		if (!string.IsNullOrEmpty(txtNombreLote.Text))
		{
			oLoteConsultaDespacho.dsNombreLote = txtNombreLote.Text;
		}
		if (cbxListaUsuarios.SelectedIndex != -1)
		{
			oLoteConsultaDespacho.cdUsuario = Convert.ToInt32(cbxListaUsuarios.SelectedValue);
		}
		if (cbxListaEstados.SelectedIndex != -1)
		{
			oLoteConsultaDespacho.cdEstado = Convert.ToInt32(cbxListaEstados.SelectedValue);
		}
		if (!string.IsNullOrEmpty(txtFechaAltaDesde.Text))
		{
			oLoteConsultaDespacho.feAltaDesde = DateTime.ParseExact(txtFechaAltaDesde.Text, "ddMMyyyy", CultureInfo.InvariantCulture);
		}
		if (!string.IsNullOrEmpty(txtFechaAltaHasta.Text))
		{
			oLoteConsultaDespacho.feAltaHasta = DateTime.ParseExact(txtFechaAltaHasta.Text, "ddMMyyyy", CultureInfo.InvariantCulture);
		}
		if (!string.IsNullOrEmpty(txtFechaFinalizacionDesde.Text))
		{
			oLoteConsultaDespacho.feFinalizacionDesde = DateTime.ParseExact(txtFechaFinalizacionDesde.Text, "ddMMyyyy", CultureInfo.InvariantCulture);
		}
		if (!string.IsNullOrEmpty(txtFechaFinalizacionHasta.Text))
		{
			oLoteConsultaDespacho.feFinalizacionHasta = DateTime.ParseExact(txtFechaFinalizacionHasta.Text, "ddMMyyyy", CultureInfo.InvariantCulture);
		}
		if (!string.IsNullOrEmpty(txtDespacho.Text))
		{
			oLoteConsultaDespacho.dsDespacho = txtDespacho.Text;
		}
		if (!string.IsNullOrEmpty(txtNumeroGuia.Text))
		{
			oLoteConsultaDespacho.nuGuia = txtNumeroGuia.Text;
		}
		if (!string.IsNullOrEmpty(txtSerieDocumental.Text))
		{
			oLoteConsultaDespacho.dsSerieDocumental = txtSerieDocumental.Text;
		}
		if (!string.IsNullOrEmpty(txtSIGEA.Text))
		{
			oLoteConsultaDespacho.dsSIGEA = txtSIGEA.Text;
		}
		if (cbxUsuarioDigitalizacion.SelectedIndex != -1)
		{
			oLoteConsultaDespacho.cdUsuarioDigitalizacion = Convert.ToInt32(cbxUsuarioDigitalizacion.SelectedValue);
		}
		if (cbxOrigen.SelectedIndex != -1)
		{
			oLoteConsultaDespacho.cdOrigen = Convert.ToInt32(cbxOrigen.SelectedValue);
		}
		try
		{
			oTableResultadoConsulta = nLotes.obtenerLoteConsultaDespachos(oUsuarioLogueado, oLoteConsultaDespacho);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		return oTableResultadoConsulta;
	}

	private void btnBuscar_Click(object sender, EventArgs e)
	{
		llenarGrillaResultadoConsulta();
	}

	private void vaciar()
	{
		txtCodigoLote.Clear();
		txtNombreLote.Clear();
		cbxListaUsuarios.SelectedIndex = -1;
		cbxListaEstados.SelectedIndex = -1;
		txtFechaAltaDesde.Clear();
		txtFechaAltaHasta.Clear();
		txtFechaFinalizacionDesde.Clear();
		txtFechaFinalizacionHasta.Clear();
		txtDespacho.Clear();
		txtNumeroGuia.Clear();
		txtSerieDocumental.Clear();
		txtSIGEA.Clear();
		cbxUsuarioDigitalizacion.SelectedIndex = -1;
		cbxOrigen.SelectedIndex = -1;
		llenarGrillaResultadoConsulta();
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		vaciar();
	}

	private void btnExportar_Click(object sender, EventArgs e)
	{
		if (dgvLotesEncontrados.Rows.Count <= 0)
		{
			MessageBox.Show("No ha registros que exportar", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		SaveFileDialog oSeleccionarArchivoGuardar = new SaveFileDialog();
		oSeleccionarArchivoGuardar.Filter = "Archivo Excel (*.xlxs)|*.xlsx|Archivo CSV (*.csv)|*.csv";
		oSeleccionarArchivoGuardar.Title = "Seleccione el Archivo a Exportar";
		oSeleccionarArchivoGuardar.FileName = "Detalle_Lotes_" + DateTime.Now.ToString("yyyyMMdd");
		if (oSeleccionarArchivoGuardar.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string archivoGenerar = oSeleccionarArchivoGuardar.FileName;
		DataTable oTablaExportar = (DataTable)dgvLotesEncontrados.DataSource;
		try
		{
			nExportar.exportarTabla(oTablaExportar, archivoGenerar);
			MessageBox.Show("Se exportó el detalle correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void btnVerLote_Click(object sender, EventArgs e)
	{
		if (dgvLotesEncontrados.Rows.Count == 0)
		{
			MessageBox.Show("Debe seleccionar un lote", "Información", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		pnlDetalleLote.Visible = true;
		pnlDetalleLote.Dock = DockStyle.Fill;
		obtenerUnLote();
	}

	private void btnCerrarDetalle_Click(object sender, EventArgs e)
	{
		pnlDetalleLote.Visible = false;
	}

	private void obtenerUnLote()
	{
		int cdLoteSeleccionado = Convert.ToInt32(dgvLotesEncontrados.SelectedRows[0].Cells[0].Value.ToString());
		DataTable oTablaUnLote = new DataTable();
		eLoteConsultaDespacho oLoteConsultaDespacho = new eLoteConsultaDespacho();
		oLoteConsultaDespacho.cdLote = cdLoteSeleccionado;
		oTablaUnLote = nLotes.obtenerLote(oUsuarioLogueado, cdLoteSeleccionado);
		txtDetalleCdLote.Text = Convert.ToString(cdLoteSeleccionado);
		cbxDetalleEstado.Text = oTablaUnLote.Rows[0]["dsEstado"].ToString();
		txtDetalleNombreLote.Text = oTablaUnLote.Rows[0]["dsNombreLote"].ToString();
		txtDetalleRutaLote.Text = oTablaUnLote.Rows[0]["dsRutaLote"].ToString();
		txtDetalleCantidadDespachos.Text = oTablaUnLote.Rows[0]["nuCantidadArchivos"].ToString();
		cbxDetalleUsuario.Text = oTablaUnLote.Rows[0]["dsUsuarioNombreAlta"].ToString();
		txtDetalleFechaAlta.Text = oTablaUnLote.Rows[0]["feAlta"].ToString();
		txtDetalleFechaControlCalidad.Text = oTablaUnLote.Rows[0]["feControlCalidad"].ToString();
		txtDetalleFechaIndexacion.Text = oTablaUnLote.Rows[0]["feIndexacion"].ToString();
		txtDetalleFechaFinalizacion.Text = oTablaUnLote.Rows[0]["feSalida"].ToString();
		cbxDetalleUsuarioDigitalizacion.Text = oTablaUnLote.Rows[0]["dsUsuarioDigitalizacion"].ToString();
		cbxDetalleOrigen.Text = oTablaUnLote.Rows[0]["dsOrigen"].ToString();
		dgvDetalleLote.DataSource = nLotes.obtenerLoteDetalle(oUsuarioLogueado, cdLoteSeleccionado);
		dgvDetalleLote.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._05_Consultas.frmLoteConsultaDespachos));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.label11 = new System.Windows.Forms.Label();
		this.label12 = new System.Windows.Forms.Label();
		this.label13 = new System.Windows.Forms.Label();
		this.txtCodigoLote = new System.Windows.Forms.TextBox();
		this.txtNombreLote = new System.Windows.Forms.TextBox();
		this.cbxListaUsuarios = new System.Windows.Forms.ComboBox();
		this.cbxListaEstados = new System.Windows.Forms.ComboBox();
		this.txtFechaAltaDesde = new System.Windows.Forms.MaskedTextBox();
		this.txtFechaAltaHasta = new System.Windows.Forms.MaskedTextBox();
		this.txtDespacho = new System.Windows.Forms.TextBox();
		this.txtNumeroGuia = new System.Windows.Forms.TextBox();
		this.txtSerieDocumental = new System.Windows.Forms.TextBox();
		this.txtSIGEA = new System.Windows.Forms.TextBox();
		this.txtFechaFinalizacionDesde = new System.Windows.Forms.MaskedTextBox();
		this.txtFechaFinalizacionHasta = new System.Windows.Forms.MaskedTextBox();
		this.pnlLotesEncontrados = new System.Windows.Forms.Panel();
		this.dgvLotesEncontrados = new System.Windows.Forms.DataGridView();
		this.pnlDetalleLote = new System.Windows.Forms.Panel();
		this.btnCerrarDetalle = new System.Windows.Forms.Button();
		this.label26 = new System.Windows.Forms.Label();
		this.txtDetalleFechaIndexacion = new System.Windows.Forms.TextBox();
		this.label25 = new System.Windows.Forms.Label();
		this.txtDetalleFechaControlCalidad = new System.Windows.Forms.TextBox();
		this.txtDetalleFechaFinalizacion = new System.Windows.Forms.TextBox();
		this.txtDetalleFechaAlta = new System.Windows.Forms.TextBox();
		this.dgvDetalleLote = new System.Windows.Forms.DataGridView();
		this.txtDetalleCantidadDespachos = new System.Windows.Forms.TextBox();
		this.txtDetalleRutaLote = new System.Windows.Forms.TextBox();
		this.cbxDetalleOrigen = new System.Windows.Forms.ComboBox();
		this.cbxDetalleUsuarioDigitalizacion = new System.Windows.Forms.ComboBox();
		this.label24 = new System.Windows.Forms.Label();
		this.txtDetalleCdLote = new System.Windows.Forms.TextBox();
		this.cbxDetalleEstado = new System.Windows.Forms.ComboBox();
		this.label23 = new System.Windows.Forms.Label();
		this.cbxDetalleUsuario = new System.Windows.Forms.ComboBox();
		this.label15 = new System.Windows.Forms.Label();
		this.txtDetalleNombreLote = new System.Windows.Forms.TextBox();
		this.label22 = new System.Windows.Forms.Label();
		this.label16 = new System.Windows.Forms.Label();
		this.label21 = new System.Windows.Forms.Label();
		this.label17 = new System.Windows.Forms.Label();
		this.label20 = new System.Windows.Forms.Label();
		this.label18 = new System.Windows.Forms.Label();
		this.label19 = new System.Windows.Forms.Label();
		this.btnVerLote = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnExportar = new System.Windows.Forms.Button();
		this.btnBuscar = new System.Windows.Forms.Button();
		this.cbxUsuarioDigitalizacion = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.cbxOrigen = new System.Windows.Forms.ComboBox();
		this.label14 = new System.Windows.Forms.Label();
		this.pnlLotesEncontrados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesEncontrados).BeginInit();
		this.pnlDetalleLote.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDetalleLote).BeginInit();
		base.SuspendLayout();
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(12, 21);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(103, 16);
		this.label2.TabIndex = 1;
		this.label2.Text = "Código de Lote:";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(12, 54);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(111, 16);
		this.label3.TabIndex = 2;
		this.label3.Text = "Nombre del Lote:";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(12, 159);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(138, 16);
		this.label4.TabIndex = 3;
		this.label4.Text = "Fecha de Alta Desde:";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(278, 159);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(47, 16);
		this.label5.TabIndex = 4;
		this.label5.Text = "Hasta:";
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.ForeColor = System.Drawing.Color.White;
		this.label6.Location = new System.Drawing.Point(12, 87);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(58, 16);
		this.label6.TabIndex = 5;
		this.label6.Text = "Usuario:";
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.ForeColor = System.Drawing.Color.White;
		this.label7.Location = new System.Drawing.Point(467, 54);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(64, 16);
		this.label7.TabIndex = 6;
		this.label7.Text = "Nro Guia:";
		this.label8.AutoSize = true;
		this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label8.ForeColor = System.Drawing.Color.White;
		this.label8.Location = new System.Drawing.Point(467, 87);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(118, 16);
		this.label8.TabIndex = 7;
		this.label8.Text = "Serie Documental:";
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label9.ForeColor = System.Drawing.Color.White;
		this.label9.Location = new System.Drawing.Point(467, 123);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(51, 16);
		this.label9.TabIndex = 8;
		this.label9.Text = "SIGEA:";
		this.label10.AutoSize = true;
		this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label10.ForeColor = System.Drawing.Color.White;
		this.label10.Location = new System.Drawing.Point(467, 21);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(74, 16);
		this.label10.TabIndex = 9;
		this.label10.Text = "Despacho:";
		this.label11.AutoSize = true;
		this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label11.ForeColor = System.Drawing.Color.White;
		this.label11.Location = new System.Drawing.Point(12, 123);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(54, 16);
		this.label11.TabIndex = 10;
		this.label11.Text = "Estado:";
		this.label12.AutoSize = true;
		this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label12.ForeColor = System.Drawing.Color.White;
		this.label12.Location = new System.Drawing.Point(278, 193);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(47, 16);
		this.label12.TabIndex = 12;
		this.label12.Text = "Hasta:";
		this.label13.AutoSize = true;
		this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label13.ForeColor = System.Drawing.Color.White;
		this.label13.Location = new System.Drawing.Point(12, 192);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(147, 16);
		this.label13.TabIndex = 11;
		this.label13.Text = "Fecha de Final. Desde:";
		this.txtCodigoLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCodigoLote.Location = new System.Drawing.Point(184, 18);
		this.txtCodigoLote.Name = "txtCodigoLote";
		this.txtCodigoLote.Size = new System.Drawing.Size(230, 22);
		this.txtCodigoLote.TabIndex = 17;
		this.txtNombreLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNombreLote.Location = new System.Drawing.Point(184, 51);
		this.txtNombreLote.Name = "txtNombreLote";
		this.txtNombreLote.Size = new System.Drawing.Size(230, 22);
		this.txtNombreLote.TabIndex = 18;
		this.cbxListaUsuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaUsuarios.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaUsuarios.FormattingEnabled = true;
		this.cbxListaUsuarios.Location = new System.Drawing.Point(184, 84);
		this.cbxListaUsuarios.Name = "cbxListaUsuarios";
		this.cbxListaUsuarios.Size = new System.Drawing.Size(230, 25);
		this.cbxListaUsuarios.TabIndex = 19;
		this.cbxListaEstados.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaEstados.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaEstados.FormattingEnabled = true;
		this.cbxListaEstados.Location = new System.Drawing.Point(184, 120);
		this.cbxListaEstados.Name = "cbxListaEstados";
		this.cbxListaEstados.Size = new System.Drawing.Size(230, 25);
		this.cbxListaEstados.TabIndex = 20;
		this.txtFechaAltaDesde.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFechaAltaDesde.Location = new System.Drawing.Point(184, 156);
		this.txtFechaAltaDesde.Mask = "00/00/0000";
		this.txtFechaAltaDesde.Name = "txtFechaAltaDesde";
		this.txtFechaAltaDesde.Size = new System.Drawing.Size(77, 23);
		this.txtFechaAltaDesde.TabIndex = 21;
		this.txtFechaAltaDesde.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
		this.txtFechaAltaDesde.ValidatingType = typeof(System.DateTime);
		this.txtFechaAltaHasta.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFechaAltaHasta.Location = new System.Drawing.Point(337, 156);
		this.txtFechaAltaHasta.Mask = "00/00/0000";
		this.txtFechaAltaHasta.Name = "txtFechaAltaHasta";
		this.txtFechaAltaHasta.Size = new System.Drawing.Size(77, 23);
		this.txtFechaAltaHasta.TabIndex = 22;
		this.txtFechaAltaHasta.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
		this.txtFechaAltaHasta.ValidatingType = typeof(System.DateTime);
		this.txtDespacho.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDespacho.Location = new System.Drawing.Point(674, 18);
		this.txtDespacho.Name = "txtDespacho";
		this.txtDespacho.Size = new System.Drawing.Size(230, 22);
		this.txtDespacho.TabIndex = 23;
		this.txtNumeroGuia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNumeroGuia.Location = new System.Drawing.Point(674, 51);
		this.txtNumeroGuia.Name = "txtNumeroGuia";
		this.txtNumeroGuia.Size = new System.Drawing.Size(230, 22);
		this.txtNumeroGuia.TabIndex = 24;
		this.txtSerieDocumental.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtSerieDocumental.Location = new System.Drawing.Point(674, 84);
		this.txtSerieDocumental.Name = "txtSerieDocumental";
		this.txtSerieDocumental.Size = new System.Drawing.Size(230, 22);
		this.txtSerieDocumental.TabIndex = 25;
		this.txtSIGEA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtSIGEA.Location = new System.Drawing.Point(674, 120);
		this.txtSIGEA.Name = "txtSIGEA";
		this.txtSIGEA.Size = new System.Drawing.Size(230, 22);
		this.txtSIGEA.TabIndex = 26;
		this.txtFechaFinalizacionDesde.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFechaFinalizacionDesde.Location = new System.Drawing.Point(184, 190);
		this.txtFechaFinalizacionDesde.Mask = "00/00/0000";
		this.txtFechaFinalizacionDesde.Name = "txtFechaFinalizacionDesde";
		this.txtFechaFinalizacionDesde.Size = new System.Drawing.Size(77, 23);
		this.txtFechaFinalizacionDesde.TabIndex = 27;
		this.txtFechaFinalizacionDesde.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
		this.txtFechaFinalizacionDesde.ValidatingType = typeof(System.DateTime);
		this.txtFechaFinalizacionHasta.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFechaFinalizacionHasta.Location = new System.Drawing.Point(337, 190);
		this.txtFechaFinalizacionHasta.Mask = "00/00/0000";
		this.txtFechaFinalizacionHasta.Name = "txtFechaFinalizacionHasta";
		this.txtFechaFinalizacionHasta.Size = new System.Drawing.Size(77, 23);
		this.txtFechaFinalizacionHasta.TabIndex = 28;
		this.txtFechaFinalizacionHasta.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
		this.txtFechaFinalizacionHasta.ValidatingType = typeof(System.DateTime);
		this.pnlLotesEncontrados.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlLotesEncontrados.BackColor = System.Drawing.Color.Black;
		this.pnlLotesEncontrados.Controls.Add(this.dgvLotesEncontrados);
		this.pnlLotesEncontrados.Location = new System.Drawing.Point(15, 237);
		this.pnlLotesEncontrados.Name = "pnlLotesEncontrados";
		this.pnlLotesEncontrados.Size = new System.Drawing.Size(1184, 378);
		this.pnlLotesEncontrados.TabIndex = 57;
		this.dgvLotesEncontrados.AllowUserToAddRows = false;
		this.dgvLotesEncontrados.AllowUserToDeleteRows = false;
		this.dgvLotesEncontrados.AllowUserToResizeRows = false;
		this.dgvLotesEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.dgvLotesEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesEncontrados.DefaultCellStyle = dataGridViewCellStyle10;
		this.dgvLotesEncontrados.EnableHeadersVisualStyles = false;
		this.dgvLotesEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesEncontrados.Location = new System.Drawing.Point(13, 31);
		this.dgvLotesEncontrados.Name = "dgvLotesEncontrados";
		this.dgvLotesEncontrados.ReadOnly = true;
		this.dgvLotesEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
		this.dgvLotesEncontrados.RowHeadersVisible = false;
		this.dgvLotesEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle12.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle12;
		this.dgvLotesEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesEncontrados.Size = new System.Drawing.Size(1144, 308);
		this.dgvLotesEncontrados.TabIndex = 18;
		this.pnlDetalleLote.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlDetalleLote.BackColor = System.Drawing.Color.Black;
		this.pnlDetalleLote.Controls.Add(this.btnCerrarDetalle);
		this.pnlDetalleLote.Controls.Add(this.label26);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleFechaIndexacion);
		this.pnlDetalleLote.Controls.Add(this.label25);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleFechaControlCalidad);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleFechaFinalizacion);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleFechaAlta);
		this.pnlDetalleLote.Controls.Add(this.dgvDetalleLote);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleCantidadDespachos);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleRutaLote);
		this.pnlDetalleLote.Controls.Add(this.cbxDetalleOrigen);
		this.pnlDetalleLote.Controls.Add(this.cbxDetalleUsuarioDigitalizacion);
		this.pnlDetalleLote.Controls.Add(this.label24);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleCdLote);
		this.pnlDetalleLote.Controls.Add(this.cbxDetalleEstado);
		this.pnlDetalleLote.Controls.Add(this.label23);
		this.pnlDetalleLote.Controls.Add(this.cbxDetalleUsuario);
		this.pnlDetalleLote.Controls.Add(this.label15);
		this.pnlDetalleLote.Controls.Add(this.txtDetalleNombreLote);
		this.pnlDetalleLote.Controls.Add(this.label22);
		this.pnlDetalleLote.Controls.Add(this.label16);
		this.pnlDetalleLote.Controls.Add(this.label21);
		this.pnlDetalleLote.Controls.Add(this.label17);
		this.pnlDetalleLote.Controls.Add(this.label20);
		this.pnlDetalleLote.Controls.Add(this.label18);
		this.pnlDetalleLote.Controls.Add(this.label19);
		this.pnlDetalleLote.Location = new System.Drawing.Point(-1, 8);
		this.pnlDetalleLote.Name = "pnlDetalleLote";
		this.pnlDetalleLote.Size = new System.Drawing.Size(1207, 618);
		this.pnlDetalleLote.TabIndex = 58;
		this.pnlDetalleLote.Visible = false;
		this.btnCerrarDetalle.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.btnCerrarDetalle.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrarDetalle.FlatAppearance.BorderSize = 0;
		this.btnCerrarDetalle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrarDetalle.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrarDetalle.ForeColor = System.Drawing.Color.White;
		this.btnCerrarDetalle.Image = (System.Drawing.Image)resources.GetObject("btnCerrarDetalle.Image");
		this.btnCerrarDetalle.Location = new System.Drawing.Point(986, 51);
		this.btnCerrarDetalle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCerrarDetalle.Name = "btnCerrarDetalle";
		this.btnCerrarDetalle.Size = new System.Drawing.Size(175, 31);
		this.btnCerrarDetalle.TabIndex = 98;
		this.btnCerrarDetalle.Text = "   Cerrar Detalle";
		this.btnCerrarDetalle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCerrarDetalle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrarDetalle.UseVisualStyleBackColor = false;
		this.btnCerrarDetalle.Click += new System.EventHandler(btnCerrarDetalle_Click);
		this.label26.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label26.AutoSize = true;
		this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label26.ForeColor = System.Drawing.Color.White;
		this.label26.Location = new System.Drawing.Point(492, 124);
		this.label26.Name = "label26";
		this.label26.Size = new System.Drawing.Size(136, 16);
		this.label26.TabIndex = 118;
		this.label26.Text = "Fecha de Indexación:";
		this.txtDetalleFechaIndexacion.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleFechaIndexacion.Enabled = false;
		this.txtDetalleFechaIndexacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleFechaIndexacion.Location = new System.Drawing.Point(681, 121);
		this.txtDetalleFechaIndexacion.Name = "txtDetalleFechaIndexacion";
		this.txtDetalleFechaIndexacion.Size = new System.Drawing.Size(230, 22);
		this.txtDetalleFechaIndexacion.TabIndex = 117;
		this.label25.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label25.AutoSize = true;
		this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label25.ForeColor = System.Drawing.Color.White;
		this.label25.Location = new System.Drawing.Point(492, 91);
		this.label25.Name = "label25";
		this.label25.Size = new System.Drawing.Size(182, 16);
		this.label25.TabIndex = 116;
		this.label25.Text = "Fecha de Control de Calidad:";
		this.txtDetalleFechaControlCalidad.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleFechaControlCalidad.Enabled = false;
		this.txtDetalleFechaControlCalidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleFechaControlCalidad.Location = new System.Drawing.Point(681, 88);
		this.txtDetalleFechaControlCalidad.Name = "txtDetalleFechaControlCalidad";
		this.txtDetalleFechaControlCalidad.Size = new System.Drawing.Size(230, 22);
		this.txtDetalleFechaControlCalidad.TabIndex = 115;
		this.txtDetalleFechaFinalizacion.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleFechaFinalizacion.Enabled = false;
		this.txtDetalleFechaFinalizacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleFechaFinalizacion.Location = new System.Drawing.Point(681, 154);
		this.txtDetalleFechaFinalizacion.Name = "txtDetalleFechaFinalizacion";
		this.txtDetalleFechaFinalizacion.Size = new System.Drawing.Size(230, 22);
		this.txtDetalleFechaFinalizacion.TabIndex = 114;
		this.txtDetalleFechaAlta.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleFechaAlta.Enabled = false;
		this.txtDetalleFechaAlta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleFechaAlta.Location = new System.Drawing.Point(681, 55);
		this.txtDetalleFechaAlta.Name = "txtDetalleFechaAlta";
		this.txtDetalleFechaAlta.Size = new System.Drawing.Size(230, 22);
		this.txtDetalleFechaAlta.TabIndex = 113;
		this.dgvDetalleLote.AllowUserToAddRows = false;
		this.dgvDetalleLote.AllowUserToDeleteRows = false;
		this.dgvDetalleLote.AllowUserToResizeRows = false;
		this.dgvDetalleLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.dgvDetalleLote.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvDetalleLote.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvDetalleLote.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvDetalleLote.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvDetalleLote.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle13.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDetalleLote.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
		this.dgvDetalleLote.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvDetalleLote.DefaultCellStyle = dataGridViewCellStyle14;
		this.dgvDetalleLote.EnableHeadersVisualStyles = false;
		this.dgvDetalleLote.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvDetalleLote.Location = new System.Drawing.Point(26, 283);
		this.dgvDetalleLote.Name = "dgvDetalleLote";
		this.dgvDetalleLote.ReadOnly = true;
		this.dgvDetalleLote.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDetalleLote.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
		this.dgvDetalleLote.RowHeadersVisible = false;
		this.dgvDetalleLote.RowHeadersWidth = 15;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
		this.dgvDetalleLote.RowsDefaultCellStyle = dataGridViewCellStyle16;
		this.dgvDetalleLote.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvDetalleLote.Size = new System.Drawing.Size(1168, 281);
		this.dgvDetalleLote.TabIndex = 59;
		this.txtDetalleCantidadDespachos.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleCantidadDespachos.Enabled = false;
		this.txtDetalleCantidadDespachos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleCantidadDespachos.Location = new System.Drawing.Point(217, 209);
		this.txtDetalleCantidadDespachos.Name = "txtDetalleCantidadDespachos";
		this.txtDetalleCantidadDespachos.Size = new System.Drawing.Size(230, 22);
		this.txtDetalleCantidadDespachos.TabIndex = 112;
		this.txtDetalleRutaLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleRutaLote.Enabled = false;
		this.txtDetalleRutaLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleRutaLote.Location = new System.Drawing.Point(217, 158);
		this.txtDetalleRutaLote.Multiline = true;
		this.txtDetalleRutaLote.Name = "txtDetalleRutaLote";
		this.txtDetalleRutaLote.Size = new System.Drawing.Size(230, 41);
		this.txtDetalleRutaLote.TabIndex = 111;
		this.cbxDetalleOrigen.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.cbxDetalleOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxDetalleOrigen.Enabled = false;
		this.cbxDetalleOrigen.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxDetalleOrigen.FormattingEnabled = true;
		this.cbxDetalleOrigen.Location = new System.Drawing.Point(681, 223);
		this.cbxDetalleOrigen.Name = "cbxDetalleOrigen";
		this.cbxDetalleOrigen.Size = new System.Drawing.Size(230, 25);
		this.cbxDetalleOrigen.TabIndex = 110;
		this.cbxDetalleUsuarioDigitalizacion.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.cbxDetalleUsuarioDigitalizacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxDetalleUsuarioDigitalizacion.Enabled = false;
		this.cbxDetalleUsuarioDigitalizacion.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxDetalleUsuarioDigitalizacion.FormattingEnabled = true;
		this.cbxDetalleUsuarioDigitalizacion.Location = new System.Drawing.Point(681, 187);
		this.cbxDetalleUsuarioDigitalizacion.Name = "cbxDetalleUsuarioDigitalizacion";
		this.cbxDetalleUsuarioDigitalizacion.Size = new System.Drawing.Size(230, 25);
		this.cbxDetalleUsuarioDigitalizacion.TabIndex = 109;
		this.label24.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label24.AutoSize = true;
		this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label24.ForeColor = System.Drawing.Color.White;
		this.label24.Location = new System.Drawing.Point(28, 212);
		this.label24.Name = "label24";
		this.label24.Size = new System.Drawing.Size(157, 16);
		this.label24.TabIndex = 108;
		this.label24.Text = "Cantidad de Despachos:";
		this.txtDetalleCdLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleCdLote.Enabled = false;
		this.txtDetalleCdLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleCdLote.Location = new System.Drawing.Point(217, 55);
		this.txtDetalleCdLote.Name = "txtDetalleCdLote";
		this.txtDetalleCdLote.Size = new System.Drawing.Size(230, 22);
		this.txtDetalleCdLote.TabIndex = 98;
		this.cbxDetalleEstado.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.cbxDetalleEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxDetalleEstado.Enabled = false;
		this.cbxDetalleEstado.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxDetalleEstado.FormattingEnabled = true;
		this.cbxDetalleEstado.Location = new System.Drawing.Point(217, 88);
		this.cbxDetalleEstado.Name = "cbxDetalleEstado";
		this.cbxDetalleEstado.Size = new System.Drawing.Size(230, 25);
		this.cbxDetalleEstado.TabIndex = 100;
		this.label23.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label23.AutoSize = true;
		this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label23.ForeColor = System.Drawing.Color.White;
		this.label23.Location = new System.Drawing.Point(28, 161);
		this.label23.Name = "label23";
		this.label23.Size = new System.Drawing.Size(90, 16);
		this.label23.TabIndex = 106;
		this.label23.Text = "Ruta del Lote:";
		this.cbxDetalleUsuario.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.cbxDetalleUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxDetalleUsuario.Enabled = false;
		this.cbxDetalleUsuario.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxDetalleUsuario.FormattingEnabled = true;
		this.cbxDetalleUsuario.Location = new System.Drawing.Point(217, 242);
		this.cbxDetalleUsuario.Name = "cbxDetalleUsuario";
		this.cbxDetalleUsuario.Size = new System.Drawing.Size(230, 25);
		this.cbxDetalleUsuario.TabIndex = 99;
		this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label15.AutoSize = true;
		this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label15.ForeColor = System.Drawing.Color.White;
		this.label15.Location = new System.Drawing.Point(492, 226);
		this.label15.Name = "label15";
		this.label15.Size = new System.Drawing.Size(51, 16);
		this.label15.TabIndex = 105;
		this.label15.Text = "Origen:";
		this.txtDetalleNombreLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtDetalleNombreLote.Enabled = false;
		this.txtDetalleNombreLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetalleNombreLote.Location = new System.Drawing.Point(217, 125);
		this.txtDetalleNombreLote.Name = "txtDetalleNombreLote";
		this.txtDetalleNombreLote.Size = new System.Drawing.Size(230, 22);
		this.txtDetalleNombreLote.TabIndex = 98;
		this.label22.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label22.AutoSize = true;
		this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label22.ForeColor = System.Drawing.Color.White;
		this.label22.Location = new System.Drawing.Point(28, 58);
		this.label22.Name = "label22";
		this.label22.Size = new System.Drawing.Size(103, 16);
		this.label22.TabIndex = 98;
		this.label22.Text = "Código de Lote:";
		this.label16.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label16.AutoSize = true;
		this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label16.ForeColor = System.Drawing.Color.White;
		this.label16.Location = new System.Drawing.Point(492, 190);
		this.label16.Name = "label16";
		this.label16.Size = new System.Drawing.Size(141, 16);
		this.label16.TabIndex = 104;
		this.label16.Text = "Usuario Digitalización:";
		this.label21.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label21.AutoSize = true;
		this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label21.ForeColor = System.Drawing.Color.White;
		this.label21.Location = new System.Drawing.Point(28, 128);
		this.label21.Name = "label21";
		this.label21.Size = new System.Drawing.Size(111, 16);
		this.label21.TabIndex = 99;
		this.label21.Text = "Nombre del Lote:";
		this.label17.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label17.AutoSize = true;
		this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label17.ForeColor = System.Drawing.Color.White;
		this.label17.Location = new System.Drawing.Point(492, 157);
		this.label17.Name = "label17";
		this.label17.Size = new System.Drawing.Size(153, 16);
		this.label17.TabIndex = 103;
		this.label17.Text = "Fecha de Finalalización:";
		this.label20.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label20.AutoSize = true;
		this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label20.ForeColor = System.Drawing.Color.White;
		this.label20.Location = new System.Drawing.Point(492, 58);
		this.label20.Name = "label20";
		this.label20.Size = new System.Drawing.Size(94, 16);
		this.label20.TabIndex = 100;
		this.label20.Text = "Fecha de Alta:";
		this.label18.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label18.AutoSize = true;
		this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label18.ForeColor = System.Drawing.Color.White;
		this.label18.Location = new System.Drawing.Point(28, 91);
		this.label18.Name = "label18";
		this.label18.Size = new System.Drawing.Size(54, 16);
		this.label18.TabIndex = 102;
		this.label18.Text = "Estado:";
		this.label19.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label19.AutoSize = true;
		this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label19.ForeColor = System.Drawing.Color.White;
		this.label19.Location = new System.Drawing.Point(28, 245);
		this.label19.Name = "label19";
		this.label19.Size = new System.Drawing.Size(58, 16);
		this.label19.TabIndex = 101;
		this.label19.Text = "Usuario:";
		this.btnVerLote.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnVerLote.BackColor = System.Drawing.Color.SeaGreen;
		this.btnVerLote.FlatAppearance.BorderSize = 0;
		this.btnVerLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnVerLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnVerLote.ForeColor = System.Drawing.Color.White;
		this.btnVerLote.Image = (System.Drawing.Image)resources.GetObject("btnVerLote.Image");
		this.btnVerLote.Location = new System.Drawing.Point(1014, 48);
		this.btnVerLote.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnVerLote.Name = "btnVerLote";
		this.btnVerLote.Size = new System.Drawing.Size(175, 31);
		this.btnVerLote.TabIndex = 93;
		this.btnVerLote.Text = "   Ver Lote";
		this.btnVerLote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnVerLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnVerLote.UseVisualStyleBackColor = false;
		this.btnVerLote.Click += new System.EventHandler(btnVerLote_Click);
		this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(1014, 165);
		this.btnCerrar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(175, 31);
		this.btnCerrar.TabIndex = 92;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancelar.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(1014, 126);
		this.btnCancelar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(175, 31);
		this.btnCancelar.TabIndex = 91;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.btnExportar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnExportar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnExportar.FlatAppearance.BorderSize = 0;
		this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnExportar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnExportar.ForeColor = System.Drawing.Color.White;
		this.btnExportar.Image = (System.Drawing.Image)resources.GetObject("btnExportar.Image");
		this.btnExportar.Location = new System.Drawing.Point(1014, 87);
		this.btnExportar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnExportar.Name = "btnExportar";
		this.btnExportar.Size = new System.Drawing.Size(175, 31);
		this.btnExportar.TabIndex = 90;
		this.btnExportar.Text = "   Exportar";
		this.btnExportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnExportar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnExportar.UseVisualStyleBackColor = false;
		this.btnExportar.Click += new System.EventHandler(btnExportar_Click);
		this.btnBuscar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnBuscar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnBuscar.FlatAppearance.BorderSize = 0;
		this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnBuscar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBuscar.ForeColor = System.Drawing.Color.White;
		this.btnBuscar.Image = (System.Drawing.Image)resources.GetObject("btnBuscar.Image");
		this.btnBuscar.Location = new System.Drawing.Point(1014, 9);
		this.btnBuscar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnBuscar.Name = "btnBuscar";
		this.btnBuscar.Size = new System.Drawing.Size(175, 31);
		this.btnBuscar.TabIndex = 89;
		this.btnBuscar.Text = "   Buscar";
		this.btnBuscar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnBuscar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnBuscar.UseVisualStyleBackColor = false;
		this.btnBuscar.Click += new System.EventHandler(btnBuscar_Click);
		this.cbxUsuarioDigitalizacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxUsuarioDigitalizacion.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxUsuarioDigitalizacion.FormattingEnabled = true;
		this.cbxUsuarioDigitalizacion.Location = new System.Drawing.Point(674, 153);
		this.cbxUsuarioDigitalizacion.Name = "cbxUsuarioDigitalizacion";
		this.cbxUsuarioDigitalizacion.Size = new System.Drawing.Size(230, 25);
		this.cbxUsuarioDigitalizacion.TabIndex = 95;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(467, 156);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(141, 16);
		this.label1.TabIndex = 94;
		this.label1.Text = "Usuario Digitalización:";
		this.cbxOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxOrigen.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxOrigen.FormattingEnabled = true;
		this.cbxOrigen.Location = new System.Drawing.Point(674, 189);
		this.cbxOrigen.Name = "cbxOrigen";
		this.cbxOrigen.Size = new System.Drawing.Size(230, 25);
		this.cbxOrigen.TabIndex = 97;
		this.label14.AutoSize = true;
		this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label14.ForeColor = System.Drawing.Color.White;
		this.label14.Location = new System.Drawing.Point(467, 192);
		this.label14.Name = "label14";
		this.label14.Size = new System.Drawing.Size(51, 16);
		this.label14.TabIndex = 96;
		this.label14.Text = "Origen:";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1211, 627);
		base.Controls.Add(this.pnlDetalleLote);
		base.Controls.Add(this.cbxOrigen);
		base.Controls.Add(this.label14);
		base.Controls.Add(this.cbxUsuarioDigitalizacion);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.btnVerLote);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnExportar);
		base.Controls.Add(this.btnBuscar);
		base.Controls.Add(this.pnlLotesEncontrados);
		base.Controls.Add(this.txtFechaFinalizacionHasta);
		base.Controls.Add(this.txtFechaFinalizacionDesde);
		base.Controls.Add(this.txtSIGEA);
		base.Controls.Add(this.txtSerieDocumental);
		base.Controls.Add(this.txtNumeroGuia);
		base.Controls.Add(this.txtDespacho);
		base.Controls.Add(this.txtFechaAltaHasta);
		base.Controls.Add(this.txtFechaAltaDesde);
		base.Controls.Add(this.cbxListaEstados);
		base.Controls.Add(this.cbxListaUsuarios);
		base.Controls.Add(this.txtNombreLote);
		base.Controls.Add(this.txtCodigoLote);
		base.Controls.Add(this.label12);
		base.Controls.Add(this.label13);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.label10);
		base.Controls.Add(this.label9);
		base.Controls.Add(this.label8);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Name = "frmLoteConsultaDespachos";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Consulta de Lotes";
		base.Load += new System.EventHandler(frmLoteConsultaDespachos_Load);
		this.pnlLotesEncontrados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesEncontrados).EndInit();
		this.pnlDetalleLote.ResumeLayout(false);
		this.pnlDetalleLote.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDetalleLote).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
