using IdentityExpress.Identity;
using Microsoft.EntityFrameworkCore;

namespace microservices.identity.Data;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : IdentityExpressDbContext<ApplicationUser>(options);