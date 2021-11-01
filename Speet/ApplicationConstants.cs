namespace Speet
{
    public abstract class ApplicationConstants
    {
        //Database cleanup
        public const string DatabaseCleanUpTime = "00:00:00";
        public const int SportGroupsExpirationDays = 90;

        //Pagination
        public const int SportGroupsPerPage = 5;
        public const int MaxShownPreviousPages = 2;
        public const int MaxShownNextPages = 2;

        //Filter
        public const int MinSportGroupParticipants = 2;
        public const int MaxSportGroupParticipants = 20;
        public const int MinShownSportGroupDistance = 1;
        public const int MaxShownSportGroupDistance = 250;

        //CreateSportGroup
        public const int MaxGroupNameLength = 75;
    }
}
