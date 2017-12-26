using System;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.extensions;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
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

        [HttpGet("auth/tracks/{id}")]
        public IActionResult GetById(Int32? id)
        {
            try
            {
                if (!IsAuthorized(out IActionResult result))
                {
                    return result;
                }

                // validate
                if (id == null)
                {
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = NewDataContext())
                {
                    Track track = dc.SetTracks.AsNoTracking().FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == track)
                    {
                        // no matching record found
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    return Ok(track.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}