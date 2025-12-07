using GpsUtil.Location;
using System.Collections.Concurrent;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private static int count = 0;


    // Per-user locks to prevent concurrent reward calculations for the same user
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _userLocks = new();


    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral =rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    public async Task CalculateRewardsAsync(User user)
    {
        count++;
        var semaphore = _userLocks.GetOrAdd(user.UserId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            List<VisitedLocation> userLocations = user.VisitedLocations.ToList();
            List<Attraction> attractions = await _gpsUtil.GetAttractionsAsync();

            var rewardsToAdd = new List<UserReward>();

            foreach (var visitedLocation in userLocations)
            {
                foreach (var attraction in attractions)
                {
                    if (!user.UserRewards.Exists(r => r.Attraction.AttractionName == attraction.AttractionName))
                    {
                        if (NearAttraction(visitedLocation, attraction))
                        {
                            var points = await GetRewardPointsAsync(attraction, user);
                            rewardsToAdd.Add(new UserReward(visitedLocation, attraction, points));
                        }
                    }
                }
            }

            foreach (var reward in rewardsToAdd)
            {
                if (!user.UserRewards.Exists(r => r.Attraction.AttractionName == reward.Attraction.AttractionName))
                {
                    user.AddUserReward(reward);
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    public async Task<int> GetRewardPointsAsync(Attraction attraction, User user)
    {
        return await _rewardsCentral.GetAttractionRewardPointsAsync(attraction.AttractionId, user.UserId);
    }

    public double GetDistance(Locations loc1, Locations loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }
}
