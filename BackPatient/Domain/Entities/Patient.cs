using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }

}
