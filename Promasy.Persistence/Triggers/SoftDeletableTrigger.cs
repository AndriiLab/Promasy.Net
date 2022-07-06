using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using Promasy.Core.Persistence;
using Promasy.Persistence.Context;

namespace Promasy.Persistence.Triggers;

public class SoftDeletableTrigger : IBeforeSaveTrigger<ISoftDeletable>
{
    private readonly PromasyContext _context;

    public SoftDeletableTrigger(PromasyContext context)
    {
        _context = context;
    }

    public Task BeforeSave(ITriggerContext<ISoftDeletable> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Deleted)
        {
            var entry = _context.Entry(context.Entity);
            context.Entity.Deleted = true;
            entry.State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }
}