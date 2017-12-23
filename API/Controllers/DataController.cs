using System;
using System.Net;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.logging.exception;
using ch.wuerth.tobias.mux.Core.logging.information;
using ch.wuerth.tobias.mux.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public abstract class DataController : Controller
    {
        private readonly JwtAuthenticator _authenticator;

        protected readonly LoggerBundle Logger = new LoggerBundle
        {
            Exception = new ExceptionFileLogger(null),
            Information = new InformationFileLogger(null) // todo callback
        };

        protected DataController(IConfiguration configuration)
        {
            _authenticator = new JwtAuthenticator(configuration[JwtConfig.JWT_SECRET_KEY]);
        }

        protected static void NormalizePageSize(ref Int32 pageSize)
        {
            pageSize = pageSize > 100 ? 100 : pageSize < 0 ? 0 : pageSize;
        }

        protected Boolean IsAuthorized(out IActionResult statusCode)
        {
            (JwtPayload output, Boolean success) authRes = _authenticator.Handle(HttpContext, Logger);
            if (authRes.success)
            {
                statusCode = null;
                return true;
            }

            statusCode = StatusCode((Int32) HttpStatusCode.Unauthorized);
            return false;
        }

        protected IActionResult HandleException(Exception ex)
        {
            Logger?.Exception?.Log(ex);
            return StatusCode((Int32) HttpStatusCode.InternalServerError);
        }

        protected DataContext NewDataContext()
        {
            return new DataContext(new DbContextOptions<DataContext>(), Logger);
        }
    }
}