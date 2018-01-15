using System;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.processor;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class JwtPayloadProcessor : IProcessor<User, JwtPayload>
    {
        public (JwtPayload output, Boolean success) Handle(User input, LoggerBundle logger)
        {
            if (input != null)
            {
                return (new JwtPayload
                {
                    Name = input.Username
                    , ClientId = input.UniqueId
                }, true);
            }

            logger?.Exception?.Log(new ArgumentNullException(nameof(input)));
            return (null, false);
        }
    }
}