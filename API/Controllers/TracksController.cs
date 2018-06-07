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
    public class TracksController : DataController
    {
        [ HttpGet("auth/tracks") ]
        public IActionResult GetAll([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on TracksController.GetAll");
                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetTracks.AsNoTracking()
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

        [ HttpGet("auth/tracks/{id}") ]
        public IActionResult GetById(Int32? id)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on TracksController.GetById");
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
                    Track track = dc.SetTracks.AsNoTracking().FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null != track)
                    {
                        return Ok(track.ToJsonDictionary());
                    }

                    LoggerBundle.Trace($"No track found for given id '{id}'");
                    return StatusCode((Int32) HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/tracks/search/{query}") ]
        public IActionResult GetBySearchQuery(String query
            , [ FromQuery(Name = "ps") ] Int32 pageSize = 50
            , [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on TracksController.GetBySearchQuery");
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
                query = $"%{query}%";

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetTracks.AsNoTracking()
                        .FromSql(TrackQuery.GET_TRACKS_LIKE_PATH, query)
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

        [ HttpGet("auth/tracks/{id}/records") ]
        public IActionResult GetRecordsById(Int32? id)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on TracksController.GetRecordsById");
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
                    Track track = dc.SetTracks.AsNoTracking()
                        .Include(x => x.AcoustIdResults)
                        .ThenInclude(x => x.AcoustId)
                        .ThenInclude(x => x.MusicBrainzRecordAcoustIds)
                        .ThenInclude(x => x.MusicBrainzRecord)
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == track)
                    {
                        LoggerBundle.Trace($"No track found for given id '{id}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    // todo might need optimization with a direct database sql query

                    // loop references and keep track of relevant stats
                    Dictionary<MusicBrainzRecord, (Int32 count, Double sum)> ret =
                        new Dictionary<MusicBrainzRecord, (Int32, Double)>();

                    track.AcoustIdResults.ForEach(x =>
                    {
                        Double score = x.Score;
                        x.AcoustId.MusicBrainzRecordAcoustIds.ForEach(y =>
                        {
                            MusicBrainzRecord record = y.MusicBrainzRecord;
                            if (!ret.ContainsKey(record))
                            {
                                ret.Add(record, (1, score));
                                return; // continue
                            }

                            ret[record] = (ret[record].count + 1, ret[record].sum + score);
                        });
                    });

                    return Ok(ret.Select(x => new Dictionary<String, Object>
                    {
                        {
                            "Score", x.Value.sum / x.Value.count
                        }
                        , // calculate match score (average)
                        {
                            "Record", x.Key.ToJsonDictionary()
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}