using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

        [ApiController]
        [Route("api/[controller]")]
        public class PatientsController : ControllerBase
        {
            private readonly IPatientService _patientService;
            private readonly ILogger<PatientsController> _logger;
            private readonly IMapper _mapper;
        public PatientsController(IPatientService patientService,ILogger<PatientsController> logger, IMapper mapper)
            {
                _patientService = patientService;
                  _logger = logger;
                  _mapper = mapper;
        }
        // GET: api/patients
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            try
            {
                var patients = await _patientService.GetAllPatientsAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
                // Récupérer et retourner la liste des patients
            }
        }

            // GET: api/patients/{id}
            [HttpGet("{id}")]
            public async Task<IActionResult> GetPatientById(int id)
            {
                try
                {
                    var patient = await _patientService.GetPatientByIdAsync(id);
                    if (patient == null)
                    {
                        return NotFound();
                    }
                     return Ok(patient);
            }
                catch(Exception ex)
                {
                    return NotFound(ex.Message);
                }
                // Récupérer et retourner les informations d'un patient spécifique
            }

        // POST: api/patients
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientCreateDto newPatient)
        {
            try
            {
                var patient = _mapper.Map<Patient>(newPatient);
                  await _patientService.AddPatientAsync(patient);
                  return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex, "Error creating new patient");
                  return BadRequest(ex.Message);

                // Ajouter un nouveau patient
            }
        }

            // PUT: api/patients/{id}
            [HttpPut("{id}")]
            public async  Task<IActionResult> UpdatePatient(int id, [FromBody] PatientUpdateDto updatedPatient)
            {
                // Mettre à jour les informations personnelles du patient
                try
                {
                     var patient = _mapper.Map<Patient>(updatedPatient);
                     patient.Id = id;
                     await _patientService.UpdatePatientAsync(patient);
                     return RedirectToAction(nameof(GetPatientById), new { id = patient.Id });
                }
                catch (Exception ex)
                {
                     _logger.LogError(ex, $"Error updating patient with ID {id}");
                     return BadRequest(ex.Message);
                }

        }
            // DELETE: api/patients/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeletePatient(int id)
            {
                // Supprimer un patient
                  try
                  {
                        await _patientService.DeletePatientAsync(id);
                        return NoContent();
                  }
                  catch (Exception ex)
                  {
                        _logger.LogError(ex, $"Error deleting patient with ID {id}");
                        return BadRequest(ex.Message);
            }
        }
        }


    
}
