using AspNetCoreGeneratedDocument;
using BaseIntroductionDotNetMentoring.Data;
using BaseIntroductionDotNetMentoring.Helpers;
using BaseIntroductionDotNetMentoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BaseIntroductionDotNetMentoring.Controllers
{
    public class CatalogController : Controller
    {
        private readonly NorthwindContext _db;
        private readonly ProductSettings _settings;
        private readonly ILogger<CatalogController> _logger;
        protected CategoryActivities _categories;

        public CatalogController(NorthwindContext db, IOptions<ProductSettings> settings, ILogger<CatalogController> logger)
        {
            _db = db;
            _settings = settings.Value;
            _logger = logger;
            _categories = new CategoryActivities(_db);
        }


        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Categories()
        {
            var categories = await _db.Categories
                .Select(c => new { c.CategoryID, c.CategoryName, c.Description })
                .ToListAsync();
            _logger.LogInformation("Retrieved {Count} categories", categories.Count);
            return View(categories);
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Products()
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .OrderBy(p => p.ProductId)
                .AsQueryable();

            if (_settings.MaxProducts > 0)
                query = query.Take(_settings.MaxProducts);

            var products = await query.ToListAsync();
            _logger.LogInformation("Retrieved {Count} products (MaxProducts={Max})", products.Count, _settings.MaxProducts);
            return View(products);
        }
        
        // GET: /Catalog/Create        
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            _logger.LogInformation("Dropdowns are loaded for suppliers & categories");
            return View(new ProductInput());
        }

        // POST: /Catalog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInput input)
        {
            _logger.LogInformation("Validating categories");
            int validateCategoryResult = _categories.ValidateCategoryId(input);
            if(validateCategoryResult == 0)
                ModelState.AddModelError(nameof(input.CategoryId), "Category is required.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create validation failed with {ErrorCount} errors", ModelState.ErrorCount);
                await LoadDropdownsAsync();
                return View(input);
            }

            var product = new Models.Product { Discontinued = false };
            ProductInput.ApplyInput(product, input);
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Product created: {Name} (Id={Id})", product.ProductName, product.ProductId);
            return RedirectToAction(nameof(Products));
        }

        // GET: /Catalog/Edit/5
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null)
            {
                _logger.LogWarning("Product {Id} not found for edit", id);
                return NotFound();
            }

            await LoadDropdownsAsync();
            return View(new ProductInput
            {
                Id = p.ProductId,
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
                StockQuantity = p.UnitsInStock ?? 0,
                CategoryId = p.CategoryId ?? 0,
                SupplierId = p.SupplierId
            });
        }

        // POST: /Catalog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductInput input)
        {
            if (id != input.Id)
            {
                _logger.LogError("ID mismatch: route={RouteId}, input={InputId}", id, input.Id);
                return BadRequest();
            }

            int validateCategoryResult = _categories.ValidateCategoryId(input);
            if (validateCategoryResult == 0)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Category is required.");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit validation failed with {ErrorCount} errors for Product {Id}", ModelState.ErrorCount, id);
                await LoadDropdownsAsync();
                return View(input);
            }

            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogError("Product {Id} not found for update", id);
                return NotFound();
            }

            _logger.LogInformation("Validating products");
            ProductInput.ApplyInput(product, input);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Product updated: {Name} (Id={Id})", product.ProductName, product.ProductId);
            return RedirectToAction(nameof(Products));
        }

        private async Task LoadDropdownsAsync()
        {
            var categoriesTask = _db.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            var suppliersTask = _db.Suppliers.OrderBy(s => s.CompanyName).ToListAsync();
            await Task.WhenAll(categoriesTask, suppliersTask);
            ViewBag.Categories = categoriesTask.Result;
            ViewBag.Suppliers = suppliersTask.Result;
        }
    }
}
