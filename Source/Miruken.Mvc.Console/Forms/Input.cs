namespace Miruken.Mvc.Console
{
    using System;
    using FluentValidation;

    public abstract class Input
    {
        protected const string ValidationMessage = "Input in invalid.";

        protected Action<string> _selectedAction { get; set; }

        public string Text { get; set; }

        public abstract void Handle(Buffer buffer, int longestText, InputLocation inputLocation);
    }

    public abstract class Input<T> : Input
    {
        public AbstractValidator<T> Validator = new InlineValidator<T>();
    }
}