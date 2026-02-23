using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._04_Separador;

public class frmDespachoSeparar : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string rutaCarpetaInicial;

	private string rutaLoteOrigen;

	private string rutaLoteDestino;

	private int totalRegistros;

	private int contador = 0;

	private IContainer components = null;

	private GroupBox gbSeleccionarCarpeta;

	private TextBox txtRutaCarpetaOrigen;

	private Button btnSeleccionarCarpetaOrigen;

	private GroupBox gbIndice;

	private DataGridView dgvIndice;

	private Button btnProcesar;

	private GroupBox groupBox1;

	private TextBox txtRutaCarpetaDestino;

	private Button btnSeleccionarCarpetaDestino;

	private ProgressBar progressBar1;

	private Button btnCancelar;

	private Button btnCerrar;

	public frmDespachoSeparar(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmDespachoSeparar_Load(object sender, EventArgs e)
	{
		deshabilitarFormulario();
		eProyectoConfiguracion oProyectoConfiguracion = nControlCalidad.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
	}

	private void habilitarFormulario()
	{
		btnSeleccionarCarpetaOrigen.Enabled = false;
		btnSeleccionarCarpetaDestino.Enabled = true;
		btnProcesar.Enabled = true;
		btnCancelar.Enabled = true;
	}

	private void deshabilitarFormulario()
	{
		btnSeleccionarCarpetaOrigen.Enabled = true;
		txtRutaCarpetaOrigen.Clear();
		dgvIndice.DataSource = null;
		btnSeleccionarCarpetaDestino.Enabled = false;
		txtRutaCarpetaDestino.Clear();
		btnProcesar.Enabled = false;
		btnCancelar.Enabled = false;
		progressBar1.Value = 0;
	}

	private void btnSeleccionarCarpeta_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLote = new FolderBrowserDialog();
		oSeleccionarLote.Description = "Seleccionar la carpeta a procesar";
		oSeleccionarLote.SelectedPath = rutaCarpetaInicial;
		oSeleccionarLote.ShowNewFolderButton = false;
		if (oSeleccionarLote.ShowDialog() == DialogResult.OK)
		{
			rutaLoteOrigen = oSeleccionarLote.SelectedPath;
			txtRutaCarpetaOrigen.Text = rutaLoteOrigen;
			dgvIndice.DataSource = nArchivos.cargarArchivoIndice(rutaLoteOrigen, "INDEX_TERMINADO.DAT");
			dgvIndice.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			totalRegistros = dgvIndice.Rows.Count;
			habilitarFormulario();
		}
	}

	private void btnProcesar_Click(object sender, EventArgs e)
	{
		if (validarProcesar())
		{
			procesar();
		}
	}

	private bool validarProcesar()
	{
		bool respuesta = true;
		if (string.IsNullOrEmpty(txtRutaCarpetaOrigen.Text))
		{
			MessageBox.Show("Debe seleccionar carpeta de origen", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			respuesta = false;
		}
		if (dgvIndice.Rows.Count == 0)
		{
			MessageBox.Show("No hay registros que procesar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			respuesta = false;
		}
		if (string.IsNullOrEmpty(txtRutaCarpetaDestino.Text))
		{
			MessageBox.Show("Debe seleccionar carpeta de destino", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			respuesta = false;
		}
		return respuesta;
	}

	private async Task procesar()
	{
		eDespacho oDespacho = new eDespacho();
		totalRegistros = dgvIndice.Rows.Count;
		contador = 0;
		progressBar1.Value = 0;
		foreach (DataGridViewRow row in (IEnumerable)dgvIndice.Rows)
		{
			oDespacho.id = Convert.ToInt32(row.Cells[0].Value.ToString());
			oDespacho.dsDespacho = row.Cells[1].Value.ToString();
			oDespacho.cdSerieDocumental = row.Cells[2].Value.ToString();
			oDespacho.nuSIGEA = row.Cells[3].Value.ToString();
			oDespacho.nuGuia = row.Cells[4].Value.ToString();
			oDespacho.dsUsuarioDigitalizacion = row.Cells[5].Value.ToString();
			string carpetaCreada = crearCarpeta(oDespacho);
			await procesarDespacho(oDespacho, carpetaCreada);
		}
		MessageBox.Show("El proceso se terminó correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		deshabilitarFormulario();
	}

	private async Task procesarDespacho(eDespacho pDespacho, string pCarpetaCreada)
	{
		try
		{
			nCodigoBarras_v4 onCodigoBarras_v4 = new nCodigoBarras_v4();
			await Task.Run(delegate
			{
				onCodigoBarras_v4.ProcesarPDF(rutaLoteOrigen, pDespacho, pCarpetaCreada, oUsuarioLogueado);
			});
			Interlocked.Increment(ref contador);
			Invoke(new Action(actualizarBarra));
		}
		catch (Exception ex)
		{
			Exception Ex = ex;
			MessageBox.Show(Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void actualizarBarra()
	{
		if (progressBar1.InvokeRequired)
		{
			progressBar1.Invoke(new Action(actualizarBarra));
		}
		else if (totalRegistros > 0)
		{
			decimal porcentajeProcesado = (decimal)contador / (decimal)totalRegistros * 100m;
			progressBar1.Value = Convert.ToInt32(porcentajeProcesado);
		}
	}

	private string crearCarpeta(eDespacho pDespacho)
	{
		return nSeparador.crearCarpeta(rutaLoteDestino, pDespacho);
	}

	private void btnSeleccionarCarpetaDestino_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLoteDestino = new FolderBrowserDialog();
		oSeleccionarLoteDestino.Description = "Seleccionar la carpeta destino";
		oSeleccionarLoteDestino.SelectedPath = "C:\\Lote\\Despachos\\BARRA\\Final2";
		oSeleccionarLoteDestino.ShowNewFolderButton = true;
		if (oSeleccionarLoteDestino.ShowDialog() == DialogResult.OK)
		{
			rutaLoteDestino = oSeleccionarLoteDestino.SelectedPath;
			txtRutaCarpetaDestino.Text = rutaLoteDestino;
		}
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		deshabilitarFormulario();
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
		this.gbSeleccionarCarpeta = new System.Windows.Forms.GroupBox();
		this.txtRutaCarpetaOrigen = new System.Windows.Forms.TextBox();
		this.btnSeleccionarCarpetaOrigen = new System.Windows.Forms.Button();
		this.gbIndice = new System.Windows.Forms.GroupBox();
		this.dgvIndice = new System.Windows.Forms.DataGridView();
		this.btnProcesar = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.txtRutaCarpetaDestino = new System.Windows.Forms.TextBox();
		this.btnSeleccionarCarpetaDestino = new System.Windows.Forms.Button();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.gbSeleccionarCarpeta.SuspendLayout();
		this.gbIndice.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvIndice).BeginInit();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.gbSeleccionarCarpeta.Controls.Add(this.txtRutaCarpetaOrigen);
		this.gbSeleccionarCarpeta.Controls.Add(this.btnSeleccionarCarpetaOrigen);
		this.gbSeleccionarCarpeta.Location = new System.Drawing.Point(13, 13);
		this.gbSeleccionarCarpeta.Name = "gbSeleccionarCarpeta";
		this.gbSeleccionarCarpeta.Size = new System.Drawing.Size(775, 74);
		this.gbSeleccionarCarpeta.TabIndex = 0;
		this.gbSeleccionarCarpeta.TabStop = false;
		this.txtRutaCarpetaOrigen.Enabled = false;
		this.txtRutaCarpetaOrigen.Location = new System.Drawing.Point(243, 20);
		this.txtRutaCarpetaOrigen.Multiline = true;
		this.txtRutaCarpetaOrigen.Name = "txtRutaCarpetaOrigen";
		this.txtRutaCarpetaOrigen.Size = new System.Drawing.Size(523, 40);
		this.txtRutaCarpetaOrigen.TabIndex = 1;
		this.btnSeleccionarCarpetaOrigen.Location = new System.Drawing.Point(7, 20);
		this.btnSeleccionarCarpetaOrigen.Name = "btnSeleccionarCarpetaOrigen";
		this.btnSeleccionarCarpetaOrigen.Size = new System.Drawing.Size(230, 40);
		this.btnSeleccionarCarpetaOrigen.TabIndex = 0;
		this.btnSeleccionarCarpetaOrigen.Text = "Seleccionar Carpeta Origen";
		this.btnSeleccionarCarpetaOrigen.UseVisualStyleBackColor = true;
		this.btnSeleccionarCarpetaOrigen.Click += new System.EventHandler(btnSeleccionarCarpeta_Click);
		this.gbIndice.Controls.Add(this.dgvIndice);
		this.gbIndice.Location = new System.Drawing.Point(13, 93);
		this.gbIndice.Name = "gbIndice";
		this.gbIndice.Size = new System.Drawing.Size(775, 153);
		this.gbIndice.TabIndex = 1;
		this.gbIndice.TabStop = false;
		this.gbIndice.Text = "Indice Encontrado";
		this.dgvIndice.AllowUserToAddRows = false;
		this.dgvIndice.AllowUserToDeleteRows = false;
		this.dgvIndice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvIndice.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dgvIndice.Location = new System.Drawing.Point(3, 16);
		this.dgvIndice.Name = "dgvIndice";
		this.dgvIndice.ReadOnly = true;
		this.dgvIndice.Size = new System.Drawing.Size(769, 134);
		this.dgvIndice.TabIndex = 0;
		this.btnProcesar.Location = new System.Drawing.Point(13, 329);
		this.btnProcesar.Name = "btnProcesar";
		this.btnProcesar.Size = new System.Drawing.Size(348, 61);
		this.btnProcesar.TabIndex = 2;
		this.btnProcesar.Text = "Procesar";
		this.btnProcesar.UseVisualStyleBackColor = true;
		this.btnProcesar.Click += new System.EventHandler(btnProcesar_Click);
		this.groupBox1.Controls.Add(this.txtRutaCarpetaDestino);
		this.groupBox1.Controls.Add(this.btnSeleccionarCarpetaDestino);
		this.groupBox1.Location = new System.Drawing.Point(13, 249);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(775, 74);
		this.groupBox1.TabIndex = 2;
		this.groupBox1.TabStop = false;
		this.txtRutaCarpetaDestino.Enabled = false;
		this.txtRutaCarpetaDestino.Location = new System.Drawing.Point(243, 20);
		this.txtRutaCarpetaDestino.Multiline = true;
		this.txtRutaCarpetaDestino.Name = "txtRutaCarpetaDestino";
		this.txtRutaCarpetaDestino.Size = new System.Drawing.Size(523, 40);
		this.txtRutaCarpetaDestino.TabIndex = 1;
		this.btnSeleccionarCarpetaDestino.Location = new System.Drawing.Point(7, 20);
		this.btnSeleccionarCarpetaDestino.Name = "btnSeleccionarCarpetaDestino";
		this.btnSeleccionarCarpetaDestino.Size = new System.Drawing.Size(230, 40);
		this.btnSeleccionarCarpetaDestino.TabIndex = 0;
		this.btnSeleccionarCarpetaDestino.Text = "Seleccionar Carpeta Destino";
		this.btnSeleccionarCarpetaDestino.UseVisualStyleBackColor = true;
		this.btnSeleccionarCarpetaDestino.Click += new System.EventHandler(btnSeleccionarCarpetaDestino_Click);
		this.progressBar1.Location = new System.Drawing.Point(12, 396);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(773, 42);
		this.progressBar1.TabIndex = 3;
		this.btnCancelar.Location = new System.Drawing.Point(367, 329);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(207, 61);
		this.btnCancelar.TabIndex = 4;
		this.btnCancelar.Text = "Cancelar";
		this.btnCancelar.UseVisualStyleBackColor = true;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.btnCerrar.Location = new System.Drawing.Point(580, 329);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(205, 61);
		this.btnCerrar.TabIndex = 5;
		this.btnCerrar.Text = "Cerrar";
		this.btnCerrar.UseVisualStyleBackColor = true;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.btnProcesar);
		base.Controls.Add(this.gbIndice);
		base.Controls.Add(this.gbSeleccionarCarpeta);
		base.Name = "frmDespachoSeparar";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "frmDespachoSeparar";
		base.Load += new System.EventHandler(frmDespachoSeparar_Load);
		this.gbSeleccionarCarpeta.ResumeLayout(false);
		this.gbSeleccionarCarpeta.PerformLayout();
		this.gbIndice.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvIndice).EndInit();
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		base.ResumeLayout(false);
	}
}
