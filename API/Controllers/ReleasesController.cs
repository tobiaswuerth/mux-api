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
    public class ReleasesController : DataController
    {
        [ HttpGet("auth/releases/{id}/aliases") ]
        public IActionResult GetAliasesById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetAliasesById");
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
                    return Ok(dc.SetAliases.AsNoTracking()
                        .FromSql(ReleaseQuery.GET_ALIASES_BY_ID, id)
                        .OrderBy(x => x.Name)
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

        [ HttpGet("auth/releases") ]
        public IActionResult GetAll([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetAll");
                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetReleases.AsNoTracking()
                        .Where(x => null != x.Title)
                        .Select(x => x.Title)
                        .OrderBy(x => x)
                        .Distinct()
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Select(x => new Dictionary<String, Object>
                        {
                            {
                                "Title", x
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

        [ HttpGet("auth/releases/{id}/artists") ]
        public IActionResult GetArtistsById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetArtistsById");
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
                    return Ok(dc.SetArtistCredits.AsNoTracking()
                        .FromSql(ReleaseQuery.GET_ARTISTS_BY_ID, id)
                        .Include(x => x.Artist)
                        .ThenInclude(x => x.MusicBrainzArtistMusicBrainzAliases)
                        .ThenInclude(x => x.MusicBrainzAlias)
                        .OrderBy(x => x.Name)
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

        [ HttpGet("auth/releases/{id}") ]
        public IActionResult GetById(Int32? id)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetById");
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
                    MusicBrainzRelease release = dc.SetReleases.AsNoTracking()
                        .Include(x => x.TextRepresentation)
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == release)
                    {
                        LoggerBundle.Trace($"No release found for given id '{id}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    return Ok(release.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/releases/lookup/{query}") ]
        public IActionResult GetByLookupQuery(String query
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetByLookupQuery");
                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // verify
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
                    return Ok(dc.SetReleases.AsNoTracking()
                        .Where(x => x.Title.Equals(query))
                        .Include(x => x.TextRepresentation)
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

        [ HttpGet("auth/releases/search/{query}") ]
        public IActionResult GetBySearchQuery(String query
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetBySearchQuery");
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
                    return Ok(dc.SetReleases.AsNoTracking()
                        .Where(x => x.Title.Contains(query))
                        .Select(x => x.Title)
                        .OrderBy(x => x)
                        .Distinct()
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Select(x => new Dictionary<String, Object>
                        {
                            {
                                "Title", x
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

        [ HttpGet("auth/releases/{id}/events") ]
        public IActionResult GetEventsById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetEventsById");
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
                    return Ok(dc.SetReleaseEvents.AsNoTracking()
                        .FromSql(ReleaseQuery.GET_RELEASE_EVENTS_BY_ID, id)
                        .Include(x => x.Area)
                        .ThenInclude(x => x.MusicBrainzIsoCodeMusicBrainzAreas)
                        .ThenInclude(x => x.MusicBrainzIsoCode)
                        .OrderByDescending(x => x.Date)
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

        [ HttpGet("auth/releases/{id}/records") ]
        public IActionResult GetRecordsById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on ReleasesController.GetRecordsById");
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
                        .FromSql(ReleaseQuery.GET_RECORDS_BY_ID, id)
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