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
    public class ReleasesController : Controller
    {
        private readonly JwtAuthenticator _authenticator;

        private readonly LoggerBundle _logger = new LoggerBundle
        {
            Exception = new ExceptionFileLogger(null),
            Information = new InformationFileLogger(null) // todo callback
        };

        public ReleasesController(IConfiguration configuration)
        {
            _authenticator = new JwtAuthenticator(configuration["jwt:secret"]);
        }

        [HttpGet("api/v1/auth/releases")]
        public IActionResult GetAll([FromQuery(Name = "ps")] Int32 pageSize = 50,
            [FromQuery(Name = "p")] Int32 page = 0)
        {
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

        [HttpGet("api/v1/auth/releases/{id}")]
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

            return Ok(new MusicBrainzRelease()); // todo load from db
        }

        [HttpGet("api/v1/auth/releases/{id}/records")]
        public IActionResult GetRecordsById(Int32? id, [FromQuery(Name = "ps")] Int32 pageSize = 50,
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

            return Ok(new List<IMusicBrainzRecord>
            {
                new MusicBrainzRecord(),
                new MusicBrainzRecord(),
                new MusicBrainzRecord()
            }); // todo load from db
        }

        [HttpGet("api/v1/auth/releases/{id}/artists")]
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

        [HttpGet("api/v1/auth/releases/search/{query}")]
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

            return Ok(new List<IMusicBrainzRelease>
            {
                new MusicBrainzRelease(),
                new MusicBrainzRelease(),
                new MusicBrainzRelease()
            }); // todo load from db
        }
    }
}