using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Models.Authentication
{
    public class AppUser: IdentityUser<int>
    {
        public string NameSurname { get; set; }

        
    }
}
