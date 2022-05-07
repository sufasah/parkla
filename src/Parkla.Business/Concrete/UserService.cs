using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class UserService : EntityServiceBase<User>
{
    public UserService(
        IEntityRepository<User> entityRepository, 
        IValidator<User> validator
    ) : base(entityRepository, validator)
    {
    }
}