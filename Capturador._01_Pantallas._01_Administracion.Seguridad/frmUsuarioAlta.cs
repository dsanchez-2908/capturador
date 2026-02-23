using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._01_Administracion.Seguridad;

public class frmUsuarioAlta : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private BindingList<eRol> rolesDisponibles = new BindingList<eRol>();

	private BindingList<eRol> rolesSeleccionados = new BindingList<eRol>();

	private BindingList<eProyecto> proyectosDisponibles = new BindingList<eProyecto>();

	private BindingList<eProyecto> proyectosSeleccionados = new BindingList<eProyecto>();

	private IContainer components = null;

	private TextBox txtMail;

	private Label label7;

	private TextBox txtApellido;

	private Label label6;

	private TextBox txtNombre;

	private Label label1;

	private TextBox txtUsuario;

	private Label label3;

	private TextBox txtClaveProvisoria;

	private Label label2;

	private Panel pnlRolesDisponibles;

	private DataGridView dgvRolesDisponibles;

	private Panel pnlRolesSeleccionados;

	private DataGridView dgvRolesSeleccionados;

	private Button btnQuitarRoles;

	private Button btnAgregarRoles;

	private Label label4;

	private Button btnQuitarProyectos;

	private Button btnAgregarProyectos;

	private Panel pnlProyectosSeleccionados;

	private DataGridView dgvProyectosSeleccionados;

	private Panel pnlProyectosDisponibles;

	private DataGridView dgvProyectosDisponibles;

	private Button btnAgregarUsuario;

	private Button btnCancelar;

	private TextBox txtDNI;

	private GroupBox gbRol;

	private GroupBox groupBox1;

	public event Action<string> OnProcesoTerminado;

	public frmUsuarioAlta(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmUsuarioAlta_Load(object sender, EventArgs e)
	{
		actualizarLotesDisponibles();
		inicializarRolesSeleccionados();
		actualizarProyectosDisponibles();
		inicializarProyectosSeleccionados();
	}

	private void actualizarLotesDisponibles()
	{
		dgvRolesDisponibles.Columns.Clear();
		rolesDisponibles = new BindingList<eRol>(nListas.ObtenerListaRoles(oUsuarioLogueado));
		dgvRolesDisponibles.DataSource = rolesDisponibles;
		DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
		chk.HeaderText = "Selección";
		chk.Name = "chkSeleccionado";
		chk.Width = 30;
		chk.ReadOnly = false;
		dgvRolesDisponibles.Columns.Insert(0, chk);
		dgvRolesDisponibles.ReadOnly = false;
		foreach (DataGridViewColumn col in dgvRolesDisponibles.Columns)
		{
			if (col.Name != "chkSeleccionado")
			{
				col.ReadOnly = true;
			}
		}
		dgvRolesDisponibles.Columns["cdRol"].HeaderText = "Código Rol";
		dgvRolesDisponibles.Columns["dsRol"].HeaderText = "Rol";
		dgvRolesDisponibles.Columns["cdRol"].DisplayIndex = 1;
		dgvRolesDisponibles.Columns["dsRol"].DisplayIndex = 2;
		dgvRolesDisponibles.Columns["cdRol"].Visible = false;
		dgvRolesDisponibles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		dgvRolesDisponibles.Columns["chkSeleccionado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvRolesDisponibles.Columns["dsRol"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Roles Disponibles";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvRolesDisponibles.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Roles: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvRolesDisponibles.Dock = DockStyle.Fill;
		pnlRolesDisponibles.Controls.Clear();
		pnlRolesDisponibles.Controls.Add(dgvRolesDisponibles);
		pnlRolesDisponibles.Controls.Add(labelTitulo);
		pnlRolesDisponibles.Controls.Add(labelTotal);
	}

	private void btnAgregarRoles_Click(object sender, EventArgs e)
	{
		List<DataGridViewRow> seleccionados = (from DataGridViewRow r in dgvRolesDisponibles.Rows
			where Convert.ToBoolean(r.Cells["chkSeleccionado"].Value)
			select r).ToList();
		foreach (DataGridViewRow row in seleccionados)
		{
			eRol rol = (eRol)row.DataBoundItem;
			if (!rolesSeleccionados.Any((eRol r) => r.cdRol == rol.cdRol))
			{
				rolesSeleccionados.Add(rol);
			}
			rolesDisponibles.Remove(rol);
		}
		actualizarTotalRoles(dgvRolesDisponibles, pnlRolesDisponibles);
		actualizarTotalRoles(dgvRolesSeleccionados, pnlRolesSeleccionados);
	}

	private void btnQuitarRoles_Click(object sender, EventArgs e)
	{
		List<DataGridViewRow> seleccionados = (from DataGridViewRow r in dgvRolesSeleccionados.Rows
			where Convert.ToBoolean(r.Cells["chkSeleccionado"].Value)
			select r).ToList();
		foreach (DataGridViewRow row in seleccionados)
		{
			eRol rol = (eRol)row.DataBoundItem;
			if (!rolesDisponibles.Any((eRol r) => r.cdRol == rol.cdRol))
			{
				rolesDisponibles.Add(rol);
			}
			rolesSeleccionados.Remove(rol);
		}
		actualizarTotalRoles(dgvRolesDisponibles, pnlRolesDisponibles);
		actualizarTotalRoles(dgvRolesSeleccionados, pnlRolesSeleccionados);
	}

	private void inicializarRolesSeleccionados()
	{
		dgvRolesSeleccionados.Columns.Clear();
		rolesSeleccionados = new BindingList<eRol>();
		dgvRolesSeleccionados.DataSource = rolesSeleccionados;
		DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
		chk.HeaderText = "Selección";
		chk.Name = "chkSeleccionado";
		chk.Width = 30;
		chk.ReadOnly = false;
		dgvRolesSeleccionados.Columns.Insert(0, chk);
		dgvRolesSeleccionados.Columns["cdRol"].Visible = false;
		dgvRolesSeleccionados.Columns["dsRol"].HeaderText = "Rol";
		dgvRolesSeleccionados.Columns["dsRol"].DisplayIndex = 1;
		dgvRolesSeleccionados.ReadOnly = false;
		foreach (DataGridViewColumn col in dgvRolesSeleccionados.Columns)
		{
			if (col.Name != "chkSeleccionado")
			{
				col.ReadOnly = true;
			}
		}
		dgvRolesSeleccionados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		dgvRolesSeleccionados.Columns["chkSeleccionado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvRolesSeleccionados.Columns["dsRol"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		dgvRolesSeleccionados.Dock = DockStyle.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Roles Seleccionados";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		Label labelTotal = new Label();
		labelTotal.Name = "labelTotalSeleccionados";
		labelTotal.Text = "Total de Roles: 0";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		pnlRolesSeleccionados.Controls.Clear();
		pnlRolesSeleccionados.Controls.Add(dgvRolesSeleccionados);
		pnlRolesSeleccionados.Controls.Add(labelTitulo);
		pnlRolesSeleccionados.Controls.Add(labelTotal);
	}

	private void actualizarTotalRoles(DataGridView dgv, Panel panelContenedor)
	{
		Label lbl = panelContenedor.Controls.OfType<Label>().FirstOrDefault((Label l) => l.Text.StartsWith("Total de Roles"));
		if (lbl != null)
		{
			lbl.Text = $"Total de Roles: {dgv.Rows.Count}";
		}
	}

	private void actualizarProyectosDisponibles()
	{
		dgvProyectosDisponibles.Columns.Clear();
		proyectosDisponibles = new BindingList<eProyecto>(nListas.ObtenerListaProyectosActivos(oUsuarioLogueado));
		dgvProyectosDisponibles.DataSource = proyectosDisponibles;
		DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
		chk.HeaderText = "Selección";
		chk.Name = "chkSeleccionado";
		chk.Width = 30;
		chk.ReadOnly = false;
		dgvProyectosDisponibles.Columns.Insert(0, chk);
		dgvProyectosDisponibles.ReadOnly = false;
		foreach (DataGridViewColumn col in dgvProyectosDisponibles.Columns)
		{
			if (col.Name != "chkSeleccionado")
			{
				col.ReadOnly = true;
			}
		}
		dgvProyectosDisponibles.Columns["cdProyecto"].HeaderText = "Código Proyecto";
		dgvProyectosDisponibles.Columns["dsProyecto"].HeaderText = "Proyecto";
		dgvProyectosDisponibles.Columns["snHabilitado"].HeaderText = "Habilitado";
		dgvProyectosDisponibles.Columns["cdProyecto"].DisplayIndex = 1;
		dgvProyectosDisponibles.Columns["dsProyecto"].DisplayIndex = 2;
		dgvProyectosDisponibles.Columns["snHabilitado"].DisplayIndex = 3;
		dgvProyectosDisponibles.Columns["cdProyecto"].Visible = false;
		dgvProyectosDisponibles.Columns["snHabilitado"].Visible = false;
		dgvProyectosDisponibles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		dgvProyectosDisponibles.Columns["chkSeleccionado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvProyectosDisponibles.Columns["dsProyecto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Proyectos Disponibles";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvProyectosDisponibles.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Proyectos: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvProyectosDisponibles.Dock = DockStyle.Fill;
		pnlProyectosDisponibles.Controls.Clear();
		pnlProyectosDisponibles.Controls.Add(dgvProyectosDisponibles);
		pnlProyectosDisponibles.Controls.Add(labelTitulo);
		pnlProyectosDisponibles.Controls.Add(labelTotal);
	}

	private void inicializarProyectosSeleccionados()
	{
		dgvProyectosSeleccionados.Columns.Clear();
		proyectosSeleccionados = new BindingList<eProyecto>();
		dgvProyectosSeleccionados.DataSource = proyectosSeleccionados;
		DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
		chk.HeaderText = "Selección";
		chk.Name = "chkSeleccionado";
		chk.Width = 30;
		chk.ReadOnly = false;
		dgvProyectosSeleccionados.Columns.Insert(0, chk);
		dgvProyectosSeleccionados.Columns["cdProyecto"].Visible = false;
		dgvProyectosSeleccionados.Columns["snHabilitado"].Visible = false;
		dgvProyectosSeleccionados.Columns["dsProyecto"].HeaderText = "Proyecto";
		dgvProyectosSeleccionados.Columns["dsProyecto"].DisplayIndex = 1;
		dgvProyectosSeleccionados.ReadOnly = false;
		foreach (DataGridViewColumn col in dgvProyectosSeleccionados.Columns)
		{
			if (col.Name != "chkSeleccionado")
			{
				col.ReadOnly = true;
			}
		}
		dgvProyectosSeleccionados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		dgvProyectosSeleccionados.Columns["chkSeleccionado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvProyectosSeleccionados.Columns["dsProyecto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		dgvProyectosSeleccionados.Dock = DockStyle.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Proyectos Seleccionados";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		Label labelTotal = new Label();
		labelTotal.Name = "labelTotalSeleccionados";
		labelTotal.Text = "Total de Proyectos: 0";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		pnlProyectosSeleccionados.Controls.Clear();
		pnlProyectosSeleccionados.Controls.Add(dgvProyectosSeleccionados);
		pnlProyectosSeleccionados.Controls.Add(labelTitulo);
		pnlProyectosSeleccionados.Controls.Add(labelTotal);
	}

	private void actualizarTotalProyectos(DataGridView dgv, Panel panelContenedor)
	{
		Label lbl = panelContenedor.Controls.OfType<Label>().FirstOrDefault((Label l) => l.Text.StartsWith("Total de Proyectos"));
		if (lbl != null)
		{
			lbl.Text = $"Total de Proyectos: {dgv.Rows.Count}";
		}
	}

	private void btnAgregarProyectos_Click(object sender, EventArgs e)
	{
		List<DataGridViewRow> seleccionados = (from DataGridViewRow r in dgvProyectosDisponibles.Rows
			where Convert.ToBoolean(r.Cells["chkSeleccionado"].Value)
			select r).ToList();
		foreach (DataGridViewRow row in seleccionados)
		{
			eProyecto proyecto = (eProyecto)row.DataBoundItem;
			if (!proyectosSeleccionados.Any((eProyecto r) => r.cdProyecto == proyecto.cdProyecto))
			{
				proyectosSeleccionados.Add(proyecto);
			}
			proyectosDisponibles.Remove(proyecto);
		}
		actualizarTotalProyectos(dgvProyectosDisponibles, pnlProyectosDisponibles);
		actualizarTotalProyectos(dgvProyectosSeleccionados, pnlProyectosSeleccionados);
	}

	private void btnQuitarProyectos_Click(object sender, EventArgs e)
	{
		List<DataGridViewRow> seleccionados = (from DataGridViewRow r in dgvProyectosSeleccionados.Rows
			where Convert.ToBoolean(r.Cells["chkSeleccionado"].Value)
			select r).ToList();
		foreach (DataGridViewRow row in seleccionados)
		{
			eProyecto proyecto = (eProyecto)row.DataBoundItem;
			if (!proyectosDisponibles.Any((eProyecto r) => r.cdProyecto == proyecto.cdProyecto))
			{
				proyectosDisponibles.Add(proyecto);
			}
			proyectosSeleccionados.Remove(proyecto);
		}
		actualizarTotalProyectos(dgvProyectosDisponibles, pnlProyectosDisponibles);
		actualizarTotalProyectos(dgvProyectosSeleccionados, pnlProyectosSeleccionados);
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnAgregarUsuario_Click(object sender, EventArgs e)
	{
		List<int> listaCdRolesSeleccionados = new List<int>();
		List<int> listaCdProyectosSeleccionados = new List<int>();
		foreach (DataGridViewRow row in (IEnumerable)dgvRolesSeleccionados.Rows)
		{
			listaCdRolesSeleccionados.Add(Convert.ToInt32(row.Cells["cdRol"].Value.ToString()));
		}
		foreach (DataGridViewRow row2 in (IEnumerable)dgvProyectosSeleccionados.Rows)
		{
			listaCdProyectosSeleccionados.Add(Convert.ToInt32(row2.Cells["cdProyecto"].Value.ToString()));
		}
		eUsuario oUsuario = new eUsuario();
		oUsuario.dsUsuario = txtUsuario.Text;
		oUsuario.dsNombre = txtNombre.Text;
		oUsuario.dsApellido = txtApellido.Text;
		oUsuario.nuDNI = txtDNI.Text;
		oUsuario.dsMail = txtMail.Text;
		oUsuario.dsClave = txtClaveProvisoria.Text;
		try
		{
			nSeguridad.agregarUsuario(oUsuarioLogueado, oUsuario, listaCdRolesSeleccionados, listaCdProyectosSeleccionados);
			MessageBox.Show("Se ingresó correctamente el usuario", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			TerminarProceso();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void TerminarProceso()
	{
		this.OnProcesoTerminado?.Invoke("");
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._01_Administracion.Seguridad.frmUsuarioAlta));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
		this.txtMail = new System.Windows.Forms.TextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.txtApellido = new System.Windows.Forms.TextBox();
		this.label6 = new System.Windows.Forms.Label();
		this.txtNombre = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.txtUsuario = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.txtClaveProvisoria = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.pnlRolesDisponibles = new System.Windows.Forms.Panel();
		this.dgvRolesDisponibles = new System.Windows.Forms.DataGridView();
		this.pnlRolesSeleccionados = new System.Windows.Forms.Panel();
		this.dgvRolesSeleccionados = new System.Windows.Forms.DataGridView();
		this.btnQuitarRoles = new System.Windows.Forms.Button();
		this.btnAgregarRoles = new System.Windows.Forms.Button();
		this.label4 = new System.Windows.Forms.Label();
		this.btnQuitarProyectos = new System.Windows.Forms.Button();
		this.btnAgregarProyectos = new System.Windows.Forms.Button();
		this.pnlProyectosSeleccionados = new System.Windows.Forms.Panel();
		this.dgvProyectosSeleccionados = new System.Windows.Forms.DataGridView();
		this.pnlProyectosDisponibles = new System.Windows.Forms.Panel();
		this.dgvProyectosDisponibles = new System.Windows.Forms.DataGridView();
		this.btnAgregarUsuario = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.txtDNI = new System.Windows.Forms.TextBox();
		this.gbRol = new System.Windows.Forms.GroupBox();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.pnlRolesDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvRolesDisponibles).BeginInit();
		this.pnlRolesSeleccionados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvRolesSeleccionados).BeginInit();
		this.pnlProyectosSeleccionados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosSeleccionados).BeginInit();
		this.pnlProyectosDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosDisponibles).BeginInit();
		this.gbRol.SuspendLayout();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.txtMail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtMail.Location = new System.Drawing.Point(625, 63);
		this.txtMail.Name = "txtMail";
		this.txtMail.Size = new System.Drawing.Size(230, 22);
		this.txtMail.TabIndex = 5;
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.ForeColor = System.Drawing.Color.White;
		this.label7.Location = new System.Drawing.Point(453, 66);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(36, 16);
		this.label7.TabIndex = 77;
		this.label7.Text = "Mail:";
		this.txtApellido.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtApellido.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtApellido.Location = new System.Drawing.Point(203, 96);
		this.txtApellido.Name = "txtApellido";
		this.txtApellido.Size = new System.Drawing.Size(230, 22);
		this.txtApellido.TabIndex = 3;
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.ForeColor = System.Drawing.Color.White;
		this.label6.Location = new System.Drawing.Point(31, 99);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(61, 16);
		this.label6.TabIndex = 75;
		this.label6.Text = "Apellido:";
		this.txtNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNombre.Location = new System.Drawing.Point(203, 63);
		this.txtNombre.Name = "txtNombre";
		this.txtNombre.Size = new System.Drawing.Size(230, 22);
		this.txtNombre.TabIndex = 2;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(31, 66);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(60, 16);
		this.label1.TabIndex = 73;
		this.label1.Text = "Nombre:";
		this.txtUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtUsuario.Location = new System.Drawing.Point(203, 30);
		this.txtUsuario.Name = "txtUsuario";
		this.txtUsuario.Size = new System.Drawing.Size(230, 22);
		this.txtUsuario.TabIndex = 1;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(31, 33);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(58, 16);
		this.label3.TabIndex = 64;
		this.label3.Text = "Usuario:";
		this.txtClaveProvisoria.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtClaveProvisoria.Location = new System.Drawing.Point(625, 96);
		this.txtClaveProvisoria.Name = "txtClaveProvisoria";
		this.txtClaveProvisoria.Size = new System.Drawing.Size(230, 22);
		this.txtClaveProvisoria.TabIndex = 7;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(453, 99);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(110, 16);
		this.label2.TabIndex = 79;
		this.label2.Text = "Clave Provisoria:";
		this.pnlRolesDisponibles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlRolesDisponibles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlRolesDisponibles.Controls.Add(this.dgvRolesDisponibles);
		this.pnlRolesDisponibles.Location = new System.Drawing.Point(21, 19);
		this.pnlRolesDisponibles.Name = "pnlRolesDisponibles";
		this.pnlRolesDisponibles.Size = new System.Drawing.Size(417, 207);
		this.pnlRolesDisponibles.TabIndex = 81;
		this.dgvRolesDisponibles.AllowUserToAddRows = false;
		this.dgvRolesDisponibles.AllowUserToDeleteRows = false;
		this.dgvRolesDisponibles.AllowUserToResizeRows = false;
		this.dgvRolesDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvRolesDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvRolesDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvRolesDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvRolesDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dgvRolesDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvRolesDisponibles.DefaultCellStyle = dataGridViewCellStyle2;
		this.dgvRolesDisponibles.EnableHeadersVisualStyles = false;
		this.dgvRolesDisponibles.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvRolesDisponibles.Location = new System.Drawing.Point(20, 37);
		this.dgvRolesDisponibles.Name = "dgvRolesDisponibles";
		this.dgvRolesDisponibles.ReadOnly = true;
		this.dgvRolesDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dgvRolesDisponibles.RowHeadersVisible = false;
		this.dgvRolesDisponibles.RowHeadersWidth = 15;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dgvRolesDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dgvRolesDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvRolesDisponibles.Size = new System.Drawing.Size(361, 139);
		this.dgvRolesDisponibles.TabIndex = 18;
		this.pnlRolesSeleccionados.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlRolesSeleccionados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlRolesSeleccionados.Controls.Add(this.dgvRolesSeleccionados);
		this.pnlRolesSeleccionados.Location = new System.Drawing.Point(644, 19);
		this.pnlRolesSeleccionados.Name = "pnlRolesSeleccionados";
		this.pnlRolesSeleccionados.Size = new System.Drawing.Size(417, 207);
		this.pnlRolesSeleccionados.TabIndex = 82;
		this.dgvRolesSeleccionados.AllowUserToAddRows = false;
		this.dgvRolesSeleccionados.AllowUserToDeleteRows = false;
		this.dgvRolesSeleccionados.AllowUserToResizeRows = false;
		this.dgvRolesSeleccionados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvRolesSeleccionados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvRolesSeleccionados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvRolesSeleccionados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvRolesSeleccionados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesSeleccionados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
		this.dgvRolesSeleccionados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvRolesSeleccionados.DefaultCellStyle = dataGridViewCellStyle6;
		this.dgvRolesSeleccionados.EnableHeadersVisualStyles = false;
		this.dgvRolesSeleccionados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvRolesSeleccionados.Location = new System.Drawing.Point(35, 37);
		this.dgvRolesSeleccionados.Name = "dgvRolesSeleccionados";
		this.dgvRolesSeleccionados.ReadOnly = true;
		this.dgvRolesSeleccionados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesSeleccionados.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dgvRolesSeleccionados.RowHeadersVisible = false;
		this.dgvRolesSeleccionados.RowHeadersWidth = 15;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		this.dgvRolesSeleccionados.RowsDefaultCellStyle = dataGridViewCellStyle8;
		this.dgvRolesSeleccionados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvRolesSeleccionados.Size = new System.Drawing.Size(337, 139);
		this.dgvRolesSeleccionados.TabIndex = 18;
		this.btnQuitarRoles.BackColor = System.Drawing.Color.SeaGreen;
		this.btnQuitarRoles.FlatAppearance.BorderSize = 0;
		this.btnQuitarRoles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnQuitarRoles.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnQuitarRoles.ForeColor = System.Drawing.Color.White;
		this.btnQuitarRoles.Image = (System.Drawing.Image)resources.GetObject("btnQuitarRoles.Image");
		this.btnQuitarRoles.Location = new System.Drawing.Point(456, 121);
		this.btnQuitarRoles.Name = "btnQuitarRoles";
		this.btnQuitarRoles.Size = new System.Drawing.Size(170, 25);
		this.btnQuitarRoles.TabIndex = 8;
		this.btnQuitarRoles.Text = "   Quitar Rol";
		this.btnQuitarRoles.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnQuitarRoles.UseVisualStyleBackColor = false;
		this.btnQuitarRoles.Click += new System.EventHandler(btnQuitarRoles_Click);
		this.btnAgregarRoles.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarRoles.FlatAppearance.BorderSize = 0;
		this.btnAgregarRoles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarRoles.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarRoles.ForeColor = System.Drawing.Color.White;
		this.btnAgregarRoles.Image = (System.Drawing.Image)resources.GetObject("btnAgregarRoles.Image");
		this.btnAgregarRoles.Location = new System.Drawing.Point(456, 90);
		this.btnAgregarRoles.Name = "btnAgregarRoles";
		this.btnAgregarRoles.Size = new System.Drawing.Size(170, 25);
		this.btnAgregarRoles.TabIndex = 7;
		this.btnAgregarRoles.Text = "   Agregar Rol";
		this.btnAgregarRoles.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarRoles.UseVisualStyleBackColor = false;
		this.btnAgregarRoles.Click += new System.EventHandler(btnAgregarRoles_Click);
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(453, 33);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(34, 16);
		this.label4.TabIndex = 85;
		this.label4.Text = "DNI:";
		this.btnQuitarProyectos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnQuitarProyectos.FlatAppearance.BorderSize = 0;
		this.btnQuitarProyectos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnQuitarProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnQuitarProyectos.ForeColor = System.Drawing.Color.White;
		this.btnQuitarProyectos.Image = (System.Drawing.Image)resources.GetObject("btnQuitarProyectos.Image");
		this.btnQuitarProyectos.Location = new System.Drawing.Point(456, 123);
		this.btnQuitarProyectos.Name = "btnQuitarProyectos";
		this.btnQuitarProyectos.Size = new System.Drawing.Size(170, 25);
		this.btnQuitarProyectos.TabIndex = 10;
		this.btnQuitarProyectos.Text = "   Quitar Proyecto";
		this.btnQuitarProyectos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnQuitarProyectos.UseVisualStyleBackColor = false;
		this.btnQuitarProyectos.Click += new System.EventHandler(btnQuitarProyectos_Click);
		this.btnAgregarProyectos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarProyectos.FlatAppearance.BorderSize = 0;
		this.btnAgregarProyectos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarProyectos.ForeColor = System.Drawing.Color.White;
		this.btnAgregarProyectos.Image = (System.Drawing.Image)resources.GetObject("btnAgregarProyectos.Image");
		this.btnAgregarProyectos.Location = new System.Drawing.Point(456, 92);
		this.btnAgregarProyectos.Name = "btnAgregarProyectos";
		this.btnAgregarProyectos.Size = new System.Drawing.Size(170, 25);
		this.btnAgregarProyectos.TabIndex = 9;
		this.btnAgregarProyectos.Text = "   Agregar Proyecto";
		this.btnAgregarProyectos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarProyectos.UseVisualStyleBackColor = false;
		this.btnAgregarProyectos.Click += new System.EventHandler(btnAgregarProyectos_Click);
		this.pnlProyectosSeleccionados.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlProyectosSeleccionados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlProyectosSeleccionados.Controls.Add(this.dgvProyectosSeleccionados);
		this.pnlProyectosSeleccionados.Location = new System.Drawing.Point(644, 19);
		this.pnlProyectosSeleccionados.Name = "pnlProyectosSeleccionados";
		this.pnlProyectosSeleccionados.Size = new System.Drawing.Size(417, 207);
		this.pnlProyectosSeleccionados.TabIndex = 88;
		this.dgvProyectosSeleccionados.AllowUserToAddRows = false;
		this.dgvProyectosSeleccionados.AllowUserToDeleteRows = false;
		this.dgvProyectosSeleccionados.AllowUserToResizeRows = false;
		this.dgvProyectosSeleccionados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvProyectosSeleccionados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvProyectosSeleccionados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvProyectosSeleccionados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvProyectosSeleccionados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosSeleccionados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.dgvProyectosSeleccionados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvProyectosSeleccionados.DefaultCellStyle = dataGridViewCellStyle10;
		this.dgvProyectosSeleccionados.EnableHeadersVisualStyles = false;
		this.dgvProyectosSeleccionados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvProyectosSeleccionados.Location = new System.Drawing.Point(35, 37);
		this.dgvProyectosSeleccionados.Name = "dgvProyectosSeleccionados";
		this.dgvProyectosSeleccionados.ReadOnly = true;
		this.dgvProyectosSeleccionados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosSeleccionados.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
		this.dgvProyectosSeleccionados.RowHeadersVisible = false;
		this.dgvProyectosSeleccionados.RowHeadersWidth = 15;
		dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle12.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
		this.dgvProyectosSeleccionados.RowsDefaultCellStyle = dataGridViewCellStyle12;
		this.dgvProyectosSeleccionados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvProyectosSeleccionados.Size = new System.Drawing.Size(337, 139);
		this.dgvProyectosSeleccionados.TabIndex = 18;
		this.pnlProyectosDisponibles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlProyectosDisponibles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlProyectosDisponibles.Controls.Add(this.dgvProyectosDisponibles);
		this.pnlProyectosDisponibles.Location = new System.Drawing.Point(22, 19);
		this.pnlProyectosDisponibles.Name = "pnlProyectosDisponibles";
		this.pnlProyectosDisponibles.Size = new System.Drawing.Size(417, 207);
		this.pnlProyectosDisponibles.TabIndex = 87;
		this.dgvProyectosDisponibles.AllowUserToAddRows = false;
		this.dgvProyectosDisponibles.AllowUserToDeleteRows = false;
		this.dgvProyectosDisponibles.AllowUserToResizeRows = false;
		this.dgvProyectosDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvProyectosDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvProyectosDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvProyectosDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvProyectosDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle13.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
		this.dgvProyectosDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvProyectosDisponibles.DefaultCellStyle = dataGridViewCellStyle14;
		this.dgvProyectosDisponibles.EnableHeadersVisualStyles = false;
		this.dgvProyectosDisponibles.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvProyectosDisponibles.Location = new System.Drawing.Point(20, 37);
		this.dgvProyectosDisponibles.Name = "dgvProyectosDisponibles";
		this.dgvProyectosDisponibles.ReadOnly = true;
		this.dgvProyectosDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
		this.dgvProyectosDisponibles.RowHeadersVisible = false;
		this.dgvProyectosDisponibles.RowHeadersWidth = 15;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
		this.dgvProyectosDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle16;
		this.dgvProyectosDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvProyectosDisponibles.Size = new System.Drawing.Size(361, 139);
		this.dgvProyectosDisponibles.TabIndex = 18;
		this.btnAgregarUsuario.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnAgregarUsuario.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarUsuario.FlatAppearance.BorderSize = 0;
		this.btnAgregarUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarUsuario.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarUsuario.ForeColor = System.Drawing.Color.White;
		this.btnAgregarUsuario.Image = (System.Drawing.Image)resources.GetObject("btnAgregarUsuario.Image");
		this.btnAgregarUsuario.Location = new System.Drawing.Point(947, 21);
		this.btnAgregarUsuario.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnAgregarUsuario.Name = "btnAgregarUsuario";
		this.btnAgregarUsuario.Size = new System.Drawing.Size(175, 31);
		this.btnAgregarUsuario.TabIndex = 11;
		this.btnAgregarUsuario.Text = "   Agregar Usuario";
		this.btnAgregarUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnAgregarUsuario.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarUsuario.UseVisualStyleBackColor = false;
		this.btnAgregarUsuario.Click += new System.EventHandler(btnAgregarUsuario_Click);
		this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancelar.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(947, 66);
		this.btnCancelar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(175, 31);
		this.btnCancelar.TabIndex = 12;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.txtDNI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDNI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDNI.Location = new System.Drawing.Point(624, 30);
		this.txtDNI.Name = "txtDNI";
		this.txtDNI.Size = new System.Drawing.Size(230, 22);
		this.txtDNI.TabIndex = 4;
		this.gbRol.Controls.Add(this.pnlRolesSeleccionados);
		this.gbRol.Controls.Add(this.btnAgregarRoles);
		this.gbRol.Controls.Add(this.pnlRolesDisponibles);
		this.gbRol.Controls.Add(this.btnQuitarRoles);
		this.gbRol.Location = new System.Drawing.Point(32, 136);
		this.gbRol.Name = "gbRol";
		this.gbRol.Size = new System.Drawing.Size(1090, 240);
		this.gbRol.TabIndex = 103;
		this.gbRol.TabStop = false;
		this.groupBox1.Controls.Add(this.pnlProyectosDisponibles);
		this.groupBox1.Controls.Add(this.btnAgregarProyectos);
		this.groupBox1.Controls.Add(this.btnQuitarProyectos);
		this.groupBox1.Controls.Add(this.pnlProyectosSeleccionados);
		this.groupBox1.Location = new System.Drawing.Point(32, 387);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(1090, 240);
		this.groupBox1.TabIndex = 104;
		this.groupBox1.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1134, 636);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.gbRol);
		base.Controls.Add(this.txtDNI);
		base.Controls.Add(this.btnAgregarUsuario);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.txtClaveProvisoria);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.txtMail);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.txtApellido);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.txtNombre);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.txtUsuario);
		base.Controls.Add(this.label3);
		base.MaximizeBox = false;
		base.Name = "frmUsuarioAlta";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Alta de Usuario";
		base.Load += new System.EventHandler(frmUsuarioAlta_Load);
		this.pnlRolesDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvRolesDisponibles).EndInit();
		this.pnlRolesSeleccionados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvRolesSeleccionados).EndInit();
		this.pnlProyectosSeleccionados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosSeleccionados).EndInit();
		this.pnlProyectosDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosDisponibles).EndInit();
		this.gbRol.ResumeLayout(false);
		this.groupBox1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
