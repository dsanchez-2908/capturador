using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Capturador._02_Negocio;
using Capturador._04_Entidades;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmDespachosAutomatico : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private string rutaLote;

	private int totalArchivosEncontrados;

	private string rutaCarpetaInicial;

	private IContainer components = null;

	private GroupBox groupBox1;

	private TextBox txtRutaCarpeta;

	private Button btnSeleccionarCarpeta;

	private GroupBox gboxArchivosEncontrados;

	private ListBox lboxArchivosEncontrados;

	private GroupBox groupBox2;

	private DataGridView dgvDespachosEncontradosFinal;

	private GroupBox gboxDespachosMasDeUno;

	private DataGridView dgvDespachosEncontrados;

	private GroupBox gboxDespachosNoEncontrados;

	private DataGridView dgvDespachosNoEncontrados;

	private DataGridViewTextBoxColumn id;

	private DataGridViewTextBoxColumn dsDespacho;

	private DataGridViewTextBoxColumn cdSerieDocumental;

	private DataGridViewTextBoxColumn nuSIGEA;

	private DataGridViewTextBoxColumn nuGuia;

	private GroupBox groupBox6;

	private Button btnVerDespacho;

	private TextBox txtDespachoSeleccionado;

	private Label label1;

	private ComboBox cbxUsuarioDigitalizacion;

	private Button btnGenerarIndice;

	private Button btnCancelar;

	private Button btnCerrar;

	private Label label2;

	private ComboBox cbxOrigen;

	private Button btnCargarLote;

	public frmDespachosAutomatico(eUsuario pUsuario)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
	}

	private void frmDespachosAutomatico_Load(object sender, EventArgs e)
	{
		deshabilitarFormulario();
		ajustarFormulario();
		llenarListas();
		eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
		rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
		txtRutaCarpeta.Text = rutaCarpetaInicial;
	}

	private void llenarListas()
	{
		cbxUsuarioDigitalizacion.DataSource = nIndexacion.ObtenerLista(oUsuarioLogueado, 1, 1);
		cbxUsuarioDigitalizacion.DisplayMember = "dsValorLista";
		cbxUsuarioDigitalizacion.SelectedIndex = -1;
		cbxOrigen.DataSource = nIndexacion.ObtenerLista(oUsuarioLogueado, 1, 2);
		cbxOrigen.DisplayMember = "dsValorLista";
		cbxOrigen.SelectedIndex = -1;
	}

	private void btnCerrar_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click(object sender, EventArgs e)
	{
		deshabilitarFormulario();
	}

	private void deshabilitarFormulario()
	{
		lboxArchivosEncontrados.DataSource = null;
		gboxArchivosEncontrados.Text = "Archivos Encontrados: ";
		btnVerDespacho.Enabled = false;
		cbxUsuarioDigitalizacion.SelectedIndex = -1;
		cbxUsuarioDigitalizacion.Enabled = false;
		cbxOrigen.SelectedIndex = -1;
		cbxOrigen.Enabled = false;
		btnGenerarIndice.Enabled = false;
		btnCancelar.Enabled = false;
		btnSeleccionarCarpeta.Enabled = true;
		dgvDespachosEncontradosFinal.DataSource = null;
		dgvDespachosEncontradosFinal.Rows.Clear();
		dgvDespachosEncontradosFinal.Columns.Clear();
		dgvDespachosNoEncontrados.Rows.Clear();
		dgvDespachosNoEncontrados.Columns.Clear();
		dgvDespachosNoEncontrados.DataSource = null;
		if (dgvDespachosNoEncontrados.Rows.Count == 0)
		{
			dgvDespachosNoEncontrados.Visible = false;
		}
		else
		{
			dgvDespachosNoEncontrados.Visible = true;
		}
		btnSeleccionarCarpeta.Enabled = true;
		btnCargarLote.Enabled = true;
		dgvDespachosNoEncontrados.Rows.Clear();
		dgvDespachosEncontrados.Columns.Clear();
		dgvDespachosEncontrados.DataSource = null;
		txtDespachoSeleccionado.Clear();
		ajustarFormulario();
	}

	private void habilitarFormulario()
	{
		btnSeleccionarCarpeta.Enabled = false;
		btnVerDespacho.Enabled = true;
		cbxUsuarioDigitalizacion.SelectedIndex = -1;
		cbxUsuarioDigitalizacion.Enabled = true;
		cbxOrigen.SelectedIndex = -1;
		cbxOrigen.Enabled = true;
		if (dgvDespachosNoEncontrados.Rows.Count == 0)
		{
			dgvDespachosNoEncontrados.Visible = false;
		}
		else
		{
			dgvDespachosNoEncontrados.Visible = true;
		}
		btnGenerarIndice.Enabled = true;
		btnGenerarIndice.Enabled = true;
		btnCancelar.Enabled = true;
	}

	private void btnSeleccionarCarpeta_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog oSeleccionarLote = new FolderBrowserDialog();
		oSeleccionarLote.Description = "Seleccionar la carpeta del Lote a Indexar";
		oSeleccionarLote.SelectedPath = rutaCarpetaInicial;
		oSeleccionarLote.ShowNewFolderButton = false;
		if (oSeleccionarLote.ShowDialog() == DialogResult.OK)
		{
			rutaLote = oSeleccionarLote.SelectedPath;
			txtRutaCarpeta.Text = rutaLote;
			nConfiguracion.actualizarUltimaCarpetaOrigen(oUsuarioLogueado, 1, rutaCarpetaInicial, txtRutaCarpeta.Text);
			LlenarListaNombreArchivos();
		}
	}

	private void btnCargarLote_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtRutaCarpeta.Text))
		{
			MessageBox.Show("Sebe seleccionar una carpeta", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		rutaLote = txtRutaCarpeta.Text;
		LlenarListaNombreArchivos();
	}

	private void LlenarListaNombreArchivos()
	{
		try
		{
			lboxArchivosEncontrados.DataSource = nIndexacion.ObtenerNombreArchivos(txtRutaCarpeta.Text);
			if (lboxArchivosEncontrados.Items.Count == 0)
			{
				MessageBox.Show("No se encontraron archivos en la carpeta seleccionada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			btnSeleccionarCarpeta.Enabled = false;
			btnCargarLote.Enabled = false;
			totalArchivosEncontrados = lboxArchivosEncontrados.Items.Count;
			gboxArchivosEncontrados.Text = "Archivos Encontrados: " + Convert.ToString(totalArchivosEncontrados);
			procesarArchivosEncontrados();
			habilitarFormulario();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void procesarArchivosEncontrados()
	{
		foreach (object archivoEncontrado in lboxArchivosEncontrados.Items)
		{
			string archivo = Path.GetFileNameWithoutExtension(Convert.ToString(archivoEncontrado));
			AgregarResultado(archivo);
		}
		ajustarFormulario();
	}

	private void ajustarFormulario()
	{
		if (dgvDespachosEncontrados.Rows.Count > 1 || dgvDespachosNoEncontrados.Rows.Count > 1)
		{
			base.Size = new Size(1050, 650);
			base.StartPosition = FormStartPosition.Manual;
			base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
			if (dgvDespachosNoEncontrados.Rows.Count == 0)
			{
				gboxDespachosNoEncontrados.Visible = false;
			}
		}
		if (dgvDespachosEncontrados.Rows.Count == 0 && dgvDespachosNoEncontrados.Rows.Count == 0)
		{
			base.Size = new Size(536, 650);
			base.StartPosition = FormStartPosition.Manual;
			base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
		}
	}

	private void AgregarResultado(string pDespacho)
	{
		DataTable oTableDespachosEncotrados = new DataTable();
		oTableDespachosEncotrados = nIndexacion.BuscarDespacho(pDespacho);
		if (oTableDespachosEncotrados.Rows.Count == 0)
		{
			if (dgvDespachosNoEncontrados.DataSource == null)
			{
				dgvDespachosNoEncontrados.Columns.Add("id", "id");
				dgvDespachosNoEncontrados.Columns.Add("dsDespacho", "dsDespacho");
				dgvDespachosNoEncontrados.Columns.Add("cdSerieDocumental", "cdSerieDocumental");
				dgvDespachosNoEncontrados.Columns.Add("nuSIGEA", "nuSIGEA");
				dgvDespachosNoEncontrados.Columns.Add("nuGuia", "nuGuia");
			}
			dgvDespachosNoEncontrados.Rows.Add("", pDespacho, "", "", "");
		}
		else if (oTableDespachosEncotrados.Rows.Count == 1)
		{
			if (dgvDespachosEncontradosFinal.Rows.Count == 0)
			{
				dgvDespachosEncontradosFinal.DataSource = oTableDespachosEncotrados;
			}
			else
			{
				DataTable oTableActual = (DataTable)dgvDespachosEncontradosFinal.DataSource;
				foreach (DataRow row in oTableDespachosEncotrados.Rows)
				{
					oTableActual.ImportRow(row);
				}
				dgvDespachosEncontradosFinal.DataSource = oTableActual;
			}
		}
		else if (dgvDespachosEncontrados.Rows.Count == 0)
		{
			dgvDespachosEncontrados.DataSource = oTableDespachosEncotrados;
		}
		else
		{
			DataTable oTableActual2 = (DataTable)dgvDespachosEncontrados.DataSource;
			foreach (DataRow row2 in oTableDespachosEncotrados.Rows)
			{
				oTableActual2.ImportRow(row2);
			}
			dgvDespachosEncontrados.DataSource = oTableActual2;
		}
		dgvDespachosEncontradosFinal.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		dgvDespachosEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		dgvDespachosNoEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
	}

	private void dgvDespachosEncontradosFinal_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		txtDespachoSeleccionado.Text = dgvDespachosEncontradosFinal.CurrentRow.Cells[1].Value.ToString();
	}

	private void dgvDespachosEncontrados_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		txtDespachoSeleccionado.Text = dgvDespachosEncontrados.CurrentRow.Cells[1].Value.ToString();
	}

	private void dgvDespachosNoEncontrados_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		txtDespachoSeleccionado.Text = dgvDespachosNoEncontrados.CurrentRow.Cells[1].Value.ToString();
	}

	private void btnVerDespacho_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtDespachoSeleccionado.Text))
		{
			MessageBox.Show("Debe seleccionar un despacho", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		frmVerArchivoPDF ofrmVerArchivoPDF = new frmVerArchivoPDF("Despacho: " + txtDespachoSeleccionado.Text, rutaLote, txtDespachoSeleccionado.Text + ".pdf");
		ofrmVerArchivoPDF.Show();
	}

	private void dgvDespachosEncontrados_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		string id = dgvDespachosEncontrados.CurrentRow.Cells[0].Value.ToString();
		string dsDespacho = dgvDespachosEncontrados.CurrentRow.Cells[1].Value.ToString();
		string nuSerieDocumental = dgvDespachosEncontrados.CurrentRow.Cells[2].Value.ToString();
		string nuSIGEA = dgvDespachosEncontrados.CurrentRow.Cells[3].Value.ToString();
		string nuGuia = dgvDespachosEncontrados.CurrentRow.Cells[4].Value.ToString();
		DataTable oDespachosEncontradosFinalActual = (DataTable)dgvDespachosEncontradosFinal.DataSource;
		if (dgvDespachosEncontradosFinal.Rows.Count == 0)
		{
			DataTable dtManual = new DataTable();
			dtManual.Columns.Add("id");
			dtManual.Columns.Add("dsDespacho");
			dtManual.Columns.Add("cdSerieDocumental");
			dtManual.Columns.Add("nuSIGEA");
			dtManual.Columns.Add("nuGuia");
			DataRow row = dtManual.NewRow();
			row["id"] = id;
			row["dsDespacho"] = dsDespacho;
			row["cdSerieDocumental"] = nuSerieDocumental;
			row["nuSIGEA"] = nuSIGEA;
			row["nuGuia"] = nuGuia;
			dtManual.Rows.Add(row);
			dgvDespachosEncontradosFinal.DataSource = dtManual;
		}
		else
		{
			oDespachosEncontradosFinalActual.Rows.Add(id, dsDespacho, nuSerieDocumental, nuSIGEA, nuGuia);
			dgvDespachosEncontradosFinal.DataSource = oDespachosEncontradosFinalActual;
		}
		dgvDespachosEncontradosFinal.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		DataTable oDespachosEncontradosActual = (DataTable)dgvDespachosEncontrados.DataSource;
		for (int i = oDespachosEncontradosActual.Rows.Count - 1; i >= 0; i--)
		{
			DataRow fila = oDespachosEncontradosActual.Rows[i];
			if (fila[1].ToString() == dsDespacho)
			{
				oDespachosEncontradosActual.Rows.Remove(fila);
			}
		}
		dgvDespachosEncontrados.DataSource = oDespachosEncontradosActual;
		dgvDespachosEncontrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		ajustarFormulario();
	}

	private void btnGenerarIndice_Click(object sender, EventArgs e)
	{
		int totalRegistosEncontrados = dgvDespachosEncontradosFinal.Rows.Count;
		if (totalArchivosEncontrados != totalRegistosEncontrados)
		{
			MessageBox.Show("No coincide la cantidad de archivos con los despachos encontrados", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string valorUsuarioDigitalizacion = cbxUsuarioDigitalizacion.Text;
		string origen = cbxOrigen.Text;
		if (!string.IsNullOrEmpty(valorUsuarioDigitalizacion))
		{
			DataTable oDespachosEncontradosFinalExportar = (DataTable)dgvDespachosEncontradosFinal.DataSource;
			oDespachosEncontradosFinalExportar.Columns.Add("usuarioDigitalizacion", typeof(string));
			oDespachosEncontradosFinalExportar.Columns.Add("origen", typeof(string));
			foreach (DataRow row in oDespachosEncontradosFinalExportar.Rows)
			{
				row["usuarioDigitalizacion"] = valorUsuarioDigitalizacion;
				row["origen"] = origen;
			}
			nArchivos.GenerarArchivoIndice(rutaLote, "INDEX_TERMINADO.DAT", oDespachosEncontradosFinalExportar);
			MessageBox.Show("El Indice se generó correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			deshabilitarFormulario();
		}
		else
		{
			MessageBox.Show("Seleccione un usuario de digitalización de la lista antes de continuar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.txtRutaCarpeta = new System.Windows.Forms.TextBox();
		this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
		this.gboxArchivosEncontrados = new System.Windows.Forms.GroupBox();
		this.lboxArchivosEncontrados = new System.Windows.Forms.ListBox();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.dgvDespachosEncontradosFinal = new System.Windows.Forms.DataGridView();
		this.gboxDespachosMasDeUno = new System.Windows.Forms.GroupBox();
		this.dgvDespachosEncontrados = new System.Windows.Forms.DataGridView();
		this.gboxDespachosNoEncontrados = new System.Windows.Forms.GroupBox();
		this.dgvDespachosNoEncontrados = new System.Windows.Forms.DataGridView();
		this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dsDespacho = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.cdSerieDocumental = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.nuSIGEA = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.nuGuia = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.groupBox6 = new System.Windows.Forms.GroupBox();
		this.btnVerDespacho = new System.Windows.Forms.Button();
		this.txtDespachoSeleccionado = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.cbxUsuarioDigitalizacion = new System.Windows.Forms.ComboBox();
		this.btnGenerarIndice = new System.Windows.Forms.Button();
		this.btnCancelar = new System.Windows.Forms.Button();
		this.btnCerrar = new System.Windows.Forms.Button();
		this.label2 = new System.Windows.Forms.Label();
		this.cbxOrigen = new System.Windows.Forms.ComboBox();
		this.btnCargarLote = new System.Windows.Forms.Button();
		this.groupBox1.SuspendLayout();
		this.gboxArchivosEncontrados.SuspendLayout();
		this.groupBox2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontradosFinal).BeginInit();
		this.gboxDespachosMasDeUno.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontrados).BeginInit();
		this.gboxDespachosNoEncontrados.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosNoEncontrados).BeginInit();
		this.groupBox6.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.btnCargarLote);
		this.groupBox1.Controls.Add(this.txtRutaCarpeta);
		this.groupBox1.Controls.Add(this.btnSeleccionarCarpeta);
		this.groupBox1.Location = new System.Drawing.Point(13, 13);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(236, 115);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.txtRutaCarpeta.Enabled = false;
		this.txtRutaCarpeta.Location = new System.Drawing.Point(10, 58);
		this.txtRutaCarpeta.Multiline = true;
		this.txtRutaCarpeta.Name = "txtRutaCarpeta";
		this.txtRutaCarpeta.Size = new System.Drawing.Size(215, 44);
		this.txtRutaCarpeta.TabIndex = 1;
		this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(8, 18);
		this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
		this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(141, 34);
		this.btnSeleccionarCarpeta.TabIndex = 0;
		this.btnSeleccionarCarpeta.Text = "Seleccionar Carpeta";
		this.btnSeleccionarCarpeta.UseVisualStyleBackColor = true;
		this.btnSeleccionarCarpeta.Click += new System.EventHandler(btnSeleccionarCarpeta_Click);
		this.gboxArchivosEncontrados.Controls.Add(this.lboxArchivosEncontrados);
		this.gboxArchivosEncontrados.Location = new System.Drawing.Point(255, 13);
		this.gboxArchivosEncontrados.Name = "gboxArchivosEncontrados";
		this.gboxArchivosEncontrados.Size = new System.Drawing.Size(240, 115);
		this.gboxArchivosEncontrados.TabIndex = 1;
		this.gboxArchivosEncontrados.TabStop = false;
		this.gboxArchivosEncontrados.Text = "Archivos Encontrados:";
		this.lboxArchivosEncontrados.FormattingEnabled = true;
		this.lboxArchivosEncontrados.Location = new System.Drawing.Point(8, 20);
		this.lboxArchivosEncontrados.Name = "lboxArchivosEncontrados";
		this.lboxArchivosEncontrados.Size = new System.Drawing.Size(218, 82);
		this.lboxArchivosEncontrados.TabIndex = 0;
		this.groupBox2.Controls.Add(this.dgvDespachosEncontradosFinal);
		this.groupBox2.Location = new System.Drawing.Point(13, 134);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(482, 250);
		this.groupBox2.TabIndex = 3;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Despachos Encontrados";
		this.dgvDespachosEncontradosFinal.AllowUserToAddRows = false;
		this.dgvDespachosEncontradosFinal.AllowUserToDeleteRows = false;
		this.dgvDespachosEncontradosFinal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvDespachosEncontradosFinal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dgvDespachosEncontradosFinal.Location = new System.Drawing.Point(3, 16);
		this.dgvDespachosEncontradosFinal.Name = "dgvDespachosEncontradosFinal";
		this.dgvDespachosEncontradosFinal.ReadOnly = true;
		this.dgvDespachosEncontradosFinal.RowHeadersVisible = false;
		this.dgvDespachosEncontradosFinal.Size = new System.Drawing.Size(476, 231);
		this.dgvDespachosEncontradosFinal.TabIndex = 0;
		this.dgvDespachosEncontradosFinal.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosEncontradosFinal_CellContentClick);
		this.gboxDespachosMasDeUno.Controls.Add(this.dgvDespachosEncontrados);
		this.gboxDespachosMasDeUno.Location = new System.Drawing.Point(547, 133);
		this.gboxDespachosMasDeUno.Name = "gboxDespachosMasDeUno";
		this.gboxDespachosMasDeUno.Size = new System.Drawing.Size(472, 247);
		this.gboxDespachosMasDeUno.TabIndex = 4;
		this.gboxDespachosMasDeUno.TabStop = false;
		this.gboxDespachosMasDeUno.Text = "Despachos con más de un registros";
		this.dgvDespachosEncontrados.AllowUserToAddRows = false;
		this.dgvDespachosEncontrados.AllowUserToDeleteRows = false;
		this.dgvDespachosEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvDespachosEncontrados.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dgvDespachosEncontrados.Location = new System.Drawing.Point(3, 16);
		this.dgvDespachosEncontrados.Name = "dgvDespachosEncontrados";
		this.dgvDespachosEncontrados.ReadOnly = true;
		this.dgvDespachosEncontrados.RowHeadersVisible = false;
		this.dgvDespachosEncontrados.Size = new System.Drawing.Size(466, 228);
		this.dgvDespachosEncontrados.TabIndex = 0;
		this.dgvDespachosEncontrados.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosEncontrados_CellContentClick);
		this.dgvDespachosEncontrados.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosEncontrados_CellContentDoubleClick);
		this.gboxDespachosNoEncontrados.Controls.Add(this.dgvDespachosNoEncontrados);
		this.gboxDespachosNoEncontrados.Location = new System.Drawing.Point(553, 12);
		this.gboxDespachosNoEncontrados.Name = "gboxDespachosNoEncontrados";
		this.gboxDespachosNoEncontrados.Size = new System.Drawing.Size(469, 115);
		this.gboxDespachosNoEncontrados.TabIndex = 5;
		this.gboxDespachosNoEncontrados.TabStop = false;
		this.gboxDespachosNoEncontrados.Text = "Despachos no encontrados:";
		this.dgvDespachosNoEncontrados.AllowUserToAddRows = false;
		this.dgvDespachosNoEncontrados.AllowUserToDeleteRows = false;
		this.dgvDespachosNoEncontrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvDespachosNoEncontrados.Columns.AddRange(this.id, this.dsDespacho, this.cdSerieDocumental, this.nuSIGEA, this.nuGuia);
		this.dgvDespachosNoEncontrados.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dgvDespachosNoEncontrados.Location = new System.Drawing.Point(3, 16);
		this.dgvDespachosNoEncontrados.Name = "dgvDespachosNoEncontrados";
		this.dgvDespachosNoEncontrados.RowHeadersVisible = false;
		this.dgvDespachosNoEncontrados.Size = new System.Drawing.Size(463, 96);
		this.dgvDespachosNoEncontrados.TabIndex = 0;
		this.dgvDespachosNoEncontrados.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dgvDespachosNoEncontrados_CellContentClick);
		this.id.HeaderText = "id";
		this.id.Name = "id";
		this.dsDespacho.HeaderText = "dsDespacho";
		this.dsDespacho.Name = "dsDespacho";
		this.cdSerieDocumental.HeaderText = "cdSerieDocumental";
		this.cdSerieDocumental.Name = "cdSerieDocumental";
		this.nuSIGEA.HeaderText = "nuSIGEA";
		this.nuSIGEA.Name = "nuSIGEA";
		this.nuGuia.HeaderText = "nuGuia";
		this.nuGuia.Name = "nuGuia";
		this.groupBox6.Controls.Add(this.btnVerDespacho);
		this.groupBox6.Controls.Add(this.txtDespachoSeleccionado);
		this.groupBox6.Location = new System.Drawing.Point(13, 390);
		this.groupBox6.Name = "groupBox6";
		this.groupBox6.Size = new System.Drawing.Size(482, 51);
		this.groupBox6.TabIndex = 7;
		this.groupBox6.TabStop = false;
		this.groupBox6.Text = "Despacho Seleccionado";
		this.btnVerDespacho.Location = new System.Drawing.Point(268, 14);
		this.btnVerDespacho.Name = "btnVerDespacho";
		this.btnVerDespacho.Size = new System.Drawing.Size(166, 23);
		this.btnVerDespacho.TabIndex = 1;
		this.btnVerDespacho.Text = "Ver Despacho";
		this.btnVerDespacho.UseVisualStyleBackColor = true;
		this.btnVerDespacho.Click += new System.EventHandler(btnVerDespacho_Click);
		this.txtDespachoSeleccionado.Enabled = false;
		this.txtDespachoSeleccionado.Location = new System.Drawing.Point(65, 17);
		this.txtDespachoSeleccionado.Name = "txtDespachoSeleccionado";
		this.txtDespachoSeleccionado.Size = new System.Drawing.Size(171, 20);
		this.txtDespachoSeleccionado.TabIndex = 0;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(20, 466);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(126, 13);
		this.label1.TabIndex = 8;
		this.label1.Text = "Usuario de Digitalización:";
		this.cbxUsuarioDigitalizacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxUsuarioDigitalizacion.FormattingEnabled = true;
		this.cbxUsuarioDigitalizacion.Location = new System.Drawing.Point(173, 463);
		this.cbxUsuarioDigitalizacion.Name = "cbxUsuarioDigitalizacion";
		this.cbxUsuarioDigitalizacion.Size = new System.Drawing.Size(308, 21);
		this.cbxUsuarioDigitalizacion.TabIndex = 9;
		this.btnGenerarIndice.Location = new System.Drawing.Point(12, 559);
		this.btnGenerarIndice.Name = "btnGenerarIndice";
		this.btnGenerarIndice.Size = new System.Drawing.Size(150, 40);
		this.btnGenerarIndice.TabIndex = 10;
		this.btnGenerarIndice.Text = "Generar Indice";
		this.btnGenerarIndice.UseVisualStyleBackColor = true;
		this.btnGenerarIndice.Click += new System.EventHandler(btnGenerarIndice_Click);
		this.btnCancelar.Location = new System.Drawing.Point(173, 559);
		this.btnCancelar.Name = "btnCancelar";
		this.btnCancelar.Size = new System.Drawing.Size(150, 40);
		this.btnCancelar.TabIndex = 11;
		this.btnCancelar.Text = "Cancelar";
		this.btnCancelar.UseVisualStyleBackColor = true;
		this.btnCancelar.Click += new System.EventHandler(btnCancelar_Click);
		this.btnCerrar.Location = new System.Drawing.Point(345, 559);
		this.btnCerrar.Name = "btnCerrar";
		this.btnCerrar.Size = new System.Drawing.Size(150, 40);
		this.btnCerrar.TabIndex = 12;
		this.btnCerrar.Text = "Cerrar";
		this.btnCerrar.UseVisualStyleBackColor = true;
		this.btnCerrar.Click += new System.EventHandler(btnCerrar_Click);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(20, 515);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(41, 13);
		this.label2.TabIndex = 13;
		this.label2.Text = "Origen:";
		this.cbxOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxOrigen.FormattingEnabled = true;
		this.cbxOrigen.Location = new System.Drawing.Point(173, 512);
		this.cbxOrigen.Name = "cbxOrigen";
		this.cbxOrigen.Size = new System.Drawing.Size(308, 21);
		this.cbxOrigen.TabIndex = 14;
		this.btnCargarLote.Location = new System.Drawing.Point(155, 19);
		this.btnCargarLote.Name = "btnCargarLote";
		this.btnCargarLote.Size = new System.Drawing.Size(70, 34);
		this.btnCargarLote.TabIndex = 2;
		this.btnCargarLote.Text = "Cargar Lote";
		this.btnCargarLote.UseVisualStyleBackColor = true;
		this.btnCargarLote.Click += new System.EventHandler(btnCargarLote_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1069, 611);
		base.Controls.Add(this.cbxOrigen);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.btnCerrar);
		base.Controls.Add(this.btnCancelar);
		base.Controls.Add(this.btnGenerarIndice);
		base.Controls.Add(this.groupBox6);
		base.Controls.Add(this.cbxUsuarioDigitalizacion);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.gboxDespachosNoEncontrados);
		base.Controls.Add(this.gboxDespachosMasDeUno);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.gboxArchivosEncontrados);
		base.Controls.Add(this.groupBox1);
		base.Name = "frmDespachosAutomatico";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Indexación de Despachos Auduaneros";
		base.Load += new System.EventHandler(frmDespachosAutomatico_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.gboxArchivosEncontrados.ResumeLayout(false);
		this.groupBox2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontradosFinal).EndInit();
		this.gboxDespachosMasDeUno.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosEncontrados).EndInit();
		this.gboxDespachosNoEncontrados.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dgvDespachosNoEncontrados).EndInit();
		this.groupBox6.ResumeLayout(false);
		this.groupBox6.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
