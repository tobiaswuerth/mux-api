using System;
using ch.wuerth.tobias.mux.Core.logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ch.wuerth.tobias.mux.API
{
    public class Program
    {
        public static IWebHost BuildWebHost(String[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();
        }

        public static void Main(String[] args)
        {
            LoggerBundle.Trace("Starting...");
            BuildWebHost(args).Run();
        }
    }
}