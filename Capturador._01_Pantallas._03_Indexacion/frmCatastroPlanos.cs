using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmCatastroPlanos : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string rutaLote;

	private int totalArchivosEncontrados;

	private string rutaCarpetaInicial;

	private List<eDespacho> ListaFinalDespachosEncontrados = new List<eDespacho>();

	private List<eDespacho> ListaFinalDespachosEncontradosMultiples = new List<eDespacho>();

	private List<string> ListaFinalDespachosNoEncontrados = new List<string>();

	private eLote oLoteSeleccionado = new eLote();

	private IContainer components = null;

	private Button btnCerrar;

	private Button btnCancelar;

	private Label label3;

	private TextBox txtCdDocumento;

	private Label label4;

	private Label label5;

	private TextBox txtDesde;

	private TextBox txtHasta;

	private Button btnProcesar;

	private Panel pnlDetallePlanos;

	private DataGridView dgvDetallePlanos;

	private Button btnIngresarPlanos;

	public frmCatastroPlanos(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
	}

	private void frmCatastroPlanos_Load(object sender, EventArgs e)
	{
		IniciarEventoEnter();
		crearDataGridView();
		eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		ajustarFormularioInicial();
		cargarProximaSecuencia();
		actualizarBotones();
		txtCdDocumento.Focus();
	}

	private void IniciarEventoEnter()
	{
		txtCdDocumento.KeyDown += txt_KeyDown;
		txtDesde.KeyDown += txt_KeyDown;
		txtHasta.KeyDown += txt_KeyDown;
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

	private void cargarProximaSecuencia()
	{
		try
		{
			txtDesde.Text = Convert.ToString(obtenerProximoNuSecuencia());
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void ajustarFormularioInicial()
	{
		base.MaximizeBox = false;
		base.Size = new Size(550, 500);
		base.StartPosition = FormStartPosition.Manual;
		base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
	}

	private void crearDataGridView()
	{
		DataTable oTablaDetalle = new DataTable();
		oTablaDetalle.Columns.Add("Código Documento");
		oTablaDetalle.Columns.Add("Número Secuencia");
		oTablaDetalle.Columns.Add("Nombre de Archivo");
		dgvDetallePlanos.DataSource = oTablaDetalle;
		actualizarDetallePlanos();
	}

	private void actualizarDetallePlanos()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Planos a Ingresar";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvDetallePlanos.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Planos: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvDetallePlanos.Dock = DockStyle.Fill;
		pnlDetallePlanos.Controls.Clear();
		pnlDetallePlanos.Controls.Add(dgvDetallePlanos);
		pnlDetallePlanos.Controls.Add(labelTitulo);
		pnlDetallePlanos.Controls.Add(labelTotal);
	}

	private void btnCerrar_Click_1(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click_1(object sender, EventArgs e)
	{
		deshabilitarFormulario();
	}

	private int obtenerProximoNuSecuencia()
	{
		return nIndexacion.obtenerProximaSecuencia(oUsuarioLogueado);
	}

	private void deshabilitarFormulario()
	{
		txtCdDocumento.Clear();
		txtDesde.Clear();
		txtHasta.Clear();
		DataTable oCopiaTabla = new DataTable();
		oCopiaTabla = dgvDetallePlanos.DataSource as DataTable;
		oCopiaTabla.Rows.Clear();
		dgvDetallePlanos.DataSource = oCopiaTabla;
		actualizarDetallePlanos();
		cargarProximaSecuencia();
		actualizarBotones();
		txtCdDocumento.Focus();
	}

	private void btnIngresarPlanos_Click(object sender, EventArgs e)
	{
		DataTable oTablaPlanosIngresar = new DataTable();
		oTablaPlanosIngresar = dgvDetallePlanos.DataSource as DataTable;
		try
		{
			validarIngreso(oTablaPlanosIngresar);
			foreach (DataRow row in oTablaPlanosIngresar.Rows)
			{
				eCatastroPlanos oCatastroPlano = new eCatastroPlanos();
				oCatastroPlano.cdDocumento = row[0].ToString();
				oCatastroPlano.nuSecuencia = Convert.ToInt32(row[1].ToString());
				oCatastroPlano.dsNombreArchivo = row[2].ToString();
				IngresarPlano(oCatastroPlano);
			}
			MessageBox.Show("Se ingresaron los Planos correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			deshabilitarFormulario();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void validarIngreso(DataTable pTabla)
	{
		foreach (DataRow row in pTabla.Rows)
		{
			eCatastroPlanos oCatastroPlano = new eCatastroPlanos();
			oCatastroPlano.cdDocumento = row[0].ToString();
			oCatastroPlano.nuSecuencia = Convert.ToInt32(row[1].ToString());
			oCatastroPlano.dsNombreArchivo = row[2].ToString();
			nIndexacion.validarExistePlano(oUsuarioLogueado, oCatastroPlano);
		}
	}

	private void IngresarPlano(eCatastroPlanos pCatastroPlano)
	{
		nIndexacion.agregarCatastroPlanos(oUsuarioLogueado, pCatastroPlano);
	}

	private void btnProcesar_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtCdDocumento.Text) || string.IsNullOrEmpty(txtDesde.Text) || string.IsNullOrEmpty(txtHasta.Text))
		{
			MessageBox.Show("Debe ingresar todos los campos", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		DataTable oCopiaTabla = new DataTable();
		oCopiaTabla = dgvDetallePlanos.DataSource as DataTable;
		oCopiaTabla.Rows.Clear();
		dgvDetallePlanos.DataSource = oCopiaTabla;
		procesar();
		actualizarBotones();
	}

	private void procesar()
	{
		if (int.TryParse(txtDesde.Text, out var desde) && int.TryParse(txtHasta.Text, out var hasta))
		{
			if (desde <= hasta)
			{
				for (int i = desde; i <= hasta; i++)
				{
					agregarPlano(i);
				}
			}
			else
			{
				MessageBox.Show("El valor 'Desde' debe ser menor o igual que el valor 'Hasta'.");
			}
		}
		else
		{
			MessageBox.Show("Por favor, ingresa números válidos en los campos.");
		}
	}

	private void agregarPlano(int pNumeroPlano)
	{
		DataTable oCopiaTabla = new DataTable();
		oCopiaTabla = dgvDetallePlanos.DataSource as DataTable;
		DataRow dataRow = oCopiaTabla.NewRow();
		dataRow[0] = txtCdDocumento.Text;
		dataRow[1] = pNumeroPlano;
		string numeroPlano = pNumeroPlano.ToString("D3");
		dataRow[2] = "scan" + numeroPlano + ".pdf";
		oCopiaTabla.Rows.Add(dataRow);
		dgvDetallePlanos.DataSource = oCopiaTabla;
		actualizarDetallePlanos();
	}

	private void actualizarBotones()
	{
		if (dgvDetallePlanos.Rows.Count != 0)
		{
			btnIngresarPlanos.Enabled = true;
			btnIngresarPlanos.BackColor = Color.SeaGreen;
			btnCancelar.Enabled = true;
			btnCancelar.BackColor = Color.Salmon;
		}
		else
		{
			btnIngresarPlanos.Enabled = false;
			btnIngresarPlanos.BackColor = Color.DarkGray;
			btnCancelar.Enabled = false;
			btnCancelar.BackColor = Color.DarkGray;
		}
	}

	private void dgvDetallePlanos_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.RowIndex >= 0)
		{
			DataGridViewRow row = dgvDetallePlanos.Rows[e.RowIndex];
			DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar esta fila?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{
				dgvDetallePlanos.Rows.Remove(row);
				actualizarDetallePlanos();
			}
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._03_Indexacion.frmCatastroPlanos));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.label3 = new System.Windows.Forms.Label();
		this.txtCdDocumento = new System.Windows.Forms.TextBox();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.txtDesde = new System.Windows.Forms.TextBox();
		this.txtHasta = new System.Windows.Forms.TextBox();
		this.btnProcesar = new System.Windows.Forms.Button();
		this.pnlDetallePlanos = new System.Windows.Forms.Panel();
		this.dgvDetallePlanos = new System.Windows.Forms.DataGridView();
		this.btnIngresarPlanos = new System.Windows.Forms.Button();
		this.pnlDetallePlanos.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDetallePlanos).BeginInit();
		base.SuspendLayout();
		this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
		this.btnCerrar.FlatAppearance.BorderSize = 0;
		this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCerrar.ForeColor = System.Drawing.Color.White;
		this.btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
		this.btnCerrar.Location = new System.Drawing.Point(379, 425);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(130, 24);
		this.btnCerrar.TabIndex = 63;
		this.btnCerrar.Text = "   Cerrar";
		this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCerrar.UseVisualStyleBackColor = false;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click_1);
		this.btnCancelar.BackColor = System.Drawing.Color.DarkGray;
		this.btnCancelar.Enabled = false;
		this.btnCancelar.FlatAppearance.BorderSize = 0;
		this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancelar.ForeColor = System.Drawing.Color.White;
		this.btnCancelar.Image = (System.Drawing.Image)resources.GetObject("btnCancelar.Image");
		this.btnCancelar.Location = new System.Drawing.Point(243, 424);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(130, 25);
		this.btnCancelar.TabIndex = 62;
		this.btnCancelar.Text = "   Cancelar";
		this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnCancelar.UseVisualStyleBackColor = false;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click_1);
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(23, 28);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(146, 16);
		this.label3.TabIndex = 68;
		this.label3.Text = "Código de Documento:";
		this.txtCdDocumento.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCdDocumento.Location = new System.Drawing.Point(177, 22);
		this.txtCdDocumento.Name = "txtCdDocumento";
		this.txtCdDocumento.Size = new System.Drawing.Size(332, 26);
		this.txtCdDocumento.TabIndex = 1;
		this.txtCdDocumento.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.White;
		this.label4.Location = new System.Drawing.Point(23, 69);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(52, 16);
		this.label4.TabIndex = 70;
		this.label4.Text = "Desde:";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.ForeColor = System.Drawing.Color.White;
		this.label5.Location = new System.Drawing.Point(23, 101);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(47, 16);
		this.label5.TabIndex = 71;
		this.label5.Text = "Hasta:";
		this.txtDesde.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDesde.Location = new System.Drawing.Point(177, 63);
		this.txtDesde.Name = "txtDesde";
		this.txtDesde.Size = new System.Drawing.Size(332, 26);
		this.txtDesde.TabIndex = 2;
		this.txtDesde.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.txtHasta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtHasta.Location = new System.Drawing.Point(177, 95);
		this.txtHasta.Name = "txtHasta";
		this.txtHasta.Size = new System.Drawing.Size(332, 26);
		this.txtHasta.TabIndex = 3;
		this.txtHasta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.btnProcesar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnProcesar.FlatAppearance.BorderSize = 0;
		this.btnProcesar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnProcesar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnProcesar.ForeColor = System.Drawing.Color.White;
		this.btnProcesar.Image = (System.Drawing.Image)resources.GetObject("btnProcesar.Image");
		this.btnProcesar.Location = new System.Drawing.Point(23, 138);
		this.btnProcesar.Name = "btnProcesar";
		this.btnProcesar.Size = new System.Drawing.Size(486, 25);
		this.btnProcesar.TabIndex = 4;
		this.btnProcesar.Text = "   Procesar";
		this.btnProcesar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnProcesar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnProcesar.UseVisualStyleBackColor = false;
		this.btnProcesar.Click += new System.EventHandler(btnProcesar_Click);
		this.pnlDetallePlanos.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlDetallePlanos.Controls.Add(this.dgvDetallePlanos);
		this.pnlDetallePlanos.Location = new System.Drawing.Point(23, 172);
		this.pnlDetallePlanos.Name = "pnlDetallePlanos";
		this.pnlDetallePlanos.Size = new System.Drawing.Size(483, 232);
		this.pnlDetallePlanos.TabIndex = 65;
		this.dgvDetallePlanos.AllowUserToAddRows = false;
		this.dgvDetallePlanos.AllowUserToDeleteRows = false;
		this.dgvDetallePlanos.AllowUserToResizeRows = false;
		this.dgvDetallePlanos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dgvDetallePlanos.BackgroundColor = System.Drawing.Color.FromArgb(49, 66, 82);
		this.dgvDetallePlanos.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dgvDetallePlanos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
		this.dgvDetallePlanos.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(77, 96, 130);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDetallePlanos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dgvDetallePlanos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dgvDetallePlanos.DefaultCellStyle = dataGridViewCellStyle2;
		this.dgvDetallePlanos.EnableHeadersVisualStyles = false;
		this.dgvDetallePlanos.GridColor = System.Drawing.Color.SteelBlue;
		this.dgvDetallePlanos.Location = new System.Drawing.Point(27, 38);
		this.dgvDetallePlanos.Name = "dgvDetallePlanos";
		this.dgvDetallePlanos.ReadOnly = true;
		this.dgvDetallePlanos.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvDetallePlanos.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dgvDetallePlanos.RowHeadersVisible = false;
		this.dgvDetallePlanos.RowHeadersWidth = 15;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dgvDetallePlanos.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dgvDetallePlanos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvDetallePlanos.Size = new System.Drawing.Size(443, 139);
		this.dgvDetallePlanos.TabIndex = 18;
		this.dgvDetallePlanos.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDetallePlanos_CellContentDoubleClick);
		this.btnIngresarPlanos.BackColor = System.Drawing.Color.SeaGreen;
		this.btnIngresarPlanos.FlatAppearance.BorderSize = 0;
		this.btnIngresarPlanos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnIngresarPlanos.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnIngresarPlanos.ForeColor = System.Drawing.Color.White;
		this.btnIngresarPlanos.Image = (System.Drawing.Image)resources.GetObject("btnIngresarPlanos.Image");
		this.btnIngresarPlanos.Location = new System.Drawing.Point(28, 424);
		this.btnIngresarPlanos.Name = "btnIngresarPlanos";
		this.btnIngresarPlanos.Size = new System.Drawing.Size(212, 25);
		this.btnIngresarPlanos.TabIndex = 5;
		this.btnIngresarPlanos.Text = "   Ingresar Planos";
		this.btnIngresarPlanos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnIngresarPlanos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnIngresarPlanos.UseVisualStyleBackColor = false;
		this.btnIngresarPlanos.Click += new System.EventHandler(btnIngresarPlanos_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(534, 461);
		base.Controls.Add(this.btnIngresarPlanos);
		base.Controls.Add(this.pnlDetallePlanos);
		base.Controls.Add(this.btnProcesar);
		base.Controls.Add(this.txtHasta);
		base.Controls.Add(this.txtDesde);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.txtCdDocumento);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.MinimizeBox = false;
		base.Name = "frmCatastroPlanos";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Ingreso de Planos de Catastro";
		base.Load += new System.EventHandler(frmCatastroPlanos_Load);
		this.pnlDetallePlanos.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDetallePlanos).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
