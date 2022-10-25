using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class EvrakEkle2
    {
        //for upload document file (pdf, png ...)

        [Required(ErrorMessage = "Boş geçilemez !")]
        public int Id { get; set; }
 

        [Required(ErrorMessage = "Lütfen Birim Seçiniz !")]
        public int BirimId { get; set; }
        public int UserId { get; set; }


        [Required(ErrorMessage = "Gönderildiği Yer Boş Geçilemez !")]
        public string GidenGonderilen { get; set; }
        public IFormFile URL { get; set; }


        [StringLength(133, ErrorMessage = "Açıklama en fazla 132 karakterden oluşabilir.")]
        [Required(ErrorMessage = "Açıklama Alanı Boş Geçilemez !")]
        public string GidenKonu { get; set; }


        [Required(ErrorMessage = "Belge Tipi Boş Geçilemez !")]
        public string GidenCins { get; set; }


        [Required(ErrorMessage = "Tarih Bilgisi Boş Geçilemez !")]
        public DateTime GidenTarih { get; set; }

        
    }
}
