using System.Collections.Generic;

namespace Speet.Models
{
    public class PaginationInfo
    {
        public int CurrentPageIndex { get; private set; }
        public int[] PreviousPageIndexes { get; private set; }
        public int[] NextPageIndexes { get; private set; }

        public PaginationInfo(int pageIndex, int totalNumberOfGroups)
        {
            int totalPages = GetTotalPages(totalNumberOfGroups);
            CurrentPageIndex = pageIndex;
            PreviousPageIndexes = GetPreviousPageIndexes(totalPages);
            NextPageIndexes = GetNextPageIndexes(totalPages);
        }

        private int GetTotalPages(int numberOfGroups)
        {
            int totalPages = numberOfGroups / ApplicationConstants.SportGroupsPerPage;
            if ((numberOfGroups % ApplicationConstants.SportGroupsPerPage) > 0)
                totalPages++;

            return totalPages;
        }

        private int[] GetPreviousPageIndexes(int totalPages)
        {
            List<int> indexes = new List<int>();
            for (int i = (CurrentPageIndex-ApplicationConstants.MaxShownPreviousPages); i < CurrentPageIndex; i++)
                if(i > 0)
                    indexes.Add(i);

            return indexes.ToArray();
        }

        private int[] GetNextPageIndexes(int totalPages)
        {
            List<int> indexes = new List<int>();
            for (int i = (CurrentPageIndex+1); i <= (CurrentPageIndex + ApplicationConstants.MaxShownNextPages); i++)
                if(i <= totalPages)
                indexes.Add(i);

            return indexes.ToArray();
        }
    }
}
