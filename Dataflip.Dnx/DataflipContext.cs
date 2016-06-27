using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip
{
    public class DataflipContext
    {
        public DataflipSettings Settings { get; set; }
        public static event Action<DataflipConfigurationEventArgs> Configure = null;

        private void InvokeOnConfigure(DataflipConfigurationEventArgs e)
        {
            if (e.Settings == null)
                e.Settings = new DataflipSettings();

            if (Configure != null)
                Configure(e);

            Settings = e.Settings;
        }

        public DataflipContext(DataflipSettings settings)
        {
            Settings = settings;    
        }

        public DataflipContext(string contextName)
        {
            DataflipConfigurationEventArgs e = new DataflipConfigurationEventArgs();
            e.ContextName = contextName;
            InvokeOnConfigure(e);

            if (Settings == null)
            {
                throw new DataflipSettingsException(
                    $"The settings haven't been configured for the '{contextName}' context yet. The following line (or something that performs a similar operation) is required before instantiating this class: \n\n" 
                    +
                    "DataflipContext.Configure += (df) => df.Settings.ConnectionString = \"{connectionString goes here}\";"
                );
            }
        }
    }
}
