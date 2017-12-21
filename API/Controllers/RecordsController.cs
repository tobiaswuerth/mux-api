using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.logging.exception;
using ch.wuerth.tobias.mux.Core.logging.information;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            List<MusicBrainzRecord> musicBrainzRecords = new List<MusicBrainzRecord>
            {
                new MusicBrainzRecord
                {
                    Title = "Test",
                    Length = 12345,
                    Disambiguation = "the real test",
                    UniqueId = 1337,
                    Video = false
                }
            }; // load from db
            return Ok(musicBrainzRecords);
        }

        [HttpGet("api/v1/auth/records/{id}")]
        public IActionResult GetById(Int32 id)
        {
            return Ok("NOT IMPLEMENTED YET");
        }

        [HttpGet("api/v1/auth/records/{id}/files")]
        public IActionResult GetFilesById(Int32 id)
        {
            return Ok("NOT IMPLEMENTED YET");
        }

        [HttpGet("api/v1/auth/records/{id}/releases")]
        public IActionResult GetReleasesById(Int32 id)
        {
            return Ok("NOT IMPLEMENTED YET");
        }

        [HttpGet("api/v1/auth/records/{id}/artists")]
        public IActionResult GetArtistsById(Int32 id)
        {
            return Ok("NOT IMPLEMENTED YET");
        }

        [HttpGet("api/v1/auth/records/search/{query}")]
        public IActionResult GetBySearchQuery(String query)
        {
            return Ok("NOT IMPLEMENTED YET");
        }
    }
}