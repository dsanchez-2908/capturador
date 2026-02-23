using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Capturador._04_Entidades;
using NTwain;

namespace Capturador._01_Pantallas._07_Digitalizacion;

public class frmDiditalizador_v2 : Form
{
	private eUsuario oUsuarioLogueado;

	private TwainSession _twain;

	private DataSource _fuenteActiva;

	private List<string> _imagenesEscaneadas = new List<string>();

	private string _carpetaTemporal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DigitalizadorTemp");

	private IContainer components = null;

	private ComboBox cbxEscaner;

	private Label label1;

	private Label label2;

	private ComboBox cbxResolucion;

	private Button btnEscanear;

	private TableLayoutPanel tableLayoutImagenes;

	private Panel panelVisor;

	private PictureBox pbImagenSeleccionada;

	private TreeView treeDocumentos;

	private Button btnRotarIzq;

	private Button btnRotarDer;

	private Button btnEliminar;

	private Button btnGuardarPDF;

	private ProgressBar progressBar;

	private Label lblEstado;

	private Button btnIndexar;

	private TextBox txtNombreLote;

	public frmDiditalizador_v2(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmDiditalizador_v2_Load(object sender, EventArgs e)
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
		this.cbxEscaner = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.cbxResolucion = new System.Windows.Forms.ComboBox();
		this.btnEscanear = new System.Windows.Forms.Button();
		this.tableLayoutImagenes = new System.Windows.Forms.TableLayoutPanel();
		this.panelVisor = new System.Windows.Forms.Panel();
		this.pbImagenSeleccionada = new System.Windows.Forms.PictureBox();
		this.treeDocumentos = new System.Windows.Forms.TreeView();
		this.btnRotarIzq = new System.Windows.Forms.Button();
		this.btnRotarDer = new System.Windows.Forms.Button();
		this.btnEliminar = new System.Windows.Forms.Button();
		this.btnGuardarPDF = new System.Windows.Forms.Button();
		this.progressBar = new System.Windows.Forms.ProgressBar();
		this.lblEstado = new System.Windows.Forms.Label();
		this.btnIndexar = new System.Windows.Forms.Button();
		this.txtNombreLote = new System.Windows.Forms.TextBox();
		((System.ComponentModel.ISupportInitialize)this.pbImagenSeleccionada).BeginInit();
		base.SuspendLayout();
		this.cbxEscaner.FormattingEnabled = true;
		this.cbxEscaner.Location = new System.Drawing.Point(179, 10);
		this.cbxEscaner.Name = "cbxEscaner";
		this.cbxEscaner.Size = new System.Drawing.Size(207, 21);
		this.cbxEscaner.TabIndex = 0;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 13);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(151, 13);
		this.label1.TabIndex = 1;
		this.label1.Text = "Lista de escáneres disponibles";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(12, 51);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(60, 13);
		this.label2.TabIndex = 2;
		this.label2.Text = "Resolución";
		this.cbxResolucion.FormattingEnabled = true;
		this.cbxResolucion.Location = new System.Drawing.Point(179, 42);
		this.cbxResolucion.Name = "cbxResolucion";
		this.cbxResolucion.Size = new System.Drawing.Size(207, 21);
		this.cbxResolucion.TabIndex = 3;
		this.btnEscanear.Location = new System.Drawing.Point(16, 94);
		this.btnEscanear.Name = "btnEscanear";
		this.btnEscanear.Size = new System.Drawing.Size(148, 23);
		this.btnEscanear.TabIndex = 4;
		this.btnEscanear.Text = "Inicia el escaneo";
		this.btnEscanear.UseVisualStyleBackColor = true;
		this.tableLayoutImagenes.ColumnCount = 2;
		this.tableLayoutImagenes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutImagenes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutImagenes.Location = new System.Drawing.Point(279, 134);
		this.tableLayoutImagenes.Name = "tableLayoutImagenes";
		this.tableLayoutImagenes.RowCount = 2;
		this.tableLayoutImagenes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutImagenes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutImagenes.Size = new System.Drawing.Size(353, 246);
		this.tableLayoutImagenes.TabIndex = 5;
		this.panelVisor.Location = new System.Drawing.Point(639, 134);
		this.panelVisor.Name = "panelVisor";
		this.panelVisor.Size = new System.Drawing.Size(443, 275);
		this.panelVisor.TabIndex = 6;
		this.pbImagenSeleccionada.Location = new System.Drawing.Point(639, 429);
		this.pbImagenSeleccionada.Name = "pbImagenSeleccionada";
		this.pbImagenSeleccionada.Size = new System.Drawing.Size(443, 128);
		this.pbImagenSeleccionada.TabIndex = 7;
		this.pbImagenSeleccionada.TabStop = false;
		this.treeDocumentos.Location = new System.Drawing.Point(16, 134);
		this.treeDocumentos.Name = "treeDocumentos";
		this.treeDocumentos.Size = new System.Drawing.Size(243, 459);
		this.treeDocumentos.TabIndex = 8;
		this.btnRotarIzq.Location = new System.Drawing.Point(191, 94);
		this.btnRotarIzq.Name = "btnRotarIzq";
		this.btnRotarIzq.Size = new System.Drawing.Size(135, 23);
		this.btnRotarIzq.TabIndex = 9;
		this.btnRotarIzq.Text = "Rotar 90° izquierda";
		this.btnRotarIzq.UseVisualStyleBackColor = true;
		this.btnRotarDer.Location = new System.Drawing.Point(332, 94);
		this.btnRotarDer.Name = "btnRotarDer";
		this.btnRotarDer.Size = new System.Drawing.Size(135, 23);
		this.btnRotarDer.TabIndex = 10;
		this.btnRotarDer.Text = "Rotar 90° derecha";
		this.btnRotarDer.UseVisualStyleBackColor = true;
		this.btnEliminar.Location = new System.Drawing.Point(473, 94);
		this.btnEliminar.Name = "btnEliminar";
		this.btnEliminar.Size = new System.Drawing.Size(159, 23);
		this.btnEliminar.TabIndex = 11;
		this.btnEliminar.Text = "Eliminar imagen seleccionada";
		this.btnEliminar.UseVisualStyleBackColor = true;
		this.btnGuardarPDF.Location = new System.Drawing.Point(639, 94);
		this.btnGuardarPDF.Name = "btnGuardarPDF";
		this.btnGuardarPDF.Size = new System.Drawing.Size(179, 23);
		this.btnGuardarPDF.TabIndex = 12;
		this.btnGuardarPDF.Text = "Guardar imágenes como PDF\n\n";
		this.btnGuardarPDF.UseVisualStyleBackColor = true;
		this.progressBar.Location = new System.Drawing.Point(541, 13);
		this.progressBar.Name = "progressBar";
		this.progressBar.Size = new System.Drawing.Size(479, 23);
		this.progressBar.TabIndex = 13;
		this.lblEstado.AutoSize = true;
		this.lblEstado.Location = new System.Drawing.Point(541, 51);
		this.lblEstado.Name = "lblEstado";
		this.lblEstado.Size = new System.Drawing.Size(50, 13);
		this.lblEstado.TabIndex = 14;
		this.lblEstado.Text = "lblEstado";
		this.btnIndexar.Location = new System.Drawing.Point(852, 51);
		this.btnIndexar.Name = "btnIndexar";
		this.btnIndexar.Size = new System.Drawing.Size(207, 23);
		this.btnIndexar.TabIndex = 15;
		this.btnIndexar.Text = "Abre formulario de indexación";
		this.btnIndexar.UseVisualStyleBackColor = true;
		this.txtNombreLote.Location = new System.Drawing.Point(852, 94);
		this.txtNombreLote.Name = "txtNombreLote";
		this.txtNombreLote.Size = new System.Drawing.Size(100, 20);
		this.txtNombreLote.TabIndex = 16;
		this.txtNombreLote.Text = "txtNombreLote";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1094, 605);
		base.Controls.Add(this.txtNombreLote);
		base.Controls.Add(this.btnIndexar);
		base.Controls.Add(this.lblEstado);
		base.Controls.Add(this.progressBar);
		base.Controls.Add(this.btnGuardarPDF);
		base.Controls.Add(this.btnEliminar);
		base.Controls.Add(this.btnRotarDer);
		base.Controls.Add(this.btnRotarIzq);
		base.Controls.Add(this.treeDocumentos);
		base.Controls.Add(this.pbImagenSeleccionada);
		base.Controls.Add(this.panelVisor);
		base.Controls.Add(this.tableLayoutImagenes);
		base.Controls.Add(this.btnEscanear);
		base.Controls.Add(this.cbxResolucion);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.cbxEscaner);
		base.Name = "frmDiditalizador_v2";
		this.Text = "frmDiditalizador_v2";
		base.Load += new System.EventHandler(frmDiditalizador_v2_Load);
		((System.ComponentModel.ISupportInitialize)this.pbImagenSeleccionada).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
