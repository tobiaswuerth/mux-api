using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class PlaylistPermissionExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this PlaylistPermission obj)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", obj.UniqueId
                }
                ,
                {
                    "User", obj.User?.ToJsonDictionary()
                }
                ,
                {
                    "CanModify", obj.CanModify
                }
            };
        }
    }
}