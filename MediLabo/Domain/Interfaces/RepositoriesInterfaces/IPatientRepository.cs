using Domain.Entities;


namespace Domain.Interfaces.RepositoriesInterfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient> GetPatientByIdAsync(int id);
        Task AddPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient existingPatient,Patient newValues);
        Task DeletePatientAsync(int id);
        Task<bool> PatientExistsAsync(int id);
    }
}
