using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ch.wuerth.tobias.mux.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class ArtistsController : DataController
    {
        public ArtistsController(IConfiguration configuration) : base(configuration) { }
        
        [HttpGet("auth/artists")]
        public IActionResult GetAll([FromQuery(Name = "ps")] Int32 pageSize = 50,
            [FromQuery(Name = "p")] Int32 page = 0)
        {
            try
            {
                if (!IsAuthorized(out IActionResult result))
                {
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = NewDataContext())
                {
                    return Ok(dc.SetArtists.AsNoTracking().Select(x => x.Name)
                        .OrderBy(x => x).Distinct().Skip(page * pageSize).Take(pageSize)
                        .Select(x => new Dictionary<String, Object> { { "Name", x } }).ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
