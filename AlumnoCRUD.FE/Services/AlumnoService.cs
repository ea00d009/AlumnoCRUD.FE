using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlumnoCRUD.FE.Models; // Asegúrate de que este using sea correcto según tu proyecto

namespace AlumnoCRUD.FE.Services
{
    public class AlumnoService
    {
        private readonly HttpClient _httpClient;

        // 1. Creamos una configuración única para evitar problemas de mayúsculas/minúsculas
        private readonly JsonSerializerOptions _jsonOptions;

        public AlumnoService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Configuración robusta para JSON
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // Ignora mayúsculas al LEER (API -> Form)
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Convierte a minúsculas al ENVIAR (Form -> API)
                WriteIndented = true
            };
        }

        // GET: Obtener listado
        public async Task<List<Alumno>> ObtenerAlumnosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/alumnos");

                if (!response.IsSuccessStatusCode)
                {
                    // Si la API da error, retornamos lista vacía para que no explote el programa
                    return new List<Alumno>();
                }

                var json = await response.Content.ReadAsStringAsync();

                // Usamos nuestras opciones configuradas
                var resultado = JsonSerializer.Deserialize<List<Alumno>>(json, _jsonOptions);
                return resultado ?? new List<Alumno>();
            }
            catch (Exception)
            {
                // En caso de error de conexión, retornamos lista vacía
                return new List<Alumno>();
            }
        }

        // POST: Agregar alumno
        public async Task<bool> AgregarAlumnoAsync(Alumno alumno)
        {
            // 2. IMPORTANTE: Usar _jsonOptions aquí también para enviar en formato correcto
            var json = JsonSerializer.Serialize(alumno, _jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/alumnos", content);

            return response.IsSuccessStatusCode;
        }

        // PUT: Actualizar alumno
        public async Task<bool> ActualizarAlumnoAsync(Alumno alumno)
        {
            var json = JsonSerializer.Serialize(alumno, _jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // Nota: Unifiqué la ruta a minúsculas "api/alumnos" para ser consistentes
            var response = await _httpClient.PutAsync($"api/alumnos/{alumno.Id}", content);

            return response.IsSuccessStatusCode;
        }

        // DELETE: Eliminar alumno
        public async Task<bool> EliminarAlumnoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/alumnos/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}