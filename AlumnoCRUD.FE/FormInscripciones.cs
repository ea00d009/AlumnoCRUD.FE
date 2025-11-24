using AlumnoCRUD.FE.Models;
using AlumnoCRUD.FE.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace AlumnoCRUD.FE
{
    public partial class FormInscripciones : Form
    {
        private readonly InscripcionService _inscripcionService;
        private readonly MateriaService _materiaService;

        private int _alumnoId; // Guardamos el ID del alumno que estamos editando

        // CONSTRUCTOR ESPECIAL: Recibe el ID y Nombre del Alumno desde el Form1
        public FormInscripciones(int alumnoId, string nombreCompleto, InscripcionService inscripcionService, MateriaService materiaService)
        {
            InitializeComponent();

            _alumnoId = alumnoId;
            lblAlumno.Text = "Inscripciones de: " + nombreCompleto;

            _inscripcionService = inscripcionService;
            _materiaService = materiaService;

            this.Load += FormInscripciones_Load;
        }

        private async void FormInscripciones_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            await CargarComboMaterias(); // Llenar el desplegable con TODAS
            await CargarInscripciones(); // Llenar la tabla con las del ALUMNO
        }

        private void ConfigurarTabla()
        {
            dgvInscripciones.AutoGenerateColumns = false;
            dgvInscripciones.Columns.Clear();
            dgvInscripciones.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvInscripciones.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Materia", Width = 200 });
            dgvInscripciones.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Creditos", HeaderText = "Créditos" });
        }

        private async Task CargarComboMaterias()
        {
            var listaTodas = await _materiaService.ObtenerMateriasAsync();

            // Configurar qué muestra el combo y qué valor guarda oculto
            cmbMaterias.DataSource = listaTodas;
            cmbMaterias.DisplayMember = "Nombre"; // Lo que ve el usuario
            cmbMaterias.ValueMember = "Id";       // El valor real (ID)
        }

        private async Task CargarInscripciones()
        {
            var listaDelAlumno = await _inscripcionService.ObtenerMateriasDeAlumnoAsync(_alumnoId);
            dgvInscripciones.DataSource = null;
            dgvInscripciones.DataSource = listaDelAlumno;
        }

        // ------------------------------------------------------
        // CONECTAR ESTE BOTÓN CON EL RAYO ⚡
        // ------------------------------------------------------
        private async void btnInscribir_Click(object sender, EventArgs e)
        {
            if (cmbMaterias.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una materia");
                return;
            }

            int materiaId = (int)cmbMaterias.SelectedValue;

            // Llamamos al servicio para crear la unión
            bool exito = await _inscripcionService.InscribirAlumnoAsync(_alumnoId, materiaId);

            if (exito)
            {
                MessageBox.Show("¡Inscrito con éxito!");
                await CargarInscripciones(); // Recargar la tabla para ver el cambio
            }
            else
            {
                MessageBox.Show("Error: Quizás ya está inscrito en esa materia.");
            }
        }
    }
}