using AlumnoCRUD.FE.Models;
using AlumnoCRUD.FE.Services;
using System;
using System.Collections.Generic; // Necesario para List<>
using System.Drawing; // A veces necesario para UI
using System.Threading.Tasks;
using System.Windows.Forms;

using AlumnoCRUD.FE.Helpers; // Importar Helper

namespace AlumnoCRUD.FE
{
    public partial class Form1 : Form
    {
        private readonly AlumnoService _service;
        private readonly MateriaService _materiaService;
        private readonly InscripcionService _inscripcionService;
        private int _idSeleccionado = 0;

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

            // OJO: Ahora usamos "Legajo" (PascalCase)
            dgvAlumnos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Legajo", DataPropertyName = "Legajo", HeaderText = "N° Legajo" });

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
                Legajo = txtLegajo.Text, // "Legajo" PascalCase en tu modelo
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
                Legajo = txtLegajo.Text,
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
                txtLegajo.Text = row.Cells["Legajo"].Value?.ToString(); // "Legajo" PascalCase
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
            // Usamos el Helper para validar vacíos
            if (!ValidationHelper.AreFieldsNotEmpty(txtNombre, txtApellido, txtLegajo))
                return false;

            // Usamos el Helper para validar edad
            if (!ValidationHelper.IsOlderThan(dtpFechaNacimiento.Value, 16))
                return false;

            return true;
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
            // Lógica para no abrir múltiples ventanas iguales
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is FormMaterias)
                {
                    openForm.BringToFront();
                    return;
                }
            }

            FormMaterias frm = new FormMaterias(_materiaService);
            frm.Show(); // Show() NO bloquea
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

            // Para inscripciones sí usamos ShowDialog porque depende estrictamente del alumno seleccionado
            // y queremos que al cerrar se "termine" esa acción. Pero es una decisión de diseño.
            // Lo dejaremos modal (bloqueante) para evitar confusiones de editar otro alumno mientras inscribes.
            FormInscripciones frm = new FormInscripciones(_idSeleccionado, nombreCompleto, _inscripcionService, _materiaService);
            frm.ShowDialog();
        }
    }
    }

