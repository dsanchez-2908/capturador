using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;
using PdfiumViewer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Capturador._01_Pantallas._02_ControlCalidad;

public class frmControlCalidad_v3 : Form
{
	private eUsuario oUsuarioLogueado;

	private string rutaCarpetaInicial;

	private List<string> pdfFiles = new List<string>();

	private int currentFileIndex = 0;

	private int currentPage = 0;

	private int pagesPerView = 1;

	private PdfiumViewer.PdfDocument pdfDocument;

	private bool hasChanges = false;

	private List<string> listaArchivos = new List<string>();

	private string rutaArchivoAgregar;

	private int zoomLevel = 5;

	private const int zoomMin = 2;

	private const int zoomMax = 10;

	private eLote oLoteSeleccionado = new eLote();

	private Dictionary<int, int> pageRotations = new Dictionary<int, int>();

	private FormWindowState LastWindowState = FormWindowState.Minimized;

	private IContainer components = null;

	private TextBox txtFuncionSeleccionado;

	private Label label1;

	private ListBox lboxListaArchivos;

	private Panel pnlVisor;

	private GroupBox gbPaginasMostrar;

	private ProgressBar progressBar1;

	private Panel pnlMoverPagina;

	private Button btnCancelarMover;

	private Button btnMover;

	private TextBox txtNumeroPaginaMover;

	private Label label5;

	private Label label4;

	private TextBox txtPaginaSelccionadaMover;

	private Label lblZoom;

	private Panel pnlLotesDisponibles;

	private DataGridView dgvLotesDisponibles;

	private Button btnCerrar;

	private Button btnCancelar;

	private Button btnCargarLote;

	private Panel pnlLoteSeleccionado;

	private Label lblNombreLote;

	private Label lblCarpetaLote;

	private Label lblTotalArchivos;

	private Label lblProyeto;

	private TextBox txtRutaLote;

	private TextBox txtLoteSeleccionado;

	private TextBox txtTotalArchivosSeleccionado;

	private TextBox txtProyectoSeleccionado;

	private GroupBox gbNavegacion;

	private TextBox txtArchivoActual;

	private Button btnPaginaSiguiente;

	private Button btnPaginaAnterior;

	private Button btnArchivoSiguiente;

	private Button btnArchivoAnterior;

	private TextBox txtTotalPaginas;

	private GroupBox gbModificacion;

	private Button btnZoom;

	private Button btnZoomMenos;

	private Button btnZoomMas;

	private Button btnEliminarPagina;

	private Button btnMoverPaginas;

	private Button btnGirarIzquierda;

	private Button btnGirarDerecha;

	private GroupBox gbZoom;

	private Button btnEliminarPaginasBlanco;

	private GroupBox groupBox1;

	private ComboBox cbxCantidadPaginas;

	private Label label2;

	private Button btnMarcarLoteCompletado;

	private Button btnModificarNombre;

	private Panel pnlModificarNombreArchivo;

	private Button btnModificarNombreArchivo;

	private Label label3;

	private Label label6;

	private TextBox txtNombreArchivoActual;

	private TextBox txtNuevoNombreArchivo;

	private Button btnCancelarModificarNombre;

	private Button btnAgregarPaginas;

	private Panel pnlAgregarPaginas;

	private Button btnSeleccionarArchivoAgregar;

	private Button btnCancelarAgregarPaginas;

	private Button btnAgregarPaginasArchivo;

	private Label label7;

	private TextBox txtRutaArchivoAgregar;

	private TextBox txtNumeroPaginaParaAgregar;

	public frmControlCalidad_v3(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
		cbxCantidadPaginas.Items.AddRange(new object[7] { 1, 2, 4, 6, 8, 10, 12 });
		cbxCantidadPaginas.SelectedIndex = 6;
		cbxCantidadPaginas.SelectedIndexChanged += delegate
		{
			ReloadPdf();
		};
		base.KeyPreview = true;
		base.KeyDown += frmControlCalidad_v2_KeyDown;
	}

	private void frmControlCalidad_v2_Load(object sender, EventArgs e)
	{
		eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		cargarToolTip();
		actualizarLotesDisponibles();
		ajustarPanelLoteSeleccionado();
		ajustarFormulario_SeleccionarLote();
	}

	private void cargarToolTip()
	{
		ToolTip oToolTip1 = new ToolTip();
		oToolTip1.SetToolTip(btnCargarLote, "Carga el lote Seleccionado");
		oToolTip1.SetToolTip(btnCancelar, "Cancela el lote seleccionado");
		oToolTip1.SetToolTip(btnCerrar, "Cierra la Ventana de Control de Calidad");
		oToolTip1.SetToolTip(btnMarcarLoteCompletado, "Marca el lote seleccionado como terminado");
		oToolTip1.SetToolTip(btnArchivoAnterior, "Vuelve al archivo anterior");
		oToolTip1.SetToolTip(btnPaginaAnterior, "Vuelve al anterior grupo de páginas");
		oToolTip1.SetToolTip(btnPaginaSiguiente, "Pasa al siguiente grupo de páginas");
		oToolTip1.SetToolTip(btnArchivoSiguiente, "Pasa al siguiente archivo");
		oToolTip1.SetToolTip(btnModificarNombre, "Modifica el nombre del archivo actual");
		oToolTip1.SetToolTip(btnZoom, "Permite hacer Zoom al hacer clic en una página");
		oToolTip1.SetToolTip(btnZoomMas, "Permite hacer mayor zoom a todas las páginas");
		oToolTip1.SetToolTip(btnZoomMenos, "Permite reducir zoom a todas las páginas");
		oToolTip1.SetToolTip(btnGirarIzquierda, "Gira a la izquierda al hacer clic en una página");
		oToolTip1.SetToolTip(btnGirarDerecha, "Gira a la derecha al hacer clic en una página");
		oToolTip1.SetToolTip(btnMoverPaginas, "Mueve página al hacer clic en una página");
		oToolTip1.SetToolTip(btnEliminarPagina, "Elimina página al hacer clic en una página");
		oToolTip1.SetToolTip(btnAgregarPaginas, "Agrega páginas al archivo actual");
		oToolTip1.SetToolTip(btnEliminarPaginasBlanco, "Elimina masivamente todas las páginas en blanco del lote seleccionado");
		oToolTip1.SetToolTip(btnModificarNombreArchivo, "Modifica el nombre del archivo actual");
		oToolTip1.SetToolTip(btnCancelarModificarNombre, "Cancela la moficicación del nombre del archivo seleccionado");
		oToolTip1.SetToolTip(btnMover, "Mueve la página");
		oToolTip1.SetToolTip(btnCancelarMover, "Cancela mover la página seleccionada");
		oToolTip1.SetToolTip(btnZoomMenos, "");
		oToolTip1.SetToolTip(btnZoomMenos, "");
		oToolTip1.SetToolTip(btnZoomMenos, "");
	}

	private void ajustarFormulario_SeleccionarLote()
	{
		base.Size = new Size(640, 240);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_ControlCalidad()
	{
		base.Size = new Size(1600, 850);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void actualizarLotesDisponibles()
	{
		dgvLotesDisponibles.DataSource = nLotes.obtenerLotesDisponibleControlCalidad(oUsuarioLogueado, 0);
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
		pnlLoteSeleccionado.Controls.Add(labelTituloLoteSeleccionado);
	}

	private void btnCargarLotes_Click(object sender, EventArgs e)
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
		btnCargarLote.BackColor = Color.DarkGray;
		btnCargarLote.Enabled = false;
		dgvLotesDisponibles.Enabled = false;
		btnCancelar.Enabled = true;
		btnCancelar.BackColor = Color.Salmon;
		base.MaximizeBox = true;
		ajustarFormulario_ControlCalidad();
		oLoteSeleccionado.cdProyecto = Convert.ToInt32(dgvLotesDisponibles.SelectedRows[0].Cells["cdProyecto"].Value.ToString());
		oLoteSeleccionado.cdLote = Convert.ToInt32(dgvLotesDisponibles.SelectedRows[0].Cells["cdLote"].Value.ToString());
		oLoteSeleccionado.dsRutaLote = dgvLotesDisponibles.SelectedRows[0].Cells["dsRutaLote"].Value.ToString();
		txtProyectoSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsProyecto"].Value.ToString();
		txtLoteSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsNombreLote"].Value.ToString();
		txtTotalArchivosSeleccionado.Text = dgvLotesDisponibles.SelectedRows[0].Cells["nuCantidadArchivos"].Value.ToString();
		txtRutaLote.Text = dgvLotesDisponibles.SelectedRows[0].Cells["dsRutaLote"].Value.ToString();
		LoadPdfFiles(txtRutaLote.Text);
	}

	private void btnCerrar_Click_1(object sender, EventArgs e)
	{
		pdfDocument.Dispose();
		Close();
	}

	private void btnCancelar_Click_1(object sender, EventArgs e)
	{
		deshabilitarFormulario();
	}

	private void btnArchivoAnterior_Click_1(object sender, EventArgs e)
	{
		if (currentFileIndex > 0)
		{
			currentFileIndex--;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			progressBar1.Value = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
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
			progressBar1.Value = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
		else if (MessageBox.Show("Termino de Controlar el Lote, lo quiere dar por finalizado?", "Confirmar Finalización de Lote", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			finalizarControlCalidad();
		}
	}

	private void btnArchivoSiguiente_Click_1(object sender, EventArgs e)
	{
		if (currentFileIndex < pdfFiles.Count - 1)
		{
			currentFileIndex++;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			progressBar1.Value = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
	}

	private void btnZoomMas_Click_1(object sender, EventArgs e)
	{
		if (zoomLevel < 10)
		{
			zoomLevel++;
			UpdateViewer();
			lblZoom.Text = $"Zoom: {zoomLevel * 20}%";
		}
	}

	private void btnZoomMenos_Click_1(object sender, EventArgs e)
	{
		if (zoomLevel > 2)
		{
			zoomLevel--;
			UpdateViewer();
			lblZoom.Text = $"Zoom: {zoomLevel * 20}%";
		}
	}

	private void btnGirarIzquierda_Click_1(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "GIRAR A LA IZQUIERDA")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "GIRAR A LA IZQUIERDA";
		}
		actualizarbotones();
	}

	private void btnGirarDerecha_Click_1(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "GIRAR A LA DERECHA")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "GIRAR A LA DERECHA";
		}
		actualizarbotones();
	}

