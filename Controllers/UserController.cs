using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.Authentication;
using System.Web;
using System.Net.Mail;
using System.Net;
using ClosedXML.Excel;
using System.IO;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        
        Context c =new Context();
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly RoleManager<AppRole> _roleManager;
        
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;            
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Index(string kullaniciAdi, string adSoyad,string eposta)
        {
            var degerler = _userManager.Users.ToList();

            if (!string.IsNullOrEmpty(kullaniciAdi))
            {
                degerler = degerler.Where(x => x.UserName.ToLower().ToString().Contains(kullaniciAdi.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(adSoyad))
            {
                degerler = degerler.Where(x => x.NameSurname.ToLower().ToString().Contains(adSoyad.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(eposta))
            {
                degerler = degerler.Where(x => x.Email.ToLower().ToString().Contains(eposta.ToLower())).ToList();
            }
            
            return View(degerler);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignIn(AppUserViewModel appUserViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new()
                {
                    UserName = appUserViewModel.UserName,  
                    NameSurname=appUserViewModel.NameSurname,
                    Email = appUserViewModel.Email,
                    PhoneNumber = appUserViewModel.PhoneNumber,
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, appUserViewModel.Sifre);
                if (result.Succeeded)      
                    return RedirectToAction("Login");
                else
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //İlgili kullanıcıya dair önceden oluşturulmuş bir Cookie varsa siliyoruz.
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.Persistent, model.Lock);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user); //Önceki hataları girişler neticesinde +1 arttırılmış tüm değerleri 0(sıfır)a çekiyoruz.

                        if (string.IsNullOrEmpty(TempData["returnUrl"] != null ? TempData["returnUrl"].ToString() : ""))
                            return RedirectToAction("Index","Home");  
                        return Redirect(TempData["returnUrl"].ToString());
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(user); //Eğer ki başarısız bir account girişi söz konusu ise AccessFailedCount kolonundaki değer +1 arttırılacaktır. 

                        int failcount = await _userManager.GetAccessFailedCountAsync(user); //Kullanıcının yapmış olduğu başarısız giriş deneme adedini alıyoruz.
                        if (failcount == 3)
                        {
                            await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(1))); //Eğer ki başarısız giriş denemesi 3'ü bulduysa ilgili kullanıcının hesabını kilitliyoruz.
                            ModelState.AddModelError("Locked", "Art arda 3 başarısız giriş denemesi yaptığınızdan dolayı hesabınız 1 dk kitlenmiştir.");
                        }
                        else
                        {
                            if (result.IsLockedOut)
                                ModelState.AddModelError("Locked", "Art arda 3 başarısız giriş denemesi yaptığınızdan dolayı hesabınız 1 dk kilitlenmiştir.");
                            else
                                ModelState.AddModelError("NotUser2", "E-posta veya şifre yanlış.");
                        }

                    }
                }
                else
                {
                    ModelState.AddModelError("NotUser", "Böyle bir kullanıcı bulunmamaktadır.");
                    ModelState.AddModelError("NotUser2", "E-posta veya şifre yanlış.");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [AllowAnonymous]
        public IActionResult PasswordReset()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordReset(ResetPasswordViewModel model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.To.Add(user.Email);
                mail.From = new MailAddress("info.evraktakipsistemi@gmail.com", "Evrak Takip Sistemi", System.Text.Encoding.UTF8);
                mail.Subject = "Şifre Güncelleme Talebi";
                mail.Body = $"<a target=\"_blank\" href=\"https://evraktakipsistemi.somee.com{Url.Action("UpdatePassword", "User", new { userId = user.Id, token = resetToken })}\">Yeni şifre talebi için tıklayınız</a>";
                mail.IsBodyHtml = true;
                SmtpClient smp = new SmtpClient();
                smp.Credentials = new NetworkCredential("info.evraktakipsistemi@gmail.com", "emircan41");
                smp.Port = 587;
                smp.Host = "smtp.gmail.com";
                smp.EnableSsl = true;
                smp.Send(mail);

                ViewBag.State = true;
            }
            else
                ViewBag.State = false;

            return View();
        }

        // Şifre güncelleme
        [AllowAnonymous]
        [HttpGet("[action]/{userId}/{token}")]
        public IActionResult UpdatePassword(string userId, string token)
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost("[action]/{userId}/{token}")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model, string userId, string token)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(token), model.Password);
            if (result.Succeeded)
            {
                ViewBag.State = true;
                await _userManager.UpdateSecurityStampAsync(user);
            }
            else
                ViewBag.State = false;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DeleteProfile(string id)
        {

            AppUser user = await _userManager.FindByIdAsync(id);
            if (ModelState.IsValid)
            {
                
                if (user.Id == 1 & _userManager.GetUserId(HttpContext.User) == id) //admin hesabını silmeye calısan kişi adminse hata verir
                {                                                                 //eğer öyleyse adminin profil linkini döndürür.             

                    TempData["ErrMsg"] = "Ana Kullanıcı(admin) silinemez";
                    return Redirect("/User/Profile/" + user.UserName);
                }
                else if (user.UserName == User.Identity.Name)//kendisini sildiğine emin oluyoruz 
                {
                    IdentityResult result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignOutAsync();
                        //Başarılı...
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect("/User/Profile/" + user);
                }
            }
            else {return RedirectToAction("Page", "Authority"); }
            
        }



        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            List<string> userRoles = await _userManager.GetRolesAsync(user) as List<string>; //Seçilen personelin rollerini listeliyoruz.
            if (user.Id == 1)// kuruluşta id'si 1 oldugu icin admin hesabı silinemez
            {               
                TempData["ErrorMsg"] = "Ana Kullanıcı(admin) silinemez";
                return RedirectToAction("index");
            }
            else if (user.UserName == User.Identity.Name)
            {
                TempData["ErrorMsg"] = "Kendinizi buradan silemezsiniz. Hesabınızı silmek için profilinize gidiniz.";
                return RedirectToAction("index");
            }           
            else
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    //Başarılı...
                }
                return RedirectToAction("index");
            }
        }

        [Authorize]
        public async Task<IActionResult> Profile(string id) //parametre adı "id" fakat username değeri taşıyor.
        {
            if(User.Identity.Name==id) //link ile başka kişinin profiline erişmesini engelliyoruz.
            {                         //id parametresi ile gelen Name bilgisi, giriş yapanın name bilgisi ile aynıysa getir. 
                var user = await _userManager.FindByNameAsync(id);
                var model = new AppUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NameSurname = user.NameSurname,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Page", "Authority");
            }
            
            
        }


        // PERSONEL BİLGİ GETİRME
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if(id=="1")
            {
                TempData["ErrorMsg"] = "Ana Kullanıcı(admin) düzenlenemez";
                return RedirectToAction("Index", "User");
            }
            if (_userManager.GetUserId(HttpContext.User)==id || User.IsInRole("Admin")) //mevcut kullanıcı sadece kendisini bilgilerini degisebilir
            {                                                                         //ya da admin rolündeyse güncelleme yapabilir                
                var user = await _userManager.FindByIdAsync(id);         
                var model = new AppUserViewModel
                {
                    UserName = user.UserName,
                    NameSurname = user.NameSurname,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Page", "Authority");
            }
            
        }
        // PERSONEL BİLGİ GÜNCELLEME
        [HttpPost]
        public async Task<IActionResult> EditUser(AppUserViewModel model)
        { 
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            user.UserName = model.UserName;
            user.NameSurname = model.NameSurname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            
            var result = await _userManager.UpdateAsync(user);      
            if (result.Succeeded && User.Identity.Name==user.UserName) //kendi bilgilerini değişirse oturumu düsürüyoruz.
            {
                await _userManager.UpdateSecurityStampAsync(user); // veritabanındaki kritik değişikliklerden dolayı
                await _signInManager.SignOutAsync();               //kullanıcı oturumunu düşürüp yeniden açıyoruz.
                await _signInManager.SignInAsync(user, true);
                return RedirectToAction("Index","Home");
            }
            else if(result.Succeeded)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }    
        }


        [HttpGet]
        public  IActionResult EditPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (await _userManager.CheckPasswordAsync(user, model.OldPassword))
                {
                    IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (!result.Succeeded)
                    {
                        result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                        return View(model);
                    }
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, true);
                    
                }
            }     
            return RedirectToAction("Index","Home");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult ExcelAktar()
        {
            var users = _userManager.Users.ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Kullanıcı Adı";
                worksheet.Cell(currentRow, 3).Value = "Ad Soyad";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "Telefon NO";                
                #endregion

                #region Body
                foreach (var x in users)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = x.Id;
                    worksheet.Cell(currentRow, 2).Value = x.UserName;
                    worksheet.Cell(currentRow, 3).Value = x.NameSurname;
                    worksheet.Cell(currentRow, 4).Value = x.Email;
                    worksheet.Cell(currentRow, 5).Value = x.PhoneNumber;                                        
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Users.xlsx"
                        );
                }
            }
        }

    }
}
