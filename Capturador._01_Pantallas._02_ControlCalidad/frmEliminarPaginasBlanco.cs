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

public class frmEliminarPaginasBlanco : Form
{
	private eUsuario oUsuarioLogueado;

	private string rutaCarpeta;

	private List<string> ListaArchivos;

	private List<(string filePath, int pageIndex, bool seleccionado)> paginasBlanco = new List<(string, int, bool)>();

	private int currentPageIndex = 0;

	private int pagesPerView = 8;

	private int totalPaginas = 0;

	private int barraProgeso1 = 0;

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

	private Button btnDeseleccionarTodo;

	private Button btnSeleccionarTodo;

	private Button btnPaginaSiguiente;

	private Button btnPaginaAnterior;

	private Button btnBuscarPaginasBlanco;

	private Panel pnlVisorPaginasBlanco;

	private Button btnEliminarPaginasBlanco;

	private ProgressBar progressBar1;

	private GroupBox groupBox3;

	private ProgressBar progressBar2;

	private Label label4;

	private TextBox txtPaginasEncontradas;

	private Button btnCerrar;

	public event Action<string> OnProcesoTerminado;

	public frmEliminarPaginasBlanco(eUsuario pUsuarioLogueado, string pRutaCarpeta, List<string> pListaArchivos)
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
		progressBar1.Maximum = totalPaginas - 1;
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

