using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class MusicBrainzArtistCreditExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this MusicBrainzArtistCredit ac)
        {
            return new Dictionary<String, Object>
            {
                {
                    "Joinphrase", ac.Joinphrase
                }
                ,
                {
                    "Artist", ac.Artist?.ToJsonDictionary()
                }
            };
        }
    }
}