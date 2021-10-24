using System.Collections.Generic;

namespace Speet.Models.ContainerModels
{
    public class MyGroupsContainer
    {
        public List<SportGroup> GroupsToDisplay { get; set; }
        public User UserToDisplay { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
