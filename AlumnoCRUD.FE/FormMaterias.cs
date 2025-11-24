using AlumnoCRUD.FE.Models;
using AlumnoCRUD.FE.Services;
using AlumnoCRUD.FE.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlumnoCRUD.FE
{
    public partial class FormMaterias : Form
    {
        private readonly MateriaService _service;

        public FormMaterias(MateriaService service)
        {
            InitializeComponent();

            _service = service;

            // Conectar evento Load
            this.Load += FormMaterias_Load;
        }

        private async void FormMaterias_Load(object sender, EventArgs e)
        {
            // 2. Configurar columnas manualmente (Para que no falle)
            dgvMaterias.AutoGenerateColumns = false;
            dgvMaterias.Columns.Clear();

            dgvMaterias.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvMaterias.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Materia", Width = 200 });
            dgvMaterias.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Creditos", HeaderText = "Créditos" });

            await CargarMateriasAsync();
        }

        private async Task CargarMateriasAsync()
        {
            try
            {
                var lista = await _service.ObtenerMateriasAsync();
                dgvMaterias.DataSource = null;
                dgvMaterias.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar materias: " + ex.Message);
            }
        }

        // --------------------------------------------------------
        // ¡IMPORTANTE! 
        // Asegúrate de conectar este evento al botón desde el "Rayo" ⚡
        // --------------------------------------------------------
        private async void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidationHelper.AreFieldsNotEmpty(txtNombre, txtCreditos)) return;

            if (!ValidationHelper.IsValidNumber(txtCreditos.Text, out int creditos, "Créditos")) return;

            var nueva = new Materia
            {
                Nombre = txtNombre.Text,
                Creditos = creditos
            };

            bool exito = await _service.AgregarMateriaAsync(nueva);

            if (exito)
            {
                MessageBox.Show("Materia agregada.");
                txtNombre.Clear();
                txtCreditos.Clear();
                await CargarMateriasAsync();
            }
            else
            {
                MessageBox.Show("Error al agregar materia.");
            }
        }
    }
}