using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._07_Digitalizacion;

public class frmCrearLote : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string carpetaRepositorio;

	private IContainer components = null;

	private TextBox txtNombreLote;

	private Label lblNombreLote;

	private Button btnCancelar;

	private Button btnCrearLote;

	public frmCrearLote(eUsuario pUsuarioLogueado, string pCarpetaRepositorio)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
		carpetaRepositorio = pCarpetaRepositorio;
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCrearLote_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtNombreLote.Text))
		{
			MessageBox.Show("Debe indicar el nombre del lote", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		try
		{
			string lote = "TMP_" + txtNombreLote.Text;
			nDigitalizacion.crearLote(carpetaRepositorio, lote);
			frmDigitalizador_v1 ofrmDigitalizador_v1 = base.Owner as frmDigitalizador_v1;
			ofrmDigitalizador_v1.txtNombreLote.Text = lote;
			cerrarFormulario();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void cerrarFormulario()
	{
		Close();
		Dispose();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._07_Digitalizacion.frmCrearLote));
		this.txtNombreLote = new System.Windows.Forms.TextBox();
		this.lblNombreLote = new System.Windows.Forms.Label();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCrearLote = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.txtNombreLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNombreLote.Location = new System.Drawing.Point(265, 61);
		this.txtNombreLote.Name = "txtNombreLote";
		this.txtNombreLote.Size = new System.Drawing.Size(229, 29);
		this.txtNombreLote.TabIndex = 69;
		this.lblNombreLote.AutoSize = true;
		this.lblNombreLote.Font = new System.Drawing.Font("Century Gothic", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNombreLote.ForeColor = System.Drawing.Color.White;
		this.lblNombreLote.Location = new System.Drawing.Point(64, 65);
		this.lblNombreLote.Name = "lblNombreLote";
		this.lblNombreLote.Size = new System.Drawing.Size(167, 22);
		this.lblNombreLote.TabIndex = 68;
		this.lblNombreLote.Text = "Nombre del Lote:";
		this.btnCancelar.BackColor = System.Drawing.Color.Salmon;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(294, 146);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(200, 50);
		this.btnCancelar.TabIndex = 71;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.btnCrearLote.BackColor = System.Drawing.Color.SeaGreen;
		this.btnCrearLote.FlatAppearance.BorderSize = 0;
		this.btnCrearLote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCrearLote.Font = new System.Drawing.Font("Century Gothic", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCrearLote.ForeColor = System.Drawing.Color.White;
		this.btnCrearLote.Image = (System.Drawing.Image)resources.GetObject("btnCrearLote.Image");
		this.btnCrearLote.Location = new System.Drawing.Point(56, 145);
		this.btnCrearLote.Name = "btnCrearLote";
		this.btnCrearLote.Size = new System.Drawing.Size(200, 50);
		this.btnCrearLote.TabIndex = 70;
		this.btnCrearLote.Text = "   Crear Lote";
		this.btnCrearLote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCrearLote.UseVisualStyleBackColor = false;
		this.btnCrearLote.Click += new System.EventHandler(btnCrearLote_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		base.ClientSize = new System.Drawing.Size(520, 241);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnCrearLote);
		base.Controls.Add(this.txtNombreLote);
		base.Controls.Add(this.lblNombreLote);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "frmCrearLote";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "frmCrearLote";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
