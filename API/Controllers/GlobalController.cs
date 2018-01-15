using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class GlobalController : DataController
    {
        public GlobalController(IConfiguration configuration) : base(configuration) { }
    }
}