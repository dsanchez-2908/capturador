using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._06_Lotes;

public class frmIngresoLote : Form
{
	private eUsuario oUsuarioLogueado;

	private eProyectoConfiguracion oProyectoConfiguracion;

	private string carpetaSeleccionada;

	private string carpetaInicialConfigurada;

	private List<eLote> LotesIngresar;

	private IContainer components = null;

	private ComboBox cbxListaProyectos;

	private Label lblNombre;

	private Button btnSeleccionarCarpeta;

	private TextBox txtRutaCarpetaSeleccionada;

	private DataGridView dgvLotesEncontrados;

	private Button btnBuscarLotes;

	private DataGridView dgvLotesYaExistentes;

	private Button btnIngresarLotes;

	private Button btnCerrar;

	private Button btnCancelar;

	private Panel pnlLotesEncontrados;

	private Panel pnlLotesYaIngresados;

	public frmIngresoLote(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmIngresoLote_Load(object sender, EventArgs e)
	{
		cargarListas();
		oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		carpetaInicialConfigurada = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		txtRutaCarpetaSeleccionada.Text = carpetaInicialConfigurada;
		carpetaSeleccionada = carpetaInicialConfigurada;
		ajustarGridLotesEncontrados();
		ajustarGridLotesYaEncontrados();
		ajustarFormulario_SeleccionarCarpeta();
	}

	private void cargarListas()
	{
		cbxListaProyectos.DataSource = nListas.ObtenerListaProyectosActivos(oUsuarioLogueado);
		cbxListaProyectos.DisplayMember = "dsProyecto";
		cbxListaProyectos.ValueMember = "cdProyecto";
	}

	private void ajustarGridLotesEncontrados()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Lotes Encontrados";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		dgvLotesEncontrados.Dock = DockStyle.Fill;
		pnlLotesEncontrados.Controls.Clear();
		pnlLotesEncontrados.Controls.Add(dgvLotesEncontrados);
		pnlLotesEncontrados.Controls.Add(labelTitulo);
	}

