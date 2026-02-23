using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmVerArchivoPDF : Form
{
	private string titulo;

	private string ruta;

	private string nombreArchivo;

	private IContainer components = null;

	private WebBrowser webBrowser1;

	public frmVerArchivoPDF(string pTitulo, string pRuta, string pNombreArchivo)
	{
		InitializeComponent();
		titulo = pTitulo;
		ruta = pRuta;
		nombreArchivo = pNombreArchivo;
	}

	private void frmVerArchivoPDF_Load(object sender, EventArgs e)
	{
		Text = titulo;
		mostrarArchivo();
	}

	private void mostrarArchivo()
	{
		string archivoMostrar = Path.Combine(ruta, nombreArchivo);
		webBrowser1.Navigate(archivoMostrar);
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
		this.webBrowser1 = new System.Windows.Forms.WebBrowser();
		base.SuspendLayout();
		this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.webBrowser1.Location = new System.Drawing.Point(0, 0);
		this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
		this.webBrowser1.Name = "webBrowser1";
		this.webBrowser1.Size = new System.Drawing.Size(784, 561);
		this.webBrowser1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(784, 561);
		base.Controls.Add(this.webBrowser1);
		base.Name = "frmVerArchivoPDF";
		this.Text = "Visor de Archivo PDF";
		base.Load += new System.EventHandler(frmVerArchivoPDF_Load);
		base.ResumeLayout(false);
	}
}
