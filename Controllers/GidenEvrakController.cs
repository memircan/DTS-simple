using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.ViewModels;

using Microsoft.AspNetCore.Identity;
using WebApplication1.Models.Authentication;
using Microsoft.AspNetCore.Authorization;

//-----------GİDEN EVRAK

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Mod,Personel")]
    public class GidenEvrakController : Controller
    {
        Context c =new Context();

        readonly UserManager<AppUser> _userManager;
        public GidenEvrakController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        public IActionResult Index(string birimAra, string tarihAra,string personelAra,string gonderilenAra)
        {
            var degerler = c.GidenEvraks.Include(x => x.Birim).Include(x=>x.User).ToList(); //evrak listelerken birim id kısmında birim adı gözükmesi icin include ettim

            if (!string.IsNullOrEmpty(birimAra))
            {
                degerler = degerler.Where(x => x.Birim.BirimAdi.ToLower().ToString().Contains(birimAra.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(personelAra))
            {
                degerler = degerler.Where(x => x.User.UserName.ToLower().ToString().Contains(personelAra.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(gonderilenAra))
            {
                degerler = degerler.Where(x => x.GidenGonderilen.ToLower().ToString().Contains(gonderilenAra.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(tarihAra))
            {
                degerler = degerler.Where(x => x.GidenTarih.ToString().Contains(tarihAra)).ToList();
            }
            

            return View(degerler);
        }

        
        [HttpGet] //sayfa yüklendiginde calısıcak
        public IActionResult YeniGidenE()
        {
            //user id çekme
            ViewBag.userId = _userManager.GetUserId(HttpContext.User).ToString();

            List<SelectListItem> degerler = (from x in c.Birimlers.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = x.BirimAdi,
                                                 Value = x.BirimId.ToString()
                                             }).ToList();
            ViewBag.brm = degerler;
            return View();
        }
        [HttpPost] //tıklandıgında calısıcak
        public IActionResult YeniGidenE(EvrakEkle2 gi)
        {
            GidenEvrak y = new GidenEvrak();
            if (gi.URL != null)
            {
                var uzanti = Path.GetExtension(gi.URL.FileName);
                var newimagename = Guid.NewGuid() + uzanti; //rastgele ön ek olusturucak
                var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ekler/", newimagename);
                var stream = new FileStream(konum, FileMode.Create);
                gi.URL.CopyTo(stream);
                y.URL = newimagename;
            }
            y.GidenTarih = gi.GidenTarih;
            y.UserId = gi.UserId;
            y.GidenGonderilen = gi.GidenGonderilen;
            y.BirimId = gi.BirimId;
            y.GidenCins = gi.GidenCins;
            y.GidenKonu = gi.GidenKonu;
           
            c.GidenEvraks.Add(y);
            c.SaveChanges();
            return RedirectToAction("Index");

        }

        [Authorize(Roles ="Admin,Mod")]
        public IActionResult Sil(int Id)
        {
            var evrk = c.GidenEvraks.Find(Id);
            DirectoryInfo df = new DirectoryInfo(path: "wwwroot/ekler/");
            if (evrk.URL != null)
            {
                foreach (FileInfo item in df.GetFiles(evrk.URL))
                {
                    item.Delete();
                }
            }
            c.GidenEvraks.Remove(evrk);
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        //Evrak bilgilerini getir
        [Authorize(Roles = "Admin,Mod")]
        public IActionResult Guncelle(int Id)
        {
            List<SelectListItem> degerler = (from x in c.Birimlers.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = x.BirimAdi,
                                                 Value = x.BirimId.ToString()
                                             }).ToList();
            ViewBag.brm = degerler;
            var evrak = c.GidenEvraks.Find(Id);    //evrakları getirme
            return View("Guncelle", evrak);
        }
        [Authorize(Roles = "Admin,Mod")]
        public IActionResult Guncelle2(GidenEvrak e)
        {
            var evrak = c.GidenEvraks.Find(e.GidenId);
            evrak.GidenId = e.GidenId;
            evrak.BirimId = e.BirimId;
            evrak.GidenTarih = e.GidenTarih;
            evrak.UserId = e.UserId;
            evrak.GidenGonderilen = e.GidenGonderilen;
            evrak.GidenCins = e.GidenCins;
            evrak.GidenKonu = e.GidenKonu;
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult ExcelAktar()
        {
            var gidenevrak = c.GidenEvraks.Include(x => x.Birim).Include(x => x.User).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("GidenEvrak");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Tarih";
                worksheet.Cell(currentRow, 3).Value = "Birim";
                worksheet.Cell(currentRow, 4).Value = "Belge Tipi";
                worksheet.Cell(currentRow, 5).Value = "Gönderilen Yer";
                worksheet.Cell(currentRow, 6).Value = "Açıklama";
                worksheet.Cell(currentRow, 7).Value = "Oluşturan Personel";
                

                #endregion

                #region Body
                foreach (var evrak in gidenevrak)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = evrak.GidenId;
                    worksheet.Cell(currentRow, 2).Value = evrak.GidenTarih;
                    worksheet.Cell(currentRow, 3).Value = evrak.Birim.BirimAdi;
                    worksheet.Cell(currentRow, 4).Value = evrak.GidenGonderilen;
                    worksheet.Cell(currentRow, 5).Value = evrak.GidenCins;
                    worksheet.Cell(currentRow, 6).Value = evrak.GidenKonu;
                    worksheet.Cell(currentRow, 7).Value = evrak.User.UserName;
                    

                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "GidenEvraklar.xlsx"
                        );
                }
            }
        }

        public IActionResult Detay(int id)
        {
            var model = c.GidenEvraks.Include(x => x.User).Include(x => x.Birim).First(x => x.GidenId == id);
            return View(model);
        }

    }
}
