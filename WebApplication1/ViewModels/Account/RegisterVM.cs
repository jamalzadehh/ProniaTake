using System.ComponentModel.DataAnnotations;
using WebApplication1.Utilities.Enums;

namespace WebApplication1.ViewModels.Account
{
    public class RegisterVM
    {
        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        public string Username { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(25)]

        public string Name { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(25)]

        public string Surname { get; set; }
        [Required]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Duzgun simvollardan istifade edin.")]

        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]

        public string ConfirmPassword { get; set; }
        public Gender Gender { get; set; }

    }
}
