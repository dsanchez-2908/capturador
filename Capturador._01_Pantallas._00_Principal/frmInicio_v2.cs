using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._00_Principal;

public class frmInicio_v2 : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private IContainer components = null;

	private PictureBox pictureBox1;

	private TextBox txtUsuario;

	private Label lblUsuario;

	private TextBox txtClave;

	private Label lblClave;

	private TextBox txtNuevaClave;

	private Label lblNuevaClave;

	private TextBox txtConfirmarNuevaClave;

	private Label lblConfirmarNuevaClave;

	private Button btnCancelar;

	private Button btnIngresar;

	private Button btnActualizarClave;

	public frmInicio_v2()
	{
		InitializeComponent();
	}

	private void frmInicio_v2_Load(object sender, EventArgs e)
	{
		IniciarEventoEnter();
	}

	private void IniciarEventoEnter()
	{
		txtUsuario.KeyDown += txt_KeyDown;
		txtClave.KeyDown += txt_KeyDown;
		txtNuevaClave.KeyDown += txt_KeyDown;
		txtConfirmarNuevaClave.KeyDown += txt_KeyDown;
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
			if (oUsuarioLogueado.cdEstado == 1)
			{
				lblClave.Visible = false;
				txtClave.Visible = false;
				btnIngresar.Visible = false;
				txtUsuario.Enabled = false;
				lblNuevaClave.Visible = true;
				txtNuevaClave.Visible = true;
				lblConfirmarNuevaClave.Visible = true;
				txtConfirmarNuevaClave.Visible = true;
				btnActualizarClave.Visible = true;
				txtNuevaClave.Focus();
			}
			if (oUsuarioLogueado.cdEstado == 2)
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

	private void AbrirFormularioPrincipal(eUsuario pUsuarioLogueado)
	{
		Hide();
		frmPrincipal oFormPrincipal = new frmPrincipal(pUsuarioLogueado);
		oFormPrincipal.Show();
	}

	private void btnActualizarClave_Click(object sender, EventArgs e)
	{
		if (txtNuevaClave.Text != txtConfirmarNuevaClave.Text)
		{
			MessageBox.Show("No coincide las claves ingresadas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		nSeguridad.ActualizarClaveNueva(oUsuarioLogueado, oUsuarioLogueado, txtNuevaClave.Text);
		MessageBox.Show("La clave se actualizó correctamente", "Login", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		lblClave.Visible = true;
		txtClave.Visible = true;
		btnIngresar.Visible = true;
		txtUsuario.Enabled = true;
		lblNuevaClave.Visible = false;
		txtNuevaClave.Visible = false;
		lblConfirmarNuevaClave.Visible = false;
		txtConfirmarNuevaClave.Visible = false;
		btnActualizarClave.Visible = false;
		txtUsuario.Clear();
		txtClave.Clear();
		txtNuevaClave.Clear();
		txtConfirmarNuevaClave.Clear();
		txtUsuario.Focus();
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblClave = new System.Windows.Forms.Label();
            this.txtNuevaClave = new System.Windows.Forms.TextBox();
            this.lblNuevaClave = new System.Windows.Forms.Label();
            this.txtConfirmarNuevaClave = new System.Windows.Forms.TextBox();
            this.lblConfirmarNuevaClave = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.btnActualizarClave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(28, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(222, 139);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // txtUsuario
            // 
            this.txtUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuario.Location = new System.Drawing.Point(457, 63);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(175, 26);
            this.txtUsuario.TabIndex = 1;
            this.txtUsuario.Text = "ADMIN";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.ForeColor = System.Drawing.Color.White;
            this.lblUsuario.Location = new System.Drawing.Point(280, 66);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(68, 20);
            this.lblUsuario.TabIndex = 29;
            this.lblUsuario.Text = "Usuario:";
            // 
            // txtClave
            // 
            this.txtClave.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClave.Location = new System.Drawing.Point(457, 120);
            this.txtClave.Name = "txtClave";
            this.txtClave.PasswordChar = '*';
            this.txtClave.Size = new System.Drawing.Size(175, 26);
            this.txtClave.TabIndex = 2;
            this.txtClave.Text = "1234$";
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClave.ForeColor = System.Drawing.Color.White;
            this.lblClave.Location = new System.Drawing.Point(280, 123);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(52, 20);
            this.lblClave.TabIndex = 31;
            this.lblClave.Text = "Clave:";
            // 
            // txtNuevaClave
            // 
            this.txtNuevaClave.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNuevaClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNuevaClave.Location = new System.Drawing.Point(457, 95);
            this.txtNuevaClave.Name = "txtNuevaClave";
            this.txtNuevaClave.PasswordChar = '*';
            this.txtNuevaClave.Size = new System.Drawing.Size(175, 26);
            this.txtNuevaClave.TabIndex = 4;
            this.txtNuevaClave.Visible = false;
            // 
            // lblNuevaClave
            // 
            this.lblNuevaClave.AutoSize = true;
            this.lblNuevaClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNuevaClave.ForeColor = System.Drawing.Color.White;
            this.lblNuevaClave.Location = new System.Drawing.Point(280, 98);
            this.lblNuevaClave.Name = "lblNuevaClave";
            this.lblNuevaClave.Size = new System.Drawing.Size(101, 20);
            this.lblNuevaClave.TabIndex = 33;
            this.lblNuevaClave.Text = "Nueva Clave:";
            this.lblNuevaClave.Visible = false;
            // 
            // txtConfirmarNuevaClave
            // 
            this.txtConfirmarNuevaClave.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtConfirmarNuevaClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfirmarNuevaClave.Location = new System.Drawing.Point(457, 144);
            this.txtConfirmarNuevaClave.Name = "txtConfirmarNuevaClave";
            this.txtConfirmarNuevaClave.PasswordChar = '*';
            this.txtConfirmarNuevaClave.Size = new System.Drawing.Size(175, 26);
            this.txtConfirmarNuevaClave.TabIndex = 5;
            this.txtConfirmarNuevaClave.Visible = false;
            // 
            // lblConfirmarNuevaClave
            // 
            this.lblConfirmarNuevaClave.AutoSize = true;
            this.lblConfirmarNuevaClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirmarNuevaClave.ForeColor = System.Drawing.Color.White;
            this.lblConfirmarNuevaClave.Location = new System.Drawing.Point(280, 147);
            this.lblConfirmarNuevaClave.Name = "lblConfirmarNuevaClave";
            this.lblConfirmarNuevaClave.Size = new System.Drawing.Size(174, 20);
            this.lblConfirmarNuevaClave.TabIndex = 35;
            this.lblConfirmarNuevaClave.Text = "Confirmar Nueva Clave:";
            this.lblConfirmarNuevaClave.Visible = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.Salmon;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(457, 185);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(175, 31);
            this.btnCancelar.TabIndex = 98;
            this.btnCancelar.Text = "   Cancelar";
            this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnIngresar
            // 
            this.btnIngresar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIngresar.BackColor = System.Drawing.Color.SeaGreen;
            this.btnIngresar.FlatAppearance.BorderSize = 0;
            this.btnIngresar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIngresar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIngresar.ForeColor = System.Drawing.Color.White;
            this.btnIngresar.Location = new System.Drawing.Point(275, 185);
            this.btnIngresar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Size = new System.Drawing.Size(175, 31);
            this.btnIngresar.TabIndex = 97;
            this.btnIngresar.Text = "   Ingresar";
            this.btnIngresar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnIngresar.UseVisualStyleBackColor = false;
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);
            // 
            // btnActualizarClave
            // 
            this.btnActualizarClave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActualizarClave.BackColor = System.Drawing.Color.SeaGreen;
            this.btnActualizarClave.FlatAppearance.BorderSize = 0;
            this.btnActualizarClave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizarClave.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActualizarClave.ForeColor = System.Drawing.Color.White;
            this.btnActualizarClave.Location = new System.Drawing.Point(276, 185);
            this.btnActualizarClave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnActualizarClave.Name = "btnActualizarClave";
            this.btnActualizarClave.Size = new System.Drawing.Size(175, 31);
            this.btnActualizarClave.TabIndex = 6;
            this.btnActualizarClave.Text = "   Actualizar Clave";
            this.btnActualizarClave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnActualizarClave.UseVisualStyleBackColor = false;
            this.btnActualizarClave.Visible = false;
            this.btnActualizarClave.Click += new System.EventHandler(this.btnActualizarClave_Click);
            // 
            // frmInicio_v2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(684, 261);
            this.Controls.Add(this.btnActualizarClave);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnIngresar);
            this.Controls.Add(this.txtConfirmarNuevaClave);
            this.Controls.Add(this.lblConfirmarNuevaClave);
            this.Controls.Add(this.txtNuevaClave);
            this.Controls.Add(this.lblNuevaClave);
            this.Controls.Add(this.txtClave);
            this.Controls.Add(this.lblClave);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.pictureBox1);
            this.MaximizeBox = false;
            this.Name = "frmInicio_v2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Capturador de Datos - MODOC";
            this.Load += new System.EventHandler(this.frmInicio_v2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
