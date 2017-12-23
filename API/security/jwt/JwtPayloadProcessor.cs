using System;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.processor;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class JwtPayloadProcessor : IProcessor<User, JwtPayload>, IProcessor<JwtPayload, JwtPayload>
    {
        public (JwtPayload output, Boolean success) Handle(JwtPayload input, LoggerBundle logger)
        {
            if (input == null)
            {
                logger?.Exception?.Log(new ArgumentNullException(nameof(input)));
                return (null, false);
            }

            return (input, true);
        }

        public (JwtPayload output, Boolean success) Handle(User input, LoggerBundle logger)
        {
            if (input == null)
            {
                logger?.Exception?.Log(new ArgumentNullException(nameof(input)));
                return (null, false);
            }

            return (new JwtPayload {Name = input.Username}, true);
        }
    }
}