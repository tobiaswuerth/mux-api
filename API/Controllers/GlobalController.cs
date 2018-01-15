using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class GlobalController : DataController
    {
        public GlobalController(IConfiguration configuration) : base(configuration) { }
    }
}
