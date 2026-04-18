using System;
using System.Drawing;
using System.Windows.Forms;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmValidarHistoriaClinica : Form
{
    private TextBox txtValidacion;
    private Label lblMensaje;
    private Button btnValidar;
    private Button btnCancelar;

    public string ValorOriginal { get; set; }
    public bool ValidacionExitosa { get; private set; }

    public frmValidarHistoriaClinica()
    {
        InitializeComponent();
        ValidacionExitosa = false;
    }

    private void InitializeComponent()
    {
        this.lblMensaje = new Label();
        this.txtValidacion = new TextBox();
        this.btnValidar = new Button();
        this.btnCancelar = new Button();
        this.SuspendLayout();

        // lblMensaje
        this.lblMensaje.AutoSize = false;
        this.lblMensaje.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
        this.lblMensaje.ForeColor = Color.White;
        this.lblMensaje.Location = new Point(20, 20);
        this.lblMensaje.Name = "lblMensaje";
        this.lblMensaje.Size = new Size(360, 60);
        this.lblMensaje.TabIndex = 0;
        this.lblMensaje.Text = "Por favor, vuelva a ingresar el número de Historia Clínica para validar:";
        this.lblMensaje.TextAlign = ContentAlignment.MiddleCenter;

        // txtValidacion
        this.txtValidacion.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
        this.txtValidacion.Location = new Point(50, 90);
        this.txtValidacion.Name = "txtValidacion";
        this.txtValidacion.Size = new Size(300, 29);
        this.txtValidacion.TabIndex = 1;
        this.txtValidacion.TextAlign = HorizontalAlignment.Center;
        this.txtValidacion.KeyDown += TxtValidacion_KeyDown;

        // btnValidar
        this.btnValidar.BackColor = Color.SeaGreen;
        this.btnValidar.FlatAppearance.BorderSize = 0;
        this.btnValidar.FlatStyle = FlatStyle.Flat;
        this.btnValidar.Font = new Font("Century Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        this.btnValidar.ForeColor = Color.White;
        this.btnValidar.Location = new Point(50, 140);
        this.btnValidar.Name = "btnValidar";
        this.btnValidar.Size = new Size(140, 35);
        this.btnValidar.TabIndex = 2;
        this.btnValidar.Text = "Validar";
        this.btnValidar.UseVisualStyleBackColor = false;
        this.btnValidar.Click += BtnValidar_Click;

        // btnCancelar
        this.btnCancelar.BackColor = Color.Salmon;
        this.btnCancelar.FlatAppearance.BorderSize = 0;
        this.btnCancelar.FlatStyle = FlatStyle.Flat;
        this.btnCancelar.Font = new Font("Century Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        this.btnCancelar.ForeColor = Color.White;
        this.btnCancelar.Location = new Point(210, 140);
        this.btnCancelar.Name = "btnCancelar";
        this.btnCancelar.Size = new Size(140, 35);
        this.btnCancelar.TabIndex = 3;
        this.btnCancelar.Text = "Cancelar";
        this.btnCancelar.UseVisualStyleBackColor = false;
        this.btnCancelar.Click += BtnCancelar_Click;

        // frmValidarHistoriaClinica
        this.AutoScaleDimensions = new SizeF(6F, 13F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.FromArgb(49, 66, 82);
        this.ClientSize = new Size(400, 200);
        this.Controls.Add(this.btnCancelar);
        this.Controls.Add(this.btnValidar);
        this.Controls.Add(this.txtValidacion);
        this.Controls.Add(this.lblMensaje);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "frmValidarHistoriaClinica";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Validar Historia Clínica";
        this.Load += FrmValidarHistoriaClinica_Load;
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void FrmValidarHistoriaClinica_Load(object sender, EventArgs e)
    {
        txtValidacion.Focus();
    }

    private void TxtValidacion_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            e.SuppressKeyPress = true;
            ValidarHistoriaClinica();
        }
        else if (e.KeyCode == Keys.Escape)
        {
            e.SuppressKeyPress = true;
            CancelarValidacion();
        }
    }

    private void BtnValidar_Click(object sender, EventArgs e)
    {
        ValidarHistoriaClinica();
    }

    private void BtnCancelar_Click(object sender, EventArgs e)
    {
        CancelarValidacion();
    }

    private void ValidarHistoriaClinica()
    {
        if (string.IsNullOrEmpty(txtValidacion.Text))
        {
            MessageBox.Show("Debe ingresar el número de Historia Clínica", "Advertencia", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtValidacion.Focus();
            return;
        }

        if (txtValidacion.Text.Trim() == ValorOriginal.Trim())
        {
            ValidacionExitosa = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            MessageBox.Show("Los valores no coinciden. Por favor, intente nuevamente.", "Error de Validación", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            ValidacionExitosa = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    private void CancelarValidacion()
    {
        ValidacionExitosa = false;
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
