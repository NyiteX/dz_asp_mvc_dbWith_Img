using dz_asp_mvc_db.Classes;
using dz_asp_mvc_db.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Linq;

namespace dz_asp_mvc_db.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        public AccountController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetUser_From_CookieAsync();

            return View(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            UserModel userTmp = await GetUser_From_CookieAsync();

            return View(userTmp);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(string? name, string? password, string? email, IFormFile image)
        {
            UserModel userTmp = await GetUser_From_CookieAsync();
            if(name != null){   userTmp.Login = name;   }
            if(password != null) { userTmp.Password = HashClass.ToSHA256(password); }
            if(email != null) { userTmp.Email = email; }
            if (image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    userTmp.Pic = imageBytes;
                }
            }

            _context.Users.Update(userTmp);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            UserModel userTmp = await GetUser_From_CookieAsync();
            _context.RemoveRange(userTmp);
            await _context.SaveChangesAsync();


            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Buy(int? count, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return RedirectToAction("Index", "Home");

            var user = await GetUser_From_CookieAsync();

            var editproduct = await _context.Cart.Where(u => u.ProductId == productId).FirstOrDefaultAsync();
            if (editproduct != null)
            {
                editproduct.Count += count;
                _context.Cart.Update(editproduct);
            }
            else
            {
                var cartmodel = new CartModel(null, user.Id, productId, count);
                _context.Cart.Add(cartmodel);
            }
            
            product.Count -= count;
            _context.Products.Update(product);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }





        public async Task<UserModel> GetUser_From_CookieAsync()
        {
            var context = HttpContext;
            var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            string? name, password;
            var auth = authResult.Principal;

            var nameClaim = auth.FindFirst(ClaimTypes.Name);
            var emailClaim = auth.FindFirst(ClaimTypes.Email);

            name = nameClaim.Value;
            password = emailClaim.Value;

            return _context.Users.FirstOrDefault(u => u.Login == name && u.Password == HashClass.ToSHA256(password));
        }
    }
}
