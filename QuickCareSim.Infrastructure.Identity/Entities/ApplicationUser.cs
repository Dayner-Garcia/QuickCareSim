
using Microsoft.AspNetCore.Identity;

namespace QuickCareSim.Infrastructure.Identity.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public required string Name;
        public required string LastName;
    }
}
