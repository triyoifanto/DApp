using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dto
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage="Minimum password is 6 character")]
        public string Password { get; set; }
    }
}