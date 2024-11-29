using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MyEStore.Entities;
using MyEStore.Models;

namespace MyEStore.Controllers
{
	public class ProductController : Controller
	{
		private readonly MyeStoreContext _ctx;

		public ProductController(MyeStoreContext ctx)
		{
			_ctx = ctx;
		}

		public IActionResult Detail(int id)
		{ 
			var data = _ctx.HangHoas.SingleOrDefault(p => p.MaHh == id);

			return View(data);
		}

		[HttpGet("san-pham/{slug}")]
        public IActionResult Details(string slug)
        {
            var data = _ctx.HangHoas.SingleOrDefault(p => p.TenAlias == slug);
            if (data == null)
            {
                return NotFound();
            }

            return View("Detail", data);
        }

        public IActionResult Index(int? category)
		{
			var data = _ctx.HangHoas.AsQueryable();

			if (category.HasValue)
			{
				data = data.Where(hh => hh.MaLoai == category.Value);
			}

			var result = data.Select(hh => new HangHoaVM
			{
				MaHh = hh.MaHh,
				TenHh = hh.TenHh,
				DonGia = hh.DonGia ?? 0,
				Hinh = hh.Hinh
			});
			return View(result);
		}
	}
}
