using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Required]

        public bool IsRemembered { get; set; }
    }
}
