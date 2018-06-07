﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.API.Controllers.models;
using ch.wuerth.tobias.mux.API.extensions;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class PlaylistsController : DataController
    {
        [ HttpPut("auth/playlists") ]
        public IActionResult Create([ FromBody ] PlaylistModel model)
        {
            try
            {
                LoggerBundle.Trace("Registered PUT request on PlaylistsController.Create");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (model == null)
                {
                    LoggerBundle.Trace("Validation failed: model is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                String name = model.Name?.Trim();
                if (String.IsNullOrWhiteSpace(name) || name.Length < 3)
                {
                    LoggerBundle.Trace("Validation failed: invalid name");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    dc.SetUsers.Attach(AuthorizedUser);

                    Playlist playlist = new Playlist
                    {
                        CreateUser = AuthorizedUser
                        , Name = name
                        , PlaylistEntries = new List<PlaylistEntry>()
                        , PlaylistPermissions = new List<PlaylistPermission>()
                    };
                    dc.SetPlaylists.Add(playlist);
                    dc.SaveChanges();

                    return Ok(playlist.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpPut("auth/playlists/{id}/entries") ]
        public IActionResult CreateEntry(Int32? id, [ FromBody ] PlaylistEntryModel model)
        {
            try
            {
                LoggerBundle.Trace("Registered PUT request on PlaylistsController.CreateEntry");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (!id.HasValue)
                {
                    LoggerBundle.Trace("Validation failed: id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                if (null == model)
                {
                    LoggerBundle.Trace("Validation failed: model is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                Int32? trackId = model.TrackId;
                if (!trackId.HasValue)
                {
                    LoggerBundle.Trace("Validation failed: track identifier is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                String title = model.Title?.Trim();
                if (String.IsNullOrWhiteSpace(title) || title.Length < 3)
                {
                    LoggerBundle.Trace("Validation failed: invalid title");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    dc.SetUsers.Attach(AuthorizedUser);

                    Track track = dc.SetTracks.FirstOrDefault(x => x.UniqueId.Equals(trackId));
                    if (null == track)
                    {
                        LoggerBundle.Trace($"Validation failed: no track found for given id '{trackId}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    Playlist playlist = dc.SetPlaylists.Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.Track)
                        .Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.CreateUser)
                        .Include(x => x.PlaylistPermissions)
                        .ThenInclude(x => x.User)
                        .Where(x => x.CreateUser.UniqueId.Equals(AuthorizedUser.UniqueId)
                            || x.PlaylistPermissions.Any(y => y.User.UniqueId.Equals(AuthorizedUser.UniqueId) && y.CanModify))
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == playlist)
                    {
                        LoggerBundle.Trace($"Validation failed: no playlist found for given id '{id}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    PlaylistEntry entry = new PlaylistEntry
                    {
                        CreateUser = AuthorizedUser
                        , Title = title
                        , Track = track
                        , Playlist = playlist
                    };
                    dc.SetPlaylistEntries.Add(entry);
                    dc.SaveChanges();

                    return Ok(playlist.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpDelete("auth/playlists/{id}") ]
        public IActionResult Delete(Int32? id)
        {
            try
            {
                LoggerBundle.Trace("Registered DELETE request on PlaylistsController.Delete");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (!id.HasValue)
                {
                    LoggerBundle.Trace("Validation failed: id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    Playlist playlist = dc.SetPlaylists.Include(x => x.PlaylistEntries)
                        .Include(x => x.PlaylistPermissions)
                        .Where(x => x.CreateUser.UniqueId.Equals(AuthorizedUser.UniqueId))
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == playlist)
                    {
                        LoggerBundle.Trace($"No playlist found for given id '{id}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    dc.SetPlaylistEntries.RemoveRange(playlist.PlaylistEntries);
                    dc.SetPlaylistPermissions.RemoveRange(playlist.PlaylistPermissions);
                    dc.SetPlaylists.Remove(playlist);
                    dc.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpDelete("auth/playlists/{playlistId}/entries/{entryId}") ]
        public IActionResult DeleteEntry(Int32? playlistId, Int32? entryId)
        {
            try
            {
                LoggerBundle.Trace("Registered DELETE request on PlaylistsController.DeleteEntry");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (!playlistId.HasValue)
                {
                    LoggerBundle.Trace("Validation failed: playlist id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                if (!entryId.HasValue)
                {
                    LoggerBundle.Trace("Validation failed: entry id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    Playlist playlist = dc.SetPlaylists.Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.Track)
                        .Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.CreateUser)
                        .Include(x => x.PlaylistPermissions)
                        .ThenInclude(x => x.User)
                        .Where(x => x.CreateUser.UniqueId.Equals(AuthorizedUser.UniqueId)
                            || x.PlaylistPermissions.Any(y => y.User.UniqueId.Equals(AuthorizedUser.UniqueId) && y.CanModify))
                        .FirstOrDefault(x => x.UniqueId.Equals(playlistId));

                    if (null == playlist)
                    {
                        LoggerBundle.Trace($"Validation failed: no playlist found for given id '{playlistId}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    PlaylistEntry entry = playlist.PlaylistEntries.FirstOrDefault(x => x.UniqueId.Equals(entryId));

                    if (null == entry)
                    {
                        LoggerBundle.Trace($"Validation failed: no entry found in playlist for given id '{entryId}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    dc.SetPlaylistEntries.Remove(entry);
                    dc.SaveChanges();

                    return Ok(playlist.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/playlists") ]
        public IActionResult GetAll([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on PlaylistsController.GetAll");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    dc.SetUsers.Attach(AuthorizedUser);

                    List<Playlist> playlists = AuthorizedUser.PlaylistPermissions.Select(x => x.Playlist).ToList();
                    playlists.AddRange(AuthorizedUser.Playlists);
                    playlists.Sort((a, b) => String.CompareOrdinal(a.Name, b.Name));

                    return Ok(playlists.Select(x => new Dictionary<String, Object>
                    {
                        {
                            "UniqueId", x.UniqueId
                        }
                        ,
                        {
                            "Name", x.Name
                        }
                        ,
                        {
                            "CreateUser", x.CreateUser?.Username
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/playlists/{id}") ]
        public IActionResult GetById(Int32? id)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on PlaylistsController.GetById");

                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (id == null)
                {
                    LoggerBundle.Trace("Validation failed: id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    Playlist playlist = dc.SetPlaylists.AsNoTracking()
                        .Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.Track)
                        .Include(x => x.PlaylistEntries)
                        .ThenInclude(x => x.CreateUser)
                        .Include(x => x.PlaylistPermissions)
                        .ThenInclude(x => x.User)
                        .Where(x => x.CreateUser.UniqueId.Equals(AuthorizedUser.UniqueId)
                            || x.PlaylistPermissions.Any(y => y.User.UniqueId.Equals(AuthorizedUser.UniqueId)))
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == playlist)
                    {
                        LoggerBundle.Trace($"No playlist found for given id '{id}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    return Ok(playlist.ToJsonDictionary());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}