using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ch.wuerth.tobias.mux.API.Controllers.queries
{
    public static class ReleaseQuery
    {
        public const String GET_RECORDS_BY_ID = @"SELECT r.[uniqueid], 
       r.[musicbrainzid], 
       r.[title], 
       r.[disambiguation], 
       r.[lastmusicbrainzapicall], 
       r.[length], 
       r.[video], 
       r.[musicbrainzapicallerror] 
FROM   musicbrainzrelease AS re 
       JOIN musicbrainzreleasemusicbrainzrecord AS rr 
         ON re.uniqueid = rr.musicbrainzrelease_uniqueid 
       JOIN musicbrainzrecord AS r 
         ON rr.musicbrainzrecord_uniqueid = r.uniqueid 
WHERE  re.uniqueid = {0}";
    }
}
