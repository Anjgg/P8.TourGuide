using RewardCentral.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RewardCentral;

public class RewardCentral
{
    public Task<int> GetAttractionRewardPoints(Guid attractionId, Guid userId)
    {
        int randomInt = new Random().Next(1, 1000);
        return Task.FromResult(randomInt);
    }
}
