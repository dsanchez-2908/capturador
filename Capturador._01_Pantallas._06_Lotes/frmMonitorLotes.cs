using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._06_Lotes;

public class frmMonitorLotes : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private int cdLoteSeleccionado;

	private IContainer components = null;

	private Panel pnlTotalLotesEstados;

	private DataGridView dgvTotalLotesEstados;

	private Panel pnlTotalLotesUsuario;

	private DataGridView dgvTotalLotesUsuarios;

	private Panel pnlLotesDetalle;

	private DataGridView dgvLotesDetalle;

	private Button btnEliminarLote;

	private Button btnExportar;

	private Button btnActualizar;

	private Button btnCerrar;

	private Panel pnlModificarLote;

	private Label lblCodigoLote;

	private Label label4;

	private Label label3;

	private Label label2;

	private Label label1;

	private Button btnModificarLote;

	private Label lblNombreLote;

	private Label lblEstado;

	private Label lblUsuario;

	private ComboBox cbxListaEstados;

	private ComboBox cbxListaUsuarios;

	private Button btnAccionModificarLote;

	private Button btnCancelarModificacion;

	private Label label5;

	private Label label7;

	private Label label6;

	private Label label8;

	private ComboBox cbxListaProyectos;

	private Label label10;

	private Label lblRutaLote;

	private Label label9;

	private Button btnSeleccionarCarpeta;

	private TextBox txtRutaLote;

	public frmMonitorLotes(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmMonitorLotes_Load(object sender, EventArgs e)
	{
		actualizarTodasLasGrillas();
		cargarListas();
	}

	private void actualizarTodasLasGrillas()
	{
		llenarGrillaLotesEstado();
		llenarGrillaLotesUsuario();
		llenarGrillaLotesDetalle();
	}

	private void llenarGrillaLotesEstado()
	{
		dgvTotalLotesEstados.DataSource = nLotes.obtenerTablaLotesEstado(oUsuarioLogueado);
		dgvTotalLotesEstados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Lotes por Estado";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvTotalLotesEstados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Filas: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvTotalLotesEstados.Dock = DockStyle.Fill;
		pnlTotalLotesEstados.Controls.Clear();
		pnlTotalLotesEstados.Controls.Add(dgvTotalLotesEstados);
		pnlTotalLotesEstados.Controls.Add(labelTitulo);
		pnlTotalLotesEstados.Controls.Add(labelTotal);
	}

	private void llenarGrillaLotesUsuario()
	{
		dgvTotalLotesUsuarios.DataSource = nLotes.obtenerTablaLotesUsuario(oUsuarioLogueado);
		dgvTotalLotesUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Lotes por Usuario y Estado";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvTotalLotesUsuarios.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Filas: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvTotalLotesUsuarios.Dock = DockStyle.Fill;
		pnlTotalLotesUsuario.Controls.Clear();
		pnlTotalLotesUsuario.Controls.Add(dgvTotalLotesUsuarios);
		pnlTotalLotesUsuario.Controls.Add(labelTitulo);
		pnlTotalLotesUsuario.Controls.Add(labelTotal);
	}

	private void llenarGrillaLotesDetalle()
	{
		dgvLotesDetalle.DataSource = nLotes.obtenerTablaLotesDetalle(oUsuarioLogueado);
		foreach (DataGridViewColumn col in dgvLotesDetalle.Columns)
		{
			if (new string[1] { "Ruta Lote" }.Contains(col.Name))
			{
				col.Visible = false;
			}
		}
		dgvLotesDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Lotes";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvLotesDetalle.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Filas: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvLotesDetalle.Dock = DockStyle.Fill;
		pnlLotesDetalle.Controls.Clear();
		pnlLotesDetalle.Controls.Add(dgvLotesDetalle);
		pnlLotesDetalle.Controls.Add(labelTitulo);
		pnlLotesDetalle.Controls.Add(labelTotal);
	}

	private void btnEliminarLote_Click(object sender, EventArgs e)
	{
		if (cdLoteSeleccionado == 0)
		{
			MessageBox.Show("Debe seleccionar un Lote de la grilla detalle", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else if (MessageBox.Show("¿Está seguro que quiere eliminar el lote " + Convert.ToString(cdLoteSeleccionado) + "?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
		{
			nLotes.eliminarLote(oUsuarioLogueado, cdLoteSeleccionado);
			MessageBox.Show("Se elimino el lote correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			vaciarModificacionLote();
		}
	}

	private void btnActualizar_Click(object sender, EventArgs e)
	{
		actualizarTodasLasGrillas();
	}

	private void btnExportar_Click(object sender, EventArgs e)
	{
		SaveFileDialog oSeleccionarArchivoGuardar = new SaveFileDialog();
		oSeleccionarArchivoGuardar.Filter = "Archivo Excel (*.xlxs)|*.xlsx|Archivo CSV (*.csv)|*.csv";
		oSeleccionarArchivoGuardar.Title = "Seleccione el Archivo a Exportar";
		oSeleccionarArchivoGuardar.FileName = "Detalle_Lotes_" + DateTime.Now.ToString("yyyyMMdd");
		if (oSeleccionarArchivoGuardar.ShowDialog() == DialogResult.OK)
		{
			string archivoGenerar = oSeleccionarArchivoGuardar.FileName;
			DataTable oTablaExportar = (DataTable)dgvLotesDetalle.DataSource;
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

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnModificarLote_Click(object sender, EventArgs e)
	{
		cdLoteSeleccionado = Convert.ToInt32(dgvLotesDetalle.SelectedRows[0].Cells[1].Value.ToString());
		if (cdLoteSeleccionado == 0)
		{
			MessageBox.Show("Debe seleccionar un Lote de la grilla detalle", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		pnlModificarLote.Visible = true;
		pnlModificarLote.Dock = DockStyle.Fill;
		lblCodigoLote.Text = Convert.ToString(cdLoteSeleccionado);
		lblNombreLote.Text = dgvLotesDetalle.SelectedRows[0].Cells["Nombre Lote"].Value.ToString();
		lblUsuario.Text = dgvLotesDetalle.SelectedRows[0].Cells["Usuario"].Value.ToString();
		lblEstado.Text = dgvLotesDetalle.SelectedRows[0].Cells["Estado"].Value.ToString();
		lblRutaLote.Text = dgvLotesDetalle.SelectedRows[0].Cells["Ruta Lote"].Value.ToString();
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
		cbxListaProyectos.DataSource = nListas.ObtenerListaProyectosActivos(oUsuarioLogueado);
		cbxListaProyectos.DisplayMember = "dsProyecto";
		cbxListaProyectos.ValueMember = "cdProyecto";
		cbxListaProyectos.SelectedIndex = -1;
	}

	private void btnCancelarModificacion_Click(object sender, EventArgs e)
	{
		vaciarModificacionLote();
	}

	private void vaciarModificacionLote()
	{
		lblCodigoLote.Text = "...";
		lblNombreLote.Text = "...";
		lblUsuario.Text = "...";
		lblEstado.Text = "...";
		cbxListaEstados.SelectedIndex = -1;
		cbxListaUsuarios.SelectedIndex = -1;
		pnlModificarLote.Visible = false;
		actualizarTodasLasGrillas();
	}

	private void btnAccionModificarLote_Click(object sender, EventArgs e)
	{
		int cdLoteSeleccionado = Convert.ToInt32(lblCodigoLote.Text);
		int cdEstadoSeleccionado = Convert.ToInt32(cbxListaEstados.SelectedValue);
		int cdUsuarioSeleccionado = Convert.ToInt32(cbxListaUsuarios.SelectedValue);
		int cdProyectoSeleccionado = Convert.ToInt32(cbxListaProyectos.SelectedValue);
		string rutaLote = txtRutaLote.Text;
		if (cdEstadoSeleccionado == 0 && cdUsuarioSeleccionado == 0 && cdProyectoSeleccionado == 0)
		{
			MessageBox.Show("No seleccionó ningun cambio", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else if (MessageBox.Show("¿Está seguro que quiere modificar el lote " + Convert.ToString(lblCodigoLote.Text) + "?", "Confirmar modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
		{
			try
			{
				nLotes.modificarLote(oUsuarioLogueado, cdLoteSeleccionado, cdEstadoSeleccionado, cdUsuarioSeleccionado, cdProyectoSeleccionado, rutaLote);
				MessageBox.Show("El lote se modifico correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				vaciarModificacionLote();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
	}

	private void btnSeleccionarCarpeta_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLoteDestino = new FolderBrowserDialog();
		oSeleccionarLoteDestino.Description = "Seleccionar nueva ruta del lote";
		oSeleccionarLoteDestino.ShowNewFolderButton = true;
		if (oSeleccionarLoteDestino.ShowDialog() == DialogResult.OK)
		{
			txtRutaLote.Text = oSeleccionarLoteDestino.SelectedPath;
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._06_Lotes.frmMonitorLotes));
		this.pnlTotalLotesEstados = new System.Windows.Forms.Panel();
		this.dgvTotalLotesEstados = new System.Windows.Forms.DataGridView();
		this.pnlTotalLotesUsuario = new System.Windows.Forms.Panel();
		this.dgvTotalLotesUsuarios = new System.Windows.Forms.DataGridView();
		this.pnlLotesDetalle = new System.Windows.Forms.Panel();
		this.dgvLotesDetalle = new System.Windows.Forms.DataGridView();
		this.btnEliminarLote = new System.Windows.Forms.Button();
		this.btnExportar = new System.Windows.Forms.Button();
		this.btnActualizar = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.pnlModificarLote = new System.Windows.Forms.Panel();
		this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
		this.txtRutaLote = new System.Windows.Forms.TextBox();
		this.label10 = new System.Windows.Forms.Label();
		this.lblRutaLote = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.cbxListaProyectos = new System.Windows.Forms.ComboBox();
		this.label7 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.btnCancelarModificacion = new System.Windows.Forms.Button();
		this.btnAccionModificarLote = new System.Windows.Forms.Button();
		this.cbxListaEstados = new System.Windows.Forms.ComboBox();
		this.cbxListaUsuarios = new System.Windows.Forms.ComboBox();
		this.lblEstado = new System.Windows.Forms.Label();
		this.lblUsuario = new System.Windows.Forms.Label();
		this.lblNombreLote = new System.Windows.Forms.Label();
		this.lblCodigoLote = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.btnModificarLote = new System.Windows.Forms.Button();
		this.pnlTotalLotesEstados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesEstados).BeginInit();
		this.pnlTotalLotesUsuario.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesUsuarios).BeginInit();
		this.pnlLotesDetalle.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDetalle).BeginInit();
		this.pnlModificarLote.SuspendLayout();
		base.SuspendLayout();
		this.pnlTotalLotesEstados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlTotalLotesEstados.Controls.Add(this.dgvTotalLotesEstados);
		this.pnlTotalLotesEstados.Location = new System.Drawing.Point(12, 12);
		this.pnlTotalLotesEstados.Name = "pnlTotalLotesEstados";
		this.pnlTotalLotesEstados.Size = new System.Drawing.Size(500, 200);
		this.pnlTotalLotesEstados.TabIndex = 60;
		this.dgvTotalLotesEstados.AllowUserToAddRows = false;
		this.dgvTotalLotesEstados.AllowUserToDeleteRows = false;
		this.dgvTotalLotesEstados.AllowUserToResizeRows = false;
		this.dgvTotalLotesEstados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvTotalLotesEstados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvTotalLotesEstados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvTotalLotesEstados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvTotalLotesEstados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle13.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvTotalLotesEstados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
		this.dgvTotalLotesEstados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvTotalLotesEstados.DefaultCellStyle = dataGridViewCellStyle14;
		this.dgvTotalLotesEstados.EnableHeadersVisualStyles = false;
		this.dgvTotalLotesEstados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvTotalLotesEstados.Location = new System.Drawing.Point(11, 22);
		this.dgvTotalLotesEstados.Name = "dgvTotalLotesEstados";
		this.dgvTotalLotesEstados.ReadOnly = true;
		this.dgvTotalLotesEstados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvTotalLotesEstados.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
		this.dgvTotalLotesEstados.RowHeadersVisible = false;
		this.dgvTotalLotesEstados.RowHeadersWidth = 15;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
		this.dgvTotalLotesEstados.RowsDefaultCellStyle = dataGridViewCellStyle16;
		this.dgvTotalLotesEstados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvTotalLotesEstados.Size = new System.Drawing.Size(472, 140);
		this.dgvTotalLotesEstados.TabIndex = 18;
		this.pnlTotalLotesUsuario.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.pnlTotalLotesUsuario.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlTotalLotesUsuario.Controls.Add(this.dgvTotalLotesUsuarios);
		this.pnlTotalLotesUsuario.Location = new System.Drawing.Point(622, 12);
		this.pnlTotalLotesUsuario.Name = "pnlTotalLotesUsuario";
		this.pnlTotalLotesUsuario.Size = new System.Drawing.Size(600, 200);
		this.pnlTotalLotesUsuario.TabIndex = 61;
		this.dgvTotalLotesUsuarios.AllowUserToAddRows = false;
		this.dgvTotalLotesUsuarios.AllowUserToDeleteRows = false;
		this.dgvTotalLotesUsuarios.AllowUserToResizeRows = false;
		this.dgvTotalLotesUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvTotalLotesUsuarios.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvTotalLotesUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvTotalLotesUsuarios.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvTotalLotesUsuarios.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle17.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle17.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle17.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle17.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvTotalLotesUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
		this.dgvTotalLotesUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvTotalLotesUsuarios.DefaultCellStyle = dataGridViewCellStyle18;
		this.dgvTotalLotesUsuarios.EnableHeadersVisualStyles = false;
		this.dgvTotalLotesUsuarios.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvTotalLotesUsuarios.Location = new System.Drawing.Point(11, 22);
		this.dgvTotalLotesUsuarios.Name = "dgvTotalLotesUsuarios";
		this.dgvTotalLotesUsuarios.ReadOnly = true;
		this.dgvTotalLotesUsuarios.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle19.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle19.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvTotalLotesUsuarios.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
		this.dgvTotalLotesUsuarios.RowHeadersVisible = false;
		this.dgvTotalLotesUsuarios.RowHeadersWidth = 15;
		dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle20.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle20.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.White;
		this.dgvTotalLotesUsuarios.RowsDefaultCellStyle = dataGridViewCellStyle20;
		this.dgvTotalLotesUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvTotalLotesUsuarios.Size = new System.Drawing.Size(574, 140);
		this.dgvTotalLotesUsuarios.TabIndex = 18;
		this.pnlLotesDetalle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlLotesDetalle.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLotesDetalle.Controls.Add(this.dgvLotesDetalle);
		this.pnlLotesDetalle.Location = new System.Drawing.Point(12, 258);
		this.pnlLotesDetalle.Name = "pnlLotesDetalle";
		this.pnlLotesDetalle.Size = new System.Drawing.Size(1210, 300);
		this.pnlLotesDetalle.TabIndex = 62;
		this.dgvLotesDetalle.AllowUserToAddRows = false;
		this.dgvLotesDetalle.AllowUserToDeleteRows = false;
		this.dgvLotesDetalle.AllowUserToResizeRows = false;
		this.dgvLotesDetalle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesDetalle.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesDetalle.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle21.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle21.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle21.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle21.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDetalle.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle21;
		this.dgvLotesDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle22.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesDetalle.DefaultCellStyle = dataGridViewCellStyle22;
		this.dgvLotesDetalle.EnableHeadersVisualStyles = false;
		this.dgvLotesDetalle.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesDetalle.Location = new System.Drawing.Point(328, 72);
		this.dgvLotesDetalle.Name = "dgvLotesDetalle";
		this.dgvLotesDetalle.ReadOnly = true;
		this.dgvLotesDetalle.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle23.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle23.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle23.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDetalle.RowHeadersDefaultCellStyle = dataGridViewCellStyle23;
		this.dgvLotesDetalle.RowHeadersVisible = false;
		this.dgvLotesDetalle.RowHeadersWidth = 15;
		dataGridViewCellStyle24.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle24.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle24.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle24.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle24.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesDetalle.RowsDefaultCellStyle = dataGridViewCellStyle24;
		this.dgvLotesDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesDetalle.Size = new System.Drawing.Size(472, 140);
		this.dgvLotesDetalle.TabIndex = 18;
		this.btnEliminarLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.btnEliminarLote.BackColor = System.Drawing.Color.SeaGreen;
		this.btnEliminarLote.FlatAppearance.BorderSize = 0;
		this.btnEliminarLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnEliminarLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnEliminarLote.ForeColor = System.Drawing.Color.White;
		this.btnEliminarLote.Image = (System.Drawing.Image)resources.GetObject("btnEliminarLote.Image");
		this.btnEliminarLote.Location = new System.Drawing.Point(510, 473);
		this.btnEliminarLote.Name = "btnEliminarLote";
		this.btnEliminarLote.Size = new System.Drawing.Size(175, 25);
		this.btnEliminarLote.TabIndex = 78;
		this.btnEliminarLote.Text = "   Eliminar Lote";
		this.btnEliminarLote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnEliminarLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnEliminarLote.UseVisualStyleBackColor = false;
		this.btnEliminarLote.Click += new System.EventHandler(btnEliminarLote_Click);
		this.btnExportar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnExportar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnExportar.FlatAppearance.BorderSize = 0;
		this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnExportar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnExportar.ForeColor = System.Drawing.Color.White;
		this.btnExportar.Image = (System.Drawing.Image)resources.GetObject("btnExportar.Image");
		this.btnExportar.Location = new System.Drawing.Point(193, 574);
		this.btnExportar.Name = "btnExportar";
		this.btnExportar.Size = new System.Drawing.Size(175, 25);
		this.btnExportar.TabIndex = 79;
		this.btnExportar.Text = "   Exportar Detalle";
		this.btnExportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnExportar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnExportar.UseVisualStyleBackColor = false;
		this.btnExportar.Click += new System.EventHandler(btnExportar_Click);
		this.btnActualizar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnActualizar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnActualizar.FlatAppearance.BorderSize = 0;
		this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnActualizar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnActualizar.ForeColor = System.Drawing.Color.White;
		this.btnActualizar.Image = (System.Drawing.Image)resources.GetObject("btnActualizar.Image");
		this.btnActualizar.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnActualizar.Location = new System.Drawing.Point(12, 574);
		this.btnActualizar.Name = "btnActualizar";
		this.btnActualizar.Size = new System.Drawing.Size(175, 25);
		this.btnActualizar.TabIndex = 80;
		this.btnActualizar.Text = "   Actualizar";
		this.btnActualizar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnActualizar.UseVisualStyleBackColor = false;
		this.btnActualizar.Click += new System.EventHandler(btnActualizar_Click);
		this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(1092, 574);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(130, 25);
		this.btnCerrar.TabIndex = 81;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.pnlModificarLote.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.pnlModificarLote.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlModificarLote.Controls.Add(this.btnSeleccionarCarpeta);
		this.pnlModificarLote.Controls.Add(this.txtRutaLote);
		this.pnlModificarLote.Controls.Add(this.label10);
		this.pnlModificarLote.Controls.Add(this.lblRutaLote);
		this.pnlModificarLote.Controls.Add(this.label9);
		this.pnlModificarLote.Controls.Add(this.label8);
		this.pnlModificarLote.Controls.Add(this.cbxListaProyectos);
		this.pnlModificarLote.Controls.Add(this.label7);
		this.pnlModificarLote.Controls.Add(this.label6);
		this.pnlModificarLote.Controls.Add(this.label5);
		this.pnlModificarLote.Controls.Add(this.btnCancelarModificacion);
		this.pnlModificarLote.Controls.Add(this.btnAccionModificarLote);
		this.pnlModificarLote.Controls.Add(this.cbxListaEstados);
		this.pnlModificarLote.Controls.Add(this.cbxListaUsuarios);
		this.pnlModificarLote.Controls.Add(this.lblEstado);
		this.pnlModificarLote.Controls.Add(this.lblUsuario);
		this.pnlModificarLote.Controls.Add(this.btnEliminarLote);
		this.pnlModificarLote.Controls.Add(this.lblNombreLote);
		this.pnlModificarLote.Controls.Add(this.lblCodigoLote);
		this.pnlModificarLote.Controls.Add(this.label4);
		this.pnlModificarLote.Controls.Add(this.label3);
		this.pnlModificarLote.Controls.Add(this.label2);
		this.pnlModificarLote.Controls.Add(this.label1);
		this.pnlModificarLote.Location = new System.Drawing.Point(2, 12);
		this.pnlModificarLote.Name = "pnlModificarLote";
		this.pnlModificarLote.Size = new System.Drawing.Size(1220, 594);
		this.pnlModificarLote.TabIndex = 82;
		this.pnlModificarLote.Visible = false;
		this.btnSeleccionarCarpeta.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.btnSeleccionarCarpeta.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarCarpeta.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarCarpeta.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarCarpeta.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarCarpeta.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarCarpeta.Image");
		this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(816, 423);
		this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
		this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(122, 25);
		this.btnSeleccionarCarpeta.TabIndex = 113;
		this.btnSeleccionarCarpeta.Text = "   Seleccionar";
		this.btnSeleccionarCarpeta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnSeleccionarCarpeta.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarCarpeta.UseVisualStyleBackColor = false;
		this.btnSeleccionarCarpeta.Click += new System.EventHandler(btnSeleccionarCarpeta_Click);
		this.txtRutaLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.txtRutaLote.Enabled = false;
		this.txtRutaLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtRutaLote.Location = new System.Drawing.Point(510, 417);
		this.txtRutaLote.Multiline = true;
		this.txtRutaLote.Name = "txtRutaLote";
		this.txtRutaLote.Size = new System.Drawing.Size(300, 41);
		this.txtRutaLote.TabIndex = 112;
		this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label10.AutoSize = true;
		this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label10.ForeColor = System.Drawing.Color.White;
		this.label10.Location = new System.Drawing.Point(318, 423);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(165, 20);
		this.label10.TabIndex = 93;
		this.label10.Text = "Cambiar Ruta Lote:";
		this.lblRutaLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.lblRutaLote.AutoSize = true;
		this.lblRutaLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblRutaLote.ForeColor = System.Drawing.Color.White;
		this.lblRutaLote.Location = new System.Drawing.Point(554, 153);
		this.lblRutaLote.Name = "lblRutaLote";
		this.lblRutaLote.Size = new System.Drawing.Size(80, 20);
		this.lblRutaLote.TabIndex = 92;
		this.lblRutaLote.Text = "Ruta Lote";
		this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label9.ForeColor = System.Drawing.Color.White;
		this.label9.Location = new System.Drawing.Point(447, 153);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(94, 20);
		this.label9.TabIndex = 91;
		this.label9.Text = "Ruta Lote:";
		this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label8.AutoSize = true;
		this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label8.ForeColor = System.Drawing.Color.White;
		this.label8.Location = new System.Drawing.Point(318, 371);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(174, 20);
		this.label8.TabIndex = 90;
		this.label8.Text = "Cambiar al Proyecto:";
		this.cbxListaProyectos.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.cbxListaProyectos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaProyectos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaProyectos.FormattingEnabled = true;
		this.cbxListaProyectos.Location = new System.Drawing.Point(511, 371);
		this.cbxListaProyectos.Name = "cbxListaProyectos";
		this.cbxListaProyectos.Size = new System.Drawing.Size(299, 25);
		this.cbxListaProyectos.TabIndex = 89;
		this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.ForeColor = System.Drawing.Color.White;
		this.label7.Location = new System.Drawing.Point(331, 323);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(161, 20);
		this.label7.TabIndex = 88;
		this.label7.Text = "Cambiar al Estado:";
		this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.ForeColor = System.Drawing.Color.White;
		this.label6.Location = new System.Drawing.Point(331, 275);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(161, 20);
		this.label6.TabIndex = 87;
		this.label6.Text = "Asignar al Usuario:";
		this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(500, 14);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(184, 24);
		this.label5.TabIndex = 86;
		this.label5.Text = "Lote Seleccionado";
		this.btnCancelarModificacion.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.btnCancelarModificacion.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelarModificacion.FlatAppearance.BorderSize = 0;
		this.btnCancelarModificacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelarModificacion.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelarModificacion.ForeColor = System.Drawing.Color.White;
		this.btnCancelarModificacion.Image = (System.Drawing.Image)resources.GetObject("btnCancelarModificacion.Image");
		this.btnCancelarModificacion.Location = new System.Drawing.Point(714, 473);
		this.btnCancelarModificacion.Name = "btnCancelarModificacion";
		this.btnCancelarModificacion.Size = new System.Drawing.Size(175, 25);
		this.btnCancelarModificacion.TabIndex = 84;
		this.btnCancelarModificacion.Text = "   Cancelar";
		this.btnCancelarModificacion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelarModificacion.UseVisualStyleBackColor = false;
		this.btnCancelarModificacion.Click += new System.EventHandler(btnCancelarModificacion_Click);
		this.btnAccionModificarLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.btnAccionModificarLote.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAccionModificarLote.FlatAppearance.BorderSize = 0;
		this.btnAccionModificarLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAccionModificarLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAccionModificarLote.ForeColor = System.Drawing.Color.White;
		this.btnAccionModificarLote.Image = (System.Drawing.Image)resources.GetObject("btnAccionModificarLote.Image");
		this.btnAccionModificarLote.Location = new System.Drawing.Point(302, 473);
		this.btnAccionModificarLote.Name = "btnAccionModificarLote";
		this.btnAccionModificarLote.Size = new System.Drawing.Size(175, 25);
		this.btnAccionModificarLote.TabIndex = 84;
		this.btnAccionModificarLote.Text = "   Modificar Lote";
		this.btnAccionModificarLote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnAccionModificarLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAccionModificarLote.UseVisualStyleBackColor = false;
		this.btnAccionModificarLote.Click += new System.EventHandler(btnAccionModificarLote_Click);
		this.cbxListaEstados.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.cbxListaEstados.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaEstados.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaEstados.FormattingEnabled = true;
		this.cbxListaEstados.Location = new System.Drawing.Point(511, 323);
		this.cbxListaEstados.Name = "cbxListaEstados";
		this.cbxListaEstados.Size = new System.Drawing.Size(299, 25);
		this.cbxListaEstados.TabIndex = 85;
		this.cbxListaUsuarios.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.cbxListaUsuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxListaUsuarios.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxListaUsuarios.FormattingEnabled = true;
		this.cbxListaUsuarios.Location = new System.Drawing.Point(511, 270);
		this.cbxListaUsuarios.Name = "cbxListaUsuarios";
		this.cbxListaUsuarios.Size = new System.Drawing.Size(299, 25);
		this.cbxListaUsuarios.TabIndex = 84;
		this.lblEstado.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.lblEstado.AutoSize = true;
		this.lblEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblEstado.ForeColor = System.Drawing.Color.White;
		this.lblEstado.Location = new System.Drawing.Point(553, 230);
		this.lblEstado.Name = "lblEstado";
		this.lblEstado.Size = new System.Drawing.Size(60, 20);
		this.lblEstado.TabIndex = 8;
		this.lblEstado.Text = "Estado";
		this.lblUsuario.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.lblUsuario.AutoSize = true;
		this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblUsuario.ForeColor = System.Drawing.Color.White;
		this.lblUsuario.Location = new System.Drawing.Point(554, 196);
		this.lblUsuario.Name = "lblUsuario";
		this.lblUsuario.Size = new System.Drawing.Size(64, 20);
		this.lblUsuario.TabIndex = 7;
		this.lblUsuario.Text = "Usuario";
		this.lblNombreLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.lblNombreLote.AutoSize = true;
		this.lblNombreLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNombreLote.ForeColor = System.Drawing.Color.White;
		this.lblNombreLote.Location = new System.Drawing.Point(554, 112);
		this.lblNombreLote.Name = "lblNombreLote";
		this.lblNombreLote.Size = new System.Drawing.Size(65, 20);
		this.lblNombreLote.TabIndex = 6;
		this.lblNombreLote.Text = "Nombre";
		this.lblCodigoLote.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.lblCodigoLote.AutoSize = true;
		this.lblCodigoLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblCodigoLote.ForeColor = System.Drawing.Color.White;
		this.lblCodigoLote.Location = new System.Drawing.Point(553, 73);
		this.lblCodigoLote.Name = "lblCodigoLote";
		this.lblCodigoLote.Size = new System.Drawing.Size(59, 20);
		this.lblCodigoLote.TabIndex = 5;
		this.lblCodigoLote.Text = "Código";
		this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(465, 196);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(76, 20);
		this.label4.TabIndex = 4;
		this.label4.Text = "Usuario:";
		this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(465, 230);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(71, 20);
		this.label3.TabIndex = 3;
		this.label3.Text = "Estado:";
		this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(424, 112);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(117, 20);
		this.label2.TabIndex = 2;
		this.label2.Text = "Nombre Lote:";
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(430, 73);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(111, 20);
		this.label1.TabIndex = 1;
		this.label1.Text = "Código Lote:";
		this.btnModificarLote.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnModificarLote.BackColor = System.Drawing.Color.SeaGreen;
		this.btnModificarLote.FlatAppearance.BorderSize = 0;
		this.btnModificarLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnModificarLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnModificarLote.ForeColor = System.Drawing.Color.White;
		this.btnModificarLote.Image = (System.Drawing.Image)resources.GetObject("btnModificarLote.Image");
		this.btnModificarLote.Location = new System.Drawing.Point(374, 574);
		this.btnModificarLote.Name = "btnModificarLote";
		this.btnModificarLote.Size = new System.Drawing.Size(175, 25);
		this.btnModificarLote.TabIndex = 83;
		this.btnModificarLote.Text = "   Modificar Lote";
		this.btnModificarLote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnModificarLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnModificarLote.UseVisualStyleBackColor = false;
		this.btnModificarLote.Click += new System.EventHandler(btnModificarLote_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1234, 611);
		base.Controls.Add(this.pnlModificarLote);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnActualizar);
		base.Controls.Add(this.btnExportar);
		base.Controls.Add(this.pnlLotesDetalle);
		base.Controls.Add(this.pnlTotalLotesUsuario);
		base.Controls.Add(this.pnlTotalLotesEstados);
		base.Controls.Add(this.btnModificarLote);
		base.Name = "frmMonitorLotes";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Monitor de Lotes Activos";
		base.Load += new System.EventHandler(frmMonitorLotes_Load);
		this.pnlTotalLotesEstados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesEstados).EndInit();
		this.pnlTotalLotesUsuario.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesUsuarios).EndInit();
		this.pnlLotesDetalle.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDetalle).EndInit();
		this.pnlModificarLote.ResumeLayout(false);
		this.pnlModificarLote.PerformLayout();
		base.ResumeLayout(false);
	}
}
