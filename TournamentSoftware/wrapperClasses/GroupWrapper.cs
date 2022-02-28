using System.Collections.Generic;

namespace TournamentSoftware
{
    public class GroupWrapper
    {
        public NominationWrapper NominationWrapper { get; set; }
        public List<CategoryWrapper> Categories { get; set; }
        public GroupWrapper() 
        {
            NominationWrapper = new NominationWrapper();
            Categories = new List<CategoryWrapper>();
        }
        public GroupWrapper(NominationWrapper nomination, ParticipantWrapper participant)
        {
            NominationWrapper = nomination;
            Categories = new List<CategoryWrapper>() { new CategoryWrapper(participant) };
        }

        public bool IsCategoryExists(string categoryName)
        {
            return Categories.Exists(category => category.Category.Name.Equals(categoryName));
        }

        public void AddCategory(string categoryName)
        {
            if (!IsCategoryExists(categoryName))
            {
                Categories.Add(new CategoryWrapper(categoryName));
            }
        }

        public void AddParticipantToCategory(ParticipantWrapper participant)
        {
            string categoryName = participant.Category;
            AddCategory(categoryName);
            Categories.Find(category => category.Category.Name.Equals(categoryName)).AddParticipant(participant);
        }

        public CategoryWrapper GetCategory(string categoryName)
        {
            return Categories.Find(category => category.Category.Name.Equals(categoryName));
        }

        public List<string> GetCategoryNames()
        {
            List<string> categories = new List<string>();
            foreach (CategoryWrapper category in Categories)
            {
                if (!categories.Contains(category.Category.Name))
                {
                    categories.Add(category.Category.Name);
                }
            }
            return categories;
        }

        public SubgroupWrapper GetSubgroupByCategory(string categoryName, string subgroupName)
        {
            return GetCategory(categoryName).GetSubgroupByName(subgroupName);
        }
    }
}
