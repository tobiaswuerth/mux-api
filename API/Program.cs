using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ch.wuerth.tobias.mux.API
{
    public class Program
    {
        public static void Main(String[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(String[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();
        }
    }
}