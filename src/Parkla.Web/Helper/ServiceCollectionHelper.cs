using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Parkla.Business.Abstract;
using Parkla.Business.Concrete;
using Parkla.Core.DTOs;
using Parkla.Core.Entities;
using Parkla.Core.Validators;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Concrete;

namespace Parkla.Web.Helper;
public class ServiceCollectionHelper
{
    public static void AddDependencies<TContext>(IServiceCollection s) 
        where TContext: DbContext, new()
    {
        //----------------------------------------------------------------------------------- REPOSITORIES
        s.AddSingleton<ICityRepo, CityRepo<TContext>>();
        s.AddSingleton<IDistrictRepo, DistrictRepo<TContext>>();
        s.AddSingleton<IParkAreaRepo, ParkAreaRepo<TContext>>();
        s.AddSingleton<IParkRepo, ParkRepo<TContext>>();
        s.AddSingleton<IParkSpaceRepo, ParkSpaceRepo<TContext>>();
        s.AddSingleton<IPricingRepo, PricingRepo<TContext>>();
        s.AddSingleton<IRealParkSpaceRepo, RealParkSpaceRepo<TContext>>();
        s.AddSingleton<IReceivedSpaceStatusRepo, ReceivedSpaceStatusRepo<TContext>>();
        s.AddSingleton<IReservationRepo, ReservationRepo<TContext>>();
        s.AddSingleton<IUserRepo, UserRepo<TContext>>();

        //----------------------------------------------------------------------------------- SERVICES
        s.AddSingleton<ICollectorService, CollectorService>();
        s.AddSingleton<ICityService, CityService>();
        s.AddSingleton<IDistrictService, DistrictService>();
        s.AddSingleton<IParkAreaService, ParkAreaService>();
        s.AddSingleton<IParkService, ParkService>();
        s.AddSingleton<IParkSpaceService, ParkSpaceService>();
        s.AddSingleton<IPricingService, PricingService>();
        s.AddSingleton<IRealParkSpaceService, RealParkSpaceService>();
        s.AddSingleton<IReceivedSpaceStatusService, ReceivedSpaceStatusService>();
        s.AddSingleton<IReservationService, ReservationService>();
        s.AddSingleton<IAuthService, AuthService>();
        s.AddSingleton<IUserService, UserService>();
        s.AddSingleton<IMailService, MailService>();
        s.AddSingleton<IParklaHubService, ParklaHubService>();
        
        //----------------------------------------------------------------------------------- VALIDATORS
        s.AddTransient<IValidator<ParkSpaceStatusDto>, ParkSpaceStatusValidator>();
        s.AddTransient<IValidator<City>, CityValidator>();
        s.AddTransient<IValidator<District>, DistrictValidator>();
        s.AddTransient<IValidator<ParkArea>, ParkAreaValidator>();
        s.AddTransient<IValidator<Park>, ParkValidator>();
        s.AddTransient<IValidator<ParkSpace>, ParkSpaceValidator>();
        s.AddTransient<IValidator<Pricing>, PricingValidator>();
        s.AddTransient<IValidator<RealParkSpace>, RealParkSpaceValidator>();
        s.AddTransient<IValidator<ReceivedSpaceStatus>, ReceivedSpaceStatusValidator>();
        s.AddTransient<IValidator<Reservation>, ReservationValidator>();
        s.AddTransient<IValidator<User>, UserValidator>();
    }
}