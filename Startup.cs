using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.CustomValidations;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; set; }
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddMvc();
            services.AddControllersWithViews();
            services.AddSession();           
            services.AddDbContext<Context>();
            services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));//******
            
            services.AddIdentity<AppUser,AppRole>(_ =>
            {
                _.Password.RequiredLength = 5; //En az ka? karakterli olmas? gerekti?ini belirtiyoruz.
                _.Password.RequireNonAlphanumeric = false; //Alfanumerik zorunlulu?unu kald?r?yoruz.
                _.Password.RequireLowercase = false; //K???k harf zorunlulu?unu kald?r?yoruz.
                _.Password.RequireUppercase = false; //B?y?k harf zorunlulu?unu kald?r?yoruz.
                _.Password.RequireDigit = false; //0-9 aras? say?sal karakter zorunlulu?unu kald?r?yoruz.
                _.User.RequireUniqueEmail = true; //Email adreslerini tekille?tiriyoruz.           
                _.User.AllowedUserNameCharacters = "abc?defghi?jklmno?pqrs?tu?vwxyzABC?DEFGHI?JKLMNO?PQRS?TU?VWXYZ0123456789-._@+"; //Kullan?c? ad?nda ge?erli olan karakterleri belirtiyoruz.
            }).AddEntityFrameworkStores<Context>()
              .AddPasswordValidator<CustomPasswordValidation>()
              .AddUserValidator<CustomUserValidation>()
              .AddErrorDescriber<CustomIdentityErrorDescriber>()
              .AddEntityFrameworkStores<Context>()
              .AddDefaultTokenProviders();
              
            
            
            services.ConfigureApplicationCookie(_ =>
            {
                _.LoginPath = new PathString("/User/Login");
                _.Cookie = new CookieBuilder
                {
                    Name = "IdentityCookie", //Olu?turulacak Cookie'yi isimlendiriyoruz.
                    HttpOnly = false, //K?t? niyetli insanlar?n client-side taraf?ndan Cookie'ye eri?mesini engelliyoruz.
                    //Expiration = TimeSpan.FromMinutes(2), //Olu?turulacak Cookie'nin vadesini belirliyoruz.
                    SameSite = SameSiteMode.Lax, //Top level navigasyonlara sebep olmayan requestlere Cookie'nin g?nderilmemesini belirtiyoruz.
                    SecurePolicy = CookieSecurePolicy.Always //HTTPS ?zerinden eri?ilebilir yap?yoruz.
                };
                _.SlidingExpiration = true; //Expiration s?resinin yar?s? kadar s?re zarf?nda istekte bulunulursa e?er geri kalan yar?s?n? tekrar s?f?rlayarak ilk ayarlanan s?reyi tazeleyecektir.
                _.ExpireTimeSpan = TimeSpan.FromMinutes(20); //CookieBuilder nesnesinde tan?mlanan Expiration de?erinin varsay?lan de?erlerle ezilme ihtimaline kar??n tekrardan Cookie vadesi burada da belirtiliyor.
                _.AccessDeniedPath = new PathString("/Authority/Page"); //Yetkisi olmayan kullan?c?lar y?nlendiriliyor.
            });


            
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {   
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            

            app.UseRouting();
            app.UseStaticFiles();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages(); 
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=Login}/{id?}");
            });
        }
    }
}