	private void ajustarGridLotesYaEncontrados()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Lotes Encontrados";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		dgvLotesYaExistentes.Dock = DockStyle.Fill;
		pnlLotesYaIngresados.Controls.Clear();
		pnlLotesYaIngresados.Controls.Add(dgvLotesYaExistentes);
		pnlLotesYaIngresados.Controls.Add(labelTitulo);
	}

	private void ajustarFormulario_SeleccionarCarpeta()
	{
		base.Size = new Size(800, 175);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_ConYaIngresados()
	{
		base.Size = new Size(1500, 500);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_SinYaIngresados()
	{
		base.Size = new Size(800, 500);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void btnSeleccionarCarpeta_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLote = new FolderBrowserDialog();
		oSeleccionarLote.Description = "Seleccionar la carpeta a procesar";
		oSeleccionarLote.SelectedPath = carpetaInicialConfigurada;
		oSeleccionarLote.ShowNewFolderButton = false;
		if (oSeleccionarLote.ShowDialog() == DialogResult.OK)
		{
			carpetaSeleccionada = oSeleccionarLote.SelectedPath;
			txtRutaCarpetaSeleccionada.Text = carpetaSeleccionada;
			nConfiguracion.actualizarUltimaCarpetaOrigen(oUsuarioLogueado, 1, carpetaInicialConfigurada, carpetaSeleccionada);
			cargarLotes();
		}
	}

	private void btnBuscarLotes_Click(object sender, EventArgs e)
	{
		cargarLotes();
	}

	private void cargarLotes()
	{
		btnSeleccionarCarpeta.Enabled = false;
		btnSeleccionarCarpeta.BackColor = Color.DarkGray;
		btnBuscarLotes.Enabled = false;
		btnBuscarLotes.BackColor = Color.DarkGray;
		LotesIngresar = nLotes.buscarLotes(oUsuarioLogueado, Convert.ToInt32(cbxListaProyectos.SelectedValue), carpetaSeleccionada);
		dgvLotesEncontrados.DataSource = LotesIngresar;
		dgvLotesEncontrados.Columns["dsNombreLote"].HeaderText = "Lote";
		dgvLotesEncontrados.Columns["dsRutaLote"].HeaderText = "Ruta Lote";
		dgvLotesEncontrados.Columns["nuCantidadArchivos"].HeaderText = "Cantidad Archivos";
		dgvLotesEncontrados.Columns["dsNombreLote"].DisplayIndex = 0;
		dgvLotesEncontrados.Columns["dsRutaLote"].DisplayIndex = 1;
		dgvLotesEncontrados.Columns["nuCantidadArchivos"].DisplayIndex = 2;
		dgvLotesEncontrados.Columns["dsProyecto"].Visible = false;
		dgvLotesEncontrados.Columns["cdEstado"].Visible = false;
		dgvLotesEncontrados.Columns["cdUsuarioSalida"].Visible = false;
		dgvLotesEncontrados.Columns["feSalida"].Visible = false;
		dgvLotesEncontrados.Columns["cdUsuarioRestaurado"].Visible = false;
		dgvLotesEncontrados.Columns["feRestaurado"].Visible = false;
		dgvLotesEncontrados.Columns["cdUsuarioIndexacion"].Visible = false;
		dgvLotesEncontrados.Columns["feIndexacion"].Visible = false;
		dgvLotesEncontrados.Columns["cdUsuarioControlCalidad"].Visible = false;
		dgvLotesEncontrados.Columns["feControlCalidad"].Visible = false;
		dgvLotesEncontrados.Columns["cdUsuarioPreparado"].Visible = false;
		dgvLotesEncontrados.Columns["fePreparado"].Visible = false;
		dgvLotesEncontrados.Columns["cdUsuarioAlta"].Visible = false;
		dgvLotesEncontrados.Columns["feAlta"].Visible = false;
		dgvLotesEncontrados.Columns["cdProyecto"].Visible = false;
		dgvLotesEncontrados.Columns["cdLote"].Visible = false;
		dgvLotesEncontrados.Columns["dsEstado"].Visible = false;
		dgvLotesEncontrados.Columns["dsUsuarioAlta"].Visible = false;
		dgvLotesEncontrados.Columns["dsRutaLoteFinal"].Visible = false;
		dgvLotesEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		List<eLote> lotesExistentes = nLotes.validarSiExiste(oUsuarioLogueado, LotesIngresar);
		if (lotesExistentes.Count > 0)
		{
			MessageBox.Show("La carpeta seleccionada tiene lotes ya ingresado", "Advertemcoa", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			dgvLotesYaExistentes.DataSource = lotesExistentes;
			dgvLotesYaExistentes.Columns["cdLote"].HeaderText = "Código Lote";
			dgvLotesYaExistentes.Columns["dsNombreLote"].HeaderText = "Nombre del Lote";
			dgvLotesYaExistentes.Columns["nuCantidadArchivos"].HeaderText = "Cantidad de Archivos";
			dgvLotesYaExistentes.Columns["feAlta"].HeaderText = "Fecha de Alta";
			dgvLotesYaExistentes.Columns["dsUsuarioAlta"].HeaderText = "Usuario de Alta";
			dgvLotesYaExistentes.Columns["dsEstado"].HeaderText = "Estado";
			dgvLotesYaExistentes.Columns["cdLote"].DisplayIndex = 0;
			dgvLotesYaExistentes.Columns["dsNombreLote"].DisplayIndex = 1;
			dgvLotesYaExistentes.Columns["nuCantidadArchivos"].DisplayIndex = 2;
			dgvLotesYaExistentes.Columns["feAlta"].DisplayIndex = 3;
			dgvLotesYaExistentes.Columns["dsUsuarioAlta"].DisplayIndex = 4;
			dgvLotesYaExistentes.Columns["dsEstado"].DisplayIndex = 5;
			dgvLotesYaExistentes.Columns["dsProyecto"].Visible = false;
			dgvLotesYaExistentes.Columns["dsRutaLote"].Visible = false;
			dgvLotesYaExistentes.Columns["cdEstado"].Visible = false;
			dgvLotesYaExistentes.Columns["cdUsuarioSalida"].Visible = false;
			dgvLotesYaExistentes.Columns["feSalida"].Visible = false;
			dgvLotesYaExistentes.Columns["cdUsuarioRestaurado"].Visible = false;
			dgvLotesYaExistentes.Columns["feRestaurado"].Visible = false;
			dgvLotesYaExistentes.Columns["cdUsuarioIndexacion"].Visible = false;
			dgvLotesYaExistentes.Columns["feIndexacion"].Visible = false;
			dgvLotesYaExistentes.Columns["cdUsuarioControlCalidad"].Visible = false;
			dgvLotesYaExistentes.Columns["feControlCalidad"].Visible = false;
			dgvLotesYaExistentes.Columns["cdUsuarioPreparado"].Visible = false;
			dgvLotesYaExistentes.Columns["fePreparado"].Visible = false;
			dgvLotesYaExistentes.Columns["cdUsuarioAlta"].Visible = false;
			dgvLotesYaExistentes.Columns["cdProyecto"].Visible = false;
			dgvLotesYaExistentes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			btnIngresarLotes.Enabled = false;
			btnIngresarLotes.BackColor = Color.DarkGray;
			ajustarFormulario_ConYaIngresados();
		}
		else
		{
			ajustarFormulario_SinYaIngresados();
			btnIngresarLotes.Enabled = true;
			btnIngresarLotes.BackColor = Color.SeaGreen;
		}
	}

	private void btnIngresarLotes_Click(object sender, EventArgs e)
	{
		nLotes.ingresarLote(oUsuarioLogueado, LotesIngresar);
		MessageBox.Show("Los lotes se ingresaron correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		vaciarFormulario();
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void vaciarFormulario()
	{
		base.WindowState = FormWindowState.Normal;
		ajustarFormulario_SeleccionarCarpeta();
		dgvLotesEncontrados.DataSource = null;
		dgvLotesEncontrados.Columns.Clear();
		dgvLotesYaExistentes.DataSource = null;
		dgvLotesYaExistentes.Columns.Clear();
		btnSeleccionarCarpeta.Enabled = true;
		btnSeleccionarCarpeta.BackColor = Color.SeaGreen;
		btnBuscarLotes.Enabled = true;
		btnBuscarLotes.BackColor = Color.SeaGreen;
		btnIngresarLotes.Enabled = true;
		btnIngresarLotes.BackColor = Color.SeaGreen;
		LotesIngresar.Clear();
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		vaciarFormulario();
	}

	private void dgvLotesYaExistentes_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
	}

	private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
	}

	private void pnlLotesYaIngresados_Paint(object sender, PaintEventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._06_Lotes.frmIngresoLote));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		this.cbxListaProyectos = new System.Windows.Forms.ComboBox();
		this.lblNombre = new System.Windows.Forms.Label();
		this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
		this.txtRutaCarpetaSeleccionada = new System.Windows.Forms.TextBox();
		this.dgvLotesEncontrados = new System.Windows.Forms.DataGridView();
		this.btnBuscarLotes = new System.Windows.Forms.Button();
		this.dgvLotesYaExistentes = new System.Windows.Forms.DataGridView();
		this.btnIngresarLotes = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.pnlLotesEncontrados = new System.Windows.Forms.Panel();
		this.pnlLotesYaIngresados = new System.Windows.Forms.Panel();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesEncontrados).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesYaExistentes).BeginInit();
		this.pnlLotesEncontrados.SuspendLayout();
		base.SuspendLayout();
		this.cbxListaProyectos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaProyectos.FormattingEnabled = true;
		this.cbxListaProyectos.Location = new System.Drawing.Point(225, 22);
		this.cbxListaProyectos.Name = "cbxListaProyectos";
		this.cbxListaProyectos.Size = new System.Drawing.Size(528, 25);
		this.cbxListaProyectos.TabIndex = 9;
		this.lblNombre.AutoSize = true;
		this.lblNombre.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNombre.ForeColor = System.Drawing.Color.White;
		this.lblNombre.Location = new System.Drawing.Point(12, 25);
		this.lblNombre.Name = "lblNombre";
		this.lblNombre.Size = new System.Drawing.Size(147, 17);
		this.lblNombre.TabIndex = 10;
		this.lblNombre.Text = "Seleccionar Proyecto:";
		this.btnSeleccionarCarpeta.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarCarpeta.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarCarpeta.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarCarpeta.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarCarpeta.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarCarpeta.Image");
		this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(15, 60);
		this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
		this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(204, 25);
		this.btnSeleccionarCarpeta.TabIndex = 15;
		this.btnSeleccionarCarpeta.Text = "   Seleccionar Carpeta";
		this.btnSeleccionarCarpeta.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarCarpeta.UseVisualStyleBackColor = false;
		this.btnSeleccionarCarpeta.Click += new System.EventHandler(btnSeleccionarCarpeta_Click);
		this.txtRutaCarpetaSeleccionada.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtRutaCarpetaSeleccionada.Enabled = false;
		this.txtRutaCarpetaSeleccionada.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtRutaCarpetaSeleccionada.Location = new System.Drawing.Point(225, 61);
		this.txtRutaCarpetaSeleccionada.Multiline = true;
		this.txtRutaCarpetaSeleccionada.Name = "txtRutaCarpetaSeleccionada";
		this.txtRutaCarpetaSeleccionada.Size = new System.Drawing.Size(392, 55);
		this.txtRutaCarpetaSeleccionada.TabIndex = 16;
		this.dgvLotesEncontrados.AllowUserToAddRows = false;
		this.dgvLotesEncontrados.AllowUserToDeleteRows = false;
		this.dgvLotesEncontrados.AllowUserToResizeRows = false;
		this.dgvLotesEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dgvLotesEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesEncontrados.DefaultCellStyle = dataGridViewCellStyle2;
		this.dgvLotesEncontrados.EnableHeadersVisualStyles = false;
		this.dgvLotesEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesEncontrados.Location = new System.Drawing.Point(13, 31);
		this.dgvLotesEncontrados.Name = "dgvLotesEncontrados";
		this.dgvLotesEncontrados.ReadOnly = true;
		this.dgvLotesEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dgvLotesEncontrados.RowHeadersVisible = false;
		this.dgvLotesEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dgvLotesEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesEncontrados.Size = new System.Drawing.Size(711, 221);
		this.dgvLotesEncontrados.TabIndex = 18;
		this.btnBuscarLotes.BackColor = System.Drawing.Color.SeaGreen;
		this.btnBuscarLotes.FlatAppearance.BorderSize = 0;
		this.btnBuscarLotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnBuscarLotes.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBuscarLotes.ForeColor = System.Drawing.Color.White;
		this.btnBuscarLotes.Image = (System.Drawing.Image)resources.GetObject("btnBuscarLotes.Image");
		this.btnBuscarLotes.Location = new System.Drawing.Point(15, 91);
		this.btnBuscarLotes.Name = "btnBuscarLotes";
		this.btnBuscarLotes.Size = new System.Drawing.Size(204, 25);
		this.btnBuscarLotes.TabIndex = 37;
		this.btnBuscarLotes.Text = "   Buscar";
		this.btnBuscarLotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnBuscarLotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnBuscarLotes.UseVisualStyleBackColor = false;
		this.btnBuscarLotes.Click += new System.EventHandler(btnBuscarLotes_Click);
		this.dgvLotesYaExistentes.AllowUserToAddRows = false;
		this.dgvLotesYaExistentes.AllowUserToDeleteRows = false;
		this.dgvLotesYaExistentes.AllowUserToResizeRows = false;
		this.dgvLotesYaExistentes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesYaExistentes.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesYaExistentes.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesYaExistentes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesYaExistentes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesYaExistentes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
		this.dgvLotesYaExistentes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesYaExistentes.DefaultCellStyle = dataGridViewCellStyle6;
		this.dgvLotesYaExistentes.EnableHeadersVisualStyles = false;
		this.dgvLotesYaExistentes.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesYaExistentes.Location = new System.Drawing.Point(806, 167);
		this.dgvLotesYaExistentes.Name = "dgvLotesYaExistentes";
		this.dgvLotesYaExistentes.ReadOnly = true;
		this.dgvLotesYaExistentes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesYaExistentes.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dgvLotesYaExistentes.RowHeadersVisible = false;
		this.dgvLotesYaExistentes.RowHeadersWidth = 15;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesYaExistentes.RowsDefaultCellStyle = dataGridViewCellStyle8;
		this.dgvLotesYaExistentes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesYaExistentes.Size = new System.Drawing.Size(643, 207);
		this.dgvLotesYaExistentes.TabIndex = 38;
		this.dgvLotesYaExistentes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvLotesYaExistentes_CellContentClick);
		this.btnIngresarLotes.BackColor = System.Drawing.Color.SeaGreen;
		this.btnIngresarLotes.FlatAppearance.BorderSize = 0;
		this.btnIngresarLotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnIngresarLotes.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnIngresarLotes.ForeColor = System.Drawing.Color.White;
		this.btnIngresarLotes.Image = (System.Drawing.Image)resources.GetObject("btnIngresarLotes.Image");
		this.btnIngresarLotes.Location = new System.Drawing.Point(15, 409);
		this.btnIngresarLotes.Name = "btnIngresarLotes";
		this.btnIngresarLotes.Size = new System.Drawing.Size(237, 25);
		this.btnIngresarLotes.TabIndex = 53;
		this.btnIngresarLotes.Text = "   Ingresar Lotes";
		this.btnIngresarLotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnIngresarLotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnIngresarLotes.UseVisualStyleBackColor = false;
		this.btnIngresarLotes.Click += new System.EventHandler(btnIngresarLotes_Click);
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(623, 91);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(130, 24);
		this.btnCerrar.TabIndex = 55;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.btnCancelar.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(623, 61);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(130, 25);
		this.btnCancelar.TabIndex = 54;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.pnlLotesEncontrados.BackColor = System.Drawing.Color.Black;
		this.pnlLotesEncontrados.Controls.Add(this.dgvLotesEncontrados);
		this.pnlLotesEncontrados.Location = new System.Drawing.Point(12, 136);
		this.pnlLotesEncontrados.Name = "pnlLotesEncontrados";
		this.pnlLotesEncontrados.Size = new System.Drawing.Size(741, 255);
		this.pnlLotesEncontrados.TabIndex = 56;
		this.pnlLotesYaIngresados.BackColor = System.Drawing.Color.Black;
		this.pnlLotesYaIngresados.Location = new System.Drawing.Point(783, 136);
		this.pnlLotesYaIngresados.Name = "pnlLotesYaIngresados";
		this.pnlLotesYaIngresados.Size = new System.Drawing.Size(689, 255);
		this.pnlLotesYaIngresados.TabIndex = 57;
		this.pnlLotesYaIngresados.Paint += new System.Windows.Forms.PaintEventHandler(pnlLotesYaIngresados_Paint);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1484, 461);
		base.Controls.Add(this.dgvLotesYaExistentes);
		base.Controls.Add(this.pnlLotesEncontrados);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnIngresarLotes);
		base.Controls.Add(this.btnBuscarLotes);
		base.Controls.Add(this.txtRutaCarpetaSeleccionada);
		base.Controls.Add(this.btnSeleccionarCarpeta);
		base.Controls.Add(this.lblNombre);
		base.Controls.Add(this.cbxListaProyectos);
		base.Controls.Add(this.pnlLotesYaIngresados);
		base.MaximizeBox = false;
		base.Name = "frmIngresoLote";
		this.Text = "Ingreso de Lotes";
		base.Load += new System.EventHandler(frmIngresoLote_Load);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesEncontrados).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesYaExistentes).EndInit();
		this.pnlLotesEncontrados.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
