using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class PlaylistEntryExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this PlaylistEntry obj)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", obj.UniqueId
                }
                ,
                {
                    "Title", obj.Title
                }
                ,
                {
                    "CreateUser", obj.CreateUser?.ToJsonDictionary()
                }
                ,
                {
                    "Track", obj.Track?.ToJsonDictionary()
                }
            };
        }
    }
}