	private void btnMoverPaginas_Click_1(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "MOVER PAGINAS")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "MOVER PAGINAS";
		}
		actualizarbotones();
	}

	private void btnEliminarPagina_Click(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "ELIMINAR")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "ELIMINAR";
		}
		actualizarbotones();
	}

	private void btnZoom_Click(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "ZOOM")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "ZOOM";
		}
		actualizarbotones();
	}

	private void actualizarbotones()
	{
		if (string.IsNullOrEmpty(txtFuncionSeleccionado.Text))
		{
			btnZoom.BackColor = Color.SeaGreen;
			btnGirarIzquierda.BackColor = Color.SeaGreen;
			btnGirarDerecha.BackColor = Color.SeaGreen;
			btnMoverPaginas.BackColor = Color.SeaGreen;
			btnEliminarPagina.BackColor = Color.SeaGreen;
			return;
		}
		if (txtFuncionSeleccionado.Text == "ZOOM")
		{
			btnZoom.BackColor = Color.SteelBlue;
			btnGirarIzquierda.BackColor = Color.SeaGreen;
			btnGirarDerecha.BackColor = Color.SeaGreen;
			btnMoverPaginas.BackColor = Color.SeaGreen;
			btnEliminarPagina.BackColor = Color.SeaGreen;
		}
		if (txtFuncionSeleccionado.Text == "GIRAR A LA IZQUIERDA")
		{
			btnZoom.BackColor = Color.SeaGreen;
			btnGirarIzquierda.BackColor = Color.SteelBlue;
			btnGirarDerecha.BackColor = Color.SeaGreen;
			btnMoverPaginas.BackColor = Color.SeaGreen;
			btnEliminarPagina.BackColor = Color.SeaGreen;
		}
		if (txtFuncionSeleccionado.Text == "GIRAR A LA DERECHA")
		{
			btnZoom.BackColor = Color.SeaGreen;
			btnGirarIzquierda.BackColor = Color.SeaGreen;
			btnGirarDerecha.BackColor = Color.SteelBlue;
			btnMoverPaginas.BackColor = Color.SeaGreen;
			btnEliminarPagina.BackColor = Color.SeaGreen;
		}
		if (txtFuncionSeleccionado.Text == "MOVER PAGINAS")
		{
			btnZoom.BackColor = Color.SeaGreen;
			btnGirarIzquierda.BackColor = Color.SeaGreen;
			btnGirarDerecha.BackColor = Color.SeaGreen;
			btnMoverPaginas.BackColor = Color.SteelBlue;
			btnEliminarPagina.BackColor = Color.SeaGreen;
		}
		if (txtFuncionSeleccionado.Text == "ELIMINAR")
		{
			btnZoom.BackColor = Color.SeaGreen;
			btnGirarIzquierda.BackColor = Color.SeaGreen;
			btnGirarDerecha.BackColor = Color.SeaGreen;
			btnMoverPaginas.BackColor = Color.SeaGreen;
			btnEliminarPagina.BackColor = Color.SteelBlue;
		}
	}

	private void btnEliminarPaginasBlanco_Click_1(object sender, EventArgs e)
	{
		pdfDocument.Dispose();
		frmEliminarPaginasBlanco_v2 oFrmEliminarPaginasBlanco_v2 = new frmEliminarPaginasBlanco_v2(oUsuarioLogueado, txtRutaLote.Text, listaArchivos);
		oFrmEliminarPaginasBlanco_v2.Show();
		oFrmEliminarPaginasBlanco_v2.OnProcesoTerminado += delegate(string filePath)
		{
			LoadPdfFiles(filePath);
		};
		oFrmEliminarPaginasBlanco_v2.Show();
	}

	private void LoadPdfFiles(string folderPath)
	{
		pdfFiles = Directory.GetFiles(folderPath, "*.pdf").ToList();
		lboxListaArchivos.Items.Clear();
		ListBox.ObjectCollection items = lboxListaArchivos.Items;
		object[] items2 = pdfFiles.Select(Path.GetFileName).ToArray();
		items.AddRange(items2);
		listaArchivos.AddRange(pdfFiles.Select(Path.GetFileName).ToArray());
		if (pdfFiles.Count > 0)
		{
			currentFileIndex = 0;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			progressBar1.Maximum = pdfFiles.Count - 1;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
		else
		{
			MessageBox.Show("La carpeta seleccionada no tiene archivos PDF", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void LoadPdf(string filePath)
	{
		if (hasChanges)
		{
			SavePdf();
		}
		pdfDocument?.Dispose();
		pdfDocument = PdfiumViewer.PdfDocument.Load(filePath);
		currentPage = 0;
		hasChanges = false;
		txtArchivoActual.Text = Path.GetFileName(filePath);
		txtTotalPaginas.Text = pdfDocument.PageCount.ToString();
		habilitarFormulario();
		GC.Collect();
		UpdateViewer();
	}

	private void habilitarFormulario()
	{
		btnCancelar.Enabled = true;
		btnEliminarPaginasBlanco.Enabled = true;
		cbxCantidadPaginas.Enabled = true;
		btnPaginaAnterior.Enabled = true;
		btnPaginaSiguiente.Enabled = true;
		btnArchivoAnterior.Enabled = true;
		btnArchivoSiguiente.Enabled = true;
		btnZoom.Enabled = true;
		btnGirarIzquierda.Enabled = true;
		btnGirarDerecha.Enabled = true;
		btnEliminarPagina.Enabled = true;
		btnMoverPaginas.Enabled = true;
		btnZoomMas.Enabled = true;
		btnZoomMenos.Enabled = true;
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
		pagesPerView = (int)cbxCantidadPaginas.SelectedItem;
		foreach (Control control in pnlVisor.Controls)
		{
			control.Dispose();
		}
		pnlVisor.Controls.Clear();
		GC.Collect();
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
		int slotWidth = pnlVisor.Width / columns;
		int slotHeight = pnlVisor.Height / rows;
		for (int i = 0; i < pagesPerView; i++)
		{
			int pageIndex = currentPage + i;
			if (pageIndex >= pdfDocument.PageCount)
			{
				break;
			}
			int dpi = zoomLevel * 20;
			Bitmap renderedBitmap;
			using (Image bmp = pdfDocument.Render(pageIndex, dpi, dpi, forPrinting: true))
			{
				renderedBitmap = new Bitmap(bmp);
			}
			float scale = (float)zoomLevel / 5f;
			int picWidth = (int)((float)slotWidth * scale);
			int picHeight = (int)((float)(slotHeight - 20) * scale);
			Label lblPagina = new Label
			{
				Text = $"Página {pageIndex + 1}",
				TextAlign = ContentAlignment.MiddleCenter,
				Dock = DockStyle.Top,
				Height = 20,
				BackColor = Color.FromArgb(77, 96, 130),
				ForeColor = Color.White
			};
			PictureBox pictureBox = new PictureBox
			{
				Width = picWidth,
				Height = picHeight,
				Image = renderedBitmap,
				SizeMode = PictureBoxSizeMode.Zoom,
				BorderStyle = BorderStyle.FixedSingle,
				Anchor = (AnchorStyles.Top | AnchorStyles.Left)
			};
			Panel panel = new Panel
			{
				Width = slotWidth,
				Height = slotHeight,
				BorderStyle = BorderStyle.None
			};
			pictureBox.Top = lblPagina.Height;
			pictureBox.Left = (panel.Width - pictureBox.Width) / 2;
			panel.Controls.Add(lblPagina);
			panel.Controls.Add(pictureBox);
			pnlVisor.Controls.Add(panel);
			pictureBox.MouseClick += PdfViewer_MouseClick;
			pictureBox.AllowDrop = true;
			int col = i % columns;
			int row = i / columns;
			panel.Left = col * slotWidth;
			panel.Top = row * slotHeight;
		}
		lblZoom.Text = $"Zoom: {zoomLevel * 20}%";
		GC.Collect();
	}

	private void PdfViewer_MouseClick(object sender, MouseEventArgs e)
	{
		PictureBox pictureBox = sender as PictureBox;
		if (pictureBox == null)
		{
			return;
		}
		int pageIndex = currentPage + pnlVisor.Controls.IndexOf(pictureBox.Parent);
		if (pageIndex >= pdfDocument.PageCount)
		{
			return;
		}
		if (!pageRotations.ContainsKey(pageIndex))
		{
			pageRotations[pageIndex] = 0;
		}
		int currentRotation = pageRotations[pageIndex];
		PictureBox zoomPictureBox;
		float zoomFactor;
		if (txtFuncionSeleccionado.Text == "GIRAR A LA IZQUIERDA")
		{
			currentRotation = (currentRotation + 3) % 4;
			pdfDocument.RotatePage(pageIndex, (PdfRotation)currentRotation);
			pageRotations[pageIndex] = currentRotation;
			hasChanges = true;
			SavePdf();
			UpdateViewer();
		}
		else if (txtFuncionSeleccionado.Text == "GIRAR A LA DERECHA")
		{
			currentRotation = (currentRotation + 1) % 4;
			pdfDocument.RotatePage(pageIndex, (PdfRotation)currentRotation);
			pageRotations[pageIndex] = currentRotation;
			hasChanges = true;
			SavePdf();
			UpdateViewer();
		}
		else if (txtFuncionSeleccionado.Text == "ELIMINAR")
		{
			if (MessageBox.Show("¿Está seguro que quiere eliminar la página seleccionada?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				pdfDocument.DeletePage(pageIndex);
				pageRotations.Remove(pageIndex);
				hasChanges = true;
				SavePdf();
				UpdateViewer();
			}
		}
		else if (txtFuncionSeleccionado.Text == "ZOOM")
		{
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
		else if (txtFuncionSeleccionado.Text == "MOVER PAGINAS")
		{
			txtPaginaSelccionadaMover.Text = Convert.ToString(pageIndex + 1);
			pnlMoverPagina.Visible = true;
			txtNumeroPaginaMover.Focus();
		}
		void AplicarZoom(float factor)
		{
			zoomFactor *= factor;
			zoomPictureBox.Width = (int)((float)pictureBox.Image.Width * zoomFactor);
			zoomPictureBox.Height = (int)((float)pictureBox.Image.Height * zoomFactor);
		}
	}

	private void SavePdf()
	{
		if (pdfDocument != null && hasChanges)
		{
			string filePath = pdfFiles[currentFileIndex];
			string tempPath = filePath + ".temp";
			pdfDocument.Save(tempPath);
			pdfDocument.Dispose();
			File.Delete(filePath);
			File.Move(tempPath, filePath);
			pdfDocument = PdfiumViewer.PdfDocument.Load(filePath);
			hasChanges = false;
		}
	}

	private void deshabilitarFormulario()
	{
		base.MaximizeBox = false;
		base.WindowState = FormWindowState.Normal;
		btnCargarLote.BackColor = Color.SeaGreen;
		btnCargarLote.Enabled = true;
		btnCancelar.Enabled = false;
		btnCancelar.BackColor = Color.DarkGray;
		dgvLotesDisponibles.Enabled = true;
		txtProyectoSeleccionado.Clear();
		txtLoteSeleccionado.Clear();
		txtTotalArchivosSeleccionado.Clear();
		txtRutaLote.Clear();
		listaArchivos.Clear();
		lboxListaArchivos.Items.Clear();
		lboxListaArchivos.DataSource = null;
		txtArchivoActual.Clear();
		txtTotalPaginas.Clear();
		progressBar1.Value = 0;
		txtFuncionSeleccionado.Clear();
		pnlVisor.Controls.Clear();
		ajustarFormulario_SeleccionarLote();
		GC.Collect();
	}

	private void frmControlCalidad_v2_KeyDown(object sender, KeyEventArgs e)
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

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Up:
			btnArchivoAnterior_Click_1(null, null);
			return true;
		case Keys.Down:
			btnArchivoSiguiente_Click_1(null, null);
			return true;
		case Keys.Left:
			btnPaginaAnterior_Click_1(null, null);
			return true;
		case Keys.Right:
			btnPaginaSiguiente_Click_1(null, null);
			return true;
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}

	private void btnCancelarMover_Click(object sender, EventArgs e)
	{
		txtNumeroPaginaMover.Clear();
		pnlMoverPagina.Visible = false;
	}

	private void btnMover_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtNumeroPaginaMover.Text))
		{
			MessageBox.Show("Debe indicar el numero de página a donde mover", "Advertecia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		MovePdfPage(Convert.ToInt32(txtPaginaSelccionadaMover.Text), Convert.ToInt32(txtNumeroPaginaMover.Text));
		txtPaginaSelccionadaMover.Clear();
		txtNumeroPaginaMover.Clear();
		pnlMoverPagina.Visible = false;
	}

	private void MovePdfPage(int fromIndex, int toIndex)
	{
		string rutaArchivoActual = Path.Combine(txtRutaLote.Text, txtArchivoActual.Text);
		string tempFilePath = Path.GetTempFileName();
		using (PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(rutaArchivoActual, PdfDocumentOpenMode.Import))
		{
			List<PdfPage> pageList = inputDocument.Pages.Cast<PdfPage>().ToList();
			fromIndex--;
			PdfPage movedPage = pageList[fromIndex];
			pageList.RemoveAt(fromIndex);
			pageList.Insert(toIndex, movedPage);
			PdfSharp.Pdf.PdfDocument newDocument = new PdfSharp.Pdf.PdfDocument();
			foreach (PdfPage page in pageList)
			{
				PdfPage importedPage = newDocument.AddPage(page);
			}
			newDocument.Save(tempFilePath);
		}
		pdfDocument.Dispose();
		File.Copy(tempFilePath, rutaArchivoActual, overwrite: true);
		pdfDocument = PdfiumViewer.PdfDocument.Load(rutaArchivoActual);
		UpdateViewer();
	}

	private void finalizarControlCalidad()
	{
		nLotes.finalizarControlCalidad(oUsuarioLogueado, oLoteSeleccionado);
		MessageBox.Show("Se finalizo el Control de Calidad del Lote", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		pdfDocument.Dispose();
		base.WindowState = FormWindowState.Normal;
		deshabilitarFormulario();
		actualizarLotesDisponibles();
	}

	private void pnlLoteSeleccionado_Paint(object sender, PaintEventArgs e)
	{
	}

	private void groupBox1_Enter(object sender, EventArgs e)
	{
	}

	private void btnMarcarLoteCompletado_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Esta seguro que quiere dar el lote como terminado?", "Confirmar Finalización de Lote", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			finalizarControlCalidad();
		}
	}

	private void frmControlCalidad_v3_Resize(object sender, EventArgs e)
	{
		if (base.WindowState != LastWindowState)
		{
			LastWindowState = base.WindowState;
			if (base.WindowState == FormWindowState.Maximized)
			{
				ReloadPdf();
			}
			if (base.WindowState == FormWindowState.Normal)
			{
				ReloadPdf();
			}
		}
	}

	private void btnModificarNombre_Click(object sender, EventArgs e)
	{
		pnlModificarNombreArchivo.Visible = true;
		txtNombreArchivoActual.Text = txtArchivoActual.Text;
	}

	private void btnModificarNombreArchivo_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtNuevoNombreArchivo.Text))
		{
			MessageBox.Show("Debe ingresar el nuevo nombre del archivo", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string nuevoNombre = txtNuevoNombreArchivo.Text.Trim();
		if (Path.GetExtension(nuevoNombre).ToLower() != ".pdf")
		{
			MessageBox.Show("El nombre del archivo debe tener la extensión .PDF.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		try
		{
			pdfDocument.Dispose();
			nArchivos.modificarNombreArchivo(txtRutaLote.Text, txtNombreArchivoActual.Text, txtNuevoNombreArchivo.Text);
			nControlCalidad.actualizarNombreArchivo(oUsuarioLogueado, oLoteSeleccionado.cdLote, txtNombreArchivoActual.Text + "|" + txtNuevoNombreArchivo.Text);
			txtNombreArchivoActual.Clear();
			txtNuevoNombreArchivo.Clear();
			pnlModificarNombreArchivo.Visible = false;
			listaArchivos.Clear();
			lboxListaArchivos.Items.Clear();
			lboxListaArchivos.DataSource = null;
			LoadPdfFiles(txtRutaLote.Text);
			MessageBox.Show("Se modificó el nombre del archivo correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void btnCancelarModificarNombre_Click(object sender, EventArgs e)
	{
		txtNombreArchivoActual.Clear();
		txtNuevoNombreArchivo.Clear();
		pnlModificarNombreArchivo.Visible = false;
	}

	private void btnAgregarPaginas_Click(object sender, EventArgs e)
	{
		pnlAgregarPaginas.Visible = true;
	}

	private void btnSeleccionarArchivoAgregar_Click(object sender, EventArgs e)
	{
		OpenFileDialog oSeleccionarArchivo = new OpenFileDialog();
		oSeleccionarArchivo.Filter = "Archivo PDF (*.pdf)|*.pdf";
		oSeleccionarArchivo.Title = "Seleccione el archivo a agregar";
		if (oSeleccionarArchivo.ShowDialog() == DialogResult.OK)
		{
			rutaArchivoAgregar = oSeleccionarArchivo.FileName;
			txtRutaArchivoAgregar.Text = rutaArchivoAgregar;
		}
	}

	private void btnCancelarAgregarPaginas_Click(object sender, EventArgs e)
	{
		pnlAgregarPaginas.Visible = false;
		rutaArchivoAgregar = "";
		txtRutaArchivoAgregar.Clear();
		txtNumeroPaginaParaAgregar.Clear();
	}

	private void btnAgregarPaginasArchivo_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtRutaArchivoAgregar.Text))
		{
			MessageBox.Show("Debe seleccionar un archivo PDF a agregar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		if (string.IsNullOrEmpty(txtNumeroPaginaParaAgregar.Text))
		{
			MessageBox.Show("Debe indicar a partir de que página agregar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		agregarPaginas2(txtRutaArchivoAgregar.Text, Convert.ToInt32(txtNumeroPaginaParaAgregar.Text));
		pnlAgregarPaginas.Visible = false;
		rutaArchivoAgregar = "";
		txtRutaArchivoAgregar.Clear();
		txtNumeroPaginaParaAgregar.Clear();
	}

	private void agregarPaginas(string pRutaArchivoAgregar, int pNumeroPagina)
	{
		string rutaArchivoActual = Path.Combine(txtRutaLote.Text, txtArchivoActual.Text);
		if (pNumeroPagina < 1 || pNumeroPagina > pdfDocument.PageCount)
		{
			throw new ArgumentOutOfRangeException("pNumeroPagina", "Número de página inválido.");
		}
		string tempPath = Path.GetTempFileName() + ".pdf";
		using (FileStream stream = new FileStream(tempPath, FileMode.Create))
		{
			pdfDocument.Save(stream);
		}
		using (PdfSharp.Pdf.PdfDocument outputDocument = PdfReader.Open(tempPath, PdfDocumentOpenMode.Modify))
		{
			PdfSharp.Pdf.PdfDocument documentToAdd = PdfReader.Open(pRutaArchivoAgregar, PdfDocumentOpenMode.Import);
			int insertIndex = pNumeroPagina - 1;
			foreach (PdfPage page in documentToAdd.Pages)
			{
				outputDocument.Pages.Insert(insertIndex, page.Clone() as PdfPage);
				insertIndex++;
			}
			outputDocument.Save(tempPath);
		}
		if (pdfDocument != null)
		{
			pdfDocument.Dispose();
		}
		pdfDocument = PdfiumViewer.PdfDocument.Load(tempPath);
		File.Copy(tempPath, rutaArchivoActual, overwrite: true);
		pdfDocument = PdfiumViewer.PdfDocument.Load(rutaArchivoActual);
		UpdateViewer();
	}

	private void agregarPaginas2(string pRutaArchivoAgregar, int pNumeroPagina)
	{
		string rutaArchivoActual = Path.Combine(txtRutaLote.Text, txtArchivoActual.Text);
		string tempFilePath = Path.GetTempFileName();
		using (PdfSharp.Pdf.PdfDocument documentoActual = PdfReader.Open(rutaArchivoActual, PdfDocumentOpenMode.Import))
		{
			using PdfSharp.Pdf.PdfDocument documentoAgregar = PdfReader.Open(pRutaArchivoAgregar, PdfDocumentOpenMode.Import);
			if (pNumeroPagina < 1 || pNumeroPagina > documentoActual.PageCount + 1)
			{
				MessageBox.Show("Número de página inválido.");
				return;
			}
			using PdfSharp.Pdf.PdfDocument nuevoDocumento = new PdfSharp.Pdf.PdfDocument();
			int paginaActual;
			for (paginaActual = 0; paginaActual < pNumeroPagina - 1; paginaActual++)
			{
				nuevoDocumento.AddPage(documentoActual.Pages[paginaActual]);
			}
			for (int i = 0; i < documentoAgregar.PageCount; i++)
			{
				nuevoDocumento.AddPage(documentoAgregar.Pages[i]);
			}
			for (; paginaActual < documentoActual.PageCount; paginaActual++)
			{
				nuevoDocumento.AddPage(documentoActual.Pages[paginaActual]);
			}
			nuevoDocumento.Save(tempFilePath);
		}
		pdfDocument.Dispose();
		File.Copy(tempFilePath, rutaArchivoActual, overwrite: true);
		pdfDocument = PdfiumViewer.PdfDocument.Load(rutaArchivoActual);
		UpdateViewer();
	}

	private void frmControlCalidad_v3_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (pdfDocument != null)
		{
			pdfDocument.Dispose();
			pdfDocument = null;
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._02_ControlCalidad.frmControlCalidad_v3));
		this.label1 = new System.Windows.Forms.Label();
		this.txtFuncionSeleccionado = new System.Windows.Forms.TextBox();
		this.lboxListaArchivos = new System.Windows.Forms.ListBox();
		this.pnlVisor = new System.Windows.Forms.Panel();
		this.pnlMoverPagina = new System.Windows.Forms.Panel();
		this.label5 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.txtPaginaSelccionadaMover = new System.Windows.Forms.TextBox();
		this.btnCancelarMover = new System.Windows.Forms.Button();
		this.btnMover = new System.Windows.Forms.Button();
		this.txtNumeroPaginaMover = new System.Windows.Forms.TextBox();
		this.lblZoom = new System.Windows.Forms.Label();
		this.gbPaginasMostrar = new System.Windows.Forms.GroupBox();
		this.cbxCantidadPaginas = new System.Windows.Forms.ComboBox();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.pnlLotesDisponibles = new System.Windows.Forms.Panel();
		this.dgvLotesDisponibles = new System.Windows.Forms.DataGridView();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCargarLote = new System.Windows.Forms.Button();
		this.pnlLoteSeleccionado = new System.Windows.Forms.Panel();
		this.btnMarcarLoteCompletado = new System.Windows.Forms.Button();
		this.txtRutaLote = new System.Windows.Forms.TextBox();
		this.txtLoteSeleccionado = new System.Windows.Forms.TextBox();
		this.txtTotalArchivosSeleccionado = new System.Windows.Forms.TextBox();
		this.txtProyectoSeleccionado = new System.Windows.Forms.TextBox();
		this.lblNombreLote = new System.Windows.Forms.Label();
		this.lblCarpetaLote = new System.Windows.Forms.Label();
		this.lblProyeto = new System.Windows.Forms.Label();
		this.lblTotalArchivos = new System.Windows.Forms.Label();
		this.gbNavegacion = new System.Windows.Forms.GroupBox();
		this.btnArchivoSiguiente = new System.Windows.Forms.Button();
		this.btnArchivoAnterior = new System.Windows.Forms.Button();
		this.btnPaginaSiguiente = new System.Windows.Forms.Button();
		this.btnPaginaAnterior = new System.Windows.Forms.Button();
		this.txtTotalPaginas = new System.Windows.Forms.TextBox();
		this.txtArchivoActual = new System.Windows.Forms.TextBox();
		this.btnZoomMenos = new System.Windows.Forms.Button();
		this.btnZoomMas = new System.Windows.Forms.Button();
		this.btnZoom = new System.Windows.Forms.Button();
		this.gbModificacion = new System.Windows.Forms.GroupBox();
		this.btnAgregarPaginas = new System.Windows.Forms.Button();
		this.btnEliminarPaginasBlanco = new System.Windows.Forms.Button();
		this.btnGirarDerecha = new System.Windows.Forms.Button();
		this.btnEliminarPagina = new System.Windows.Forms.Button();
		this.btnMoverPaginas = new System.Windows.Forms.Button();
		this.btnGirarIzquierda = new System.Windows.Forms.Button();
		this.gbZoom = new System.Windows.Forms.GroupBox();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.btnModificarNombre = new System.Windows.Forms.Button();
		this.label2 = new System.Windows.Forms.Label();
		this.pnlModificarNombreArchivo = new System.Windows.Forms.Panel();
		this.btnCancelarModificarNombre = new System.Windows.Forms.Button();
		this.btnModificarNombreArchivo = new System.Windows.Forms.Button();
		this.label3 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.txtNombreArchivoActual = new System.Windows.Forms.TextBox();
		this.txtNuevoNombreArchivo = new System.Windows.Forms.TextBox();
		this.pnlAgregarPaginas = new System.Windows.Forms.Panel();
		this.btnSeleccionarArchivoAgregar = new System.Windows.Forms.Button();
		this.btnCancelarAgregarPaginas = new System.Windows.Forms.Button();
		this.btnAgregarPaginasArchivo = new System.Windows.Forms.Button();
		this.label7 = new System.Windows.Forms.Label();
		this.txtRutaArchivoAgregar = new System.Windows.Forms.TextBox();
		this.txtNumeroPaginaParaAgregar = new System.Windows.Forms.TextBox();
		this.pnlMoverPagina.SuspendLayout();
		this.gbPaginasMostrar.SuspendLayout();
		this.pnlLotesDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).BeginInit();
		this.pnlLoteSeleccionado.SuspendLayout();
		this.gbNavegacion.SuspendLayout();
		this.gbModificacion.SuspendLayout();
		this.gbZoom.SuspendLayout();
		this.groupBox1.SuspendLayout();
		this.pnlModificarNombreArchivo.SuspendLayout();
		this.pnlAgregarPaginas.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(6, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(134, 16);
		this.label1.TabIndex = 0;
		this.label1.Text = "Páginas a mostrar";
		this.txtFuncionSeleccionado.Enabled = false;
		this.txtFuncionSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFuncionSeleccionado.Location = new System.Drawing.Point(508, 63);
		this.txtFuncionSeleccionado.Multiline = true;
		this.txtFuncionSeleccionado.Name = "txtFuncionSeleccionado";
		this.txtFuncionSeleccionado.Size = new System.Drawing.Size(65, 36);
		this.txtFuncionSeleccionado.TabIndex = 10;
		this.txtFuncionSeleccionado.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.txtFuncionSeleccionado.Visible = false;
		this.lboxListaArchivos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lboxListaArchivos.FormattingEnabled = true;
		this.lboxListaArchivos.ItemHeight = 16;
		this.lboxListaArchivos.Location = new System.Drawing.Point(415, 34);
		this.lboxListaArchivos.Name = "lboxListaArchivos";
		this.lboxListaArchivos.Size = new System.Drawing.Size(239, 100);
		this.lboxListaArchivos.TabIndex = 2;
		this.pnlVisor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlVisor.Location = new System.Drawing.Point(12, 313);
		this.pnlVisor.Name = "pnlVisor";
		this.pnlVisor.Size = new System.Drawing.Size(1540, 486);
		this.pnlVisor.TabIndex = 3;
		this.pnlMoverPagina.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlMoverPagina.Controls.Add(this.label5);
		this.pnlMoverPagina.Controls.Add(this.label4);
		this.pnlMoverPagina.Controls.Add(this.txtPaginaSelccionadaMover);
		this.pnlMoverPagina.Controls.Add(this.btnCancelarMover);
		this.pnlMoverPagina.Controls.Add(this.btnMover);
		this.pnlMoverPagina.Controls.Add(this.txtNumeroPaginaMover);
		this.pnlMoverPagina.Location = new System.Drawing.Point(12, 193);
		this.pnlMoverPagina.Name = "pnlMoverPagina";
		this.pnlMoverPagina.Size = new System.Drawing.Size(1539, 114);
		this.pnlMoverPagina.TabIndex = 0;
		this.pnlMoverPagina.Visible = false;
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(637, 46);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(124, 16);
		this.label5.TabIndex = 5;
		this.label5.Text = "Mover después de:";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(637, 21);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(141, 16);
		this.label4.TabIndex = 4;
		this.label4.Text = "Página Seleccionada:";
		this.txtPaginaSelccionadaMover.Enabled = false;
		this.txtPaginaSelccionadaMover.Location = new System.Drawing.Point(780, 21);
		this.txtPaginaSelccionadaMover.Name = "txtPaginaSelccionadaMover";
		this.txtPaginaSelccionadaMover.Size = new System.Drawing.Size(100, 20);
		this.txtPaginaSelccionadaMover.TabIndex = 3;
		this.btnCancelarMover.Location = new System.Drawing.Point(806, 73);
		this.btnCancelarMover.Name = "btnCancelarMover";
		this.btnCancelarMover.Size = new System.Drawing.Size(74, 23);
		this.btnCancelarMover.TabIndex = 2;
		this.btnCancelarMover.Text = "Cancelar";
		this.btnCancelarMover.UseVisualStyleBackColor = true;
		this.btnCancelarMover.Click += new System.EventHandler(btnCancelarMover_Click);
		this.btnMover.Location = new System.Drawing.Point(640, 73);
		this.btnMover.Name = "btnMover";
		this.btnMover.Size = new System.Drawing.Size(142, 23);
		this.btnMover.TabIndex = 1;
		this.btnMover.Text = "Mover";
		this.btnMover.UseVisualStyleBackColor = true;
		this.btnMover.Click += new System.EventHandler(btnMover_Click);
		this.txtNumeroPaginaMover.Location = new System.Drawing.Point(780, 47);
		this.txtNumeroPaginaMover.Name = "txtNumeroPaginaMover";
		this.txtNumeroPaginaMover.Size = new System.Drawing.Size(100, 20);
		this.txtNumeroPaginaMover.TabIndex = 0;
		this.lblZoom.AutoSize = true;
		this.lblZoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblZoom.ForeColor = System.Drawing.Color.White;
		this.lblZoom.Location = new System.Drawing.Point(182, 33);
		this.lblZoom.Name = "lblZoom";
		this.lblZoom.Size = new System.Drawing.Size(36, 16);
		this.lblZoom.TabIndex = 16;
		this.lblZoom.Text = ".......";
		this.gbPaginasMostrar.Controls.Add(this.cbxCantidadPaginas);
		this.gbPaginasMostrar.Controls.Add(this.label1);
		this.gbPaginasMostrar.Location = new System.Drawing.Point(579, 198);
		this.gbPaginasMostrar.Name = "gbPaginasMostrar";
		this.gbPaginasMostrar.Size = new System.Drawing.Size(156, 80);
		this.gbPaginasMostrar.TabIndex = 8;
		this.gbPaginasMostrar.TabStop = false;
		this.cbxCantidadPaginas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxCantidadPaginas.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxCantidadPaginas.FormattingEnabled = true;
		this.cbxCantidadPaginas.Location = new System.Drawing.Point(18, 43);
		this.cbxCantidadPaginas.Name = "cbxCantidadPaginas";
		this.cbxCantidadPaginas.Size = new System.Drawing.Size(112, 25);
		this.cbxCantidadPaginas.TabIndex = 76;
		this.progressBar1.Location = new System.Drawing.Point(12, 284);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(1540, 23);
		this.progressBar1.TabIndex = 9;
		this.pnlLotesDisponibles.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLotesDisponibles.Controls.Add(this.dgvLotesDisponibles);
		this.pnlLotesDisponibles.Location = new System.Drawing.Point(12, 12);
		this.pnlLotesDisponibles.Name = "pnlLotesDisponibles";
		this.pnlLotesDisponibles.Size = new System.Drawing.Size(450, 175);
		this.pnlLotesDisponibles.TabIndex = 57;
		this.dgvLotesDisponibles.AllowUserToAddRows = false;
		this.dgvLotesDisponibles.AllowUserToDeleteRows = false;
		this.dgvLotesDisponibles.AllowUserToResizeRows = false;
		this.dgvLotesDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvLotesDisponibles.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvLotesDisponibles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvLotesDisponibles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvLotesDisponibles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDisponibles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
		this.dgvLotesDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesDisponibles.DefaultCellStyle = dataGridViewCellStyle6;
		this.dgvLotesDisponibles.EnableHeadersVisualStyles = false;
		this.dgvLotesDisponibles.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesDisponibles.Location = new System.Drawing.Point(17, 13);
		this.dgvLotesDisponibles.Name = "dgvLotesDisponibles";
		this.dgvLotesDisponibles.ReadOnly = true;
		this.dgvLotesDisponibles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesDisponibles.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dgvLotesDisponibles.RowHeadersVisible = false;
		this.dgvLotesDisponibles.RowHeadersWidth = 15;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesDisponibles.RowsDefaultCellStyle = dataGridViewCellStyle8;
		this.dgvLotesDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesDisponibles.Size = new System.Drawing.Size(420, 140);
		this.dgvLotesDisponibles.TabIndex = 18;
		this.dgvLotesDisponibles.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvLotesDisponibles_CellContentDoubleClick);
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(468, 159);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(130, 24);
		this.btnCerrar.TabIndex = 60;
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
		this.btnCancelar.Location = new System.Drawing.Point(468, 127);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(130, 25);
		this.btnCancelar.TabIndex = 59;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click_1);
		this.btnCargarLote.BackColor = System.Drawing.Color.SeaGreen;
		this.btnCargarLote.FlatAppearance.BorderSize = 0;
		this.btnCargarLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCargarLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCargarLote.ForeColor = System.Drawing.Color.White;
		this.btnCargarLote.Image = (System.Drawing.Image)resources.GetObject("btnCargarLote.Image");
		this.btnCargarLote.Location = new System.Drawing.Point(468, 23);
		this.btnCargarLote.Name = "btnCargarLote";
		this.btnCargarLote.Size = new System.Drawing.Size(130, 25);
		this.btnCargarLote.TabIndex = 58;
		this.btnCargarLote.Text = "   Cargar Lote";
		this.btnCargarLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCargarLote.UseVisualStyleBackColor = false;
		this.btnCargarLote.Click += new System.EventHandler(btnCargarLotes_Click);
		this.pnlLoteSeleccionado.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlLoteSeleccionado.Controls.Add(this.btnMarcarLoteCompletado);
		this.pnlLoteSeleccionado.Controls.Add(this.lboxListaArchivos);
		this.pnlLoteSeleccionado.Controls.Add(this.txtRutaLote);
		this.pnlLoteSeleccionado.Controls.Add(this.txtLoteSeleccionado);
		this.pnlLoteSeleccionado.Controls.Add(this.txtTotalArchivosSeleccionado);
		this.pnlLoteSeleccionado.Controls.Add(this.txtProyectoSeleccionado);
		this.pnlLoteSeleccionado.Controls.Add(this.lblNombreLote);
		this.pnlLoteSeleccionado.Controls.Add(this.lblCarpetaLote);
		this.pnlLoteSeleccionado.Controls.Add(this.lblProyeto);
		this.pnlLoteSeleccionado.Controls.Add(this.lblTotalArchivos);
		this.pnlLoteSeleccionado.Location = new System.Drawing.Point(890, 14);
		this.pnlLoteSeleccionado.Name = "pnlLoteSeleccionado";
		this.pnlLoteSeleccionado.Size = new System.Drawing.Size(662, 173);
		this.pnlLoteSeleccionado.TabIndex = 58;
		this.pnlLoteSeleccionado.Paint += new System.Windows.Forms.PaintEventHandler(pnlLoteSeleccionado_Paint);
		this.btnMarcarLoteCompletado.BackColor = System.Drawing.Color.SeaGreen;
		this.btnMarcarLoteCompletado.FlatAppearance.BorderSize = 0;
		this.btnMarcarLoteCompletado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnMarcarLoteCompletado.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnMarcarLoteCompletado.ForeColor = System.Drawing.Color.White;
		this.btnMarcarLoteCompletado.Image = (System.Drawing.Image)resources.GetObject("btnMarcarLoteCompletado.Image");
		this.btnMarcarLoteCompletado.Location = new System.Drawing.Point(415, 141);
		this.btnMarcarLoteCompletado.Name = "btnMarcarLoteCompletado";
		this.btnMarcarLoteCompletado.Size = new System.Drawing.Size(236, 25);
		this.btnMarcarLoteCompletado.TabIndex = 76;
		this.btnMarcarLoteCompletado.Text = "   Marcar Completado";
		this.btnMarcarLoteCompletado.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnMarcarLoteCompletado.UseVisualStyleBackColor = false;
		this.btnMarcarLoteCompletado.Click += new System.EventHandler(btnMarcarLoteCompletado_Click);
		this.txtRutaLote.Enabled = false;
		this.txtRutaLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtRutaLote.Location = new System.Drawing.Point(151, 121);
		this.txtRutaLote.Multiline = true;
		this.txtRutaLote.Name = "txtRutaLote";
		this.txtRutaLote.Size = new System.Drawing.Size(248, 45);
		this.txtRutaLote.TabIndex = 20;
		this.txtLoteSeleccionado.Enabled = false;
		this.txtLoteSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtLoteSeleccionado.Location = new System.Drawing.Point(151, 65);
		this.txtLoteSeleccionado.Name = "txtLoteSeleccionado";
		this.txtLoteSeleccionado.Size = new System.Drawing.Size(248, 22);
		this.txtLoteSeleccionado.TabIndex = 18;
		this.txtTotalArchivosSeleccionado.Enabled = false;
		this.txtTotalArchivosSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtTotalArchivosSeleccionado.Location = new System.Drawing.Point(151, 93);
		this.txtTotalArchivosSeleccionado.Name = "txtTotalArchivosSeleccionado";
		this.txtTotalArchivosSeleccionado.Size = new System.Drawing.Size(248, 22);
		this.txtTotalArchivosSeleccionado.TabIndex = 17;
		this.txtProyectoSeleccionado.Enabled = false;
		this.txtProyectoSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtProyectoSeleccionado.Location = new System.Drawing.Point(151, 37);
		this.txtProyectoSeleccionado.Name = "txtProyectoSeleccionado";
		this.txtProyectoSeleccionado.Size = new System.Drawing.Size(248, 22);
		this.txtProyectoSeleccionado.TabIndex = 16;
		this.lblNombreLote.AutoSize = true;
		this.lblNombreLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNombreLote.ForeColor = System.Drawing.Color.White;
		this.lblNombreLote.Location = new System.Drawing.Point(14, 68);
		this.lblNombreLote.Name = "lblNombreLote";
		this.lblNombreLote.Size = new System.Drawing.Size(40, 17);
		this.lblNombreLote.TabIndex = 15;
		this.lblNombreLote.Text = "Lote:";
		this.lblCarpetaLote.AutoSize = true;
		this.lblCarpetaLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblCarpetaLote.ForeColor = System.Drawing.Color.White;
		this.lblCarpetaLote.Location = new System.Drawing.Point(14, 124);
		this.lblCarpetaLote.Name = "lblCarpetaLote";
		this.lblCarpetaLote.Size = new System.Drawing.Size(99, 17);
		this.lblCarpetaLote.TabIndex = 14;
		this.lblCarpetaLote.Text = "Carpeta Lote:";
		this.lblProyeto.AutoSize = true;
		this.lblProyeto.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblProyeto.ForeColor = System.Drawing.Color.White;
		this.lblProyeto.Location = new System.Drawing.Point(14, 40);
		this.lblProyeto.Name = "lblProyeto";
		this.lblProyeto.Size = new System.Drawing.Size(69, 17);
		this.lblProyeto.TabIndex = 11;
		this.lblProyeto.Text = "Proyecto:";
		this.lblTotalArchivos.AutoSize = true;
		this.lblTotalArchivos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblTotalArchivos.ForeColor = System.Drawing.Color.White;
		this.lblTotalArchivos.Location = new System.Drawing.Point(14, 96);
		this.lblTotalArchivos.Name = "lblTotalArchivos";
		this.lblTotalArchivos.Size = new System.Drawing.Size(122, 17);
		this.lblTotalArchivos.TabIndex = 12;
		this.lblTotalArchivos.Text = "Total de Archivos:";
		this.gbNavegacion.Controls.Add(this.btnArchivoSiguiente);
		this.gbNavegacion.Controls.Add(this.btnArchivoAnterior);
		this.gbNavegacion.Controls.Add(this.btnPaginaSiguiente);
		this.gbNavegacion.Controls.Add(this.btnPaginaAnterior);
		this.gbNavegacion.Location = new System.Drawing.Point(13, 198);
		this.gbNavegacion.Name = "gbNavegacion";
		this.gbNavegacion.Size = new System.Drawing.Size(300, 80);
		this.gbNavegacion.TabIndex = 61;
		this.gbNavegacion.TabStop = false;
		this.btnArchivoSiguiente.BackColor = System.Drawing.Color.SeaGreen;
		this.btnArchivoSiguiente.FlatAppearance.BorderSize = 0;
		this.btnArchivoSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnArchivoSiguiente.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnArchivoSiguiente.ForeColor = System.Drawing.Color.White;
		this.btnArchivoSiguiente.Image = (System.Drawing.Image)resources.GetObject("btnArchivoSiguiente.Image");
		this.btnArchivoSiguiente.Location = new System.Drawing.Point(239, 19);
		this.btnArchivoSiguiente.Name = "btnArchivoSiguiente";
		this.btnArchivoSiguiente.Size = new System.Drawing.Size(50, 50);
		this.btnArchivoSiguiente.TabIndex = 64;
		this.btnArchivoSiguiente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnArchivoSiguiente.UseVisualStyleBackColor = false;
		this.btnArchivoSiguiente.Click += new System.EventHandler(btnArchivoSiguiente_Click_1);
		this.btnArchivoAnterior.BackColor = System.Drawing.Color.SeaGreen;
		this.btnArchivoAnterior.FlatAppearance.BorderSize = 0;
		this.btnArchivoAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnArchivoAnterior.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnArchivoAnterior.ForeColor = System.Drawing.Color.White;
		this.btnArchivoAnterior.Image = (System.Drawing.Image)resources.GetObject("btnArchivoAnterior.Image");
		this.btnArchivoAnterior.Location = new System.Drawing.Point(6, 19);
		this.btnArchivoAnterior.Name = "btnArchivoAnterior";
		this.btnArchivoAnterior.Size = new System.Drawing.Size(50, 50);
		this.btnArchivoAnterior.TabIndex = 63;
		this.btnArchivoAnterior.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnArchivoAnterior.UseVisualStyleBackColor = false;
		this.btnArchivoAnterior.Click += new System.EventHandler(btnArchivoAnterior_Click_1);
		this.btnPaginaSiguiente.BackColor = System.Drawing.Color.SeaGreen;
		this.btnPaginaSiguiente.FlatAppearance.BorderSize = 0;
		this.btnPaginaSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnPaginaSiguiente.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnPaginaSiguiente.ForeColor = System.Drawing.Color.White;
		this.btnPaginaSiguiente.Image = (System.Drawing.Image)resources.GetObject("btnPaginaSiguiente.Image");
		this.btnPaginaSiguiente.Location = new System.Drawing.Point(153, 19);
		this.btnPaginaSiguiente.Name = "btnPaginaSiguiente";
		this.btnPaginaSiguiente.Size = new System.Drawing.Size(75, 50);
		this.btnPaginaSiguiente.TabIndex = 66;
		this.btnPaginaSiguiente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnPaginaSiguiente.UseVisualStyleBackColor = false;
		this.btnPaginaSiguiente.Click += new System.EventHandler(btnPaginaSiguiente_Click_1);
		this.btnPaginaAnterior.BackColor = System.Drawing.Color.SeaGreen;
		this.btnPaginaAnterior.FlatAppearance.BorderSize = 0;
		this.btnPaginaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnPaginaAnterior.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnPaginaAnterior.ForeColor = System.Drawing.Color.White;
		this.btnPaginaAnterior.Image = (System.Drawing.Image)resources.GetObject("btnPaginaAnterior.Image");
		this.btnPaginaAnterior.Location = new System.Drawing.Point(67, 19);
		this.btnPaginaAnterior.Name = "btnPaginaAnterior";
		this.btnPaginaAnterior.Size = new System.Drawing.Size(75, 50);
		this.btnPaginaAnterior.TabIndex = 65;
		this.btnPaginaAnterior.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnPaginaAnterior.UseVisualStyleBackColor = false;
		this.btnPaginaAnterior.Click += new System.EventHandler(btnPaginaAnterior_Click_1);
		this.txtTotalPaginas.Enabled = false;
		this.txtTotalPaginas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtTotalPaginas.Location = new System.Drawing.Point(139, 48);
		this.txtTotalPaginas.Name = "txtTotalPaginas";
		this.txtTotalPaginas.Size = new System.Drawing.Size(67, 26);
		this.txtTotalPaginas.TabIndex = 67;
		this.txtTotalPaginas.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.txtArchivoActual.Enabled = false;
		this.txtArchivoActual.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtArchivoActual.Location = new System.Drawing.Point(6, 18);
		this.txtArchivoActual.Name = "txtArchivoActual";
		this.txtArchivoActual.Size = new System.Drawing.Size(231, 26);
		this.txtArchivoActual.TabIndex = 21;
		this.txtArchivoActual.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.btnZoomMenos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnZoomMenos.FlatAppearance.BorderSize = 0;
		this.btnZoomMenos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnZoomMenos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnZoomMenos.ForeColor = System.Drawing.Color.White;
		this.btnZoomMenos.Image = (System.Drawing.Image)resources.GetObject("btnZoomMenos.Image");
		this.btnZoomMenos.Location = new System.Drawing.Point(126, 18);
		this.btnZoomMenos.Name = "btnZoomMenos";
		this.btnZoomMenos.Size = new System.Drawing.Size(50, 50);
		this.btnZoomMenos.TabIndex = 74;
		this.btnZoomMenos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnZoomMenos.UseVisualStyleBackColor = false;
		this.btnZoomMenos.Click += new System.EventHandler(btnZoomMenos_Click_1);
		this.btnZoomMas.BackColor = System.Drawing.Color.SeaGreen;
		this.btnZoomMas.FlatAppearance.BorderSize = 0;
		this.btnZoomMas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnZoomMas.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnZoomMas.ForeColor = System.Drawing.Color.White;
		this.btnZoomMas.Image = (System.Drawing.Image)resources.GetObject("btnZoomMas.Image");
		this.btnZoomMas.Location = new System.Drawing.Point(70, 18);
		this.btnZoomMas.Name = "btnZoomMas";
		this.btnZoomMas.Size = new System.Drawing.Size(50, 50);
		this.btnZoomMas.TabIndex = 73;
		this.btnZoomMas.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnZoomMas.UseVisualStyleBackColor = false;
		this.btnZoomMas.Click += new System.EventHandler(btnZoomMas_Click_1);
		this.btnZoom.BackColor = System.Drawing.Color.SeaGreen;
		this.btnZoom.FlatAppearance.BorderSize = 0;
		this.btnZoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnZoom.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnZoom.ForeColor = System.Drawing.Color.White;
		this.btnZoom.Image = (System.Drawing.Image)resources.GetObject("btnZoom.Image");
		this.btnZoom.Location = new System.Drawing.Point(6, 18);
		this.btnZoom.Name = "btnZoom";
		this.btnZoom.Size = new System.Drawing.Size(50, 50);
		this.btnZoom.TabIndex = 68;
		this.btnZoom.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnZoom.UseVisualStyleBackColor = false;
		this.btnZoom.Click += new System.EventHandler(btnZoom_Click);
		this.gbModificacion.Controls.Add(this.btnAgregarPaginas);
		this.gbModificacion.Controls.Add(this.btnEliminarPaginasBlanco);
		this.gbModificacion.Controls.Add(this.btnGirarDerecha);
		this.gbModificacion.Controls.Add(this.btnEliminarPagina);
		this.gbModificacion.Controls.Add(this.btnMoverPaginas);
		this.gbModificacion.Controls.Add(this.btnGirarIzquierda);
		this.gbModificacion.Location = new System.Drawing.Point(1022, 198);
		this.gbModificacion.Name = "gbModificacion";
		this.gbModificacion.Size = new System.Drawing.Size(530, 80);
		this.gbModificacion.TabIndex = 62;
		this.gbModificacion.TabStop = false;
		this.btnAgregarPaginas.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarPaginas.FlatAppearance.BorderSize = 0;
		this.btnAgregarPaginas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarPaginas.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarPaginas.ForeColor = System.Drawing.Color.White;
		this.btnAgregarPaginas.Image = (System.Drawing.Image)resources.GetObject("btnAgregarPaginas.Image");
		this.btnAgregarPaginas.Location = new System.Drawing.Point(230, 18);
		this.btnAgregarPaginas.Name = "btnAgregarPaginas";
		this.btnAgregarPaginas.Size = new System.Drawing.Size(50, 50);
		this.btnAgregarPaginas.TabIndex = 75;
		this.btnAgregarPaginas.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarPaginas.UseVisualStyleBackColor = false;
		this.btnAgregarPaginas.Click += new System.EventHandler(btnAgregarPaginas_Click);
		this.btnEliminarPaginasBlanco.BackColor = System.Drawing.Color.SeaGreen;
		this.btnEliminarPaginasBlanco.FlatAppearance.BorderSize = 0;
		this.btnEliminarPaginasBlanco.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnEliminarPaginasBlanco.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnEliminarPaginasBlanco.ForeColor = System.Drawing.Color.White;
		this.btnEliminarPaginasBlanco.Image = (System.Drawing.Image)resources.GetObject("btnEliminarPaginasBlanco.Image");
		this.btnEliminarPaginasBlanco.Location = new System.Drawing.Point(303, 19);
		this.btnEliminarPaginasBlanco.Name = "btnEliminarPaginasBlanco";
		this.btnEliminarPaginasBlanco.Size = new System.Drawing.Size(219, 50);
		this.btnEliminarPaginasBlanco.TabIndex = 74;
		this.btnEliminarPaginasBlanco.Text = " Eliminar Páginas en Blanco";
		this.btnEliminarPaginasBlanco.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnEliminarPaginasBlanco.UseVisualStyleBackColor = false;
		this.btnEliminarPaginasBlanco.Click += new System.EventHandler(btnEliminarPaginasBlanco_Click_1);
		this.btnGirarDerecha.BackColor = System.Drawing.Color.SeaGreen;
		this.btnGirarDerecha.FlatAppearance.BorderSize = 0;
		this.btnGirarDerecha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnGirarDerecha.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnGirarDerecha.ForeColor = System.Drawing.Color.White;
		this.btnGirarDerecha.Image = (System.Drawing.Image)resources.GetObject("btnGirarDerecha.Image");
		this.btnGirarDerecha.Location = new System.Drawing.Point(62, 19);
		this.btnGirarDerecha.Name = "btnGirarDerecha";
		this.btnGirarDerecha.Size = new System.Drawing.Size(50, 50);
		this.btnGirarDerecha.TabIndex = 73;
		this.btnGirarDerecha.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnGirarDerecha.UseVisualStyleBackColor = false;
		this.btnGirarDerecha.Click += new System.EventHandler(btnGirarDerecha_Click_1);
		this.btnEliminarPagina.BackColor = System.Drawing.Color.SeaGreen;
		this.btnEliminarPagina.FlatAppearance.BorderSize = 0;
		this.btnEliminarPagina.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnEliminarPagina.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnEliminarPagina.ForeColor = System.Drawing.Color.White;
		this.btnEliminarPagina.Image = (System.Drawing.Image)resources.GetObject("btnEliminarPagina.Image");
		this.btnEliminarPagina.Location = new System.Drawing.Point(174, 18);
		this.btnEliminarPagina.Name = "btnEliminarPagina";
		this.btnEliminarPagina.Size = new System.Drawing.Size(50, 50);
		this.btnEliminarPagina.TabIndex = 72;
		this.btnEliminarPagina.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnEliminarPagina.UseVisualStyleBackColor = false;
		this.btnEliminarPagina.Click += new System.EventHandler(btnEliminarPagina_Click);
		this.btnMoverPaginas.BackColor = System.Drawing.Color.SeaGreen;
		this.btnMoverPaginas.FlatAppearance.BorderSize = 0;
		this.btnMoverPaginas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnMoverPaginas.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnMoverPaginas.ForeColor = System.Drawing.Color.White;
		this.btnMoverPaginas.Image = (System.Drawing.Image)resources.GetObject("btnMoverPaginas.Image");
		this.btnMoverPaginas.Location = new System.Drawing.Point(118, 18);
		this.btnMoverPaginas.Name = "btnMoverPaginas";
		this.btnMoverPaginas.Size = new System.Drawing.Size(50, 50);
		this.btnMoverPaginas.TabIndex = 71;
		this.btnMoverPaginas.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnMoverPaginas.UseVisualStyleBackColor = false;
		this.btnMoverPaginas.Click += new System.EventHandler(btnMoverPaginas_Click_1);
		this.btnGirarIzquierda.BackColor = System.Drawing.Color.SeaGreen;
		this.btnGirarIzquierda.FlatAppearance.BorderSize = 0;
		this.btnGirarIzquierda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnGirarIzquierda.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnGirarIzquierda.ForeColor = System.Drawing.Color.White;
		this.btnGirarIzquierda.Image = (System.Drawing.Image)resources.GetObject("btnGirarIzquierda.Image");
		this.btnGirarIzquierda.Location = new System.Drawing.Point(6, 18);
		this.btnGirarIzquierda.Name = "btnGirarIzquierda";
		this.btnGirarIzquierda.Size = new System.Drawing.Size(50, 50);
		this.btnGirarIzquierda.TabIndex = 70;
		this.btnGirarIzquierda.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnGirarIzquierda.UseVisualStyleBackColor = false;
		this.btnGirarIzquierda.Click += new System.EventHandler(btnGirarIzquierda_Click_1);
		this.gbZoom.Controls.Add(this.btnZoom);
		this.gbZoom.Controls.Add(this.lblZoom);
		this.gbZoom.Controls.Add(this.btnZoomMas);
		this.gbZoom.Controls.Add(this.btnZoomMenos);
		this.gbZoom.Location = new System.Drawing.Point(741, 198);
		this.gbZoom.Name = "gbZoom";
		this.gbZoom.Size = new System.Drawing.Size(275, 80);
		this.gbZoom.TabIndex = 75;
		this.gbZoom.TabStop = false;
		this.groupBox1.Controls.Add(this.btnModificarNombre);
		this.groupBox1.Controls.Add(this.label2);
		this.groupBox1.Controls.Add(this.txtArchivoActual);
		this.groupBox1.Controls.Add(this.txtTotalPaginas);
		this.groupBox1.Location = new System.Drawing.Point(319, 198);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(254, 80);
		this.groupBox1.TabIndex = 67;
		this.groupBox1.TabStop = false;
		this.groupBox1.Enter += new System.EventHandler(groupBox1_Enter);
		this.btnModificarNombre.BackColor = System.Drawing.Color.SeaGreen;
		this.btnModificarNombre.FlatAppearance.BorderSize = 0;
		this.btnModificarNombre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnModificarNombre.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnModificarNombre.ForeColor = System.Drawing.Color.White;
		this.btnModificarNombre.Image = (System.Drawing.Image)resources.GetObject("btnModificarNombre.Image");
		this.btnModificarNombre.Location = new System.Drawing.Point(212, 48);
		this.btnModificarNombre.Name = "btnModificarNombre";
		this.btnModificarNombre.Size = new System.Drawing.Size(25, 25);
		this.btnModificarNombre.TabIndex = 76;
		this.btnModificarNombre.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnModificarNombre.UseVisualStyleBackColor = false;
		this.btnModificarNombre.Click += new System.EventHandler(btnModificarNombre_Click);
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(6, 52);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(127, 16);
		this.label2.TabIndex = 77;
		this.label2.Text = "Total de Páginas";
		this.pnlModificarNombreArchivo.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlModificarNombreArchivo.Controls.Add(this.btnCancelarModificarNombre);
		this.pnlModificarNombreArchivo.Controls.Add(this.btnModificarNombreArchivo);
		this.pnlModificarNombreArchivo.Controls.Add(this.label3);
		this.pnlModificarNombreArchivo.Controls.Add(this.label6);
		this.pnlModificarNombreArchivo.Controls.Add(this.txtNombreArchivoActual);
		this.pnlModificarNombreArchivo.Controls.Add(this.txtNuevoNombreArchivo);
		this.pnlModificarNombreArchivo.Location = new System.Drawing.Point(12, 193);
		this.pnlModificarNombreArchivo.Name = "pnlModificarNombreArchivo";
		this.pnlModificarNombreArchivo.Size = new System.Drawing.Size(1539, 114);
		this.pnlModificarNombreArchivo.TabIndex = 6;
		this.pnlModificarNombreArchivo.Visible = false;
		this.btnCancelarModificarNombre.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelarModificarNombre.FlatAppearance.BorderSize = 0;
		this.btnCancelarModificarNombre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelarModificarNombre.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelarModificarNombre.ForeColor = System.Drawing.Color.White;
		this.btnCancelarModificarNombre.Image = (System.Drawing.Image)resources.GetObject("btnCancelarModificarNombre.Image");
		this.btnCancelarModificarNombre.Location = new System.Drawing.Point(791, 73);
		this.btnCancelarModificarNombre.Name = "btnCancelarModificarNombre";
		this.btnCancelarModificarNombre.Size = new System.Drawing.Size(130, 25);
		this.btnCancelarModificarNombre.TabIndex = 76;
		this.btnCancelarModificarNombre.Text = "   Cancelar";
		this.btnCancelarModificarNombre.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelarModificarNombre.UseVisualStyleBackColor = false;
		this.btnCancelarModificarNombre.Click += new System.EventHandler(btnCancelarModificarNombre_Click);
		this.btnModificarNombreArchivo.BackColor = System.Drawing.Color.SeaGreen;
		this.btnModificarNombreArchivo.FlatAppearance.BorderSize = 0;
		this.btnModificarNombreArchivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnModificarNombreArchivo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnModificarNombreArchivo.ForeColor = System.Drawing.Color.White;
		this.btnModificarNombreArchivo.Image = (System.Drawing.Image)resources.GetObject("btnModificarNombreArchivo.Image");
		this.btnModificarNombreArchivo.Location = new System.Drawing.Point(589, 73);
		this.btnModificarNombreArchivo.Name = "btnModificarNombreArchivo";
		this.btnModificarNombreArchivo.Size = new System.Drawing.Size(175, 25);
		this.btnModificarNombreArchivo.TabIndex = 76;
		this.btnModificarNombreArchivo.Text = "   Modificar Nombre";
		this.btnModificarNombreArchivo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnModificarNombreArchivo.UseVisualStyleBackColor = false;
		this.btnModificarNombreArchivo.Click += new System.EventHandler(btnModificarNombreArchivo_Click);
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(586, 46);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(170, 16);
		this.label3.TabIndex = 5;
		this.label3.Text = "Nuevo Nombre de Archivo:";
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.ForeColor = System.Drawing.Color.White;
		this.label6.Location = new System.Drawing.Point(608, 18);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(148, 16);
		this.label6.TabIndex = 4;
		this.label6.Text = "Nombre Archivo Actual:";
		this.txtNombreArchivoActual.Enabled = false;
		this.txtNombreArchivoActual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNombreArchivoActual.Location = new System.Drawing.Point(771, 15);
		this.txtNombreArchivoActual.Name = "txtNombreArchivoActual";
		this.txtNombreArchivoActual.Size = new System.Drawing.Size(150, 22);
		this.txtNombreArchivoActual.TabIndex = 3;
		this.txtNombreArchivoActual.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.txtNuevoNombreArchivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNuevoNombreArchivo.Location = new System.Drawing.Point(771, 43);
		this.txtNuevoNombreArchivo.Name = "txtNuevoNombreArchivo";
		this.txtNuevoNombreArchivo.Size = new System.Drawing.Size(150, 22);
		this.txtNuevoNombreArchivo.TabIndex = 0;
		this.pnlAgregarPaginas.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlAgregarPaginas.Controls.Add(this.btnSeleccionarArchivoAgregar);
		this.pnlAgregarPaginas.Controls.Add(this.btnCancelarAgregarPaginas);
		this.pnlAgregarPaginas.Controls.Add(this.btnAgregarPaginasArchivo);
		this.pnlAgregarPaginas.Controls.Add(this.label7);
		this.pnlAgregarPaginas.Controls.Add(this.txtRutaArchivoAgregar);
		this.pnlAgregarPaginas.Controls.Add(this.txtNumeroPaginaParaAgregar);
		this.pnlAgregarPaginas.Location = new System.Drawing.Point(11, 193);
		this.pnlAgregarPaginas.Name = "pnlAgregarPaginas";
		this.pnlAgregarPaginas.Size = new System.Drawing.Size(1561, 114);
		this.pnlAgregarPaginas.TabIndex = 77;
		this.pnlAgregarPaginas.Visible = false;
		this.btnSeleccionarArchivoAgregar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarArchivoAgregar.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarArchivoAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarArchivoAgregar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarArchivoAgregar.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarArchivoAgregar.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarArchivoAgregar.Image");
		this.btnSeleccionarArchivoAgregar.Location = new System.Drawing.Point(429, 14);
		this.btnSeleccionarArchivoAgregar.Name = "btnSeleccionarArchivoAgregar";
		this.btnSeleccionarArchivoAgregar.Size = new System.Drawing.Size(175, 25);
		this.btnSeleccionarArchivoAgregar.TabIndex = 77;
		this.btnSeleccionarArchivoAgregar.Text = "   Seleccionar Archivo";
		this.btnSeleccionarArchivoAgregar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarArchivoAgregar.UseVisualStyleBackColor = false;
		this.btnSeleccionarArchivoAgregar.Click += new System.EventHandler(btnSeleccionarArchivoAgregar_Click);
		this.btnCancelarAgregarPaginas.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelarAgregarPaginas.FlatAppearance.BorderSize = 0;
		this.btnCancelarAgregarPaginas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelarAgregarPaginas.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelarAgregarPaginas.ForeColor = System.Drawing.Color.White;
		this.btnCancelarAgregarPaginas.Image = (System.Drawing.Image)resources.GetObject("btnCancelarAgregarPaginas.Image");
		this.btnCancelarAgregarPaginas.Location = new System.Drawing.Point(791, 73);
		this.btnCancelarAgregarPaginas.Name = "btnCancelarAgregarPaginas";
		this.btnCancelarAgregarPaginas.Size = new System.Drawing.Size(130, 25);
		this.btnCancelarAgregarPaginas.TabIndex = 76;
		this.btnCancelarAgregarPaginas.Text = "   Cancelar";
		this.btnCancelarAgregarPaginas.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelarAgregarPaginas.UseVisualStyleBackColor = false;
		this.btnCancelarAgregarPaginas.Click += new System.EventHandler(btnCancelarAgregarPaginas_Click);
		this.btnAgregarPaginasArchivo.BackColor = System.Drawing.Color.SeaGreen;
		this.btnAgregarPaginasArchivo.FlatAppearance.BorderSize = 0;
		this.btnAgregarPaginasArchivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAgregarPaginasArchivo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnAgregarPaginasArchivo.ForeColor = System.Drawing.Color.White;
		this.btnAgregarPaginasArchivo.Image = (System.Drawing.Image)resources.GetObject("btnAgregarPaginasArchivo.Image");
		this.btnAgregarPaginasArchivo.Location = new System.Drawing.Point(589, 73);
		this.btnAgregarPaginasArchivo.Name = "btnAgregarPaginasArchivo";
		this.btnAgregarPaginasArchivo.Size = new System.Drawing.Size(175, 25);
		this.btnAgregarPaginasArchivo.TabIndex = 76;
		this.btnAgregarPaginasArchivo.Text = "   Agregar Archivo";
		this.btnAgregarPaginasArchivo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnAgregarPaginasArchivo.UseVisualStyleBackColor = false;
		this.btnAgregarPaginasArchivo.Click += new System.EventHandler(btnAgregarPaginasArchivo_Click);
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.ForeColor = System.Drawing.Color.White;
		this.label7.Location = new System.Drawing.Point(567, 46);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(189, 16);
		this.label7.TabIndex = 5;
		this.label7.Text = "Insertar después de la página:";
		this.txtRutaArchivoAgregar.Enabled = false;
		this.txtRutaArchivoAgregar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtRutaArchivoAgregar.Location = new System.Drawing.Point(612, 15);
		this.txtRutaArchivoAgregar.Name = "txtRutaArchivoAgregar";
		this.txtRutaArchivoAgregar.Size = new System.Drawing.Size(421, 22);
		this.txtRutaArchivoAgregar.TabIndex = 3;
		this.txtRutaArchivoAgregar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.txtNumeroPaginaParaAgregar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNumeroPaginaParaAgregar.Location = new System.Drawing.Point(771, 43);
		this.txtNumeroPaginaParaAgregar.Name = "txtNumeroPaginaParaAgregar";
		this.txtNumeroPaginaParaAgregar.Size = new System.Drawing.Size(150, 22);
		this.txtNumeroPaginaParaAgregar.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1584, 811);
		base.Controls.Add(this.pnlAgregarPaginas);
		base.Controls.Add(this.pnlMoverPagina);
		base.Controls.Add(this.pnlModificarNombreArchivo);
		base.Controls.Add(this.txtFuncionSeleccionado);
		base.Controls.Add(this.gbZoom);
		base.Controls.Add(this.gbModificacion);
		base.Controls.Add(this.gbNavegacion);
		base.Controls.Add(this.pnlLoteSeleccionado);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnCargarLote);
		base.Controls.Add(this.pnlLotesDisponibles);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.gbPaginasMostrar);
		base.Controls.Add(this.pnlVisor);
		base.Controls.Add(this.groupBox1);
		base.MaximizeBox = false;
		base.Name = "frmControlCalidad_v3";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Control de Calidad";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmControlCalidad_v3_FormClosing);
		base.Load += new System.EventHandler(frmControlCalidad_v2_Load);
		base.Resize += new System.EventHandler(frmControlCalidad_v3_Resize);
		this.pnlMoverPagina.ResumeLayout(false);
		this.pnlMoverPagina.PerformLayout();
		this.gbPaginasMostrar.ResumeLayout(false);
		this.gbPaginasMostrar.PerformLayout();
		this.pnlLotesDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).EndInit();
		this.pnlLoteSeleccionado.ResumeLayout(false);
		this.pnlLoteSeleccionado.PerformLayout();
		this.gbNavegacion.ResumeLayout(false);
		this.gbModificacion.ResumeLayout(false);
		this.gbZoom.ResumeLayout(false);
		this.gbZoom.PerformLayout();
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.pnlModificarNombreArchivo.ResumeLayout(false);
		this.pnlModificarNombreArchivo.PerformLayout();
		this.pnlAgregarPaginas.ResumeLayout(false);
		this.pnlAgregarPaginas.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
