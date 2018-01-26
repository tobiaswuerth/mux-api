using System;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.ProcessPipeline;
using JWT;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class JwtContextAuthenticatorPipe : ProcessPipe<HttpContext, JwtPayload>
    {
        private const String BEARER_PREFIX = "Bearer ";

        public JwtContextAuthenticatorPipe(String secret) : base(o =>
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            // validate inputs
            IHeaderDictionary headers = o.Request?.Headers;
            if (null == headers)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            // check for authorization header
            String headerKeyAuth = HttpRequestHeader.Authorization.ToString();
            if (!headers.ContainsKey(headerKeyAuth))
            {
                throw new ArgumentNullException(nameof(headerKeyAuth));
            }

            // get header and validate content
            StringValues values = headers[headerKeyAuth];

            if (!values.Count.Equals(1))
            {
                throw new ArgumentOutOfRangeException(nameof(values));
            }

            String value = values.First().Trim();

            if (!value.StartsWith(BEARER_PREFIX))
            {
                throw new FormatException("unknown schema");
            }

            // extract token
            String token = value.Substring(BEARER_PREFIX.Length).Trim();

            if (String.IsNullOrWhiteSpace(token))
            {
                throw new FormatException("empty token");
            }

            // prepare to decode token

            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            // decode token 
            String json = decoder.Decode(token, secret, true);

            if (String.IsNullOrWhiteSpace(json))
            {
                throw new FormatException("decoded json empty");
            }

            // deserialize json
            JwtPayload payload = serializer.Deserialize<JwtPayload>(json);

            // validate
            if (null == payload)
            {
                throw new FormatException("unknown payload schema");
            }

            if (DateTime.Now > payload.Exp || DateTime.Now < payload.Iat)
            {
                throw new TokenExpiredException("token expired");
            }

            return payload;
        }) { }
    }
}