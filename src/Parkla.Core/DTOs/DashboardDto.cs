using Parkla.Core.Entities;

namespace Parkla.Core.DTOs
{
    public class DashboardDto
    {
        public List<Park> TopPopularParks { get; set; }
        public long TotalParks { get; set; }
        public double? MinParkPrice { get; set; }
        public double? AverageParkPrice { get; set; }
        public double? MaxParkPrice { get; set; }
        public long TotalEmptySpaces { get; set; }
        public long TotalOccupiedSpaces { get; set; }
        public long TotalAreas { get; set; }
        public long TotalReservationEnabledAreas { get; set; }
        public long TotalSpaces { get; set; }
        public long TotalReservedSpaces { get; set; }
        public long TotalReservations { get; set; }
        public double TotalEarning { get; set; }
        public List<User> MostActiveUsers { get; set; }
        public double? MinStatusDataTransferDelayInSeconds { get; set; }
        public double? AverageStatusDataTransferDelayInSeconds { get; set; }
        public double? MaxStatusDataTransferDelayInSeconds { get; set; }
        public long TotalCarsUsedSpaces { get; set; }
        public List<TimeSeriesData> TotalEarningPerDay { get; set; }
        public List<TimeSeriesData> CarCountUsedSpacePerDay { get; set; }
        public List<TimeSeriesData> SpaceUsageTimePerDay { get; set; }
        public List<List<double>> SpaceUsageTimePercentagesPerWeekday { get; set; }
        public DashboardDto(DateTime? timeSeriesStart = null)
        {
            TopPopularParks = new List<Park>();
            MostActiveUsers = new List<User>();

            TotalEarningPerDay = new List<TimeSeriesData>();
            CarCountUsedSpacePerDay = new List<TimeSeriesData>();
            SpaceUsageTimePerDay = new List<TimeSeriesData>();

            if(timeSeriesStart != null) {
                var iter = timeSeriesStart.Value.Date;
                var end = DateTime.UtcNow.Date;
                while(iter <= end) {
                    TotalEarningPerDay.Add(new(iter, 0));
                    CarCountUsedSpacePerDay.Add(new(iter, 0));
                    SpaceUsageTimePerDay.Add(new(iter, new MinAvgMaxSum()));
                    iter = iter.AddDays(1);
                }
            }

            SpaceUsageTimePercentagesPerWeekday = new List<List<double>>();
            
            for(var i = 0; i < 24; i++) SpaceUsageTimePercentagesPerWeekday.Add(new());
            SpaceUsageTimePercentagesPerWeekday.ForEach(x => {
                for(var i = 0; i < 7; i++) x.Add(0);
            });
        }
    }

    public class TimeSeriesData {
        public DateTime X { get; set; }
        public object Y { get; set; }

        public TimeSeriesData(DateTime x, object y)
        {
            X = x;
            Y = y;
        }
    }

    public class MinAvgMaxSum {
        public double Min { get; set; } = double.MaxValue;
        public double Avg { get; set; } = 0;
        public double Max { get; set; } = double.MinValue;
        public double Sum { get; set; } = 0;
        public double Count { get; set; } = 0;
    }
}