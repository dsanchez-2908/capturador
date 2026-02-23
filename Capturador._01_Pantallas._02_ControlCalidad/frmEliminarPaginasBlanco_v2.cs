using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capturador._04_Entidades;
using PdfiumViewer;

namespace Capturador._01_Pantallas._02_ControlCalidad;

public class frmEliminarPaginasBlanco_v2 : Form
{
	private eUsuario oUsuarioLogueado;

	private string rutaCarpeta;

	private List<string> ListaArchivos;

	private List<(string filePath, int pageIndex, bool seleccionado)> paginasBlanco = new List<(string, int, bool)>();

	private Dictionary<string, Image> imagenesCacheadas = new Dictionary<string, Image>();

	private int currentPageIndex = 0;

	private int pagesPerView = 8;

	private int totalPaginas = 0;

	private FormWindowState LastWindowState = FormWindowState.Minimized;

	private IContainer components = null;

	private GroupBox groupBox1;

	private CheckBox ckbPaginasPares;

	private Label label3;

	private Label label2;

	private ComboBox cbxCantidadPaginas;

	private TextBox txtTamanoArchivo;

	private Label label1;

	private TextBox txtRutaCapeta;

	private GroupBox groupBox2;

	private Panel pnlVisorPaginasBlanco;

	private ProgressBar progressBar1;

	private GroupBox groupBox3;

	private ProgressBar progressBar2;

	private Label label4;

	private TextBox txtPaginasEncontradas;

	private Button btnCerrar;

	private Button btnBuscarPaginasBlanco;

	private Button btnPaginaSiguiente;

	private Button btnPaginaAnterior;

	private Button btnDeseleccionarTodo;

	private Button btnSeleccionarTodo;

	private Button btnEliminarPaginasBlanco;

	private Button btnZoom;

	private GroupBox groupBox4;

	private TextBox txtSelectZoom;

	public event Action<string> OnProcesoTerminado;

