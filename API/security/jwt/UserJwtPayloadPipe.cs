using System;
using ch.wuerth.tobias.mux.Data.models;
using ch.wuerth.tobias.ProcessPipeline;

namespace ch.wuerth.tobias.mux.API.security.jwt
{
    public class UserJwtPayloadPipe : ProcessPipe<User, JwtPayload>
    {
        public UserJwtPayloadPipe() : base(o =>
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            return new JwtPayload
            {
                Name = o.Username
                , ClientId = o.UniqueId
                , CanInvite = o.CanInvite
            };
        }) { }
    }
}