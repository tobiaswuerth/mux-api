using System;
using System.Collections.Generic;
using System.Linq;
using ch.wuerth.tobias.mux.Data;
using Microsoft.AspNetCore.Mvc;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class GlobalsController : DataController
    {
        [ HttpGet("auth/globals") ]
        public IActionResult Get()
        {
            try
            {
                if (!IsAuthorized(out IActionResult result))
                {
                    return result;
                }

                using (DataContext dataContext = NewDataContext())
                {
                    return Ok(new Dictionary<String, Object>
                    {
                        {
                            "Counts", new Dictionary<String, Object>
                            {
                                {
                                    "Tracks", dataContext.SetTracks.Count()
                                }
                                ,
                                {
                                    "Records", dataContext.SetMusicBrainzRecords.Count()
                                }
                                ,
                                {
                                    "Releases", dataContext.SetReleases.Count()
                                }
                                ,
                                {
                                    "Artists", dataContext.SetArtists.Count()
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}