using BaseIntroductionDotNetMentoring.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BaseIntroductionDotNetMentoring.Helpers;

namespace BaseIntroductionDotNetMentoring.Controllers
{
    public class CatalogController : Controller
    {
        private readonly NorthwindContext _db;
        private readonly ProductSettings _settings;

        public CatalogController(NorthwindContext db, IOptions<ProductSettings> settings)
        {
            _db = db;
            _settings = settings.Value;
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _db.Categories
                .Select(c => new { c.CategoryID, c.CategoryName, c.Description })
                .ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Products()
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .OrderBy(p => p.ProductId)
                .AsQueryable();

            if (_settings.MaxProducts > 0)
            {
                query = query.Take(_settings.MaxProducts);
            }

            var products = await query.ToListAsync();
            return View(products);
        }

        // GET: /Catalog/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Suppliers = await _db.Suppliers.OrderBy(s => s.CompanyName).ToListAsync();
            return View(new ProductInput());
        }

        // POST: /Catalog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInput input)
        {
            ViewBag.Categories = await _db.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Suppliers = await _db.Suppliers.OrderBy(s => s.CompanyName).ToListAsync();

            // Server-side validation rules
            if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length < 3)
            {
                ModelState.AddModelError(nameof(input.Name), "Name is required and must be at least 3 characters.");
            }
            if (input.Price <= 0)
            {
                ModelState.AddModelError(nameof(input.Price), "Price must be greater than zero.");
            }
            if (input.StockQuantity < 0)
            {
                ModelState.AddModelError(nameof(input.StockQuantity), "Stock must be 0 or greater.");
            }
            if (input.CategoryId == 0)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Category is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var product = new Models.Product
            {
                ProductName = input.Name!,
                UnitPrice = input.Price,
                UnitsInStock = (short?)input.StockQuantity,
                CategoryId = input.CategoryId == 0 ? null : input.CategoryId,
                SupplierId = input.SupplierId,
                QuantityPerUnit = null,
                Discontinued = false
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }

        // GET: /Catalog/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            ViewBag.Categories = await _db.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Suppliers = await _db.Suppliers.OrderBy(s => s.CompanyName).ToListAsync();

            var input = new ProductInput
            {
                Id = p.ProductId,
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
                StockQuantity = p.UnitsInStock ?? 0,
                CategoryId = p.CategoryId ?? 0,
                SupplierId = p.SupplierId
            };
            return View(input);
        }

        // POST: /Catalog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductInput input)
        {
            if (id != input.Id) return BadRequest();

            ViewBag.Categories = await _db.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Suppliers = await _db.Suppliers.OrderBy(s => s.CompanyName).ToListAsync();

            if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length < 3)
            {
                ModelState.AddModelError(nameof(input.Name), "Name is required and must be at least 3 characters.");
            }
            if (input.Price <= 0)
            {
                ModelState.AddModelError(nameof(input.Price), "Price must be greater than zero.");
            }
            if (input.StockQuantity < 0)
            {
                ModelState.AddModelError(nameof(input.StockQuantity), "Stock must be 0 or greater.");
            }
            if (input.CategoryId == 0)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Category is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.ProductName = input.Name!;
            product.UnitPrice = input.Price;
            product.UnitsInStock = (short?)input.StockQuantity;
            product.CategoryId = input.CategoryId == 0 ? null : input.CategoryId;
            product.SupplierId = input.SupplierId;

            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }
    }
}
