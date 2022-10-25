using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Authentication;

namespace WebApplication1.Models.ViewModels
{
    public class GidenEvrak
    {
        [Key]
        public int GidenId { get; set; }

        [Required]
        public int UserId { get; set; }

        public AppUser User { get; set; }

        public string URL { get; set; }

        [Required(ErrorMessage = "Lütfen Birim Seçiniz !")]
        public int BirimId { get; set; }
        public Birimler Birim { get; set; }

        [Required(ErrorMessage = "Gönderildiği Yer Boş Geçilemez !")]
        public string GidenGonderilen { get; set; }


        [StringLength(133, ErrorMessage = "Açıklama en fazla 132 karakterden oluşabilir.")]
        [Required(ErrorMessage = "Açıklama Alanı Boş Geçilemez !")]
        public string GidenKonu { get; set; }

        [Required(ErrorMessage = "Belge Tipi Boş Geçilemez !")]
        public string GidenCins { get; set; }

        [Required(ErrorMessage = "Tarih Bilgisi Boş Geçilemez !")]
        public DateTime GidenTarih { get; set; }

         
    }
}
