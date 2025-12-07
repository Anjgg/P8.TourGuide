using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPricer;

public class Provider
{
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public Guid TripId { get; set; }
}
