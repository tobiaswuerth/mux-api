using System;
using System.Collections.Generic;
using System.Linq;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class PlaylistExtension
    {
        private static void SortPlaylistProperties(Playlist playlist)
        {
            playlist.PlaylistEntries?.Sort((a, b)
                => String.Compare(a.Title, b.Title, StringComparison.InvariantCultureIgnoreCase));

            playlist.PlaylistPermissions?.Sort((a, b)
                => String.Compare(a.User?.Username, b.User?.Username, StringComparison.InvariantCultureIgnoreCase));
        }

        public static Dictionary<String, Object> ToJsonDictionary(this Playlist obj)
        {
            SortPlaylistProperties(obj);

            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", obj.UniqueId
                }
                ,
                {
                    "Name", obj.Name
                }
                ,
                {
                    "CreateUser", obj.CreateUser?.ToJsonDictionary()
                }
                ,
                {
                    "Permissions", obj.PlaylistPermissions?.Select(x => x.ToJsonDictionary())
                }
                ,
                {
                    "Entries", obj.PlaylistEntries?.Select(x => x.ToJsonDictionary())
                }
            };
        }
    }
}