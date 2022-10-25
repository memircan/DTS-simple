using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class EditPasswordViewModel
    {
        [Required(ErrorMessage = "Boş Geçilemez !")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Boş Geçilemez !")]
        public string NewPassword { get; set; }
    }
}
