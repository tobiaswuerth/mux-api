﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.API.security.models;
using ch.wuerth.tobias.mux.Core.processor;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class LoginController : DataController
    {
        private readonly Int32 _expirationShift;
        private readonly String _secret;

        public LoginController(IConfiguration configuration) : base(configuration)
        {
            _secret = configuration[JwtConfig.JWT_SECRET_KEY];
            _expirationShift = Convert.ToInt32(configuration[JwtConfig.JWT_EXPIRATION_SHIFT_KEY]);
        }

        private IActionResult ProcessPayload(JwtPayload payload)
        {
            // generate token
            String token = JwtGenerator.GetToken(payload, _secret, _expirationShift);

            if (String.IsNullOrWhiteSpace(token))
            {
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }

            return Ok(new Dictionary<String, String> {{"token", token}});
        }

        [HttpPost("public/login")]
        public IActionResult Login([FromBody] AuthenticationModel values)
        {
            try
            {
                //validate data
                if (String.IsNullOrWhiteSpace(values?.Password) || String.IsNullOrWhiteSpace(values.Username))
                {
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                // hash password
                (String output, Boolean success) pp = new PasswordProcessor().Handle(values.Password, Logger);

                if (!pp.success)
                {
                    return StatusCode((Int32) HttpStatusCode.InternalServerError);
                }

                // normalize username
                values.Username = values.Username.Trim();

                // check database for given username
                User user;
                using (DataContext dc = NewDataContext())
                {
                    user = dc.SetUsers.AsNoTracking().FirstOrDefault(x => x.Username.Equals(values.Username));
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
                (JwtPayload output, Boolean success) jp = new JwtPayloadProcessor().Handle(user, Logger);

                return !jp.success ? StatusCode((Int32) HttpStatusCode.InternalServerError) : ProcessPayload(jp.output);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("public/login")]
        public IActionResult RefreshToken()
        {
            try
            {
                return !IsAuthorized(out IActionResult statusCode) ? statusCode : ProcessPayload(AuthorizedPayload);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}