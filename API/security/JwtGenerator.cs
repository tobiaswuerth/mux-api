using System;
using ch.wuerth.tobias.mux.API.security.jwt;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace ch.wuerth.tobias.mux.API.security
{
    public static class JwtGenerator
    {
        public static String GetToken(JwtPayload payload, String secret)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
            if (secret == null)
            {
                throw new ArgumentNullException(nameof(secret));
            }

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }
    }
}