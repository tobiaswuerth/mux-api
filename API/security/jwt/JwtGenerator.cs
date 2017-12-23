using System;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public static class JwtGenerator
    {
        public static String GetToken(JwtPayload payload, String secret, Int32 expirationShift)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
            if (secret == null)
            {
                throw new ArgumentNullException(nameof(secret));
            }

            payload.Iat = DateTime.Now;
            payload.Exp = payload.Iat.AddMinutes(expirationShift); // in minutes

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }
    }
}