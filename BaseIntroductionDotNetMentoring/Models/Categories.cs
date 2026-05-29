namespace BaseIntroductionDotNetMentoring.Models
{
    public class Categories
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        // Persist binary image data
        public byte[]? Picture { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
