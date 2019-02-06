using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dto
{
    public class UserRegisterDto
    {
        public UserRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;            
        }

        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage="Minimum password is 6 character")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}