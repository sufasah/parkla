using Parkla.Core.DTOs;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract;

public interface ICollectorRepo {
    Task<Tuple<bool, ParkSpace?, Park?>> CollectParkSpaceStatusAsync(
        ParkSpaceStatusDto dto
    );
}