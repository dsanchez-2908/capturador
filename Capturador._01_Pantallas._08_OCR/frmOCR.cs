using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capturador._01_Pantallas._06_Lotes;
using Capturador._02_Negocio;
using Capturador._04_Entidades;
using PdfiumViewer;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Tesseract;

namespace Capturador._01_Pantallas._08_OCR;

public class frmOCR : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string rutaCarpetaInicial;

	private string rutaCarpetaSalida;

	private string rutaLoteOrigen;

	private string nombreLote;

	private string nombreArchivoIndice = "INDEX_TERMINADO.DAT";

	private int cantidadTotalArchivos;

	private List<string> pdfFiles = new List<string>();

	private int currentFileIndex = 0;

	private int currentPage = 0;

	private int pagesPerView = 1;

	private PdfiumViewer.PdfDocument pdfDocument;

	private bool hasChanges = false;

	private List<string> listaArchivos = new List<string>();

	private List<eCodigosBarrasEncontrados_v2> listaCodigoBarraFinal = new List<eCodigosBarrasEncontrados_v2>();

	private BindingList<eLote> listaDisponibles;

	private BindingList<eLote> listaSeleccionados;

	private int archivosProcesadosGlobal = 0;

	private IContainer components = null;

	private GroupBox gbSeleccionarCarpetaSalida;

	private TextBox txtCarpetaSalida;

	private ProgressBar progressBar1;

	private DataGridView dgvTotalLotesEncontrados;

	private Button btnSeleccionarCarpetaSalida;

	private Button btnProcesar;

	private Button btnCancelar;

	private Button btnCerrar;

	private Panel pnlLotesDisponibles;

	private DataGridView dgvLotesDisponibles;

	private Button btnSeleccionarTodo;

	private Button btnDeseleccionarTodo;

	private Button btnAgregarLotes;

	private Button btnQuitarLotes;

	private Panel pnlLotesSeleccionados;

	private DataGridView dgvLotesSeleccionados;

	public frmOCR(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
		base.KeyPreview = true;
	}

	private void frmOCR_Load(object sender, EventArgs e)
	{
		eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		rutaCarpetaSalida = oProyectoConfiguracion.dsRutaSalida;
		txtCarpetaSalida.Text = rutaCarpetaSalida;
		rutaLoteOrigen = rutaCarpetaInicial;
		listaDisponibles = new BindingList<eLote>(nLotes.obtenerLotesDisponibleOCR(oUsuarioLogueado, 0));
		listaSeleccionados = new BindingList<eLote>();
		configurarDgvLotes(dgvLotesDisponibles, listaDisponibles, pnlLotesDisponibles, "Listado de Lotes Disponibles");
		configurarDgvLotes(dgvLotesSeleccionados, listaSeleccionados, pnlLotesSeleccionados, "Listado de Lotes Seleccionados");
		if (GlobalFontSettings.FontResolver == null)
		{
			GlobalFontSettings.FontResolver = new CustomFontResolver();
		}
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
		if (dgvLotesSeleccionados.Rows.Count <= 0)
		{
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
		List<eLote> listaDesdeBD = nLotes.obtenerLotesDisponibleOCR(oUsuarioLogueado, 0);
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

	private void vaciarFormulario()
	{
		actualizarLotesDisponibles();
		actualizarLotesSeleccionados();
	}

	private async void btnProcesar_Click_1(object sender, EventArgs e)
	{
		if (dgvLotesSeleccionados.Rows.Count == 0)
		{
			MessageBox.Show("No hay lotes seleccionados", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		cantidadTotalArchivos = 0;
		foreach (DataGridViewRow row in (IEnumerable)dgvLotesSeleccionados.Rows)
		{
			string rutaLoteContar = Convert.ToString(row.Cells["dsRutaLote"].Value);
			string[] archivosPDF = Directory.GetFiles(rutaLoteContar, "*.pdf", SearchOption.TopDirectoryOnly);
			cantidadTotalArchivos += archivosPDF.Length;
		}
		progressBar1.Minimum = 0;
		progressBar1.Maximum = cantidadTotalArchivos;
		progressBar1.Value = 0;
		progressBar1.Step = 1;
		btnProcesar.Enabled = false;
		foreach (DataGridViewRow row2 in (IEnumerable)dgvLotesSeleccionados.Rows)
		{
			eLote oLoteSeleccionado = new eLote
			{
				cdLote = Convert.ToInt32(row2.Cells["cdLote"].Value),
				cdProyecto = Convert.ToInt32(row2.Cells["cdProyecto"].Value),
				cdEstado = Convert.ToInt32(row2.Cells["cdEstado"].Value),
				dsRutaLote = Convert.ToString(row2.Cells["dsRutaLote"].Value),
				dsProyecto = Convert.ToString(row2.Cells["dsProyecto"].Value),
				dsNombreLote = Convert.ToString(row2.Cells["dsNombreLote"].Value),
				nuCantidadArchivos = Convert.ToInt32(row2.Cells["nuCantidadArchivos"].Value)
			};
			try
			{
				await procesarLotes(oLoteSeleccionado);
				nLotes.finalizarOCR(oUsuarioLogueado, oLoteSeleccionado);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		MessageBox.Show("Terminó de convertir los lotes", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		vaciarFormulario();
		btnProcesar.Enabled = true;
	}

	private async Task procesarLotes(eLote pLote)
	{
		string rutaLoteDestino = Path.Combine(rutaCarpetaSalida, pLote.dsNombreLote);
		Directory.CreateDirectory(rutaLoteDestino);
		string[] archivosPDF = Directory.GetFiles(pLote.dsRutaLote, "*.pdf", SearchOption.TopDirectoryOnly);
		await Task.Run(delegate
		{
			string[] array = archivosPDF;
			foreach (string text in array)
			{
				string fileName = Path.GetFileName(text);
				string rutaPDFDestino = Path.Combine(rutaLoteDestino, fileName);
				try
				{
					AplicarOCRyGuardarPDF_Streaming(text, rutaPDFDestino);
				}
				catch
				{
				}
				Invoke((MethodInvoker)delegate
				{
					archivosProcesadosGlobal++;
					progressBar1.Value = archivosProcesadosGlobal;
				});
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		});
	}

	private void AplicarOCRyGuardarPDF_Streaming(string rutaPDFOrigen, string rutaPDFDestino)
	{
		string nombreBase = Path.GetFileNameWithoutExtension(rutaPDFOrigen);
		string carpetaTemporal = Path.Combine(Path.GetTempPath(), "ocr_temp", nombreBase);
		Directory.CreateDirectory(carpetaTemporal);
		using (TesseractEngine engine = new TesseractEngine("./tessdata", "spa", EngineMode.Default))
		{
			using PdfiumViewer.PdfDocument document = PdfiumViewer.PdfDocument.Load(rutaPDFOrigen);
			for (int i = 0; i < document.PageCount; i++)
			{
				string rutaPaginaPDF = Path.Combine(carpetaTemporal, $"pagina_{i + 1:0000}.pdf");
				using (Image image = document.Render(i, 600f, 600f, forPrinting: true))
				{
					using Pix pix = PixConverter.ToPix((Bitmap)image);
					using Page page = engine.Process(pix);
					using PdfSharp.Pdf.PdfDocument paginaDoc = new PdfSharp.Pdf.PdfDocument();
					double widthPoints = (double)pix.Width * 72.0 / 600.0;
					double heightPoints = (double)pix.Height * 72.0 / 600.0;
					PdfPage pagePDF = paginaDoc.AddPage();
					pagePDF.Width = widthPoints;
					pagePDF.Height = heightPoints;
					using (XGraphics gfx = XGraphics.FromPdfPage(pagePDF))
					{
						using (XImage xImage = XImageFromBitmap((Bitmap)image))
						{
							gfx.DrawImage(xImage, 0.0, 0.0, pagePDF.Width, pagePDF.Height);
						}
						XFont fuenteInvisible = new XFont("Arial", 8.0);
						gfx.Save();
						gfx.TranslateTransform(0.0, pagePDF.Height);
						gfx.ScaleTransform(1.0, -1.0);
						using (ResultIterator iterator = page.GetIterator())
						{
							iterator.Begin();
							do
							{
								if (iterator.IsAtBeginningOf(PageIteratorLevel.Word))
								{
									string word = iterator.GetText(PageIteratorLevel.Word);
									if (!string.IsNullOrWhiteSpace(word) && iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var bounds))
									{
										double scaleX = pagePDF.Width / pix.Width;
										double scaleY = pagePDF.Height / pix.Height;
										double x = (double)bounds.X1 * scaleX;
										double y = (double)bounds.Y1 * scaleY;
										double width = (double)(bounds.X2 - bounds.X1) * scaleX;
										double height = (double)(bounds.Y2 - bounds.Y1) * scaleY;
										double yInvertido = pagePDF.Height - y - height;
										gfx.DrawString(word, fuenteInvisible, XBrushes.Transparent, new XRect(x, yInvertido, width, height), XStringFormats.TopLeft);
									}
								}
							}
							while (iterator.Next(PageIteratorLevel.Word));
						}
						gfx.Restore();
					}
					paginaDoc.Save(rutaPaginaPDF);
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}
		UnirPDFsTemporales(carpetaTemporal, rutaPDFDestino);
		try
		{
			Directory.Delete(carpetaTemporal, recursive: true);
		}
		catch
		{
		}
	}

	private void UnirPDFsTemporales(string carpetaOrigen, string rutaFinal)
	{
		List<string> archivos = (from f in Directory.GetFiles(carpetaOrigen, "*.pdf")
			orderby f
			select f).ToList();
		using PdfSharp.Pdf.PdfDocument documentoFinal = new PdfSharp.Pdf.PdfDocument();
		foreach (string archivo in archivos)
		{
			using PdfSharp.Pdf.PdfDocument pagina = PdfReader.Open(archivo, PdfDocumentOpenMode.Import);
			for (int i = 0; i < pagina.PageCount; i++)
			{
				documentoFinal.AddPage(pagina.Pages[i]);
			}
		}
		documentoFinal.ViewerPreferences.FitWindow = true;
		documentoFinal.ViewerPreferences.CenterWindow = true;
		documentoFinal.Save(rutaFinal);
	}

	private XImage XImageFromBitmap(Bitmap bitmap)
	{
		using MemoryStream ms = new MemoryStream();
		bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
		ms.Position = 0L;
		return XImage.FromStream(ms);
	}

	private void AplicarOCRyGuardarPDF_PaginaPorPagina(string rutaPDFOrigen, string rutaPDFDestino)
	{
		using TesseractEngine engine = new TesseractEngine("./tessdata", "spa", EngineMode.Default);
		using PdfiumViewer.PdfDocument document = PdfiumViewer.PdfDocument.Load(rutaPDFOrigen);
		PdfSharp.Pdf.PdfDocument finalDoc = new PdfSharp.Pdf.PdfDocument();
		for (int i = 0; i < document.PageCount; i++)
		{
			using Image image = document.Render(i, 300f, 300f, forPrinting: true);
			using Pix pix = PixConverter.ToPix((Bitmap)image);
			using Page page = engine.Process(pix);
			double widthPoints = (double)pix.Width * 72.0 / 300.0;
			double heightPoints = (double)pix.Height * 72.0 / 300.0;
			PdfPage pdfPage = finalDoc.AddPage();
			pdfPage.Width = widthPoints;
			pdfPage.Height = heightPoints;
			using XGraphics gfx = XGraphics.FromPdfPage(pdfPage);
			using (XImage xImage = XImageFromBitmap((Bitmap)image))
			{
				gfx.DrawImage(xImage, 0.0, 0.0, pdfPage.Width, pdfPage.Height);
			}
			XFont fuenteInvisible = new XFont("Arial", 8.0);
			gfx.Save();
			gfx.TranslateTransform(0.0, pdfPage.Height);
			gfx.ScaleTransform(1.0, -1.0);
			using (ResultIterator iterator = page.GetIterator())
			{
				iterator.Begin();
				do
				{
					if (iterator.IsAtBeginningOf(PageIteratorLevel.Word))
					{
						string word = iterator.GetText(PageIteratorLevel.Word);
						if (!string.IsNullOrWhiteSpace(word) && iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var bounds))
						{
							double scaleX = pdfPage.Width / pix.Width;
							double scaleY = pdfPage.Height / pix.Height;
							double x = (double)bounds.X1 * scaleX;
							double y = (double)bounds.Y1 * scaleY;
							double width = (double)(bounds.X2 - bounds.X1) * scaleX;
							double height = (double)(bounds.Y2 - bounds.Y1) * scaleY;
							double yInvertido = pdfPage.Height - y - height;
							gfx.DrawString(word, fuenteInvisible, XBrushes.Transparent, new XRect(x, yInvertido, width, height), XStringFormats.TopLeft);
						}
					}
				}
				while (iterator.Next(PageIteratorLevel.Word));
			}
			gfx.Restore();
		}
		finalDoc.ViewerPreferences.FitWindow = true;
		finalDoc.ViewerPreferences.CenterWindow = true;
		finalDoc.Save(rutaPDFDestino);
	}

	private void AplicarOCRyGuardarPDF(string rutaPDFOrigen, string rutaPDFDestino)
	{
		using PdfiumViewer.PdfDocument document = PdfiumViewer.PdfDocument.Load(rutaPDFOrigen);
		using TesseractEngine engine = new TesseractEngine("./tessdata", "spa", EngineMode.Default);
		PdfSharp.Pdf.PdfDocument outputPdf = new PdfSharp.Pdf.PdfDocument();
		for (int i = 0; i < document.PageCount; i++)
		{
			using Image image = document.Render(i, 300f, 300f, forPrinting: true);
			using Pix pix = PixConverter.ToPix((Bitmap)image);
			using Page page = engine.Process(pix);
			double widthPoints = (double)pix.Width * 72.0 / 300.0;
			double heightPoints = (double)pix.Height * 72.0 / 300.0;
			PdfPage pagePDF = outputPdf.AddPage();
			pagePDF.Width = widthPoints;
			pagePDF.Height = heightPoints;
			XGraphics gfx = XGraphics.FromPdfPage(pagePDF);
			using (XImage xImage = XImageFromBitmap((Bitmap)image))
			{
				gfx.DrawImage(xImage, 0.0, 0.0, pagePDF.Width, pagePDF.Height);
			}
			XFont fuenteInvisible = new XFont("Arial", 8.0);
			gfx.Save();
			gfx.TranslateTransform(0.0, pagePDF.Height);
			gfx.ScaleTransform(1.0, -1.0);
			using (ResultIterator iterator = page.GetIterator())
			{
				iterator.Begin();
				do
				{
					if (iterator.IsAtBeginningOf(PageIteratorLevel.Word))
					{
						string word = iterator.GetText(PageIteratorLevel.Word);
						if (!string.IsNullOrWhiteSpace(word) && iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var bounds))
						{
							double scaleX = pagePDF.Width / pix.Width;
							double scaleY = pagePDF.Height / pix.Height;
							double x = (double)bounds.X1 * scaleX;
							double y = (double)bounds.Y1 * scaleY;
							double width = (double)(bounds.X2 - bounds.X1) * scaleX;
							double height = (double)(bounds.Y2 - bounds.Y1) * scaleY;
							double yInvertido = pagePDF.Height - y - height;
							gfx.DrawString(word, fuenteInvisible, XBrushes.Transparent, new XRect(x, yInvertido, width, height), XStringFormats.TopLeft);
						}
					}
				}
				while (iterator.Next(PageIteratorLevel.Word));
			}
			gfx.Restore();
		}
		outputPdf.ViewerPreferences.FitWindow = true;
		outputPdf.ViewerPreferences.CenterWindow = true;
		outputPdf.Save(rutaPDFDestino);
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

	private void btnCancelar_Click(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._08_OCR.frmOCR));
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
		this.gbSeleccionarCarpetaSalida = new System.Windows.Forms.GroupBox();
		this.btnProcesar = new System.Windows.Forms.Button();
		this.btnSeleccionarCarpetaSalida = new System.Windows.Forms.Button();
		this.txtCarpetaSalida = new System.Windows.Forms.TextBox();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.dgvTotalLotesEncontrados = new System.Windows.Forms.DataGridView();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.pnlLotesDisponibles = new System.Windows.Forms.Panel();
		this.dgvLotesDisponibles = new System.Windows.Forms.DataGridView();
		this.btnSeleccionarTodo = new System.Windows.Forms.Button();
		this.btnDeseleccionarTodo = new System.Windows.Forms.Button();
		this.btnAgregarLotes = new System.Windows.Forms.Button();
		this.btnQuitarLotes = new System.Windows.Forms.Button();
		this.pnlLotesSeleccionados = new System.Windows.Forms.Panel();
		this.dgvLotesSeleccionados = new System.Windows.Forms.DataGridView();
		this.gbSeleccionarCarpetaSalida.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesEncontrados).BeginInit();
		this.pnlLotesDisponibles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).BeginInit();
		this.pnlLotesSeleccionados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvLotesSeleccionados).BeginInit();
		base.SuspendLayout();
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.btnProcesar);
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.btnSeleccionarCarpetaSalida);
		this.gbSeleccionarCarpetaSalida.Controls.Add(this.txtCarpetaSalida);
		this.gbSeleccionarCarpetaSalida.Location = new System.Drawing.Point(5, 249);
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
		this.progressBar1.Location = new System.Drawing.Point(5, 313);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(1340, 30);
		this.progressBar1.TabIndex = 10;
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
		this.dgvLotesDisponibles.Location = new System.Drawing.Point(11, 22);
		this.dgvLotesDisponibles.Name = "dgvLotesDisponibles";
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
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesSeleccionados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.dgvLotesSeleccionados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvLotesSeleccionados.DefaultCellStyle = dataGridViewCellStyle10;
		this.dgvLotesSeleccionados.EnableHeadersVisualStyles = false;
		this.dgvLotesSeleccionados.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvLotesSeleccionados.Location = new System.Drawing.Point(11, 22);
		this.dgvLotesSeleccionados.Name = "dgvLotesSeleccionados";
		this.dgvLotesSeleccionados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvLotesSeleccionados.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
		this.dgvLotesSeleccionados.RowHeadersVisible = false;
		this.dgvLotesSeleccionados.RowHeadersWidth = 15;
		dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle12.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
		this.dgvLotesSeleccionados.RowsDefaultCellStyle = dataGridViewCellStyle12;
		this.dgvLotesSeleccionados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvLotesSeleccionados.Size = new System.Drawing.Size(472, 140);
		this.dgvLotesSeleccionados.TabIndex = 18;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1534, 374);
		base.Controls.Add(this.pnlLotesSeleccionados);
		base.Controls.Add(this.btnQuitarLotes);
		base.Controls.Add(this.btnAgregarLotes);
		base.Controls.Add(this.btnDeseleccionarTodo);
		base.Controls.Add(this.btnSeleccionarTodo);
		base.Controls.Add(this.pnlLotesDisponibles);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.dgvTotalLotesEncontrados);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.gbSeleccionarCarpetaSalida);
		base.Name = "frmOCR";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Convertir Lotes a PDF con OCR";
		base.Load += new System.EventHandler(frmOCR_Load);
		this.gbSeleccionarCarpetaSalida.ResumeLayout(false);
		this.gbSeleccionarCarpetaSalida.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvTotalLotesEncontrados).EndInit();
		this.pnlLotesDisponibles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesDisponibles).EndInit();
		this.pnlLotesSeleccionados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvLotesSeleccionados).EndInit();
		base.ResumeLayout(false);
	}
}
