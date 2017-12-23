﻿using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class AcoustIdResultExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this AcoustIdResult air)
        {
            return new Dictionary<String, Object>
            {
                {"Score", air.Score},
                {
                    "Track",
                    new Dictionary<String, Object>
                    {
                        {"UniqueId", air.Track.UniqueId},
                        {"Duration", air.Track.Duration},
                        {"Path", air.Track.Path}
                    }
                }
            };
        }
    }
}