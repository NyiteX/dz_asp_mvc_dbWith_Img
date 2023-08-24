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
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<ProductModel> products = _context.Products.ToList();
            return View(products);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(string? search)
        {
            var products = _context.Products.Where(u => u.Name.Contains(search)).ToList();

            if (products == null)
            {
                return View();
            }
            return View(products);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Users()
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
            await Login_method(name, password);

            return RedirectToAction("Index", "Account");
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
        public async Task<IActionResult> Registration(string name, string password, string email)
        {
            byte[] pic;
            string imagePath = "Pictures/2.ico";
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                pic = new byte[fs.Length];
                await fs.ReadAsync(pic, 0, pic.Length);
            }

            UserModel user = new UserModel(Id: null, Login: name, Password: HashClass.ToSHA256(password),
                                            Email: email, Pic: pic);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreateProduct()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct(string Name, string Discription, double Price, long Count, IFormFile Image)
        {
            if (Image != null && Image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Image.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    ProductModel product = new ProductModel(Id: null, Name, Discription, Price, Count, imageBytes);
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", "Home");
        }


        async Task Login_method(string name, string password)
        {
            bool f = _context.Users.Any(user => user.Login == name && user.Password == HashClass.ToSHA256(password));
            if (!f)
            {
                ModelState.AddModelError("", "Wrong login or password.");
                return;
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
        }
    }
}
