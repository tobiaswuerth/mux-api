using System;
using System.Collections.Generic;
using ch.wuerth.tobias.mux.Data.models;

namespace ch.wuerth.tobias.mux.API.extensions
{
    public static class MusicBrainzReleaseExtension
    {
        public static Dictionary<String, Object> ToJsonDictionary(this MusicBrainzRelease mbr)
        {
            return new Dictionary<String, Object>
            {
                {
                    "UniqueId", mbr.UniqueId
                }
                ,
                {
                    "Title", mbr.Title
                }
                ,
                {
                    "Disambiguation", mbr.Disambiguation
                }
                ,
                {
                    "Country", mbr.Country
                }
                ,
                {
                    "Date", mbr.Date
                }
                ,
                {
                    "Quality", mbr.Quality
                }
                ,
                {
                    "Status", mbr.Status
                }
                ,
                {
                    "TextRepresentation", new Dictionary<String, Object>
                    {
                        {
                            "Language", mbr.TextRepresentation?.Language
                        }
                        ,
                        {
                            "Script", mbr.TextRepresentation?.Script
                        }
                    }
                }
            };
        }
    }
}