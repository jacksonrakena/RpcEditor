using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui;

namespace RpcEditor
{
    public static class Utilities
    {
        public static bool IsUInt64(this string value)
        {
            return ulong.TryParse(value, out _);
        }

        public static bool IsDiscordId(this string value)
        {
            return value.IsUInt64() && value.Length >= 17;
        }

        public static void RefreshApplication(this Action actor)
        {
            actor();
            Application.Refresh();
        }
    }
}
