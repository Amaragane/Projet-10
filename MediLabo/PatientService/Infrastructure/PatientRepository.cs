using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    internal class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _context;
        private readonly ILogger<PatientRepository> _logger;
        // Implementation of IPatientRepository methods would go here
        public PatientRepository(PatientDbContext context, ILogger<PatientRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await Task.FromResult(_context.Patients.ToList());
        }
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }
            return patient;
        }
        public async Task AddPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }
        public async Task UpdatePatientAsync(Patient patient)
        {
 
            _context.Entry(patient).CurrentValues.SetValues(patient);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> PatientExistsAsync(int id)
        {
            return await Task.FromResult(_context.Patients.Any(p => p.Id == id));
        }

    }
}
