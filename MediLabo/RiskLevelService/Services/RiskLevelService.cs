using RiskLevelService.Dto;
using System.Net.Http;

namespace RiskLevelService.Services
{
    internal class RiskService : IRiskService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public RiskService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> CalculateRiskAsync(string patientId, string token)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Appel patient API SQL
            var reqPatient = new HttpRequestMessage(HttpMethod.Get, $"http://patientservice:5001/api/patients/{patientId}");
            reqPatient.Headers.Add("Authorization", $"Bearer {token}");
            var patientRes = await httpClient.SendAsync(reqPatient);
            if (!patientRes.IsSuccessStatusCode) return null;
            var patient = await patientRes.Content.ReadFromJsonAsync<PatientDto>();

            // Appel notes API Mongo
            var reqNotes = new HttpRequestMessage(HttpMethod.Get, $"http://patientnotesservice:5003/api/notes/{patientId}");
            reqNotes.Headers.Add("Authorization", $"Bearer {token}");
            var notesRes = await httpClient.SendAsync(reqNotes);
            if (!notesRes.IsSuccessStatusCode) return null;
            var notes = await notesRes.Content.ReadFromJsonAsync<List<NoteDto>>();

            // logiques métier de calcul de risque
            var risk = RiskLevelLogic.CalculateRisk(patient, notes);

            return  risk ;
        }
    }

}

