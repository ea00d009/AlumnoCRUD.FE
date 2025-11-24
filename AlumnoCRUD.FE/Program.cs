using AlumnoCRUD.FE;
using AlumnoCRUD.FE.Services;
using System;
using System.Windows.Forms;

namespace SistemaAlumnos.FE
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 1. Activamos los estilos visuales (botones modernos, etc.)
            Application.EnableVisualStyles();

            // 2. Configuración de renderizado de texto
            Application.SetCompatibleTextRenderingDefault(false);

            // 3. Composición de Servicios (Dependency Injection Manual)
            var httpClient = ApiClientFactory.CreateClient();
            var alumnoService = new AlumnoService(httpClient);
            var materiaService = new MateriaService(httpClient);
            var inscripcionService = new InscripcionService(httpClient);

            // 4. Arrancamos el Formulario inyectando dependencias
            Application.Run(new Form1(alumnoService, materiaService, inscripcionService));
        }
    }
}