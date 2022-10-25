using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class Context: IdentityDbContext<AppUser,AppRole,int>
    {      
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("conectingString");
        }
        
        public DbSet<GelenEvrak> GelenEvraks { get; set; }//incoming doc
        public DbSet<GidenEvrak> GidenEvraks { get; set; }//up doc
        public DbSet<Birimler> Birimlers { get; set; }//depart

    }
}
