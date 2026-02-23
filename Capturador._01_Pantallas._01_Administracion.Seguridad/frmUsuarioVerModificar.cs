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

public class frmUsuarioVerModificar : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private eUsuario oUsuarioSeleccionado = new eUsuario();

	private int cdUsuarioSeleccionado;

	private BindingList<eRol> rolesDisponibles = new BindingList<eRol>();

	private BindingList<eRol> rolesSeleccionados = new BindingList<eRol>();

	private BindingList<eProyecto> proyectosDisponibles = new BindingList<eProyecto>();

	private BindingList<eProyecto> proyectosSeleccionados = new BindingList<eProyecto>();

	private IContainer components = null;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

	private Label label8;

	private Label label9;

	private TextBox txtCdUsuario;

	private TextBox txtDsUsuario;

	private TextBox txtDsNombre;

	private TextBox txtDsApellido;

	private TextBox txtDsEstado;

	private TextBox txtFeAlta;

	private TextBox txtDsMail;

	private TextBox txtDsLegajo;

	private TextBox txtNuDNI;

	private Panel pnlRoles;

	private Panel pnlProyectos;

	private ListBox lboxRoles;

	private ListBox lboxProyectos;

	private TabControl tabControl1;

	private TabPage tabBlanquearClave;

	private TabPage tabModificarRoles;

	private TabPage tabModificarProyectos;

	private TextBox txtNuevaClave;

	private Label label10;

	private Button btnCerrar;

	private Button btnDesbloquearUsuario;

	private Panel pnlRolesSeleccionados;

	private DataGridView dgvRolesSeleccionados;

	private Button btnAgregarRoles;

	private Panel pnlRolesDisponibles;

	private DataGridView dgvRolesDisponibles;

	private Button btnQuitarRoles;

	private Panel pnlProyectosDisponibles;

	private DataGridView dgvProyectosDisponibles;

	private Button btnAgregarProyectos;

	private Button btnQuitarProyectos;

	private Panel pnlProyectosSeleccionados;

	private DataGridView dgvProyectosSeleccionados;

	private Button btnActualizarRoles;

	private Button btnActualizarProyectos;

	private TabPage tabEliminarUsuario;

	private Button btnEliminarUsuario;

	public event Action<string> OnProcesoTerminado;

	public frmUsuarioVerModificar(eUsuario pUsuarioLogueado, int pCdUsuarioSeleccionado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
		cdUsuarioSeleccionado = pCdUsuarioSeleccionado;
	}

	private void frmUsuarioVerModificar_Load(object sender, EventArgs e)
	{
		cargarUsuarioSeleccionado();
		cargarListaRolesProyectos();
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

	private void btnAgregarProyectos_Click_1(object sender, EventArgs e)
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

	private void btnQuitarProyectos_Click_1(object sender, EventArgs e)
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

	private void cargarUsuarioSeleccionado()
	{
		oUsuarioSeleccionado = nSeguridad.obtenerUsuario(oUsuarioLogueado, cdUsuarioSeleccionado);
		txtCdUsuario.Text = Convert.ToString(oUsuarioSeleccionado.cdUsuario);
		txtDsUsuario.Text = oUsuarioSeleccionado.dsUsuario;
		txtDsNombre.Text = oUsuarioSeleccionado.dsNombre;
		txtDsApellido.Text = oUsuarioSeleccionado.dsApellido;
		txtDsEstado.Text = oUsuarioSeleccionado.dsEstado;
		txtNuDNI.Text = oUsuarioSeleccionado.nuDNI;
		txtDsLegajo.Text = oUsuarioSeleccionado.dsLegajo;
		txtDsMail.Text = oUsuarioSeleccionado.dsMail;
		txtFeAlta.Text = Convert.ToString(oUsuarioSeleccionado.feAlta);
	}

	private void cargarListaRolesProyectos()
	{
		lboxRoles.DataSource = nSeguridad.obtenerListaRolesUsuario(oUsuarioLogueado, cdUsuarioSeleccionado);
		lboxProyectos.DataSource = nSeguridad.obtenerListaProyectosUsuario(oUsuarioLogueado, cdUsuarioSeleccionado);
		Label labelTituloRoles = new Label();
		labelTituloRoles.Text = "Roles Asociados";
		labelTituloRoles.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTituloRoles.BackColor = Color.FromArgb(26, 32, 40);
		labelTituloRoles.ForeColor = Color.White;
		labelTituloRoles.Dock = DockStyle.Top;
		labelTituloRoles.Height = 30;
		labelTituloRoles.TextAlign = ContentAlignment.MiddleCenter;
		lboxRoles.Dock = DockStyle.Fill;
		pnlRoles.Controls.Clear();
		pnlRoles.Controls.Add(lboxRoles);
		pnlRoles.Controls.Add(labelTituloRoles);
		Label labelTituloProyectos = new Label();
		labelTituloProyectos.Text = "Proyectos Asociados";
		labelTituloProyectos.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTituloProyectos.BackColor = Color.FromArgb(26, 32, 40);
		labelTituloProyectos.ForeColor = Color.White;
		labelTituloProyectos.Dock = DockStyle.Top;
		labelTituloProyectos.Height = 30;
		labelTituloProyectos.TextAlign = ContentAlignment.MiddleCenter;
		lboxProyectos.Dock = DockStyle.Fill;
		pnlProyectos.Controls.Clear();
		pnlProyectos.Controls.Add(lboxProyectos);
		pnlProyectos.Controls.Add(labelTituloProyectos);
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		TerminarProceso();
	}

	private void btnDesbloquearUsuario_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("¿Está seguro que quiere desbloquear el usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		if (string.IsNullOrEmpty(txtNuevaClave.Text))
		{
			MessageBox.Show("Debe ingresar una clave provisoria", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		try
		{
			nSeguridad.desbloquearUsuario(oUsuarioLogueado, cdUsuarioSeleccionado, txtNuevaClave.Text);
			MessageBox.Show("Se desbloqueo el usuario correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			TerminarProceso();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void btnActualizarRoles_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("¿Está seguro que quiere atualizar los roles del usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		List<int> listaCdRolesSeleccionados = new List<int>();
		foreach (DataGridViewRow row in (IEnumerable)dgvRolesSeleccionados.Rows)
		{
			listaCdRolesSeleccionados.Add(Convert.ToInt32(row.Cells["cdRol"].Value.ToString()));
		}
		try
		{
			nSeguridad.ActualizarRolesUsuario(oUsuarioLogueado, cdUsuarioSeleccionado, listaCdRolesSeleccionados);
			MessageBox.Show("Se actualizó los roles correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			TerminarProceso();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void btnActualizarProyectos_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("¿Está seguro que quiere atualizar los proyectos del usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		List<int> listaCdProyectosSeleccionados = new List<int>();
		foreach (DataGridViewRow row in (IEnumerable)dgvProyectosSeleccionados.Rows)
		{
			listaCdProyectosSeleccionados.Add(Convert.ToInt32(row.Cells["cdProyecto"].Value.ToString()));
		}
		try
		{
			nSeguridad.ActualizarProyectoUsuario(oUsuarioLogueado, cdUsuarioSeleccionado, listaCdProyectosSeleccionados);
			MessageBox.Show("Se actualizó los proyectos correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			TerminarProceso();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void btnEliminarUsuario_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("¿Está seguro que quiere eliminar el usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
		{
			try
			{
				nSeguridad.eliminarUsuario(oUsuarioLogueado, cdUsuarioSeleccionado);
				MessageBox.Show("Se marco el usuario como eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				TerminarProceso();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._01_Administracion.Seguridad.frmUsuarioVerModificar));
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
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.txtCdUsuario = new System.Windows.Forms.TextBox();
		this.txtDsUsuario = new System.Windows.Forms.TextBox();
		this.txtDsNombre = new System.Windows.Forms.TextBox();
		this.txtDsApellido = new System.Windows.Forms.TextBox();
		this.txtDsEstado = new System.Windows.Forms.TextBox();
		this.txtFeAlta = new System.Windows.Forms.TextBox();
		this.txtDsMail = new System.Windows.Forms.TextBox();
		this.txtDsLegajo = new System.Windows.Forms.TextBox();
		this.txtNuDNI = new System.Windows.Forms.TextBox();
		this.pnlRoles = new System.Windows.Forms.Panel();
		this.lboxRoles = new System.Windows.Forms.ListBox();
		this.pnlProyectos = new System.Windows.Forms.Panel();
		this.lboxProyectos = new System.Windows.Forms.ListBox();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tabBlanquearClave = new System.Windows.Forms.TabPage();
		this.btnDesbloquearUsuario = new System.Windows.Forms.Button();
		this.txtNuevaClave = new System.Windows.Forms.TextBox();
		this.label10 = new System.Windows.Forms.Label();
		this.tabModificarRoles = new System.Windows.Forms.TabPage();
		this.btnActualizarRoles = new System.Windows.Forms.Button();
		this.pnlRolesSeleccionados = new System.Windows.Forms.Panel();
		this.dgvRolesSeleccionados = new System.Windows.Forms.DataGridView();
		this.btnAgregarRoles = new System.Windows.Forms.Button();
		this.pnlRolesDisponibles = new System.Windows.Forms.Panel();
		this.dgvRolesDisponibles = new System.Windows.Forms.DataGridView();
		this.btnQuitarRoles = new System.Windows.Forms.Button();
		this.tabModificarProyectos = new System.Windows.Forms.TabPage();
		this.btnActualizarProyectos = new System.Windows.Forms.Button();
		this.pnlProyectosDisponibles = new System.Windows.Forms.Panel();
		this.dgvProyectosDisponibles = new System.Windows.Forms.DataGridView();
		this.btnAgregarProyectos = new System.Windows.Forms.Button();
		this.btnQuitarProyectos = new System.Windows.Forms.Button();
		this.pnlProyectosSeleccionados = new System.Windows.Forms.Panel();
		this.dgvProyectosSeleccionados = new System.Windows.Forms.DataGridView();
		this.tabEliminarUsuario = new System.Windows.Forms.TabPage();
		this.btnEliminarUsuario = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.pnlRoles.SuspendLayout();
		this.pnlProyectos.SuspendLayout();
		this.tabControl1.SuspendLayout();
		this.tabBlanquearClave.SuspendLayout();
		this.tabModificarRoles.SuspendLayout();
		this.pnlRolesSeleccionados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvRolesSeleccionados).BeginInit();
		this.pnlRolesDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvRolesDisponibles).BeginInit();
		this.tabModificarProyectos.SuspendLayout();
		this.pnlProyectosDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosDisponibles).BeginInit();
		this.pnlProyectosSeleccionados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosSeleccionados).BeginInit();
		this.tabEliminarUsuario.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(26, 34);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(144, 20);
		this.label1.TabIndex = 0;
		this.label1.Text = "Código de Usuario:";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(26, 72);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(68, 20);
		this.label2.TabIndex = 1;
		this.label2.Text = "Usuario:";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(26, 110);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(69, 20);
		this.label3.TabIndex = 2;
		this.label3.Text = "Nombre:";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(26, 148);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(69, 20);
		this.label4.TabIndex = 3;
		this.label4.Text = "Apellido:";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(366, 35);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(41, 20);
		this.label5.TabIndex = 4;
		this.label5.Text = "DNI:";
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.ForeColor = System.Drawing.Color.White;
		this.label6.Location = new System.Drawing.Point(366, 73);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(61, 20);
		this.label6.TabIndex = 5;
		this.label6.Text = "Legajo:";
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.ForeColor = System.Drawing.Color.White;
		this.label7.Location = new System.Drawing.Point(366, 111);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(41, 20);
		this.label7.TabIndex = 6;
		this.label7.Text = "Mail:";
		this.label8.AutoSize = true;
		this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label8.ForeColor = System.Drawing.Color.White;
		this.label8.Location = new System.Drawing.Point(366, 149);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(112, 20);
		this.label8.TabIndex = 7;
		this.label8.Text = "Fecha de Alta:";
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label9.ForeColor = System.Drawing.Color.White;
		this.label9.Location = new System.Drawing.Point(26, 186);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(64, 20);
		this.label9.TabIndex = 8;
		this.label9.Text = "Estado:";
		this.txtCdUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtCdUsuario.Enabled = false;
		this.txtCdUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCdUsuario.Location = new System.Drawing.Point(177, 34);
		this.txtCdUsuario.Name = "txtCdUsuario";
		this.txtCdUsuario.Size = new System.Drawing.Size(150, 22);
		this.txtCdUsuario.TabIndex = 9;
		this.txtDsUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDsUsuario.Enabled = false;
		this.txtDsUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDsUsuario.Location = new System.Drawing.Point(177, 72);
		this.txtDsUsuario.Name = "txtDsUsuario";
		this.txtDsUsuario.Size = new System.Drawing.Size(150, 22);
		this.txtDsUsuario.TabIndex = 10;
		this.txtDsNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDsNombre.Enabled = false;
		this.txtDsNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDsNombre.Location = new System.Drawing.Point(177, 110);
		this.txtDsNombre.Name = "txtDsNombre";
		this.txtDsNombre.Size = new System.Drawing.Size(150, 22);
		this.txtDsNombre.TabIndex = 11;
		this.txtDsApellido.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDsApellido.Enabled = false;
		this.txtDsApellido.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDsApellido.Location = new System.Drawing.Point(177, 148);
		this.txtDsApellido.Name = "txtDsApellido";
		this.txtDsApellido.Size = new System.Drawing.Size(150, 22);
		this.txtDsApellido.TabIndex = 12;
		this.txtDsEstado.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDsEstado.Enabled = false;
		this.txtDsEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDsEstado.Location = new System.Drawing.Point(177, 186);
		this.txtDsEstado.Name = "txtDsEstado";
		this.txtDsEstado.Size = new System.Drawing.Size(150, 22);
		this.txtDsEstado.TabIndex = 13;
		this.txtFeAlta.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtFeAlta.Enabled = false;
		this.txtFeAlta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFeAlta.Location = new System.Drawing.Point(511, 149);
		this.txtFeAlta.Name = "txtFeAlta";
		this.txtFeAlta.Size = new System.Drawing.Size(150, 22);
		this.txtFeAlta.TabIndex = 17;
		this.txtDsMail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDsMail.Enabled = false;
		this.txtDsMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDsMail.Location = new System.Drawing.Point(511, 111);
		this.txtDsMail.Name = "txtDsMail";
		this.txtDsMail.Size = new System.Drawing.Size(150, 22);
		this.txtDsMail.TabIndex = 16;
		this.txtDsLegajo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtDsLegajo.Enabled = false;
		this.txtDsLegajo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDsLegajo.Location = new System.Drawing.Point(511, 73);
		this.txtDsLegajo.Name = "txtDsLegajo";
		this.txtDsLegajo.Size = new System.Drawing.Size(150, 22);
		this.txtDsLegajo.TabIndex = 15;
		this.txtNuDNI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtNuDNI.Enabled = false;
		this.txtNuDNI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNuDNI.Location = new System.Drawing.Point(511, 35);
		this.txtNuDNI.Name = "txtNuDNI";
		this.txtNuDNI.Size = new System.Drawing.Size(150, 22);
		this.txtNuDNI.TabIndex = 14;
		this.pnlRoles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlRoles.Controls.Add(this.lboxRoles);
		this.pnlRoles.Location = new System.Drawing.Point(699, 34);
		this.pnlRoles.Name = "pnlRoles";
		this.pnlRoles.Size = new System.Drawing.Size(239, 174);
		this.pnlRoles.TabIndex = 18;
		this.lboxRoles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lboxRoles.FormattingEnabled = true;
		this.lboxRoles.ItemHeight = 20;
		this.lboxRoles.Location = new System.Drawing.Point(51, 38);
		this.lboxRoles.Name = "lboxRoles";
		this.lboxRoles.Size = new System.Drawing.Size(120, 84);
		this.lboxRoles.TabIndex = 0;
		this.pnlProyectos.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlProyectos.Controls.Add(this.lboxProyectos);
		this.pnlProyectos.Location = new System.Drawing.Point(983, 32);
		this.pnlProyectos.Name = "pnlProyectos";
		this.pnlProyectos.Size = new System.Drawing.Size(239, 174);
		this.pnlProyectos.TabIndex = 19;
		this.lboxProyectos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lboxProyectos.FormattingEnabled = true;
		this.lboxProyectos.ItemHeight = 20;
		this.lboxProyectos.Location = new System.Drawing.Point(56, 37);
		this.lboxProyectos.Name = "lboxProyectos";
		this.lboxProyectos.Size = new System.Drawing.Size(120, 84);
		this.lboxProyectos.TabIndex = 1;
		this.tabControl1.Controls.Add(this.tabBlanquearClave);
		this.tabControl1.Controls.Add(this.tabModificarRoles);
		this.tabControl1.Controls.Add(this.tabModificarProyectos);
		this.tabControl1.Controls.Add(this.tabEliminarUsuario);
		this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tabControl1.Location = new System.Drawing.Point(30, 234);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(1192, 305);
		this.tabControl1.TabIndex = 20;
		this.tabBlanquearClave.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.tabBlanquearClave.Controls.Add(this.btnDesbloquearUsuario);
		this.tabBlanquearClave.Controls.Add(this.txtNuevaClave);
		this.tabBlanquearClave.Controls.Add(this.label10);
		this.tabBlanquearClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tabBlanquearClave.Location = new System.Drawing.Point(4, 29);
		this.tabBlanquearClave.Name = "tabBlanquearClave";
		this.tabBlanquearClave.Padding = new System.Windows.Forms.Padding(3);
		this.tabBlanquearClave.Size = new System.Drawing.Size(1184, 272);
		this.tabBlanquearClave.TabIndex = 0;
		this.tabBlanquearClave.Text = "Blanquear Clave / Desbloquear";
		this.btnDesbloquearUsuario.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnDesbloquearUsuario.BackColor = System.Drawing.Color.SeaGreen;
		this.btnDesbloquearUsuario.FlatAppearance.BorderSize = 0;
		this.btnDesbloquearUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnDesbloquearUsuario.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnDesbloquearUsuario.ForeColor = System.Drawing.Color.White;
		this.btnDesbloquearUsuario.Image = (System.Drawing.Image)resources.GetObject("btnDesbloquearUsuario.Image");
		this.btnDesbloquearUsuario.Location = new System.Drawing.Point(415, 135);
		this.btnDesbloquearUsuario.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnDesbloquearUsuario.Name = "btnDesbloquearUsuario";
		this.btnDesbloquearUsuario.Size = new System.Drawing.Size(291, 31);
		this.btnDesbloquearUsuario.TabIndex = 24;
		this.btnDesbloquearUsuario.Text = "   Desbloquear Usuario";
		this.btnDesbloquearUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnDesbloquearUsuario.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnDesbloquearUsuario.UseVisualStyleBackColor = false;
		this.btnDesbloquearUsuario.Click += new System.EventHandler(btnDesbloquearUsuario_Click);
		this.txtNuevaClave.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		this.txtNuevaClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNuevaClave.Location = new System.Drawing.Point(556, 85);
		this.txtNuevaClave.Name = "txtNuevaClave";
		this.txtNuevaClave.Size = new System.Drawing.Size(150, 22);
		this.txtNuevaClave.TabIndex = 23;
		this.label10.AutoSize = true;
		this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label10.ForeColor = System.Drawing.Color.White;
		this.label10.Location = new System.Drawing.Point(411, 85);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(124, 20);
		this.label10.TabIndex = 22;
		this.label10.Text = "Clave Provisoria:";
		this.tabModificarRoles.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.tabModificarRoles.Controls.Add(this.btnActualizarRoles);
		this.tabModificarRoles.Controls.Add(this.pnlRolesSeleccionados);
		this.tabModificarRoles.Controls.Add(this.btnAgregarRoles);
		this.tabModificarRoles.Controls.Add(this.pnlRolesDisponibles);
		this.tabModificarRoles.Controls.Add(this.btnQuitarRoles);
		this.tabModificarRoles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tabModificarRoles.Location = new System.Drawing.Point(4, 29);
		this.tabModificarRoles.Name = "tabModificarRoles";
		this.tabModificarRoles.Padding = new System.Windows.Forms.Padding(3);
		this.tabModificarRoles.Size = new System.Drawing.Size(1184, 272);
		this.tabModificarRoles.TabIndex = 1;
		this.tabModificarRoles.Text = "Modificar Roles";
		this.btnActualizarRoles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnActualizarRoles.BackColor = System.Drawing.Color.SeaGreen;
		this.btnActualizarRoles.FlatAppearance.BorderSize = 0;
		this.btnActualizarRoles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnActualizarRoles.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnActualizarRoles.ForeColor = System.Drawing.Color.White;
		this.btnActualizarRoles.Image = (System.Drawing.Image)resources.GetObject("btnActualizarRoles.Image");
		this.btnActualizarRoles.Location = new System.Drawing.Point(507, 230);
		this.btnActualizarRoles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnActualizarRoles.Name = "btnActualizarRoles";
		this.btnActualizarRoles.Size = new System.Drawing.Size(170, 31);
		this.btnActualizarRoles.TabIndex = 87;
		this.btnActualizarRoles.Text = "   Actualizar Roles";
		this.btnActualizarRoles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnActualizarRoles.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnActualizarRoles.UseVisualStyleBackColor = false;
		this.btnActualizarRoles.Click += new System.EventHandler(btnActualizarRoles_Click);
		this.pnlRolesSeleccionados.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlRolesSeleccionados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlRolesSeleccionados.Controls.Add(this.dgvRolesSeleccionados);
		this.pnlRolesSeleccionados.Location = new System.Drawing.Point(695, 11);
		this.pnlRolesSeleccionados.Name = "pnlRolesSeleccionados";
		this.pnlRolesSeleccionados.Size = new System.Drawing.Size(417, 250);
		this.pnlRolesSeleccionados.TabIndex = 86;
		this.dgvRolesSeleccionados.AllowUserToAddRows = false;
		this.dgvRolesSeleccionados.AllowUserToDeleteRows = false;
		this.dgvRolesSeleccionados.AllowUserToResizeRows = false;
		this.dgvRolesSeleccionados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvRolesSeleccionados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvRolesSeleccionados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvRolesSeleccionados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvRolesSeleccionados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesSeleccionados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dgvRolesSeleccionados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvRolesSeleccionados.DefaultCellStyle = dataGridViewCellStyle2;
		this.dgvRolesSeleccionados.EnableHeadersVisualStyles = false;
		this.dgvRolesSeleccionados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvRolesSeleccionados.Location = new System.Drawing.Point(35, 37);
		this.dgvRolesSeleccionados.Name = "dgvRolesSeleccionados";
		this.dgvRolesSeleccionados.ReadOnly = true;
		this.dgvRolesSeleccionados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesSeleccionados.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dgvRolesSeleccionados.RowHeadersVisible = false;
		this.dgvRolesSeleccionados.RowHeadersWidth = 15;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dgvRolesSeleccionados.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dgvRolesSeleccionados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvRolesSeleccionados.Size = new System.Drawing.Size(337, 139);
		this.dgvRolesSeleccionados.TabIndex = 18;
		this.btnAgregarRoles.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarRoles.FlatAppearance.BorderSize = 0;
		this.btnAgregarRoles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarRoles.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarRoles.ForeColor = System.Drawing.Color.White;
		this.btnAgregarRoles.Image = (System.Drawing.Image)resources.GetObject("btnAgregarRoles.Image");
		this.btnAgregarRoles.Location = new System.Drawing.Point(507, 82);
		this.btnAgregarRoles.Name = "btnAgregarRoles";
		this.btnAgregarRoles.Size = new System.Drawing.Size(170, 25);
		this.btnAgregarRoles.TabIndex = 83;
		this.btnAgregarRoles.Text = "   Agregar Rol";
		this.btnAgregarRoles.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarRoles.UseVisualStyleBackColor = false;
		this.btnAgregarRoles.Click += new System.EventHandler(btnAgregarRoles_Click);
		this.pnlRolesDisponibles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlRolesDisponibles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlRolesDisponibles.Controls.Add(this.dgvRolesDisponibles);
		this.pnlRolesDisponibles.Location = new System.Drawing.Point(72, 11);
		this.pnlRolesDisponibles.Name = "pnlRolesDisponibles";
		this.pnlRolesDisponibles.Size = new System.Drawing.Size(417, 250);
		this.pnlRolesDisponibles.TabIndex = 85;
		this.dgvRolesDisponibles.AllowUserToAddRows = false;
		this.dgvRolesDisponibles.AllowUserToDeleteRows = false;
		this.dgvRolesDisponibles.AllowUserToResizeRows = false;
		this.dgvRolesDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvRolesDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvRolesDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvRolesDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvRolesDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
		this.dgvRolesDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvRolesDisponibles.DefaultCellStyle = dataGridViewCellStyle6;
		this.dgvRolesDisponibles.EnableHeadersVisualStyles = false;
		this.dgvRolesDisponibles.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvRolesDisponibles.Location = new System.Drawing.Point(20, 37);
		this.dgvRolesDisponibles.Name = "dgvRolesDisponibles";
		this.dgvRolesDisponibles.ReadOnly = true;
		this.dgvRolesDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvRolesDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dgvRolesDisponibles.RowHeadersVisible = false;
		this.dgvRolesDisponibles.RowHeadersWidth = 15;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		this.dgvRolesDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle8;
		this.dgvRolesDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvRolesDisponibles.Size = new System.Drawing.Size(361, 139);
		this.dgvRolesDisponibles.TabIndex = 18;
		this.btnQuitarRoles.BackColor = System.Drawing.Color.SeaGreen;
		this.btnQuitarRoles.FlatAppearance.BorderSize = 0;
		this.btnQuitarRoles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnQuitarRoles.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnQuitarRoles.ForeColor = System.Drawing.Color.White;
		this.btnQuitarRoles.Image = (System.Drawing.Image)resources.GetObject("btnQuitarRoles.Image");
		this.btnQuitarRoles.Location = new System.Drawing.Point(507, 113);
		this.btnQuitarRoles.Name = "btnQuitarRoles";
		this.btnQuitarRoles.Size = new System.Drawing.Size(170, 25);
		this.btnQuitarRoles.TabIndex = 84;
		this.btnQuitarRoles.Text = "   Quitar Rol";
		this.btnQuitarRoles.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnQuitarRoles.UseVisualStyleBackColor = false;
		this.btnQuitarRoles.Click += new System.EventHandler(btnQuitarRoles_Click);
		this.tabModificarProyectos.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.tabModificarProyectos.Controls.Add(this.btnActualizarProyectos);
		this.tabModificarProyectos.Controls.Add(this.pnlProyectosDisponibles);
		this.tabModificarProyectos.Controls.Add(this.btnAgregarProyectos);
		this.tabModificarProyectos.Controls.Add(this.btnQuitarProyectos);
		this.tabModificarProyectos.Controls.Add(this.pnlProyectosSeleccionados);
		this.tabModificarProyectos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tabModificarProyectos.Location = new System.Drawing.Point(4, 29);
		this.tabModificarProyectos.Name = "tabModificarProyectos";
		this.tabModificarProyectos.Padding = new System.Windows.Forms.Padding(3);
		this.tabModificarProyectos.Size = new System.Drawing.Size(1184, 272);
		this.tabModificarProyectos.TabIndex = 2;
		this.tabModificarProyectos.Text = "Modificar Proyectos";
		this.btnActualizarProyectos.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnActualizarProyectos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnActualizarProyectos.FlatAppearance.BorderSize = 0;
		this.btnActualizarProyectos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnActualizarProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnActualizarProyectos.ForeColor = System.Drawing.Color.White;
		this.btnActualizarProyectos.Image = (System.Drawing.Image)resources.GetObject("btnActualizarProyectos.Image");
		this.btnActualizarProyectos.Location = new System.Drawing.Point(501, 227);
		this.btnActualizarProyectos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnActualizarProyectos.Name = "btnActualizarProyectos";
		this.btnActualizarProyectos.Size = new System.Drawing.Size(187, 31);
		this.btnActualizarProyectos.TabIndex = 93;
		this.btnActualizarProyectos.Text = "   Actualizar Proyectos";
		this.btnActualizarProyectos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnActualizarProyectos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnActualizarProyectos.UseVisualStyleBackColor = false;
		this.btnActualizarProyectos.Click += new System.EventHandler(btnActualizarProyectos_Click);
		this.pnlProyectosDisponibles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlProyectosDisponibles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlProyectosDisponibles.Controls.Add(this.dgvProyectosDisponibles);
		this.pnlProyectosDisponibles.Location = new System.Drawing.Point(68, 11);
		this.pnlProyectosDisponibles.Name = "pnlProyectosDisponibles";
		this.pnlProyectosDisponibles.Size = new System.Drawing.Size(417, 250);
		this.pnlProyectosDisponibles.TabIndex = 91;
		this.dgvProyectosDisponibles.AllowUserToAddRows = false;
		this.dgvProyectosDisponibles.AllowUserToDeleteRows = false;
		this.dgvProyectosDisponibles.AllowUserToResizeRows = false;
		this.dgvProyectosDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvProyectosDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvProyectosDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvProyectosDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvProyectosDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.dgvProyectosDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvProyectosDisponibles.DefaultCellStyle = dataGridViewCellStyle10;
		this.dgvProyectosDisponibles.EnableHeadersVisualStyles = false;
		this.dgvProyectosDisponibles.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvProyectosDisponibles.Location = new System.Drawing.Point(20, 37);
		this.dgvProyectosDisponibles.Name = "dgvProyectosDisponibles";
		this.dgvProyectosDisponibles.ReadOnly = true;
		this.dgvProyectosDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
		this.dgvProyectosDisponibles.RowHeadersVisible = false;
		this.dgvProyectosDisponibles.RowHeadersWidth = 15;
		dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle12.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
		this.dgvProyectosDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle12;
		this.dgvProyectosDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvProyectosDisponibles.Size = new System.Drawing.Size(361, 139);
		this.dgvProyectosDisponibles.TabIndex = 18;
		this.btnAgregarProyectos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarProyectos.FlatAppearance.BorderSize = 0;
		this.btnAgregarProyectos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarProyectos.ForeColor = System.Drawing.Color.White;
		this.btnAgregarProyectos.Image = (System.Drawing.Image)resources.GetObject("btnAgregarProyectos.Image");
		this.btnAgregarProyectos.Location = new System.Drawing.Point(507, 84);
		this.btnAgregarProyectos.Name = "btnAgregarProyectos";
		this.btnAgregarProyectos.Size = new System.Drawing.Size(170, 25);
		this.btnAgregarProyectos.TabIndex = 89;
		this.btnAgregarProyectos.Text = "   Agregar Proyecto";
		this.btnAgregarProyectos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarProyectos.UseVisualStyleBackColor = false;
		this.btnAgregarProyectos.Click += new System.EventHandler(btnAgregarProyectos_Click_1);
		this.btnQuitarProyectos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnQuitarProyectos.FlatAppearance.BorderSize = 0;
		this.btnQuitarProyectos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnQuitarProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnQuitarProyectos.ForeColor = System.Drawing.Color.White;
		this.btnQuitarProyectos.Image = (System.Drawing.Image)resources.GetObject("btnQuitarProyectos.Image");
		this.btnQuitarProyectos.Location = new System.Drawing.Point(507, 115);
		this.btnQuitarProyectos.Name = "btnQuitarProyectos";
		this.btnQuitarProyectos.Size = new System.Drawing.Size(170, 25);
		this.btnQuitarProyectos.TabIndex = 90;
		this.btnQuitarProyectos.Text = "   Quitar Proyecto";
		this.btnQuitarProyectos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnQuitarProyectos.UseVisualStyleBackColor = false;
		this.btnQuitarProyectos.Click += new System.EventHandler(btnQuitarProyectos_Click_1);
		this.pnlProyectosSeleccionados.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlProyectosSeleccionados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlProyectosSeleccionados.Controls.Add(this.dgvProyectosSeleccionados);
		this.pnlProyectosSeleccionados.Location = new System.Drawing.Point(700, 11);
		this.pnlProyectosSeleccionados.Name = "pnlProyectosSeleccionados";
		this.pnlProyectosSeleccionados.Size = new System.Drawing.Size(417, 250);
		this.pnlProyectosSeleccionados.TabIndex = 92;
		this.dgvProyectosSeleccionados.AllowUserToAddRows = false;
		this.dgvProyectosSeleccionados.AllowUserToDeleteRows = false;
		this.dgvProyectosSeleccionados.AllowUserToResizeRows = false;
		this.dgvProyectosSeleccionados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvProyectosSeleccionados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvProyectosSeleccionados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvProyectosSeleccionados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvProyectosSeleccionados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle13.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosSeleccionados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
		this.dgvProyectosSeleccionados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvProyectosSeleccionados.DefaultCellStyle = dataGridViewCellStyle14;
		this.dgvProyectosSeleccionados.EnableHeadersVisualStyles = false;
		this.dgvProyectosSeleccionados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvProyectosSeleccionados.Location = new System.Drawing.Point(35, 37);
		this.dgvProyectosSeleccionados.Name = "dgvProyectosSeleccionados";
		this.dgvProyectosSeleccionados.ReadOnly = true;
		this.dgvProyectosSeleccionados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvProyectosSeleccionados.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
		this.dgvProyectosSeleccionados.RowHeadersVisible = false;
		this.dgvProyectosSeleccionados.RowHeadersWidth = 15;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
		this.dgvProyectosSeleccionados.RowsDefaultCellStyle = dataGridViewCellStyle16;
		this.dgvProyectosSeleccionados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvProyectosSeleccionados.Size = new System.Drawing.Size(337, 139);
		this.dgvProyectosSeleccionados.TabIndex = 18;
		this.tabEliminarUsuario.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.tabEliminarUsuario.Controls.Add(this.btnEliminarUsuario);
		this.tabEliminarUsuario.Location = new System.Drawing.Point(4, 29);
		this.tabEliminarUsuario.Name = "tabEliminarUsuario";
		this.tabEliminarUsuario.Padding = new System.Windows.Forms.Padding(3);
		this.tabEliminarUsuario.Size = new System.Drawing.Size(1184, 272);
		this.tabEliminarUsuario.TabIndex = 3;
		this.tabEliminarUsuario.Text = "Eliminar Usuario";
		this.btnEliminarUsuario.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnEliminarUsuario.BackColor = System.Drawing.Color.SeaGreen;
		this.btnEliminarUsuario.FlatAppearance.BorderSize = 0;
		this.btnEliminarUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnEliminarUsuario.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnEliminarUsuario.ForeColor = System.Drawing.Color.White;
		this.btnEliminarUsuario.Image = (System.Drawing.Image)resources.GetObject("btnEliminarUsuario.Image");
		this.btnEliminarUsuario.Location = new System.Drawing.Point(499, 121);
		this.btnEliminarUsuario.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnEliminarUsuario.Name = "btnEliminarUsuario";
		this.btnEliminarUsuario.Size = new System.Drawing.Size(187, 31);
		this.btnEliminarUsuario.TabIndex = 94;
		this.btnEliminarUsuario.Text = "   Eliminar Usuario";
		this.btnEliminarUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnEliminarUsuario.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnEliminarUsuario.UseVisualStyleBackColor = false;
		this.btnEliminarUsuario.Click += new System.EventHandler(btnEliminarUsuario_Click);
		this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(1039, 558);
		this.btnCerrar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(175, 31);
		this.btnCerrar.TabIndex = 21;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1234, 611);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.tabControl1);
		base.Controls.Add(this.pnlProyectos);
		base.Controls.Add(this.pnlRoles);
		base.Controls.Add(this.txtFeAlta);
		base.Controls.Add(this.txtDsMail);
		base.Controls.Add(this.txtDsLegajo);
		base.Controls.Add(this.txtNuDNI);
		base.Controls.Add(this.txtDsEstado);
		base.Controls.Add(this.txtDsApellido);
		base.Controls.Add(this.txtDsNombre);
		base.Controls.Add(this.txtDsUsuario);
		base.Controls.Add(this.txtCdUsuario);
		base.Controls.Add(this.label9);
		base.Controls.Add(this.label8);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmUsuarioVerModificar";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Ver Usuario";
		base.Load += new System.EventHandler(frmUsuarioVerModificar_Load);
		this.pnlRoles.ResumeLayout(false);
		this.pnlProyectos.ResumeLayout(false);
		this.tabControl1.ResumeLayout(false);
		this.tabBlanquearClave.ResumeLayout(false);
		this.tabBlanquearClave.PerformLayout();
		this.tabModificarRoles.ResumeLayout(false);
		this.pnlRolesSeleccionados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvRolesSeleccionados).EndInit();
		this.pnlRolesDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvRolesDisponibles).EndInit();
		this.tabModificarProyectos.ResumeLayout(false);
		this.pnlProyectosDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosDisponibles).EndInit();
		this.pnlProyectosSeleccionados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvProyectosSeleccionados).EndInit();
		this.tabEliminarUsuario.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
