using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Capturador._02_Negocio;
using Capturador._04_Entidades;
using PdfiumViewer;

namespace Capturador._01_Pantallas._03_Indexacion;

public class frmHistoriasClinicas : Form
{
	private eUsuario oUsuarioLogueado = new eUsuario();

    eIDX_Historia_Clinica oHistoriaClinicaActual = new eIDX_Historia_Clinica();

    List<eIDX_Historia_Clinica> ListaHistoriasClinicas = new List<eIDX_Historia_Clinica>();


    private string rutaLote;

	private int totalArchivosEncontrados;

	private string rutaCarpetaInicial;

	private List<eDespacho> ListaFinalDespachosEncontrados = new List<eDespacho>();

	private List<eDespacho> ListaFinalDespachosEncontradosMultiples = new List<eDespacho>();

	private List<string> ListaFinalDespachosNoEncontrados = new List<string>();

	private eLote oLoteSeleccionado = new eLote();

	private IContainer components = null;

	private Button btnCerrar;

	private Label label3;

	private TextBox txtHistoriaClinica;

	private Label label4;

	private Label label5;

	private TextBox txtDNI;

	private Button btnProcesar;

	private Panel pnlDetallePlanos;

	private DataGridView dgvDetalleIndexacion;
	private PdfViewer pdfViewer1;
	private TextBox txtFeNacimiento;
    private Panel pnlLote;
    private Label label1;
    private Label lblRutaLote;
    private Label lblLote;
    private Label lblProyecto;
    private Label label7;
    private Label label6;
    private Label lblCantidadArchivos;
    private Panel panel1;
    private Label lblArchivo;
    private Panel panel2;
    private Label label10;
    private Label lblCantidadPendiente;
    private Label label11;
    private Label lblCantidadIndexado;
    private Label label8;
    private ProgressBar progressBar1;
    private TextBox txtNombreApellido;
    private Label label2;
    private Button btnGuardar;

	public frmHistoriasClinicas(eUsuario pUsuario, eLote pLote)
	{
		InitializeComponent();
		oUsuarioLogueado = pUsuario;
		oLoteSeleccionado = pLote;
	}

	private void frmHistoriasClinicas_Load(object sender, EventArgs e)
	{
		IniciarEventoEnter();
        cargarDatosLote();
        crearDataGridView();
        cargarProximoDocumentoIndexar();
        //crearDataGridView();
        //eProyectoConfiguracion oProyectoConfiguracion = nConfiguracion.ObtenerUltimaCarpeta(oUsuarioLogueado, 1);
        //rutaCarpetaInicial = oProyectoConfiguracion.dsRSrutaUltimaCarpeta;
        //ajustarFormularioInicial();
        //cargarProximaSecuencia();
        //actualizarBotones();
        //nuHistoriaClinica.Focus();
    }

	private void IniciarEventoEnter()
	{
		txtHistoriaClinica.KeyDown += txt_KeyDown;
		txtDNI.KeyDown += txt_KeyDown;
        txtNombreApellido.KeyDown += txt_KeyDown;
        txtFeNacimiento.KeyDown += txt_KeyDown;

		// Agregar eventos de foco al botón Guardar para indicar visualmente cuando tiene el foco
		btnGuardar.Enter += btnGuardar_Enter;
		btnGuardar.Leave += btnGuardar_Leave;

		// Agregar evento de resize al panel para mantener el label centrado
		panel1.Resize += (s, e) => CentrarLabelEnPanel(lblArchivo, panel1);
	}

	private void btnGuardar_Enter(object sender, EventArgs e)
	{
		// Cambiar color cuando recibe el foco para que sea visible
		btnGuardar.BackColor = Color.DarkSeaGreen;
	}

