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
            _authenticator = new JwtAuthenticator(configuration["jwt:secret"]);
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
        public IActionResult GetById(Int32 id)
        {
            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }

            return Ok(new MusicBrainzRecord() as IMusicBrainzRecord); // todo load from db
        }

        [HttpGet("api/v1/auth/records/{id}/sources")]
        public IActionResult GetFilesById(Int32 id)
        {
            return Ok("NOT IMPLEMENTED YET");
        }

        [HttpGet("api/v1/auth/records/{id}/releases")]
        public IActionResult GetReleasesById(Int32 id)
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

        [HttpGet("api/v1/auth/records/{id}/artists")]
        public IActionResult GetArtistsById(Int32 id)
        {
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
        public IActionResult GetBySearchQuery(String query)
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
    }
}