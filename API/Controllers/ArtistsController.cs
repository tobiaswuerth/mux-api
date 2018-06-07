using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.Controllers.queries;
using ch.wuerth.tobias.mux.API.extensions;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class ArtistsController : DataController
    {
        [ HttpGet("auth/artists") ]
        public IActionResult GetAll([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ArtistsController.GetAll");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
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
                LoggerBundle.Trace("Registered GET request on ArtistsController.GetById");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (id == null)
                {
                    LoggerBundle.Trace("Validation failed: id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    MusicBrainzArtist artist = dc.SetArtists.AsNoTracking()
                        .Include(x => x.MusicBrainzArtistMusicBrainzAliases)
                        .ThenInclude(x => x.MusicBrainzAlias)
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == artist)
                    {
                        LoggerBundle.Trace($"No artist found for given id '{id}'");
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

        [ HttpGet("auth/artists/lookup/{query}") ]
        public IActionResult GetByLookupQuery(String query
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ArtistsController.GetByLookupQuery");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (String.IsNullOrWhiteSpace(query))
                {
                    LoggerBundle.Trace("Validation failed: empty query");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                NormalizePageSize(ref pageSize);
                query = query.Trim();

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetArtists.AsNoTracking()
                        .Where(x => x.Name.Equals(query))
                        .Include(x => x.MusicBrainzArtistMusicBrainzAliases)
                        .ThenInclude(x => x.MusicBrainzAlias)
                        .OrderBy(x => x.UniqueId)
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Select(x => x.ToJsonDictionary())
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/artists/search/{query}") ]
        public IActionResult GetBySearchQuery(String query
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ArtistsController.GetBySearchQuery");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (String.IsNullOrWhiteSpace(query))
                {
                    LoggerBundle.Trace("Validation failed: empty query");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                NormalizePageSize(ref pageSize);
                query = query.Trim();

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
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

        [ HttpGet("auth/artists/{id}/records") ]
        public IActionResult GetRecordsById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ArtistsController.GetRecordsById");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (id == null)
                {
                    LoggerBundle.Trace("Validation failed: id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetMusicBrainzRecords.AsNoTracking()
                        .FromSql(ArtistQuery.GET_RECORDS_BY_ID, id)
                        .OrderBy(x => x.Title)
                        .Skip(pageSize * page)
                        .Take(pageSize)
                        .Select(x => x.ToJsonDictionary())
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/artists/{id}/releases") ]
        public IActionResult GetReleasesById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ArtistsController.GetReleasesById");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (id == null)
                {
                    LoggerBundle.Trace("Validation failed: id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetReleases.AsNoTracking()
                        .FromSql(ArtistQuery.GET_RELEASES_BY_ID, id)
                        .Include(x => x.TextRepresentation)
                        .OrderBy(x => x.Title)
                        .Skip(pageSize * page)
                        .Take(pageSize)
                        .Select(x => x.ToJsonDictionary())
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}