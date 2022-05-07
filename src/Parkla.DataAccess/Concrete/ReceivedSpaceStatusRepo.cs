using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Context;

namespace Parkla.DataAccess.Concrete;

public class ReceivedSpaceStatusRepo<TContext> : EntityRepoBase<ReceivedSpaceStatus, TContext>, IReceivedSpaceStatusRepo 
    where TContext: DbContext, new()
{
    
}