using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.logging.exception;
using ch.wuerth.tobias.mux.Core.logging.information;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class SourceController : Controller
    {
        private readonly JwtAuthenticator _authenticator;

        private readonly LoggerBundle _logger = new LoggerBundle
        {
            Exception = new ExceptionFileLogger(null),
            Information = new InformationFileLogger(null) // todo callback
        };

        public SourceController(IConfiguration configuration)
        {
            _authenticator = new JwtAuthenticator(configuration["jwt:secret"]);
        }

        [HttpGet("api/v1/auth/source/{id}")]
        public IActionResult GetById(Int32 id)
        {
            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
            if (!authRes.success)
            {
                return StatusCode((Int32)HttpStatusCode.Unauthorized);
            }

            return Ok("NOT IMPLEMENTED YET");
        }
    }
}
