using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiskLevelService.Services;

[ApiController]
[Route("api/[controller]")]
public class RiskLevelController : ControllerBase
{
    private readonly IRiskService _riskLevelService;

    public RiskLevelController(IRiskService riskLevelService)
    {
        _riskLevelService = riskLevelService;
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetRiskLevel(string patientId)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            return Unauthorized();

        var result = await _riskLevelService.CalculateRiskAsync(patientId, token);

        if (result == null)
            return NotFound();

        return Ok(new { level = result });
    }
}
