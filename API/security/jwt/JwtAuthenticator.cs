using System;
using System.Linq;
using System.Net;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.processor;
using JWT;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class JwtAuthenticator : IProcessor<HttpContext, JwtPayload>
    {
        private const String BEARER_PREFIX = "Bearer ";
        private readonly String _secret;

        public JwtAuthenticator(String secret)
        {
            _secret = secret;
        }

        public (JwtPayload output, Boolean success) Handle(HttpContext input, LoggerBundle logger)
        {
            if (null == input)
            {
                logger?.Exception?.Log(new ArgumentNullException(nameof(input)));
                return (null, false);
            }

            try
            {
                // validate inputs
                IHeaderDictionary headers = input.Request?.Headers;
                if (null == headers)
                {
                    throw new ArgumentNullException(nameof(headers));
                }

                // check for authorization header
                String hkey = HttpRequestHeader.Authorization.ToString();
                if (!headers.ContainsKey(hkey))
                {
                    throw new ArgumentNullException(nameof(headers));
                }

                // get header and validate content
                StringValues values = headers[hkey];

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
                String json = decoder.Decode(token, _secret, true);

                if (String.IsNullOrWhiteSpace(json))
                {
                    throw new FormatException("decoded json empty");
                }

                // deserializer json
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

                return (payload, true);
            }
            catch (Exception ex)
            {
                logger?.Exception?.Log(ex);
                return (null, false);
            }
        }
    }
}