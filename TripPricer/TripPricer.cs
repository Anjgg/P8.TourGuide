using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPricer.Helpers;

namespace TripPricer;

public class TripPricer
{
    Dictionary<int, string> _providerNames = new()
        {
            {1, "Holiday Travels"},
            {2, "Enterprize Ventures Limited"},
            {3, "Sunny Days"},
            {4, "FlyAway Trips"},
            {5, "United Partners Vacations"},
            {6, "Dream Trips"},
            {7, "Live Free"},
            {8, "Dancing Waves Cruselines and Partners"},
            {9, "AdventureCo"},
            {10, "Cure-Your-Blues"}
        };

    public Task<List<Provider>> GetPriceAsync(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints)
    {
        List<Provider> providers = new List<Provider>();
        HashSet<string> providersUsed = new HashSet<string>();
        var dictionary2 = _providerNames;

        for (int i = 0; i < 10; i++)
        {
            int multiple = ThreadLocalRandom.Current.Next(100, 700);
            double childrenDiscount = children / 3.0;
            double price = multiple * adults + multiple * childrenDiscount * nightsStay + 0.99 - rewardsPoints;

            if (price < 0.0)
            {
                price = 0.0;
            }

            string provider;
            do
            {
                provider = GetProviderName(dictionary2);
            } while (providersUsed.Contains(provider));

            providersUsed.Add(provider);
            providers.Add(new Provider 
            {
                Name = provider, 
                TripId = attractionId,
                Price = price });
        }
        return Task.FromResult(providers);
    }

    public string GetProviderName(Dictionary<int, string> dictionary)
    {

        int multiple = ThreadLocalRandom.Current.Next(1, 10);
        if (!dictionary.ContainsKey(multiple))
        {
            do
            {
                if (multiple == 10)
                    multiple = 1;
                else
                    multiple += 1;
            } while (!dictionary.ContainsKey(multiple));
        }
        var providerName = dictionary[multiple];
        dictionary.Remove(multiple);
        return providerName;
    }
}
