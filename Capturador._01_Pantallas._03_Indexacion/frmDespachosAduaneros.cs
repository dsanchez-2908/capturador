using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmDespachosAduaneros : Form
{
	private IContainer components = null;

	private GroupBox groupBox1;

	private TextBox txtRutaLote;

	private Button btnSeleccionarCarpeta;

	private GroupBox groupBox2;

	private ListBox lboxIndice;

	private GroupBox groupBox3;

	private ProgressBar progressBar1;

	private Label label4;

	private Label label3;

	private Label label2;

	private Label label1;

	private GroupBox groupBox4;

	private Label label9;

	private Label label8;

	private Label label7;

	private Label label6;

	private Label label5;

	private GroupBox groupBox5;

	private WebBrowser webBrowser1;

	private Button button2;

	private TextBox textBox4;

	private TextBox textBox3;

	private TextBox textBox2;

	private TextBox textBox1;

	private ComboBox comboBox2;

	private TextBox textBox7;

	private TextBox textBox6;

	private ComboBox comboBox1;

	private TextBox textBox5;

	public frmDespachosAduaneros()
	{
		InitializeComponent();
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
		this.txtRutaLote = new System.Windows.Forms.TextBox();
		this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.lboxIndice = new System.Windows.Forms.ListBox();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.label4 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.groupBox4 = new System.Windows.Forms.GroupBox();
		this.label5 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.groupBox5 = new System.Windows.Forms.GroupBox();
		this.webBrowser1 = new System.Windows.Forms.WebBrowser();
		this.button2 = new System.Windows.Forms.Button();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.textBox2 = new System.Windows.Forms.TextBox();
		this.textBox3 = new System.Windows.Forms.TextBox();
		this.textBox4 = new System.Windows.Forms.TextBox();
		this.textBox5 = new System.Windows.Forms.TextBox();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.textBox6 = new System.Windows.Forms.TextBox();
		this.textBox7 = new System.Windows.Forms.TextBox();
		this.comboBox2 = new System.Windows.Forms.ComboBox();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.groupBox4.SuspendLayout();
		this.groupBox5.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.txtRutaLote);
		this.groupBox1.Controls.Add(this.btnSeleccionarCarpeta);
		this.groupBox1.Location = new System.Drawing.Point(13, 13);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(301, 100);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Selección de Lote a Indexar";
		this.txtRutaLote.Location = new System.Drawing.Point(7, 56);
		this.txtRutaLote.Multiline = true;
		this.txtRutaLote.Name = "txtRutaLote";
		this.txtRutaLote.Size = new System.Drawing.Size(288, 38);
		this.txtRutaLote.TabIndex = 1;
		this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(7, 20);
		this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
		this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(288, 29);
		this.btnSeleccionarCarpeta.TabIndex = 0;
		this.btnSeleccionarCarpeta.Text = "Seleccionar Carpeta";
		this.btnSeleccionarCarpeta.UseVisualStyleBackColor = true;
		this.groupBox2.Controls.Add(this.lboxIndice);
		this.groupBox2.Location = new System.Drawing.Point(13, 120);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(301, 470);
		this.groupBox2.TabIndex = 1;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Indice";
		this.lboxIndice.FormattingEnabled = true;
		this.lboxIndice.HorizontalScrollbar = true;
		this.lboxIndice.Location = new System.Drawing.Point(7, 20);
		this.lboxIndice.Name = "lboxIndice";
		this.lboxIndice.Size = new System.Drawing.Size(288, 446);
		this.lboxIndice.TabIndex = 0;
		this.groupBox3.Controls.Add(this.textBox4);
		this.groupBox3.Controls.Add(this.textBox3);
		this.groupBox3.Controls.Add(this.textBox2);
		this.groupBox3.Controls.Add(this.textBox1);
		this.groupBox3.Controls.Add(this.progressBar1);
		this.groupBox3.Controls.Add(this.label4);
		this.groupBox3.Controls.Add(this.label3);
		this.groupBox3.Controls.Add(this.label2);
		this.groupBox3.Controls.Add(this.label1);
		this.groupBox3.Location = new System.Drawing.Point(321, 13);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(284, 162);
		this.groupBox3.TabIndex = 2;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "Lote Seleccionado";
		this.progressBar1.Location = new System.Drawing.Point(10, 127);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(268, 23);
		this.progressBar1.TabIndex = 4;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(7, 98);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(110, 13);
		this.label4.TabIndex = 3;
		this.label4.Text = "Registros Pendientes:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(7, 72);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(106, 13);
		this.label3.TabIndex = 2;
		this.label3.Text = "Registros Indexados:";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(7, 46);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(99, 13);
		this.label2.TabIndex = 1;
		this.label2.Text = "Cantidad Registros:";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(7, 20);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(99, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Lote Seleccionado:";
		this.groupBox4.Controls.Add(this.comboBox2);
		this.groupBox4.Controls.Add(this.textBox7);
		this.groupBox4.Controls.Add(this.textBox6);
		this.groupBox4.Controls.Add(this.comboBox1);
		this.groupBox4.Controls.Add(this.textBox5);
		this.groupBox4.Controls.Add(this.label9);
		this.groupBox4.Controls.Add(this.label8);
		this.groupBox4.Controls.Add(this.label7);
		this.groupBox4.Controls.Add(this.label6);
		this.groupBox4.Controls.Add(this.label5);
		this.groupBox4.Location = new System.Drawing.Point(321, 193);
		this.groupBox4.Name = "groupBox4";
		this.groupBox4.Size = new System.Drawing.Size(284, 184);
		this.groupBox4.TabIndex = 3;
		this.groupBox4.TabStop = false;
		this.groupBox4.Text = "Campos de Carga";
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(10, 23);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(59, 13);
		this.label5.TabIndex = 0;
		this.label5.Text = "Daspacho:";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(10, 49);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(94, 13);
		this.label6.TabIndex = 1;
		this.label6.Text = "Serie Documental:";
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(10, 76);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(65, 13);
		this.label7.TabIndex = 2;
		this.label7.Text = "Nro. SIDEA:";
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(10, 102);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(55, 13);
		this.label8.TabIndex = 3;
		this.label8.Text = "Nro. Guia:";
		this.label9.AutoSize = true;
		this.label9.Location = new System.Drawing.Point(10, 128);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(111, 13);
		this.label9.TabIndex = 4;
		this.label9.Text = "Usuario Digitalización:";
		this.groupBox5.Controls.Add(this.webBrowser1);
		this.groupBox5.Location = new System.Drawing.Point(612, 13);
		this.groupBox5.Name = "groupBox5";
		this.groupBox5.Size = new System.Drawing.Size(550, 573);
		this.groupBox5.TabIndex = 4;
		this.groupBox5.TabStop = false;
		this.groupBox5.Text = "Imagen";
		this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.webBrowser1.Location = new System.Drawing.Point(3, 16);
		this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
		this.webBrowser1.Name = "webBrowser1";
		this.webBrowser1.Size = new System.Drawing.Size(544, 554);
		this.webBrowser1.TabIndex = 0;
		this.button2.Location = new System.Drawing.Point(322, 543);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(284, 47);
		this.button2.TabIndex = 5;
		this.button2.Text = "Grabar";
		this.button2.UseVisualStyleBackColor = true;
		this.textBox1.Location = new System.Drawing.Point(177, 17);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(100, 20);
		this.textBox1.TabIndex = 5;
		this.textBox2.Location = new System.Drawing.Point(177, 43);
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size(100, 20);
		this.textBox2.TabIndex = 6;
		this.textBox3.Location = new System.Drawing.Point(178, 69);
		this.textBox3.Name = "textBox3";
		this.textBox3.Size = new System.Drawing.Size(100, 20);
		this.textBox3.TabIndex = 7;
		this.textBox4.Location = new System.Drawing.Point(178, 95);
		this.textBox4.Name = "textBox4";
		this.textBox4.Size = new System.Drawing.Size(100, 20);
		this.textBox4.TabIndex = 8;
		this.textBox5.Location = new System.Drawing.Point(127, 20);
		this.textBox5.Name = "textBox5";
		this.textBox5.Size = new System.Drawing.Size(150, 20);
		this.textBox5.TabIndex = 5;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(127, 46);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(150, 21);
		this.comboBox1.TabIndex = 6;
		this.textBox6.Location = new System.Drawing.Point(127, 73);
		this.textBox6.Name = "textBox6";
		this.textBox6.Size = new System.Drawing.Size(150, 20);
		this.textBox6.TabIndex = 7;
		this.textBox7.Location = new System.Drawing.Point(127, 99);
		this.textBox7.Name = "textBox7";
		this.textBox7.Size = new System.Drawing.Size(150, 20);
		this.textBox7.TabIndex = 8;
		this.comboBox2.FormattingEnabled = true;
		this.comboBox2.Location = new System.Drawing.Point(127, 125);
		this.comboBox2.Name = "comboBox2";
		this.comboBox2.Size = new System.Drawing.Size(150, 21);
		this.comboBox2.TabIndex = 9;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1174, 602);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.groupBox5);
		base.Controls.Add(this.groupBox4);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.Name = "frmDespachosAduaneros";
		this.Text = "Indexación de Despachos Aduaneros";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox3.ResumeLayout(false);
		this.groupBox3.PerformLayout();
		this.groupBox4.ResumeLayout(false);
		this.groupBox4.PerformLayout();
		this.groupBox5.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
