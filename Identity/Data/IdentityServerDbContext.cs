using Microsoft.EntityFrameworkCore;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLEXOS.Identity.Data
{
    public class IdentityServerDbContext  : DbContext
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) : base(options)
        {
          
        }

        public DbSet<Organization> Organizations { get; set; }
    }
}
