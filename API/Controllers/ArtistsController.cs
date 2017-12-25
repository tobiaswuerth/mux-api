using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ch.wuerth.tobias.mux.API.extensions;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
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

        [HttpGet("auth/artists/{id}")]
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
                    return StatusCode((Int32)HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = NewDataContext())
                {
                    MusicBrainzArtist artist = dc.SetArtists.AsNoTracking()
                        .Include(x => x.MusicBrainzArtistMusicBrainzAliases).ThenInclude(x => x.MusicBrainzAlias)
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == artist)
                    {
                        // no matching record found
                        return StatusCode((Int32)HttpStatusCode.NotFound);
                    }

                    return Ok(artist.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }


}
