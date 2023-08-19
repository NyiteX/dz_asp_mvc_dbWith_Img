using dz_asp_mvc_db.Classes;
using dz_asp_mvc_db.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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
        public IActionResult Index()
        {
            List<UserModel> users = _context.Users.ToList(); // Получите список пользователей из базы данных
            return View(users);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View("Registration");
        }

        [HttpPost]
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

            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
