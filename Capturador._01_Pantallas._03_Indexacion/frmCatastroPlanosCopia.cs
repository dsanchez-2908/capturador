using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmCatastroPlanosCopia : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private eProyectoConfiguracion oProyectoConfiguracion;

	private string carpetaPlanosConfigurada;

	private string carpetaLotesConfigurada;

	private string carpetaPlanosSeleccionada;

	private string carpetaLotesSelecionada;

	private IContainer components = null;

	private Button btnCerrar;

	private Button btnCancelar;

	private Panel pnlDetallePlanos;

	private DataGridView dgvDetallePlanos;

	private Button btnSeleccionarCarpetaLotes;

	private Button btnBuscarLotes;

	private Button btnSeleccionarCarpetaPlanos;

	private TextBox txtCarpetaPlanos;

	private TextBox txtCarpetaLotes;

	private Panel pnlLotesEncontrados;

	private DataGridView dgvLotesEncontrados;

	private Button btnCopiarPlanos;

	private GroupBox groupBox1;

	private Button btnActualizar;

	private Button btnExportar;

	private Panel pnlPlanosNoEncontrados;

	private DataGridView dgvPlanosNoEncontrados;

	private Panel pnlLotesNoCargados;

	private DataGridView dgvLotesNoCargados;

	public frmCatastroPlanosCopia(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
	}

	private void frmCatastroPlanosCopia_Load(object sender, EventArgs e)
	{
		actualizarTablaPlanosCargados();
		cargarUltimaCarpetaSeleccionada();
		ajustarFormularioInicial();
		crearPrimerTablaResultado();
		actualizarBotones();
	}

	private void cargarUltimaCarpetaSeleccionada()
	{
		oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 6);
		carpetaPlanosConfigurada = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		carpetaLotesConfigurada = oProyectoConfiguracion.dsRutaSalida;
		txtCarpetaPlanos.Text = carpetaPlanosConfigurada;
		txtCarpetaLotes.Text = carpetaLotesConfigurada;
	}

	private void ajustarFormularioInicial()
	{
		base.MaximizeBox = false;
		base.Size = new Size(1600, 750);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void crearPrimerTablaResultado()
	{
		DataTable oTablaDetalle = new DataTable();
		oTablaDetalle.Columns.Add("Id");
		oTablaDetalle.Columns.Add("Código Documento");
		oTablaDetalle.Columns.Add("Nombre de Archivo");
		oTablaDetalle.Columns.Add("Ruta de Lote");
		dgvLotesEncontrados.DataSource = oTablaDetalle;
		darFormatoTablarResultado();
		DataTable oTablaNoEncontrados = new DataTable();
		oTablaNoEncontrados.Columns.Add("Id");
		oTablaNoEncontrados.Columns.Add("Fecha de Alta");
		oTablaNoEncontrados.Columns.Add("Usuario Alta");
		oTablaNoEncontrados.Columns.Add("Código Documento");
		oTablaNoEncontrados.Columns.Add("Nombre Archivo");
		dgvPlanosNoEncontrados.DataSource = oTablaDetalle;
		darFormatoTablarNoEncontrados();
		DataTable oTablaLotesNoCargados = new DataTable();
		oTablaLotesNoCargados.Columns.Add("Código Documento");
		oTablaLotesNoCargados.Columns.Add("Ruta Lote");
		dgvLotesNoCargados.DataSource = oTablaLotesNoCargados;
		darFormatoTablaLotesNoEncontrados();
	}

	private void actualizarTablaPlanosCargados()
	{
		dgvDetallePlanos.DataSource = nIndexacion.buscarPlanosDisponibles(oUsuarioLogueado);
		dgvDetallePlanos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		dgvDetallePlanos.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvDetallePlanos.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvDetallePlanos.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		dgvDetallePlanos.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		dgvDetallePlanos.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		darFormatoTablaPlanosCargados();
	}

	private void darFormatoTablaPlanosCargados()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Planos Cargados Pendiente de Copiar";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvDetallePlanos.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Planos: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvDetallePlanos.Dock = DockStyle.Fill;
		pnlDetallePlanos.Controls.Clear();
		pnlDetallePlanos.Controls.Add(dgvDetallePlanos);
		pnlDetallePlanos.Controls.Add(labelTitulo);
		pnlDetallePlanos.Controls.Add(labelTotal);
	}

	private void btnCerrar_Click_1(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click_1(object sender, EventArgs e)
	{
		deshabilitarFormulario();
	}

	private void deshabilitarFormulario()
	{
		DataTable oCopiaTabla = new DataTable();
		oCopiaTabla = dgvLotesEncontrados.DataSource as DataTable;
		oCopiaTabla.Rows.Clear();
		darFormatoTablarResultado();
		dgvLotesEncontrados.DataSource = oCopiaTabla;
		DataTable oCopiaNoEncontrado = new DataTable();
		oCopiaNoEncontrado = dgvPlanosNoEncontrados.DataSource as DataTable;
		oCopiaNoEncontrado.Rows.Clear();
		darFormatoTablarNoEncontrados();
		dgvPlanosNoEncontrados.DataSource = oCopiaNoEncontrado;
		DataTable oCopiaLotesNoCargado = new DataTable();
		oCopiaLotesNoCargado = dgvLotesNoCargados.DataSource as DataTable;
		oCopiaLotesNoCargado.Rows.Clear();
		darFormatoTablaLotesNoEncontrados();
		dgvLotesNoCargados.DataSource = oCopiaLotesNoCargado;
		actualizarTablaPlanosCargados();
		actualizarBotones();
	}

	private void actualizarBotones()
	{
		if (dgvLotesEncontrados.Rows.Count != 0)
		{
			btnCopiarPlanos.Enabled = true;
			btnCopiarPlanos.BackColor = Color.SeaGreen;
			btnCancelar.Enabled = true;
			btnCancelar.BackColor = Color.Salmon;
		}
		else
		{
			btnCopiarPlanos.Enabled = false;
			btnCopiarPlanos.BackColor = Color.DarkGray;
			btnCancelar.Enabled = false;
			btnCancelar.BackColor = Color.DarkGray;
		}
	}

	private void btnSeleccionarCarpetaPlanos_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLote = new FolderBrowserDialog();
		oSeleccionarLote.Description = "Seleccionar la carpeta de Planos";
		oSeleccionarLote.SelectedPath = carpetaPlanosConfigurada;
		oSeleccionarLote.ShowNewFolderButton = false;
		if (oSeleccionarLote.ShowDialog() == DialogResult.OK)
		{
			carpetaPlanosSeleccionada = oSeleccionarLote.SelectedPath;
			txtCarpetaPlanos.Text = carpetaPlanosSeleccionada;
			nConfiguracion.actualizarUltimaCarpetaOrigen(oUsuarioLogueado, 6, carpetaPlanosConfigurada, carpetaPlanosSeleccionada);
		}
	}

	private void btnSeleccionarCarpetaLotes_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLote = new FolderBrowserDialog();
		oSeleccionarLote.Description = "Seleccionar la carpeta de Lotes";
		oSeleccionarLote.SelectedPath = carpetaPlanosConfigurada;
		oSeleccionarLote.ShowNewFolderButton = false;
		if (oSeleccionarLote.ShowDialog() == DialogResult.OK)
		{
			carpetaLotesSelecionada = oSeleccionarLote.SelectedPath;
			txtCarpetaLotes.Text = carpetaLotesSelecionada;
			nConfiguracion.actualizarUltimaCarpetaSalida(oUsuarioLogueado, 6, carpetaLotesConfigurada, carpetaLotesSelecionada);
		}
	}

	private void btnBuscarLotes_Click(object sender, EventArgs e)
	{
		DataTable oOriginalDataTable = new DataTable();
		oOriginalDataTable = dgvDetallePlanos.DataSource as DataTable;
		DataTable oLotePlanosBuscar = new DataTable();
		oLotePlanosBuscar = oOriginalDataTable.Clone();
		foreach (DataRow fila in oOriginalDataTable.Rows)
		{
			oLotePlanosBuscar.ImportRow(fila);
		}
		dgvLotesEncontrados.DataSource = nIndexacion.buscarLotesPlanos(oUsuarioLogueado, txtCarpetaPlanos.Text, txtCarpetaLotes.Text, oLotePlanosBuscar);
		dgvLotesEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		dgvLotesEncontrados.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvLotesEncontrados.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvLotesEncontrados.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvLotesEncontrados.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		darFormatoTablarResultado();
		buscarPlanosNoEncontrados();
		buscarLotesNoCargados();
		actualizarBotones();
	}

	private void darFormatoTablarResultado()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Planos Encontrados a copiar";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvLotesEncontrados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Planos Encontrados: {totalRegistros}";
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

	private void btnCopiarPlanos_Click(object sender, EventArgs e)
	{
		DataTable oOriginalDataTable = new DataTable();
		oOriginalDataTable = dgvLotesEncontrados.DataSource as DataTable;
		DataTable oPlanosCopiar = new DataTable();
		oPlanosCopiar = oOriginalDataTable.Clone();
		foreach (DataRow fila in oOriginalDataTable.Rows)
		{
			oPlanosCopiar.ImportRow(fila);
		}
		try
		{
			nIndexacion.copiarPlanos(oUsuarioLogueado, txtCarpetaPlanos.Text, oPlanosCopiar);
			MessageBox.Show("Se copiaron los Planos correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			deshabilitarFormulario();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void btnActualizar_Click(object sender, EventArgs e)
	{
		actualizarTablaPlanosCargados();
	}

	private void btnExportar_Click(object sender, EventArgs e)
	{
		if (dgvDetallePlanos.Rows.Count <= 0)
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
		DataTable oTablaExportar = (DataTable)dgvDetallePlanos.DataSource;
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

	private void buscarPlanosNoEncontrados()
	{
		DataTable oTablaOriginaPlanosCargados = new DataTable();
		oTablaOriginaPlanosCargados = dgvDetallePlanos.DataSource as DataTable;
		DataTable oTablaPlanosCargados = new DataTable();
		oTablaPlanosCargados = oTablaOriginaPlanosCargados.Clone();
		foreach (DataRow fila in oTablaOriginaPlanosCargados.Rows)
		{
			oTablaPlanosCargados.ImportRow(fila);
		}
		DataTable oTablaOriginaPlanosEncotrados = new DataTable();
		oTablaOriginaPlanosEncotrados = dgvLotesEncontrados.DataSource as DataTable;
		DataTable oTablaPlanosEncontrados = new DataTable();
		oTablaPlanosEncontrados = oTablaOriginaPlanosEncotrados.Clone();
		foreach (DataRow fila2 in oTablaOriginaPlanosEncotrados.Rows)
		{
			oTablaPlanosEncontrados.ImportRow(fila2);
		}
		dgvPlanosNoEncontrados.DataSource = nIndexacion.buscarPlanosNoEncontrados(oTablaPlanosCargados, oTablaPlanosEncontrados);
		dgvPlanosNoEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		dgvPlanosNoEncontrados.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvPlanosNoEncontrados.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		dgvPlanosNoEncontrados.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		dgvPlanosNoEncontrados.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		dgvPlanosNoEncontrados.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		darFormatoTablarNoEncontrados();
	}

	private void darFormatoTablarNoEncontrados()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Archivos de Planos No Encontrados";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvPlanosNoEncontrados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Planos No Encontrados: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvPlanosNoEncontrados.Dock = DockStyle.Fill;
		pnlPlanosNoEncontrados.Controls.Clear();
		pnlPlanosNoEncontrados.Controls.Add(dgvPlanosNoEncontrados);
		pnlPlanosNoEncontrados.Controls.Add(labelTitulo);
		pnlPlanosNoEncontrados.Controls.Add(labelTotal);
	}

	private void buscarLotesNoCargados()
	{
		DataTable oTablaOriginaPlanosCargados = new DataTable();
		oTablaOriginaPlanosCargados = dgvDetallePlanos.DataSource as DataTable;
		DataTable oTablaPlanosCargados = new DataTable();
		oTablaPlanosCargados = oTablaOriginaPlanosCargados.Clone();
		foreach (DataRow fila in oTablaOriginaPlanosCargados.Rows)
		{
			oTablaPlanosCargados.ImportRow(fila);
		}
		dgvLotesNoCargados.DataSource = nIndexacion.buscarCarpetaLoteNoCargado(oUsuarioLogueado, oTablaPlanosCargados, txtCarpetaLotes.Text);
		dgvLotesNoCargados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		darFormatoTablaLotesNoEncontrados();
	}

	private void darFormatoTablaLotesNoEncontrados()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Lotes NO cargados";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvLotesNoCargados.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Planos: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvLotesNoCargados.Dock = DockStyle.Fill;
		pnlLotesNoCargados.Controls.Clear();
		pnlLotesNoCargados.Controls.Add(dgvLotesNoCargados);
		pnlLotesNoCargados.Controls.Add(labelTitulo);
		pnlLotesNoCargados.Controls.Add(labelTotal);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._03_Indexacion.frmCatastroPlanosCopia));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle29 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle30 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.pnlDetallePlanos = new System.Windows.Forms.Panel();
		this.dgvDetallePlanos = new System.Windows.Forms.DataGridView();
		this.btnSeleccionarCarpetaLotes = new System.Windows.Forms.Button();
		this.btnBuscarLotes = new System.Windows.Forms.Button();
		this.btnSeleccionarCarpetaPlanos = new System.Windows.Forms.Button();
		this.txtCarpetaPlanos = new System.Windows.Forms.TextBox();
		this.txtCarpetaLotes = new System.Windows.Forms.TextBox();
		this.pnlLotesEncontrados = new System.Windows.Forms.Panel();
		this.dgvLotesEncontrados = new System.Windows.Forms.DataGridView();
		this.btnCopiarPlanos = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.btnActualizar = new System.Windows.Forms.Button();
		this.btnExportar = new System.Windows.Forms.Button();
		this.pnlPlanosNoEncontrados = new System.Windows.Forms.Panel();
		this.dgvPlanosNoEncontrados = new System.Windows.Forms.DataGridView();
		this.pnlLotesNoCargados = new System.Windows.Forms.Panel();
		this.dgvLotesNoCargados = new System.Windows.Forms.DataGridView();
		this.pnlDetallePlanos.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDetallePlanos).BeginInit();
		this.pnlLotesEncontrados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesEncontrados).BeginInit();
		this.pnlPlanosNoEncontrados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvPlanosNoEncontrados).BeginInit();
		this.pnlLotesNoCargados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesNoCargados).BeginInit();
		base.SuspendLayout();
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(962, 662);
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
		this.btnCancelar.Location = new System.Drawing.Point(826, 662);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(130, 25);
		this.btnCancelar.TabIndex = 62;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click_1);
		this.pnlDetallePlanos.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlDetallePlanos.Controls.Add(this.dgvDetallePlanos);
		this.pnlDetallePlanos.Location = new System.Drawing.Point(12, 12);
		this.pnlDetallePlanos.Name = "pnlDetallePlanos";
		this.pnlDetallePlanos.Size = new System.Drawing.Size(550, 632);
		this.pnlDetallePlanos.TabIndex = 65;
		this.dgvDetallePlanos.AllowUserToAddRows = false;
		this.dgvDetallePlanos.AllowUserToDeleteRows = false;
		this.dgvDetallePlanos.AllowUserToResizeRows = false;
		this.dgvDetallePlanos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvDetallePlanos.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvDetallePlanos.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvDetallePlanos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvDetallePlanos.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle17.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle17.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle17.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle17.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDetallePlanos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
		this.dgvDetallePlanos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvDetallePlanos.DefaultCellStyle = dataGridViewCellStyle18;
		this.dgvDetallePlanos.EnableHeadersVisualStyles = false;
		this.dgvDetallePlanos.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvDetallePlanos.Location = new System.Drawing.Point(16, 35);
		this.dgvDetallePlanos.Name = "dgvDetallePlanos";
		this.dgvDetallePlanos.ReadOnly = true;
		this.dgvDetallePlanos.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle19.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle19.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDetallePlanos.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
		this.dgvDetallePlanos.RowHeadersVisible = false;
		this.dgvDetallePlanos.RowHeadersWidth = 15;
		dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle20.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle20.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.White;
		this.dgvDetallePlanos.RowsDefaultCellStyle = dataGridViewCellStyle20;
		this.dgvDetallePlanos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvDetallePlanos.Size = new System.Drawing.Size(512, 569);
		this.dgvDetallePlanos.TabIndex = 18;
		this.btnSeleccionarCarpetaLotes.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarCarpetaLotes.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarCarpetaLotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarCarpetaLotes.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarCarpetaLotes.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarCarpetaLotes.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarCarpetaLotes.Image");
		this.btnSeleccionarCarpetaLotes.Location = new System.Drawing.Point(592, 49);
		this.btnSeleccionarCarpetaLotes.Name = "btnSeleccionarCarpetaLotes";
		this.btnSeleccionarCarpetaLotes.Size = new System.Drawing.Size(234, 25);
		this.btnSeleccionarCarpetaLotes.TabIndex = 72;
		this.btnSeleccionarCarpetaLotes.Text = "   Seleccionar Carpeta Lotes";
		this.btnSeleccionarCarpetaLotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnSeleccionarCarpetaLotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarCarpetaLotes.UseVisualStyleBackColor = false;
		this.btnSeleccionarCarpetaLotes.Click += new System.EventHandler(btnSeleccionarCarpetaLotes_Click);
		this.btnBuscarLotes.BackColor = System.Drawing.Color.SeaGreen;
		this.btnBuscarLotes.FlatAppearance.BorderSize = 0;
		this.btnBuscarLotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnBuscarLotes.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBuscarLotes.ForeColor = System.Drawing.Color.White;
		this.btnBuscarLotes.Image = (System.Drawing.Image)resources.GetObject("btnBuscarLotes.Image");
		this.btnBuscarLotes.Location = new System.Drawing.Point(592, 89);
		this.btnBuscarLotes.Name = "btnBuscarLotes";
		this.btnBuscarLotes.Size = new System.Drawing.Size(500, 25);
		this.btnBuscarLotes.TabIndex = 73;
		this.btnBuscarLotes.Text = "   Buscar Lotes";
		this.btnBuscarLotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnBuscarLotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnBuscarLotes.UseVisualStyleBackColor = false;
		this.btnBuscarLotes.Click += new System.EventHandler(btnBuscarLotes_Click);
		this.btnSeleccionarCarpetaPlanos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarCarpetaPlanos.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarCarpetaPlanos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarCarpetaPlanos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarCarpetaPlanos.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarCarpetaPlanos.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarCarpetaPlanos.Image");
		this.btnSeleccionarCarpetaPlanos.Location = new System.Drawing.Point(592, 12);
		this.btnSeleccionarCarpetaPlanos.Name = "btnSeleccionarCarpetaPlanos";
		this.btnSeleccionarCarpetaPlanos.Size = new System.Drawing.Size(234, 25);
		this.btnSeleccionarCarpetaPlanos.TabIndex = 74;
		this.btnSeleccionarCarpetaPlanos.Text = "   Seleccionar Carpeta Planos";
		this.btnSeleccionarCarpetaPlanos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnSeleccionarCarpetaPlanos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarCarpetaPlanos.UseVisualStyleBackColor = false;
		this.btnSeleccionarCarpetaPlanos.Click += new System.EventHandler(btnSeleccionarCarpetaPlanos_Click);
		this.txtCarpetaPlanos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCarpetaPlanos.Location = new System.Drawing.Point(832, 12);
		this.txtCarpetaPlanos.Name = "txtCarpetaPlanos";
		this.txtCarpetaPlanos.Size = new System.Drawing.Size(260, 26);
		this.txtCarpetaPlanos.TabIndex = 75;
		this.txtCarpetaPlanos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.txtCarpetaLotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCarpetaLotes.Location = new System.Drawing.Point(832, 47);
		this.txtCarpetaLotes.Name = "txtCarpetaLotes";
		this.txtCarpetaLotes.Size = new System.Drawing.Size(260, 26);
		this.txtCarpetaLotes.TabIndex = 76;
		this.txtCarpetaLotes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.pnlLotesEncontrados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLotesEncontrados.Controls.Add(this.dgvLotesEncontrados);
		this.pnlLotesEncontrados.Location = new System.Drawing.Point(592, 131);
		this.pnlLotesEncontrados.Name = "pnlLotesEncontrados";
		this.pnlLotesEncontrados.Size = new System.Drawing.Size(500, 513);
		this.pnlLotesEncontrados.TabIndex = 66;
		this.dgvLotesEncontrados.AllowUserToAddRows = false;
		this.dgvLotesEncontrados.AllowUserToDeleteRows = false;
		this.dgvLotesEncontrados.AllowUserToResizeRows = false;
		this.dgvLotesEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle21.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle21.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle21.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle21.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle21;
		this.dgvLotesEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle22.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesEncontrados.DefaultCellStyle = dataGridViewCellStyle22;
		this.dgvLotesEncontrados.EnableHeadersVisualStyles = false;
		this.dgvLotesEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesEncontrados.Location = new System.Drawing.Point(16, 26);
		this.dgvLotesEncontrados.Name = "dgvLotesEncontrados";
		this.dgvLotesEncontrados.ReadOnly = true;
		this.dgvLotesEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle23.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle23.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle23.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle23;
		this.dgvLotesEncontrados.RowHeadersVisible = false;
		this.dgvLotesEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle24.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle24.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle24.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle24.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle24.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle24;
		this.dgvLotesEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesEncontrados.Size = new System.Drawing.Size(452, 459);
		this.dgvLotesEncontrados.TabIndex = 18;
		this.btnCopiarPlanos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnCopiarPlanos.FlatAppearance.BorderSize = 0;
		this.btnCopiarPlanos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCopiarPlanos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCopiarPlanos.ForeColor = System.Drawing.Color.White;
		this.btnCopiarPlanos.Image = (System.Drawing.Image)resources.GetObject("btnCopiarPlanos.Image");
		this.btnCopiarPlanos.Location = new System.Drawing.Point(592, 662);
		this.btnCopiarPlanos.Name = "btnCopiarPlanos";
		this.btnCopiarPlanos.Size = new System.Drawing.Size(189, 25);
		this.btnCopiarPlanos.TabIndex = 77;
		this.btnCopiarPlanos.Text = "   Copiar Planos";
		this.btnCopiarPlanos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCopiarPlanos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCopiarPlanos.UseVisualStyleBackColor = false;
		this.btnCopiarPlanos.Click += new System.EventHandler(btnCopiarPlanos_Click);
		this.groupBox1.BackColor = System.Drawing.Color.White;
		this.groupBox1.Location = new System.Drawing.Point(569, 12);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(10, 676);
		this.groupBox1.TabIndex = 78;
		this.groupBox1.TabStop = false;
		this.btnActualizar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnActualizar.FlatAppearance.BorderSize = 0;
		this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnActualizar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnActualizar.ForeColor = System.Drawing.Color.White;
		this.btnActualizar.Image = (System.Drawing.Image)resources.GetObject("btnActualizar.Image");
		this.btnActualizar.Location = new System.Drawing.Point(12, 663);
		this.btnActualizar.Name = "btnActualizar";
		this.btnActualizar.Size = new System.Drawing.Size(250, 25);
		this.btnActualizar.TabIndex = 79;
		this.btnActualizar.Text = "   Actualizar";
		this.btnActualizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnActualizar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnActualizar.UseVisualStyleBackColor = false;
		this.btnActualizar.Click += new System.EventHandler(btnActualizar_Click);
		this.btnExportar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnExportar.FlatAppearance.BorderSize = 0;
		this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnExportar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnExportar.ForeColor = System.Drawing.Color.White;
		this.btnExportar.Image = (System.Drawing.Image)resources.GetObject("btnExportar.Image");
		this.btnExportar.Location = new System.Drawing.Point(312, 663);
		this.btnExportar.Name = "btnExportar";
		this.btnExportar.Size = new System.Drawing.Size(250, 25);
		this.btnExportar.TabIndex = 80;
		this.btnExportar.Text = "   Exportar";
		this.btnExportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnExportar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnExportar.UseVisualStyleBackColor = false;
		this.btnExportar.Click += new System.EventHandler(btnExportar_Click);
		this.pnlPlanosNoEncontrados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlPlanosNoEncontrados.Controls.Add(this.dgvPlanosNoEncontrados);
		this.pnlPlanosNoEncontrados.Location = new System.Drawing.Point(1128, 12);
		this.pnlPlanosNoEncontrados.Name = "pnlPlanosNoEncontrados";
		this.pnlPlanosNoEncontrados.Size = new System.Drawing.Size(444, 330);
		this.pnlPlanosNoEncontrados.TabIndex = 67;
		this.dgvPlanosNoEncontrados.AllowUserToAddRows = false;
		this.dgvPlanosNoEncontrados.AllowUserToDeleteRows = false;
		this.dgvPlanosNoEncontrados.AllowUserToResizeRows = false;
		this.dgvPlanosNoEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvPlanosNoEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvPlanosNoEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvPlanosNoEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvPlanosNoEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle25.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle25.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle25.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle25.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvPlanosNoEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle25;
		this.dgvPlanosNoEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle26.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle26.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle26.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle26.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle26.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvPlanosNoEncontrados.DefaultCellStyle = dataGridViewCellStyle26;
		this.dgvPlanosNoEncontrados.EnableHeadersVisualStyles = false;
		this.dgvPlanosNoEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvPlanosNoEncontrados.Location = new System.Drawing.Point(35, 26);
		this.dgvPlanosNoEncontrados.Name = "dgvPlanosNoEncontrados";
		this.dgvPlanosNoEncontrados.ReadOnly = true;
		this.dgvPlanosNoEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle27.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle27.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle27.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvPlanosNoEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle27;
		this.dgvPlanosNoEncontrados.RowHeadersVisible = false;
		this.dgvPlanosNoEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle28.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle28.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle28.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle28.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle28.SelectionForeColor = System.Drawing.Color.White;
		this.dgvPlanosNoEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle28;
		this.dgvPlanosNoEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvPlanosNoEncontrados.Size = new System.Drawing.Size(373, 76);
		this.dgvPlanosNoEncontrados.TabIndex = 18;
		this.pnlLotesNoCargados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLotesNoCargados.Controls.Add(this.dgvLotesNoCargados);
		this.pnlLotesNoCargados.Location = new System.Drawing.Point(1128, 385);
		this.pnlLotesNoCargados.Name = "pnlLotesNoCargados";
		this.pnlLotesNoCargados.Size = new System.Drawing.Size(444, 259);
		this.pnlLotesNoCargados.TabIndex = 67;
		this.dgvLotesNoCargados.AllowUserToAddRows = false;
		this.dgvLotesNoCargados.AllowUserToDeleteRows = false;
		this.dgvLotesNoCargados.AllowUserToResizeRows = false;
		this.dgvLotesNoCargados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesNoCargados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesNoCargados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesNoCargados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesNoCargados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle29.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle29.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle29.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle29.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle29.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle29.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle29.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesNoCargados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle29;
		this.dgvLotesNoCargados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle30.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle30.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle30.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle30.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle30.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle30.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesNoCargados.DefaultCellStyle = dataGridViewCellStyle30;
		this.dgvLotesNoCargados.EnableHeadersVisualStyles = false;
		this.dgvLotesNoCargados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesNoCargados.Location = new System.Drawing.Point(25, 58);
		this.dgvLotesNoCargados.Name = "dgvLotesNoCargados";
		this.dgvLotesNoCargados.ReadOnly = true;
		this.dgvLotesNoCargados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle31.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle31.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle31.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle31.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle31.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle31.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesNoCargados.RowHeadersDefaultCellStyle = dataGridViewCellStyle31;
		this.dgvLotesNoCargados.RowHeadersVisible = false;
		this.dgvLotesNoCargados.RowHeadersWidth = 15;
		dataGridViewCellStyle32.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle32.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle32.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle32.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle32.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesNoCargados.RowsDefaultCellStyle = dataGridViewCellStyle32;
		this.dgvLotesNoCargados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesNoCargados.Size = new System.Drawing.Size(403, 173);
		this.dgvLotesNoCargados.TabIndex = 18;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1584, 711);
		base.Controls.Add(this.pnlLotesNoCargados);
		base.Controls.Add(this.pnlPlanosNoEncontrados);
		base.Controls.Add(this.btnExportar);
		base.Controls.Add(this.btnActualizar);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.btnCopiarPlanos);
		base.Controls.Add(this.pnlLotesEncontrados);
		base.Controls.Add(this.txtCarpetaLotes);
		base.Controls.Add(this.txtCarpetaPlanos);
		base.Controls.Add(this.btnSeleccionarCarpetaPlanos);
		base.Controls.Add(this.btnBuscarLotes);
		base.Controls.Add(this.btnSeleccionarCarpetaLotes);
		base.Controls.Add(this.pnlDetallePlanos);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.MinimizeBox = false;
		base.Name = "frmCatastroPlanosCopia";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Copia de Planos de Catastro";
		base.Load += new System.EventHandler(frmCatastroPlanosCopia_Load);
		this.pnlDetallePlanos.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDetallePlanos).EndInit();
		this.pnlLotesEncontrados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesEncontrados).EndInit();
		this.pnlPlanosNoEncontrados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvPlanosNoEncontrados).EndInit();
		this.pnlLotesNoCargados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesNoCargados).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
