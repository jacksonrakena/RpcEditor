using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui;

namespace RpcEditor.Views
{
    public class InputSetWithValidation : InputSet
    {
        private readonly static Terminal.Gui.Attribute _errorColor = Terminal.Gui.Attribute.Make(Color.Red, Color.Black);

        private readonly static Terminal.Gui.Attribute _successColor = Terminal.Gui.Attribute.Make(Color.Green, Color.Black);

        private readonly Predicate<string> _validator;
        private readonly string _errorText;
        private readonly string _successText;
        public FixedLengthLabel ErrorText { get; }

        public InputSetWithValidation(string name, string errorText, string successText, Predicate<string> validator, int? valueWidth = null) : base(name, valueWidth)
        {
            _validator = validator;
            _errorText = errorText;
            _successText = successText;
            ErrorText = new FixedLengthLabel(errorText)
            {
                X = Pos.Right(Value),
                Width = errorText.Length,
                TextColor = _errorColor
            };

            Value.Changed += Value_Changed;

            Add(ErrorText);

            Width = Dim.Width(Name) + Dim.Width(Value) + Dim.Width(ErrorText);
            Height = Name.Height;
        }

        public bool IsValid()
        {
            return _validator(Value.Text.ToString());
        }

        private void Value_Changed(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                ErrorText.TextColor = _errorColor;
                ErrorText.SetText(_errorText);
            } else
            {
                ErrorText.TextColor = _successColor;
                ErrorText.SetText(_successText);
            }
        }
    }
}
