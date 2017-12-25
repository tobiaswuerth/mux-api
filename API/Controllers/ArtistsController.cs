using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class ArtistsController : DataController
    {
        public ArtistsController(IConfiguration configuration) : base(configuration) { }
    }
}
