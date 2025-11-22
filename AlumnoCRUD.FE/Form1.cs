using AlumnoCRUD.FE.Models;
using AlumnoCRUD.FE.Services;
using System;
using System.Collections.Generic; // Necesario para List<>
using System.Drawing; // A veces necesario para UI
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlumnoCRUD.FE
{
    public partial class Form1 : Form
    {
        private readonly AlumnoService _service;
        private int _idSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();

            // ---------------------------------------------------------
            // 1. CONFIGURACIÓN DE CONEXIÓN
            // ---------------------------------------------------------
            var handler = new System.Net.Http.HttpClientHandler();
            // Ignorar errores de certificado SSL en desarrollo
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            var httpClient = new System.Net.Http.HttpClient(handler)
            {
                // TU URL CONFIRMADA (NO LE QUITES LA BARRA AL FINAL)
                BaseAddress = new Uri("http://localhost:5261/")
            };

            _service = new AlumnoService(httpClient);
        }

        // Evento que se ejecuta al abrir la ventana
        private async void Form1_Load(object sender, EventArgs e)
        {
            ConfigurarColumnasTabla();
            await CargarAlumnosAsync();
        }

        // ---------------------------------------------------------
        // 2. CONFIGURACIÓN DE COLUMNAS (SOLUCIÓN DEFINITIVA)
        // ---------------------------------------------------------
        private void ConfigurarColumnasTabla()
        {
            dgvAlumnos.AutoGenerateColumns = false; // Apagamos el automático
            dgvAlumnos.Columns.Clear(); // Limpiamos basura anterior

            // Agregamos columnas manualmente.
            // IMPORTANTE: 'DataPropertyName' debe ser IGUAL a tu clase Alumno (Models)
            // 'Name' es para buscar la columna luego en el evento Click.

            dgvAlumnos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", HeaderText = "Código", Width = 50 });
            dgvAlumnos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", DataPropertyName = "Nombre", HeaderText = "Nombre" });
            dgvAlumnos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellido", DataPropertyName = "Apellido", HeaderText = "Apellido" });

            // OJO: Aquí uso "legajo" (minúscula) en DataPropertyName porque así estaba en tu foto de la clase Alumno
            dgvAlumnos.Columns.Add(new DataGridViewTextBoxColumn { Name = "legajo", DataPropertyName = "legajo", HeaderText = "N° Legajo" });

            // Columna Fecha con formato
            var colFecha = new DataGridViewTextBoxColumn { Name = "FechaNacimiento", DataPropertyName = "FechaNacimiento", HeaderText = "Fecha Nac." };
            colFecha.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvAlumnos.Columns.Add(colFecha);
        }

        // ---------------------------------------------------------
        // 3. CARGA DE DATOS
        // ---------------------------------------------------------
        private async Task CargarAlumnosAsync()
        {
            try
            {
                var lista = await _service.ObtenerAlumnosAsync();

                if (lista == null)
                {
                    MessageBox.Show("Error: La API devolvió una lista NULA.");
                    return;
                }

                // Truco para refrescar visualmente
                dgvAlumnos.DataSource = null;
                dgvAlumnos.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo conectar a la API.\n\nAsegúrate de que el proyecto negro (Backend) esté corriendo en http://localhost:5261\n\nError: {ex.Message}");
            }
        }

        // ---------------------------------------------------------
        // 4. BOTONES Y EVENTOS
        // ---------------------------------------------------------

        private async void btnAgregar_Click(object sender, EventArgs e)
        {

    
            if (!ValidarCampos()) return;

            var nuevo = new Alumno
            {
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                legajo = txtLegajo.Text, // "legajo" minúscula en tu modelo
                FechaNacimiento = dtpFechaNacimiento.Value
            };

            bool exito = await _service.AgregarAlumnoAsync(nuevo);

            if (exito)
            {
                MessageBox.Show("Alumno agregado correctamente.");
                await CargarAlumnosAsync();
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Error al agregar. Revisa si el legajo ya existe o si la API falló.");
            }
        }

        private async void btnActualizar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un alumno de la lista primero.");
                return;
            }
            if (!ValidarCampos()) return;

            var actualizado = new Alumno
            {
                Id = _idSeleccionado,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                legajo = txtLegajo.Text,
                FechaNacimiento = dtpFechaNacimiento.Value
            };

            bool exito = await _service.ActualizarAlumnoAsync(actualizado);

            if (exito)
            {
                MessageBox.Show("Alumno actualizado.");
                await CargarAlumnosAsync();
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Error al actualizar.");
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un alumno para eliminar.");
                return;
            }

            var confirm = MessageBox.Show("¿Está seguro de eliminar este alumno?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                bool exito = await _service.EliminarAlumnoAsync(_idSeleccionado);
                if (exito)
                {
                    MessageBox.Show("Alumno eliminado.");
                    await CargarAlumnosAsync();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar.");
                }
            }
        }

        private void dgvAlumnos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Obtenemos la fila actual
                var row = dgvAlumnos.Rows[e.RowIndex];

                // Pasamos los datos a los TextBox
                // Usamos los Nombres ("Name") que definimos en ConfigurarColumnasTabla
                _idSeleccionado = Convert.ToInt32(row.Cells["Id"].Value);
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString();
                txtApellido.Text = row.Cells["Apellido"].Value?.ToString();
                txtLegajo.Text = row.Cells["legajo"].Value?.ToString(); // "legajo" minúscula
                dtpFechaNacimiento.Value = Convert.ToDateTime(row.Cells["FechaNacimiento"].Value);

                // Control de botones
                btnAgregar.Enabled = false;
                btnActualizar.Enabled = true;
                btnEliminar.Enabled = true;
            }
        }

        // ---------------------------------------------------------
        // 5. MÉTODOS DE AYUDA (Helpers)
        // ---------------------------------------------------------

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtLegajo.Text))
            {
                MessageBox.Show("Nombre, Apellido y Legajo son obligatorios");
                return false;
            }

            if (!EsMayorDe16(dtpFechaNacimiento.Value))
            {
                MessageBox.Show("El alumno debe ser mayor de 16 años");
                return false;
            }
            return true;
        }

        private bool EsMayorDe16(DateTime fechaNacimiento)
        {
            var edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;
            return edad >= 16;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtLegajo.Clear();
            dtpFechaNacimiento.Value = DateTime.Now;
            _idSeleccionado = 0;

            btnAgregar.Enabled = true;
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;

            // Opcional: Deseleccionar fila de la tabla
            dgvAlumnos.ClearSelection();
        }

        private void btnIrAMaterias_Click(object sender, EventArgs e)
        {
            // Esto abre la ventana de materias
            FormMaterias frm = new FormMaterias();
            frm.ShowDialog(); // ShowDialog bloquea la ventana anterior hasta que cierres esta
        }

        private void btnVerInscripciones_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Primero selecciona un alumno de la lista (Click en la fila).");
                return;
            }

            // Obtenemos el nombre para ponerlo bonito en el título
            string nombreCompleto = txtNombre.Text + " " + txtApellido.Text;

            
            FormInscripciones frm = new FormInscripciones(_idSeleccionado, nombreCompleto);
            frm.ShowDialog();
        }
    }
    }

