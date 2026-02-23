using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._01_Administracion;

public class frmListas : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private IContainer components = null;

	private Label lblNombre;

	private ComboBox cbxListaProyectos;

	private Label label1;

	private ComboBox cbxListaCampos;

	private Panel pnlValoresEncontrados;

	private DataGridView dgvValoresEncontrados;

	private TextBox txtCdValor;

	private Label label3;

	private TextBox txtDsValorLista;

	private Label label2;

	private ComboBox cbxSnHabilitado;

	private Label label4;

	private TextBox txtCdExterno;

	private Label label5;

	private Button btnAgregarValor;

	private Button btnModificarValor;

	private Button btnLimpiarCampos;

	private Button btnCerrar;

	public frmListas(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmListas_Load(object sender, EventArgs e)
	{
		cargarListasProyectos();
		cargarListasCampos();
		cargarListaHabilitado();
		llenarGrillaDatos();
	}

	private void cargarListasProyectos()
	{
		cbxListaProyectos.DataSource = nListas.ObtenerListaProyectosActivos(oUsuarioLogueado);
		cbxListaProyectos.DisplayMember = "dsProyecto";
		cbxListaProyectos.ValueMember = "cdProyecto";
	}

	private void cargarListasCampos()
	{
		int cdProyectoSeleccionado = 0;
		if (cbxListaProyectos.SelectedIndex != -1 && cbxListaProyectos.SelectedValue is int)
		{
			cdProyectoSeleccionado = (int)cbxListaProyectos.SelectedValue;
		}
		cbxListaCampos.DataSource = nListas.ObtenerListaNombreCampos(oUsuarioLogueado, cdProyectoSeleccionado);
		cbxListaCampos.DisplayMember = "dsCampo";
		cbxListaCampos.ValueMember = "cdCampo";
	}

	private void cargarListaHabilitado()
	{
		cbxSnHabilitado.Items.Add("SI");
		cbxSnHabilitado.Items.Add("NO");
	}

	private void cbxListaProyectos_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cbxListaProyectos.SelectedIndex != -1)
		{
			cargarListasCampos();
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		llenarGrillaDatos();
	}

	private void llenarGrillaDatos()
	{
		int cdProyectoSeleccionado = Convert.ToInt32(cbxListaProyectos.SelectedValue);
		int cdCampoSeleccionado = Convert.ToInt32(cbxListaCampos.SelectedValue);
		dgvValoresEncontrados.DataSource = nListas.ObtenerListaValor(oUsuarioLogueado, cdProyectoSeleccionado, cdCampoSeleccionado);
		dgvValoresEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		if (dgvValoresEncontrados.Columns.Contains("cdValor"))
		{
			dgvValoresEncontrados.Columns["cdValor"].HeaderText = "Código";
			dgvValoresEncontrados.Columns["cdValor"].DisplayIndex = 0;
		}
		if (dgvValoresEncontrados.Columns.Contains("dsValorLista"))
		{
			dgvValoresEncontrados.Columns["dsValorLista"].HeaderText = "Valor Lista";
			dgvValoresEncontrados.Columns["dsValorLista"].DisplayIndex = 1;
		}
		if (dgvValoresEncontrados.Columns.Contains("snHabilitado"))
		{
			dgvValoresEncontrados.Columns["snHabilitado"].HeaderText = "Habilitado";
			dgvValoresEncontrados.Columns["snHabilitado"].DisplayIndex = 2;
		}
		if (dgvValoresEncontrados.Columns.Contains("cdExterno"))
		{
			dgvValoresEncontrados.Columns["cdExterno"].HeaderText = "Código Externo";
			dgvValoresEncontrados.Columns["cdExterno"].DisplayIndex = 3;
		}
		dgvValoresEncontrados.Columns["dsCampo"].Visible = false;
		dgvValoresEncontrados.Columns["cdCampo"].Visible = false;
		dgvValoresEncontrados.Columns["dsProyecto"].Visible = false;
		dgvValoresEncontrados.Columns["cdProyecto"].Visible = false;
		dgvValoresEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Detalle de Lotes Encontrados";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvValoresEncontrados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Lotes Encontrados: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvValoresEncontrados.Dock = DockStyle.Fill;
		pnlValoresEncontrados.Controls.Clear();
		pnlValoresEncontrados.Controls.Add(dgvValoresEncontrados);
		pnlValoresEncontrados.Controls.Add(labelTitulo);
		pnlValoresEncontrados.Controls.Add(labelTotal);
	}

	private void dgvValoresEncontrados_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		txtCdValor.Text = dgvValoresEncontrados.CurrentRow.Cells["cdValor"].Value.ToString();
		txtDsValorLista.Text = dgvValoresEncontrados.CurrentRow.Cells["dsValorLista"].Value.ToString();
		cbxSnHabilitado.SelectedItem = dgvValoresEncontrados.CurrentRow.Cells["snHabilitado"].Value.ToString();
		txtCdExterno.Text = dgvValoresEncontrados.CurrentRow.Cells["cdExterno"].Value.ToString();
	}

	private void btnLimpiarCampos_Click(object sender, EventArgs e)
	{
		limpiarCampos();
	}

	private void limpiarCampos()
	{
		txtCdValor.Clear();
		txtDsValorLista.Clear();
		cbxSnHabilitado.SelectedIndex = -1;
		txtCdExterno.Clear();
	}

	private void btnAgregarValor_Click(object sender, EventArgs e)
	{
		eLista oListaValores = new eLista();
		oListaValores.cdProyecto = Convert.ToInt32(cbxListaProyectos.SelectedValue);
		oListaValores.cdCampo = Convert.ToInt32(cbxListaCampos.SelectedValue);
		oListaValores.cdExterno = txtCdExterno.Text;
		oListaValores.dsValorLista = txtDsValorLista.Text;
		oListaValores.snHabilitado = cbxSnHabilitado.Text;
		try
		{
			nListas.agregarLista(oUsuarioLogueado, oListaValores);
			MessageBox.Show("Se agregó a la lista correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			limpiarCampos();
			llenarGrillaDatos();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void btnModificarValor_Click(object sender, EventArgs e)
	{
		eLista oListaValores = new eLista();
		oListaValores.cdProyecto = Convert.ToInt32(cbxListaProyectos.SelectedValue);
		oListaValores.cdCampo = Convert.ToInt32(cbxListaCampos.SelectedValue);
		oListaValores.cdValor = Convert.ToInt32(txtCdValor.Text);
		oListaValores.cdExterno = txtCdExterno.Text;
		oListaValores.dsValorLista = txtDsValorLista.Text;
		oListaValores.snHabilitado = cbxSnHabilitado.Text;
		try
		{
			nListas.modificarLista(oUsuarioLogueado, oListaValores);
			MessageBox.Show("Se modificó la lista correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			limpiarCampos();
			llenarGrillaDatos();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void cbxListaCampos_SelectedIndexChanged(object sender, EventArgs e)
	{
		llenarGrillaDatos();
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._01_Administracion.frmListas));
		this.lblNombre = new System.Windows.Forms.Label();
		this.cbxListaProyectos = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.cbxListaCampos = new System.Windows.Forms.ComboBox();
		this.pnlValoresEncontrados = new System.Windows.Forms.Panel();
		this.dgvValoresEncontrados = new System.Windows.Forms.DataGridView();
		this.txtCdValor = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.txtDsValorLista = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.cbxSnHabilitado = new System.Windows.Forms.ComboBox();
		this.label4 = new System.Windows.Forms.Label();
		this.txtCdExterno = new System.Windows.Forms.TextBox();
		this.label5 = new System.Windows.Forms.Label();
		this.btnAgregarValor = new System.Windows.Forms.Button();
		this.btnModificarValor = new System.Windows.Forms.Button();
		this.btnLimpiarCampos = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.pnlValoresEncontrados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvValoresEncontrados).BeginInit();
		base.SuspendLayout();
		this.lblNombre.AutoSize = true;
		this.lblNombre.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNombre.ForeColor = System.Drawing.Color.White;
		this.lblNombre.Location = new System.Drawing.Point(30, 35);
		this.lblNombre.Name = "lblNombre";
		this.lblNombre.Size = new System.Drawing.Size(147, 17);
		this.lblNombre.TabIndex = 12;
		this.lblNombre.Text = "Seleccionar Proyecto:";
		this.cbxListaProyectos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaProyectos.FormattingEnabled = true;
		this.cbxListaProyectos.Location = new System.Drawing.Point(226, 32);
		this.cbxListaProyectos.Name = "cbxListaProyectos";
		this.cbxListaProyectos.Size = new System.Drawing.Size(528, 25);
		this.cbxListaProyectos.TabIndex = 11;
		this.cbxListaProyectos.SelectedIndexChanged += new System.EventHandler(cbxListaProyectos_SelectedIndexChanged);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(30, 91);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(141, 17);
		this.label1.TabIndex = 14;
		this.label1.Text = "Seleccionar Campo:";
		this.cbxListaCampos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaCampos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaCampos.FormattingEnabled = true;
		this.cbxListaCampos.Location = new System.Drawing.Point(226, 88);
		this.cbxListaCampos.Name = "cbxListaCampos";
		this.cbxListaCampos.Size = new System.Drawing.Size(528, 25);
		this.cbxListaCampos.TabIndex = 13;
		this.cbxListaCampos.SelectedIndexChanged += new System.EventHandler(cbxListaCampos_SelectedIndexChanged);
		this.pnlValoresEncontrados.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlValoresEncontrados.BackColor = System.Drawing.Color.Black;
		this.pnlValoresEncontrados.Controls.Add(this.dgvValoresEncontrados);
		this.pnlValoresEncontrados.Location = new System.Drawing.Point(33, 130);
		this.pnlValoresEncontrados.Name = "pnlValoresEncontrados";
		this.pnlValoresEncontrados.Size = new System.Drawing.Size(721, 269);
		this.pnlValoresEncontrados.TabIndex = 58;
		this.dgvValoresEncontrados.AllowUserToAddRows = false;
		this.dgvValoresEncontrados.AllowUserToDeleteRows = false;
		this.dgvValoresEncontrados.AllowUserToResizeRows = false;
		this.dgvValoresEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvValoresEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvValoresEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvValoresEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvValoresEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvValoresEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
		this.dgvValoresEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvValoresEncontrados.DefaultCellStyle = dataGridViewCellStyle6;
		this.dgvValoresEncontrados.EnableHeadersVisualStyles = false;
		this.dgvValoresEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvValoresEncontrados.Location = new System.Drawing.Point(13, 31);
		this.dgvValoresEncontrados.Name = "dgvValoresEncontrados";
		this.dgvValoresEncontrados.ReadOnly = true;
		this.dgvValoresEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvValoresEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dgvValoresEncontrados.RowHeadersVisible = false;
		this.dgvValoresEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		this.dgvValoresEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle8;
		this.dgvValoresEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvValoresEncontrados.Size = new System.Drawing.Size(678, 189);
		this.dgvValoresEncontrados.TabIndex = 18;
		this.dgvValoresEncontrados.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvValoresEncontrados_CellContentClick);
		this.txtCdValor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtCdValor.BackColor = System.Drawing.Color.DarkGray;
		this.txtCdValor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtCdValor.Enabled = false;
		this.txtCdValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCdValor.Location = new System.Drawing.Point(198, 426);
		this.txtCdValor.Name = "txtCdValor";
		this.txtCdValor.Size = new System.Drawing.Size(230, 22);
		this.txtCdValor.TabIndex = 65;
		this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(26, 429);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(55, 16);
		this.label3.TabIndex = 66;
		this.label3.Text = "Código:";
		this.txtDsValorLista.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtDsValorLista.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDsValorLista.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDsValorLista.Location = new System.Drawing.Point(198, 454);
		this.txtDsValorLista.Name = "txtDsValorLista";
		this.txtDsValorLista.Size = new System.Drawing.Size(230, 22);
		this.txtDsValorLista.TabIndex = 67;
		this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(26, 457);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(74, 16);
		this.label2.TabIndex = 68;
		this.label2.Text = "Valor Lista:";
		this.cbxSnHabilitado.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cbxSnHabilitado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxSnHabilitado.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxSnHabilitado.FormattingEnabled = true;
		this.cbxSnHabilitado.Location = new System.Drawing.Point(198, 482);
		this.cbxSnHabilitado.Name = "cbxSnHabilitado";
		this.cbxSnHabilitado.Size = new System.Drawing.Size(230, 25);
		this.cbxSnHabilitado.TabIndex = 69;
		this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(26, 485);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(73, 16);
		this.label4.TabIndex = 70;
		this.label4.Text = "Habilitado:";
		this.txtCdExterno.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtCdExterno.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtCdExterno.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCdExterno.Location = new System.Drawing.Point(198, 513);
		this.txtCdExterno.Name = "txtCdExterno";
		this.txtCdExterno.Size = new System.Drawing.Size(230, 22);
		this.txtCdExterno.TabIndex = 71;
		this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(26, 516);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(103, 16);
		this.label5.TabIndex = 72;
		this.label5.Text = "Código Externo:";
		this.btnAgregarValor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnAgregarValor.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarValor.FlatAppearance.BorderSize = 0;
		this.btnAgregarValor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarValor.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarValor.ForeColor = System.Drawing.Color.White;
		this.btnAgregarValor.Image = (System.Drawing.Image)resources.GetObject("btnAgregarValor.Image");
		this.btnAgregarValor.Location = new System.Drawing.Point(584, 420);
		this.btnAgregarValor.Name = "btnAgregarValor";
		this.btnAgregarValor.Size = new System.Drawing.Size(170, 25);
		this.btnAgregarValor.TabIndex = 73;
		this.btnAgregarValor.Text = "   Agregar Valor";
		this.btnAgregarValor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarValor.UseVisualStyleBackColor = false;
		this.btnAgregarValor.Click += new System.EventHandler(btnAgregarValor_Click);
		this.btnModificarValor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnModificarValor.BackColor = System.Drawing.Color.SeaGreen;
		this.btnModificarValor.FlatAppearance.BorderSize = 0;
		this.btnModificarValor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnModificarValor.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnModificarValor.ForeColor = System.Drawing.Color.White;
		this.btnModificarValor.Image = (System.Drawing.Image)resources.GetObject("btnModificarValor.Image");
		this.btnModificarValor.Location = new System.Drawing.Point(584, 451);
		this.btnModificarValor.Name = "btnModificarValor";
		this.btnModificarValor.Size = new System.Drawing.Size(170, 25);
		this.btnModificarValor.TabIndex = 74;
		this.btnModificarValor.Text = "   Modificar Valor";
		this.btnModificarValor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnModificarValor.UseVisualStyleBackColor = false;
		this.btnModificarValor.Click += new System.EventHandler(btnModificarValor_Click);
		this.btnLimpiarCampos.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnLimpiarCampos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnLimpiarCampos.FlatAppearance.BorderSize = 0;
		this.btnLimpiarCampos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnLimpiarCampos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnLimpiarCampos.ForeColor = System.Drawing.Color.White;
		this.btnLimpiarCampos.Image = (System.Drawing.Image)resources.GetObject("btnLimpiarCampos.Image");
		this.btnLimpiarCampos.Location = new System.Drawing.Point(584, 485);
		this.btnLimpiarCampos.Name = "btnLimpiarCampos";
		this.btnLimpiarCampos.Size = new System.Drawing.Size(170, 25);
		this.btnLimpiarCampos.TabIndex = 75;
		this.btnLimpiarCampos.Text = "   Limpiar Campos";
		this.btnLimpiarCampos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnLimpiarCampos.UseVisualStyleBackColor = false;
		this.btnLimpiarCampos.Click += new System.EventHandler(btnLimpiarCampos_Click);
		this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(584, 517);
		this.btnCerrar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(170, 25);
		this.btnCerrar.TabIndex = 76;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(784, 561);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnLimpiarCampos);
		base.Controls.Add(this.btnAgregarValor);
		base.Controls.Add(this.btnModificarValor);
		base.Controls.Add(this.txtCdExterno);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.cbxSnHabilitado);
		base.Controls.Add(this.txtDsValorLista);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.txtCdValor);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.pnlValoresEncontrados);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.cbxListaCampos);
		base.Controls.Add(this.lblNombre);
		base.Controls.Add(this.cbxListaProyectos);
		this.ForeColor = System.Drawing.Color.Black;
		base.Name = "frmListas";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Administración de Listas";
		base.Load += new System.EventHandler(frmListas_Load);
		this.pnlValoresEncontrados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvValoresEncontrados).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
