using System;
using System.IO;
using ch.wuerth.tobias.mux.Core.exceptions;
using ch.wuerth.tobias.mux.Core.io;
using ch.wuerth.tobias.mux.Core.logging;

namespace ch.wuerth.tobias.mux.API
{
    public static class Configurator
    {
        public static T RequestConfig<T>(String configPath, LoggerBundle logger = null) where T : class
        {
            if (!File.Exists(configPath))
            {
                logger?.Information?.Log($"File '{configPath}' not found. Trying to create it...");
                FileInterface.Save(Activator.CreateInstance<T>(), configPath, false, logger);
                logger?.Information?.Log($"Successfully created file '{configPath}'");
                logger?.Information?.Log($"Please adjust the newly created file '{configPath}' as needed and run again");
                throw new ProcessAbortedException();
            }

            (T output, Boolean success) readResult = FileInterface.Read<T>(configPath, logger);
            if (!readResult.success)
            {
                throw new ProcessAbortedException();
            }

            logger?.Information?.Log($"Successfully read configuration file '{configPath}'");
            return readResult.output;
        }
    }
}