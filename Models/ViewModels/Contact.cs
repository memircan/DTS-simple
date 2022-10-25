using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class Contact
    {
        [Required(ErrorMessage = "Boş geçilemez !")]
        public string Baslik { get; set; }


        [Required(ErrorMessage = "Boş geçilemez !")]
        public string Desc { get; set; } //acıklama 


        [Required(ErrorMessage = "Boş geçilemez !")]
        public string Kadi { get; set; } //kullanıcı adı


        [Required,EmailAddress(ErrorMessage = "Lütfen ornek@ formatında bir mail giriniz.")]
        public string Eposta { get; set; }
    }
}
