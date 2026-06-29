using System.ComponentModel.DataAnnotations;

namespace BaseIntroductionDotNetMentoring.Helpers
{
    public class ProductInput
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be 3-100 characters")]
        public string? Name { get; set; }                       // Rule 1: required + min length

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1_000_000, ErrorMessage = "Price must be at least 0.01")]
        public decimal Price { get; set; }                      // Rule 2: range

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be 0 or greater")]
        public int StockQuantity { get; set; }                  // Rule 3: non-negative

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }                     // Rule 4: required for dropdown

        // Optional supplier selection
        public int? SupplierId { get; set; }

        [StringLength(200, ErrorMessage = "Supplier name max 200 characters")]
        public string? SupplierName { get; set; }

        public static void ApplyInput(Models.Product product, ProductInput input)
        {
            product.ProductName = input.Name!;
            product.UnitPrice = input.Price;
            product.UnitsInStock = (short?)input.StockQuantity;
            product.CategoryId = input.CategoryId;
            product.SupplierId = input.SupplierId;
        }
    }
}
