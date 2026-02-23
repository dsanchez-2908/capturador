using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;
using PdfiumViewer;

namespace Capturador._01_Pantallas._04_Separador;

public class frmMunicipalidadVillaReginaCatastro : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string rutaCarpetaInicial;

	private string rutaCarpetaSalida;

	private string rutaLoteOrigen;

	private string nombreLote;

	private string nombreArchivoIndice = "INDEX_TERMINADO.DAT";

	private int cantidadTotalDespachos;

	private List<string> pdfFiles = new List<string>();

	private int currentFileIndex = 0;

	private int currentPage = 0;

	private int pagesPerView = 1;

	private PdfDocument pdfDocument;

	private bool hasChanges = false;

	private List<string> listaArchivos = new List<string>();

	private List<eCodigosBarrasEncontrados_v3> listaCodigoBarraFinal = new List<eCodigosBarrasEncontrados_v3>();

	private BindingList<eLote> listaDisponibles;

	private BindingList<eLote> listaSeleccionados;

	private System.Windows.Forms.Timer timerLimpiezaMemoria;

	private bool isDisposing = false;

	private IContainer components = null;

	private Panel pnlVisor;

	private GroupBox gbBotonesNavegacion;

	private ListBox lboxListaArchivos;

	private Label label1;

	private TextBox txtArchivoActual;

	private TextBox txtTotalPaginas;

	private Panel pnlAsignarCodigo;

	private TextBox txtNuevoCodigo;

	private Label label3;

	private TextBox txtPaginaSeleccionada;

	private Label label4;

	private GroupBox gbSeleccionarCarpetaSalida;

	private TextBox txtCarpetaSalida;

	private ProgressBar progressBar1;

	private DataGridView dgvTotalLotesEncontrados;

	private DataGridView dgvIndicesEncontrados;

	private DataGridView dgvSeparadoresEncontrados;

	private Label label5;

	private Button btnSeleccionarCarpetaSalida;

	private Button btnProcesar;

	private Button btnCancelar;

	private Button btnCerrar;

	private Button btnPaginaSiguiente;

	private Button btnPaginaAnterior;

	private Button btnArchivoSiguiente;

	private Button btnArchivoAnterior;

	private Button btnAgregar;

	private Button btnZoom;

	private TextBox txtSeleccion;

	private GroupBox groupBox1;

	private Button btnCancelarAsignar;

	private Button btnAsignarCodigo;

	private Panel pnlLotesDisponibles;

	private DataGridView dgvLotesDisponibles;

	private Button btnSeleccionarTodo;

	private Button btnDeseleccionarTodo;

	private Button btnAgregarLotes;

	private Button btnQuitarLotes;

	private Panel pnlLotesSeleccionados;

	private DataGridView dgvLotesSeleccionados;

	private Button btnProcesarIndices;

	private Panel pnlSeparadoresEncontrados;

	public frmMunicipalidadVillaReginaCatastro(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
		base.KeyPreview = true;
		base.KeyDown += frmDespachoSeparar_v2_KeyDown;
	}

	private void frmMunicipalidadVillaReginaCatastro_Load(object sender, EventArgs e)
	{
		InicializarTimerLimpieza();
		eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		rutaCarpetaSalida = oProyectoConfiguracion.dsRutaSalida;
		txtCarpetaSalida.Text = rutaCarpetaSalida;
		rutaLoteOrigen = rutaCarpetaInicial;
		listaDisponibles = new BindingList<eLote>(nLotes.obtenerLotesDisponibleSeparacion(oUsuarioLogueado, 6));
		listaSeleccionados = new BindingList<eLote>();
		configurarDgvLotes(dgvLotesDisponibles, listaDisponibles, pnlLotesDisponibles, "Listado de Lotes Disponibles");
		configurarDgvLotes(dgvLotesSeleccionados, listaSeleccionados, pnlLotesSeleccionados, "Listado de Lotes Seleccionados");
		dgvSeparadoresEncontrados.CellValidating += dgv_CellValidating;
		ajustarFormulario_SeleccionarLote();
	}

	private void btnAgregarLotes_Click(object sender, EventArgs e)
	{
		for (int i = dgvLotesDisponibles.Rows.Count - 1; i >= 0; i--)
		{
			DataGridViewRow row = dgvLotesDisponibles.Rows[i];
			if (Convert.ToBoolean(row.Cells["chkSeleccionado"].Value))
			{
				eLote lote = (eLote)row.DataBoundItem;
				listaDisponibles.Remove(lote);
				listaSeleccionados.Add(lote);
			}
		}
		configurarDgvLotes(dgvLotesDisponibles, listaDisponibles, pnlLotesDisponibles, "Listado de Lotes Disponibles");
		configurarDgvLotes(dgvLotesSeleccionados, listaSeleccionados, pnlLotesSeleccionados, "Listado de Lotes Seleccionados");
		activarBotonBuscarSeperadores();
	}

	private void btnQuitarLotes_Click(object sender, EventArgs e)
	{
		BindingList<eLote> disponibles = (BindingList<eLote>)dgvLotesDisponibles.DataSource;
		BindingList<eLote> seleccionados = (BindingList<eLote>)dgvLotesSeleccionados.DataSource;
		bool haySeleccionados = false;
		for (int i = dgvLotesSeleccionados.Rows.Count - 1; i >= 0; i--)
		{
			DataGridViewRow row = dgvLotesSeleccionados.Rows[i];
			if (Convert.ToBoolean(row.Cells["chkSeleccionado"].Value ?? ((object)false)))
			{
				haySeleccionados = true;
				eLote lote = (eLote)row.DataBoundItem;
				if (!disponibles.Any((eLote x) => x.dsNombreLote == lote.dsNombreLote))
				{
					disponibles.Add(lote);
				}
				seleccionados.Remove(lote);
			}
		}
		if (!haySeleccionados)
		{
			MessageBox.Show("Debe seleccionar al menos un lote para quitar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			return;
		}
		dgvLotesDisponibles.DataSource = null;
		dgvLotesDisponibles.DataSource = disponibles;
		actualizarLotesDisponibles();
		dgvLotesSeleccionados.DataSource = null;
		dgvLotesSeleccionados.DataSource = seleccionados;
		actualizarLotesSeleccionados();
		activarBotonBuscarSeperadores();
	}

	private void activarBotonBuscarSeperadores()
	{
		if (dgvLotesSeleccionados.Rows.Count > 0)
		{
			btnProcesarIndices.Enabled = true;
			btnProcesarIndices.BackColor = Color.SeaGreen;
		}
		else
		{
			btnProcesarIndices.Enabled = false;
			btnProcesarIndices.BackColor = Color.DarkGray;
		}
	}

	private void configurarDgvLotes(DataGridView dgv, BindingList<eLote> lista, Panel panel, string titulo)
	{
		dgv.Columns.Clear();
		dgv.AutoGenerateColumns = true;
		dgv.DataSource = lista;
		dgv.DataBindingComplete += delegate
		{
			if (dgv.Columns.Contains("dsProyecto"))
			{
				dgv.Columns["dsProyecto"].HeaderText = "Proyecto";
				dgv.Columns["dsProyecto"].DisplayIndex = 1;
			}
			if (dgv.Columns.Contains("dsNombreLote"))
			{
				dgv.Columns["dsNombreLote"].HeaderText = "Lote";
				dgv.Columns["dsNombreLote"].DisplayIndex = 2;
			}
			if (dgv.Columns.Contains("nuCantidadArchivos"))
			{
				dgv.Columns["nuCantidadArchivos"].HeaderText = "Cantidad Archivos";
				dgv.Columns["nuCantidadArchivos"].DisplayIndex = 3;
			}
			if (dgv.Columns.Contains("feAlta"))
			{
				dgv.Columns["feAlta"].HeaderText = "Fecha de Alta";
				dgv.Columns["feAlta"].DisplayIndex = 4;
			}
			foreach (DataGridViewColumn dataGridViewColumn in dgv.Columns)
			{
				if (!new string[5] { "chkSeleccionado", "dsProyecto", "dsNombreLote", "nuCantidadArchivos", "feAlta" }.Contains(dataGridViewColumn.Name))
				{
					dataGridViewColumn.Visible = false;
				}
			}
		};
		DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
		chk.HeaderText = "";
		chk.Name = "chkSeleccionado";
		chk.Width = 30;
		chk.ReadOnly = false;
		dgv.Columns.Insert(0, chk);
		dgv.ReadOnly = false;
		foreach (DataGridViewColumn col in dgv.Columns)
		{
			if (col.Name != "chkSeleccionado")
			{
				col.ReadOnly = true;
			}
		}
		dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		dgv.Dock = DockStyle.Fill;
		Label labelTitulo = new Label();
		labelTitulo.Text = titulo;
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Lotes: {lista.Count}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		panel.Controls.Clear();
		panel.Controls.Add(dgv);
		panel.Controls.Add(labelTitulo);
		panel.Controls.Add(labelTotal);
	}

	private void actualizarLotesSeleccionados()
	{
		dgvLotesSeleccionados.Columns.Clear();
		dgvLotesSeleccionados.DataSource = new BindingList<eLote>();
		DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
		chk.HeaderText = "";
		chk.Name = "chkSeleccionado";
		chk.Width = 30;
		chk.ReadOnly = false;
		dgvLotesSeleccionados.Columns.Insert(0, chk);
		dgvLotesSeleccionados.ReadOnly = false;
		foreach (DataGridViewColumn col in dgvLotesSeleccionados.Columns)
		{
			if (col.Name != "chkSeleccionado")
			{
				col.ReadOnly = true;
			}
		}
		dgvLotesSeleccionados.Columns["dsProyecto"].HeaderText = "Proyecto";
		dgvLotesSeleccionados.Columns["dsNombreLote"].HeaderText = "Lote";
		dgvLotesSeleccionados.Columns["nuCantidadArchivos"].HeaderText = "Cantidad Archivos";
		dgvLotesSeleccionados.Columns["feAlta"].HeaderText = "Fecha de Alta";
		dgvLotesSeleccionados.Columns["dsProyecto"].DisplayIndex = 1;
		dgvLotesSeleccionados.Columns["dsNombreLote"].DisplayIndex = 2;
		dgvLotesSeleccionados.Columns["nuCantidadArchivos"].DisplayIndex = 3;
		dgvLotesSeleccionados.Columns["feAlta"].DisplayIndex = 4;
		foreach (DataGridViewColumn col2 in dgvLotesSeleccionados.Columns)
		{
			if (col2.Name != "chkSeleccionado" && col2.Name != "dsProyecto" && col2.Name != "dsNombreLote" && col2.Name != "nuCantidadArchivos" && col2.Name != "feAlta")
			{
				col2.Visible = false;
			}
		}
		dgvLotesSeleccionados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		pnlLotesSeleccionados.Controls.Clear();
		Label labelTitulo = new Label
		{
			Text = "Lotes Seleccionados",
			Font = new Font("Segoe UI", 12f, FontStyle.Bold),
			BackColor = Color.FromArgb(26, 32, 40),
			ForeColor = Color.White,
			Dock = DockStyle.Top,
			Height = 30,
			TextAlign = ContentAlignment.MiddleCenter
		};
		Label labelTotal = new Label
		{
			Text = $"Total de Lotes: {dgvLotesSeleccionados.Rows.Count}",
			Font = new Font("Segoe UI", 8f),
			Dock = DockStyle.Bottom,
			Height = 25,
			BackColor = Color.FromArgb(26, 32, 40),
			ForeColor = Color.White,
			TextAlign = ContentAlignment.MiddleCenter
		};
		dgvLotesSeleccionados.Dock = DockStyle.Fill;
		pnlLotesSeleccionados.Controls.Add(dgvLotesSeleccionados);
		pnlLotesSeleccionados.Controls.Add(labelTitulo);
		pnlLotesSeleccionados.Controls.Add(labelTotal);
	}

	private void actualizarLotesDisponibles()
	{
		dgvLotesDisponibles.Columns.Clear();
		List<eLote> listaDesdeBD = nLotes.obtenerLotesDisponibleSeparacion(oUsuarioLogueado, 6);
		listaDisponibles = new BindingList<eLote>(listaDesdeBD);
		dgvLotesDisponibles.DataSource = listaDisponibles;
		DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
		chk.HeaderText = "";
		chk.Name = "chkSeleccionado";
		chk.Width = 30;
		chk.ReadOnly = false;
		dgvLotesDisponibles.Columns.Insert(0, chk);
		dgvLotesDisponibles.ReadOnly = false;
		foreach (DataGridViewColumn col in dgvLotesDisponibles.Columns)
		{
			if (col.Name != "chkSeleccionado")
			{
				col.ReadOnly = true;
			}
		}
		dgvLotesDisponibles.Columns["dsProyecto"].HeaderText = "Proyecto";
		dgvLotesDisponibles.Columns["dsNombreLote"].HeaderText = "Lote";
		dgvLotesDisponibles.Columns["nuCantidadArchivos"].HeaderText = "Cantidad Archivos";
		dgvLotesDisponibles.Columns["feAlta"].HeaderText = "Fecha de Alta";
		dgvLotesDisponibles.Columns["dsProyecto"].DisplayIndex = 1;
		dgvLotesDisponibles.Columns["dsNombreLote"].DisplayIndex = 2;
		dgvLotesDisponibles.Columns["nuCantidadArchivos"].DisplayIndex = 3;
		dgvLotesDisponibles.Columns["feAlta"].DisplayIndex = 4;
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

	private void btnSeleccionarTodo_Click(object sender, EventArgs e)
	{
		foreach (DataGridViewRow row in (IEnumerable)dgvLotesDisponibles.Rows)
		{
			row.Cells["chkSeleccionado"].Value = true;
		}
	}

	private void btnDeseleccionarTodo_Click(object sender, EventArgs e)
	{
		foreach (DataGridViewRow row in (IEnumerable)dgvLotesDisponibles.Rows)
		{
			row.Cells["chkSeleccionado"].Value = false;
		}
	}

	private void btnProcesarIndices_Click(object sender, EventArgs e)
	{
		ajustarFormulario_BuscarSeparadores();
		btnSeleccionarTodo.Enabled = false;
		btnSeleccionarTodo.BackColor = Color.DarkGray;
		btnDeseleccionarTodo.Enabled = false;
		btnDeseleccionarTodo.BackColor = Color.DarkGray;
		btnAgregarLotes.Enabled = false;
		btnAgregarLotes.BackColor = Color.DarkGray;
		btnQuitarLotes.Enabled = false;
		btnQuitarLotes.BackColor = Color.DarkGray;
		DataTable oTablaIndice = new DataTable();
		oTablaIndice.Columns.Add("Nombre Lote", typeof(string));
		oTablaIndice.Columns.Add("Nombre Archivo", typeof(string));
		oTablaIndice.Columns.Add("Ruta Archivo", typeof(string));
		foreach (DataGridViewRow row in (IEnumerable)dgvLotesSeleccionados.Rows)
		{
			string rutaLote = row.Cells["dsRutaLote"].Value.ToString();
			nombreLote = row.Cells["dsNombreLote"].Value.ToString();
			DataTable tablaTemporal = nArchivos.buscarArchivosPDF(nombreLote, rutaLote);
			foreach (DataRow fila in tablaTemporal.Rows)
			{
				oTablaIndice.ImportRow(fila);
			}
		}
		dgvIndicesEncontrados.DataSource = oTablaIndice;
		dgvIndicesEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		cantidadTotalDespachos = dgvIndicesEncontrados.RowCount;
		dgvTotalLotesEncontrados.DataSource = cargarTablaTotales();
		dgvTotalLotesEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		buscarSeparadores();
		btnProcesarIndices.Enabled = false;
		btnProcesarIndices.BackColor = Color.DarkGray;
	}

	private async void buscarSeparadores()
	{
		_ = (DataTable)dgvIndicesEncontrados.DataSource;
		dgvSeparadoresEncontrados.AutoGenerateColumns = false;
		dgvSeparadoresEncontrados.Columns.Clear();
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "NombreLote",
			HeaderText = "Lote",
			DataPropertyName = "NombreLote"
		});
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "Despacho",
			HeaderText = "Despacho",
			DataPropertyName = "Despacho"
		});
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "NumeroPagina",
			HeaderText = "Página",
			DataPropertyName = "NumeroPagina"
		});
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "ValorEncontrado",
			HeaderText = "Código",
			DataPropertyName = "ValorEncontrado"
		});
		progressBar1.Visible = true;
		await buscarCodigosBarraAsync();
		dgvSeparadoresEncontrados.DataSource = listaCodigoBarraFinal;
		dgvSeparadoresEncontrados.ReadOnly = false;
		foreach (DataGridViewColumn col in dgvSeparadoresEncontrados.Columns)
		{
			if (!new string[2] { "NumeroPagina", "ValorEncontrado" }.Contains(col.Name))
			{
				col.ReadOnly = true;
			}
		}
		dgvSeparadoresEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		dgvSeparadoresEncontrados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		Label labelTitulo = new Label
		{
			Text = "Listado de Separadores Encontrados",
			Font = new Font("Segoe UI", 12f, FontStyle.Bold),
			BackColor = Color.FromArgb(26, 32, 40),
			ForeColor = Color.White,
			Dock = DockStyle.Top,
			Height = 30,
			TextAlign = ContentAlignment.MiddleCenter
		};
		int totalRegistros = dgvSeparadoresEncontrados.Rows.Count;
		Label labelTotal = new Label
		{
			Text = $"Total de Seperadores: {totalRegistros}",
			Font = new Font("Segoe UI", 8f, FontStyle.Regular),
			Dock = DockStyle.Bottom,
			Height = 25,
			TextAlign = ContentAlignment.MiddleCenter,
			BackColor = Color.FromArgb(26, 32, 40),
			ForeColor = Color.White
		};
		dgvSeparadoresEncontrados.Dock = DockStyle.Fill;
		pnlSeparadoresEncontrados.Controls.Clear();
		pnlSeparadoresEncontrados.Controls.Add(dgvSeparadoresEncontrados);
		pnlSeparadoresEncontrados.Controls.Add(labelTitulo);
		pnlSeparadoresEncontrados.Controls.Add(labelTotal);
		OrdenarDgvSeparadores();
		LoadPdfFiles(rutaLoteOrigen);
		lboxListaArchivos.Visible = true;
		ajustarFormulario_Procesar();
	}

	private void ajustarFormulario_SeleccionarLote()
	{
		base.MaximizeBox = false;
		base.Size = new Size(1550, 270);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_BuscarSeparadores()
	{
		base.Size = new Size(1550, 317);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_Procesar()
	{
		base.MaximizeBox = true;
		base.Size = new Size(1700, 950);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void dgv_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
	{
		if (dgvSeparadoresEncontrados.Columns[e.ColumnIndex].Name == "ValorEncontrado")
		{
			string valorIngresado = e.FormattedValue.ToString();
		}
	}

	private DataTable cargarTablaTotales()
	{
		DataTable dtOrigen = (DataTable)dgvIndicesEncontrados.DataSource;
		DataTable dtAgrupado = new DataTable();
		dtAgrupado.Columns.Add("Nombre Lote", typeof(string));
		dtAgrupado.Columns.Add("Cantidad Despacho", typeof(int));
		var consulta = from row in dtOrigen.AsEnumerable()
			group row by row.Field<string>("Nombre Lote") into grupo
			select new
			{
				NombreLote = grupo.Key,
				CantidadDespacho = grupo.Count()
			};
		foreach (var item in consulta)
		{
			dtAgrupado.Rows.Add(item.NombreLote, item.CantidadDespacho);
		}
		return dtAgrupado;
	}

	private async void btnBuscarSeparadores_Click_1(object sender, EventArgs e)
	{
		_ = (DataTable)dgvIndicesEncontrados.DataSource;
		dgvSeparadoresEncontrados.AutoGenerateColumns = false;
		dgvSeparadoresEncontrados.Columns.Clear();
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "NombreLote",
			HeaderText = "Lote",
			DataPropertyName = "NombreLote"
		});
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "Archivo",
			HeaderText = "Despacho",
			DataPropertyName = "Despacho"
		});
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "NumeroPagina",
			HeaderText = "Página",
			DataPropertyName = "NumeroPagina"
		});
		dgvSeparadoresEncontrados.Columns.Add(new DataGridViewTextBoxColumn
		{
			Name = "ValorEncontrado",
			HeaderText = "Código",
			DataPropertyName = "ValorEncontrado"
		});
		progressBar1.Visible = true;
		try
		{
			await buscarCodigosBarraAsync();
		}
		catch (Exception ex)
		{
			Exception Ex = ex;
			MessageBox.Show("Error: " + Ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		dgvSeparadoresEncontrados.DataSource = listaCodigoBarraFinal;
		dgvSeparadoresEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		dgvSeparadoresEncontrados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		OrdenarDgvSeparadores();
		LoadPdfFiles(rutaLoteOrigen);
		ajustarFormulario_Procesar();
	}

	private async Task buscarCodigosBarraAsync()
	{
		int cantidadArchivos = dgvIndicesEncontrados.Rows.Count;
		progressBar1.Maximum = cantidadArchivos + 1;
		progressBar1.Value = 1;
		int maxGradoParalelismo = 4;
		SemaphoreSlim semaphore = new SemaphoreSlim(maxGradoParalelismo);
		try
		{
			List<Task> tareas = new List<Task>();
			foreach (DataGridViewRow row in (IEnumerable)dgvIndicesEncontrados.Rows)
			{
				await semaphore.WaitAsync();
				tareas.Add(Task.Run(async delegate
				{
					try
					{
						List<eCodigosBarrasEncontrados_v3> resultado = await nCodigoBarras_Muni_v2.buscarCodigoBarrasPATCHAsync(pNombreLote: row.Cells[0].Value.ToString(), pNombreArchivo: row.Cells[1].Value.ToString(), pRutaArchivoPDF: row.Cells[2].Value.ToString(), pUsuarioLogueado: oUsuarioLogueado);
						lock (listaCodigoBarraFinal)
						{
							listaCodigoBarraFinal.AddRange(resultado);
						}
						Invoke((Action)delegate
						{
							progressBar1.Value++;
						});
					}
					finally
					{
						semaphore.Release();
					}
				}));
			}
			await Task.WhenAll(tareas);
		}
		finally
		{
			if (semaphore != null)
			{
				((IDisposable)semaphore).Dispose();
			}
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	private void LoadPdfFiles(string folderPath)
	{
		foreach (DataGridViewRow row in (IEnumerable)dgvLotesSeleccionados.Rows)
		{
			string rutaLoteBuscar = row.Cells["dsRutaLote"].Value.ToString();
			pdfFiles.AddRange(Directory.GetFiles(rutaLoteBuscar, "*.pdf", SearchOption.AllDirectories).ToList());
		}
		lboxListaArchivos.Items.Clear();
		ListBox.ObjectCollection items = lboxListaArchivos.Items;
		object[] items2 = pdfFiles.Select(Path.GetFileName).ToArray();
		items.AddRange(items2);
		listaArchivos.AddRange(pdfFiles.Select(Path.GetFileName).ToArray());
		if (pdfFiles.Count > 0)
		{
			currentFileIndex = 0;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
		else
		{
			MessageBox.Show("La carpeta seleccionada no tiene archivos PDF", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void LoadPdf(string filePath)
	{
		if (isDisposing)
		{
			return;
		}
		LimpiarVisorCompletamente();
		if (pdfDocument != null)
		{
			try
			{
				pdfDocument.Dispose();
			}
			catch
			{
			}
			finally
			{
				pdfDocument = null;
			}
		}
		GC.Collect(2, GCCollectionMode.Forced);
		GC.WaitForPendingFinalizers();
		Thread.Sleep(100);
		try
		{
			pdfDocument = PdfDocument.Load(filePath);
			currentPage = 0;
			hasChanges = false;
			txtArchivoActual.Text = Path.GetFileName(filePath);
			txtTotalPaginas.Text = pdfDocument.PageCount.ToString();
			UpdateViewer();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error al cargar PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void ReloadPdf()
	{
		if (pdfDocument != null)
		{
			LoadPdf(pdfFiles[currentFileIndex]);
		}
	}

	private void UpdateViewer()
	{
		if (isDisposing)
		{
			return;
		}
		pagesPerView = 12;
		LimpiarVisorCompletamente();
		int columns;
		int rows;
		if (pagesPerView == 10)
		{
			columns = 5;
			rows = 2;
		}
		else if (pagesPerView == 12)
		{
			columns = 6;
			rows = 2;
		}
		else
		{
			columns = ((pagesPerView >= 4) ? (pagesPerView / 2) : pagesPerView);
			rows = ((pagesPerView < 4) ? 1 : 2);
		}
		string archivoActual = Path.GetFileNameWithoutExtension(txtArchivoActual.Text);
		for (int i = 0; i < pagesPerView; i++)
		{
			int pageIndex = currentPage + i;
			if (pageIndex >= pdfDocument.PageCount)
			{
				break;
			}
			PictureBox pictureBox = new PictureBox
			{
				Width = pnlVisor.Width / columns,
				Height = pnlVisor.Height / rows - 40,
				Dock = DockStyle.None,
				SizeMode = PictureBoxSizeMode.Zoom,
				BorderStyle = BorderStyle.FixedSingle,
				Tag = pageIndex + 1
			};
			using (Image bitmap = pdfDocument.Render(pageIndex, 1f, 1f, forPrinting: true))
			{
				pictureBox.Image = new Bitmap(bitmap);
			}
			Label lblPagina = new Label
			{
				Text = $"Página {pageIndex + 1}",
				TextAlign = ContentAlignment.MiddleCenter,
				Dock = DockStyle.Bottom,
				Height = 20,
				BackColor = Color.LightGray
			};
			CheckBox chkSeleccionar = new CheckBox
			{
				Dock = DockStyle.Top,
				Height = 20,
				TextAlign = ContentAlignment.TopCenter,
				Tag = pageIndex + 1
			};
			bool estaSeleccionado = false;
			foreach (DataGridViewRow row in (IEnumerable)dgvSeparadoresEncontrados.Rows)
			{
				if (row.Cells[1].Value?.ToString() == archivoActual + ".pdf" && int.TryParse(row.Cells[2].Value?.ToString(), out var paginaDgv) && paginaDgv == pageIndex + 1)
				{
					chkSeleccionar.Checked = true;
					estaSeleccionado = true;
					break;
				}
			}
			if (estaSeleccionado)
			{
				pictureBox.BackColor = Color.FromArgb(100, 0, 120, 215);
			}
			PictureBox localPictureBox = pictureBox;
			int localPageIndex = pageIndex;
			EventHandler checkedChangedHandler = delegate
			{
				if (chkSeleccionar.Checked)
				{
					localPictureBox.BackColor = Color.FromArgb(100, 0, 120, 215);
				}
				else
				{
					localPictureBox.BackColor = Color.Transparent;
				}
			};
			chkSeleccionar.CheckedChanged += checkedChangedHandler;
			chkSeleccionar.Tag = checkedChangedHandler;
			Panel panel = new Panel
			{
				Width = pictureBox.Width,
				Height = pictureBox.Height + lblPagina.Height + chkSeleccionar.Height,
				BorderStyle = BorderStyle.None
			};
			panel.Controls.Add(chkSeleccionar);
			panel.Controls.Add(pictureBox);
			panel.Controls.Add(lblPagina);
			pnlVisor.Controls.Add(panel);
			pictureBox.MouseClick += PdfViewer_MouseClick;
			panel.Left = i % columns * (pnlVisor.Width / columns);
			panel.Top = i / columns * (pnlVisor.Height / rows);
		}
		GC.Collect(1, GCCollectionMode.Optimized);
	}

	private void btnPaginaAnterior_Click_1(object sender, EventArgs e)
	{
		if (currentPage - pagesPerView >= 0)
		{
			currentPage -= pagesPerView;
			UpdateViewer();
		}
		else
		{
			btnArchivoAnterior_Click_1(sender, e);
		}
	}

	private void btnPaginaSiguiente_Click_1(object sender, EventArgs e)
	{
		if (currentPage + pagesPerView < pdfDocument.PageCount)
		{
			currentPage += pagesPerView;
			UpdateViewer();
		}
		else if (currentFileIndex + 1 < pdfFiles.Count)
		{
			currentFileIndex++;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
		else
		{
			MessageBox.Show("Termino de Controlar la Carpeta", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}

	private void btnArchivoAnterior_Click_1(object sender, EventArgs e)
	{
		if (currentFileIndex > 0)
		{
			currentFileIndex--;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
	}

	private void btnArchivoSiguiente_Click_1(object sender, EventArgs e)
	{
		if (currentFileIndex < pdfFiles.Count - 1)
		{
			currentFileIndex++;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
	}

	private void PdfViewer_MouseClick(object sender, MouseEventArgs e)
	{
		PictureBox pictureBox;
		PictureBox zoomPictureBox;
		float zoomFactor;
		if (txtSeleccion.Text == "ZOOM")
		{
			pictureBox = sender as PictureBox;
			if (pictureBox != null)
			{
				int pageIndex = currentPage + pnlVisor.Controls.IndexOf(pictureBox.Parent);
				if (pageIndex >= pdfDocument.PageCount)
				{
					return;
				}
				Form zoomForm = new Form
				{
					Text = "Vista ampliada",
					Size = new Size(800, 600),
					StartPosition = FormStartPosition.CenterScreen
				};
				zoomPictureBox = new PictureBox
				{
					Image = new Bitmap(pictureBox.Image),
					SizeMode = PictureBoxSizeMode.StretchImage,
					Width = pictureBox.Image.Width,
					Height = pictureBox.Image.Height
				};
				Panel zoomPanel = new Panel
				{
					Dock = DockStyle.Fill,
					AutoScroll = true
				};
				zoomPanel.Controls.Add(zoomPictureBox);
				zoomFactor = 1f;
				zoomPictureBox.MouseWheel += delegate(object s, MouseEventArgs ev)
				{
					if (ev.Delta > 0)
					{
						AplicarZoom(1.1f);
					}
					else
					{
						AplicarZoom(0.9090909f);
					}
				};
				zoomPictureBox.Focus();
				zoomForm.MouseWheel += delegate
				{
					zoomPictureBox.Focus();
				};
				Button btnZoomIn = new Button
				{
					Text = "+",
					Dock = DockStyle.Top,
					Height = 40
				};
				btnZoomIn.Click += delegate
				{
					AplicarZoom(1.1f);
				};
				Button btnZoomOut = new Button
				{
					Text = "-",
					Dock = DockStyle.Top,
					Height = 40
				};
				btnZoomOut.Click += delegate
				{
					AplicarZoom(0.9090909f);
				};
				Panel panelBotones = new Panel
				{
					Dock = DockStyle.Right,
					Width = 50
				};
				panelBotones.Controls.Add(btnZoomOut);
				panelBotones.Controls.Add(btnZoomIn);
				zoomForm.Controls.Add(panelBotones);
				zoomForm.Controls.Add(zoomPanel);
				zoomForm.Shown += delegate
				{
					zoomPanel.Focus();
				};
				zoomForm.Show();
			}
		}
		if (txtSeleccion.Text == "AGREGAR" && sender is PictureBox pictureBox2)
		{
			int pageIndex2 = currentPage + pnlVisor.Controls.IndexOf(pictureBox2.Parent);
			if (pageIndex2 < pdfDocument.PageCount)
			{
				pnlAsignarCodigo.Visible = true;
				txtPaginaSeleccionada.Text = Convert.ToString(pageIndex2 + 1);
				txtNuevoCodigo.Focus();
			}
		}
		void AplicarZoom(float factor)
		{
			zoomFactor *= factor;
			zoomPictureBox.Width = (int)((float)pictureBox.Image.Width * zoomFactor);
			zoomPictureBox.Height = (int)((float)pictureBox.Image.Height * zoomFactor);
		}
	}

	private void btnCancelarAsignar_Click_1(object sender, EventArgs e)
	{
		txtNuevoCodigo.Clear();
		pnlAsignarCodigo.Visible = false;
	}

	private void btnAsignarCodigo_Click_1(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtNuevoCodigo.Text))
		{
			MessageBox.Show("Debe indicar el código", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		List<eCodigosBarrasEncontrados_v3> listaActual = dgvSeparadoresEncontrados.DataSource as List<eCodigosBarrasEncontrados_v3>;
		if (listaActual == null)
		{
			listaActual = new List<eCodigosBarrasEncontrados_v3>();
		}
		string rutaLoteAgregar = string.Empty;
		string nombreLote = string.Empty;
		string nombreArchivo = txtArchivoActual.Text;
		foreach (DataGridViewRow fila in (IEnumerable)dgvIndicesEncontrados.Rows)
		{
			if (fila.Cells["Nombre Archivo"].Value != null && fila.Cells["Nombre Archivo"].Value.ToString().Equals(nombreArchivo, StringComparison.OrdinalIgnoreCase))
			{
				nombreLote = fila.Cells["Nombre Lote"].Value?.ToString();
				rutaLoteAgregar = fila.Cells["Ruta Archivo"].Value?.ToString();
				break;
			}
		}
		int pagina;
		int numeroPagina = (int.TryParse(txtPaginaSeleccionada.Text, out pagina) ? pagina : 0);
		string nuevoValorEncontrado = txtNuevoCodigo.Text;
		listaActual.Add(new eCodigosBarrasEncontrados_v3
		{
			NombreLote = nombreLote,
			Despacho = nombreArchivo,
			NumeroPagina = numeroPagina,
			ValorEncontrado = nuevoValorEncontrado
		});
		agregarAlArchivo(rutaLoteAgregar, nombreLote, nombreArchivo, numeroPagina, nuevoValorEncontrado);
		dgvSeparadoresEncontrados.DataSource = null;
		dgvSeparadoresEncontrados.DataSource = listaActual;
		OrdenarDgvSeparadores();
		UpdateViewer();
		txtPaginaSeleccionada.Clear();
		pnlAsignarCodigo.Visible = false;
	}

	private void agregarAlArchivo(string pRutaLote, string pNombreLote, string pNombreArchivo, int pNumeroPagina, string pValorEncontrado)
	{
		try
		{
			string carpeta = Path.GetDirectoryName(pRutaLote);
			string nombreArchivoTxt = pNombreLote + ".txt";
			string rutaCompleta = Path.Combine(pRutaLote, nombreArchivoTxt);
			if (!Directory.Exists(carpeta))
			{
				Directory.CreateDirectory(carpeta);
			}
			string linea = $"{pRutaLote};{pNombreLote};{pNombreArchivo};{pNumeroPagina};{pValorEncontrado}";
			File.AppendAllText(rutaCompleta, linea + Environment.NewLine);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error al guardar en el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void OrdenarDgvSeparadores()
	{
		if (dgvSeparadoresEncontrados.DataSource is List<eCodigosBarrasEncontrados_v3> lista)
		{
			List<eCodigosBarrasEncontrados_v3> listaOrdenada = (from x in lista
				orderby x.NombreLote, x.Despacho, x.NumeroPagina
				select x).ToList();
			dgvSeparadoresEncontrados.DataSource = null;
			dgvSeparadoresEncontrados.DataSource = listaOrdenada;
		}
	}

	private void btnSeleccionarCarpetaSalida_Click_1(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLoteDestino = new FolderBrowserDialog();
		oSeleccionarLoteDestino.Description = "Seleccionar la carpeta destino";
		oSeleccionarLoteDestino.SelectedPath = rutaCarpetaSalida;
		oSeleccionarLoteDestino.ShowNewFolderButton = true;
		if (oSeleccionarLoteDestino.ShowDialog() == DialogResult.OK)
		{
			string rutaLoteDestino = oSeleccionarLoteDestino.SelectedPath;
			txtCarpetaSalida.Text = rutaLoteDestino;
			nConfiguracion.actualizarUltimaCarpetaSalida(oUsuarioLogueado, 1, rutaCarpetaSalida, rutaLoteDestino);
			rutaCarpetaSalida = rutaLoteDestino;
		}
	}

	private void btnProcesar_Click_1(object sender, EventArgs e)
	{
		try
		{
			procesarTodo();
			MessageBox.Show("El proceso se terminó correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			vaciarFormulario();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void procesarTodo()
	{
		List<eLote> oListaLotesSeleccionadosProcesar = new List<eLote>();
		foreach (DataGridViewRow row in (IEnumerable)dgvLotesSeleccionados.Rows)
		{
			int cdLote = Convert.ToInt32(row.Cells["cdLote"].Value.ToString());
			eLote oLoteProcesar = new eLote();
			oLoteProcesar.cdProyecto = Convert.ToInt32(row.Cells["cdProyecto"].Value.ToString());
			oLoteProcesar.cdLote = Convert.ToInt32(row.Cells["cdLote"].Value.ToString());
			oLoteProcesar.dsNombreLote = row.Cells["dsNombreLote"].Value.ToString();
			oLoteProcesar.cdEstado = Convert.ToInt32(row.Cells["cdEstado"].Value.ToString());
			oLoteProcesar.dsRutaLote = row.Cells["dsRutaLote"].Value.ToString();
			oLoteProcesar.dsRutaLoteFinal = txtCarpetaSalida.Text;
			oListaLotesSeleccionadosProcesar.Add(oLoteProcesar);
		}
		List<eCodigosBarrasEncontrados_v3> listaSeperadores = dgvSeparadoresEncontrados.DataSource as List<eCodigosBarrasEncontrados_v3>;
		nLotes.separarArchivosLoteCatastro(oUsuarioLogueado, oListaLotesSeleccionadosProcesar, listaSeperadores);
	}

	private void procesarDespacho(string pNombreLote, string pNombreArchivo, string pRutaArchivoPDF, List<eCodigosBarrasEncontrados_v3> pListaSeparadores)
	{
		try
		{
			nCodigoBarras_v5_Muni.ProcesarPDF(oUsuarioLogueado, rutaCarpetaSalida, pListaSeparadores, pNombreLote, pNombreArchivo, pRutaArchivoPDF);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnZoom_Click(object sender, EventArgs e)
	{
		txtSeleccion.Text = "ZOOM";
		btnZoom.BackColor = Color.SteelBlue;
		btnAgregar.BackColor = Color.SeaGreen;
	}

	private void btnModificar_Click(object sender, EventArgs e)
	{
		txtSeleccion.Text = "AGREGAR";
		btnZoom.BackColor = Color.SeaGreen;
		btnAgregar.BackColor = Color.SteelBlue;
	}

	private void vaciarFormulario()
	{
		base.WindowState = FormWindowState.Normal;
		LimpiarTodoAntesDeCerrar();
		txtSeleccion.Text = "";
		btnZoom.BackColor = Color.SeaGreen;
		btnAgregar.BackColor = Color.SeaGreen;
		btnSeleccionarTodo.Enabled = true;
		btnSeleccionarTodo.BackColor = Color.SeaGreen;
		btnDeseleccionarTodo.Enabled = true;
		btnDeseleccionarTodo.BackColor = Color.SeaGreen;
		btnAgregarLotes.Enabled = true;
		btnAgregarLotes.BackColor = Color.SeaGreen;
		btnQuitarLotes.Enabled = true;
		btnQuitarLotes.BackColor = Color.SeaGreen;
		rutaLoteOrigen = string.Empty;
		progressBar1.Value = 0;
		progressBar1.Maximum = 0;
		progressBar1.Visible = false;
		lboxListaArchivos.Visible = false;
		listaDisponibles = new BindingList<eLote>();
		listaSeleccionados = new BindingList<eLote>();
		actualizarLotesDisponibles();
		actualizarLotesSeleccionados();
		ajustarFormulario_SeleccionarLote();
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		vaciarFormulario();
	}

	private void frmDespachoSeparar_v2_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Up:
			btnArchivoAnterior_Click_1(sender, e);
			break;
		case Keys.Down:
			btnArchivoSiguiente_Click_1(sender, e);
			break;
		case Keys.Left:
			btnPaginaAnterior_Click_1(sender, e);
			break;
		case Keys.Right:
			btnPaginaSiguiente_Click_1(sender, e);
			break;
		}
	}

	private void pnlAsignarCodigo_Paint(object sender, PaintEventArgs e)
	{
	}

	private void frmMunicipalidadVillaReginaCatastro_FormClosing(object sender, FormClosingEventArgs e)
	{
		isDisposing = true;
		if (timerLimpiezaMemoria != null)
		{
			timerLimpiezaMemoria.Stop();
			timerLimpiezaMemoria.Dispose();
			timerLimpiezaMemoria = null;
		}
		LimpiarTodoAntesDeCerrar();
	}

	private void LimpiarTodoAntesDeCerrar()
	{
		try
		{
			LimpiarVisorCompletamente();
			if (pdfDocument != null)
			{
				try
				{
					pdfDocument.Dispose();
				}
				catch
				{
				}
				finally
				{
					pdfDocument = null;
				}
			}
			dgvTotalLotesEncontrados.DataSource = null;
			dgvTotalLotesEncontrados.Columns.Clear();
			dgvTotalLotesEncontrados.Rows.Clear();
			dgvIndicesEncontrados.DataSource = null;
			dgvIndicesEncontrados.Columns.Clear();
			dgvIndicesEncontrados.Rows.Clear();
			dgvSeparadoresEncontrados.DataSource = null;
			dgvSeparadoresEncontrados.Columns.Clear();
			dgvSeparadoresEncontrados.Rows.Clear();
			dgvLotesDisponibles.DataSource = null;
			dgvLotesDisponibles.Columns.Clear();
			dgvLotesDisponibles.Rows.Clear();
			dgvLotesSeleccionados.DataSource = null;
			dgvLotesSeleccionados.Columns.Clear();
			dgvLotesSeleccionados.Rows.Clear();
			listaCodigoBarraFinal?.Clear();
			pdfFiles?.Clear();
			listaArchivos?.Clear();
			listaDisponibles?.Clear();
			listaSeleccionados?.Clear();
			lboxListaArchivos.Items.Clear();
			for (int i = 0; i < 3; i++)
			{
				GC.Collect(2, GCCollectionMode.Forced);
				GC.WaitForPendingFinalizers();
				Thread.Sleep(100);
			}
		}
		catch (Exception)
		{
		}
	}

	private void LimpiarVisorCompletamente()
	{
		if (pnlVisor == null || pnlVisor.IsDisposed)
		{
			return;
		}
		pnlVisor.SuspendLayout();
		try
		{
			List<Control> controlesALiberar = new List<Control>();
			foreach (Control control in pnlVisor.Controls)
			{
				controlesALiberar.Add(control);
			}
			foreach (Control control2 in controlesALiberar)
			{
				if (!(control2 is Panel panel))
				{
					continue;
				}
				List<Control> childControls = new List<Control>();
				foreach (Control child in panel.Controls)
				{
					childControls.Add(child);
				}
				foreach (Control childControl in childControls)
				{
					if (childControl is PictureBox pictureBox)
					{
						pictureBox.MouseClick -= PdfViewer_MouseClick;
						pictureBox.AllowDrop = false;
						if (pictureBox.Image != null)
						{
							Image img = pictureBox.Image;
							pictureBox.Image = null;
							img.Dispose();
						}
					}
					else if (childControl is CheckBox checkBox)
					{
						RemoverEventHandlersCheckBox(checkBox);
					}
					if (!childControl.IsDisposed)
					{
						childControl.Dispose();
					}
				}
				panel.Controls.Clear();
				if (!panel.IsDisposed)
				{
					panel.Dispose();
				}
			}
			pnlVisor.Controls.Clear();
		}
		catch (Exception)
		{
		}
		finally
		{
			if (!pnlVisor.IsDisposed)
			{
				pnlVisor.ResumeLayout();
			}
		}
		GC.Collect(2, GCCollectionMode.Forced);
		GC.WaitForPendingFinalizers();
	}

	private void RemoverEventHandlersCheckBox(CheckBox checkBox)
	{
		FieldInfo eventField = typeof(CheckBox).GetField("EVENT_CHECKEDCHANGED", BindingFlags.Static | BindingFlags.NonPublic);
		if (eventField != null)
		{
			object eventKey = eventField.GetValue(null);
			PropertyInfo eventsProperty = typeof(Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
			if (eventsProperty != null && eventKey != null)
			{
				EventHandlerList events = (EventHandlerList)eventsProperty.GetValue(checkBox);
				events?.RemoveHandler(eventKey, events[eventKey]);
			}
		}
	}

	private void InicializarTimerLimpieza()
	{
		timerLimpiezaMemoria = new System.Windows.Forms.Timer();
		timerLimpiezaMemoria.Interval = 15000;
		timerLimpiezaMemoria.Tick += delegate
		{
			if (!progressBar1.Visible && !isDisposing)
			{
				GC.Collect(1, GCCollectionMode.Optimized);
				if (DateTime.Now.Second % 60 == 0)
				{
					GC.Collect(2, GCCollectionMode.Forced);
					GC.WaitForPendingFinalizers();
				}
			}
		};
		timerLimpiezaMemoria.Start();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._04_Separador.frmMunicipalidadVillaReginaCatastro));
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
		this.pnlVisor = new System.Windows.Forms.Panel();
		this.gbBotonesNavegacion = new System.Windows.Forms.GroupBox();
		this.btnAgregar = new System.Windows.Forms.Button();
		this.btnZoom = new System.Windows.Forms.Button();
		this.btnArchivoSiguiente = new System.Windows.Forms.Button();
		this.btnArchivoAnterior = new System.Windows.Forms.Button();
		this.btnPaginaSiguiente = new System.Windows.Forms.Button();
		this.btnPaginaAnterior = new System.Windows.Forms.Button();
		this.label5 = new System.Windows.Forms.Label();
		this.txtTotalPaginas = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.txtArchivoActual = new System.Windows.Forms.TextBox();
		this.lboxListaArchivos = new System.Windows.Forms.ListBox();
		this.pnlAsignarCodigo = new System.Windows.Forms.Panel();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.btnAsignarCodigo = new System.Windows.Forms.Button();
		this.btnCancelarAsignar = new System.Windows.Forms.Button();
		this.label4 = new System.Windows.Forms.Label();
		this.txtPaginaSeleccionada = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.txtNuevoCodigo = new System.Windows.Forms.TextBox();
		this.gbSeleccionarCarpetaSalida = new System.Windows.Forms.GroupBox();
		this.btnProcesar = new System.Windows.Forms.Button();
		this.btnSeleccionarCarpetaSalida = new System.Windows.Forms.Button();
		this.txtCarpetaSalida = new System.Windows.Forms.TextBox();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.dgvTotalLotesEncontrados = new System.Windows.Forms.DataGridView();
		this.dgvIndicesEncontrados = new System.Windows.Forms.DataGridView();
		this.dgvSeparadoresEncontrados = new System.Windows.Forms.DataGridView();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.txtSeleccion = new System.Windows.Forms.TextBox();
		this.pnlLotesDisponibles = new System.Windows.Forms.Panel();
		this.dgvLotesDisponibles = new System.Windows.Forms.DataGridView();
		this.btnSeleccionarTodo = new System.Windows.Forms.Button();
		this.btnDeseleccionarTodo = new System.Windows.Forms.Button();
		this.btnAgregarLotes = new System.Windows.Forms.Button();
		this.btnQuitarLotes = new System.Windows.Forms.Button();
		this.pnlLotesSeleccionados = new System.Windows.Forms.Panel();
		this.dgvLotesSeleccionados = new System.Windows.Forms.DataGridView();
		this.btnProcesarIndices = new System.Windows.Forms.Button();
		this.pnlSeparadoresEncontrados = new System.Windows.Forms.Panel();
		this.gbBotonesNavegacion.SuspendLayout();
		this.pnlAsignarCodigo.SuspendLayout();
		this.groupBox1.SuspendLayout();
		this.gbSeleccionarCarpetaSalida.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesEncontrados).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dgvIndicesEncontrados).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dgvSeparadoresEncontrados).BeginInit();
		this.pnlLotesDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).BeginInit();
		this.pnlLotesSeleccionados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesSeleccionados).BeginInit();
		this.pnlSeparadoresEncontrados.SuspendLayout();
		base.SuspendLayout();
		this.pnlVisor.Location = new System.Drawing.Point(379, 343);
		this.pnlVisor.Name = "pnlVisor";
		this.pnlVisor.Size = new System.Drawing.Size(1292, 483);
		this.pnlVisor.TabIndex = 5;
		this.gbBotonesNavegacion.Controls.Add(this.btnAgregar);
		this.gbBotonesNavegacion.Controls.Add(this.btnZoom);
		this.gbBotonesNavegacion.Controls.Add(this.btnArchivoSiguiente);
		this.gbBotonesNavegacion.Controls.Add(this.btnArchivoAnterior);
		this.gbBotonesNavegacion.Controls.Add(this.btnPaginaSiguiente);
		this.gbBotonesNavegacion.Controls.Add(this.btnPaginaAnterior);
		this.gbBotonesNavegacion.Controls.Add(this.label5);
		this.gbBotonesNavegacion.Controls.Add(this.txtTotalPaginas);
		this.gbBotonesNavegacion.Controls.Add(this.label1);
		this.gbBotonesNavegacion.Controls.Add(this.txtArchivoActual);
		this.gbBotonesNavegacion.Location = new System.Drawing.Point(379, 278);
		this.gbBotonesNavegacion.Name = "gbBotonesNavegacion";
		this.gbBotonesNavegacion.Size = new System.Drawing.Size(1292, 56);
		this.gbBotonesNavegacion.TabIndex = 6;
		this.gbBotonesNavegacion.TabStop = false;
		this.btnAgregar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnAgregar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregar.FlatAppearance.BorderSize = 0;
		this.btnAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregar.ForeColor = System.Drawing.Color.White;
		this.btnAgregar.Image = (System.Drawing.Image)resources.GetObject("btnAgregar.Image");
		this.btnAgregar.Location = new System.Drawing.Point(722, 16);
		this.btnAgregar.Name = "btnAgregar";
		this.btnAgregar.Size = new System.Drawing.Size(125, 25);
		this.btnAgregar.TabIndex = 42;
		this.btnAgregar.Text = "   Agregar";
		this.btnAgregar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregar.UseVisualStyleBackColor = false;
		this.btnAgregar.Click += new System.EventHandler(btnModificar_Click);
		this.btnZoom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnZoom.BackColor = System.Drawing.Color.SeaGreen;
		this.btnZoom.FlatAppearance.BorderSize = 0;
		this.btnZoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnZoom.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnZoom.ForeColor = System.Drawing.Color.White;
		this.btnZoom.Image = (System.Drawing.Image)resources.GetObject("btnZoom.Image");
		this.btnZoom.Location = new System.Drawing.Point(591, 16);
		this.btnZoom.Name = "btnZoom";
		this.btnZoom.Size = new System.Drawing.Size(125, 25);
		this.btnZoom.TabIndex = 41;
		this.btnZoom.Text = "   Zoom";
		this.btnZoom.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnZoom.UseVisualStyleBackColor = false;
		this.btnZoom.Click += new System.EventHandler(btnZoom_Click);
		this.btnArchivoSiguiente.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnArchivoSiguiente.BackColor = System.Drawing.Color.SeaGreen;
		this.btnArchivoSiguiente.FlatAppearance.BorderSize = 0;
		this.btnArchivoSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnArchivoSiguiente.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnArchivoSiguiente.ForeColor = System.Drawing.Color.White;
		this.btnArchivoSiguiente.Location = new System.Drawing.Point(435, 13);
		this.btnArchivoSiguiente.Name = "btnArchivoSiguiente";
		this.btnArchivoSiguiente.Size = new System.Drawing.Size(132, 34);
		this.btnArchivoSiguiente.TabIndex = 40;
		this.btnArchivoSiguiente.Text = "Archivo Siguiente";
		this.btnArchivoSiguiente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnArchivoSiguiente.UseVisualStyleBackColor = false;
		this.btnArchivoSiguiente.Click += new System.EventHandler(btnArchivoSiguiente_Click_1);
		this.btnArchivoAnterior.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnArchivoAnterior.BackColor = System.Drawing.Color.SeaGreen;
		this.btnArchivoAnterior.FlatAppearance.BorderSize = 0;
		this.btnArchivoAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnArchivoAnterior.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnArchivoAnterior.ForeColor = System.Drawing.Color.White;
		this.btnArchivoAnterior.Location = new System.Drawing.Point(297, 13);
		this.btnArchivoAnterior.Name = "btnArchivoAnterior";
		this.btnArchivoAnterior.Size = new System.Drawing.Size(132, 34);
		this.btnArchivoAnterior.TabIndex = 39;
		this.btnArchivoAnterior.Text = "Archivo Anterior";
		this.btnArchivoAnterior.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnArchivoAnterior.UseVisualStyleBackColor = false;
		this.btnArchivoAnterior.Click += new System.EventHandler(btnArchivoAnterior_Click_1);
		this.btnPaginaSiguiente.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnPaginaSiguiente.BackColor = System.Drawing.Color.SeaGreen;
		this.btnPaginaSiguiente.FlatAppearance.BorderSize = 0;
		this.btnPaginaSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnPaginaSiguiente.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnPaginaSiguiente.ForeColor = System.Drawing.Color.White;
		this.btnPaginaSiguiente.Location = new System.Drawing.Point(144, 13);
		this.btnPaginaSiguiente.Name = "btnPaginaSiguiente";
		this.btnPaginaSiguiente.Size = new System.Drawing.Size(132, 34);
		this.btnPaginaSiguiente.TabIndex = 38;
		this.btnPaginaSiguiente.Text = "Página Siguiente";
		this.btnPaginaSiguiente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnPaginaSiguiente.UseVisualStyleBackColor = false;
		this.btnPaginaSiguiente.Click += new System.EventHandler(btnPaginaSiguiente_Click_1);
		this.btnPaginaAnterior.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnPaginaAnterior.BackColor = System.Drawing.Color.SeaGreen;
		this.btnPaginaAnterior.FlatAppearance.BorderSize = 0;
		this.btnPaginaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnPaginaAnterior.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnPaginaAnterior.ForeColor = System.Drawing.Color.White;
		this.btnPaginaAnterior.Location = new System.Drawing.Point(6, 13);
		this.btnPaginaAnterior.Name = "btnPaginaAnterior";
		this.btnPaginaAnterior.Size = new System.Drawing.Size(132, 34);
		this.btnPaginaAnterior.TabIndex = 37;
		this.btnPaginaAnterior.Text = "Página Anterior";
		this.btnPaginaAnterior.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnPaginaAnterior.UseVisualStyleBackColor = false;
		this.btnPaginaAnterior.Click += new System.EventHandler(btnPaginaAnterior_Click_1);
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(1125, 22);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(119, 17);
		this.label5.TabIndex = 8;
		this.label5.Text = "Total de Páginas:";
		this.txtTotalPaginas.BackColor = System.Drawing.Color.DarkGray;
		this.txtTotalPaginas.Location = new System.Drawing.Point(1250, 21);
		this.txtTotalPaginas.Name = "txtTotalPaginas";
		this.txtTotalPaginas.Size = new System.Drawing.Size(36, 20);
		this.txtTotalPaginas.TabIndex = 4;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(862, 22);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(107, 17);
		this.label1.TabIndex = 3;
		this.label1.Text = "Archivo Actual:";
		this.txtArchivoActual.BackColor = System.Drawing.Color.DarkGray;
		this.txtArchivoActual.Location = new System.Drawing.Point(975, 21);
		this.txtArchivoActual.Name = "txtArchivoActual";
		this.txtArchivoActual.Size = new System.Drawing.Size(144, 20);
		this.txtArchivoActual.TabIndex = 2;
		this.lboxListaArchivos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lboxListaArchivos.FormattingEnabled = true;
		this.lboxListaArchivos.ItemHeight = 16;
		this.lboxListaArchivos.Location = new System.Drawing.Point(1388, 157);
		this.lboxListaArchivos.Name = "lboxListaArchivos";
		this.lboxListaArchivos.Size = new System.Drawing.Size(281, 116);
		this.lboxListaArchivos.TabIndex = 7;
		this.lboxListaArchivos.Visible = false;
		this.pnlAsignarCodigo.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlAsignarCodigo.Controls.Add(this.groupBox1);
		this.pnlAsignarCodigo.Location = new System.Drawing.Point(379, 278);
		this.pnlAsignarCodigo.Name = "pnlAsignarCodigo";
		this.pnlAsignarCodigo.Size = new System.Drawing.Size(1302, 548);
		this.pnlAsignarCodigo.TabIndex = 8;
		this.pnlAsignarCodigo.Visible = false;
		this.pnlAsignarCodigo.Paint += new System.Windows.Forms.PaintEventHandler(pnlAsignarCodigo_Paint);
		this.groupBox1.Controls.Add(this.btnAsignarCodigo);
		this.groupBox1.Controls.Add(this.btnCancelarAsignar);
		this.groupBox1.Controls.Add(this.label4);
		this.groupBox1.Controls.Add(this.txtPaginaSeleccionada);
		this.groupBox1.Controls.Add(this.label3);
		this.groupBox1.Controls.Add(this.txtNuevoCodigo);
		this.groupBox1.Location = new System.Drawing.Point(472, 179);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(375, 161);
		this.groupBox1.TabIndex = 6;
		this.groupBox1.TabStop = false;
		this.btnAsignarCodigo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnAsignarCodigo.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAsignarCodigo.FlatAppearance.BorderSize = 0;
		this.btnAsignarCodigo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAsignarCodigo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAsignarCodigo.ForeColor = System.Drawing.Color.White;
		this.btnAsignarCodigo.Image = (System.Drawing.Image)resources.GetObject("btnAsignarCodigo.Image");
		this.btnAsignarCodigo.Location = new System.Drawing.Point(20, 118);
		this.btnAsignarCodigo.Name = "btnAsignarCodigo";
		this.btnAsignarCodigo.Size = new System.Drawing.Size(165, 25);
		this.btnAsignarCodigo.TabIndex = 56;
		this.btnAsignarCodigo.Text = "   Asignar";
		this.btnAsignarCodigo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAsignarCodigo.UseVisualStyleBackColor = false;
		this.btnAsignarCodigo.Click += new System.EventHandler(btnAsignarCodigo_Click_1);
		this.btnCancelarAsignar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancelarAsignar.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelarAsignar.FlatAppearance.BorderSize = 0;
		this.btnCancelarAsignar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelarAsignar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelarAsignar.ForeColor = System.Drawing.Color.White;
		this.btnCancelarAsignar.Image = (System.Drawing.Image)resources.GetObject("btnCancelarAsignar.Image");
		this.btnCancelarAsignar.Location = new System.Drawing.Point(224, 118);
		this.btnCancelarAsignar.Name = "btnCancelarAsignar";
		this.btnCancelarAsignar.Size = new System.Drawing.Size(130, 25);
		this.btnCancelarAsignar.TabIndex = 44;
		this.btnCancelarAsignar.Text = "   Cancelar";
		this.btnCancelarAsignar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelarAsignar.UseVisualStyleBackColor = false;
		this.btnCancelarAsignar.Click += new System.EventHandler(btnCancelarAsignar_Click_1);
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.SystemColors.ControlLight;
		this.label4.Location = new System.Drawing.Point(77, 28);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(141, 16);
		this.label4.TabIndex = 4;
		this.label4.Text = "Página Seleccionada:";
		this.txtPaginaSeleccionada.Enabled = false;
		this.txtPaginaSeleccionada.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtPaginaSeleccionada.Location = new System.Drawing.Point(224, 27);
		this.txtPaginaSeleccionada.Name = "txtPaginaSeleccionada";
		this.txtPaginaSeleccionada.Size = new System.Drawing.Size(53, 22);
		this.txtPaginaSeleccionada.TabIndex = 5;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.SystemColors.ControlLight;
		this.label3.Location = new System.Drawing.Point(103, 72);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(63, 20);
		this.label3.TabIndex = 0;
		this.label3.Text = "Código:";
		this.txtNuevoCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNuevoCodigo.Location = new System.Drawing.Point(177, 66);
		this.txtNuevoCodigo.Name = "txtNuevoCodigo";
		this.txtNuevoCodigo.Size = new System.Drawing.Size(100, 26);
		this.txtNuevoCodigo.TabIndex = 1;
		this.txtNuevoCodigo.Text = "PATCH2";
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.btnProcesar);
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.btnSeleccionarCarpetaSalida);
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.txtCarpetaSalida);
		this.gbSeleccionarCarpetaSalida.Location = new System.Drawing.Point(15, 832);
		this.gbSeleccionarCarpetaSalida.Name = "gbSeleccionarCarpetaSalida";
		this.gbSeleccionarCarpetaSalida.Size = new System.Drawing.Size(1660, 58);
		this.gbSeleccionarCarpetaSalida.TabIndex = 9;
		this.gbSeleccionarCarpetaSalida.TabStop = false;
		this.btnProcesar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnProcesar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnProcesar.FlatAppearance.BorderSize = 0;
		this.btnProcesar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnProcesar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnProcesar.ForeColor = System.Drawing.Color.White;
		this.btnProcesar.Image = (System.Drawing.Image)resources.GetObject("btnProcesar.Image");
		this.btnProcesar.Location = new System.Drawing.Point(702, 19);
		this.btnProcesar.Name = "btnProcesar";
		this.btnProcesar.Size = new System.Drawing.Size(237, 25);
		this.btnProcesar.TabIndex = 52;
		this.btnProcesar.Text = "   Iniciar Proceso";
		this.btnProcesar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnProcesar.UseVisualStyleBackColor = false;
		this.btnProcesar.Click += new System.EventHandler(btnProcesar_Click_1);
		this.btnSeleccionarCarpetaSalida.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarCarpetaSalida.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarCarpetaSalida.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarCarpetaSalida.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarCarpetaSalida.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarCarpetaSalida.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarCarpetaSalida.Image");
		this.btnSeleccionarCarpetaSalida.Location = new System.Drawing.Point(6, 19);
		this.btnSeleccionarCarpetaSalida.Name = "btnSeleccionarCarpetaSalida";
		this.btnSeleccionarCarpetaSalida.Size = new System.Drawing.Size(243, 25);
		this.btnSeleccionarCarpetaSalida.TabIndex = 37;
		this.btnSeleccionarCarpetaSalida.Text = "   Seleccionar Carpeta de Salida";
		this.btnSeleccionarCarpetaSalida.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarCarpetaSalida.UseVisualStyleBackColor = false;
		this.btnSeleccionarCarpetaSalida.Click += new System.EventHandler(btnSeleccionarCarpetaSalida_Click_1);
		this.txtCarpetaSalida.BackColor = System.Drawing.Color.DarkGray;
		this.txtCarpetaSalida.Enabled = false;
		this.txtCarpetaSalida.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCarpetaSalida.Location = new System.Drawing.Point(255, 19);
		this.txtCarpetaSalida.Multiline = true;
		this.txtCarpetaSalida.Name = "txtCarpetaSalida";
		this.txtCarpetaSalida.Size = new System.Drawing.Size(409, 25);
		this.txtCarpetaSalida.TabIndex = 1;
		this.progressBar1.Location = new System.Drawing.Point(12, 232);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(1340, 30);
		this.progressBar1.TabIndex = 10;
		this.progressBar1.Visible = false;
		this.dgvTotalLotesEncontrados.AllowUserToAddRows = false;
		this.dgvTotalLotesEncontrados.AllowUserToDeleteRows = false;
		this.dgvTotalLotesEncontrados.AllowUserToResizeRows = false;
		this.dgvTotalLotesEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvTotalLotesEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvTotalLotesEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvTotalLotesEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvTotalLotesEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvTotalLotesEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dgvTotalLotesEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvTotalLotesEncontrados.DefaultCellStyle = dataGridViewCellStyle2;
		this.dgvTotalLotesEncontrados.EnableHeadersVisualStyles = false;
		this.dgvTotalLotesEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvTotalLotesEncontrados.Location = new System.Drawing.Point(1388, 13);
		this.dgvTotalLotesEncontrados.Name = "dgvTotalLotesEncontrados";
		this.dgvTotalLotesEncontrados.ReadOnly = true;
		this.dgvTotalLotesEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvTotalLotesEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dgvTotalLotesEncontrados.RowHeadersVisible = false;
		this.dgvTotalLotesEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dgvTotalLotesEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dgvTotalLotesEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvTotalLotesEncontrados.Size = new System.Drawing.Size(220, 174);
		this.dgvTotalLotesEncontrados.TabIndex = 17;
		this.dgvTotalLotesEncontrados.Visible = false;
		this.dgvIndicesEncontrados.AllowUserToAddRows = false;
		this.dgvIndicesEncontrados.AllowUserToDeleteRows = false;
		this.dgvIndicesEncontrados.AllowUserToResizeRows = false;
		this.dgvIndicesEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvIndicesEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvIndicesEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvIndicesEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvIndicesEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvIndicesEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
		this.dgvIndicesEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvIndicesEncontrados.DefaultCellStyle = dataGridViewCellStyle6;
		this.dgvIndicesEncontrados.EnableHeadersVisualStyles = false;
		this.dgvIndicesEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvIndicesEncontrados.Location = new System.Drawing.Point(1377, 88);
		this.dgvIndicesEncontrados.Name = "dgvIndicesEncontrados";
		this.dgvIndicesEncontrados.ReadOnly = true;
		this.dgvIndicesEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvIndicesEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dgvIndicesEncontrados.RowHeadersVisible = false;
		this.dgvIndicesEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		this.dgvIndicesEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle8;
		this.dgvIndicesEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvIndicesEncontrados.Size = new System.Drawing.Size(225, 130);
		this.dgvIndicesEncontrados.TabIndex = 18;
		this.dgvIndicesEncontrados.Visible = false;
		this.dgvSeparadoresEncontrados.AllowUserToAddRows = false;
		this.dgvSeparadoresEncontrados.AllowUserToDeleteRows = false;
		this.dgvSeparadoresEncontrados.AllowUserToResizeRows = false;
		this.dgvSeparadoresEncontrados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvSeparadoresEncontrados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvSeparadoresEncontrados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvSeparadoresEncontrados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvSeparadoresEncontrados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvSeparadoresEncontrados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.dgvSeparadoresEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvSeparadoresEncontrados.DefaultCellStyle = dataGridViewCellStyle10;
		this.dgvSeparadoresEncontrados.EnableHeadersVisualStyles = false;
		this.dgvSeparadoresEncontrados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvSeparadoresEncontrados.Location = new System.Drawing.Point(60, 65);
		this.dgvSeparadoresEncontrados.Name = "dgvSeparadoresEncontrados";
		this.dgvSeparadoresEncontrados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvSeparadoresEncontrados.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
		this.dgvSeparadoresEncontrados.RowHeadersVisible = false;
		this.dgvSeparadoresEncontrados.RowHeadersWidth = 15;
		dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle12.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
		this.dgvSeparadoresEncontrados.RowsDefaultCellStyle = dataGridViewCellStyle12;
		this.dgvSeparadoresEncontrados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvSeparadoresEncontrados.Size = new System.Drawing.Size(248, 301);
		this.dgvSeparadoresEncontrados.TabIndex = 38;
		this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancelar.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(1395, 13);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(130, 25);
		this.btnCancelar.TabIndex = 39;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(1395, 55);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(130, 25);
		this.btnCerrar.TabIndex = 40;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.txtSeleccion.BackColor = System.Drawing.Color.DarkGray;
		this.txtSeleccion.Location = new System.Drawing.Point(1377, 34);
		this.txtSeleccion.Name = "txtSeleccion";
		this.txtSeleccion.Size = new System.Drawing.Size(144, 20);
		this.txtSeleccion.TabIndex = 43;
		this.txtSeleccion.Visible = false;
		this.pnlLotesDisponibles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLotesDisponibles.Controls.Add(this.dgvLotesDisponibles);
		this.pnlLotesDisponibles.Location = new System.Drawing.Point(12, 12);
		this.pnlLotesDisponibles.Name = "pnlLotesDisponibles";
		this.pnlLotesDisponibles.Size = new System.Drawing.Size(600, 175);
		this.pnlLotesDisponibles.TabIndex = 59;
		this.dgvLotesDisponibles.AllowUserToAddRows = false;
		this.dgvLotesDisponibles.AllowUserToDeleteRows = false;
		this.dgvLotesDisponibles.AllowUserToResizeRows = false;
		this.dgvLotesDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle13.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
		this.dgvLotesDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesDisponibles.DefaultCellStyle = dataGridViewCellStyle14;
		this.dgvLotesDisponibles.EnableHeadersVisualStyles = false;
		this.dgvLotesDisponibles.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesDisponibles.Location = new System.Drawing.Point(11, 22);
		this.dgvLotesDisponibles.Name = "dgvLotesDisponibles";
		this.dgvLotesDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
		this.dgvLotesDisponibles.RowHeadersVisible = false;
		this.dgvLotesDisponibles.RowHeadersWidth = 15;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle16;
		this.dgvLotesDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesDisponibles.Size = new System.Drawing.Size(472, 140);
		this.dgvLotesDisponibles.TabIndex = 18;
		this.btnSeleccionarTodo.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarTodo.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarTodo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarTodo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarTodo.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarTodo.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarTodo.Image");
		this.btnSeleccionarTodo.Location = new System.Drawing.Point(12, 193);
		this.btnSeleccionarTodo.Name = "btnSeleccionarTodo";
		this.btnSeleccionarTodo.Size = new System.Drawing.Size(180, 25);
		this.btnSeleccionarTodo.TabIndex = 37;
		this.btnSeleccionarTodo.Text = "   Seleccionar Todo";
		this.btnSeleccionarTodo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarTodo.UseVisualStyleBackColor = false;
		this.btnSeleccionarTodo.Click += new System.EventHandler(btnSeleccionarTodo_Click);
		this.btnDeseleccionarTodo.BackColor = System.Drawing.Color.SeaGreen;
		this.btnDeseleccionarTodo.FlatAppearance.BorderSize = 0;
		this.btnDeseleccionarTodo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnDeseleccionarTodo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnDeseleccionarTodo.ForeColor = System.Drawing.Color.White;
		this.btnDeseleccionarTodo.Image = (System.Drawing.Image)resources.GetObject("btnDeseleccionarTodo.Image");
		this.btnDeseleccionarTodo.Location = new System.Drawing.Point(432, 193);
		this.btnDeseleccionarTodo.Name = "btnDeseleccionarTodo";
		this.btnDeseleccionarTodo.Size = new System.Drawing.Size(180, 25);
		this.btnDeseleccionarTodo.TabIndex = 60;
		this.btnDeseleccionarTodo.Text = "   Deseleccionar Todo";
		this.btnDeseleccionarTodo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnDeseleccionarTodo.UseVisualStyleBackColor = false;
		this.btnDeseleccionarTodo.Click += new System.EventHandler(btnDeseleccionarTodo_Click);
		this.btnAgregarLotes.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarLotes.FlatAppearance.BorderSize = 0;
		this.btnAgregarLotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarLotes.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarLotes.ForeColor = System.Drawing.Color.White;
		this.btnAgregarLotes.Image = (System.Drawing.Image)resources.GetObject("btnAgregarLotes.Image");
		this.btnAgregarLotes.Location = new System.Drawing.Point(618, 55);
		this.btnAgregarLotes.Name = "btnAgregarLotes";
		this.btnAgregarLotes.Size = new System.Drawing.Size(132, 25);
		this.btnAgregarLotes.TabIndex = 37;
		this.btnAgregarLotes.Text = "   Agregar";
		this.btnAgregarLotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarLotes.UseVisualStyleBackColor = false;
		this.btnAgregarLotes.Click += new System.EventHandler(btnAgregarLotes_Click);
		this.btnQuitarLotes.BackColor = System.Drawing.Color.SeaGreen;
		this.btnQuitarLotes.FlatAppearance.BorderSize = 0;
		this.btnQuitarLotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnQuitarLotes.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnQuitarLotes.ForeColor = System.Drawing.Color.White;
		this.btnQuitarLotes.Image = (System.Drawing.Image)resources.GetObject("btnQuitarLotes.Image");
		this.btnQuitarLotes.Location = new System.Drawing.Point(618, 86);
		this.btnQuitarLotes.Name = "btnQuitarLotes";
		this.btnQuitarLotes.Size = new System.Drawing.Size(132, 25);
		this.btnQuitarLotes.TabIndex = 61;
		this.btnQuitarLotes.Text = "   Quitar";
		this.btnQuitarLotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnQuitarLotes.UseVisualStyleBackColor = false;
		this.btnQuitarLotes.Click += new System.EventHandler(btnQuitarLotes_Click);
		this.pnlLotesSeleccionados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLotesSeleccionados.Controls.Add(this.dgvLotesSeleccionados);
		this.pnlLotesSeleccionados.Location = new System.Drawing.Point(756, 12);
		this.pnlLotesSeleccionados.Name = "pnlLotesSeleccionados";
		this.pnlLotesSeleccionados.Size = new System.Drawing.Size(600, 175);
		this.pnlLotesSeleccionados.TabIndex = 60;
		this.dgvLotesSeleccionados.AllowUserToAddRows = false;
		this.dgvLotesSeleccionados.AllowUserToDeleteRows = false;
		this.dgvLotesSeleccionados.AllowUserToResizeRows = false;
		this.dgvLotesSeleccionados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesSeleccionados.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesSeleccionados.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesSeleccionados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesSeleccionados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle17.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle17.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle17.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle17.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesSeleccionados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
		this.dgvLotesSeleccionados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesSeleccionados.DefaultCellStyle = dataGridViewCellStyle18;
		this.dgvLotesSeleccionados.EnableHeadersVisualStyles = false;
		this.dgvLotesSeleccionados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesSeleccionados.Location = new System.Drawing.Point(11, 22);
		this.dgvLotesSeleccionados.Name = "dgvLotesSeleccionados";
		this.dgvLotesSeleccionados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle19.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle19.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesSeleccionados.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
		this.dgvLotesSeleccionados.RowHeadersVisible = false;
		this.dgvLotesSeleccionados.RowHeadersWidth = 15;
		dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle20.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle20.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesSeleccionados.RowsDefaultCellStyle = dataGridViewCellStyle20;
		this.dgvLotesSeleccionados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesSeleccionados.Size = new System.Drawing.Size(472, 140);
		this.dgvLotesSeleccionados.TabIndex = 18;
		this.btnProcesarIndices.BackColor = System.Drawing.Color.DarkGray;
		this.btnProcesarIndices.Enabled = false;
		this.btnProcesarIndices.FlatAppearance.BorderSize = 0;
		this.btnProcesarIndices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnProcesarIndices.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnProcesarIndices.ForeColor = System.Drawing.Color.White;
		this.btnProcesarIndices.Image = (System.Drawing.Image)resources.GetObject("btnProcesarIndices.Image");
		this.btnProcesarIndices.Location = new System.Drawing.Point(752, 193);
		this.btnProcesarIndices.Name = "btnProcesarIndices";
		this.btnProcesarIndices.Size = new System.Drawing.Size(600, 25);
		this.btnProcesarIndices.TabIndex = 53;
		this.btnProcesarIndices.Text = "   Buscar Seperadores";
		this.btnProcesarIndices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnProcesarIndices.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnProcesarIndices.UseVisualStyleBackColor = false;
		this.btnProcesarIndices.Click += new System.EventHandler(btnProcesarIndices_Click);
		this.pnlSeparadoresEncontrados.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlSeparadoresEncontrados.Controls.Add(this.dgvSeparadoresEncontrados);
		this.pnlSeparadoresEncontrados.Location = new System.Drawing.Point(12, 278);
		this.pnlSeparadoresEncontrados.Name = "pnlSeparadoresEncontrados";
		this.pnlSeparadoresEncontrados.Size = new System.Drawing.Size(361, 548);
		this.pnlSeparadoresEncontrados.TabIndex = 60;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1534, 883);
		base.Controls.Add(this.pnlSeparadoresEncontrados);
		base.Controls.Add(this.btnProcesarIndices);
		base.Controls.Add(this.pnlLotesSeleccionados);
		base.Controls.Add(this.btnQuitarLotes);
		base.Controls.Add(this.btnAgregarLotes);
		base.Controls.Add(this.btnDeseleccionarTodo);
		base.Controls.Add(this.btnSeleccionarTodo);
		base.Controls.Add(this.pnlLotesDisponibles);
		base.Controls.Add(this.txtSeleccion);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.dgvIndicesEncontrados);
		base.Controls.Add(this.dgvTotalLotesEncontrados);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.gbSeleccionarCarpetaSalida);
		base.Controls.Add(this.pnlAsignarCodigo);
		base.Controls.Add(this.lboxListaArchivos);
		base.Controls.Add(this.gbBotonesNavegacion);
		base.Controls.Add(this.pnlVisor);
		base.Name = "frmMunicipalidadVillaReginaCatastro";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Procesar Municipalidad de Villa Regina - CATASTRO";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmMunicipalidadVillaReginaCatastro_FormClosing);
		base.Load += new System.EventHandler(frmMunicipalidadVillaReginaCatastro_Load);
		this.gbBotonesNavegacion.ResumeLayout(false);
		this.gbBotonesNavegacion.PerformLayout();
		this.pnlAsignarCodigo.ResumeLayout(false);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.gbSeleccionarCarpetaSalida.ResumeLayout(false);
		this.gbSeleccionarCarpetaSalida.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesEncontrados).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dgvIndicesEncontrados).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dgvSeparadoresEncontrados).EndInit();
		this.pnlLotesDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).EndInit();
		this.pnlLotesSeleccionados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesSeleccionados).EndInit();
		this.pnlSeparadoresEncontrados.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
