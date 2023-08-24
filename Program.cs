using dz_asp_mvc_db.Classes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//db
string connection = @"Data Source = WIN-U669V8L9R5E; Initial Catalog = UserASP; Trusted_Connection=True; TrustServerCertificate = True";
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

//печеньки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => 
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.LoginPath = "/Home/Login";
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");
/*pattern: "{controller=Home}/{action=Registration}/{id?}");*/

app.Run();
