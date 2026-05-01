using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Capturador._01_Pantallas._01_Administracion.Seguridad;
using Capturador._01_Pantallas._02_ControlCalidad;
using Capturador._01_Pantallas._03_Indexacion;
using Capturador._01_Pantallas._04_Separador;
using Capturador._01_Pantallas._05_Consultas;
using Capturador._01_Pantallas._06_Lotes;
using Capturador._02_Negocio;
using Capturador._03_Datos;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._00_Principal;

public class frmPrincipal : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string nombreBaseDeDatos;

	private string tituloFormulario = "Capturador de Datos - MODOC (v20260429)";

	private IContainer components = null;

	private StatusStrip statusStrip1;

	private ToolStripStatusLabel toolStripStatusLabel1;

	private ToolStripStatusLabel toolStripStatusLabel2;

	private MenuStrip menuStrip2;

	private ToolStripStatusLabel toolStripStatusLabel3;

	private Panel pnlPrincipal;

	private Label label2;

	private Label label1;

	public frmPrincipal(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmPrincipal_Load(object sender, EventArgs e)
	{
		nombreBaseDeDatos = nSeguridad.obtenerNombreBaseDatos();
		toolStripStatusLabel1.Text = "Usuario: " + oUsuarioLogueado.dsUsuarioNombre;
		toolStripStatusLabel2.Text = "Roles: " + oUsuarioLogueado.dsRoles;
		toolStripStatusLabel3.Text = "Base de Datos: " + nombreBaseDeDatos;
		cargarMenu();
		ajustarDisenoMenu();
	}

	private void cargarMenu()
	{
		List<eMenuItem> menuItems = dSeguridad.ObtenerMenuDesdeBD(oUsuarioLogueado);
		Dictionary<int, ToolStripMenuItem> menuMap = new Dictionary<int, ToolStripMenuItem>();
		foreach (eMenuItem item in from x in menuItems
			orderby x.nuNivel, x.nuOrden
			select x)
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem(item.dsNombre);
			menuItem.Tag = item;
			if (!string.IsNullOrEmpty(item.dsRutaFormulario))
			{
				menuItem.Click += MenuItem_Click;
			}
			if (item.nuNivel == 1)
			{
				menuStrip2.Items.Add(menuItem);
			}
			else if (menuMap.ContainsKey(item.cdMenuSuperior))
			{
				menuMap[item.cdMenuSuperior].DropDownItems.Add(menuItem);
			}
			menuMap[item.cdMenu] = menuItem;
		}
	}

	private void ajustarDisenoMenu()
	{
		if (nombreBaseDeDatos.ToUpper() == "CAPTURADOR")
		{
			Text = tituloFormulario;
			menuStrip2.BackColor = Color.FromArgb(77, 96, 130);
			menuStrip2.ForeColor = Color.White;
			menuStrip2.Font = new Font("Segoe UI", 12f, FontStyle.Regular);
		}
		if (nombreBaseDeDatos.ToUpper() == "CAPTURADOR_TEST")
		{
			Text = tituloFormulario + " - TEST...!!!!";
			menuStrip2.BackColor = Color.Red;
			menuStrip2.ForeColor = Color.Black;
			menuStrip2.Font = new Font("Segoe UI", 12f, FontStyle.Regular);
		}
	}

	private void MenuItem_Click(object sender, EventArgs e)
	{
		ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
		eMenuItem data = menuItem.Tag as eMenuItem;
		try
		{
			RestaurarHijosMDI();
			string rutaCompleta = data.dsRutaFormulario;
			string[] partes = rutaCompleta.Split('(');
			string classPath = partes[0].Trim();
			string parametroStr = partes[1].Replace(")", "").Trim();
			object parametro = GetType().GetField(parametroStr, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
			Type formType = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly asm in assemblies)
			{
				formType = asm.GetType(classPath, throwOnError: false);
				if (formType != null)
				{
					break;
				}
			}
			if (formType != null)
			{
				if (pnlPrincipal.Controls.Count > 0)
				{
					pnlPrincipal.Controls.Clear();
				}
				Form frm = (Form)Activator.CreateInstance(formType, parametro);
				frm.TopLevel = false;
				frm.MdiParent = this;
				pnlPrincipal.Controls.Add(frm);
				pnlPrincipal.Tag = frm;
				frm.WindowState = FormWindowState.Normal;
				frm.Show();
			}
			else
			{
				MessageBox.Show("No se encontró el formulario: " + classPath);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error al abrir formulario: " + ex.Message);
		}
	}

	private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
	{
		Application.Exit();
	}

	private void RestaurarHijosMDI()
	{
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form child in mdiChildren)
		{
			if (child.WindowState == FormWindowState.Maximized)
			{
				child.WindowState = FormWindowState.Normal;
			}
		}
	}

	private void despachosAduanerosToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmDespachosAutomatico ofrmDespachosAutomatico = new frmDespachosAutomatico(oUsuarioLogueado);
		ofrmDespachosAutomatico.WindowState = FormWindowState.Normal;
		ofrmDespachosAutomatico.MdiParent = this;
		ofrmDespachosAutomatico.Show();
	}

	private void despachosAduanerosV2ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmDespachosAutomatico_v2 ofrmDespachosAutomatico_v2 = new frmDespachosAutomatico_v2(oUsuarioLogueado);
		ofrmDespachosAutomatico_v2.WindowState = FormWindowState.Normal;
		ofrmDespachosAutomatico_v2.MdiParent = this;
		ofrmDespachosAutomatico_v2.Show();
	}

	private void despachjosToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmDespachoSeparar oFrmDespachoSeparar = new frmDespachoSeparar(oUsuarioLogueado);
		oFrmDespachoSeparar.WindowState = FormWindowState.Normal;
		oFrmDespachoSeparar.MdiParent = this;
		oFrmDespachoSeparar.Show();
	}

	private void controlDeCalidadToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmControlCalidad_v2 oFrmControlCalidad_v2 = new frmControlCalidad_v2(oUsuarioLogueado);
		oFrmControlCalidad_v2.WindowState = FormWindowState.Normal;
		oFrmControlCalidad_v2.MdiParent = this;
		oFrmControlCalidad_v2.Show();
	}

	private void despachosV2ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmDespachoSeparar_v2 oFrmDespachoSeparar_v2 = new frmDespachoSeparar_v2(oUsuarioLogueado);
		oFrmDespachoSeparar_v2.WindowState = FormWindowState.Normal;
		oFrmDespachoSeparar_v2.MdiParent = this;
		oFrmDespachoSeparar_v2.Show();
	}

	private void despachosV3LoteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmDespachoSeparar_v3 oFrmDespachoSeparar_v3 = new frmDespachoSeparar_v3(oUsuarioLogueado);
		oFrmDespachoSeparar_v3.WindowState = FormWindowState.Normal;
		oFrmDespachoSeparar_v3.MdiParent = this;
		oFrmDespachoSeparar_v3.Show();
	}

	private void ingresoDeLotesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmIngresoLote ofrmIngresoLote = new frmIngresoLote(oUsuarioLogueado);
		ofrmIngresoLote.WindowState = FormWindowState.Normal;
		ofrmIngresoLote.MdiParent = this;
		ofrmIngresoLote.Show();
	}

	private void controlDeCalidadV3EnDesarrolloToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmControlCalidad_v3 oFrmControlCalidad_v3 = new frmControlCalidad_v3(oUsuarioLogueado);
		oFrmControlCalidad_v3.WindowState = FormWindowState.Normal;
		oFrmControlCalidad_v3.MdiParent = this;
		oFrmControlCalidad_v3.Show();
	}

	private void monitorDeLotesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmMonitorLotes ofrmMonitorLotes = new frmMonitorLotes(oUsuarioLogueado);
		ofrmMonitorLotes.WindowState = FormWindowState.Normal;
		ofrmMonitorLotes.MdiParent = this;
		ofrmMonitorLotes.Show();
	}

	private void lotesDeDespachosToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmLoteConsultaDespachos oFrmLoteConsultaDespachos = new frmLoteConsultaDespachos(oUsuarioLogueado);
		oFrmLoteConsultaDespachos.WindowState = FormWindowState.Normal;
		oFrmLoteConsultaDespachos.MdiParent = this;
		oFrmLoteConsultaDespachos.Show();
	}

	private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RestaurarHijosMDI();
		frmUsuarios ofrmUsuarios = new frmUsuarios(oUsuarioLogueado);
		ofrmUsuarios.WindowState = FormWindowState.Normal;
		ofrmUsuarios.MdiParent = this;
		ofrmUsuarios.Show();
	}

	private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
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
		this.statusStrip1 = new System.Windows.Forms.StatusStrip();
		this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
		this.menuStrip2 = new System.Windows.Forms.MenuStrip();
		this.pnlPrincipal = new System.Windows.Forms.Panel();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.statusStrip1.SuspendLayout();
		this.pnlPrincipal.SuspendLayout();
		base.SuspendLayout();
		this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.toolStripStatusLabel1, this.toolStripStatusLabel2, this.toolStripStatusLabel3 });
		this.statusStrip1.Location = new System.Drawing.Point(0, 428);
		this.statusStrip1.Name = "statusStrip1";
		this.statusStrip1.Size = new System.Drawing.Size(800, 22);
		this.statusStrip1.TabIndex = 0;
		this.statusStrip1.Text = "statusStrip1";
		this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
		this.toolStripStatusLabel1.Size = new System.Drawing.Size(50, 17);
		this.toolStripStatusLabel1.Text = "Usuario:";
		this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
		this.toolStripStatusLabel2.Size = new System.Drawing.Size(41, 17);
		this.toolStripStatusLabel2.Text = "Roles: ";
		this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
		this.toolStripStatusLabel3.Size = new System.Drawing.Size(83, 17);
		this.toolStripStatusLabel3.Text = "Base de Datos:";
		this.menuStrip2.Location = new System.Drawing.Point(0, 0);
		this.menuStrip2.Name = "menuStrip2";
		this.menuStrip2.Size = new System.Drawing.Size(800, 24);
		this.menuStrip2.TabIndex = 3;
		this.menuStrip2.Text = "menuStrip2";
		this.menuStrip2.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(menuStrip2_ItemClicked);
		this.pnlPrincipal.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.pnlPrincipal.Controls.Add(this.label2);
		this.pnlPrincipal.Controls.Add(this.label1);
		this.pnlPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlPrincipal.Location = new System.Drawing.Point(0, 24);
		this.pnlPrincipal.Name = "pnlPrincipal";
		this.pnlPrincipal.Size = new System.Drawing.Size(800, 404);
		this.pnlPrincipal.TabIndex = 5;
		this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 30f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(288, 220);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(238, 46);
		this.label2.TabIndex = 1;
		this.label2.Text = "Technology";
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(264, 147);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(282, 73);
		this.label1.TabIndex = 0;
		this.label1.Text = "MODOC";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.pnlPrincipal);
		base.Controls.Add(this.statusStrip1);
		base.Controls.Add(this.menuStrip2);
		base.IsMdiContainer = true;
		base.Name = "frmPrincipal";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Capturador de Datos - MODOC";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmPrincipal_FormClosing);
		base.Load += new System.EventHandler(frmPrincipal_Load);
		this.statusStrip1.ResumeLayout(false);
		this.statusStrip1.PerformLayout();
		this.pnlPrincipal.ResumeLayout(false);
		this.pnlPrincipal.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
