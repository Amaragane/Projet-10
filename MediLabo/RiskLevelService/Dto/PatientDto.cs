namespace RiskLevelService.Dto
{
    public class PatientDto
    {
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = null!; // "M" ou "F"
    }

}
