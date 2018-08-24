using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class UserExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this User user)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", user.UniqueId
                }
                ,
                {
                    "Username", user.Username
                }
            };
        }
    }
}