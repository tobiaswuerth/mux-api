using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using ch.wuerth.tobias.mux.API.Controllers.models;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.processing;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class LoginsController : DataController
    {
        private static readonly UserJwtPayloadPipe UserJwtPayloadPipe = new UserJwtPayloadPipe();

        [ HttpPost("public/login") ]
        public IActionResult Login([ FromBody ] AuthenticationModel values)
        {
            try
            {
                LoggerBundle.Trace("Registered POST request on LoginsController.Login");

                //validate data
                String passwordBase64 = values?.Password;
                if (String.IsNullOrWhiteSpace(passwordBase64) || String.IsNullOrWhiteSpace(values.Username))
                {
                    LoggerBundle.Trace("Validation failed: empty username or password");
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                // hash password
                Byte[] bPassword = Convert.FromBase64String(passwordBase64);
                String password = Encoding.UTF8.GetString(bPassword);
                String passwordHash = new Sha512HashPipe().Process(password);

                // normalize username
                values.Username = values.Username.Trim();

                // check database for given username
                User user;
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    user = dc.SetUsers.AsNoTracking().FirstOrDefault(x => x.Username.Equals(values.Username));
                }

                if (null == user)
                {
                    LoggerBundle.Trace($"No user found for given username '{values.Username}'");
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                if (!user.Password.Equals(passwordHash))
                {
                    LoggerBundle.Trace($"Login attempt for user '{user.Username}' failed");
                    return StatusCode((Int32) HttpStatusCode.Unauthorized);
                }

                // prepare token generation
                JwtPayload payload = UserJwtPayloadPipe.Process(user);
                return ProcessPayload(payload);
            }
            catch (Exception ex)
            {
                LoggerBundle.Error(ex);
                return StatusCode((Int32) HttpStatusCode.Unauthorized);
            }
        }

        private IActionResult ProcessPayload(JwtPayload payload)
        {
            // generate token
            String token = JwtGenerator.GetToken(payload, Config.Authorization.Secret, Config.Authorization.ExpirationShift);

            if (String.IsNullOrWhiteSpace(token))
            {
                LoggerBundle.Warn("JWT token generation failed: empty token");
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }

            return Ok(new Dictionary<String, String>
            {
                {
                    "token", token
                }
                ,
                {
                    "expires", payload.Exp.ToString(CultureInfo.InvariantCulture)
                }
            });
        }

        [ HttpGet("auth/login") ]
        public IActionResult RefreshToken()
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on LoginController.RefreshToken");
                if (IsAuthorized(out IActionResult statusCode))
                {
                    return ProcessPayload(AuthorizedPayload);
                }

                LoggerBundle.Trace("Request not authorized");
                return statusCode;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}