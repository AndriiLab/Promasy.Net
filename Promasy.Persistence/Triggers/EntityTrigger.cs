﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Persistence.Context;

namespace Promasy.Persistence.Triggers;

public class EntityTrigger : IBeforeSaveTrigger<IBaseEntity>
{
    private readonly PromasyContext _context;
    private readonly IUserContext _userContext;

    public EntityTrigger(PromasyContext context, IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }
    
    public Task BeforeSave(ITriggerContext<IBaseEntity> context, CancellationToken cancellationToken)
    {
        
        var entry = _context.Entry(context.Entity);
        switch (context.ChangeType)
        {
            case ChangeType.Added:
                entry.Entity.CreatedDate = DateTime.UtcNow;
                entry.Entity.CreatorId = _userContext.GetId();
                break;
            case ChangeType.Modified:
            case ChangeType.Deleted:
                entry.Entity.ModifiedDate = DateTime.UtcNow;
                entry.Entity.ModifierId = _userContext.GetId();
                break;
            default:
                break;
        }
        
        return Task.CompletedTask;
    }
}