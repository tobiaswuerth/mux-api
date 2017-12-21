using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.security;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.API.security.models;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.logging.exception;
using ch.wuerth.tobias.mux.Core.logging.information;
using ch.wuerth.tobias.mux.Core.processor;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly LoggerBundle _logger = new LoggerBundle
        {
            Exception = new ExceptionConsoleLogger(null), // todo
            Information = new InformationConsoleLogger(null) // todo
        };

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("api/v1/public/login")]
        public IActionResult Post([FromBody] AuthenticationModel values)
        {
            try
            {
                //validate data
                if (String.IsNullOrWhiteSpace(values?.Password) || String.IsNullOrWhiteSpace(values?.Username))
                {
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                // hash password
                (String output, Boolean success) pp = new PasswordProcessor().Handle(values.Password, _logger);

                if (!pp.success)
                {
                    return StatusCode((Int32) HttpStatusCode.InternalServerError);
                }

                // normalize username
                values.Username = values.Username.Trim();

                // check database for given username
                User user;
                using (DataContext dc = new DataContext(new DbContextOptions<DataContext>(), _logger))
                {
                    user = dc.SetUsers.AsNoTracking().FirstOrDefault(x =>
                        x.Username.Equals(values.Username, StringComparison.OrdinalIgnoreCase));
                }

                if (null == user)
                {
                    // no user found with given username
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                if (!user.Password.Equals(pp.output))
                {
                    // password incorrect
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                // prepare token generation
                String secret = _configuration["jwt:secret"];
                (JwtPayload output, Boolean success) jp = new JwtPayloadProcessor(secret).Handle(user, _logger);

                if (!jp.success)
                {
                    return StatusCode((Int32) HttpStatusCode.InternalServerError);
                }

                // generate token
                String token = JwtGenerator.GetToken(jp.output, secret);

                if (String.IsNullOrWhiteSpace(token))
                {
                    return StatusCode((Int32) HttpStatusCode.InternalServerError);
                }

                return Ok(new Dictionary<String, String> {{"token", token}});
            }
            catch (Exception ex)
            {
                _logger.Exception?.Log(ex);
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }
        }
    }
}