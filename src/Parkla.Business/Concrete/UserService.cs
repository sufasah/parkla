using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class UserService : EntityServiceBase<User>, IUserService
{
    public UserService(
        IUserRepo userRepo, 
        IValidator<User> validator
    ) : base(userRepo, validator)
    {
    }
}