using AlumnoCRUD.FE.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace AlumnoCRUD.FE.Services
{
    public class MateriaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public MateriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<Materia>> ObtenerMateriasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/materias");
                if (!response.IsSuccessStatusCode) return new List<Materia>();

                var json = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<List<Materia>>(json, _jsonOptions);
                return resultado ?? new List<Materia>();
            }
            catch
            {
                return new List<Materia>();
            }
        }

        public async Task<bool> AgregarMateriaAsync(Materia materia)
        {
            var json = JsonSerializer.Serialize(materia, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/materias", content);
            return response.IsSuccessStatusCode;
        }

        // Agrega aquí Actualizar y Eliminar si los necesitas en el futuro
    }
}