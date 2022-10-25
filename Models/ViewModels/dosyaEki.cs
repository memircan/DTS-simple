using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class EvrakEkle
    {
        //for upload document file (pdf, png ...)

        [Required(ErrorMessage = "Boş geçilemez !")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen Birim Seçiniz !")]
        public int BirimId { get; set; }

        public int UserId { get; set; }
        public IFormFile URL { get; set; }

        [StringLength(133, ErrorMessage = "Açıklama en fazla 132 karakterden oluşabilir.")]
        [Required(ErrorMessage = "Açıklama Alanı Boş Geçilemez !")]
        public string GelenKonu { get; set; }

        [Required(ErrorMessage = "Belge Tipi Boş Geçilemez !")]
        public string GelenCins { get; set; }

        [Required(ErrorMessage = "Tarih Bilgisi Boş Geçilemez !")]
        public DateTime GelenTarih { get; set; }

        [Required(ErrorMessage = "Gönderen Bilgisi Boş Geçilemez !")]
        public string GelenGonderen { get; set; }
    }
}
