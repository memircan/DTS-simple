using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication1.Models.ViewModels;





namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            
            return View();
        }

        public IActionResult Contact()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Contact(Contact model) //İletişim
        {
            if (ModelState.IsValid)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("gmail", "Evrak Takip Sistemi", System.Text.Encoding.UTF8);
                msg.To.Add("info.evraktakipsistemi@gmail.com");
                msg.Subject = model.Baslik;
                msg.Body = "Personel: " + model.Kadi + "<br><br> Açıklama: " + model.Desc + "<br><br> Eposta: " + model.Eposta;
                msg.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("gmail", "password");
                    client.Host = "smtp.gmail.com"; 
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    try
                    {
                        client.Send(msg);
                        TempData["Message"] = "Mesajınız iletilmiştir. En kısa sürede incelenecektir.";

                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Mesaj gönderilemedi. Hata nedeni:" + ex.Message;
                    }
                }
                ModelState.Clear();
            }
            return View();
        }










        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
