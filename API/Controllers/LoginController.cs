using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly JwtAuthenticator _authenticator;
        private readonly Int32 _expirationShift;

        private readonly LoggerBundle _logger = new LoggerBundle
        {
            Exception = new ExceptionConsoleLogger(null), // todo
            Information = new InformationConsoleLogger(null) // todo
        };

        private readonly String _secret;

        public LoginController(IConfiguration configuration)
        {
            _secret = configuration[JwtConfig.JWT_SECRET_KEY];
            _expirationShift = Convert.ToInt32(configuration[JwtConfig.JWT_EXPIRATION_SHIFT_KEY]);
            _authenticator = new JwtAuthenticator(_secret);
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
                (JwtPayload output, Boolean success) jp =
                    new JwtPayloadProcessor(_secret, _expirationShift).Handle(user, _logger);

                return ProcessPayload(jp);
            }
            catch (Exception ex)
            {
                _logger.Exception?.Log(ex);
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }
        }

        private IActionResult ProcessPayload((JwtPayload output, Boolean success) jp)
        {
            if (!jp.success)
            {
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }

            // generate token
            String token = JwtGenerator.GetToken(jp.output, _secret);

            if (String.IsNullOrWhiteSpace(token))
            {
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }

            return Ok(new Dictionary<String, String> {{"token", token}});
        }

        [HttpGet("api/v1/public/login")]
        public IActionResult GetFreshToken()
        {
            try
            {
                (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, _logger);
                if (!authRes.success)
                {
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                (JwtPayload output, Boolean success) pp =
                    new JwtPayloadProcessor(_secret, _expirationShift).Handle(authRes.output, _logger);

                return ProcessPayload(pp);
            }
            catch (Exception ex)
            {
                _logger.Exception?.Log(ex);
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }
        }
    }
}