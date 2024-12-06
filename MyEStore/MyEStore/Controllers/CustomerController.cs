using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEStore.Entities;
using MyEStore.Models;
using System.Security.Claims;

namespace MyEStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly MyeStoreContext _ctx;

        public CustomerController(MyeStoreContext ctx)
        {
            _ctx = ctx;
        }

        public FileResult ExportCSV()
        {
            string fileName = "customer.csv";
            List<string> rows = new List<string>();
            rows.Add("Email Address,First Name,Last Name,Address,Phone Number");

            var dskh = _ctx.KhachHangs.Select(p => $"\"{p.Email}\",\"{p.HoTen}\",\"{p.HoTen}\",\"{p.DiaChi}\",\"{p.DienThoai}\"").ToList();
            rows.AddRange(dskh);

            byte[] fileBytes = rows.SelectMany(s =>
           System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();

            return File(fileBytes, "text/csv", fileName);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            var kh = _ctx.KhachHangs.SingleOrDefault(p => p.MaKh == model.UserName && p.MatKhau == model.Password);
            if (kh == null)
            {
                ViewBag.ThongBao = "Sai thông tin đăng nhập";
                return View();
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, kh.HoTen),
                new Claim(ClaimTypes.Email, kh.Email),
                new Claim("UserId", kh.MaKh),
                //nên làm động (đọc từ CSDL)
                new Claim(ClaimTypes.Role, "Administrator"),
            };
            var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimPrincipal);
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Profile", "Customer");

        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

            [Authorize]
        public ActionResult PurchaseOrder()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
