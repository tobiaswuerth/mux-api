using System;
using ch.wuerth.tobias.mux.API.security.jwt;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.processor;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.security
{
    public class JwtPayloadProcessor : IProcessor<User, JwtPayload>
    {
        private readonly String _secret;

        public JwtPayloadProcessor(String secret)
        {
            _secret = secret;
        }

        public (JwtPayload output, Boolean success) Handle(User input, LoggerBundle logger)
        {
            return (new JwtPayload {Name = input.Username, Exp = DateTime.Now.AddDays(14), Iat = DateTime.Now}, true);
        }
    }
}