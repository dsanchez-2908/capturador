using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;
using PdfiumViewer;

namespace Capturador._01_Pantallas._04_Separador;

public class frmDespachoSeparar_v2 : Form
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

	private List<eCodigosBarrasEncontrados_v2> listaCodigoBarraFinal = new List<eCodigosBarrasEncontrados_v2>();

	private string nombreCarpeta;

	private IContainer components = null;

	private GroupBox gbSeleccionarCarpetaOrigen;

	private TextBox txtRutaCarpetaOrigen;

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

	private Button btnSeleccionarCarpetaOrigen;

	private DataGridView dgvTotalLotesEncontrados;

	private Button btnBuscarLotes;

	private DataGridView dgvIndicesEncontrados;

	private Button btnBuscarSeparadores;

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

	public frmDespachoSeparar_v2(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
		base.KeyPreview = true;
		base.KeyDown += frmDespachoSeparar_v2_KeyDown;
	}

	private void frmDespachoSeparar_v2_Load(object sender, EventArgs e)
	{
		eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		rutaCarpetaSalida = oProyectoConfiguracion.dsRutaSalida;
		txtRutaCarpetaOrigen.Text = rutaCarpetaInicial;
		txtCarpetaSalida.Text = rutaCarpetaSalida;
		rutaLoteOrigen = rutaCarpetaInicial;
		ajustarFormulario_SeleccionarCarpeta();
	}

	private void btnSeleccionarCarpetaOrigen_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLote = new FolderBrowserDialog();
		oSeleccionarLote.Description = "Seleccionar la carpeta a procesar";
		oSeleccionarLote.SelectedPath = rutaCarpetaInicial;
		oSeleccionarLote.ShowNewFolderButton = false;
		if (oSeleccionarLote.ShowDialog() == DialogResult.OK)
		{
			rutaLoteOrigen = oSeleccionarLote.SelectedPath;
			txtRutaCarpetaOrigen.Text = rutaLoteOrigen;
			nConfiguracion.actualizarUltimaCarpetaOrigen(oUsuarioLogueado, 1, rutaCarpetaInicial, rutaLoteOrigen);
			btnBuscarLotes.Enabled = false;
			btnBuscarLotes.BackColor = Color.DarkGray;
			btnSeleccionarCarpetaOrigen.Enabled = false;
			btnSeleccionarCarpetaOrigen.BackColor = Color.DarkGray;
			buscarLotes();
			ajustarFormulario_BuscarSeparadores();
		}
	}

	private void btnBuscarLotes_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtRutaCarpetaOrigen.Text))
		{
			MessageBox.Show("Debe seleccionar una carpeta de origen", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		rutaLoteOrigen = txtRutaCarpetaOrigen.Text;
		buscarLotes();
		btnSeleccionarCarpetaOrigen.Enabled = true;
		btnSeleccionarCarpetaOrigen.BackColor = Color.DarkGray;
		btnBuscarLotes.Enabled = true;
		btnBuscarLotes.BackColor = Color.DarkGray;
		ajustarFormulario_BuscarSeparadores();
	}

	private void buscarLotes()
	{
		dgvIndicesEncontrados.DataSource = nArchivos.cargarArchivoIndice2(rutaLoteOrigen, nombreArchivoIndice);
		dgvIndicesEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		cantidadTotalDespachos = dgvIndicesEncontrados.RowCount;
		dgvTotalLotesEncontrados.DataSource = cargarTablaTotales();
		dgvTotalLotesEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
		dgvSeparadoresEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		dgvSeparadoresEncontrados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		OrdenarDgvSeparadores();
		LoadPdfFiles(rutaLoteOrigen);
		btnBuscarSeparadores.Enabled = false;
		btnBuscarSeparadores.BackColor = Color.DarkGray;
		ajustarFormulario_Procesar();
	}

	private async Task buscarCodigosBarraAsync()
	{
		int cantidadArchivos = dgvIndicesEncontrados.Rows.Count;
		progressBar1.Maximum = cantidadArchivos;
		progressBar1.Value = 0;
		int maxGradoParalelismo = 4;
		SemaphoreSlim semaphore = new SemaphoreSlim(maxGradoParalelismo);
		try
		{
			List<Task> tareas = new List<Task>();
			object lockLista = new object();
			object lockProgress = new object();
			int progreso = 0;
			foreach (DataGridViewRow row in (IEnumerable)dgvIndicesEncontrados.Rows)
			{
				await semaphore.WaitAsync();
				tareas.Add(Task.Run(async delegate
				{
					try
					{
						eDespacho oDespacho = new eDespacho
						{
							id = Convert.ToInt32(row.Cells[0].Value.ToString()),
							dsDespacho = row.Cells[1].Value.ToString(),
							cdSerieDocumental = row.Cells[2].Value.ToString(),
							nuSIGEA = row.Cells[3].Value.ToString(),
							nuGuia = row.Cells[4].Value.ToString(),
							dsUsuarioDigitalizacion = row.Cells[5].Value.ToString(),
							dsNombreLote = row.Cells[6].Value.ToString(),
							dsRutaArchivoPDF = row.Cells[7].Value.ToString()
						};
						List<eCodigosBarrasEncontrados_v2> resultado = await Task.Run(() => nCodigoBarras_v5.buscarCodigoBarras(oUsuarioLogueado, oDespacho));
						lock (lockLista)
						{
							listaCodigoBarraFinal.AddRange(resultado);
						}
						lock (lockProgress)
						{
							int num = progreso;
							progreso = num + 1;
							if (progreso % 2 == 0 || progreso == cantidadArchivos)
							{
								Invoke((Action)delegate
								{
									progressBar1.Value = progreso;
								});
							}
						}
					}
					catch (Exception ex)
					{
						Exception ex2 = ex;
						Console.WriteLine("Error procesando despacho: " + ex2.Message);
					}
					finally
					{
						semaphore.Release();
						GC.Collect();
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
		pdfFiles = Directory.GetFiles(folderPath, "*.pdf", SearchOption.AllDirectories).ToList();
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
			txtRutaCarpetaOrigen.Clear();
		}
	}

	private void LoadPdf(string filePath)
	{
		pdfDocument?.Dispose();
		pdfDocument = PdfDocument.Load(filePath);
		currentPage = 0;
		hasChanges = false;
		txtArchivoActual.Text = Path.GetFileName(filePath);
		txtTotalPaginas.Text = pdfDocument.PageCount.ToString();
		GC.Collect();
		UpdateViewer();
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
		pagesPerView = 12;
		foreach (Control control in pnlVisor.Controls)
		{
			if (control is PictureBox pictureBox)
			{
				pictureBox.Image?.Dispose();
				pictureBox.Dispose();
			}
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
		string archivoActual = Path.GetFileNameWithoutExtension(txtArchivoActual.Text);
		for (int i = 0; i < pagesPerView; i++)
		{
			int pageIndex = currentPage + i;
			if (pageIndex >= pdfDocument.PageCount)
			{
				break;
			}
			PictureBox pictureBox2 = new PictureBox
			{
				Width = pnlVisor.Width / columns,
				Height = pnlVisor.Height / rows - 40,
				Dock = DockStyle.None,
				SizeMode = PictureBoxSizeMode.Zoom,
				BorderStyle = BorderStyle.FixedSingle
			};
			using (Image bitmap = pdfDocument.Render(pageIndex, 5f, 5f, forPrinting: true))
			{
				pictureBox2.Image = new Bitmap(bitmap);
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
				TextAlign = ContentAlignment.TopCenter
			};
			foreach (DataGridViewRow row in (IEnumerable)dgvSeparadoresEncontrados.Rows)
			{
				if (row.Cells[1].Value?.ToString() == archivoActual && int.TryParse(row.Cells[2].Value?.ToString(), out var paginaDgv) && paginaDgv == pageIndex + 1)
				{
					chkSeleccionar.Checked = true;
					break;
				}
			}
			Panel panel = new Panel
			{
				Width = pictureBox2.Width,
				Height = pictureBox2.Height + lblPagina.Height + chkSeleccionar.Height,
				BorderStyle = BorderStyle.None
			};
			panel.Controls.Add(chkSeleccionar);
			panel.Controls.Add(pictureBox2);
			panel.Controls.Add(lblPagina);
			pnlVisor.Controls.Add(panel);
			pictureBox2.MouseClick += PdfViewer_MouseClick;
			pictureBox2.AllowDrop = true;
			panel.Left = i % columns * (pnlVisor.Width / columns);
			panel.Top = i / columns * (pnlVisor.Height / rows);
		}
		GC.Collect();
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
		List<eCodigosBarrasEncontrados_v2> listaActual = dgvSeparadoresEncontrados.DataSource as List<eCodigosBarrasEncontrados_v2>;
		if (listaActual == null)
		{
			listaActual = new List<eCodigosBarrasEncontrados_v2>();
		}
		listaActual.Add(new eCodigosBarrasEncontrados_v2
		{
			NombreLote = "Guia 1",
			Despacho = Path.GetFileNameWithoutExtension(txtArchivoActual.Text),
			NumeroPagina = (int.TryParse(txtPaginaSeleccionada.Text, out var pagina) ? pagina : 0),
			ValorEncontrado = Convert.ToInt32(txtNuevoCodigo.Text)
		});
		dgvSeparadoresEncontrados.DataSource = null;
		dgvSeparadoresEncontrados.DataSource = listaActual;
		OrdenarDgvSeparadores();
		UpdateViewer();
		txtPaginaSeleccionada.Clear();
		txtNuevoCodigo.Clear();
		pnlAsignarCodigo.Visible = false;
	}

	private void OrdenarDgvSeparadores()
	{
		if (dgvSeparadoresEncontrados.DataSource is List<eCodigosBarrasEncontrados_v2> lista)
		{
			List<eCodigosBarrasEncontrados_v2> listaOrdenada = (from x in lista
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
			rutaCarpetaSalida = rutaLoteDestino;
			nConfiguracion.actualizarUltimaCarpetaSalida(oUsuarioLogueado, 1, rutaCarpetaSalida, rutaLoteDestino);
		}
	}

	private void btnProcesar_Click_1(object sender, EventArgs e)
	{
		foreach (DataGridViewRow row in (IEnumerable)dgvIndicesEncontrados.Rows)
		{
			eDespacho oDespacho = new eDespacho();
			oDespacho.id = Convert.ToInt32(row.Cells[0].Value.ToString());
			oDespacho.dsDespacho = row.Cells[1].Value.ToString();
			oDespacho.cdSerieDocumental = row.Cells[2].Value.ToString();
			oDespacho.nuSIGEA = row.Cells[3].Value.ToString();
			oDespacho.nuGuia = row.Cells[4].Value.ToString();
			oDespacho.dsUsuarioDigitalizacion = row.Cells[5].Value.ToString();
			string rutaArchivoPDF = row.Cells[7].Value.ToString();
			List<eCodigosBarrasEncontrados_v2> listaSeperadores = dgvSeparadoresEncontrados.DataSource as List<eCodigosBarrasEncontrados_v2>;
			if (listaSeperadores == null)
			{
				listaSeperadores = new List<eCodigosBarrasEncontrados_v2>();
			}
			procesarDespacho(oDespacho, rutaArchivoPDF, listaSeperadores);
		}
		MessageBox.Show("El proceso se terminó correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		vaciarFormulario();
	}

	private void procesarDespacho(eDespacho pDespacho, string pRutaArchivoPDF, List<eCodigosBarrasEncontrados_v2> pListaSeparadores)
	{
		nCodigoBarras_v5.ProcesarPDF(oUsuarioLogueado, rutaCarpetaSalida, pRutaArchivoPDF, pListaSeparadores, pDespacho);
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void ajustarFormulario_SeleccionarCarpeta()
	{
		base.Size = new Size(1300, 150);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_BuscarSeparadores()
	{
		base.Size = new Size(1300, 325);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_Procesar()
	{
		base.Size = new Size(1700, 800);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
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
		dgvTotalLotesEncontrados.DataSource = null;
		dgvTotalLotesEncontrados.Columns.Clear();
		dgvIndicesEncontrados.DataSource = null;
		dgvIndicesEncontrados.Columns.Clear();
		dgvSeparadoresEncontrados.DataSource = null;
		dgvSeparadoresEncontrados.Columns.Clear();
		rutaLoteOrigen = string.Empty;
		lboxListaArchivos.Items.Clear();
		listaCodigoBarraFinal.Clear();
		progressBar1.Value = 0;
		progressBar1.Maximum = 0;
		progressBar1.Visible = false;
		txtSeleccion.Text = "";
		btnZoom.BackColor = Color.SeaGreen;
		btnAgregar.BackColor = Color.SeaGreen;
		btnBuscarLotes.Enabled = true;
		btnBuscarLotes.BackColor = Color.SeaGreen;
		btnBuscarSeparadores.Enabled = true;
		btnBuscarSeparadores.BackColor = Color.SeaGreen;
		btnSeleccionarCarpetaOrigen.Enabled = true;
		btnSeleccionarCarpetaOrigen.BackColor = Color.SeaGreen;
		foreach (Control control in pnlVisor.Controls)
		{
			if (control is PictureBox pictureBox)
			{
				pictureBox.Image?.Dispose();
				pictureBox.Dispose();
			}
		}
		pnlVisor.Controls.Clear();
		GC.Collect();
		ajustarFormulario_SeleccionarCarpeta();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._04_Separador.frmDespachoSeparar_v2));
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
		this.gbSeleccionarCarpetaOrigen = new System.Windows.Forms.GroupBox();
		this.btnBuscarLotes = new System.Windows.Forms.Button();
		this.txtRutaCarpetaOrigen = new System.Windows.Forms.TextBox();
		this.btnSeleccionarCarpetaOrigen = new System.Windows.Forms.Button();
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
		this.btnBuscarSeparadores = new System.Windows.Forms.Button();
		this.dgvSeparadoresEncontrados = new System.Windows.Forms.DataGridView();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.txtSeleccion = new System.Windows.Forms.TextBox();
		this.gbSeleccionarCarpetaOrigen.SuspendLayout();
		this.gbBotonesNavegacion.SuspendLayout();
		this.pnlAsignarCodigo.SuspendLayout();
		this.groupBox1.SuspendLayout();
		this.gbSeleccionarCarpetaSalida.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesEncontrados).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dgvIndicesEncontrados).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dgvSeparadoresEncontrados).BeginInit();
		base.SuspendLayout();
		this.gbSeleccionarCarpetaOrigen.Controls.Add(this.btnBuscarLotes);
		this.gbSeleccionarCarpetaOrigen.Controls.Add(this.txtRutaCarpetaOrigen);
		this.gbSeleccionarCarpetaOrigen.Controls.Add(this.btnSeleccionarCarpetaOrigen);
		this.gbSeleccionarCarpetaOrigen.Location = new System.Drawing.Point(13, 13);
		this.gbSeleccionarCarpetaOrigen.Name = "gbSeleccionarCarpetaOrigen";
		this.gbSeleccionarCarpetaOrigen.Size = new System.Drawing.Size(770, 58);
		this.gbSeleccionarCarpetaOrigen.TabIndex = 0;
		this.gbSeleccionarCarpetaOrigen.TabStop = false;
		this.btnBuscarLotes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnBuscarLotes.BackColor = System.Drawing.Color.SeaGreen;
		this.btnBuscarLotes.FlatAppearance.BorderSize = 0;
		this.btnBuscarLotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnBuscarLotes.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBuscarLotes.ForeColor = System.Drawing.Color.White;
		this.btnBuscarLotes.Image = (System.Drawing.Image)resources.GetObject("btnBuscarLotes.Image");
		this.btnBuscarLotes.Location = new System.Drawing.Point(632, 19);
		this.btnBuscarLotes.Name = "btnBuscarLotes";
		this.btnBuscarLotes.Size = new System.Drawing.Size(132, 25);
		this.btnBuscarLotes.TabIndex = 36;
		this.btnBuscarLotes.Text = "   Buscar";
		this.btnBuscarLotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnBuscarLotes.UseVisualStyleBackColor = false;
		this.btnBuscarLotes.Click += new System.EventHandler(btnBuscarLotes_Click);
		this.txtRutaCarpetaOrigen.BackColor = System.Drawing.Color.DarkGray;
		this.txtRutaCarpetaOrigen.Enabled = false;
		this.txtRutaCarpetaOrigen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtRutaCarpetaOrigen.Location = new System.Drawing.Point(256, 19);
		this.txtRutaCarpetaOrigen.Multiline = true;
		this.txtRutaCarpetaOrigen.Name = "txtRutaCarpetaOrigen";
		this.txtRutaCarpetaOrigen.Size = new System.Drawing.Size(370, 25);
		this.txtRutaCarpetaOrigen.TabIndex = 2;
		this.btnSeleccionarCarpetaOrigen.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarCarpetaOrigen.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarCarpetaOrigen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarCarpetaOrigen.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarCarpetaOrigen.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarCarpetaOrigen.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarCarpetaOrigen.Image");
		this.btnSeleccionarCarpetaOrigen.Location = new System.Drawing.Point(7, 19);
		this.btnSeleccionarCarpetaOrigen.Name = "btnSeleccionarCarpetaOrigen";
		this.btnSeleccionarCarpetaOrigen.Size = new System.Drawing.Size(243, 25);
		this.btnSeleccionarCarpetaOrigen.TabIndex = 15;
		this.btnSeleccionarCarpetaOrigen.Text = "   Seleccionar Carpeta Origen";
		this.btnSeleccionarCarpetaOrigen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarCarpetaOrigen.UseVisualStyleBackColor = false;
		this.btnSeleccionarCarpetaOrigen.Click += new System.EventHandler(btnSeleccionarCarpetaOrigen_Click);
		this.pnlVisor.Location = new System.Drawing.Point(380, 352);
		this.pnlVisor.Name = "pnlVisor";
		this.pnlVisor.Size = new System.Drawing.Size(1292, 333);
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
		this.gbBotonesNavegacion.Location = new System.Drawing.Point(380, 290);
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
		this.lboxListaArchivos.Location = new System.Drawing.Point(1385, 162);
		this.lboxListaArchivos.Name = "lboxListaArchivos";
		this.lboxListaArchivos.Size = new System.Drawing.Size(281, 116);
		this.lboxListaArchivos.TabIndex = 7;
		this.pnlAsignarCodigo.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlAsignarCodigo.Controls.Add(this.groupBox1);
		this.pnlAsignarCodigo.Location = new System.Drawing.Point(370, 290);
		this.pnlAsignarCodigo.Name = "pnlAsignarCodigo";
		this.pnlAsignarCodigo.Size = new System.Drawing.Size(1302, 395);
		this.pnlAsignarCodigo.TabIndex = 8;
		this.pnlAsignarCodigo.Visible = false;
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
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.btnProcesar);
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.btnSeleccionarCarpetaSalida);
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.txtCarpetaSalida);
		this.gbSeleccionarCarpetaSalida.Location = new System.Drawing.Point(12, 691);
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
		this.progressBar1.Location = new System.Drawing.Point(270, 238);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(513, 30);
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
		this.dgvTotalLotesEncontrados.Location = new System.Drawing.Point(799, 13);
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
		this.dgvTotalLotesEncontrados.Size = new System.Drawing.Size(418, 255);
		this.dgvTotalLotesEncontrados.TabIndex = 17;
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
		this.dgvIndicesEncontrados.Location = new System.Drawing.Point(11, 77);
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
		this.dgvIndicesEncontrados.Size = new System.Drawing.Size(772, 155);
		this.dgvIndicesEncontrados.TabIndex = 18;
		this.btnBuscarSeparadores.BackColor = System.Drawing.Color.SeaGreen;
		this.btnBuscarSeparadores.FlatAppearance.BorderSize = 0;
		this.btnBuscarSeparadores.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnBuscarSeparadores.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBuscarSeparadores.ForeColor = System.Drawing.Color.White;
		this.btnBuscarSeparadores.Image = (System.Drawing.Image)resources.GetObject("btnBuscarSeparadores.Image");
		this.btnBuscarSeparadores.Location = new System.Drawing.Point(11, 238);
		this.btnBuscarSeparadores.Name = "btnBuscarSeparadores";
		this.btnBuscarSeparadores.Size = new System.Drawing.Size(250, 30);
		this.btnBuscarSeparadores.TabIndex = 37;
		this.btnBuscarSeparadores.Text = "   Buscar Separadores";
		this.btnBuscarSeparadores.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnBuscarSeparadores.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnBuscarSeparadores.UseVisualStyleBackColor = false;
		this.btnBuscarSeparadores.Click += new System.EventHandler(btnBuscarSeparadores_Click_1);
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
		this.dgvSeparadoresEncontrados.Location = new System.Drawing.Point(13, 290);
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
		this.dgvSeparadoresEncontrados.Size = new System.Drawing.Size(361, 395);
		this.dgvSeparadoresEncontrados.TabIndex = 38;
		this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancelar.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(1542, 13);
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
		this.btnCerrar.Location = new System.Drawing.Point(1542, 46);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(130, 25);
		this.btnCerrar.TabIndex = 40;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.txtSeleccion.BackColor = System.Drawing.Color.DarkGray;
		this.txtSeleccion.Location = new System.Drawing.Point(1001, 264);
		this.txtSeleccion.Name = "txtSeleccion";
		this.txtSeleccion.Size = new System.Drawing.Size(144, 20);
		this.txtSeleccion.TabIndex = 43;
		this.txtSeleccion.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1684, 761);
		base.Controls.Add(this.txtSeleccion);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.dgvSeparadoresEncontrados);
		base.Controls.Add(this.btnBuscarSeparadores);
		base.Controls.Add(this.dgvIndicesEncontrados);
		base.Controls.Add(this.dgvTotalLotesEncontrados);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.gbSeleccionarCarpetaSalida);
		base.Controls.Add(this.pnlAsignarCodigo);
		base.Controls.Add(this.lboxListaArchivos);
		base.Controls.Add(this.gbBotonesNavegacion);
		base.Controls.Add(this.pnlVisor);
		base.Controls.Add(this.gbSeleccionarCarpetaOrigen);
		base.MaximizeBox = false;
		base.Name = "frmDespachoSeparar_v2";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Procesar Despachos";
		base.Load += new System.EventHandler(frmDespachoSeparar_v2_Load);
		this.gbSeleccionarCarpetaOrigen.ResumeLayout(false);
		this.gbSeleccionarCarpetaOrigen.PerformLayout();
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
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
