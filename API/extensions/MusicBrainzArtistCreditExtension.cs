using System;
using System.Collections.Generic;
using System.Linq;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class MusicBrainzArtistCreditExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this MusicBrainzArtistCredit ac)
        {
            return new Dictionary<String, Object>
            {
                {"Joinphrase", ac.Joinphrase},
                {
                    "Artist",
                    new Dictionary<String, Object>
                    {
                        {"UniqueId", ac.Artist.UniqueId},
                        {"Name", ac.Artist.Name},
                        {"Disambiguation", ac.Artist.Disambiguation},
                        {
                            "Alias",
                            ac.Artist.MusicBrainzArtistMusicBrainzAliases.Select(y => new Dictionary<String, Object>
                            {
                                {"UniqueId", y.MusicBrainzAlias.UniqueId},
                                {"Name", y.MusicBrainzAlias.Name},
                                {"ShortName", y.MusicBrainzAlias.ShortName},
                                {"Primary", y.MusicBrainzAlias.Primary},
                                {"Ended", y.MusicBrainzAlias.Ended},
                                {"Begin", y.MusicBrainzAlias.Begin},
                                {"End", y.MusicBrainzAlias.End},
                                {"Locale", y.MusicBrainzAlias.Locale},
                                {"Type", y.MusicBrainzAlias.Type}
                            })
                        }
                    }
                }
            };
        }
    }
}