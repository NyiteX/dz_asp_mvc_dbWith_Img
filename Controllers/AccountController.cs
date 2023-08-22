using dz_asp_mvc_db.Classes;
using dz_asp_mvc_db.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var context = HttpContext;
            var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            string? name, password;
            var auth = authResult.Principal;

            var nameClaim = auth.FindFirst(ClaimTypes.Name);
            var emailClaim = auth.FindFirst(ClaimTypes.Email);

            name = nameClaim.Value;
            password = emailClaim.Value;

            var user = _context.Users.FirstOrDefault(u => u.Login == name && u.Password == HashClass.ToSHA256(password));

            return View(user);
        }
    }
}
