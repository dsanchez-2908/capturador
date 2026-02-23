using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._00_Principal;

public class frmInicio : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private IContainer components = null;

	private PictureBox pictureBox1;

	private Label label1;

	private Label label2;

	private TextBox txtUsuario;

	private TextBox txtClave;

	private Button btnIngresar;

	private Button btnCancelar;

	public frmInicio()
	{
		InitializeComponent();
	}

	private void frmInicio_Load(object sender, EventArgs e)
	{
		IniciarEventoEnter();
	}

	private void IniciarEventoEnter()
	{
		txtUsuario.KeyDown += txt_KeyDown;
		txtClave.KeyDown += txt_KeyDown;
	}

	private void txt_KeyDown(object sender, KeyEventArgs e)
	{
		TextBox txt = sender as TextBox;
		if (e.KeyCode == Keys.Return)
		{
			e.SuppressKeyPress = true;
		}
		if (e.KeyCode == Keys.Return)
		{
			SelectNextControl(txt, forward: true, tabStopOnly: true, nested: true, wrap: true);
		}
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void btnIngresar_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtClave.Text))
		{
			MessageBox.Show("Debe Ingresar un usuario y una clave", "Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		try
		{
			oUsuarioLogueado = nSeguridad.ValidarLoginUsuario(oUsuarioLogueado, txtUsuario.Text, txtClave.Text);
			if (oUsuarioLogueado.cdEstado != 1)
			{
				AbrirFormularioPrincipal(oUsuarioLogueado);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Usuario", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			txtUsuario.Clear();
			txtClave.Clear();
			txtUsuario.Focus();
		}
	}

	private void AbrirFormularioPrincipal(eUsuario pUsuario)
	{
		Hide();
		frmPrincipal oFormPrincipal = new frmPrincipal(pUsuario);
		oFormPrincipal.Show();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._00_Principal.frmInicio));
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.txtUsuario = new System.Windows.Forms.TextBox();
		this.txtClave = new System.Windows.Forms.TextBox();
		this.btnIngresar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
		this.pictureBox1.Location = new System.Drawing.Point(43, 31);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(222, 139);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 0;
		this.pictureBox1.TabStop = false;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(299, 42);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(68, 20);
		this.label1.TabIndex = 1;
		this.label1.Text = "Usuario:";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Location = new System.Drawing.Point(299, 76);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(52, 20);
		this.label2.TabIndex = 2;
		this.label2.Text = "Clave:";
		this.txtUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtUsuario.Location = new System.Drawing.Point(411, 31);
		this.txtUsuario.Name = "txtUsuario";
		this.txtUsuario.Size = new System.Drawing.Size(178, 26);
		this.txtUsuario.TabIndex = 3;
		this.txtClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtClave.Location = new System.Drawing.Point(411, 73);
		this.txtClave.Name = "txtClave";
		this.txtClave.Size = new System.Drawing.Size(178, 26);
		this.txtClave.TabIndex = 4;
		this.btnIngresar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnIngresar.Location = new System.Drawing.Point(303, 120);
		this.btnIngresar.Name = "btnIngresar";
		this.btnIngresar.Size = new System.Drawing.Size(125, 50);
		this.btnIngresar.TabIndex = 5;
		this.btnIngresar.Text = "Ingresar";
		this.btnIngresar.UseVisualStyleBackColor = true;
		this.btnIngresar.Click += new System.EventHandler(btnIngresar_Click);
		this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.Location = new System.Drawing.Point(464, 120);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(125, 50);
		this.btnCancelar.TabIndex = 6;
		this.btnCancelar.Text = "Cancelar";
		this.btnCancelar.UseVisualStyleBackColor = true;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(634, 211);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnIngresar);
		base.Controls.Add(this.txtClave);
		base.Controls.Add(this.txtUsuario);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.pictureBox1);
		base.MinimizeBox = false;
		base.Name = "frmInicio";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Capturador de Datos - MODOC";
		base.Load += new System.EventHandler(frmInicio_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
