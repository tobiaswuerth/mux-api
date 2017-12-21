using System;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public static class JwtConfig
    {
        public const String JWT_SECRET_KEY = "JWT:Secret";
        public const String JWT_EXPIRATION_SHIFT_KEY = "JWT:ExpirationShift";
    }
}