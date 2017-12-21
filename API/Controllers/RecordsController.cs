using System;
using System.Collections.Generic;
using System.Net;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.logging.exception;
using ch.wuerth.tobias.mux.Core.logging.information;
using ch.wuerth.tobias.mux.Core.models;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class RecordsController : Controller
    {
        private readonly JwtAuthenticator _authenticator;

        private readonly LoggerBundle _logger = new LoggerBundle
        {
            Exception = new ExceptionFileLogger(null),
            Information = new InformationFileLogger(null) // todo callback
        };

        public RecordsController(IConfiguration configuration)
        {
            _authenticator = new JwtAuthenticator(configuration[JwtConfig.JWT_SECRET_KEY]);
        }

        [HttpGet("api/v1/auth/records")]
        public IActionResult GetAll()
        {
            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }

            return Ok(new List<IMusicBrainzRecord>
            {
                new MusicBrainzRecord(),
                new MusicBrainzRecord(),
                new MusicBrainzRecord()
            }); // todo load from db
        }

        [HttpGet("api/v1/auth/records/{id}")]
        public IActionResult GetById(Int32? id, [FromQuery(Name = "ps")] Int32 pageSize = 50,
            [FromQuery(Name = "p")] Int32 page = 0)
        {
            if (id == null)
            {
                return StatusCode((Int32) HttpStatusCode.BadRequest);
            }

            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }

            return Ok(new MusicBrainzRecord()); // todo load from db
        }

        [HttpGet("api/v1/auth/records/{id}/tracks")]
        public IActionResult GetFilesById(Int32? id, [FromQuery(Name = "ps")] Int32 pageSize = 50,
            [FromQuery(Name = "p")] Int32 page = 0)
        {
            if (id == null)
            {
                return StatusCode((Int32) HttpStatusCode.BadRequest);
            }

            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }

            return Ok(new List<ITrack> {new Track(), new Track(), new Track()}); // todo load from db
        }

        [HttpGet("api/v1/auth/records/{id}/releases")]
        public IActionResult GetReleasesById(Int32? id, [FromQuery(Name = "ps")] Int32 pageSize = 50,
            [FromQuery(Name = "p")] Int32 page = 0)
        {
            if (id == null)
            {
                return StatusCode((Int32) HttpStatusCode.BadRequest);
            }

            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }

            return Ok(new List<IMusicBrainzRelease>
            {
                new MusicBrainzRelease(),
                new MusicBrainzRelease(),
                new MusicBrainzRelease()
            }); // todo load from db
        }

        [HttpGet("api/v1/auth/records/{id}/artists")]
        public IActionResult GetArtistsById(Int32? id, [FromQuery(Name = "ps")] Int32 pageSize = 50,
            [FromQuery(Name = "p")] Int32 page = 0)
        {
            if (id == null)
            {
                return StatusCode((Int32) HttpStatusCode.BadRequest);
            }

            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }

            return Ok(new List<IMusicBrainzArtist>
            {
                new MusicBrainzArtist(),
                new MusicBrainzArtist(),
                new MusicBrainzArtist()
            }); // todo load from db
        }

        [HttpGet("api/v1/auth/records/search/{query}")]
        public IActionResult GetBySearchQuery(String query, [FromQuery(Name = "ps")] Int32 pageSize = 50,
            [FromQuery(Name = "p")] Int32 page = 0)
        {
            if (query == null)
            {
                return StatusCode((Int32) HttpStatusCode.BadRequest);
            }

            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }

            return Ok(new List<IMusicBrainzRecord>
            {
                new MusicBrainzRecord(),
                new MusicBrainzRecord(),
                new MusicBrainzRecord()
            }); // todo load from db
        }
    }
}