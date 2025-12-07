using GpsUtil.Location;

namespace TourGuide.Utilities
{
    public class NearAttraction
    {
        public string AttractionName { get; set; } = string.Empty;
        public double AttractionLatitude { get; set; }
        public double AttractionLongitude { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
        public double Distance { get; set; }
        public int RewardPoints { get; set; }
    }
}
