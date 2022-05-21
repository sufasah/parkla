using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class Pricing : IEntity {
    public Pricing()
    {
        Reservations = new HashSet<Reservation>();
    }

    public int? Id { get; set; }
    public int? AreaId { get; set; }
    public virtual ParkArea? Area { get; set; }
    public string? Type { get; set; }
    public TimeUnit? Unit { get; set; }
    public int? Amount { get; set; }
    public float? Price { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; }

    public static float GetPricePerHour(Pricing pricing) {
        var price = pricing.Price!.Value;
        var amount = pricing.Amount!.Value;

        return pricing.Unit switch
        {
            TimeUnit.MINUTE => price * 60 / amount,
            TimeUnit.DAY => price / 24 / amount,
            TimeUnit.MONTH => price / 720 / amount,
            _ => price / amount,
        };
    }

    public static Tuple<float?, float?, float?> FindMinAvgMax(ICollection<Pricing>? pricings) {
        if(pricings != null && pricings.Any()) {
            var minPrice = float.MaxValue;
            var maxPrice = float.MinValue;
            var avgPrice = pricings.Average(x => {
                var pricePerHour = GetPricePerHour(x);
                maxPrice = Math.Max(maxPrice, pricePerHour);
                minPrice = Math.Min(minPrice, pricePerHour);
                return pricePerHour;
            });
            
            return new(minPrice, avgPrice, maxPrice);
        }

        return new(null, null, null);
    }
}