using Fated.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fated.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }

        public DbSet<Note> Note { get; set; }
    }
}
