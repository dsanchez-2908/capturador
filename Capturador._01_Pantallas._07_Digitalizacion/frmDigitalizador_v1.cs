using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Capturador._04_Entidades;
using NTwain;
using NTwain.Data;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using WIA;

namespace Capturador._01_Pantallas._07_Digitalizacion;

public class frmDigitalizador_v1 : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

	private TwainSession _twain;

	private DataSource _selectedSource;

	private IContainer components = null;

	private ComboBox cbxEscanerDisponibles;

	private Label label11;

	private ComboBox cbxResolucion;

	private Label label1;

	private Button btnSeleccionarCarpetaRepositorio;

	private TextBox txtCarpetaRepositorio;

	private Panel pnlVisorImagenes;

	private Label lblNombreLote;

	private MenuStrip menuStrip1;

	private ToolStripMenuItem loteToolStripMenuItem;

	private ToolStripMenuItem nuevoToolStripMenuItem;

	private ToolStripMenuItem cargarToolStripMenuItem;

	private ToolStripMenuItem procesarToolStripMenuItem;

	private ToolStripMenuItem configuraciónToolStripMenuItem;

	public TextBox txtNombreLote;

	private ComboBox cbxTipo;

	private Label label2;

	private ComboBox cbxPaginasMostrar;

	private Label label3;

	private Button btnIniciar;

	private Button btnDetener;

	public frmDigitalizador_v1(eUsuario pUsuarioLogueado)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuarioLogueado;
	}

	private void frmDigitalizador_v1_Load(object sender, EventArgs e)
	{
		cargarListas();
	}

	private void cargarListas()
	{
		cbxResolucion.Items.Clear();
		cbxResolucion.Items.Add("100");
		cbxResolucion.Items.Add("200");
		cbxResolucion.Items.Add("300");
		cbxResolucion.SelectedIndex = 2;
		cbxTipo.Items.Clear();
		cbxTipo.Items.Add("Escala de Grises");
		cbxTipo.Items.Add("Blanco y Negro");
		cbxTipo.Items.Add("Color");
		cbxTipo.SelectedIndex = 0;
		cbxPaginasMostrar.Items.Clear();
		cbxPaginasMostrar.Items.Add("1");
		cbxPaginasMostrar.Items.Add("2");
		cbxPaginasMostrar.Items.Add("4");
		cbxPaginasMostrar.Items.Add("6");
		cbxPaginasMostrar.Items.Add("8");
		cbxPaginasMostrar.Items.Add("10");
		cbxPaginasMostrar.SelectedIndex = 4;
	}

	private void cargarEscanerDisponibles()
	{
		try
		{
			TWIdentity appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetEntryAssembly());
			_twain = new TwainSession(appId);
			_twain.TransferError += delegate(object s, TransferErrorEventArgs e)
			{
				MessageBox.Show("Error en transferencia TWAIN: " + e.Exception?.Message);
			};
			_twain.StateChanged += delegate
			{
				if (_twain.State == 3)
				{
					Invoke((MethodInvoker)delegate
					{
						cbxEscanerDisponibles.Items.Clear();
						foreach (DataSource current in _twain)
						{
							cbxEscanerDisponibles.Items.Add(current.Name);
						}
						if (cbxEscanerDisponibles.Items.Count > 0)
						{
							cbxEscanerDisponibles.SelectedIndex = 0;
						}
						else
						{
							MessageBox.Show("No se encontraron escáneres compatibles con TWAIN.");
						}
					});
				}
			};
			if (_twain.State == 2)
			{
				_twain.Open();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error al abrir TWAIN: " + ex.Message);
		}
	}

	private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
	{
		abrirCrearLote();
	}

	private void abrirCrearLote()
	{
		frmCrearLote oFrmCrearLote = new frmCrearLote(oUsuarioLogueado, txtCarpetaRepositorio.Text);
		AddOwnedForm(oFrmCrearLote);
		oFrmCrearLote.Show();
	}

	private void btnSeleccionarCarpetaRepositorio_Click(object sender, EventArgs e)
	{
		using FolderBrowserDialog fbd = new FolderBrowserDialog();
		fbd.Description = "Seleccionar la carpeta a procesar";
		fbd.ShowNewFolderButton = false;
		if (fbd.ShowDialog() == DialogResult.OK)
		{
			txtCarpetaRepositorio.Text = fbd.SelectedPath;
		}
	}

	private void btnIniciar_Click(object sender, EventArgs e)
	{
		try
		{
			WIA.CommonDialog dialog = new WIA.CommonDialog();
			Device scanner = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, AlwaysSelectDevice: true);
			if (scanner == null)
			{
				MessageBox.Show("No se seleccionó ningún escáner.");
				return;
			}
			Item item = scanner.Items[1];
			int colorValue = 1;
			if (cbxTipo.SelectedItem.ToString() == "Escala de Grises")
			{
				colorValue = 2;
			}
			else if (cbxTipo.SelectedItem.ToString() == "Blanco y Negro")
			{
				colorValue = 4;
			}
			int dpi = int.Parse(cbxResolucion.SelectedItem.ToString());
			int anchoPixeles = (int)(8.27 * (double)dpi);
			int altoPixeles = (int)(11.69 * (double)dpi);
			string rutaCarpeta = Path.Combine(txtCarpetaRepositorio.Text, txtNombreLote.Text);
			Directory.CreateDirectory(rutaCarpeta);
			int contador = ObtenerUltimoNumeroArchivo(rutaCarpeta);
			SetWIAProperty(item.Properties, "6146", colorValue);
			SetWIAProperty(item.Properties, "6147", dpi);
			SetWIAProperty(item.Properties, "6148", dpi);
			SetWIAProperty(item.Properties, "6151", anchoPixeles);
			SetWIAProperty(item.Properties, "6152", altoPixeles);
			object result = item.Transfer("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}");
			if (result is IEnumerable imageFiles)
			{
				int imagenesGuardadas = 0;
				foreach (object obj in imageFiles)
				{
					if (obj is ImageFile imgFile)
					{
						contador++;
						string tempImage = Path.Combine(rutaCarpeta, "temp_" + contador + ".jpg");
						if (File.Exists(tempImage))
						{
							File.Delete(tempImage);
						}
						imgFile.SaveFile(tempImage);
						string nombrePDF = contador.ToString("D10") + ".pdf";
						string rutaPDF = Path.Combine(rutaCarpeta, nombrePDF);
						using (Bitmap bmp = new Bitmap(tempImage))
						{
							GuardarImagenComoPDF(bmp, rutaPDF);
						}
						File.Delete(tempImage);
						imagenesGuardadas++;
					}
				}
				MessageBox.Show("Se guardaron " + imagenesGuardadas + " imágenes como PDF.");
			}
			else
			{
				contador++;
				string tempImage2 = Path.Combine(rutaCarpeta, "temp_" + contador + ".jpg");
				if (File.Exists(tempImage2))
				{
					File.Delete(tempImage2);
				}
				((ImageFile)result).SaveFile(tempImage2);
				string nombrePDF2 = contador.ToString("D10") + ".pdf";
				string rutaPDF2 = Path.Combine(rutaCarpeta, nombrePDF2);
				using (Bitmap bmp2 = new Bitmap(tempImage2))
				{
					GuardarImagenComoPDF(bmp2, rutaPDF2);
				}
				File.Delete(tempImage2);
				MessageBox.Show("Se guardó 1 imagen como PDF.");
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error al escanear: " + ex.Message);
		}
	}

	private void GuardarImagenComoPDF(Image imagen, string rutaPDF)
	{
		using PdfDocument document = new PdfDocument();
		PdfPage page = document.AddPage();
		using (XGraphics gfx = XGraphics.FromPdfPage(page))
		{
			using MemoryStream ms = new MemoryStream();
			imagen.Save(ms, ImageFormat.Jpeg);
			ms.Position = 0L;
			using XImage img = XImage.FromStream(ms);
			page.Width = (double)(img.PixelWidth * 72) / img.HorizontalResolution;
			page.Height = (double)(img.PixelHeight * 72) / img.VerticalResolution;
			gfx.DrawImage(img, 0.0, 0.0, page.Width, page.Height);
		}
		document.Save(rutaPDF);
	}

	private void SetWIAProperty(IProperties properties, object propName, object propValue)
	{
		Property prop = properties.get_Item(ref propName);
		prop.set_Value(ref propValue);
	}

	private int ObtenerUltimoNumeroArchivo(string rutaCarpeta)
	{
		if (!Directory.Exists(rutaCarpeta))
		{
			return 0;
		}
		string[] archivos = Directory.GetFiles(rutaCarpeta, "*.pdf");
		int maxNumero = 0;
		string[] array = archivos;
		foreach (string archivo in array)
		{
			string nombre = Path.GetFileNameWithoutExtension(archivo);
			if (int.TryParse(nombre, out var numero) && numero > maxNumero)
			{
				maxNumero = numero;
			}
		}
		return maxNumero;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capturador._01_Pantallas._07_Digitalizacion.frmDigitalizador_v1));
		this.cbxEscanerDisponibles = new System.Windows.Forms.ComboBox();
		this.label11 = new System.Windows.Forms.Label();
		this.cbxResolucion = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.btnSeleccionarCarpetaRepositorio = new System.Windows.Forms.Button();
		this.txtCarpetaRepositorio = new System.Windows.Forms.TextBox();
		this.pnlVisorImagenes = new System.Windows.Forms.Panel();
		this.txtNombreLote = new System.Windows.Forms.TextBox();
		this.lblNombreLote = new System.Windows.Forms.Label();
		this.menuStrip1 = new System.Windows.Forms.MenuStrip();
		this.loteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.nuevoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cargarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.procesarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.configuraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cbxTipo = new System.Windows.Forms.ComboBox();
		this.label2 = new System.Windows.Forms.Label();
		this.cbxPaginasMostrar = new System.Windows.Forms.ComboBox();
		this.label3 = new System.Windows.Forms.Label();
		this.btnIniciar = new System.Windows.Forms.Button();
		this.btnDetener = new System.Windows.Forms.Button();
		this.menuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.cbxEscanerDisponibles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxEscanerDisponibles.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxEscanerDisponibles.FormattingEnabled = true;
		this.cbxEscanerDisponibles.Location = new System.Drawing.Point(200, 83);
		this.cbxEscanerDisponibles.Name = "cbxEscanerDisponibles";
		this.cbxEscanerDisponibles.Size = new System.Drawing.Size(230, 25);
		this.cbxEscanerDisponibles.TabIndex = 28;
		this.label11.AutoSize = true;
		this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label11.ForeColor = System.Drawing.Color.White;
		this.label11.Location = new System.Drawing.Point(28, 86);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(136, 16);
		this.label11.TabIndex = 29;
		this.label11.Text = "Seleccionar Escáner:";
		this.cbxResolucion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxResolucion.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxResolucion.FormattingEnabled = true;
		this.cbxResolucion.Location = new System.Drawing.Point(200, 163);
		this.cbxResolucion.Name = "cbxResolucion";
		this.cbxResolucion.Size = new System.Drawing.Size(230, 25);
		this.cbxResolucion.TabIndex = 30;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.White;
		this.label1.Location = new System.Drawing.Point(28, 166);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(79, 16);
		this.label1.TabIndex = 31;
		this.label1.Text = "Resolución:";
		this.btnSeleccionarCarpetaRepositorio.BackColor = System.Drawing.Color.SeaGreen;
		this.btnSeleccionarCarpetaRepositorio.FlatAppearance.BorderSize = 0;
		this.btnSeleccionarCarpetaRepositorio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSeleccionarCarpetaRepositorio.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnSeleccionarCarpetaRepositorio.ForeColor = System.Drawing.Color.White;
		this.btnSeleccionarCarpetaRepositorio.Image = (System.Drawing.Image)resources.GetObject("btnSeleccionarCarpetaRepositorio.Image");
		this.btnSeleccionarCarpetaRepositorio.Location = new System.Drawing.Point(487, 46);
		this.btnSeleccionarCarpetaRepositorio.Name = "btnSeleccionarCarpetaRepositorio";
		this.btnSeleccionarCarpetaRepositorio.Size = new System.Drawing.Size(239, 25);
		this.btnSeleccionarCarpetaRepositorio.TabIndex = 62;
		this.btnSeleccionarCarpetaRepositorio.Text = "   Seleccionar Carpeta";
		this.btnSeleccionarCarpetaRepositorio.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnSeleccionarCarpetaRepositorio.UseVisualStyleBackColor = false;
		this.btnSeleccionarCarpetaRepositorio.Click += new System.EventHandler(btnSeleccionarCarpetaRepositorio_Click);
		this.txtCarpetaRepositorio.Enabled = false;
		this.txtCarpetaRepositorio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtCarpetaRepositorio.Location = new System.Drawing.Point(753, 47);
		this.txtCarpetaRepositorio.Multiline = true;
		this.txtCarpetaRepositorio.Name = "txtCarpetaRepositorio";
		this.txtCarpetaRepositorio.Size = new System.Drawing.Size(229, 61);
		this.txtCarpetaRepositorio.TabIndex = 63;
		this.txtCarpetaRepositorio.Text = "C:\\Repositorio";
		this.pnlVisorImagenes.BackColor = System.Drawing.Color.FromArgb(26, 32, 40);
		this.pnlVisorImagenes.Location = new System.Drawing.Point(200, 232);
		this.pnlVisorImagenes.Name = "pnlVisorImagenes";
		this.pnlVisorImagenes.Size = new System.Drawing.Size(972, 317);
		this.pnlVisorImagenes.TabIndex = 65;
		this.pnlVisorImagenes.Visible = false;
		this.txtNombreLote.Enabled = false;
		this.txtNombreLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNombreLote.Location = new System.Drawing.Point(201, 41);
		this.txtNombreLote.Name = "txtNombreLote";
		this.txtNombreLote.Size = new System.Drawing.Size(229, 22);
		this.txtNombreLote.TabIndex = 67;
		this.lblNombreLote.AutoSize = true;
		this.lblNombreLote.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNombreLote.ForeColor = System.Drawing.Color.White;
		this.lblNombreLote.Location = new System.Drawing.Point(28, 46);
		this.lblNombreLote.Name = "lblNombreLote";
		this.lblNombreLote.Size = new System.Drawing.Size(121, 17);
		this.lblNombreLote.TabIndex = 66;
		this.lblNombreLote.Text = "Nombre del Lote:";
		this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.loteToolStripMenuItem, this.configuraciónToolStripMenuItem });
		this.menuStrip1.Location = new System.Drawing.Point(0, 0);
		this.menuStrip1.Name = "menuStrip1";
		this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
		this.menuStrip1.TabIndex = 66;
		this.menuStrip1.Text = "menuStrip1";
		this.loteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.nuevoToolStripMenuItem, this.cargarToolStripMenuItem, this.procesarToolStripMenuItem });
		this.loteToolStripMenuItem.Name = "loteToolStripMenuItem";
		this.loteToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
		this.loteToolStripMenuItem.Text = "Lote";
		this.nuevoToolStripMenuItem.Name = "nuevoToolStripMenuItem";
		this.nuevoToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
		this.nuevoToolStripMenuItem.Text = "Nuevo";
		this.nuevoToolStripMenuItem.Click += new System.EventHandler(nuevoToolStripMenuItem_Click);
		this.cargarToolStripMenuItem.Name = "cargarToolStripMenuItem";
		this.cargarToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
		this.cargarToolStripMenuItem.Text = "Cargar";
		this.procesarToolStripMenuItem.Name = "procesarToolStripMenuItem";
		this.procesarToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
		this.procesarToolStripMenuItem.Text = "Procesar";
		this.configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
		this.configuraciónToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
		this.configuraciónToolStripMenuItem.Text = "Configuración";
		this.cbxTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxTipo.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxTipo.FormattingEnabled = true;
		this.cbxTipo.Location = new System.Drawing.Point(200, 121);
		this.cbxTipo.Name = "cbxTipo";
		this.cbxTipo.Size = new System.Drawing.Size(230, 25);
		this.cbxTipo.TabIndex = 68;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.ForeColor = System.Drawing.Color.White;
		this.label2.Location = new System.Drawing.Point(28, 124);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(96, 16);
		this.label2.TabIndex = 69;
		this.label2.Text = "Tipo Escaneo:";
		this.cbxPaginasMostrar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbxPaginasMostrar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cbxPaginasMostrar.FormattingEnabled = true;
		this.cbxPaginasMostrar.Location = new System.Drawing.Point(656, 121);
		this.cbxPaginasMostrar.Name = "cbxPaginasMostrar";
		this.cbxPaginasMostrar.Size = new System.Drawing.Size(230, 25);
		this.cbxPaginasMostrar.TabIndex = 70;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.White;
		this.label3.Location = new System.Drawing.Point(484, 124);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(120, 16);
		this.label3.TabIndex = 71;
		this.label3.Text = "Páginas a Mostrar:";
		this.btnIniciar.BackColor = System.Drawing.Color.SeaGreen;
		this.btnIniciar.FlatAppearance.BorderSize = 0;
		this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnIniciar.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnIniciar.ForeColor = System.Drawing.Color.White;
		this.btnIniciar.Image = (System.Drawing.Image)resources.GetObject("btnIniciar.Image");
		this.btnIniciar.Location = new System.Drawing.Point(473, 185);
		this.btnIniciar.Name = "btnIniciar";
		this.btnIniciar.Size = new System.Drawing.Size(150, 25);
		this.btnIniciar.TabIndex = 72;
		this.btnIniciar.Text = "   Iniciar";
		this.btnIniciar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnIniciar.UseVisualStyleBackColor = false;
		this.btnIniciar.Click += new System.EventHandler(btnIniciar_Click);
		this.btnDetener.BackColor = System.Drawing.Color.SeaGreen;
		this.btnDetener.FlatAppearance.BorderSize = 0;
		this.btnDetener.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnDetener.Font = new System.Drawing.Font("Century Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnDetener.ForeColor = System.Drawing.Color.White;
		this.btnDetener.Image = (System.Drawing.Image)resources.GetObject("btnDetener.Image");
		this.btnDetener.Location = new System.Drawing.Point(629, 185);
		this.btnDetener.Name = "btnDetener";
		this.btnDetener.Size = new System.Drawing.Size(150, 25);
		this.btnDetener.TabIndex = 73;
		this.btnDetener.Text = "   Detener";
		this.btnDetener.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.btnDetener.UseVisualStyleBackColor = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(49, 66, 82);
		base.ClientSize = new System.Drawing.Size(1184, 561);
		base.Controls.Add(this.btnDetener);
		base.Controls.Add(this.btnIniciar);
		base.Controls.Add(this.cbxPaginasMostrar);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.cbxTipo);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.txtNombreLote);
		base.Controls.Add(this.pnlVisorImagenes);
		base.Controls.Add(this.lblNombreLote);
		base.Controls.Add(this.txtCarpetaRepositorio);
		base.Controls.Add(this.btnSeleccionarCarpetaRepositorio);
		base.Controls.Add(this.cbxResolucion);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.cbxEscanerDisponibles);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.menuStrip1);
		base.MainMenuStrip = this.menuStrip1;
		base.Name = "frmDigitalizador_v1";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Digitalizador v1";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.Load += new System.EventHandler(frmDigitalizador_v1_Load);
		this.menuStrip1.ResumeLayout(false);
		this.menuStrip1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
