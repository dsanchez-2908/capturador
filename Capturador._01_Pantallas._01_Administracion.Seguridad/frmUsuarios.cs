using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._01_Administracion.Seguridad;

public class frmUsuarios : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private IContainer components = null;

	private MaskedTextBox txtFechaAltaHasta;

	private MaskedTextBox txtFechaAltaDesde;

	private ComboBox cbxEstados;

	private TextBox txtUsuario;

	private TextBox txtCodigoUsuario;

	private Label label11;

	private Label label5;

	private Label label4;

	private Label label3;

	private Label label2;

	private TextBox txtNombre;

	private Label label1;

	private TextBox txtApellido;

	private Label label6;

	private TextBox txtMail;

	private Label label7;

	private Panel pnlGrillaUsuarios;

	private DataGridView dgvUsuarios;

	private ComboBox cbxRol;

	private Label label8;

	private Button btnVerUsuario;

	private Button btnCerrar;

	private Button btnCancelar;

	private Button btnExportar;

	private Button btnBuscar;

	private Button btnAgregarUsuario;

	public frmUsuarios(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmUsuarios_Load(object sender, EventArgs e)
	{
		llenarListas();
		llenarGrillaResultadoUsuarios();
	}

	private void llenarListas()
	{
		cbxEstados.DataSource = nListas.ObtenerListaEstadosUsuarios(oUsuarioLogueado);
		cbxEstados.DisplayMember = "dsEstado";
		cbxEstados.ValueMember = "cdEstado";
		cbxEstados.SelectedIndex = -1;
		cbxRol.DataSource = nListas.ObtenerListaRoles(oUsuarioLogueado);
		cbxRol.DisplayMember = "dsRol";
		cbxRol.ValueMember = "cdRol";
		cbxRol.SelectedIndex = -1;
	}

	private void llenarGrillaResultadoUsuarios()
	{
		dgvUsuarios.DataSource = ObtenerTablaConsultaLote();
		dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Detalle de Usuarios";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvUsuarios.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Usuarios: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvUsuarios.Dock = DockStyle.Fill;
		pnlGrillaUsuarios.Controls.Clear();
		pnlGrillaUsuarios.Controls.Add(dgvUsuarios);
		pnlGrillaUsuarios.Controls.Add(labelTitulo);
		pnlGrillaUsuarios.Controls.Add(labelTotal);
	}

	private DataTable ObtenerTablaConsultaLote()
	{
		DataTable oTableResultadoConsulta = new DataTable();
		eUsuario oUsuario = new eUsuario();
		if (!string.IsNullOrEmpty(txtCodigoUsuario.Text))
		{
			oUsuario.cdUsuario = Convert.ToInt32(txtCodigoUsuario.Text);
		}
		if (!string.IsNullOrEmpty(txtUsuario.Text))
		{
			oUsuario.dsUsuario = txtUsuario.Text;
		}
		if (!string.IsNullOrEmpty(txtNombre.Text))
		{
			oUsuario.dsNombre = txtNombre.Text;
		}
		if (!string.IsNullOrEmpty(txtApellido.Text))
		{
			oUsuario.dsApellido = txtApellido.Text;
		}
		if (!string.IsNullOrEmpty(txtMail.Text))
		{
			oUsuario.dsMail = txtMail.Text;
		}
		if (cbxEstados.SelectedIndex != -1)
		{
			oUsuario.cdEstado = Convert.ToInt32(cbxEstados.SelectedValue);
		}
		if (!string.IsNullOrEmpty(txtFechaAltaDesde.Text))
		{
			oUsuario.feAltaDesde = DateTime.ParseExact(txtFechaAltaDesde.Text, "ddMMyyyy", CultureInfo.InvariantCulture);
		}
		if (!string.IsNullOrEmpty(txtFechaAltaHasta.Text))
		{
			oUsuario.feAltaHasta = DateTime.ParseExact(txtFechaAltaHasta.Text, "ddMMyyyy", CultureInfo.InvariantCulture);
		}
		if (cbxRol.SelectedIndex != -1)
		{
			oUsuario.cdRol = Convert.ToInt32(cbxRol.SelectedValue);
		}
		try
		{
			oTableResultadoConsulta = nSeguridad.ObtenerTablaUsuarios(oUsuarioLogueado, oUsuario);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		return oTableResultadoConsulta;
	}

	private void btnBuscar_Click(object sender, EventArgs e)
	{
		llenarGrillaResultadoUsuarios();
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void vaciarFormulario()
	{
		txtCodigoUsuario.Clear();
		txtUsuario.Clear();
		txtNombre.Clear();
		txtApellido.Clear();
		txtMail.Clear();
		cbxEstados.SelectedIndex = -1;
		txtFechaAltaDesde.Clear();
		txtFechaAltaHasta.Clear();
		cbxRol.SelectedIndex = -1;
		llenarGrillaResultadoUsuarios();
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		vaciarFormulario();
	}

	private void btnAgregarUsuario_Click(object sender, EventArgs e)
	{
		frmUsuarioAlta oFrmUsuarioAlta = new frmUsuarioAlta(oUsuarioLogueado);
		oFrmUsuarioAlta.Show();
		oFrmUsuarioAlta.OnProcesoTerminado += delegate
		{
			vaciarFormulario();
		};
		oFrmUsuarioAlta.Show();
	}

	private void btnVerUsuario_Click(object sender, EventArgs e)
	{
		int cdUsuarioSeleccionado = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells[0].Value.ToString());
		frmUsuarioVerModificar oFrmUsuarioVerModificar = new frmUsuarioVerModificar(oUsuarioLogueado, cdUsuarioSeleccionado);
		oFrmUsuarioVerModificar.OnProcesoTerminado += delegate
		{
			vaciarFormulario();
		};
		oFrmUsuarioVerModificar.Show();
	}

	private void btnExportar_Click(object sender, EventArgs e)
	{
		SaveFileDialog oSeleccionarArchivoGuardar = new SaveFileDialog();
		oSeleccionarArchivoGuardar.Filter = "Archivo Excel (*.xlxs)|*.xlsx|Archivo CSV (*.csv)|*.csv";
		oSeleccionarArchivoGuardar.Title = "Seleccione el Archivo a Exportar";
		oSeleccionarArchivoGuardar.FileName = "Detalle_de_Usuarios_" + DateTime.Now.ToString("yyyyMMdd");
		if (oSeleccionarArchivoGuardar.ShowDialog() == DialogResult.OK)
		{
			string archivoGenerar = oSeleccionarArchivoGuardar.FileName;
			DataTable oTablaExportar = (DataTable)dgvUsuarios.DataSource;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._01_Administracion.Seguridad.frmUsuarios));
		this.txtFechaAltaHasta = new System.Windows.Forms.MaskedTextBox();
		this.txtFechaAltaDesde = new System.Windows.Forms.MaskedTextBox();
		this.cbxEstados = new System.Windows.Forms.ComboBox();
		this.txtUsuario = new System.Windows.Forms.TextBox();
		this.txtCodigoUsuario = new System.Windows.Forms.TextBox();
		this.label11 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.txtNombre = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.txtApellido = new System.Windows.Forms.TextBox();
		this.label6 = new System.Windows.Forms.Label();
		this.txtMail = new System.Windows.Forms.TextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.pnlGrillaUsuarios = new System.Windows.Forms.Panel();
		this.dgvUsuarios = new System.Windows.Forms.DataGridView();
		this.cbxRol = new System.Windows.Forms.ComboBox();
		this.label8 = new System.Windows.Forms.Label();
		this.btnVerUsuario = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnExportar = new System.Windows.Forms.Button();
		this.btnBuscar = new System.Windows.Forms.Button();
		this.btnAgregarUsuario = new System.Windows.Forms.Button();
		this.pnlGrillaUsuarios.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvUsuarios).BeginInit();
		base.SuspendLayout();
		this.txtFechaAltaHasta.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFechaAltaHasta.Location = new System.Drawing.Point(780, 93);
		this.txtFechaAltaHasta.Mask = "00/00/0000";
		this.txtFechaAltaHasta.Name = "txtFechaAltaHasta";
		this.txtFechaAltaHasta.Size = new System.Drawing.Size(77, 23);
		this.txtFechaAltaHasta.TabIndex = 8;
		this.txtFechaAltaHasta.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
		this.txtFechaAltaHasta.ValidatingType = typeof(System.DateTime);
		this.txtFechaAltaDesde.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFechaAltaDesde.Location = new System.Drawing.Point(627, 93);
		this.txtFechaAltaDesde.Mask = "00/00/0000";
		this.txtFechaAltaDesde.Name = "txtFechaAltaDesde";
		this.txtFechaAltaDesde.Size = new System.Drawing.Size(77, 23);
		this.txtFechaAltaDesde.TabIndex = 7;
		this.txtFechaAltaDesde.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
		this.txtFechaAltaDesde.ValidatingType = typeof(System.DateTime);
		this.cbxEstados.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxEstados.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxEstados.FormattingEnabled = true;
		this.cbxEstados.Location = new System.Drawing.Point(627, 57);
		this.cbxEstados.Name = "cbxEstados";
		this.cbxEstados.Size = new System.Drawing.Size(230, 25);
		this.cbxEstados.TabIndex = 6;
		this.txtUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtUsuario.Location = new System.Drawing.Point(199, 57);
		this.txtUsuario.Name = "txtUsuario";
		this.txtUsuario.Size = new System.Drawing.Size(230, 22);
		this.txtUsuario.TabIndex = 2;
		this.txtCodigoUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtCodigoUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCodigoUsuario.Location = new System.Drawing.Point(199, 24);
		this.txtCodigoUsuario.Name = "txtCodigoUsuario";
		this.txtCodigoUsuario.Size = new System.Drawing.Size(230, 22);
		this.txtCodigoUsuario.TabIndex = 1;
		this.label11.AutoSize = true;
		this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label11.ForeColor = System.Drawing.Color.White;
		this.label11.Location = new System.Drawing.Point(455, 60);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(54, 16);
		this.label11.TabIndex = 27;
		this.label11.Text = "Estado:";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(721, 96);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(47, 16);
		this.label5.TabIndex = 26;
		this.label5.Text = "Hasta:";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(455, 96);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(138, 16);
		this.label4.TabIndex = 25;
		this.label4.Text = "Fecha de Alta Desde:";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(27, 60);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(58, 16);
		this.label3.TabIndex = 24;
		this.label3.Text = "Usuario:";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(27, 27);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(124, 16);
		this.label2.TabIndex = 23;
		this.label2.Text = "Código de Usuario:";
		this.txtNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNombre.Location = new System.Drawing.Point(199, 90);
		this.txtNombre.Name = "txtNombre";
		this.txtNombre.Size = new System.Drawing.Size(230, 22);
		this.txtNombre.TabIndex = 3;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(27, 93);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(60, 16);
		this.label1.TabIndex = 33;
		this.label1.Text = "Nombre:";
		this.txtApellido.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtApellido.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtApellido.Location = new System.Drawing.Point(199, 123);
		this.txtApellido.Name = "txtApellido";
		this.txtApellido.Size = new System.Drawing.Size(230, 22);
		this.txtApellido.TabIndex = 4;
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.ForeColor = System.Drawing.Color.White;
		this.label6.Location = new System.Drawing.Point(27, 126);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(61, 16);
		this.label6.TabIndex = 35;
		this.label6.Text = "Apellido:";
		this.txtMail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtMail.Location = new System.Drawing.Point(627, 24);
		this.txtMail.Name = "txtMail";
		this.txtMail.Size = new System.Drawing.Size(230, 22);
		this.txtMail.TabIndex = 5;
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.ForeColor = System.Drawing.Color.White;
		this.label7.Location = new System.Drawing.Point(455, 27);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(36, 16);
		this.label7.TabIndex = 37;
		this.label7.Text = "Mail:";
		this.pnlGrillaUsuarios.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlGrillaUsuarios.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlGrillaUsuarios.Controls.Add(this.dgvUsuarios);
		this.pnlGrillaUsuarios.Location = new System.Drawing.Point(30, 167);
		this.pnlGrillaUsuarios.Name = "pnlGrillaUsuarios";
		this.pnlGrillaUsuarios.Size = new System.Drawing.Size(1208, 407);
		this.pnlGrillaUsuarios.TabIndex = 60;
		this.dgvUsuarios.AllowUserToAddRows = false;
		this.dgvUsuarios.AllowUserToDeleteRows = false;
		this.dgvUsuarios.AllowUserToResizeRows = false;
		this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvUsuarios.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvUsuarios.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvUsuarios.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvUsuarios.DefaultCellStyle = dataGridViewCellStyle2;
		this.dgvUsuarios.EnableHeadersVisualStyles = false;
		this.dgvUsuarios.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvUsuarios.Location = new System.Drawing.Point(11, 42);
		this.dgvUsuarios.Name = "dgvUsuarios";
		this.dgvUsuarios.ReadOnly = true;
		this.dgvUsuarios.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvUsuarios.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dgvUsuarios.RowHeadersVisible = false;
		this.dgvUsuarios.RowHeadersWidth = 15;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dgvUsuarios.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvUsuarios.Size = new System.Drawing.Size(1171, 331);
		this.dgvUsuarios.TabIndex = 18;
		this.cbxRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxRol.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxRol.FormattingEnabled = true;
		this.cbxRol.Location = new System.Drawing.Point(627, 126);
		this.cbxRol.Name = "cbxRol";
		this.cbxRol.Size = new System.Drawing.Size(230, 25);
		this.cbxRol.TabIndex = 9;
		this.label8.AutoSize = true;
		this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label8.ForeColor = System.Drawing.Color.White;
		this.label8.Location = new System.Drawing.Point(455, 129);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(32, 16);
		this.label8.TabIndex = 61;
		this.label8.Text = "Rol:";
		this.btnVerUsuario.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnVerUsuario.BackColor = System.Drawing.Color.SeaGreen;
		this.btnVerUsuario.FlatAppearance.BorderSize = 0;
		this.btnVerUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnVerUsuario.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnVerUsuario.ForeColor = System.Drawing.Color.White;
		this.btnVerUsuario.Image = (System.Drawing.Image)resources.GetObject("btnVerUsuario.Image");
		this.btnVerUsuario.Location = new System.Drawing.Point(882, 63);
		this.btnVerUsuario.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnVerUsuario.Name = "btnVerUsuario";
		this.btnVerUsuario.Size = new System.Drawing.Size(175, 31);
		this.btnVerUsuario.TabIndex = 11;
		this.btnVerUsuario.Text = "   Ver Usuario";
		this.btnVerUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnVerUsuario.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnVerUsuario.UseVisualStyleBackColor = false;
		this.btnVerUsuario.Click += new System.EventHandler(btnVerUsuario_Click);
		this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(1063, 63);
		this.btnCerrar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(175, 31);
		this.btnCerrar.TabIndex = 15;
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
		this.btnCancelar.Location = new System.Drawing.Point(1064, 24);
		this.btnCancelar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(175, 31);
		this.btnCancelar.TabIndex = 14;
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
		this.btnExportar.Location = new System.Drawing.Point(882, 102);
		this.btnExportar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnExportar.Name = "btnExportar";
		this.btnExportar.Size = new System.Drawing.Size(175, 31);
		this.btnExportar.TabIndex = 12;
		this.btnExportar.TabStop = false;
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
		this.btnBuscar.Location = new System.Drawing.Point(882, 24);
		this.btnBuscar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnBuscar.Name = "btnBuscar";
		this.btnBuscar.Size = new System.Drawing.Size(175, 31);
		this.btnBuscar.TabIndex = 10;
		this.btnBuscar.Text = "   Buscar";
		this.btnBuscar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnBuscar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnBuscar.UseVisualStyleBackColor = false;
		this.btnBuscar.Click += new System.EventHandler(btnBuscar_Click);
		this.btnAgregarUsuario.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnAgregarUsuario.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarUsuario.FlatAppearance.BorderSize = 0;
		this.btnAgregarUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarUsuario.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarUsuario.ForeColor = System.Drawing.Color.White;
		this.btnAgregarUsuario.Image = (System.Drawing.Image)resources.GetObject("btnAgregarUsuario.Image");
		this.btnAgregarUsuario.Location = new System.Drawing.Point(1063, 102);
		this.btnAgregarUsuario.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnAgregarUsuario.Name = "btnAgregarUsuario";
		this.btnAgregarUsuario.Size = new System.Drawing.Size(175, 31);
		this.btnAgregarUsuario.TabIndex = 13;
		this.btnAgregarUsuario.Text = "   Agregar Usuario";
		this.btnAgregarUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnAgregarUsuario.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarUsuario.UseVisualStyleBackColor = false;
		this.btnAgregarUsuario.Click += new System.EventHandler(btnAgregarUsuario_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1251, 598);
		base.Controls.Add(this.btnAgregarUsuario);
		base.Controls.Add(this.btnVerUsuario);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnExportar);
		base.Controls.Add(this.btnBuscar);
		base.Controls.Add(this.cbxRol);
		base.Controls.Add(this.label8);
		base.Controls.Add(this.pnlGrillaUsuarios);
		base.Controls.Add(this.txtMail);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.txtApellido);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.txtNombre);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.txtFechaAltaHasta);
		base.Controls.Add(this.txtFechaAltaDesde);
		base.Controls.Add(this.cbxEstados);
		base.Controls.Add(this.txtUsuario);
		base.Controls.Add(this.txtCodigoUsuario);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Name = "frmUsuarios";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Administración de Usuarios";
		base.Load += new System.EventHandler(frmUsuarios_Load);
		this.pnlGrillaUsuarios.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvUsuarios).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
