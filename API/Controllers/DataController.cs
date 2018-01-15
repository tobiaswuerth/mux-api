using System;
using System.IO;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.events;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.logging.exception;
using ch.wuerth.tobias.mux.Core.logging.information;
using ch.wuerth.tobias.mux.Data;
using global::ch.wuerth.tobias.mux.Core.global;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public abstract class DataController : Controller
    {
        private static readonly ICallback<Exception> RethrowCallback = new RethrowCallback();
        private readonly JwtAuthenticator _authenticator;

        protected readonly LoggerBundle Logger = new LoggerBundle
        {
            Exception = new ExceptionFileLogger(RethrowCallback)
            , Information = new InformationFileLogger(RethrowCallback)
        };

        private ApiConfig _config;

        protected DataController(IConfiguration configuration)
        {
            _authenticator = new JwtAuthenticator(Config.Authorization.Secret);
        }

        protected ApiConfig Config
        {
            get
            {
                return _config ?? (_config = Configurator.RequestConfig<ApiConfig>(AuthConfigFilePath, Logger));
            }
        }

        private static String AuthConfigFilePath { get; } = Path.Combine(Location.ApplicationDataDirectoryPath, "mux_config_auth");

        protected JwtPayload AuthorizedPayload { get; private set; }

        protected void NormalizePageSize(ref Int32 pageSize)
        {
            pageSize = pageSize > Config.ResultMaxPageSize ? Config.ResultMaxPageSize : pageSize < 0 ? 0 : pageSize;
        }

        protected Boolean IsAuthorized(out IActionResult statusCode)
        {
            (JwtPayload payload, Boolean success) = _authenticator.Handle(HttpContext, Logger);
            if (!success)
            {
                statusCode = StatusCode((Int32) HttpStatusCode.Unauthorized);
                return false;
            }

            using (DataContext context = NewDataContext())
            {
                Boolean found = context.SetUsers.Any(x => x.UniqueId.Equals(payload.ClientId) && x.Username.ToLower().Equals(payload.Name));
                if (!found)
                {
                    statusCode = StatusCode((Int32) HttpStatusCode.Unauthorized);
                    return false;
                }
            }

            statusCode = null;
            AuthorizedPayload = payload;
            return true;
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