﻿using FluentValidation.Results;

namespace Store.Core.Messages
{
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; } = null!;

        public Command()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool EhValido();
    }
}