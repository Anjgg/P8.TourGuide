using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public TourGuideController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    [HttpGet("getLocation")]
    public async Task<ActionResult<VisitedLocation>> GetLocation([FromQuery] string userName)
    {
        var user = await GetUserAsync(userName);
        if (user == null) 
        { 
            return NotFound($"User with username '{userName}' not found.");
        }

        var location = await _tourGuideService.GetUserLocationAsync(user);
        if (location == null) 
        {
            return NotFound($"Location for user '{userName}' not found.");
        }

        return Ok(location);
    }

    // TODO: Change this method to no longer return a List of Attractions.
    // Instead: Get the closest five tourist attractions to the user - no matter how far away they are.
    // Return a new JSON object that contains:
    // Name of Tourist attraction, 
    // Tourist attractions lat/long, 
    // The user's location lat/long, 
    // The distance in miles between the user's location and each of the attractions.
    // The reward points for visiting each Attraction.
    //    Note: Attraction reward points can be gathered from RewardsCentral
    [HttpGet("getNearbyAttractions")]
    public async Task<ActionResult<List<NearAttraction>>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = await GetUserAsync(userName);
        if (user == null)
        {
            return NotFound($"User with username '{userName}' not found.");
        }
        
        var attractions = await _tourGuideService.GetFiveNearbyAttractionsAsync(user);
        if (attractions == null)
        {
            return Problem($"Nearby attractions for user '{userName}' not found.");
        }

        return Ok(attractions);
    }

    [HttpGet("getRewards")]
    public async Task<ActionResult<List<UserReward>>> GetRewards([FromQuery] string userName)
    {
        var user = await GetUserAsync(userName);
        if (user == null)
        {
            return NotFound($"User with username '{userName}' not found.");
        }

        var rewards = _tourGuideService.GetUserRewards(user);

        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public async Task<ActionResult<List<Provider>>> GetTripDeals([FromQuery] string userName)
    {
        var user = await GetUserAsync(userName);
        if (user == null)
        {
            return NotFound($"User with username '{userName}' not found.");
        }

        var deals = await _tourGuideService.GetTripDealsAsync(user);

        return Ok(deals);
    }

    private async Task<User?> GetUserAsync(string userName)
    {
        return await _tourGuideService.GetUserAsync(userName);
    }
}
