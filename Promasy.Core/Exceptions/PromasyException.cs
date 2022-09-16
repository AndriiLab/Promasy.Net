using System;

namespace Promasy.Core.Exceptions;

public abstract class PromasyException : Exception
{
    protected PromasyException(string message) : base(message)
    {
    }
}