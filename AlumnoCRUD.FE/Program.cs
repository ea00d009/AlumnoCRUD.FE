using AlumnoCRUD.FE;
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

            // 3. Arrancamos el Formulario
            // Asegúrate de que "Form1" es el nombre de tu ventana principal
            Application.Run(new Form1());
        }
    }
}