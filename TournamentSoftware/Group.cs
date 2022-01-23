using System.Collections.Generic;

namespace TournamentSoftware
{
    public class Group
    {
        public NominationWrapper NominationWrapper { get; set; }
        public List<Category> Categories { get; set; }
        public Group() { }
        public Group(NominationWrapper nomination, ParticipantWrapper participant)
        {
            NominationWrapper = nomination;
            Categories = new List<Category>() { new Category(participant) };
        }

        public bool IsCategoryExists(string categoryName)
        {
            return Categories.Exists(category => category.Name.Equals(categoryName));
        }

        public void AddCategory(string categoryName)
        {
            if (!IsCategoryExists(categoryName))
            {
                Categories.Add(new Category(categoryName));
            }
        }

        public void AddParticipantToCategory(ParticipantWrapper participant)
        {
            string categoryName = participant.Category;
            AddCategory(categoryName);
            Categories.Find(category => category.Name.Equals(categoryName)).AddParticipant(participant);
        }

        public Category GetCategory(string categoryName)
        {
            return Categories.Find(category => category.Name.Equals(categoryName));
        }

        public List<string> GetCategoryNames()
        {
            List<string> categories = new List<string>();
            foreach (Category category in Categories)
            {
                if (!categories.Contains(category.Name))
                {
                    categories.Add(category.Name);
                }
            }
            return categories;
        }

        public Subgroup GetSubgroupByCategory(string categoryName, string subgroupName)
        {
            return GetCategory(categoryName).GetSubgroupByName(subgroupName);
        }
    }
}
