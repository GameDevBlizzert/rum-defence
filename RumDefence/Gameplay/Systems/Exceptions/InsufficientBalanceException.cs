using System;

namespace RumDefence.Exceptions;

public class InsufficientBalanceException : Exception
{

    public InsufficientBalanceException() : base("The player does not have enough coins to perform this action.")
    {
    }

    public InsufficientBalanceException(string message) : base(message)
    {
    }

    public InsufficientBalanceException(string message, Exception innerException) : base(message, innerException)
    {
    }

}