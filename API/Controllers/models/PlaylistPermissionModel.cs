using System;

namespace ch.wuerth.tobias.mux.API.Controllers.models
{
    public class PlaylistPermissionModel
    {
        public Int32? UserId { get; set; }
        public Boolean CanModify { get; set; }
    }
}