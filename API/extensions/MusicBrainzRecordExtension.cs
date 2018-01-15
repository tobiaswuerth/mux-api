using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class MusicBrainzRecordExtension
    {
        public static IDictionary<String, Object> ToJsonDictionary(this MusicBrainzRecord record)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", record.UniqueId
                }
                ,
                {
                    "Title", record.Title
                }
                ,
                {
                    "Disambiguation", record.Disambiguation
                }
                ,
                {
                    "Length", record.Length
                }
            };
        }
    }
}