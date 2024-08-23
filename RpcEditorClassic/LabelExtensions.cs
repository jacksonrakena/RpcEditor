using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui;

namespace RpcEditorClassic
{
    public static class LabelExtensions
    {
        public static void SetText(this Label label, string newText, bool refreshApplication = true)
        {
            label.Clear();
            label.Text = newText;
            if (refreshApplication) Application.Refresh();
        }
    }
}
