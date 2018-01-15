using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class MusicBrainzAliasExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this MusicBrainzAlias alias)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", alias.UniqueId
                }
                ,
                {
                    "Name", alias.Name
                }
                ,
                {
                    "ShortName", alias.ShortName
                }
                ,
                {
                    "Primary", alias.Primary
                }
                ,
                {
                    "Ended", alias.Ended
                }
                ,
                {
                    "Begin", alias.Begin
                }
                ,
                {
                    "End", alias.End
                }
                ,
                {
                    "Locale", alias.Locale
                }
                ,
                {
                    "Type", alias.Type
                }
            };
        }
    }
}