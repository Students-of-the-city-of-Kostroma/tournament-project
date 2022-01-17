using System.Collections.Generic;

namespace TournamentSoftware
{
    public class SubgroupingErrorLogger
    {
        private class ErrorsInGroup
        {
            private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            public ErrorsInGroup(Group group, string subgroup, List<string> errors)
            {
                Group = group;
                Subgroup = subgroup;
                Errors.Add(subgroup, errors);
            }
            public Group Group { get; set; }
            public string Subgroup { get; set; }
            public Dictionary<string, List<string>> Errors { get { return errors; } set { errors = value; } }

            public void AddError(string subgroup, string error)
            {
                if (!Errors.ContainsKey(subgroup))
                {
                    Errors.Add(subgroup, new List<string>() { error });
                }
                else
                {
                    if (!Errors[subgroup].Contains(error))
                    {
                        Errors[subgroup].Add(error);
                    }
                }
            }

            public List<string> GetErrorsBySubgroup(string subgroup)
            {
                if (Errors.ContainsKey(subgroup))
                {
                    return Errors[subgroup];
                }
                return new List<string>();
            }
        }

        private List<ErrorsInGroup> groupLogs = new List<ErrorsInGroup>();
        public void AddError(Group group, string subgroup, string error)
        {
            if (!IsGroupLogsExists(group))
            {
                groupLogs.Add(new ErrorsInGroup(group, subgroup, new List<string>()));
            }

            groupLogs.Find(logs => logs.Group.Equals(group)).AddError(subgroup, error);
        }

        public List<string> GetErrorsBySubgroup(Group group, string subgroup)
        {
            if (IsGroupLogsExists(group))
            {
                return groupLogs.Find(logs => logs.Group.Equals(group)).GetErrorsBySubgroup(subgroup);
            }
            return new List<string>();
        }

        private bool IsGroupLogsExists(Group group)
        {
            return groupLogs.Exists(logs => logs.Group.Equals(group));
        }
    }
}
