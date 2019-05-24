using System;
using System.Linq;
using ch.wuerth.tobias.mux.API.extensions;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class UsersController : DataController
    {
        [ HttpGet("auth/users") ]
        public IActionResult Get([ FromQuery(Name = "ps") ] Int32 pageSize = 50, [ FromQuery(Name = "p") ] Int32 page = 0)
        {
            try
            {
                LoggerBundle.Trace("Registered GET request on UsersController.Get");
                if (!IsAuthorized(out IActionResult result))
                {
                    LoggerBundle.Trace("Request not authorized");
                    return result;
                }

                NormalizePageSize(ref pageSize);

                // get data
                using (DataContext dc = DataContextFactory.GetInstance())
                {
                    return Ok(dc.SetUsers.AsNoTracking()
                        .OrderBy(x => x.Username)
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
    }
}