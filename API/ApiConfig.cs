using System;
using Newtonsoft.Json;

namespace ch.wuerth.tobias.mux.API
{
    public class ApiConfig
    {
        [ JsonProperty("Authorization") ]
        public AuthorizationImpl Authorization { get; set; } = new AuthorizationImpl();

        public Int32 ResultMaxPageSize { get; set; } = 100;

        public class AuthorizationImpl
        {
            public String Secret { get; set; } = "MY_SECRET";
            public Int32 ExpirationShift { get; set; } = 20160; // expiration shift, days
        }
    }
}