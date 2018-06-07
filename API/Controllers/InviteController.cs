using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ch.wuerth.tobias.mux.API.Controllers.models;
using ch.wuerth.tobias.mux.API.extensions;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.processing;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class InviteController : DataController
    {
        [ HttpDelete("auth/invites/{id}") ]
        public IActionResult Delete(Int32? id)
        {
            try
            {
                LoggerBundle.Trace("Registered DELETE request on InviteController.Delete");
                if (!IsAuthorized(out IActionResult result, u => u.CanInvite))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // validate
                if (null == id)
                {
                    LoggerBundle.Trace("Validation failed: id is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    Invite invite = dc.SetInvites.Include(x => x.CreateUser)
                        .Include(x => x.RegisteredUser)
                        .Where(x => x.CreateUser.CanInvite)
                        .Where(x => x.CreateUser.UniqueId.Equals(AuthorizedUser.UniqueId))
                        .FirstOrDefault(x => x.UniqueId.Equals(id));

                    if (null == invite)
                    {
                        LoggerBundle.Trace($"No invite found for given id '{id}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    if (null != invite.RegisteredUser)
                    {
                        LoggerBundle.Trace($"Invite with id '{id}' has already been used and can therefor not be deleted");
                        return StatusCode((Int32) HttpStatusCode.Conflict);
                    }

                    dc.SetInvites.Remove(invite);
                    dc.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpGet("auth/invites") ]
        public IActionResult GetAll([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on InviteController.GetAll");
                if (!IsAuthorized(out IActionResult result, u => u.CanInvite))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetInvites.Include(x => x.CreateUser)
                        .Include(x => x.RegisteredUser)
                        .AsNoTracking()
                        .Where(x => x.CreateUser.CanInvite)
                        .Where(x => x.CreateUser.UniqueId.Equals(AuthorizedPayload.ClientId))
                        .OrderByDescending(x => x.CreateDate)
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Select(x => x.ToJsonDictionary())
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpPut("auth/invites") ]
        public IActionResult New()
        {
            try
            {
                LoggerBundle.Trace("Registered PUT request on InviteController.New");
                if (!IsAuthorized(out IActionResult result, u => u.CanInvite))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                // create new entry
                String token = Guid.NewGuid().ToString("N").Substring(0, 16);
                Invite invite = new Invite
                {
                    CreateDate = DateTime.Now
                    , ExpirationDate = DateTime.Now.AddDays(14)
                    , Token = token
                };

                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    dc.SetUsers.Attach(AuthorizedUser);
                    AuthorizedUser.Invites.Add(invite);
                    dc.SaveChanges();
                }

                return Ok(invite.ToJsonDictionary());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ HttpPost("public/invites/{token}") ]
        public IActionResult Use(String token, [ FromBody ] InviteModel model)
        {
            try
            {
                LoggerBundle.Trace("Registered POST request on InviteController.Use");

                // validate
                if (null == token)
                {
                    LoggerBundle.Trace("Validation failed: token is null");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                if (null == model)
                {
                    LoggerBundle.Trace("Validation failed: model is undefined");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                String passwordBase64 = model.Password?.Trim();
                if (String.IsNullOrWhiteSpace(passwordBase64))
                {
                    LoggerBundle.Trace("Validation failed: password is empty");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                String username = model.Username?.Trim();
                if (String.IsNullOrWhiteSpace(username))
                {
                    LoggerBundle.Trace("Validation failed: username is empty");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                Byte[] bPassword = Convert.FromBase64String(passwordBase64);
                String password = Encoding.UTF8.GetString(bPassword);

                if (password.Length < 8)
                {
                    LoggerBundle.Trace("Validation failed: password needs to be at least 8 characters long");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                if (!new Regex("[0-9]").IsMatch(password))
                {
                    LoggerBundle.Trace("Validation failed: password must contain at least one number");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                if (!new Regex("[a-zA-Z]").IsMatch(password))
                {
                    LoggerBundle.Trace("Validation failed: password must contain at least one letter");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                if (!new Regex("[^a-zA-Z0-9]").IsMatch(password))
                {
                    LoggerBundle.Trace("Validation failed: password must contain at least one special character");
                    return StatusCode((Int32) HttpStatusCode.BadRequest);
                }

                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    Invite invite = dc.SetInvites.Include(x => x.CreateUser)
                        .Include(x => x.RegisteredUser)
                        .Where(x => x.CreateUser.CanInvite)
                        .FirstOrDefault(x => x.Token.Equals(token));

                    if (null == invite)
                    {
                        LoggerBundle.Trace($"No invite found for given token '{token}'");
                        return StatusCode((Int32) HttpStatusCode.NotFound);
                    }

                    if (null != invite.RegisteredUser)
                    {
                        LoggerBundle.Trace($"Invite with token '{token}' has already been used");
                        return StatusCode((Int32) HttpStatusCode.Conflict);
                    }

                    if (invite.ExpirationDate < DateTime.Now)
                    {
                        LoggerBundle.Trace($"Invite with token '{token}' has expired");
                        return StatusCode((Int32) HttpStatusCode.BadRequest);
                    }

                    String passwordHash = new Sha512HashPipe().Process(password);

                    User newUser = new User
                    {
                        Password = passwordHash
                        , Username = username
                        , CanInvite = false
                        , Invite = invite
                    };

                    dc.SetUsers.Add(newUser);
                    dc.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}