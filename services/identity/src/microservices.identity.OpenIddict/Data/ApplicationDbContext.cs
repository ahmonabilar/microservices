using IdentityExpress.Identity;
using Microsoft.EntityFrameworkCore;

namespace microservices.identity.Data;

public class ApplicationDbContext : IdentityExpressDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
