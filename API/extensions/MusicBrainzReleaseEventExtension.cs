using System;
using System.Collections.Generic;
using System.Linq;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class MusicBrainzReleaseEventExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this MusicBrainzReleaseEvent revent)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", revent.UniqueId
                }
                ,
                {
                    "Date", revent.Date
                }
                ,
                {
                    "Area", new Dictionary<String, Object>
                    {
                        {
                            "UniqueId", revent.Area?.UniqueId
                        }
                        ,
                        {
                            "Name", revent.Area?.Name
                        }
                        ,
                        {
                            "Disambiguation", revent.Area?.Disambiguation
                        }
                        ,
                        {
                            "SortName", revent.Area?.SortName
                        }
                        ,
                        {
                            "IsoCodes", revent.Area?.MusicBrainzIsoCodeMusicBrainzAreas.Select(x
                                => new Dictionary<String, Object>
                                {
                                    {
                                        "UniqueId", x.MusicBrainzIsoCode.UniqueId
                                    }
                                    ,
                                    {
                                        "Code", x.MusicBrainzIsoCode.Code
                                    }
                                })
                        }
                    }
                }
            };
        }
    }
}