	public frmEliminarPaginasBlanco_v2(eUsuario pUsuarioLogueado, string pRutaCarpeta, List<string> pListaArchivos)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
		rutaCarpeta = pRutaCarpeta;
		ListaArchivos = pListaArchivos;
		cbxCantidadPaginas.Items.AddRange(new object[5] { 1, 2, 4, 6, 8 });
		cbxCantidadPaginas.SelectedIndex = 4;
		cbxCantidadPaginas.SelectedIndexChanged += delegate
		{
			UpdateViewer();
		};
		txtRutaCapeta.Text = rutaCarpeta;
		base.KeyPreview = true;
		base.KeyDown += frmControlCalidad_v2_KeyDown;
	}

	private void frmEliminarPaginasBlanco_Load(object sender, EventArgs e)
	{
		totalPaginas = cantidadTotalPaginas(rutaCarpeta, ListaArchivos);
		progressBar1.Maximum = totalPaginas;
		deshabilitarBotones();
	}

	private void establecerAnchor()
	{
		btnEliminarPaginasBlanco.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
	}

	private void ajustarFormulario_Buscar()
	{
		base.MaximizeBox = true;
		base.Size = new Size(1250, 775);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_No_Buscar()
	{
		base.Size = new Size(1250, 150);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void ajustarFormulario_Barra_Buscar()
	{
		base.Size = new Size(1250, 185);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	public static int cantidadTotalPaginas(string pRutaCarpeta, List<string> pListaArchivos)
	{
		int totalPaginas = 0;
		foreach (string archivo in pListaArchivos)
		{
			string rutaArchivo = Path.Combine(pRutaCarpeta, archivo);
			if (File.Exists(rutaArchivo))
			{
				try
				{
					using PdfDocument pdfDocument = PdfDocument.Load(rutaArchivo);
					totalPaginas += pdfDocument.PageCount;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error procesando " + archivo + ": " + ex.Message);
				}
			}
			else
			{
				Console.WriteLine("Archivo no encontrado: " + rutaArchivo);
			}
		}
		return totalPaginas;
	}

	private void habilitarBotones()
	{
		btnPaginaAnterior.BackColor = Color.SeaGreen;
		btnPaginaSiguiente.BackColor = Color.SeaGreen;
		btnSeleccionarTodo.BackColor = Color.SeaGreen;
		btnDeseleccionarTodo.BackColor = Color.SeaGreen;
		btnZoom.BackColor = Color.SeaGreen;
		btnEliminarPaginasBlanco.BackColor = Color.SeaGreen;
		btnPaginaAnterior.Enabled = true;
		btnPaginaSiguiente.Enabled = true;
		btnSeleccionarTodo.Enabled = true;
		btnDeseleccionarTodo.Enabled = true;
		btnZoom.Enabled = true;
		btnEliminarPaginasBlanco.Enabled = true;
	}

	private void deshabilitarBotones()
	{
		btnPaginaAnterior.BackColor = Color.Gray;
		btnPaginaSiguiente.BackColor = Color.Gray;
		btnSeleccionarTodo.BackColor = Color.Gray;
		btnDeseleccionarTodo.BackColor = Color.Gray;
		btnZoom.BackColor = Color.Gray;
		btnEliminarPaginasBlanco.BackColor = Color.Gray;
		btnPaginaAnterior.Enabled = false;
		btnPaginaSiguiente.Enabled = false;
		btnSeleccionarTodo.Enabled = false;
		btnDeseleccionarTodo.Enabled = false;
		btnZoom.Enabled = false;
		btnEliminarPaginasBlanco.Enabled = false;
	}

	private async void PrecacheSiguientePagina()
	{
		List<(string filePath, int pageIndex, bool seleccionado)> paginasPrecargar = paginasBlanco.Skip(currentPageIndex + pagesPerView).Take(pagesPerView).ToList();
		foreach (var item in paginasPrecargar)
		{
			string filePath = item.filePath;
			int pageIndex = item.pageIndex;
			string fullPath = Path.Combine(rutaCarpeta, filePath);
			string cacheKey = fullPath + "_" + pageIndex;
			if (!imagenesCacheadas.ContainsKey(cacheKey))
			{
				Image img = await RenderPdfPageAsync(fullPath, pageIndex, 100, 100);
				if (img != null)
				{
					imagenesCacheadas[cacheKey] = img;
				}
			}
		}
	}

	private async Task PrecargarTodasLasImagenes()
	{
		int total = paginasBlanco.Count;
		for (int i = 0; i < total; i++)
		{
			(string, int, bool) tuple = paginasBlanco[i];
			string filePath = tuple.Item1;
			int pageIndex = tuple.Item2;
			string fullPath = Path.Combine(rutaCarpeta, filePath);
			string cacheKey = fullPath + "_" + pageIndex;
			if (!imagenesCacheadas.ContainsKey(cacheKey))
			{
				Image img = await RenderPdfPageAsync(fullPath, pageIndex, 300, 400);
				if (img != null)
				{
					imagenesCacheadas[cacheKey] = img;
				}
			}
			int progreso = 500 + (int)((double)(i + 1) / (double)total * 500.0);
			Invoke((MethodInvoker)delegate
			{
				progressBar1.Value = Math.Min(progreso, 1000);
			});
		}
	}

	private async void UpdateViewer()
	{
		pnlVisorPaginasBlanco.Controls.Clear();
		pagesPerView = (int)cbxCantidadPaginas.SelectedItem;
		int columns = ((pagesPerView >= 4) ? (pagesPerView / 2) : pagesPerView);
		int rows = ((pagesPerView < 4) ? 1 : 2);
		List<(string filePath, int pageIndex, bool seleccionado)> paginasMostrar = paginasBlanco.Skip(currentPageIndex).Take(pagesPerView).ToList();
		foreach (var item in paginasMostrar)
		{
			string filePath = item.filePath;
			int pageIndex = item.pageIndex;
			bool seleccionado = item.seleccionado;
			string fullFilePath = Path.Combine(rutaCarpeta, filePath);
			Panel panel = new Panel
			{
				Width = pnlVisorPaginasBlanco.Width / columns,
				Height = pnlVisorPaginasBlanco.Height / rows,
				BorderStyle = BorderStyle.FixedSingle
			};
			PictureBox pictureBox = new PictureBox
			{
				Width = panel.Width,
				Height = panel.Height - 20,
				SizeMode = PictureBoxSizeMode.Zoom,
				Cursor = Cursors.Hand
			};
			string cacheKey = fullFilePath + "_" + pageIndex;
			if (imagenesCacheadas.TryGetValue(cacheKey, out var cachedImage))
			{
				pictureBox.Image = cachedImage;
			}
			else
			{
				Image image = await RenderPdfPageAsync(fullFilePath, pageIndex, pictureBox.Width, pictureBox.Height);
				if (image != null)
				{
					imagenesCacheadas[cacheKey] = image;
					pictureBox.Image = image;
				}
			}
			string nombreArchivo = Path.GetFileNameWithoutExtension(filePath);
			CheckBox checkBox = new CheckBox
			{
				Text = $"Página {pageIndex + 1} - {nombreArchivo}",
				Checked = seleccionado,
				Font = new Font("Segoe UI", 8f, FontStyle.Regular),
				ForeColor = Color.White,
				Height = 15,
				TextAlign = ContentAlignment.MiddleCenter,
				Dock = DockStyle.Bottom
			};
			checkBox.CheckedChanged += delegate
			{
				int num = paginasBlanco.FindIndex(((string filePath, int pageIndex, bool seleccionado) p) => p.filePath == filePath && p.pageIndex == pageIndex);
				if (num >= 0)
				{
					paginasBlanco[num] = (filePath, pageIndex, checkBox.Checked);
				}
			};
			pictureBox.Click += delegate
			{
				if (txtSelectZoom.Text == "")
				{
					checkBox.Checked = !checkBox.Checked;
				}
			};
			panel.Controls.Add(pictureBox);
			panel.Controls.Add(checkBox);
			pnlVisorPaginasBlanco.Controls.Add(panel);
			pictureBox.MouseClick += PdfViewer_MouseClick;
			panel.Left = paginasMostrar.IndexOf((filePath, pageIndex, seleccionado)) % columns * panel.Width;
			panel.Top = paginasMostrar.IndexOf((filePath, pageIndex, seleccionado)) / columns * panel.Height;
			cachedImage = null;
		}
	}

	private async Task<Image> RenderPdfPageAsync(string filePath, int pageIndex, int width, int height)
	{
		return await Task.Run(delegate
		{
			using (PdfDocument pdfDocument = PdfDocument.Load(filePath))
			{
				if (pageIndex >= 0 && pageIndex < pdfDocument.PageCount)
				{
					using (Image original = pdfDocument.Render(pageIndex, width, height, forPrinting: true))
					{
						return new Bitmap(original);
					}
				}
			}
			return (Bitmap)null;
		});
	}

	private void TerminarProceso()
	{
		this.OnProcesoTerminado?.Invoke(txtRutaCapeta.Text);
		Close();
	}

	private void frmControlCalidad_v2_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Left:
			btnPaginaAnterior_Click_1(sender, e);
			break;
		case Keys.Right:
			btnPaginaSiguiente_Click_1(sender, e);
			break;
		}
	}

	private void btnCerrar_Click_1(object sender, EventArgs e)
	{
		TerminarProceso();
	}

	private void btnPaginaAnterior_Click_1(object sender, EventArgs e)
	{
		if (currentPageIndex - pagesPerView >= 0)
		{
			currentPageIndex -= pagesPerView;
			UpdateViewer();
		}
	}

	private void btnPaginaSiguiente_Click_1(object sender, EventArgs e)
	{
		if (currentPageIndex + pagesPerView < paginasBlanco.Count)
		{
			currentPageIndex += pagesPerView;
			UpdateViewer();
		}
	}

	private void btnSeleccionarTodo_Click_1(object sender, EventArgs e)
	{
		for (int i = 0; i < paginasBlanco.Count; i++)
		{
			paginasBlanco[i] = (paginasBlanco[i].filePath, paginasBlanco[i].pageIndex, true);
		}
		UpdateViewer();
	}

	private void btnDeseleccionarTodo_Click_1(object sender, EventArgs e)
	{
		for (int i = 0; i < paginasBlanco.Count; i++)
		{
			paginasBlanco[i] = (paginasBlanco[i].filePath, paginasBlanco[i].pageIndex, false);
		}
		UpdateViewer();
	}

	private async void btnBuscarPaginasBlanco_Click_1(object sender, EventArgs e)
	{
		progressBar1.Visible = true;
		progressBar1.Value = 0;
		progressBar1.Maximum = 1000;
		foreach (Image img in imagenesCacheadas.Values)
		{
			img.Dispose();
		}
		imagenesCacheadas.Clear();
		btnBuscarPaginasBlanco.Enabled = false;
		paginasBlanco.Clear();
		bool buscarPares = ckbPaginasPares.Checked;
		int tamanoLimiteKB = int.Parse(txtTamanoArchivo.Text);
		txtPaginasEncontradas.Text = "0";
		await Task.Run(delegate
		{
			int num = 0;
			List<(string file, int index, bool seleccionado)> paginasEncontradas = new List<(string, int, bool)>();
			foreach (string current in ListaArchivos)
			{
				string path = Path.Combine(rutaCarpeta, current);
				using PdfDocument pdfDocument = PdfDocument.Load(path);
				num += (buscarPares ? (pdfDocument.PageCount / 2) : pdfDocument.PageCount);
				for (int i = 0; i < pdfDocument.PageCount; i++)
				{
					if (!buscarPares || i % 2 != 0)
					{
						using (Image image = pdfDocument.Render(i, 30f, 30f, forPrinting: true))
						{
							using MemoryStream memoryStream = new MemoryStream();
							image.Save(memoryStream, ImageFormat.Jpeg);
							if (memoryStream.Length / 1024 < tamanoLimiteKB)
							{
								paginasEncontradas.Add((current, i, true));
							}
						}
						int pageCount = pdfDocument.PageCount;
						int progresoLocal = (int)(((double)progressBar1.Value + 1.0) / (double)num * 500.0);
						Invoke((MethodInvoker)delegate
						{
							progressBar1.Value = Math.Min(progresoLocal, 500);
						});
					}
				}
			}
			Invoke((MethodInvoker)delegate
			{
				paginasBlanco.AddRange(paginasEncontradas);
				txtPaginasEncontradas.Text = paginasBlanco.Count.ToString();
			});
		});
		btnBuscarPaginasBlanco.Enabled = true;
		habilitarBotones();
		UpdateViewer();
		await PrecargarTodasLasImagenes();
	}

	private async void btnEliminarPaginasBlanco_Click_1(object sender, EventArgs e)
	{
		if (MessageBox.Show("Esta seguro que quiere eliminar las páginas seleccionadas?", "Confirmar Eliminación de Páginas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		progressBar2.Visible = true;
		btnEliminarPaginasBlanco.Enabled = false;
		List<IGrouping<string, (string filePath, int pageIndex, bool seleccionado)>> paginasEliminar = (from p in paginasBlanco
			where p.seleccionado
			group p by p.filePath).ToList();
		progressBar2.Maximum = paginasEliminar.Count;
		progressBar2.Value = 0;
		await Task.Run(delegate
		{
			foreach (IGrouping<string, (string, int, bool)> current in paginasEliminar)
			{
				string text = Path.Combine(rutaCarpeta, current.Key);
				string text2 = text + ".tmp";
				try
				{
					using (PdfDocument pdfDocument = PdfDocument.Load(text))
					{
						foreach (var item in current.OrderByDescending<(string, int, bool), int>(((string filePath, int pageIndex, bool seleccionado) p) => p.pageIndex))
						{
							pdfDocument.DeletePage(item.Item2);
						}
						pdfDocument.Save(text2);
					}
					File.Delete(text);
					File.Move(text2, text);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error al procesar " + current.Key + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				Invoke((Action)delegate
				{
					progressBar2.Value++;
				});
			}
		});
		MessageBox.Show("Se eliminaron correctamente las páginas seleccionadas.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		foreach (Image img in imagenesCacheadas.Values)
		{
			img.Dispose();
		}
		imagenesCacheadas.Clear();
		btnEliminarPaginasBlanco.Enabled = true;
		TerminarProceso();
	}

	private void PdfViewer_MouseClick(object sender, MouseEventArgs e)
	{
		if (!(txtSelectZoom.Text == "ZOOM"))
		{
			return;
		}
		PictureBox pictureBox = sender as PictureBox;
		if (pictureBox == null)
		{
			return;
		}
		int pageIndex = currentPageIndex + pnlVisorPaginasBlanco.Controls.IndexOf(pictureBox.Parent);
		Form zoomForm = new Form
		{
			Text = "Vista ampliada",
			Size = new Size(800, 600),
			StartPosition = FormStartPosition.CenterScreen
		};
		PictureBox zoomPictureBox = new PictureBox
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
		float zoomFactor = 1f;
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
		void AplicarZoom(float factor)
		{
			zoomFactor *= factor;
			zoomPictureBox.Width = (int)((float)pictureBox.Image.Width * zoomFactor);
			zoomPictureBox.Height = (int)((float)pictureBox.Image.Height * zoomFactor);
		}
	}

	private void btnZoom_Click(object sender, EventArgs e)
	{
		if (txtSelectZoom.Text == "ZOOM")
		{
			txtSelectZoom.Text = "";
			btnZoom.BackColor = Color.SeaGreen;
		}
		else
		{
			txtSelectZoom.Text = "ZOOM";
			btnZoom.BackColor = Color.SteelBlue;
		}
	}

	private void frmEliminarPaginasBlanco_v2_Resize(object sender, EventArgs e)
	{
		if (base.WindowState != LastWindowState)
		{
			LastWindowState = base.WindowState;
			if (base.WindowState == FormWindowState.Maximized)
			{
				UpdateViewer();
			}
			if (base.WindowState == FormWindowState.Normal)
			{
				UpdateViewer();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._02_ControlCalidad.frmEliminarPaginasBlanco_v2));
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.btnBuscarPaginasBlanco = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.ckbPaginasPares = new System.Windows.Forms.CheckBox();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.cbxCantidadPaginas = new System.Windows.Forms.ComboBox();
		this.txtTamanoArchivo = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.txtRutaCapeta = new System.Windows.Forms.TextBox();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.btnPaginaSiguiente = new System.Windows.Forms.Button();
		this.btnPaginaAnterior = new System.Windows.Forms.Button();
		this.pnlVisorPaginasBlanco = new System.Windows.Forms.Panel();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.btnDeseleccionarTodo = new System.Windows.Forms.Button();
		this.btnSeleccionarTodo = new System.Windows.Forms.Button();
		this.progressBar2 = new System.Windows.Forms.ProgressBar();
		this.label4 = new System.Windows.Forms.Label();
		this.txtPaginasEncontradas = new System.Windows.Forms.TextBox();
		this.btnEliminarPaginasBlanco = new System.Windows.Forms.Button();
		this.btnZoom = new System.Windows.Forms.Button();
		this.groupBox4 = new System.Windows.Forms.GroupBox();
		this.txtSelectZoom = new System.Windows.Forms.TextBox();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.groupBox4.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupBox1.Controls.Add(this.btnBuscarPaginasBlanco);
		this.groupBox1.Controls.Add(this.btnCerrar);
		this.groupBox1.Controls.Add(this.ckbPaginasPares);
		this.groupBox1.Controls.Add(this.label3);
		this.groupBox1.Controls.Add(this.label2);
		this.groupBox1.Controls.Add(this.cbxCantidadPaginas);
		this.groupBox1.Controls.Add(this.txtTamanoArchivo);
		this.groupBox1.Controls.Add(this.label1);
		this.groupBox1.Controls.Add(this.txtRutaCapeta);
		this.groupBox1.Location = new System.Drawing.Point(13, 9);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(1209, 91);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.btnBuscarPaginasBlanco.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnBuscarPaginasBlanco.BackColor = System.Drawing.Color.SeaGreen;
		this.btnBuscarPaginasBlanco.FlatAppearance.BorderSize = 0;
		this.btnBuscarPaginasBlanco.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnBuscarPaginasBlanco.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBuscarPaginasBlanco.ForeColor = System.Drawing.Color.White;
		this.btnBuscarPaginasBlanco.Image = (System.Drawing.Image)resources.GetObject("btnBuscarPaginasBlanco.Image");
		this.btnBuscarPaginasBlanco.Location = new System.Drawing.Point(980, 51);
		this.btnBuscarPaginasBlanco.Name = "btnBuscarPaginasBlanco";
		this.btnBuscarPaginasBlanco.Size = new System.Drawing.Size(223, 25);
		this.btnBuscarPaginasBlanco.TabIndex = 59;
		this.btnBuscarPaginasBlanco.Text = "   Buscar Páginas en Blanco";
		this.btnBuscarPaginasBlanco.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnBuscarPaginasBlanco.UseVisualStyleBackColor = false;
		this.btnBuscarPaginasBlanco.Click += new System.EventHandler(btnBuscarPaginasBlanco_Click_1);
		this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(980, 16);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(223, 24);
		this.btnCerrar.TabIndex = 61;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click_1);
		this.ckbPaginasPares.AutoSize = true;
		this.ckbPaginasPares.Checked = true;
		this.ckbPaginasPares.CheckState = System.Windows.Forms.CheckState.Checked;
		this.ckbPaginasPares.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ckbPaginasPares.ForeColor = System.Drawing.Color.White;
		this.ckbPaginasPares.Location = new System.Drawing.Point(531, 50);
		this.ckbPaginasPares.Name = "ckbPaginasPares";
		this.ckbPaginasPares.Size = new System.Drawing.Size(206, 20);
		this.ckbPaginasPares.TabIndex = 7;
		this.ckbPaginasPares.Text = "Buscar solo en páginas pares";
		this.ckbPaginasPares.UseVisualStyleBackColor = true;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(6, 20);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(59, 16);
		this.label3.TabIndex = 5;
		this.label3.Text = "Carpeta:";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(528, 20);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(189, 16);
		this.label2.TabIndex = 4;
		this.label2.Text = "Cantidad de Página a Mostrar:";
		this.cbxCantidadPaginas.FormattingEnabled = true;
		this.cbxCantidadPaginas.Location = new System.Drawing.Point(743, 18);
		this.cbxCantidadPaginas.Name = "cbxCantidadPaginas";
		this.cbxCantidadPaginas.Size = new System.Drawing.Size(121, 21);
		this.cbxCantidadPaginas.TabIndex = 3;
		this.txtTamanoArchivo.Location = new System.Drawing.Point(143, 50);
		this.txtTamanoArchivo.Name = "txtTamanoArchivo";
		this.txtTamanoArchivo.Size = new System.Drawing.Size(100, 20);
		this.txtTamanoArchivo.TabIndex = 2;
		this.txtTamanoArchivo.Text = "75";
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(6, 51);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(127, 16);
		this.label1.TabIndex = 1;
		this.label1.Text = "Tamaño de Pagina:";
		this.txtRutaCapeta.Enabled = false;
		this.txtRutaCapeta.Location = new System.Drawing.Point(143, 19);
		this.txtRutaCapeta.Name = "txtRutaCapeta";
		this.txtRutaCapeta.Size = new System.Drawing.Size(334, 20);
		this.txtRutaCapeta.TabIndex = 0;
		this.groupBox2.Controls.Add(this.btnPaginaSiguiente);
		this.groupBox2.Controls.Add(this.btnPaginaAnterior);
		this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.groupBox2.ForeColor = System.Drawing.Color.White;
		this.groupBox2.Location = new System.Drawing.Point(11, 140);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(191, 79);
		this.groupBox2.TabIndex = 1;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Navegación";
		this.btnPaginaSiguiente.BackColor = System.Drawing.Color.SeaGreen;
		this.btnPaginaSiguiente.FlatAppearance.BorderSize = 0;
		this.btnPaginaSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnPaginaSiguiente.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnPaginaSiguiente.ForeColor = System.Drawing.Color.White;
		this.btnPaginaSiguiente.Image = (System.Drawing.Image)resources.GetObject("btnPaginaSiguiente.Image");
		this.btnPaginaSiguiente.Location = new System.Drawing.Point(97, 19);
		this.btnPaginaSiguiente.Name = "btnPaginaSiguiente";
		this.btnPaginaSiguiente.Size = new System.Drawing.Size(75, 50);
		this.btnPaginaSiguiente.TabIndex = 68;
		this.btnPaginaSiguiente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnPaginaSiguiente.UseVisualStyleBackColor = false;
		this.btnPaginaSiguiente.Click += new System.EventHandler(btnPaginaSiguiente_Click_1);
		this.btnPaginaAnterior.BackColor = System.Drawing.Color.SeaGreen;
		this.btnPaginaAnterior.FlatAppearance.BorderSize = 0;
		this.btnPaginaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnPaginaAnterior.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnPaginaAnterior.ForeColor = System.Drawing.Color.White;
		this.btnPaginaAnterior.Image = (System.Drawing.Image)resources.GetObject("btnPaginaAnterior.Image");
		this.btnPaginaAnterior.Location = new System.Drawing.Point(11, 19);
		this.btnPaginaAnterior.Name = "btnPaginaAnterior";
		this.btnPaginaAnterior.Size = new System.Drawing.Size(75, 50);
		this.btnPaginaAnterior.TabIndex = 67;
		this.btnPaginaAnterior.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnPaginaAnterior.UseVisualStyleBackColor = false;
		this.btnPaginaAnterior.Click += new System.EventHandler(btnPaginaAnterior_Click_1);
		this.pnlVisorPaginasBlanco.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlVisorPaginasBlanco.BackColor = System.Drawing.Color.Black;
		this.pnlVisorPaginasBlanco.Location = new System.Drawing.Point(11, 225);
		this.pnlVisorPaginasBlanco.Name = "pnlVisorPaginasBlanco";
		this.pnlVisorPaginasBlanco.Size = new System.Drawing.Size(1211, 419);
		this.pnlVisorPaginasBlanco.TabIndex = 3;
		this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.progressBar1.Location = new System.Drawing.Point(13, 111);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(1209, 23);
		this.progressBar1.TabIndex = 5;
		this.groupBox3.Controls.Add(this.btnDeseleccionarTodo);
		this.groupBox3.Controls.Add(this.btnSeleccionarTodo);
		this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.groupBox3.ForeColor = System.Drawing.Color.White;
		this.groupBox3.Location = new System.Drawing.Point(208, 140);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(383, 79);
		this.groupBox3.TabIndex = 6;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "Selección";
		this.btnDeseleccionarTodo.BackColor = System.Drawing.Color.SeaGreen;
		this.btnDeseleccionarTodo.FlatAppearance.BorderSize = 0;
		this.btnDeseleccionarTodo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnDeseleccionarTodo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnDeseleccionarTodo.ForeColor = System.Drawing.Color.White;
		this.btnDeseleccionarTodo.Image = (System.Drawing.Image)resources.GetObject("btnDeseleccionarTodo.Image");
		this.btnDeseleccionarTodo.Location = new System.Drawing.Point(187, 19);
		this.btnDeseleccionarTodo.Name = "btnDeseleccionarTodo";
		this.btnDeseleccionarTodo.Size = new System.Drawing.Size(175, 50);
		this.btnDeseleccionarTodo.TabIndex = 70;
		this.btnDeseleccionarTodo.Text = "   Desleccionar Todo";
		this.btnDeseleccionarTodo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnDeseleccionarTodo.UseVisualStyleBackColor = false;
		this.btnDeseleccionarTodo.Click += new System.EventHandler(btnDeseleccionarTodo_Click_1);
		this.btnSeleccionarTodo.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarTodo.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarTodo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarTodo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarTodo.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarTodo.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarTodo.Image");
		this.btnSeleccionarTodo.Location = new System.Drawing.Point(6, 19);
		this.btnSeleccionarTodo.Name = "btnSeleccionarTodo";
		this.btnSeleccionarTodo.Size = new System.Drawing.Size(175, 50);
		this.btnSeleccionarTodo.TabIndex = 69;
		this.btnSeleccionarTodo.Text = "   Seleccionar Todo";
		this.btnSeleccionarTodo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarTodo.UseVisualStyleBackColor = false;
		this.btnSeleccionarTodo.Click += new System.EventHandler(btnSeleccionarTodo_Click_1);
		this.progressBar2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.progressBar2.Location = new System.Drawing.Point(11, 696);
		this.progressBar2.Name = "progressBar2";
		this.progressBar2.Size = new System.Drawing.Size(1211, 23);
		this.progressBar2.TabIndex = 7;
		this.progressBar2.Visible = false;
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(707, 175);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(140, 16);
		this.label4.TabIndex = 8;
		this.label4.Text = "Páginas Encontradas:";
		this.txtPaginasEncontradas.Enabled = false;
		this.txtPaginasEncontradas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtPaginasEncontradas.Location = new System.Drawing.Point(853, 171);
		this.txtPaginasEncontradas.Name = "txtPaginasEncontradas";
		this.txtPaginasEncontradas.Size = new System.Drawing.Size(100, 22);
		this.txtPaginasEncontradas.TabIndex = 9;
		this.btnEliminarPaginasBlanco.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btnEliminarPaginasBlanco.BackColor = System.Drawing.Color.SeaGreen;
		this.btnEliminarPaginasBlanco.FlatAppearance.BorderSize = 0;
		this.btnEliminarPaginasBlanco.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnEliminarPaginasBlanco.Font = new System.Drawing.Font("Century Gothic", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnEliminarPaginasBlanco.ForeColor = System.Drawing.Color.White;
		this.btnEliminarPaginasBlanco.Image = (System.Drawing.Image)resources.GetObject("btnEliminarPaginasBlanco.Image");
		this.btnEliminarPaginasBlanco.Location = new System.Drawing.Point(11, 650);
		this.btnEliminarPaginasBlanco.Name = "btnEliminarPaginasBlanco";
		this.btnEliminarPaginasBlanco.Size = new System.Drawing.Size(1211, 40);
		this.btnEliminarPaginasBlanco.TabIndex = 71;
		this.btnEliminarPaginasBlanco.Text = "   Eliminar todas las páginas en Blanco Seleccionado";
		this.btnEliminarPaginasBlanco.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnEliminarPaginasBlanco.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnEliminarPaginasBlanco.UseVisualStyleBackColor = false;
		this.btnEliminarPaginasBlanco.Click += new System.EventHandler(btnEliminarPaginasBlanco_Click_1);
		this.btnZoom.BackColor = System.Drawing.Color.SeaGreen;
		this.btnZoom.FlatAppearance.BorderSize = 0;
		this.btnZoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnZoom.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnZoom.ForeColor = System.Drawing.Color.White;
		this.btnZoom.Image = (System.Drawing.Image)resources.GetObject("btnZoom.Image");
		this.btnZoom.Location = new System.Drawing.Point(16, 19);
		this.btnZoom.Name = "btnZoom";
		this.btnZoom.Size = new System.Drawing.Size(50, 50);
		this.btnZoom.TabIndex = 72;
		this.btnZoom.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnZoom.UseVisualStyleBackColor = false;
		this.btnZoom.Click += new System.EventHandler(btnZoom_Click);
		this.groupBox4.Controls.Add(this.btnZoom);
		this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.groupBox4.ForeColor = System.Drawing.Color.White;
		this.groupBox4.Location = new System.Drawing.Point(597, 140);
		this.groupBox4.Name = "groupBox4";
		this.groupBox4.Size = new System.Drawing.Size(86, 79);
		this.groupBox4.TabIndex = 71;
		this.groupBox4.TabStop = false;
		this.groupBox4.Text = "Zoom";
		this.txtSelectZoom.Location = new System.Drawing.Point(699, 140);
		this.txtSelectZoom.Name = "txtSelectZoom";
		this.txtSelectZoom.Size = new System.Drawing.Size(100, 20);
		this.txtSelectZoom.TabIndex = 72;
		this.txtSelectZoom.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1234, 731);
		base.Controls.Add(this.progressBar2);
		base.Controls.Add(this.btnEliminarPaginasBlanco);
		base.Controls.Add(this.pnlVisorPaginasBlanco);
		base.Controls.Add(this.txtSelectZoom);
		base.Controls.Add(this.groupBox4);
		base.Controls.Add(this.txtPaginasEncontradas);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.MinimizeBox = false;
		base.Name = "frmEliminarPaginasBlanco_v2";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Eliminación Masiva de Páginas en Blanco";
		base.Load += new System.EventHandler(frmEliminarPaginasBlanco_Load);
		base.Resize += new System.EventHandler(frmEliminarPaginasBlanco_v2_Resize);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox3.ResumeLayout(false);
		this.groupBox4.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
