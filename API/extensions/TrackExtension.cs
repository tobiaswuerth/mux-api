using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class TrackExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this Track track)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", track.UniqueId
                }
                ,
                {
                    "Duration", track.Duration
                }
                ,
                {
                    "Path", track.Path
                }
            };
        }
    }
}