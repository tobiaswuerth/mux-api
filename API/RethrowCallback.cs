using System;
using ch.wuerth.tobias.mux.Core.events;

namespace ch.wuerth.tobias.mux.API
{
    public class RethrowCallback : ICallback<Exception>
    {
        public void Push(Exception arg)
        {
            throw arg;
        }
    }
}