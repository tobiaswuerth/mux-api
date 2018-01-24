using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Data;
using global::ch.wuerth.tobias.mux.Core.global;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public abstract class DataController : Controller
    {
        private readonly JwtAuthenticator _authenticator;
        
        private ApiConfig _config;

        static DataController()
        {
            new List<String>
                {
                    Location.ApplicationDataDirectoryPath
                    , Location.LogsDirectoryPath
                    , Location.PluginsDirectoryPath
                }.Where(x => !Directory.Exists(x))
                .ToList()
                .ForEach(x => Directory.CreateDirectory(x));
        }

        protected DataController()
        {
            _authenticator = new JwtAuthenticator(Config.Authorization.Secret);
        }

        protected ApiConfig Config
        {
            get
            {
                return _config ?? (_config = Configurator.Request<ApiConfig>(AuthConfigFilePath));
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
            (JwtPayload payload, Boolean success) = _authenticator.Handle(HttpContext);
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
            LoggerBundle.Error(ex);
            return StatusCode((Int32) HttpStatusCode.InternalServerError);
        }

        protected DataContext NewDataContext()
        {
            return new DataContext(new DbContextOptions<DataContext>());
        }
    }
}