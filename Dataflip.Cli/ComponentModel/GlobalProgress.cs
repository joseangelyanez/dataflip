using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip.Cli.ComponentModel
{
    public class GlobalProgress
    {
        public static event Action<GlobalProgressNotification> Notify = null;

        internal static void NotifyProgress(string progress)
        {
            if (Notify != null)
                Notify(new GlobalProgressNotification() { Text = progress, Color = ConsoleColor.Green });
        }

        internal static void NotifyProgress(GlobalProgressNotification notification)
        {
            if (Notify != null)
                Notify(notification);
        }
    }
}
