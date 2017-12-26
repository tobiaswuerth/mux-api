using System;
using System.Linq;
using ch.wuerth.tobias.mux.API.extensions;
using ch.wuerth.tobias.mux.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class TracksController : DataController
    {
        public TracksController(IConfiguration configuration) : base(configuration) { }

        [HttpGet("auth/tracks")]
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
                    return Ok(dc.SetTracks.AsNoTracking().Skip(page * pageSize).Take(pageSize)
                        .Select(x => x.ToJsonDictionary()).ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}