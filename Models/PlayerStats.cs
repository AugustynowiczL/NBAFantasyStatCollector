namespace NBAFantasy.Models
{
    public class PlayerStats
    {
        public required String PlayerID { get; set; }

        public required float AvgPoints { get; set; }

        public required float AvgRebounds { get; set; }

        public required float AvgAssists { get; set; }

        public required float FieldGoalPct { get; set; }
    }
}