	private void btnGuardar_Leave(object sender, EventArgs e)
	{
		// Restaurar color original al perder el foco
		btnGuardar.BackColor = Color.SeaGreen;
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

    private void Mastxt_KeyDown(object sender, KeyEventArgs e)
    {
        MaskedTextBox txt = sender as MaskedTextBox;
        if (e.KeyCode == Keys.Return)
        {
            e.SuppressKeyPress = true;
        }
        if (e.KeyCode == Keys.Return)
        {
            SelectNextControl(txt, forward: true, tabStopOnly: true, nested: true, wrap: true);
        }
    }

    private void txtFeNacimiento_KeyPress(object sender, KeyPressEventArgs e)
    {
        // Solo permitir números
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        {
            e.Handled = true;
        }
    }

    private void txtFeNacimiento_TextChanged(object sender, EventArgs e)
    {
        // Auto-formatear mientras escribe: agregar "/" automáticamente
        if (txtFeNacimiento.Text.Length == 2 || txtFeNacimiento.Text.Length == 5)
        {
            if (!txtFeNacimiento.Text.EndsWith("/"))
            {
                txtFeNacimiento.Text += "/";
                txtFeNacimiento.SelectionStart = txtFeNacimiento.Text.Length;
            }
        }
    }


    private void cargarDatosLote()
    {
        lblProyecto.Text = oLoteSeleccionado.dsProyecto;
        //lblOperatoria.Text = oLoteSeleccionado. .dsOperatoria;
        lblLote.Text = oLoteSeleccionado.dsNombreLote;
        lblCantidadArchivos.Text = oLoteSeleccionado.nuCantidadArchivos.ToString();
        lblRutaLote.Text = oLoteSeleccionado.dsRutaLote;
    }


    private void cargarProximoDocumentoIndexar()
    {
        eIDX_Historia_Clinica oHistoriaClinica = nIndexacion.ObtenerProximoIndexar(oUsuarioLogueado, oLoteSeleccionado.cdProyecto, oLoteSeleccionado.cdLote);

        if (oHistoriaClinica.cdLoteDetalle != 0)
        {
            oHistoriaClinicaActual = oHistoriaClinica;
            string nombreArchivo = oHistoriaClinica.dsNombreArchivo;
            lblArchivo.Text = nombreArchivo;
            CentrarLabelEnPanel(lblArchivo, panel1);

            // Acá necesito que se seleccionese la fila del DataGridView correspondiente al archivo que se esta indexando, para que el usuario pueda ver a que archivo corresponde la historia clínica que se esta indexando
            foreach (DataGridViewRow row in dgvDetalleIndexacion.Rows)
            {
                row.Selected = false;
                if (row.Cells["dsNombreArchivo"].Value.ToString() == nombreArchivo)
                {
                    row.Selected = true;
                    dgvDetalleIndexacion.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }

            cargarImagen(nombreArchivo);
        }
        else
        {
            if (MessageBox.Show("El lote ya esta indexado. ¿Desea procesarlo?", "Lote Terminado", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                procesarLote();
            }
        }

    }

    private void procesarLote()
    {
        // Diccionario para rastrear renombrados (clave: nombreNuevo, valor: nombreOriginal)
        Dictionary<string, string> archivosRenombradosRollback = new Dictionary<string, string>();

        try
        {
            // 0. Cerrar el PDF abierto en el visor para liberar el archivo
            if (currentPdfDocument != null)
            {
                currentPdfDocument.Dispose();
                currentPdfDocument = null;
            }
            pdfViewer1.Document = null;

            // 1. Validar que todas las historias clínicas estén completas
            List<string> errores = new List<string>();

            for (int i = 0; i < ListaHistoriasClinicas.Count; i++)
            {
                var historia = ListaHistoriasClinicas[i];

                if (string.IsNullOrEmpty(historia.nuHistoriaClinica))
                {
                    errores.Add($"Archivo '{historia.dsNombreArchivo}': falta Historia Clínica");
                }

                if (string.IsNullOrEmpty(historia.nuDNI))
                {
                    errores.Add($"Archivo '{historia.dsNombreArchivo}': falta DNI");
                }

                if (string.IsNullOrEmpty(historia.dsNombreApellido))
                {
                    errores.Add($"Archivo '{historia.dsNombreArchivo}': falta Nombre y Apellido");
                }

                if (historia.feNacimiento == null || historia.feNacimiento == DateTime.MinValue)
                {
                    errores.Add($"Archivo '{historia.dsNombreArchivo}': falta Fecha de Nacimiento");
                }
            }

            // Si hay errores, mostrarlos y detener el procesamiento
            if (errores.Count > 0)
            {
                string mensaje = "No se puede procesar el lote. Faltan datos en los siguientes archivos:\n\n";
                mensaje += string.Join("\n", errores);
                MessageBox.Show(mensaje, "Validación de Datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Verificar que la carpeta del lote existe
            if (!System.IO.Directory.Exists(oLoteSeleccionado.dsRutaLote))
            {
                MessageBox.Show($"La carpeta del lote no existe:\n{oLoteSeleccionado.dsRutaLote}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Verificar que TODOS los archivos existen y están disponibles ANTES de renombrar
            List<string> archivosNoDisponibles = new List<string>();

            foreach (var historia in ListaHistoriasClinicas)
            {
                string archivoOriginal = System.IO.Path.Combine(oLoteSeleccionado.dsRutaLote, historia.dsNombreArchivo);

                if (!System.IO.File.Exists(archivoOriginal))
                {
                    archivosNoDisponibles.Add($"No existe: {historia.dsNombreArchivo}");
                    continue;
                }

                // Verificar si el archivo está en uso
                try
                {
                    using (System.IO.FileStream fs = System.IO.File.Open(archivoOriginal, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
                    {
                        // Si llega aquí, el archivo no está en uso
                    }
                }
                catch (System.IO.IOException)
                {
                    archivosNoDisponibles.Add($"Archivo en uso o bloqueado: {historia.dsNombreArchivo}");
                }
            }

            // Si hay archivos no disponibles, detener
            if (archivosNoDisponibles.Count > 0)
            {
                string mensaje = "No se puede procesar el lote. Los siguientes archivos no están disponibles:\n\n";
                mensaje += string.Join("\n", archivosNoDisponibles);
                MessageBox.Show(mensaje, "Archivos No Disponibles", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Renombrar los archivos (ahora sabemos que todos están disponibles)
            List<string> erroresRenombrado = new List<string>();

            foreach (var historia in ListaHistoriasClinicas)
            {
                try
                {
                    string archivoOriginal = System.IO.Path.Combine(oLoteSeleccionado.dsRutaLote, historia.dsNombreArchivo);

                    // Generar el nuevo nombre: [HistoriaClinica]-[DNI]-[FechaNacimiento(ddMMyyyy)].pdf
                    string fechaFormateada = historia.feNacimiento.ToString("ddMMyyyy");
                    string nuevoNombre = $"{historia.nuHistoriaClinica}-{historia.nuDNI}-{fechaFormateada}.pdf";
                    string archivoNuevo = System.IO.Path.Combine(oLoteSeleccionado.dsRutaLote, nuevoNombre);

                    // Verificar que no exista ya un archivo con ese nombre
                    if (System.IO.File.Exists(archivoNuevo) && archivoOriginal != archivoNuevo)
                    {
                        erroresRenombrado.Add($"Ya existe un archivo con el nombre: {nuevoNombre}");
                        continue;
                    }

                    // Renombrar el archivo
                    if (archivoOriginal != archivoNuevo)
                    {
                        System.IO.File.Move(archivoOriginal, archivoNuevo);

                        // Registrar para rollback: clave=rutaNueva, valor=rutaOriginal
                        archivosRenombradosRollback.Add(archivoNuevo, archivoOriginal);
                    }
                }
                catch (Exception ex)
                {
                    erroresRenombrado.Add($"Error al renombrar '{historia.dsNombreArchivo}': {ex.Message}");

                    // Si hay error, hacer rollback de TODOS los renombrados anteriores
                    HacerRollbackRenombrado(archivosRenombradosRollback);

                    string mensajeError = "Ocurrió un error durante el renombrado.\n\n" +
                        "Se han revertido todos los cambios realizados.\n\n" +
                        "Errores:\n" + string.Join("\n", erroresRenombrado);
                    MessageBox.Show(mensajeError, "Error - Cambios Revertidos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Mostrar errores de renombrado si los hay (sin haber intentado renombrar)
            if (erroresRenombrado.Count > 0)
            {
                HacerRollbackRenombrado(archivosRenombradosRollback);

                string mensaje = "Ocurrieron errores durante el proceso:\n\n";
                mensaje += string.Join("\n", erroresRenombrado) + "\n\n";
                mensaje += "Se han revertido todos los cambios.";
                MessageBox.Show(mensaje, "Errores - Cambios Revertidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5. Generar el archivo INDEX.csv
            try
            {
                string archivoIndex = System.IO.Path.Combine(oLoteSeleccionado.dsRutaLote, "INDEX.csv");

                // Usar codificación Default (ANSI) para compatibilidad con Excel
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(archivoIndex, false, System.Text.Encoding.Default))
                {
                    // Escribir cada línea del CSV
                    for (int i = 0; i < ListaHistoriasClinicas.Count; i++)
                    {
                        var historia = ListaHistoriasClinicas[i];
                        string fechaFormateada = historia.feNacimiento.ToString("dd/MM/yyyy");
                        string fechaArchivoFormateada = historia.feNacimiento.ToString("ddMMyyyy");
                        string nombreArchivo = $"{historia.nuHistoriaClinica}-{historia.nuDNI}-{fechaArchivoFormateada}.pdf";

                        // Formato: [dsNombreLote],[nuCantidadPaginaInicial],[NombreArchivo],[nuHistoriaClinica],[nuDNI],[feNacimiento]
                        string linea = $"{historia.dsNombreLote},{historia.nuCantidadPaginaInicial},{nombreArchivo},{historia.nuHistoriaClinica},{historia.nuDNI},{historia.dsNombreApellido},{fechaFormateada}";
                        writer.WriteLine(linea);
                    }
                }

                nIndexacion.finalizarIndexacionHistoriaClinica(oUsuarioLogueado, oLoteSeleccionado.cdProyecto, oLoteSeleccionado.cdLote);

                MessageBox.Show($"Lote procesado exitosamente.\n\n" +
                    $"Archivos renombrados: {archivosRenombradosRollback.Count}\n" +
                    $"Archivo INDEX.csv generado en:\n{archivoIndex}", 
                    "Proceso Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Indicar que el procesamiento fue exitoso y cerrar
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Si falla la generación del CSV, hacer rollback
                HacerRollbackRenombrado(archivosRenombradosRollback);

                MessageBox.Show($"Error al generar el archivo INDEX.csv:\n{ex.Message}\n\n" +
                    "Se han revertido todos los cambios en los archivos.", 
                    "Error - Cambios Revertidos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            // Rollback general si algo inesperado falla
            HacerRollbackRenombrado(archivosRenombradosRollback);

            MessageBox.Show($"Error durante el procesamiento del lote:\n{ex.Message}\n\n" +
                "Se han revertido todos los cambios en los archivos.", 
                "Error - Cambios Revertidos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void HacerRollbackRenombrado(Dictionary<string, string> archivosRenombrados)
    {
        if (archivosRenombrados.Count == 0)
            return;

        List<string> erroresRollback = new List<string>();

        // Revertir en orden inverso usando ToList().Reverse() para LINQ
        foreach (var par in archivosRenombrados.ToList().AsEnumerable().Reverse())
        {
            try
            {
                string archivoNuevo = par.Key;
                string archivoOriginal = par.Value;

                if (System.IO.File.Exists(archivoNuevo))
                {
                    System.IO.File.Move(archivoNuevo, archivoOriginal);
                }
            }
            catch (Exception ex)
            {
                erroresRollback.Add($"Error al revertir {System.IO.Path.GetFileName(par.Key)}: {ex.Message}");
            }
        }

        if (erroresRollback.Count > 0)
        {
            string mensaje = "ADVERTENCIA: Algunos archivos no pudieron ser revertidos:\n\n";
            mensaje += string.Join("\n", erroresRollback);
            MessageBox.Show(mensaje, "Errores en Rollback", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void CentrarLabelEnPanel(Label label, Panel panel)
    {
        // Calcular la posición X para centrar horizontalmente
        int centroX = (panel.Width - label.Width) / 2;

        // Calcular la posición Y para centrar verticalmente
        int centroY = (panel.Height - label.Height) / 2;

        // Asignar nueva ubicación
        label.Location = new Point(centroX, centroY);
    }
    










	private void crearDataGridView()
	{

		ListaHistoriasClinicas = nIndexacion.ObtenerListaHistoriasClinicas(oUsuarioLogueado, oLoteSeleccionado.cdProyecto, oLoteSeleccionado.cdLote);
		dgvDetalleIndexacion.DataSource = ListaHistoriasClinicas;

		// Ocultar todas las columnas primero
		foreach (DataGridViewColumn column in dgvDetalleIndexacion.Columns)
		{
			column.Visible = false;
		}

		// Mostrar y configurar solo las columnas deseadas
		if (dgvDetalleIndexacion.Columns.Contains("dsNombreArchivo"))
		{
			dgvDetalleIndexacion.Columns["dsNombreArchivo"].Visible = true;
			dgvDetalleIndexacion.Columns["dsNombreArchivo"].HeaderText = "Nombre Archivo";
			dgvDetalleIndexacion.Columns["dsNombreArchivo"].DisplayIndex = 0;
		}

		if (dgvDetalleIndexacion.Columns.Contains("nuHistoriaClinica"))
		{
			dgvDetalleIndexacion.Columns["nuHistoriaClinica"].Visible = true;
			dgvDetalleIndexacion.Columns["nuHistoriaClinica"].HeaderText = "Historia Clinica";
			dgvDetalleIndexacion.Columns["nuHistoriaClinica"].DisplayIndex = 1;
		}

		if (dgvDetalleIndexacion.Columns.Contains("nuDNI"))
		{
			dgvDetalleIndexacion.Columns["nuDNI"].Visible = true;
			dgvDetalleIndexacion.Columns["nuDNI"].HeaderText = "DNI";
			dgvDetalleIndexacion.Columns["nuDNI"].DisplayIndex = 2;
		}

        if (dgvDetalleIndexacion.Columns.Contains("dsNombreApellido"))
        {
            dgvDetalleIndexacion.Columns["dsNombreApellido"].Visible = true;
            dgvDetalleIndexacion.Columns["dsNombreApellido"].HeaderText = "Nombre y Apellido";
            dgvDetalleIndexacion.Columns["dsNombreApellido"].DisplayIndex = 3;
        }

        if (dgvDetalleIndexacion.Columns.Contains("feNacimiento"))
		{
			dgvDetalleIndexacion.Columns["feNacimiento"].Visible = true;
			dgvDetalleIndexacion.Columns["feNacimiento"].HeaderText = "Fecha de Nacimiento";
			dgvDetalleIndexacion.Columns["feNacimiento"].DisplayIndex = 4;
			dgvDetalleIndexacion.Columns["feNacimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
		}

		// Agregar evento para formatear celdas de fecha vacías
		dgvDetalleIndexacion.CellFormatting -= dgvDetalleIndexacion_CellFormatting; // Remover si existe
		dgvDetalleIndexacion.CellFormatting += dgvDetalleIndexacion_CellFormatting;

		actualizarDetalleIndexacion();
        actualizarCantidades();
    }

	private void dgvDetalleIndexacion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{
		// Solo aplicar a la columna de fecha
		if (dgvDetalleIndexacion.Columns[e.ColumnIndex].Name == "feNacimiento")
		{
			if (e.Value != null && e.Value != DBNull.Value)
			{
				if (e.Value is DateTime fecha)
				{
					// Si la fecha es 01/01/0001 o anterior a 1900, mostrar vacío
					if (fecha <= new DateTime(1900, 1, 1))
					{
						e.Value = string.Empty;
						e.FormattingApplied = true;
					}
				}
			}
		}
	}

	private void actualizarDetalleIndexacion()
	{
		Label labelTitulo = new Label();
		labelTitulo.Text = "Listado de Documentos a Indexar";
		labelTitulo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
		labelTitulo.BackColor = Color.FromArgb(26, 32, 40);
		labelTitulo.ForeColor = Color.White;
		labelTitulo.Dock = DockStyle.Top;
		labelTitulo.Height = 30;
		labelTitulo.TextAlign = ContentAlignment.MiddleCenter;
		int totalRegistros = dgvDetalleIndexacion.Rows.Count;
		Label labelTotal = new Label();
		labelTotal.Text = $"Total de Documentos: {totalRegistros}";
		labelTotal.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
		labelTotal.Dock = DockStyle.Bottom;
		labelTotal.Height = 25;
		labelTotal.TextAlign = ContentAlignment.MiddleCenter;
		labelTotal.BackColor = Color.FromArgb(26, 32, 40);
		labelTotal.ForeColor = Color.White;
		dgvDetalleIndexacion.Dock = DockStyle.Fill;
		pnlDetallePlanos.Controls.Clear();
		pnlDetallePlanos.Controls.Add(dgvDetalleIndexacion);
		pnlDetallePlanos.Controls.Add(labelTitulo);
		pnlDetallePlanos.Controls.Add(labelTotal);
	}


    private void actualizarCantidades() 
    {
        try
        {
            // Obtener el total de documentos
            int totalDocumentos = dgvDetalleIndexacion.Rows.Count;

            if (totalDocumentos == 0)
            {
                lblCantidadIndexado.Text = "0 (0%)";
                lblCantidadPendiente.Text = "0 (0%)";
                progressBar1.Value = 0;
                return;
            }

            // Contar indexados: registros que tienen nuHistoriaClinica con valor
            int cantidadIndexados = 0;
            int cantidadPendientes = 0;

            foreach (DataGridViewRow row in dgvDetalleIndexacion.Rows)
            {
                var valorHistoriaClinica = row.Cells["nuHistoriaClinica"].Value;

                if (valorHistoriaClinica != null && 
                    valorHistoriaClinica != DBNull.Value && 
                    !string.IsNullOrEmpty(valorHistoriaClinica.ToString()))
                {
                    cantidadIndexados++;
                }
                else
                {
                    cantidadPendientes++;
                }
            }

            // Calcular porcentajes
            double porcentajeIndexados = (cantidadIndexados * 100.0) / totalDocumentos;
            double porcentajePendientes = (cantidadPendientes * 100.0) / totalDocumentos;

            // Actualizar labels con formato: "cantidad (porcentaje%)"
            lblCantidadIndexado.Text = $"{cantidadIndexados} ({porcentajeIndexados:F0}%)";
            lblCantidadPendiente.Text = $"{cantidadPendientes} ({porcentajePendientes:F0}%)";

            // Actualizar ProgressBar (valor de 0 a 100)
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = (int)Math.Round(porcentajeIndexados);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al actualizar cantidades:\n{ex.Message}", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCerrar_Click_1(object sender, EventArgs e)
	{
		Close();
	}

	private void btnCancelar_Click_1(object sender, EventArgs e)
	{
		
	}

	private int obtenerProximoNuSecuencia()
	{
		return nIndexacion.obtenerProximaSecuencia(oUsuarioLogueado);
	}

	

	private void btnIngresarPlanos_Click(object sender, EventArgs e)
	{
		oHistoriaClinicaActual.nuHistoriaClinica = txtHistoriaClinica.Text;
		oHistoriaClinicaActual.nuDNI = txtDNI.Text;
		oHistoriaClinicaActual.dsNombreApellido = txtNombreApellido.Text;
		oHistoriaClinicaActual.feNacimiento = DateTime.ParseExact(txtFeNacimiento.Text, "dd/MM/yyyy", null);

		try
		{
			nIndexacion.agregarHistoriaClinica(oUsuarioLogueado, oLoteSeleccionado.cdProyecto, oLoteSeleccionado.cdLote, oHistoriaClinicaActual);

			// Limpiar campos
			txtHistoriaClinica.Clear();
			txtDNI.Clear();
			txtNombreApellido.Clear();
			txtFeNacimiento.Clear();

			crearDataGridView();

			cargarProximoDocumentoIndexar();

			txtHistoriaClinica.Focus();
		}
		catch (Exception)
		{

			throw;
		}

	}

	

	private void btnProcesar_Click(object sender, EventArgs e)
	{
        procesarLote();
	}

	private void procesar()
	{
		//if (int.TryParse(nuDNI.Text, out var desde) && int.TryParse(txtHasta.Text, out var hasta))
		//{
		//	if (desde <= hasta)
		//	{
		//		for (int i = desde; i <= hasta; i++)
		//		{
		//			agregarPlano(i);
		//		}
		//	}
		//	else
		//	{
		//		MessageBox.Show("El valor 'Desde' debe ser menor o igual que el valor 'Hasta'.");
		//	}
		//}
		//else
		//{
		//	MessageBox.Show("Por favor, ingresa números válidos en los campos.");
		//}
	}

	

	private void actualizarBotones()
	{
		if (dgvDetalleIndexacion.Rows.Count != 0)
		{
			btnGuardar.Enabled = true;
			btnGuardar.BackColor = Color.SeaGreen;
			
		}
		else
		{
			btnGuardar.Enabled = false;
			btnGuardar.BackColor = Color.DarkGray;
			
		}
	}

	private void dgvDetallePlanos_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.RowIndex >= 0)
		{
			try
			{
				// IMPORTANTE: Actualizar oHistoriaClinicaActual con el registro seleccionado del grid
				if (e.RowIndex < ListaHistoriasClinicas.Count)
				{
					oHistoriaClinicaActual = ListaHistoriasClinicas[e.RowIndex];
				}

				// Obtener el nombre del archivo (siempre debe existir)
				string nombreArchivo = dgvDetalleIndexacion.Rows[e.RowIndex].Cells["dsNombreArchivo"].Value?.ToString() ?? string.Empty;

				lblArchivo.Text = nombreArchivo;
				CentrarLabelEnPanel(lblArchivo, panel1);

				// Mapear Historia Clínica
				var valorHistoriaClinica = dgvDetalleIndexacion.Rows[e.RowIndex].Cells["nuHistoriaClinica"].Value;
				if (valorHistoriaClinica != null && !string.IsNullOrEmpty(valorHistoriaClinica.ToString()))
				{
					txtHistoriaClinica.Text = valorHistoriaClinica.ToString();
				}
				else
				{
					txtHistoriaClinica.Clear();
				}

				// Mapear DNI
				var valorDNI = dgvDetalleIndexacion.Rows[e.RowIndex].Cells["nuDNI"].Value;
				if (valorDNI != null && !string.IsNullOrEmpty(valorDNI.ToString()))
				{
					txtDNI.Text = valorDNI.ToString();
				}
				else
				{
					txtDNI.Clear();
				}

                // Mapear Nombre y Apellido
                var valorNombreApellido = dgvDetalleIndexacion.Rows[e.RowIndex].Cells["dsNombreApellido"].Value;
                if (valorNombreApellido != null && !string.IsNullOrEmpty(valorNombreApellido.ToString()))
                {
                    txtNombreApellido.Text = valorNombreApellido.ToString();
                }
                else
                {
                    txtNombreApellido.Clear();
                }

                // Mapear Fecha de Nacimiento con validación especial
                var valorFecha = dgvDetalleIndexacion.Rows[e.RowIndex].Cells["feNacimiento"].Value;
				if (valorFecha != null && valorFecha != DBNull.Value)
				{
					if (valorFecha is DateTime fecha)
					{
						// Validar que no sea fecha vacía (01/01/0001)
						if (fecha > new DateTime(1900, 1, 1))
						{
							txtFeNacimiento.Text = fecha.ToString("dd/MM/yyyy");
						}
						else
						{
							txtFeNacimiento.Clear();
						}
					}
					else if (!string.IsNullOrEmpty(valorFecha.ToString()))
					{
						// Intentar parsear si viene como string
						if (DateTime.TryParse(valorFecha.ToString(), out DateTime fechaParseada))
						{
							if (fechaParseada > new DateTime(1900, 1, 1))
							{
								txtFeNacimiento.Text = fechaParseada.ToString("dd/MM/yyyy");
							}
							else
							{
								txtFeNacimiento.Clear();
							}
						}
						else
						{
							txtFeNacimiento.Clear();
						}
					}
					else
					{
						txtFeNacimiento.Clear();
					}
				}
				else
				{
					txtFeNacimiento.Clear();
				}

				// Cargar la imagen del PDF
				if (!string.IsNullOrEmpty(nombreArchivo))
				{
					cargarImagen(nombreArchivo);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error al cargar los datos del registro:\n{ex.Message}", 
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}

    private PdfDocument currentPdfDocument = null;

    private void cargarImagen(string pArchivo)
    { 
        string rutaArchivo = System.IO.Path.Combine(oLoteSeleccionado.dsRutaLote, pArchivo);

        if (!System.IO.File.Exists(rutaArchivo))
        {
            MessageBox.Show($"El archivo no existe:\n{rutaArchivo}", "Archivo no encontrado", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            // Liberar el documento anterior si existe
            if (currentPdfDocument != null)
            {
                currentPdfDocument.Dispose();
                currentPdfDocument = null;
            }

            // Cargar el nuevo PDF
            currentPdfDocument = PdfDocument.Load(rutaArchivo);
            pdfViewer1.Document = currentPdfDocument;

            // Configurar el zoom inicial (FitWidth similar a Adobe)
            pdfViewer1.ZoomMode = PdfViewerZoomMode.FitWidth;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar el PDF:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			// Liberar el documento PDF
			if (currentPdfDocument != null)
			{
				currentPdfDocument.Dispose();
				currentPdfDocument = null;
			}

			if (components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtHistoriaClinica = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDNI = new System.Windows.Forms.TextBox();
            this.btnProcesar = new System.Windows.Forms.Button();
            this.pnlDetallePlanos = new System.Windows.Forms.Panel();
            this.dgvDetalleIndexacion = new System.Windows.Forms.DataGridView();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.pdfViewer1 = new PdfiumViewer.PdfViewer();
            this.txtFeNacimiento = new System.Windows.Forms.TextBox();
            this.pnlLote = new System.Windows.Forms.Panel();
            this.lblRutaLote = new System.Windows.Forms.Label();
            this.lblLote = new System.Windows.Forms.Label();
            this.lblProyecto = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCantidadArchivos = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblArchivo = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblCantidadPendiente = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblCantidadIndexado = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtNombreApellido = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlDetallePlanos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleIndexacion)).BeginInit();
            this.pnlLote.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCerrar.BackColor = System.Drawing.Color.Salmon;
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(326, 806);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(200, 25);
            this.btnCerrar.TabIndex = 12;
            this.btnCerrar.Text = "   Cerrar";
            this.btnCerrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(26, 335);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 16);
            this.label3.TabIndex = 68;
            this.label3.Text = "Nro. Historia Clinica:";
            // 
            // txtHistoriaClinica
            // 
            this.txtHistoriaClinica.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHistoriaClinica.Location = new System.Drawing.Point(180, 329);
            this.txtHistoriaClinica.Name = "txtHistoriaClinica";
            this.txtHistoriaClinica.Size = new System.Drawing.Size(346, 26);
            this.txtHistoriaClinica.TabIndex = 1;
            this.txtHistoriaClinica.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtHistoriaClinica.Leave += new System.EventHandler(this.nuHistoriaClinica_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(26, 372);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 16);
            this.label4.TabIndex = 70;
            this.label4.Text = "DNI Paciente:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(26, 446);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Fecha de Nacimiento:";
            // 
            // txtDNI
            // 
            this.txtDNI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDNI.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDNI.Location = new System.Drawing.Point(180, 366);
            this.txtDNI.Name = "txtDNI";
            this.txtDNI.Size = new System.Drawing.Size(346, 26);
            this.txtDNI.TabIndex = 2;
            this.txtDNI.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnProcesar
            // 
            this.btnProcesar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnProcesar.BackColor = System.Drawing.Color.SeaGreen;
            this.btnProcesar.FlatAppearance.BorderSize = 0;
            this.btnProcesar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcesar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcesar.ForeColor = System.Drawing.Color.White;
            this.btnProcesar.Location = new System.Drawing.Point(29, 806);
            this.btnProcesar.Name = "btnProcesar";
            this.btnProcesar.Size = new System.Drawing.Size(200, 25);
            this.btnProcesar.TabIndex = 10;
            this.btnProcesar.Text = "   Procesar";
            this.btnProcesar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnProcesar.UseVisualStyleBackColor = false;
            this.btnProcesar.Click += new System.EventHandler(this.btnProcesar_Click);
            // 
            // pnlDetallePlanos
            // 
            this.pnlDetallePlanos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlDetallePlanos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.pnlDetallePlanos.Controls.Add(this.dgvDetalleIndexacion);
            this.pnlDetallePlanos.Location = new System.Drawing.Point(26, 537);
            this.pnlDetallePlanos.Name = "pnlDetallePlanos";
            this.pnlDetallePlanos.Size = new System.Drawing.Size(500, 246);
            this.pnlDetallePlanos.TabIndex = 65;
            // 
            // dgvDetalleIndexacion
            // 
            this.dgvDetalleIndexacion.AllowUserToAddRows = false;
            this.dgvDetalleIndexacion.AllowUserToDeleteRows = false;
            this.dgvDetalleIndexacion.AllowUserToResizeRows = false;
            this.dgvDetalleIndexacion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetalleIndexacion.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.dgvDetalleIndexacion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalleIndexacion.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvDetalleIndexacion.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(96)))), ((int)(((byte)(130)))));
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(96)))), ((int)(((byte)(130)))));
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetalleIndexacion.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.dgvDetalleIndexacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDetalleIndexacion.DefaultCellStyle = dataGridViewCellStyle14;
            this.dgvDetalleIndexacion.EnableHeadersVisualStyles = false;
            this.dgvDetalleIndexacion.GridColor = System.Drawing.Color.SteelBlue;
            this.dgvDetalleIndexacion.Location = new System.Drawing.Point(22, 48);
            this.dgvDetalleIndexacion.Name = "dgvDetalleIndexacion";
            this.dgvDetalleIndexacion.ReadOnly = true;
            this.dgvDetalleIndexacion.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetalleIndexacion.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dgvDetalleIndexacion.RowHeadersVisible = false;
            this.dgvDetalleIndexacion.RowHeadersWidth = 15;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
            this.dgvDetalleIndexacion.RowsDefaultCellStyle = dataGridViewCellStyle16;
            this.dgvDetalleIndexacion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalleIndexacion.Size = new System.Drawing.Size(462, 100);
            this.dgvDetalleIndexacion.TabIndex = 18;
            this.dgvDetalleIndexacion.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetallePlanos_CellContentDoubleClick);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.SeaGreen;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(26, 484);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(500, 30);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "   Guardar";
            this.btnGuardar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnIngresarPlanos_Click);
            // 
            // pdfViewer1
            // 
            this.pdfViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pdfViewer1.Location = new System.Drawing.Point(568, 12);
            this.pdfViewer1.Name = "pdfViewer1";
            this.pdfViewer1.Size = new System.Drawing.Size(849, 830);
            this.pdfViewer1.TabIndex = 72;
            this.pdfViewer1.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitWidth;
            // 
            // txtFeNacimiento
            // 
            this.txtFeNacimiento.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFeNacimiento.Location = new System.Drawing.Point(180, 440);
            this.txtFeNacimiento.MaxLength = 10;
            this.txtFeNacimiento.Name = "txtFeNacimiento";
            this.txtFeNacimiento.Size = new System.Drawing.Size(146, 26);
            this.txtFeNacimiento.TabIndex = 4;
            this.txtFeNacimiento.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtFeNacimiento.TextChanged += new System.EventHandler(this.txtFeNacimiento_TextChanged);
            this.txtFeNacimiento.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFeNacimiento_KeyPress);
            this.txtFeNacimiento.Leave += new System.EventHandler(this.txtFeNacimiento_Leave);
            // 
            // pnlLote
            // 
            this.pnlLote.BackColor = System.Drawing.Color.Black;
            this.pnlLote.Controls.Add(this.lblRutaLote);
            this.pnlLote.Controls.Add(this.lblLote);
            this.pnlLote.Controls.Add(this.lblProyecto);
            this.pnlLote.Controls.Add(this.label7);
            this.pnlLote.Controls.Add(this.label6);
            this.pnlLote.Controls.Add(this.label1);
            this.pnlLote.Location = new System.Drawing.Point(12, 12);
            this.pnlLote.Name = "pnlLote";
            this.pnlLote.Size = new System.Drawing.Size(531, 108);
            this.pnlLote.TabIndex = 66;
            // 
            // lblRutaLote
            // 
            this.lblRutaLote.AutoSize = true;
            this.lblRutaLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRutaLote.ForeColor = System.Drawing.Color.White;
            this.lblRutaLote.Location = new System.Drawing.Point(193, 69);
            this.lblRutaLote.Name = "lblRutaLote";
            this.lblRutaLote.Size = new System.Drawing.Size(75, 16);
            this.lblRutaLote.TabIndex = 81;
            this.lblRutaLote.Text = "lblRutaLote";
            // 
            // lblLote
            // 
            this.lblLote.AutoSize = true;
            this.lblLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLote.ForeColor = System.Drawing.Color.White;
            this.lblLote.Location = new System.Drawing.Point(193, 38);
            this.lblLote.Name = "lblLote";
            this.lblLote.Size = new System.Drawing.Size(47, 16);
            this.lblLote.TabIndex = 80;
            this.lblLote.Text = "lblLote";
            // 
            // lblProyecto
            // 
            this.lblProyecto.AutoSize = true;
            this.lblProyecto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProyecto.ForeColor = System.Drawing.Color.White;
            this.lblProyecto.Location = new System.Drawing.Point(193, 10);
            this.lblProyecto.Name = "lblProyecto";
            this.lblProyecto.Size = new System.Drawing.Size(75, 16);
            this.lblProyecto.TabIndex = 78;
            this.lblProyecto.Text = "lblProyecto";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(13, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 16);
            this.label7.TabIndex = 77;
            this.label7.Text = "Ruta Lote:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(13, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 16);
            this.label6.TabIndex = 76;
            this.label6.Text = "Lote:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 16);
            this.label1.TabIndex = 74;
            this.label1.Text = "Proyecto:";
            // 
            // lblCantidadArchivos
            // 
            this.lblCantidadArchivos.AutoSize = true;
            this.lblCantidadArchivos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantidadArchivos.ForeColor = System.Drawing.Color.White;
            this.lblCantidadArchivos.Location = new System.Drawing.Point(226, 12);
            this.lblCantidadArchivos.Name = "lblCantidadArchivos";
            this.lblCantidadArchivos.Size = new System.Drawing.Size(127, 16);
            this.lblCantidadArchivos.TabIndex = 83;
            this.lblCantidadArchivos.Text = "lblCantidadArchivos";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.panel1.Controls.Add(this.lblArchivo);
            this.panel1.Location = new System.Drawing.Point(12, 255);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(531, 47);
            this.panel1.TabIndex = 84;
            // 
            // lblArchivo
            // 
            this.lblArchivo.AutoSize = true;
            this.lblArchivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArchivo.ForeColor = System.Drawing.Color.White;
            this.lblArchivo.Location = new System.Drawing.Point(129, 12);
            this.lblArchivo.Name = "lblArchivo";
            this.lblArchivo.Size = new System.Drawing.Size(284, 24);
            this.lblArchivo.TabIndex = 78;
            this.lblArchivo.Text = "No hay archivo Seleccionado";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.lblCantidadPendiente);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.lblCantidadIndexado);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.lblCantidadArchivos);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Location = new System.Drawing.Point(12, 126);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(531, 123);
            this.panel2.TabIndex = 85;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(17, 92);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(500, 23);
            this.progressBar1.TabIndex = 88;
            // 
            // lblCantidadPendiente
            // 
            this.lblCantidadPendiente.AutoSize = true;
            this.lblCantidadPendiente.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantidadPendiente.ForeColor = System.Drawing.Color.White;
            this.lblCantidadPendiente.Location = new System.Drawing.Point(226, 59);
            this.lblCantidadPendiente.Name = "lblCantidadPendiente";
            this.lblCantidadPendiente.Size = new System.Drawing.Size(136, 16);
            this.lblCantidadPendiente.TabIndex = 87;
            this.lblCantidadPendiente.Text = "lblCantidadPendiente";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(14, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(121, 16);
            this.label11.TabIndex = 86;
            this.label11.Text = "Total Pendiente:";
            // 
            // lblCantidadIndexado
            // 
            this.lblCantidadIndexado.AutoSize = true;
            this.lblCantidadIndexado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantidadIndexado.ForeColor = System.Drawing.Color.White;
            this.lblCantidadIndexado.Location = new System.Drawing.Point(226, 37);
            this.lblCantidadIndexado.Name = "lblCantidadIndexado";
            this.lblCantidadIndexado.Size = new System.Drawing.Size(131, 16);
            this.lblCantidadIndexado.TabIndex = 85;
            this.lblCantidadIndexado.Text = "lblCantidadIndexado";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(14, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 16);
            this.label8.TabIndex = 84;
            this.label8.Text = "Total Indexado:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(14, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(159, 16);
            this.label10.TabIndex = 82;
            this.label10.Text = "Total de Documentos:";
            // 
            // txtNombreApellido
            // 
            this.txtNombreApellido.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreApellido.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreApellido.Location = new System.Drawing.Point(180, 403);
            this.txtNombreApellido.Name = "txtNombreApellido";
            this.txtNombreApellido.Size = new System.Drawing.Size(346, 26);
            this.txtNombreApellido.TabIndex = 3;
            this.txtNombreApellido.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(26, 409);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 16);
            this.label2.TabIndex = 87;
            this.label2.Text = "Nombre y Apellido:";
            // 
            // frmHistoriasClinicas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(1429, 861);
            this.Controls.Add(this.txtNombreApellido);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlLote);
            this.Controls.Add(this.txtFeNacimiento);
            this.Controls.Add(this.pdfViewer1);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.pnlDetallePlanos);
            this.Controls.Add(this.btnProcesar);
            this.Controls.Add(this.txtDNI);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtHistoriaClinica);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCerrar);
            this.MinimizeBox = false;
            this.Name = "frmHistoriasClinicas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ingreso de Historias Clinicas";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmHistoriasClinicas_Load);
            this.pnlDetallePlanos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleIndexacion)).EndInit();
            this.pnlLote.ResumeLayout(false);
            this.pnlLote.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

    private void nuHistoriaClinica_Leave(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtHistoriaClinica.Text))
        {
            // Guardar el color original del campo
            Color colorOriginal = txtHistoriaClinica.BackColor;
            Color colorTextoOriginal = txtHistoriaClinica.ForeColor;

            try
            {
                // Ocultar el valor del campo poniendo fondo negro y texto negro
                txtHistoriaClinica.BackColor = Color.Black;
                txtHistoriaClinica.ForeColor = Color.Black;

                // Crear y mostrar el formulario de validación
                using (frmValidarHistoriaClinica frmValidar = new frmValidarHistoriaClinica())
                {
                    frmValidar.ValorOriginal = txtHistoriaClinica.Text;
                    DialogResult resultado = frmValidar.ShowDialog(this);

                    // Restaurar el color original del campo
                    txtHistoriaClinica.BackColor = colorOriginal;
                    txtHistoriaClinica.ForeColor = colorTextoOriginal;

                    if (resultado == DialogResult.OK && frmValidar.ValidacionExitosa)
                    {
                        // Validación exitosa - pasar al siguiente campo (txtDNI)
                        txtDNI.Focus();
                    }
                    else
                    {
                        // Validación fallida - limpiar el campo y volver a poner el foco
                        txtHistoriaClinica.Clear();
                        txtHistoriaClinica.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de error, restaurar el color original
                txtHistoriaClinica.BackColor = colorOriginal;
                txtHistoriaClinica.ForeColor = colorTextoOriginal;
                MessageBox.Show($"Error en la validación: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void txtFeNacimiento_Leave(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtFeNacimiento.Text))
        {
            // Remover espacios y barras para contar solo dígitos
            string soloDigitos = txtFeNacimiento.Text.Replace("/", "").Replace(" ", "").Trim();

            // Verificar que tenga exactamente 8 dígitos
            if (soloDigitos.Length != 8)
            {
                MessageBox.Show("Debe ingresar una fecha completa (8 dígitos).\nFormato: dd/MM/yyyy o ddMMyyyy", 
                    "Fecha incompleta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFeNacimiento.Clear();
                txtFeNacimiento.Focus();
                return;
            }

            // Intentar parsear la fecha
            DateTime fechaNacimiento;
            bool fechaValida = false;

            // Intentar primero con formato dd/MM/yyyy
            if (DateTime.TryParseExact(txtFeNacimiento.Text, "dd/MM/yyyy", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, 
                out fechaNacimiento))
            {
                fechaValida = true;
            }
            // Si falla, intentar con formato sin barras: ddMMyyyy
            else if (DateTime.TryParseExact(soloDigitos, "ddMMyyyy", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, 
                out fechaNacimiento))
            {
                fechaValida = true;
                // Formatear correctamente con barras
                txtFeNacimiento.Text = fechaNacimiento.ToString("dd/MM/yyyy");
            }

            if (!fechaValida)
            {
                MessageBox.Show("La fecha ingresada no es válida.\nVerifique el día, mes y año.", 
                    "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFeNacimiento.Clear();
                txtFeNacimiento.Focus();
                return;
            }

            // Validar que la fecha no sea futura
            if (fechaNacimiento.Date > DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser una fecha futura", 
                    "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFeNacimiento.Clear();
                txtFeNacimiento.Focus();
                return;
            }

            // Validar que la fecha no sea muy antigua (no más de 150 años atrás)
            DateTime fechaMinima = DateTime.Today.AddYears(-150);
            if (fechaNacimiento.Date < fechaMinima)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser anterior a " + fechaMinima.ToString("dd/MM/yyyy"), 
                    "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFeNacimiento.Clear();
                txtFeNacimiento.Focus();
                return;
            }

            // Si llegó hasta aquí, la fecha es válida - continuar al siguiente control
            btnGuardar.Focus();
        }
    }

    private void txtFeNacimiento_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
    {

    }
}
