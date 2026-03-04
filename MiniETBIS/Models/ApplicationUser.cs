using Microsoft.AspNetCore.Identity;

namespace MiniETBIS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Company? Company { get; set; }
    }
}
