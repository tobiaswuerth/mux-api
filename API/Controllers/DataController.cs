using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using global::ch.wuerth.tobias.mux.Core.global;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public abstract class DataController : Controller
    {
        private readonly JwtContextAuthenticatorPipe _contextAuthenticatorPipe;

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
                    LoggerBundle.Trace(Logger.DefaultLogFlags & ~LogFlags.SuffixNewLine
                        , $"Trying to create directory '{x}'...");
                    Directory.CreateDirectory(x);
                    LoggerBundle.Trace(Logger.DefaultLogFlags & ~LogFlags.PrefixLoggerType & ~LogFlags.PrefixTimeStamp, "Ok.");
                });
        }

        protected DataController()
        {
            Config = Configurator.Request<ApiConfig>(AuthConfigFilePath);
            _contextAuthenticatorPipe = new JwtContextAuthenticatorPipe(Config.Authorization.Secret);
        }

        protected ApiConfig Config { get; }

        private static String AuthConfigFilePath { get; } =
            Path.Combine(Location.ApplicationDataDirectoryPath, "mux_config_auth");

        protected JwtPayload AuthorizedPayload { get; private set; }
        protected User AuthorizedUser { get; private set; }

        protected IActionResult HandleException(Exception ex)
        {
            LoggerBundle.Error(ex);
            return StatusCode((Int32) HttpStatusCode.InternalServerError);
        }

        protected Boolean IsAuthorized(out IActionResult statusCode, Func<User, Boolean> customUserAuthorization = null)
        {
            try
            {
                JwtPayload payload = _contextAuthenticatorPipe.Process(HttpContext);

                using (DataContext context = DataContextFactory.GetInstance())
                {
                    User user = context.SetUsers.Include(x => x.Invites)
                        .ThenInclude(x => x.CreateUser)
                        .Include(x => x.Invite)
                        .ThenInclude(x => x.RegisteredUser)
                        .Include(x => x.Playlists)
                        .ThenInclude(x => x.PlaylistEntries)
                        .Include(x => x.Playlists)
                        .ThenInclude(x => x.CreateUser)
                        .Include(x => x.Playlists)
                        .ThenInclude(x => x.PlaylistPermissions)
                        .Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.CreateUser)
                        .Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.Playlist)
                        .Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.Track)
                        .Include(x => x.PlaylistPermissions)
                        .ThenInclude(x => x.Playlist)
                        .Include(x => x.PlaylistPermissions)
                        .ThenInclude(x => x.User)
                        .FirstOrDefault(x => x.UniqueId.Equals(payload.ClientId) && x.Username.ToLower().Equals(payload.Name));

                    if (null == user)
                    {
                        LoggerBundle.Warn($"Got valid payload for user which is not in database: '{payload.Name}'");
                        statusCode = StatusCode((Int32) HttpStatusCode.Unauthorized);
                        return false;
                    }

                    if (null != customUserAuthorization)
                    {
                        Boolean customAuthorization = customUserAuthorization.Invoke(user);
                        if (!customAuthorization)
                        {
                            LoggerBundle.Warn($"Got valid payload for user '{payload.Name}' but custom authorization failed");
                            statusCode = StatusCode((Int32) HttpStatusCode.Unauthorized);
                            return false;
                        }
                    }

                    AuthorizedUser = user;
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

        protected void NormalizePageSize(ref Int32 pageSize)
        {
            pageSize = pageSize > Config.ResultMaxPageSize ? Config.ResultMaxPageSize : pageSize < 0 ? 0 : pageSize;
        }
    }
}