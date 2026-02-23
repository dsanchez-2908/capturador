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

public class frmControlCalidad_v2 : Form
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

	private string nombreCarpeta;

	private int zoomLevel = 5;

	private const int zoomMin = 2;

	private const int zoomMax = 10;

	private Dictionary<int, int> pageRotations = new Dictionary<int, int>();

	private IContainer components = null;

	private GroupBox groupBox1;

	private TextBox txtRutaCarpeta;

	private Button btnSeleccionarCarpeta;

	private TextBox txtFuncionSeleccionado;

	private Button btnEliminarPagina;

	private Button btnGirarDerecha;

	private Button btnGirarIzquierda;

	private Button btnArchivoSiguiente;

	private Button btnArchivoAnterior;

	private Button btnPaginaSiguiente;

	private Button btnPaginaAnterior;

	private ComboBox cbxCantidadPaginas;

	private Label label1;

	private ListBox lboxListaArchivos;

	private Panel pnlVisor;

	private TextBox txtArchivoActual;

	private GroupBox groupBox3;

	private GroupBox groupBox4;

	private TextBox txtTotalPaginas;

	private Label label2;

	private Button btnEliminarPaginasBlanco;

	private GroupBox gbArchivosEncontrados;

	private Button btnCerrar;

	private Button btnCancelar;

	private Label label3;

	private GroupBox groupBox2;

	private ProgressBar progressBar1;

	private TextBox txtNombreCarpeta;

	private Button btnZoom;

	private Button btnMoverPaginas;

	private GroupBox groupBox5;

	private Panel pnlMoverPagina;

	private Button btnCancelarMover;

	private Button btnMover;

	private TextBox txtNumeroPaginaMover;

	private Label label5;

	private Label label4;

	private TextBox txtPaginaSelccionadaMover;

	private Button btnCargarLote;

	private Button btnZoomMenos;

	private Button btnZoomMas;

	private Label lblZoom;

	public frmControlCalidad_v2(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
		cbxCantidadPaginas.Items.AddRange(new object[7] { 1, 2, 4, 6, 8, 10, 12 });
		cbxCantidadPaginas.SelectedIndex = 0;
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
		txtRutaCarpeta.Text = rutaCarpetaInicial;
	}

	private void btnSeleccionarCarpeta_Click(object sender, EventArgs e)
	{
		using FolderBrowserDialog fbd = new FolderBrowserDialog();
		fbd.Description = "Seleccionar la carpeta a procesar";
		fbd.SelectedPath = rutaCarpetaInicial;
		fbd.ShowNewFolderButton = false;
		if (fbd.ShowDialog() == DialogResult.OK)
		{
			txtRutaCarpeta.Text = fbd.SelectedPath;
			nConfiguracion.actualizarUltimaCarpetaOrigen(oUsuarioLogueado, 1, rutaCarpetaInicial, txtRutaCarpeta.Text);
			string[] parts = txtRutaCarpeta.Text.Split(new string[1] { "\\" }, StringSplitOptions.None);
			nombreCarpeta = parts[parts.Length - 1];
			txtNombreCarpeta.Text = nombreCarpeta;
			LoadPdfFiles(fbd.SelectedPath);
		}
	}

	private void btnCargarLote_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtRutaCarpeta.Text))
		{
			MessageBox.Show("Sebe seleccionar una carpeta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else
		{
			LoadPdfFiles(txtRutaCarpeta.Text);
		}
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
			btnSeleccionarCarpeta.Enabled = false;
			btnCargarLote.Enabled = false;
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
		gbArchivosEncontrados.Text = "Archivos Encontrados: " + Convert.ToString(lboxListaArchivos.Items.Count);
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
				BackColor = Color.LightGray
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

	private void btnPaginaAnterior_Click(object sender, EventArgs e)
	{
		if (currentPage - pagesPerView >= 0)
		{
			currentPage -= pagesPerView;
			UpdateViewer();
		}
		else
		{
			btnArchivoAnterior_Click(sender, e);
		}
	}

	private void btnPaginaSiguiente_Click(object sender, EventArgs e)
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
		else
		{
			MessageBox.Show("Termino de Controlar la Carpeta", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			deshabilitarFormulario();
		}
	}

	private void btnArchivoAnterior_Click(object sender, EventArgs e)
	{
		if (currentFileIndex > 0)
		{
			currentFileIndex--;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			progressBar1.Value = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
	}

	private void btnArchivoSiguiente_Click(object sender, EventArgs e)
	{
		if (currentFileIndex < pdfFiles.Count - 1)
		{
			currentFileIndex++;
			lboxListaArchivos.SelectedIndex = currentFileIndex;
			progressBar1.Value = currentFileIndex;
			LoadPdf(pdfFiles[currentFileIndex]);
		}
	}

	private void btnZoom_Click_1(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "ZOOM")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "ZOOM";
		}
	}

	private void btnGirarIzquierda_Click(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "GIRAR A LA IZQUIERDA")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "GIRAR A LA IZQUIERDA";
		}
	}

	private void btnGirarDerecha_Click(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "GIRAR A LA DERECHA")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "GIRAR A LA DERECHA";
		}
	}

	private void btnEliminar_Click(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "ELIMINAR")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "ELIMINAR";
		}
	}

	private void btnMoverPaginas_Click(object sender, EventArgs e)
	{
		if (txtFuncionSeleccionado.Text == "MOVER PAGINAS")
		{
			txtFuncionSeleccionado.Clear();
		}
		else
		{
			txtFuncionSeleccionado.Text = "MOVER PAGINAS";
		}
	}

	private void btnGuardar_Click(object sender, EventArgs e)
	{
		SavePdf();
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

	private void btnEliminarPaginasBlanco_Click(object sender, EventArgs e)
	{
		pdfDocument.Dispose();
		frmEliminarPaginasBlanco oFrmEliminarPaginasBlanco = new frmEliminarPaginasBlanco(oUsuarioLogueado, txtRutaCarpeta.Text, listaArchivos);
		oFrmEliminarPaginasBlanco.Show();
		oFrmEliminarPaginasBlanco.OnProcesoTerminado += delegate(string filePath)
		{
			LoadPdfFiles(filePath);
		};
		oFrmEliminarPaginasBlanco.Show();
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		deshabilitarFormulario();
	}

	private void deshabilitarFormulario()
	{
		btnCancelar.Enabled = false;
		btnEliminarPaginasBlanco.Enabled = false;
		cbxCantidadPaginas.Enabled = false;
		btnZoom.Enabled = false;
		btnPaginaAnterior.Enabled = false;
		btnPaginaSiguiente.Enabled = false;
		btnArchivoAnterior.Enabled = false;
		btnArchivoSiguiente.Enabled = false;
		btnGirarIzquierda.Enabled = false;
		btnGirarDerecha.Enabled = false;
		btnEliminarPagina.Enabled = false;
		btnMoverPaginas.Enabled = false;
		btnZoomMas.Enabled = true;
		btnZoomMenos.Enabled = true;
		listaArchivos.Clear();
		btnSeleccionarCarpeta.Enabled = true;
		btnCargarLote.Enabled = true;
		txtNombreCarpeta.Clear();
		progressBar1.Value = 0;
		lboxListaArchivos.Items.Clear();
		gbArchivosEncontrados.Text = "Archivos Encontrados:";
		txtArchivoActual.Clear();
		txtTotalPaginas.Clear();
		txtFuncionSeleccionado.Clear();
		pnlVisor.Controls.Clear();
		GC.Collect();
	}

	private void frmControlCalidad_v2_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Up:
			btnArchivoAnterior_Click(sender, e);
			break;
		case Keys.Down:
			btnArchivoSiguiente_Click(sender, e);
			break;
		case Keys.Left:
			btnPaginaAnterior_Click(sender, e);
			break;
		case Keys.Right:
			btnPaginaSiguiente_Click(sender, e);
			break;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Up:
			btnArchivoAnterior_Click(null, null);
			return true;
		case Keys.Down:
			btnArchivoSiguiente_Click(null, null);
			return true;
		case Keys.Left:
			btnPaginaAnterior_Click(null, null);
			return true;
		case Keys.Right:
			btnPaginaSiguiente_Click(null, null);
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
		string rutaArchivoActual = Path.Combine(txtRutaCarpeta.Text, txtArchivoActual.Text);
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

	private void btnZoomMas_Click(object sender, EventArgs e)
	{
		if (zoomLevel < 10)
		{
			zoomLevel++;
			UpdateViewer();
			lblZoom.Text = $"Zoom: {zoomLevel * 20}%";
		}
	}

	private void btnZoomMenos_Click(object sender, EventArgs e)
	{
		if (zoomLevel > 2)
		{
			zoomLevel--;
			UpdateViewer();
			lblZoom.Text = $"Zoom: {zoomLevel * 20}%";
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
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.btnCargarLote = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.txtRutaCarpeta = new System.Windows.Forms.TextBox();
		this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
		this.txtNombreCarpeta = new System.Windows.Forms.TextBox();
		this.btnEliminarPaginasBlanco = new System.Windows.Forms.Button();
		this.cbxCantidadPaginas = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.txtFuncionSeleccionado = new System.Windows.Forms.TextBox();
		this.btnEliminarPagina = new System.Windows.Forms.Button();
		this.btnGirarDerecha = new System.Windows.Forms.Button();
		this.btnGirarIzquierda = new System.Windows.Forms.Button();
		this.btnArchivoSiguiente = new System.Windows.Forms.Button();
		this.btnArchivoAnterior = new System.Windows.Forms.Button();
		this.btnPaginaSiguiente = new System.Windows.Forms.Button();
		this.btnPaginaAnterior = new System.Windows.Forms.Button();
		this.lboxListaArchivos = new System.Windows.Forms.ListBox();
		this.pnlVisor = new System.Windows.Forms.Panel();
		this.pnlMoverPagina = new System.Windows.Forms.Panel();
		this.label5 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.txtPaginaSelccionadaMover = new System.Windows.Forms.TextBox();
		this.btnCancelarMover = new System.Windows.Forms.Button();
		this.btnMover = new System.Windows.Forms.Button();
		this.txtNumeroPaginaMover = new System.Windows.Forms.TextBox();
		this.txtArchivoActual = new System.Windows.Forms.TextBox();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.label3 = new System.Windows.Forms.Label();
		this.txtTotalPaginas = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.groupBox4 = new System.Windows.Forms.GroupBox();
		this.lblZoom = new System.Windows.Forms.Label();
		this.btnZoomMenos = new System.Windows.Forms.Button();
		this.btnZoomMas = new System.Windows.Forms.Button();
		this.btnMoverPaginas = new System.Windows.Forms.Button();
		this.btnZoom = new System.Windows.Forms.Button();
		this.gbArchivosEncontrados = new System.Windows.Forms.GroupBox();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.groupBox5 = new System.Windows.Forms.GroupBox();
		this.groupBox1.SuspendLayout();
		this.pnlMoverPagina.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.groupBox4.SuspendLayout();
		this.gbArchivosEncontrados.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox5.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.btnCargarLote);
		this.groupBox1.Controls.Add(this.btnCerrar);
		this.groupBox1.Controls.Add(this.btnCancelar);
		this.groupBox1.Controls.Add(this.txtRutaCarpeta);
		this.groupBox1.Controls.Add(this.btnSeleccionarCarpeta);
		this.groupBox1.Location = new System.Drawing.Point(13, 13);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(716, 62);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Selección de Carpeta";
		this.btnCargarLote.Location = new System.Drawing.Point(464, 19);
		this.btnCargarLote.Name = "btnCargarLote";
		this.btnCargarLote.Size = new System.Drawing.Size(75, 37);
		this.btnCargarLote.TabIndex = 4;
		this.btnCargarLote.Text = "Cargar Lote";
		this.btnCargarLote.UseVisualStyleBackColor = true;
		this.btnCargarLote.Click += new System.EventHandler(btnCargarLote_Click);
		this.btnCerrar.Location = new System.Drawing.Point(635, 19);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(75, 37);
		this.btnCerrar.TabIndex = 3;
		this.btnCerrar.Text = "Cerrar";
		this.btnCerrar.UseVisualStyleBackColor = true;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.btnCancelar.Enabled = false;
		this.btnCancelar.Location = new System.Drawing.Point(554, 19);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(75, 37);
		this.btnCancelar.TabIndex = 2;
		this.btnCancelar.Text = "Cancelar";
		this.btnCancelar.UseVisualStyleBackColor = true;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.txtRutaCarpeta.Enabled = false;
		this.txtRutaCarpeta.Location = new System.Drawing.Point(162, 19);
		this.txtRutaCarpeta.Multiline = true;
		this.txtRutaCarpeta.Name = "txtRutaCarpeta";
		this.txtRutaCarpeta.Size = new System.Drawing.Size(296, 37);
		this.txtRutaCarpeta.TabIndex = 1;
		this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(6, 19);
		this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
		this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(150, 37);
		this.btnSeleccionarCarpeta.TabIndex = 0;
		this.btnSeleccionarCarpeta.Text = "Seleccionar Carpeta";
		this.btnSeleccionarCarpeta.UseVisualStyleBackColor = true;
		this.btnSeleccionarCarpeta.Click += new System.EventHandler(btnSeleccionarCarpeta_Click);
		this.txtNombreCarpeta.Enabled = false;
		this.txtNombreCarpeta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNombreCarpeta.Location = new System.Drawing.Point(25, 19);
		this.txtNombreCarpeta.Multiline = true;
		this.txtNombreCarpeta.Name = "txtNombreCarpeta";
		this.txtNombreCarpeta.Size = new System.Drawing.Size(217, 82);
		this.txtNombreCarpeta.TabIndex = 4;
		this.txtNombreCarpeta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.btnEliminarPaginasBlanco.Enabled = false;
		this.btnEliminarPaginasBlanco.Location = new System.Drawing.Point(13, 81);
		this.btnEliminarPaginasBlanco.Name = "btnEliminarPaginasBlanco";
		this.btnEliminarPaginasBlanco.Size = new System.Drawing.Size(716, 43);
		this.btnEliminarPaginasBlanco.TabIndex = 2;
		this.btnEliminarPaginasBlanco.Text = "Eliminar Páginas en Blanco";
		this.btnEliminarPaginasBlanco.UseVisualStyleBackColor = true;
		this.btnEliminarPaginasBlanco.Click += new System.EventHandler(btnEliminarPaginasBlanco_Click);
		this.cbxCantidadPaginas.Enabled = false;
		this.cbxCantidadPaginas.FormattingEnabled = true;
		this.cbxCantidadPaginas.Location = new System.Drawing.Point(45, 60);
		this.cbxCantidadPaginas.Name = "cbxCantidadPaginas";
		this.cbxCantidadPaginas.Size = new System.Drawing.Size(43, 21);
		this.cbxCantidadPaginas.TabIndex = 1;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(25, 26);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(91, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Páginas a mostrar";
		this.txtFuncionSeleccionado.Enabled = false;
		this.txtFuncionSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFuncionSeleccionado.Location = new System.Drawing.Point(6, 19);
		this.txtFuncionSeleccionado.Multiline = true;
		this.txtFuncionSeleccionado.Name = "txtFuncionSeleccionado";
		this.txtFuncionSeleccionado.Size = new System.Drawing.Size(312, 36);
		this.txtFuncionSeleccionado.TabIndex = 10;
		this.txtFuncionSeleccionado.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.btnEliminarPagina.Enabled = false;
		this.btnEliminarPagina.Location = new System.Drawing.Point(431, 61);
		this.btnEliminarPagina.Name = "btnEliminarPagina";
		this.btnEliminarPagina.Size = new System.Drawing.Size(100, 50);
		this.btnEliminarPagina.TabIndex = 8;
		this.btnEliminarPagina.Text = "Eliminar Página";
		this.btnEliminarPagina.UseVisualStyleBackColor = true;
		this.btnEliminarPagina.Click += new System.EventHandler(btnEliminar_Click);
		this.btnGirarDerecha.Enabled = false;
		this.btnGirarDerecha.Location = new System.Drawing.Point(218, 61);
		this.btnGirarDerecha.Name = "btnGirarDerecha";
		this.btnGirarDerecha.Size = new System.Drawing.Size(100, 50);
		this.btnGirarDerecha.TabIndex = 7;
		this.btnGirarDerecha.Text = "Girar a la Derecha";
		this.btnGirarDerecha.UseVisualStyleBackColor = true;
		this.btnGirarDerecha.Click += new System.EventHandler(btnGirarDerecha_Click);
		this.btnGirarIzquierda.Enabled = false;
		this.btnGirarIzquierda.Location = new System.Drawing.Point(112, 61);
		this.btnGirarIzquierda.Name = "btnGirarIzquierda";
		this.btnGirarIzquierda.Size = new System.Drawing.Size(100, 50);
		this.btnGirarIzquierda.TabIndex = 6;
		this.btnGirarIzquierda.Text = "Girar a la Izquierda";
		this.btnGirarIzquierda.UseVisualStyleBackColor = true;
		this.btnGirarIzquierda.Click += new System.EventHandler(btnGirarIzquierda_Click);
		this.btnArchivoSiguiente.Enabled = false;
		this.btnArchivoSiguiente.Location = new System.Drawing.Point(417, 61);
		this.btnArchivoSiguiente.Name = "btnArchivoSiguiente";
		this.btnArchivoSiguiente.Size = new System.Drawing.Size(130, 50);
		this.btnArchivoSiguiente.TabIndex = 5;
		this.btnArchivoSiguiente.TabStop = false;
		this.btnArchivoSiguiente.Text = "Archivo Siguiente";
		this.btnArchivoSiguiente.UseVisualStyleBackColor = true;
		this.btnArchivoSiguiente.Click += new System.EventHandler(btnArchivoSiguiente_Click);
		this.btnArchivoAnterior.Enabled = false;
		this.btnArchivoAnterior.Location = new System.Drawing.Point(281, 61);
		this.btnArchivoAnterior.Name = "btnArchivoAnterior";
		this.btnArchivoAnterior.Size = new System.Drawing.Size(130, 50);
		this.btnArchivoAnterior.TabIndex = 4;
		this.btnArchivoAnterior.TabStop = false;
		this.btnArchivoAnterior.Text = "Archivo Anterior";
		this.btnArchivoAnterior.UseVisualStyleBackColor = true;
		this.btnArchivoAnterior.Click += new System.EventHandler(btnArchivoAnterior_Click);
		this.btnPaginaSiguiente.Enabled = false;
		this.btnPaginaSiguiente.Location = new System.Drawing.Point(145, 61);
		this.btnPaginaSiguiente.Name = "btnPaginaSiguiente";
		this.btnPaginaSiguiente.Size = new System.Drawing.Size(130, 50);
		this.btnPaginaSiguiente.TabIndex = 3;
		this.btnPaginaSiguiente.Text = "Pag. Siguiente";
		this.btnPaginaSiguiente.UseVisualStyleBackColor = false;
		this.btnPaginaSiguiente.Click += new System.EventHandler(btnPaginaSiguiente_Click);
		this.btnPaginaAnterior.Enabled = false;
		this.btnPaginaAnterior.Location = new System.Drawing.Point(9, 61);
		this.btnPaginaAnterior.Name = "btnPaginaAnterior";
		this.btnPaginaAnterior.Size = new System.Drawing.Size(130, 50);
		this.btnPaginaAnterior.TabIndex = 2;
		this.btnPaginaAnterior.TabStop = false;
		this.btnPaginaAnterior.Text = "Pag. Anterior";
		this.btnPaginaAnterior.UseVisualStyleBackColor = true;
		this.btnPaginaAnterior.Click += new System.EventHandler(btnPaginaAnterior_Click);
		this.lboxListaArchivos.FormattingEnabled = true;
		this.lboxListaArchivos.Location = new System.Drawing.Point(6, 19);
		this.lboxListaArchivos.Name = "lboxListaArchivos";
		this.lboxListaArchivos.Size = new System.Drawing.Size(239, 82);
		this.lboxListaArchivos.TabIndex = 2;
		this.pnlVisor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlVisor.Location = new System.Drawing.Point(13, 283);
		this.pnlVisor.Name = "pnlVisor";
		this.pnlVisor.Size = new System.Drawing.Size(1259, 495);
		this.pnlVisor.TabIndex = 3;
		this.pnlMoverPagina.BackColor = System.Drawing.SystemColors.GrayText;
		this.pnlMoverPagina.Controls.Add(this.label5);
		this.pnlMoverPagina.Controls.Add(this.label4);
		this.pnlMoverPagina.Controls.Add(this.txtPaginaSelccionadaMover);
		this.pnlMoverPagina.Controls.Add(this.btnCancelarMover);
		this.pnlMoverPagina.Controls.Add(this.btnMover);
		this.pnlMoverPagina.Controls.Add(this.txtNumeroPaginaMover);
		this.pnlMoverPagina.Location = new System.Drawing.Point(9, 11);
		this.pnlMoverPagina.Name = "pnlMoverPagina";
		this.pnlMoverPagina.Size = new System.Drawing.Size(538, 100);
		this.pnlMoverPagina.TabIndex = 0;
		this.pnlMoverPagina.Visible = false;
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(146, 41);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(98, 13);
		this.label5.TabIndex = 5;
		this.label5.Text = "Mover después de:";
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(146, 15);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(111, 13);
		this.label4.TabIndex = 4;
		this.label4.Text = "Página Seleccionada:";
		this.txtPaginaSelccionadaMover.Enabled = false;
		this.txtPaginaSelccionadaMover.Location = new System.Drawing.Point(289, 12);
		this.txtPaginaSelccionadaMover.Name = "txtPaginaSelccionadaMover";
		this.txtPaginaSelccionadaMover.Size = new System.Drawing.Size(100, 20);
		this.txtPaginaSelccionadaMover.TabIndex = 3;
		this.btnCancelarMover.Location = new System.Drawing.Point(315, 64);
		this.btnCancelarMover.Name = "btnCancelarMover";
		this.btnCancelarMover.Size = new System.Drawing.Size(74, 23);
		this.btnCancelarMover.TabIndex = 2;
		this.btnCancelarMover.Text = "Cancelar";
		this.btnCancelarMover.UseVisualStyleBackColor = true;
		this.btnCancelarMover.Click += new System.EventHandler(btnCancelarMover_Click);
		this.btnMover.Location = new System.Drawing.Point(149, 64);
		this.btnMover.Name = "btnMover";
		this.btnMover.Size = new System.Drawing.Size(142, 23);
		this.btnMover.TabIndex = 1;
		this.btnMover.Text = "Mover";
		this.btnMover.UseVisualStyleBackColor = true;
		this.btnMover.Click += new System.EventHandler(btnMover_Click);
		this.txtNumeroPaginaMover.Location = new System.Drawing.Point(289, 38);
		this.txtNumeroPaginaMover.Name = "txtNumeroPaginaMover";
		this.txtNumeroPaginaMover.Size = new System.Drawing.Size(100, 20);
		this.txtNumeroPaginaMover.TabIndex = 0;
		this.txtArchivoActual.Enabled = false;
		this.txtArchivoActual.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.txtArchivoActual.Location = new System.Drawing.Point(58, 19);
		this.txtArchivoActual.Multiline = true;
		this.txtArchivoActual.Name = "txtArchivoActual";
		this.txtArchivoActual.Size = new System.Drawing.Size(313, 31);
		this.txtArchivoActual.TabIndex = 4;
		this.txtArchivoActual.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.groupBox3.Controls.Add(this.pnlMoverPagina);
		this.groupBox3.Controls.Add(this.label3);
		this.groupBox3.Controls.Add(this.btnPaginaAnterior);
		this.groupBox3.Controls.Add(this.btnPaginaSiguiente);
		this.groupBox3.Controls.Add(this.txtTotalPaginas);
		this.groupBox3.Controls.Add(this.label2);
		this.groupBox3.Controls.Add(this.btnArchivoAnterior);
		this.groupBox3.Controls.Add(this.btnArchivoSiguiente);
		this.groupBox3.Controls.Add(this.txtArchivoActual);
		this.groupBox3.Location = new System.Drawing.Point(13, 130);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(562, 117);
		this.groupBox3.TabIndex = 5;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "Navegación";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(6, 32);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(46, 13);
		this.label3.TabIndex = 8;
		this.label3.Text = "Archivo:";
		this.txtTotalPaginas.Enabled = false;
		this.txtTotalPaginas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.txtTotalPaginas.Location = new System.Drawing.Point(483, 19);
		this.txtTotalPaginas.Name = "txtTotalPaginas";
		this.txtTotalPaginas.Size = new System.Drawing.Size(64, 26);
		this.txtTotalPaginas.TabIndex = 7;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(378, 27);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(90, 13);
		this.label2.TabIndex = 6;
		this.label2.Text = "Total de Paginas:";
		this.groupBox4.Controls.Add(this.lblZoom);
		this.groupBox4.Controls.Add(this.btnZoomMenos);
		this.groupBox4.Controls.Add(this.btnZoomMas);
		this.groupBox4.Controls.Add(this.btnMoverPaginas);
		this.groupBox4.Controls.Add(this.btnZoom);
		this.groupBox4.Controls.Add(this.txtFuncionSeleccionado);
		this.groupBox4.Controls.Add(this.btnGirarIzquierda);
		this.groupBox4.Controls.Add(this.btnGirarDerecha);
		this.groupBox4.Controls.Add(this.btnEliminarPagina);
		this.groupBox4.Location = new System.Drawing.Point(735, 130);
		this.groupBox4.Name = "groupBox4";
		this.groupBox4.Size = new System.Drawing.Size(537, 117);
		this.groupBox4.TabIndex = 6;
		this.groupBox4.TabStop = false;
		this.groupBox4.Text = "Modificación";
		this.lblZoom.AutoSize = true;
		this.lblZoom.Location = new System.Drawing.Point(455, 26);
		this.lblZoom.Name = "lblZoom";
		this.lblZoom.Size = new System.Drawing.Size(10, 13);
		this.lblZoom.TabIndex = 16;
		this.lblZoom.Text = ".";
		this.btnZoomMenos.Enabled = false;
		this.btnZoomMenos.Location = new System.Drawing.Point(394, 8);
		this.btnZoomMenos.Name = "btnZoomMenos";
		this.btnZoomMenos.Size = new System.Drawing.Size(54, 50);
		this.btnZoomMenos.TabIndex = 15;
		this.btnZoomMenos.Text = "Zoom -";
		this.btnZoomMenos.UseVisualStyleBackColor = true;
		this.btnZoomMenos.Click += new System.EventHandler(btnZoomMenos_Click);
		this.btnZoomMas.Enabled = false;
		this.btnZoomMas.Location = new System.Drawing.Point(324, 8);
		this.btnZoomMas.Name = "btnZoomMas";
		this.btnZoomMas.Size = new System.Drawing.Size(64, 50);
		this.btnZoomMas.TabIndex = 14;
		this.btnZoomMas.Text = "Zoom +";
		this.btnZoomMas.UseVisualStyleBackColor = true;
		this.btnZoomMas.Click += new System.EventHandler(btnZoomMas_Click);
		this.btnMoverPaginas.Enabled = false;
		this.btnMoverPaginas.Location = new System.Drawing.Point(324, 61);
		this.btnMoverPaginas.Name = "btnMoverPaginas";
		this.btnMoverPaginas.Size = new System.Drawing.Size(100, 50);
		this.btnMoverPaginas.TabIndex = 13;
		this.btnMoverPaginas.Text = "Mover Páginas";
		this.btnMoverPaginas.UseVisualStyleBackColor = true;
		this.btnMoverPaginas.Click += new System.EventHandler(btnMoverPaginas_Click);
		this.btnZoom.Enabled = false;
		this.btnZoom.Location = new System.Drawing.Point(6, 61);
		this.btnZoom.Name = "btnZoom";
		this.btnZoom.Size = new System.Drawing.Size(100, 50);
		this.btnZoom.TabIndex = 12;
		this.btnZoom.Text = "Zoom";
		this.btnZoom.UseVisualStyleBackColor = true;
		this.btnZoom.Click += new System.EventHandler(btnZoom_Click_1);
		this.gbArchivosEncontrados.Controls.Add(this.lboxListaArchivos);
		this.gbArchivosEncontrados.Location = new System.Drawing.Point(1015, 13);
		this.gbArchivosEncontrados.Name = "gbArchivosEncontrados";
		this.gbArchivosEncontrados.Size = new System.Drawing.Size(251, 111);
		this.gbArchivosEncontrados.TabIndex = 7;
		this.gbArchivosEncontrados.TabStop = false;
		this.gbArchivosEncontrados.Text = "Archivos Encontrados:";
		this.groupBox2.Controls.Add(this.cbxCantidadPaginas);
		this.groupBox2.Controls.Add(this.label1);
		this.groupBox2.Location = new System.Drawing.Point(582, 131);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(147, 116);
		this.groupBox2.TabIndex = 8;
		this.groupBox2.TabStop = false;
		this.progressBar1.Location = new System.Drawing.Point(13, 254);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(1259, 23);
		this.progressBar1.TabIndex = 9;
		this.groupBox5.Controls.Add(this.txtNombreCarpeta);
		this.groupBox5.Location = new System.Drawing.Point(736, 13);
		this.groupBox5.Name = "groupBox5";
		this.groupBox5.Size = new System.Drawing.Size(273, 111);
		this.groupBox5.TabIndex = 10;
		this.groupBox5.TabStop = false;
		this.groupBox5.Text = "Nombre de Carpeta";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1284, 811);
		base.Controls.Add(this.groupBox5);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.gbArchivosEncontrados);
		base.Controls.Add(this.btnEliminarPaginasBlanco);
		base.Controls.Add(this.groupBox4);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.pnlVisor);
		base.Controls.Add(this.groupBox1);
		base.Name = "frmControlCalidad_v2";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Control de Calidad";
		base.Load += new System.EventHandler(frmControlCalidad_v2_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.pnlMoverPagina.ResumeLayout(false);
		this.pnlMoverPagina.PerformLayout();
		this.groupBox3.ResumeLayout(false);
		this.groupBox3.PerformLayout();
		this.groupBox4.ResumeLayout(false);
		this.groupBox4.PerformLayout();
		this.gbArchivosEncontrados.ResumeLayout(false);
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		this.groupBox5.ResumeLayout(false);
		this.groupBox5.PerformLayout();
		base.ResumeLayout(false);
	}
}
