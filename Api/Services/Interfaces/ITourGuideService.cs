using GpsUtil.Location;
using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Services.Interfaces
{
    public interface ITourGuideService
    {
        Tracker Tracker { get; }

        void AddUser(User user);
        Task<List<User>> GetAllUsersAsync();
        Task<List<NearAttraction>> GetFiveNearbyAttractionsAsync(User user);
        Task<List<Provider>> GetTripDealsAsync(User user);
        Task<User?> GetUserAsync(string userName);
        Task<VisitedLocation> GetUserLocationAsync(User user);
        List<UserReward> GetUserRewards(User user);
        Task<VisitedLocation> TrackUserLocationAsync(User user);
    }
}