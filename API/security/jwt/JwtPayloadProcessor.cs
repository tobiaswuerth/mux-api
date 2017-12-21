using System;
using ch.wuerth.tobias.mux.Core.logging;
using ch.wuerth.tobias.mux.Core.processor;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class JwtPayloadProcessor : IProcessor<User, JwtPayload>, IProcessor<JwtPayload, JwtPayload>
    {
        private readonly Int32 _expirationShift;

        public JwtPayloadProcessor(Int32 expirationShift)
        {
            _expirationShift = expirationShift;
        }

        public (JwtPayload output, Boolean success) Handle(JwtPayload input, LoggerBundle logger)
        {
            if (input == null)
            {
                logger?.Exception?.Log(new ArgumentNullException(nameof(input)));
                return (null, false);
            }

            ShiftPayloadDate(input);
            return (input, true);
        }

        public (JwtPayload output, Boolean success) Handle(User input, LoggerBundle logger)
        {
            if (input == null)
            {
                logger?.Exception?.Log(new ArgumentNullException(nameof(input)));
                return (null, false);
            }

            JwtPayload payload = new JwtPayload {Name = input.Username};
            ShiftPayloadDate(payload);
            return (payload, true);
        }

        private void ShiftPayloadDate(JwtPayload payload)
        {
            payload.Exp = DateTime.Now.AddDays(_expirationShift);
            payload.Iat = DateTime.Now;
        }
    }
}