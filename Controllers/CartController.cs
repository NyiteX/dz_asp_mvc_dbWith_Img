using dz_asp_mvc_db.Classes;
using dz_asp_mvc_db.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace dz_asp_mvc_db.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        public CartController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetUser_From_CookieAsync();
            //userid
            var cartItems = await _context.Cart
                                .Where(card => card.UserId == user.Id)
                                .ToListAsync();

            //productid
            var productIdsInCart = cartItems.Select(item => item.ProductId).ToList();

            //count in cart item
            var countsInCart = cartItems.Select(item => item.Count).ToList();

            //sorted products by userid and productid
            var productsInCart = await _context.Products
                                          .Where(product => productIdsInCart.Contains(product.Id))
                                          .ToListAsync();

            /*return View(productsInCart);*/
            var cartItemViewModels = new List<CartViewModel>();

            for (int i = 0; i < productsInCart.Count; i++)
            {
                var viewModel = new CartViewModel(productsInCart[i], countsInCart[i], Convert.ToInt64(productsInCart[i].Price * countsInCart[i]));
                cartItemViewModels.Add(viewModel);
            }

            return View(cartItemViewModels);
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
