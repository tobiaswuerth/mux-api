using System;
using ch.wuerth.tobias.mux.Data.models;
using ch.wuerth.tobias.ProcessPipeline;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class JwtPayloadProcessor : ProcessPipe<User, JwtPayload>
    {
        public JwtPayloadProcessor() : base(o =>
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            return new JwtPayload
            {
                Name = o.Username
                , ClientId = o.UniqueId
            };
        }) { }
    }
}