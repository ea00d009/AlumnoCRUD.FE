using AlumnoCRUD.FE.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System; // Para Console.WriteLine si hiciera falta

namespace AlumnoCRUD.FE.Services
{
    public class InscripcionService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public InscripcionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        // INSCRIBIR
        public async Task<bool> InscribirAlumnoAsync(int alumnoId, int materiaId)
        {
            var inscripcion = new Inscripcion { AlumnoId = alumnoId, MateriaId = materiaId };
            var json = JsonSerializer.Serialize(inscripcion, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/inscripciones", content);
            return response.IsSuccessStatusCode;
        }

        // OBTENER MATERIAS DEL ALUMNO
        public async Task<List<Materia>> ObtenerMateriasDeAlumnoAsync(int alumnoId)
        {
            // Ojo a la ruta: coincide con la del Controller
            var response = await _httpClient.GetAsync($"api/inscripciones/alumno/{alumnoId}");

            if (!response.IsSuccessStatusCode) return new List<Materia>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Materia>>(json, _jsonOptions) ?? new List<Materia>();
        }
    }
}