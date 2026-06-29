using BaseIntroductionDotNetMentoring.Data;

namespace BaseIntroductionDotNetMentoring.Helpers
{
    public class CategoryActivities
    {
        private readonly NorthwindContext _db;
        public CategoryActivities() { }

        public CategoryActivities(NorthwindContext db)
        {
            _db = db;
        }        

        public int ValidateCategoryId(ProductInput input)
        {
            if (input.CategoryId == 0)
                return 0;
            else
                return 1;
        }
    }
}
