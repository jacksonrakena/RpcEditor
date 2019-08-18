using System;
using System.Collections.Generic;
using System.Text;
using NStack;
using Terminal.Gui;

namespace RpcEditor.Views
{
    public class FixedLengthLabel : Label
    {
        public FixedLengthLabel(ustring text) : base(text)
        {
            Width = text.Length;
        }
    }
}
