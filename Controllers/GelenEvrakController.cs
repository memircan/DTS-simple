using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;
using System.Net;
using WebApplication1.Models.Authentication;


//------------ GELEN EVRAK

namespace WebApplication1.Controllers
{
    [Authorize(Roles ="Admin,Mod,Personel")]
    public class GelenEvrakController : Controller
    {
        Context c = new Context();
        readonly UserManager<AppUser> _userManager;       

        public GelenEvrakController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;

        }

        // Evrak listeleme ve arama
        public IActionResult Index(string birimAra, string tarihAra, string personelAra,string gonderenAra)
        {            
            var degerler = c.GelenEvraks.Include(x => x.Birim).Include(x=>x.User).ToList(); //evrak listelerken birim id kısmında birim adı gözükmesi icin include ettim
            
           
            if (!string.IsNullOrEmpty(birimAra))
            {
                degerler = degerler.Where(x => x.Birim.BirimAdi.ToLower().ToString().Contains(birimAra.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(personelAra))
            {
                degerler = degerler.Where(x => x.User.UserName.ToLower().ToString().Contains(personelAra.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(tarihAra))
            {
                degerler = degerler.Where(x => x.GelenTarih.ToString().Contains(tarihAra)).ToList();
            }
            if (!string.IsNullOrEmpty(gonderenAra))
            {
                degerler = degerler.Where(x => x.GelenGonderen.ToLower().ToString().Contains(gonderenAra.ToLower())).ToList();
            }

            return View(degerler);
        }

        

        [HttpGet] //sayfa yüklendiginde calısıcak
        public IActionResult YeniGelenE()
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
        public IActionResult YeniGelenE(EvrakEkle ge)
        {
            GelenEvrak y = new GelenEvrak();
            if(ge.URL !=null)
            {
                var uzanti = Path.GetExtension(ge.URL.FileName);
                var newimagename = Guid.NewGuid() + uzanti; //rastgele ön ek olusturucak
                var konum = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/ekler/",newimagename);
                var stream = new FileStream(konum, FileMode.Create);
                ge.URL.CopyTo(stream);
                y.URL = newimagename;
            }
            y.BirimId = ge.BirimId;
            y.GelenCins = ge.GelenCins;
            y.GelenGonderen = ge.GelenGonderen;
            y.UserId = ge.UserId;
            y.GelenKonu = ge.GelenKonu;
            y.GelenTarih = ge.GelenTarih;

            
            c.GelenEvraks.Add(y);
            c.SaveChanges();
            return RedirectToAction("Index");
            
        }



        [Authorize(Roles = "Admin,Mod")]
        public IActionResult Sil(int Id)
        {

            var evrk = c.GelenEvraks.Find(Id);
            DirectoryInfo df = new DirectoryInfo(path:"wwwroot/ekler/");
            if(evrk.URL != null) 
            {
                foreach (FileInfo item in df.GetFiles(evrk.URL))
                {
                    item.Delete();   
                }
            }
            
            c.GelenEvraks.Remove(evrk);
            c.SaveChanges();
            return RedirectToAction("Index");
        }



        //Evrak bilgilerini getir
        [Authorize(Roles ="Admin,Mod")]
        public IActionResult Guncelle(int Id)
        {
            List<SelectListItem> degerler = (from x in c.Birimlers.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = x.BirimAdi,
                                                 Value = x.BirimId.ToString()
                                             }).ToList();
            ViewBag.brm = degerler;
            var evrak = c.GelenEvraks.Find(Id);    //evrakları getirme
            return View("Guncelle",evrak);
        }

        [Authorize(Roles = "Admin,Mod")]
        public IActionResult Guncelle2(GelenEvrak e)
        {
            var evrak = c.GelenEvraks.Find(e.GelenId);
            evrak.GelenId = e.GelenId;
            evrak.BirimId=e.BirimId;
            evrak.GelenTarih = e.GelenTarih;
            evrak.UserId = e.UserId;
            evrak.GelenGonderen = e.GelenGonderen;
            evrak.GelenCins = e.GelenCins;
            evrak.GelenKonu = e.GelenKonu;
            c.SaveChanges();
            return RedirectToAction("Index");
              
        }



        //Excel tablosuna aktarma
        public IActionResult ExcelAktar()
        {
            var gelenevrak = c.GelenEvraks.Include(x=>x.Birim).Include(x=>x.User).ToList();
            using (var workbook=new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("GelenEvrak");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "ID";        
                worksheet.Cell(currentRow, 2).Value = "Tarih";
                worksheet.Cell(currentRow, 3).Value = "Birim";
                worksheet.Cell(currentRow, 4).Value = "Gönderen";
                worksheet.Cell(currentRow, 5).Value = "Belge Tipi";
                worksheet.Cell(currentRow, 6).Value = "Açıklama";
                worksheet.Cell(currentRow, 7).Value = "Oluşturan Personel";
                
                #endregion

                #region Body
                foreach (var evrak in gelenevrak)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = evrak.GelenId;
                    worksheet.Cell(currentRow, 2).Value = evrak.GelenTarih;
                    worksheet.Cell(currentRow, 3).Value = evrak.Birim.BirimAdi;
                    worksheet.Cell(currentRow, 4).Value = evrak.GelenGonderen;
                    worksheet.Cell(currentRow, 5).Value = evrak.GelenCins;
                    worksheet.Cell(currentRow, 6).Value = evrak.GelenKonu;
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
                        "GelenEvraklar.xlsx"
                        );
                }
            }
        }


        
        public IActionResult Detay(int id)
        {
            var model =c.GelenEvraks.Include(x=>x.User).Include(x=>x.Birim).First(x=>x.GelenId==id);            
            return View(model);
        }

    }
}
