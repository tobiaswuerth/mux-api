using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class InviteExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this Invite invite)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", invite.UniqueId
                }
                ,
                {
                    "CreateUser", invite.CreateUser.Username
                }
                ,
                {
                    "CreateDate", invite.CreateDate
                }
                ,
                {
                    "ExpirationDate", invite.ExpirationDate
                }
                ,
                {
                    "IsExpired", invite.ExpirationDate < DateTime.Now
                }
                ,
                {
                    "RegisteredUser", invite.RegisteredUser?.Username
                }
                ,
                {
                    "Token", invite.Token
                }
            };
        }
    }
}