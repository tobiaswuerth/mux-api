using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.Controllers.queries;
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

        [ HttpGet("auth/artists") ]
        public IActionResult GetAll([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
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
                    return Ok(dc.SetArtists.AsNoTracking()
                        .Select(x => x.Name)
                        .OrderBy(x => x)
                        .Distinct()
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Select(x => new Dictionary<String, Object>
                        {
                            {
                                "Name", x
                            }
                        })
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/artists/{id}") ]
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
                    MusicBrainzArtist artist = dc.SetArtists.AsNoTracking().Include(x => x.MusicBrainzArtistMusicBrainzAliases).ThenInclude(x => x.MusicBrainzAlias).FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == artist)
                    {
                        // no matching record found
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    return Ok(artist.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/artists/{id}/records") ]
        public IActionResult GetRecordsById(Int32? id, [ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
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

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = NewDataContext())
                {
                    return Ok(dc.SetMusicBrainzRecords.AsNoTracking().FromSql(ArtistQuery.GET_RECORDS_BY_ID, id).Skip(pageSize * page).Take(pageSize).Select(x => x.ToJsonDictionary()).ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/artists/{id}/releases") ]
        public IActionResult GetReleasesById(Int32? id, [ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
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

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = NewDataContext())
                {
                    return Ok(dc.SetReleases.AsNoTracking().FromSql(ArtistQuery.GET_RELEASES_BY_ID, id).Include(x => x.TextRepresentation).Skip(pageSize * page).Take(pageSize).Select(x => x.ToJsonDictionary()).ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/artists/search/{query}") ]
        public IActionResult GetBySearchQuery(String query, [ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                if (!IsAuthorized(out IActionResult result))
                {
                    return result;
                }

                // validate
                if (String.IsNullOrWhiteSpace(query))
                {
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                NormalizePageSize(ref pageSize);
                query = query.Trim();

                // get data
                using (DataContext dc = NewDataContext())
                {
                    return Ok(dc.SetArtists.AsNoTracking()
                        .Where(x => x.Name.Contains(query))
                        .Select(x => x.Name)
                        .OrderBy(x => x)
                        .Distinct()
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Select(x => new Dictionary<String, Object>
                        {
                            {
                                "Name", x
                            }
                        })
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/artists/lookup/{query}") ]
        public IActionResult GetByLookupQuery(String query, [ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                if (!IsAuthorized(out IActionResult result))
                {
                    return result;
                }

                // validate
                if (String.IsNullOrWhiteSpace(query))
                {
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                NormalizePageSize(ref pageSize);
                query = query.Trim();

                // get data
                using (DataContext dc = NewDataContext())
                {
                    return Ok(dc.SetArtists.AsNoTracking().Where(x => x.Name.Equals(query)).Skip(page * pageSize).Include(x => x.MusicBrainzArtistMusicBrainzAliases).ThenInclude(x => x.MusicBrainzAlias).Take(pageSize).Select(x => x.ToJsonDictionary()).ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}