using dz_asp_mvc_db.Classes;
using dz_asp_mvc_db.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace dz_asp_mvc_db.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            List<UserModel> users = _context.Users.ToList();
            return View(users);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string name, string password)
        {
            bool f = _context.Users.Any(user => user.Login == name && user.Password == HashClass.ToSHA256(password));
            if (!f)
            {
                ModelState.AddModelError("", "Wrong login or password.");
                return View();
            }

           /* int? userId = _context.Users
                        .Where(user => user.Login == name && user.Password == HashClass.ToSHA256(password))
                        .Select(user => user.Id)
                        .FirstOrDefault();*/

            var claims = new List<Claim>
            {
                /*new Claim(ClaimTypes.NameIdentifier, userId.ToString()),*/
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, password)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Registration()
        {
            return View("Registration");
        }

        [HttpPost]
        /*public async Task<IActionResult> Registration(string name, string password, string email)*/
        public IActionResult Registration(string name, string password, string email)
        {
            byte[] pic;
            string imagePath = "Pictures/2.ico";
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                pic = new byte[fs.Length];
                fs.Read(pic, 0, pic.Length);
            }

            UserModel user = new UserModel(Id: null, Login: name, Password: HashClass.ToSHA256(password),
                                            Email: email, Pic: pic);

            _context.Users.Add(user);
            _context.SaveChanges();

            return View("Index", _context.Users.ToList());          
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
