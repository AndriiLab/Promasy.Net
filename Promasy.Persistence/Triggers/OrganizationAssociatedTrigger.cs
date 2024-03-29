﻿using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Persistence.Context;

namespace Promasy.Persistence.Triggers;

public class OrganizationAssociatedTrigger : IBeforeSaveTrigger<IOrganizationAssociated>
{
    private readonly PromasyContext _context;
    private readonly IUserContext _userContext;

    public OrganizationAssociatedTrigger(PromasyContext context, IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }
    
    public Task BeforeSave(ITriggerContext<IOrganizationAssociated> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added)
        {
            var entry = _context.Entry(context.Entity);
            if (entry.Entity.OrganizationId < 1)
            {
                entry.Entity.OrganizationId = _userContext.GetOrganizationId(); 
            }
        }
        
        return Task.CompletedTask;
    }
}