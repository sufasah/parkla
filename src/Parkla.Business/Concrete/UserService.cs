using System.Linq.Expressions;
using System.Net;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.DTOs;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class UserService : EntityServiceBase<User>, IUserService
{
    private readonly IUserRepo _userRepo;
    private readonly IValidator<User> _validator;

    public UserService(
        IUserRepo userRepo, 
        IValidator<User> validator
    ) : base(userRepo, validator)
    {
        _userRepo = userRepo;
        _validator = validator;
    }

    public async Task<DashboardDto> GetDashboardAsync(int id, CancellationToken cancellationToken) {
        return await _userRepo.GetDashboardAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<User?> LoadMoneyAsync(int id, float amount, CancellationToken cancellationToken)
    {
        return await _userRepo.LoadMoneyAsync(id, amount, cancellationToken).ConfigureAwait(false);
    }

    public override async Task<User> UpdateAsync(
        User user,
        CancellationToken cancellationToken = default
    ) {
        var result = await _validator.ValidateAsync(user, o => o.IncludeRuleSets("update","id"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.Errors.First().ToString(), HttpStatusCode.BadRequest);
        
        
        var newUser = await _userRepo.UpdateAsync(
            user,
            new Expression<Func<User,object?>>[]{
                x => x.Password,
                x => x.RefreshTokenSignature,
                x => x.VerificationCode,
                x => x.Wallet
            },
            true,
            cancellationToken
        ).ConfigureAwait(false);

        return newUser;
    }
}