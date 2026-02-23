using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Capturador._04_Entidades;
using PdfiumViewer;

namespace Capturador._01_Pantallas._02_ControlCalidad;

public class frmControlCalidad : Form
{
	private eUsuario oUsuarioLogueado;

	private PdfViewer pdfViewer;

	private string[] archivosPDF;

	private int archivoActual = 0;

	private Dictionary<int, int> rotacionesPaginas = new Dictionary<int, int>();

	private IContainer components = null;

	private GroupBox groupBox1;

	private TextBox txtRutaCarpeta;

	private Button btnSeleccionarCarpeta;

	private Panel panelVisor;

	private ListBox listBoxArchivos;

	private PictureBox picVisor;

	private ToolStrip toolStrip1;

	private ToolStripButton toolStripButton1;

	private ToolStripDropDownButton toolStripDropDownButton1;

	private ToolStripSplitButton toolStripSplitButton1;

	public frmControlCalidad(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
		InicializarVisor();
	}

	private void frmControlCalidad_Load(object sender, EventArgs e)
	{
	}

	private void InicializarVisor()
	{
		pdfViewer = new PdfViewer
		{
			Dock = DockStyle.Fill,
			ZoomMode = PdfViewerZoomMode.FitWidth
		};
		picVisor = new PictureBox
		{
			Dock = DockStyle.Fill,
			SizeMode = PictureBoxSizeMode.Zoom,
			Visible = false
		};
		panelVisor.Controls.Add(pdfViewer);
		panelVisor.Controls.Add(picVisor);
		base.KeyPreview = true;
		base.KeyDown += Form1_KeyDown;
	}

	private void btnSeleccionarCarpeta_Click_1(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLote = new FolderBrowserDialog();
		oSeleccionarLote.Description = "Seleccionar la carpeta a procesar";
		oSeleccionarLote.SelectedPath = "C:\\Lote\\Despachos\\FTP";
		oSeleccionarLote.ShowNewFolderButton = false;
		if (oSeleccionarLote.ShowDialog() == DialogResult.OK)
		{
			archivosPDF = (from f in Directory.GetFiles(oSeleccionarLote.SelectedPath, "*.pdf")
				orderby f
				select f).ToArray();
			listBoxArchivos.DataSource = archivosPDF.Select(Path.GetFileName).ToList();
			archivoActual = 0;
			CargarPDF();
		}
	}

	private void listBoxArchivos_SelectedIndexChanged_1(object sender, EventArgs e)
	{
		archivoActual = listBoxArchivos.SelectedIndex;
		CargarPDF();
	}

	private void CargarPDF()
	{
		if (archivosPDF != null && archivosPDF.Length != 0 && archivoActual >= 0)
		{
			if (pdfViewer.Document != null)
			{
				pdfViewer.Document.Dispose();
			}
			pdfViewer.Document = PdfDocument.Load(archivosPDF[archivoActual]);
			pdfViewer.Visible = true;
			picVisor.Visible = false;
		}
	}

