﻿namespace ADF.Net.Core.Exceptions
{

    public class InvalidTransactionException : BaseApplicationException
    {
        public InvalidTransactionException(string message) : base(message) { }
    }
}