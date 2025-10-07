namespace RiskLevelService.Services
{
    public interface IRiskService
    {
        Task<string> CalculateRiskAsync(string patientId, string token);

    }
}
