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
        private readonly JwtContextAuthenticatorPipe _contextAuthenticatorPipe;

        protected ApiConfig Config { get; }

        static DataController()
        {
            new List<String>
                {
                    Location.ApplicationDataDirectoryPath
                    , Location.LogsDirectoryPath
                    , Location.PluginsDirectoryPath
                }.Where(x => !Directory.Exists(x))
                .ToList()
                .ForEach(x =>
                {
                    LoggerBundle.Trace(Logger.DefaultLogFlags & ~LogFlags.SuffixNewLine, $"Trying to create directory '{x}'...");
                    Directory.CreateDirectory(x);
                    LoggerBundle.Trace(Logger.DefaultLogFlags & ~LogFlags.PrefixLoggerType & ~LogFlags.PrefixTimeStamp, "Ok.");
                });
        }

        protected DataController()
        {
            Config = Configurator.Request<ApiConfig>(AuthConfigFilePath);
            _contextAuthenticatorPipe = new JwtContextAuthenticatorPipe(Config.Authorization.Secret);
        }

        private static String AuthConfigFilePath { get; } =
            Path.Combine(Location.ApplicationDataDirectoryPath, "mux_config_auth");

        protected JwtPayload AuthorizedPayload { get; private set; }

        protected void NormalizePageSize(ref Int32 pageSize)
        {
            pageSize = pageSize > Config.ResultMaxPageSize ? Config.ResultMaxPageSize : pageSize < 0 ? 0 : pageSize;
        }

        protected Boolean IsAuthorized(out IActionResult statusCode)
        {
            try
            {
                JwtPayload payload = _contextAuthenticatorPipe.Process(HttpContext);

                using (DataContext context = NewDataContext())
                {
                    Boolean found = context.SetUsers.Any(x
                        => x.UniqueId.Equals(payload.ClientId) && x.Username.ToLower().Equals(payload.Name));
                    if (!found)
                    {
                        LoggerBundle.Warn($"Got valid payload for user which is not in database: '{payload.Name}'");
                        statusCode = StatusCode((Int32) HttpStatusCode.Unauthorized);
                        return false;
                    }
                }

                statusCode = null;
                AuthorizedPayload = payload;
                return true;
            }
            catch (Exception ex)
            {
                LoggerBundle.Trace(ex);
                statusCode = StatusCode((Int32) HttpStatusCode.Unauthorized);
                return false;
            }
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