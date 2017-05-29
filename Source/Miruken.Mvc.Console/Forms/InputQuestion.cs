namespace Miruken.Mvc.Console
{
    using System;
    using System.Linq;
    using FluentValidation;

    public class Question: Input<string>
    {
        private readonly string _value;

        public Question(string text, Action<string> selected)
        {
            Text            = text;
            _selectedAction = selected;
        }

        public Question(string text, string value, Action<string> selected)
        {
            Text            = text;
            _value          = value;
            _selectedAction = selected;
        }

        public override void Handle(Buffer buffer, int longestText, InputLocation inputLocation)
        {
            var value = string.IsNullOrEmpty(_value)
                ? buffer.Prompt(Text.PadRight(longestText), inputLocation)
                : buffer.Edit(Text.PadRight(longestText), _value, inputLocation);

            var result = Validator.Validate(value);

            if (result.IsValid)
            {
                _selectedAction?.Invoke(value);
                buffer.WriteLine($"? {Text.PadRight(longestText)} {value}");
                Window.Update();
            }
            else
            {
                var message = result.Errors.FirstOrDefault()?.ToString();
                buffer.Warn(message, inputLocation);
                Handle(buffer, longestText, inputLocation);
            }
        }

        public Question Required(string message = null)
        {
            Validator.RuleFor(x => x)
                .NotEmpty()
                .WithMessage(message ?? ValidationMessage);
            return this;
        }

        public Question Number(string message = null)
        {
            Validator.RuleFor(x => x)
                .Must(x =>
                {
                    int val;
                    return int.TryParse(x, out val);
                })
                .WithMessage(message ?? "Must be a number.");
            return this;
        }

        public Question Date(string message = null)
        {
            Validator.RuleFor(x => x)
                .Must(x =>
                {
                    DateTime val;
                    return DateTime.TryParse(x, out val);
                })
                .WithMessage(message ?? "Must be a valid date.");
            return this;
        }

        public Question Must(Func<string, bool> validationRule, string message)
        {
            Validator.RuleFor(x => x)
                .Must(validationRule)
                .WithMessage(message ?? ValidationMessage);
            return this;
        }
    }
}