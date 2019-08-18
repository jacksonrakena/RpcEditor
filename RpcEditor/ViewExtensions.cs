using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui;

namespace RpcEditor
{
    public static class ViewExtensions
    {
        public static View WithSubviews(this View view, params View[] subviews) 
        {
            view.Add(subviews);
            return view;
        }
    }
}
