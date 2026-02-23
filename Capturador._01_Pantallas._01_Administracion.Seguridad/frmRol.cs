using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Capturador._01_Pantallas._01_Administracion.Seguridad;

public class frmRol : Form
{
	private IContainer components = null;

	public frmRol()
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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(674, 529);
		base.MinimizeBox = false;
		base.Name = "frmRol";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "frmRol";
		base.ResumeLayout(false);
	}
}
