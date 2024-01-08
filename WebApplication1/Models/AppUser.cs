using Microsoft.AspNetCore.Identity;
using WebApplication1.Utilities.Enums;

namespace WebApplication1.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
    }
}
