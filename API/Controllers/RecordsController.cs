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
    public class RecordsController : DataController
    {
        [ HttpGet("auth/records/{id}/aliases") ]
        public IActionResult GetAliasesById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetAliasesById");
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
                        .FromSql(RecordQuery.GET_ALIASES_BY_ID, id)
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

        [ HttpGet("auth/records") ]
        public IActionResult GetAll([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetAll");
                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetMusicBrainzRecords.AsNoTracking()
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

        [ HttpGet("auth/records/{id}/artists") ]
        public IActionResult GetArtistsById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetArtistsById");
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
                        .FromSql(RecordQuery.GET_ARTISTS_BY_ID, id)
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

        [ HttpGet("auth/records/{id}") ]
        public IActionResult GetById(Int32? id)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetById");
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
                    MusicBrainzRecord record = dc.SetMusicBrainzRecords.AsNoTracking()
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == record)
                    {
                        LoggerBundle.Trace($"No record found for given id '{id}'");
                        // no matching record found
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    return Ok(record.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/records/lookup/{query}") ]
        public IActionResult GetByLookupQuery(String query
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetByLookupQuery");
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
                    return Ok(dc.SetMusicBrainzRecords.AsNoTracking()
                        .Where(x => null != x.Title)
                        .Where(x => x.Title.Equals(query))
                        .OrderByDescending(x => x.UniqueId)
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

        [ HttpGet("auth/records/search/{query}") ]
        public IActionResult GetBySearchQuery(String query
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetBySearchQuery");
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
                    return Ok(dc.SetMusicBrainzRecords.AsNoTracking()
                        .Where(x => null != x.Title)
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

        [ HttpGet("auth/records/{id}/releases") ]
        public IActionResult GetReleasesById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetReleasesById");
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
                        .FromSql(RecordQuery.GET_RELEASES_BY_ID, id)
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

        [ HttpGet("auth/records/{id}/tracks") ]
        public IActionResult GetTracksById(Int32? id
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on RecordsController.GetTracksById");
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
                    return Ok(dc.SetAcoustIdResults.AsNoTracking()
                        .FromSql(RecordQuery.GET_TRACKS_BY_ID, id)
                        .Include(x => x.Track)
                        .OrderByDescending(x => x.Score)
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
    }
}