using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class Birimler 
    {
        [Key]
        public int BirimId { get; set; }

        public string BirimAdi { get; set; }
    }
}
