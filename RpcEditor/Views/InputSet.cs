using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui;

namespace RpcEditor.Views
{
    public class InputSet : View
    {
        public FixedLengthLabel Name { get; }
        public TextField Value { get; }

        public InputSet(string name, int? valueWidth = null)
        {
            Name = new FixedLengthLabel(name);
            Value = new TextField("")
            {
                X = Pos.Right(Name)
            };
            if (valueWidth != null) Value.Width = valueWidth.Value;

            Add(Name, Value);

            Width = Dim.Width(Name) + Dim.Width(Value);
            Height = Name.Height;
        }
    }
}