	private void Form1_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Right)
		{
			if (pdfViewer.Visible)
			{
				pdfViewer.Renderer.Page++;
			}
		}
		else if (e.KeyCode == Keys.Left)
		{
			if (pdfViewer.Visible)
			{
				pdfViewer.Renderer.Page--;
			}
		}
		else if (e.KeyCode == Keys.Down)
		{
			if (archivoActual < archivosPDF.Length - 1)
			{
				archivoActual++;
				listBoxArchivos.SelectedIndex = archivoActual;
			}
		}
		else if (e.KeyCode == Keys.Up)
		{
			if (archivoActual > 0)
			{
				archivoActual--;
				listBoxArchivos.SelectedIndex = archivoActual;
			}
		}
		else if (e.KeyCode == Keys.R)
		{
			RotarPagina(RotateFlipType.Rotate90FlipNone);
		}
		else if (e.KeyCode == Keys.L)
		{
			RotarPagina(RotateFlipType.Rotate270FlipNone);
		}
	}

	private void frmControlCalidad_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Right)
		{
			pdfViewer.Renderer.Page++;
		}
		else if (e.KeyCode == Keys.Left)
		{
			pdfViewer.Renderer.Page--;
		}
		else if (e.KeyCode == Keys.Down)
		{
			if (archivoActual < archivosPDF.Length - 1)
			{
				archivoActual++;
				listBoxArchivos.SelectedIndex = archivoActual;
			}
		}
		else if (e.KeyCode == Keys.Up)
		{
			if (archivoActual > 0)
			{
				archivoActual--;
				listBoxArchivos.SelectedIndex = archivoActual;
			}
		}
		else if (e.KeyCode == Keys.R)
		{
			RotarPagina(RotateFlipType.Rotate90FlipNone);
		}
		else if (e.KeyCode == Keys.L)
		{
			RotarPagina(RotateFlipType.Rotate270FlipNone);
		}
	}

	private void RotarPagina(RotateFlipType tipoRotacion)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._02_ControlCalidad.frmControlCalidad));
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.txtRutaCarpeta = new System.Windows.Forms.TextBox();
		this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
		this.panelVisor = new System.Windows.Forms.Panel();
		this.listBoxArchivos = new System.Windows.Forms.ListBox();
		this.picVisor = new System.Windows.Forms.PictureBox();
		this.toolStrip1 = new System.Windows.Forms.ToolStrip();
		this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
		this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
		this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
		this.groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.picVisor).BeginInit();
		this.toolStrip1.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.txtRutaCarpeta);
		this.groupBox1.Controls.Add(this.btnSeleccionarCarpeta);
		this.groupBox1.Location = new System.Drawing.Point(12, 111);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(752, 100);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "groupBox1";
		this.txtRutaCarpeta.Location = new System.Drawing.Point(189, 20);
		this.txtRutaCarpeta.Multiline = true;
		this.txtRutaCarpeta.Name = "txtRutaCarpeta";
		this.txtRutaCarpeta.Size = new System.Drawing.Size(549, 53);
		this.txtRutaCarpeta.TabIndex = 1;
		this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(7, 20);
		this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
		this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(175, 53);
		this.btnSeleccionarCarpeta.TabIndex = 0;
		this.btnSeleccionarCarpeta.Text = "Seleccionar Carpeta";
		this.btnSeleccionarCarpeta.UseVisualStyleBackColor = true;
		this.btnSeleccionarCarpeta.Click += new System.EventHandler(btnSeleccionarCarpeta_Click_1);
		this.panelVisor.Location = new System.Drawing.Point(12, 217);
		this.panelVisor.Name = "panelVisor";
		this.panelVisor.Size = new System.Drawing.Size(752, 465);
		this.panelVisor.TabIndex = 1;
		this.listBoxArchivos.FormattingEnabled = true;
		this.listBoxArchivos.Location = new System.Drawing.Point(770, 116);
		this.listBoxArchivos.Name = "listBoxArchivos";
		this.listBoxArchivos.Size = new System.Drawing.Size(329, 95);
		this.listBoxArchivos.TabIndex = 2;
		this.listBoxArchivos.SelectedIndexChanged += new System.EventHandler(listBoxArchivos_SelectedIndexChanged_1);
		this.picVisor.Location = new System.Drawing.Point(771, 218);
		this.picVisor.Name = "picVisor";
		this.picVisor.Size = new System.Drawing.Size(328, 464);
		this.picVisor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.picVisor.TabIndex = 3;
		this.picVisor.TabStop = false;
		this.picVisor.Visible = false;
		this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.toolStripButton1, this.toolStripDropDownButton1, this.toolStripSplitButton1 });
		this.toolStrip1.Location = new System.Drawing.Point(0, 0);
		this.toolStrip1.Name = "toolStrip1";
		this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
		this.toolStrip1.Size = new System.Drawing.Size(1112, 25);
		this.toolStrip1.TabIndex = 4;
		this.toolStrip1.Text = "toolStrip1";
		this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripButton1.Image");
		this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolStripButton1.Name = "toolStripButton1";
		this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
		this.toolStripButton1.Text = "toolStripButton1";
		this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
		this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
		this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
		this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
		this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripSplitButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripSplitButton1.Image");
		this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolStripSplitButton1.Name = "toolStripSplitButton1";
		this.toolStripSplitButton1.Size = new System.Drawing.Size(32, 22);
		this.toolStripSplitButton1.Text = "toolStripSplitButton1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1112, 699);
		base.Controls.Add(this.toolStrip1);
		base.Controls.Add(this.picVisor);
		base.Controls.Add(this.listBoxArchivos);
		base.Controls.Add(this.panelVisor);
		base.Controls.Add(this.groupBox1);
		base.Name = "frmControlCalidad";
		this.Text = "frmControlCalidad";
		base.Load += new System.EventHandler(frmControlCalidad_Load);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(frmControlCalidad_KeyDown);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.picVisor).EndInit();
		this.toolStrip1.ResumeLayout(false);
		this.toolStrip1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
