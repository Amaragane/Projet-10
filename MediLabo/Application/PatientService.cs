using Domain.Entities;
using Domain.Interfaces.RepositoriesInterfaces;
using Domain.Interfaces.ServicesInterfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    internal class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;
        public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync();
        }
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            try
            {
                return await _patientRepository.GetPatientByIdAsync(id);

            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving patient with ID {id}");
                throw;
            }
        }
        public async Task AddPatientAsync(Patient patient)
        {
            if(await PatientExistAsync(patient.Id))
            {
                throw new InvalidOperationException($"Patient with ID {patient.Id} already exists.");
            }
            await _patientRepository.AddPatientAsync(patient);
        }
        public async Task UpdatePatientAsync(Patient patient)
        {
            if (!await PatientExistAsync(patient.Id))
            {
                throw new KeyNotFoundException($"Patient with ID {patient.Id} not found.");
            }
            var existingPatient = await GetPatientByIdAsync(patient.Id);
            await _patientRepository.UpdatePatientAsync(existingPatient,patient);
        }
        public async Task DeletePatientAsync(int id)
        {
            if (!await PatientExistAsync(id))
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }
            await _patientRepository.DeletePatientAsync(id);
        }
        public async Task<bool> PatientExistAsync(int id)
        {
            return await _patientRepository.PatientExistsAsync(id);
        }

    }
}
