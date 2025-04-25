using Microsoft.AspNetCore.Identity;

namespace QuickCareSim.Infrastructure.Identity.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}