	private async void btnBuscarPaginasBlanco_Click(object sender, EventArgs e)
	{
		btnBuscarPaginasBlanco.Enabled = false;
		paginasBlanco.Clear();
		bool buscarPares = ckbPaginasPares.Checked;
		int tamanoLimiteKB = int.Parse(txtTamanoArchivo.Text);
		progressBar1.Value = 0;
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
						Invoke((MethodInvoker)delegate
						{
							ProgressBar progressBar = progressBar1;
							frmEliminarPaginasBlanco obj = this;
							int num2 = barraProgeso1;
							obj.barraProgeso1 = num2 + 1;
							progressBar.Value = num2;
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
	}

	private void habilitarBotones()
	{
		btnPaginaAnterior.Enabled = true;
		btnPaginaSiguiente.Enabled = true;
		btnSeleccionarTodo.Enabled = true;
		btnDeseleccionarTodo.Enabled = true;
		btnEliminarPaginasBlanco.Enabled = true;
	}

	private async void UpdateViewer()
	{
		pnlVisorPaginasBlanco.Controls.Clear();
		pagesPerView = (int)cbxCantidadPaginas.SelectedItem;
		int columns = ((pagesPerView >= 4) ? (pagesPerView / 2) : pagesPerView);
		int rows = ((pagesPerView < 4) ? 1 : 2);
		List<(string filePath, int pageIndex, bool seleccionado)> paginasMostrar = paginasBlanco.Skip(currentPageIndex).Take(pagesPerView).ToList();
		Dictionary<string, Image> imagenesCacheadas = new Dictionary<string, Image>();
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
			if (!imagenesCacheadas.ContainsKey(fullFilePath + pageIndex))
			{
				Image image = await RenderPdfPageAsync(fullFilePath, pageIndex, pictureBox.Width, pictureBox.Height);
				imagenesCacheadas[fullFilePath + pageIndex] = image;
				pictureBox.Image = image;
			}
			else
			{
				pictureBox.Image = imagenesCacheadas[fullFilePath + pageIndex];
			}
			string nombreArchivo = Path.GetFileNameWithoutExtension(filePath);
			CheckBox checkBox = new CheckBox
			{
				Text = $"Página {pageIndex + 1} - {nombreArchivo}",
				Checked = seleccionado,
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
				checkBox.Checked = !checkBox.Checked;
			};
			panel.Controls.Add(pictureBox);
			panel.Controls.Add(checkBox);
			pnlVisorPaginasBlanco.Controls.Add(panel);
			panel.Left = paginasMostrar.IndexOf((filePath, pageIndex, seleccionado)) % columns * panel.Width;
			panel.Top = paginasMostrar.IndexOf((filePath, pageIndex, seleccionado)) / columns * panel.Height;
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

	private void btnPaginaSiguiente_Click(object sender, EventArgs e)
	{
		if (currentPageIndex + pagesPerView < paginasBlanco.Count)
		{
			currentPageIndex += pagesPerView;
			UpdateViewer();
		}
	}

	private void btnPaginaAnterior_Click(object sender, EventArgs e)
	{
		if (currentPageIndex - pagesPerView >= 0)
		{
			currentPageIndex -= pagesPerView;
			UpdateViewer();
		}
	}

	private void btnSeleccionarTodo_Click(object sender, EventArgs e)
	{
		for (int i = 0; i < paginasBlanco.Count; i++)
		{
			paginasBlanco[i] = (paginasBlanco[i].filePath, paginasBlanco[i].pageIndex, true);
		}
		UpdateViewer();
	}

	private void btnDeseleccionarTodo_Click(object sender, EventArgs e)
	{
		for (int i = 0; i < paginasBlanco.Count; i++)
		{
			paginasBlanco[i] = (paginasBlanco[i].filePath, paginasBlanco[i].pageIndex, false);
		}
		UpdateViewer();
	}

	private async void btnEliminarPaginasBlanco_Click(object sender, EventArgs e)
	{
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
		btnEliminarPaginasBlanco.Enabled = true;
		TerminarProceso();
	}

	private void TerminarProceso()
	{
		this.OnProcesoTerminado?.Invoke(txtRutaCapeta.Text);
		Close();
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		TerminarProceso();
	}

	private void frmControlCalidad_v2_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Left:
			btnPaginaAnterior_Click(sender, e);
			break;
		case Keys.Right:
			btnPaginaSiguiente_Click(sender, e);
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
		this.groupBox1 = new System.Windows.Forms.GroupBox();
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
		this.btnDeseleccionarTodo = new System.Windows.Forms.Button();
		this.btnSeleccionarTodo = new System.Windows.Forms.Button();
		this.btnBuscarPaginasBlanco = new System.Windows.Forms.Button();
		this.pnlVisorPaginasBlanco = new System.Windows.Forms.Panel();
		this.btnEliminarPaginasBlanco = new System.Windows.Forms.Button();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.progressBar2 = new System.Windows.Forms.ProgressBar();
		this.label4 = new System.Windows.Forms.Label();
		this.txtPaginasEncontradas = new System.Windows.Forms.TextBox();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox3.SuspendLayout();
		base.SuspendLayout();
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
		this.groupBox1.Size = new System.Drawing.Size(1062, 107);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Configuración";
		this.btnCerrar.Location = new System.Drawing.Point(920, 40);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(119, 42);
		this.btnCerrar.TabIndex = 8;
		this.btnCerrar.Text = "Cerrar";
		this.btnCerrar.UseVisualStyleBackColor = true;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.ckbPaginasPares.AutoSize = true;
		this.ckbPaginasPares.Checked = true;
		this.ckbPaginasPares.CheckState = System.Windows.Forms.CheckState.Checked;
		this.ckbPaginasPares.Location = new System.Drawing.Point(490, 64);
		this.ckbPaginasPares.Name = "ckbPaginasPares";
		this.ckbPaginasPares.Size = new System.Drawing.Size(165, 17);
		this.ckbPaginasPares.TabIndex = 7;
		this.ckbPaginasPares.Text = "Buscar solo en páginas pares";
		this.ckbPaginasPares.UseVisualStyleBackColor = true;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(7, 30);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(47, 13);
		this.label3.TabIndex = 5;
		this.label3.Text = "Carpeta:";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(487, 31);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(150, 13);
		this.label2.TabIndex = 4;
		this.label2.Text = "Cantidad de Página a Mostrar:";
		this.cbxCantidadPaginas.FormattingEnabled = true;
		this.cbxCantidadPaginas.Location = new System.Drawing.Point(643, 27);
		this.cbxCantidadPaginas.Name = "cbxCantidadPaginas";
		this.cbxCantidadPaginas.Size = new System.Drawing.Size(121, 21);
		this.cbxCantidadPaginas.TabIndex = 3;
		this.txtTamanoArchivo.Location = new System.Drawing.Point(120, 62);
		this.txtTamanoArchivo.Name = "txtTamanoArchivo";
		this.txtTamanoArchivo.Size = new System.Drawing.Size(100, 20);
		this.txtTamanoArchivo.TabIndex = 2;
		this.txtTamanoArchivo.Text = "75";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(7, 62);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(100, 13);
		this.label1.TabIndex = 1;
		this.label1.Text = "Tamaño de Pagina:";
		this.txtRutaCapeta.Enabled = false;
		this.txtRutaCapeta.Location = new System.Drawing.Point(120, 28);
		this.txtRutaCapeta.Name = "txtRutaCapeta";
		this.txtRutaCapeta.Size = new System.Drawing.Size(334, 20);
		this.txtRutaCapeta.TabIndex = 0;
		this.groupBox2.Controls.Add(this.btnPaginaSiguiente);
		this.groupBox2.Controls.Add(this.btnPaginaAnterior);
		this.groupBox2.Location = new System.Drawing.Point(13, 198);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(290, 79);
		this.groupBox2.TabIndex = 1;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Navegación";
		this.btnPaginaSiguiente.Enabled = false;
		this.btnPaginaSiguiente.Location = new System.Drawing.Point(143, 20);
		this.btnPaginaSiguiente.Name = "btnPaginaSiguiente";
		this.btnPaginaSiguiente.Size = new System.Drawing.Size(130, 50);
		this.btnPaginaSiguiente.TabIndex = 1;
		this.btnPaginaSiguiente.Text = "Página Siguiente";
		this.btnPaginaSiguiente.UseVisualStyleBackColor = true;
		this.btnPaginaSiguiente.Click += new System.EventHandler(btnPaginaSiguiente_Click);
		this.btnPaginaAnterior.Enabled = false;
		this.btnPaginaAnterior.Location = new System.Drawing.Point(7, 20);
		this.btnPaginaAnterior.Name = "btnPaginaAnterior";
		this.btnPaginaAnterior.Size = new System.Drawing.Size(130, 50);
		this.btnPaginaAnterior.TabIndex = 0;
		this.btnPaginaAnterior.Text = "Página Anterior";
		this.btnPaginaAnterior.UseVisualStyleBackColor = true;
		this.btnPaginaAnterior.Click += new System.EventHandler(btnPaginaAnterior_Click);
		this.btnDeseleccionarTodo.Enabled = false;
		this.btnDeseleccionarTodo.Location = new System.Drawing.Point(142, 19);
		this.btnDeseleccionarTodo.Name = "btnDeseleccionarTodo";
		this.btnDeseleccionarTodo.Size = new System.Drawing.Size(130, 50);
		this.btnDeseleccionarTodo.TabIndex = 3;
		this.btnDeseleccionarTodo.Text = "Deseleccionar Todo";
		this.btnDeseleccionarTodo.UseVisualStyleBackColor = true;
		this.btnDeseleccionarTodo.Click += new System.EventHandler(btnDeseleccionarTodo_Click);
		this.btnSeleccionarTodo.Enabled = false;
		this.btnSeleccionarTodo.Location = new System.Drawing.Point(6, 19);
		this.btnSeleccionarTodo.Name = "btnSeleccionarTodo";
		this.btnSeleccionarTodo.Size = new System.Drawing.Size(130, 50);
		this.btnSeleccionarTodo.TabIndex = 2;
		this.btnSeleccionarTodo.Text = "Seleccionar Todo";
		this.btnSeleccionarTodo.UseVisualStyleBackColor = true;
		this.btnSeleccionarTodo.Click += new System.EventHandler(btnSeleccionarTodo_Click);
		this.btnBuscarPaginasBlanco.Location = new System.Drawing.Point(13, 122);
		this.btnBuscarPaginasBlanco.Name = "btnBuscarPaginasBlanco";
		this.btnBuscarPaginasBlanco.Size = new System.Drawing.Size(1062, 40);
		this.btnBuscarPaginasBlanco.TabIndex = 2;
		this.btnBuscarPaginasBlanco.Text = "Buscar Páginas en Blanco";
		this.btnBuscarPaginasBlanco.UseVisualStyleBackColor = true;
		this.btnBuscarPaginasBlanco.Click += new System.EventHandler(btnBuscarPaginasBlanco_Click);
		this.pnlVisorPaginasBlanco.Location = new System.Drawing.Point(13, 283);
		this.pnlVisorPaginasBlanco.Name = "pnlVisorPaginasBlanco";
		this.pnlVisorPaginasBlanco.Size = new System.Drawing.Size(1060, 343);
		this.pnlVisorPaginasBlanco.TabIndex = 3;
		this.btnEliminarPaginasBlanco.Enabled = false;
		this.btnEliminarPaginasBlanco.Location = new System.Drawing.Point(11, 632);
		this.btnEliminarPaginasBlanco.Name = "btnEliminarPaginasBlanco";
		this.btnEliminarPaginasBlanco.Size = new System.Drawing.Size(1062, 40);
		this.btnEliminarPaginasBlanco.TabIndex = 4;
		this.btnEliminarPaginasBlanco.Text = "Eliminar Todas las Páginas en Blanco";
		this.btnEliminarPaginasBlanco.UseVisualStyleBackColor = true;
		this.btnEliminarPaginasBlanco.Click += new System.EventHandler(btnEliminarPaginasBlanco_Click);
		this.progressBar1.Location = new System.Drawing.Point(13, 169);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(1062, 23);
		this.progressBar1.TabIndex = 5;
		this.groupBox3.Controls.Add(this.btnSeleccionarTodo);
		this.groupBox3.Controls.Add(this.btnDeseleccionarTodo);
		this.groupBox3.Location = new System.Drawing.Point(309, 198);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(285, 79);
		this.groupBox3.TabIndex = 6;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "Selección";
		this.progressBar2.Location = new System.Drawing.Point(11, 678);
		this.progressBar2.Name = "progressBar2";
		this.progressBar2.Size = new System.Drawing.Size(1062, 23);
		this.progressBar2.TabIndex = 7;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(601, 233);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(111, 13);
		this.label4.TabIndex = 8;
		this.label4.Text = "Páginas Encontradas:";
		this.txtPaginasEncontradas.Enabled = false;
		this.txtPaginasEncontradas.Location = new System.Drawing.Point(718, 234);
		this.txtPaginasEncontradas.Name = "txtPaginasEncontradas";
		this.txtPaginasEncontradas.Size = new System.Drawing.Size(100, 20);
		this.txtPaginasEncontradas.TabIndex = 9;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1085, 719);
		base.Controls.Add(this.txtPaginasEncontradas);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.progressBar2);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.btnEliminarPaginasBlanco);
		base.Controls.Add(this.pnlVisorPaginasBlanco);
		base.Controls.Add(this.btnBuscarPaginasBlanco);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmEliminarPaginasBlanco";
		this.Text = "Eliminación Masiva de Páginas en Blanco";
		base.Load += new System.EventHandler(frmEliminarPaginasBlanco_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox3.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
