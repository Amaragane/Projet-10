using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PatientController
    {
        [ApiController]
        [Route("api/[controller]")]
        public class PatientsController : ControllerBase
        {
            // GET: api/patients
            [HttpGet]
            public IActionResult GetAllPatients()
            {
                // Récupérer et retourner la liste des patients
            }

            // GET: api/patients/{id}
            [HttpGet("{id}")]
            public IActionResult GetPatientById(int id)
            {
                // Récupérer et retourner les informations d'un patient spécifique
            }

            // POST: api/patients
            [HttpPost]
            public IActionResult CreatePatient([FromBody] PatientCreateDto newPatient)
            {
                // Ajouter un nouveau patient
            }

            // PUT: api/patients/{id}
            [HttpPut("{id}")]
            public IActionResult UpdatePatient(int id, [FromBody] PatientUpdateDto updatedPatient)
            {
                // Mettre à jour les informations personnelles du patient
            }
            // DELETE: api/patients/{id}
            [HttpDelete("{id}")]
            public IActionResult DeletePatient(int id)
            {
                // Supprimer un patient
            }
        }


    }
}
