using Microsoft.AspNetCore.Mvc;
using MyEStore.Entities;
using MyEStore.Models;

namespace MyEStore.Controllers
{
    public class CartController : Controller
    {
        private readonly MyeStoreContext _ctx;

        public CartController(MyeStoreContext ctx) { _ctx = ctx; }

        public const string CART_NAME = "CART";
        public List<CartItem> CartItems
        {
            get
            {
                var carts = HttpContext.Session.Get<List<CartItem>>(CART_NAME) ?? new List<CartItem>();
                return carts;
            }
        }

        public IActionResult Index()
        {
            return View(CartItems);
        }

        public IActionResult AddToCart(int id, int qty = 1)
        {
            var cart = CartItems;
            //kiểm tra xem có ko?
            var cartItem = cart.SingleOrDefault(p => p.MaHh == id);
            if (cartItem != null)
            {
                cartItem.SoLuong += qty;
            }
            else
            {
                var hangHoa = _ctx.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["ThongBao"] = $"Tìm không thấy sản phẩm có mã {id}";
                    return RedirectToAction("Index", "Product");
                }
                cartItem = new CartItem
                {
                    MaHh = id,
                    SoLuong = qty,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh
                };
                cart.Add(cartItem);
            }
            //set session
            HttpContext.Session.Set<List<CartItem>>(CART_NAME, cart);
            return RedirectToAction("Index");
        }
    }
}
