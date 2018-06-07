using System;
using System.Collections.Generic;
using System.Linq;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class PlaylistExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this Playlist obj)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", obj.UniqueId
                }
                ,
                {
                    "Name", obj.Name
                }
                ,
                {
                    "CreateUser", obj.CreateUser?.Username
                }
                ,
                {
                    "Permissions", obj.PlaylistPermissions?.Select(x => x.ToJsonDictionary())
                }
                ,
                {
                    "Entries", obj.PlaylistEntries?.Select(x => x.ToJsonDictionary())
                }
            };
        }
    }
}