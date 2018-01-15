using System;
using System.Collections.Generic;
using System.Linq;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class MusicBrainzArtistExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this MusicBrainzArtist artist)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", artist.UniqueId
                }
                ,
                {
                    "Name", artist.Name
                }
                ,
                {
                    "Disambiguation", artist.Disambiguation
                }
                ,
                {
                    "Aliases", artist.MusicBrainzArtistMusicBrainzAliases?.Select(y => y.MusicBrainzAlias.ToJsonDictionary())
                }
            };
        }
    }
}