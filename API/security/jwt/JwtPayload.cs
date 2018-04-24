using System;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class JwtPayload
    {
        public Int32 ClientId { get; set; }
        public String Name { get; set; }
        public DateTime Exp { get; set; } // expires at
        public DateTime Iat { get; set; } // issued at
        public Boolean CanInvite { get; set; }
    }
}