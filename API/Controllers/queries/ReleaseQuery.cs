using System;

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

        public const String GET_ARTISTS_BY_ID = @"SELECT ac.[uniqueid], 
       ac.[uniquehash], 
       ac.[name], 
       ac.[joinphrase], 
       ac.[artist_uniqueid] 
FROM   musicbrainzrelease AS re 
       JOIN musicbrainzreleasemusicbrainzartistcredit AS rac 
         ON re.uniqueid = rac.musicbrainzrelease_uniqueid 
       JOIN musicbrainzartistcredit AS ac 
         ON rac.musicbrainzartistcredit_uniqueid = ac.uniqueid 
WHERE  re.uniqueid = {0}";

        public const String GET_ALIASES_BY_ID = @"SELECT a.[uniqueid], 
       a.[uniquehash], 
       a.[begin], 
       a.[locale], 
       a.[typeid], 
       a.[end], 
       a.[name], 
       a.[type], 
       a.[shortname], 
       a.[primary], 
       a.[ended] 
FROM   musicbrainzrelease AS re 
       JOIN musicbrainzreleasemusicbrainzalias AS ra 
         ON ra.musicbrainzrelease_uniqueid = re.uniqueid 
       JOIN musicbrainzalias AS a 
         ON ra.musicbrainzalias_uniqueid = a.uniqueid 
WHERE  re.uniqueid = {0}";

        public const String GET_RELEASE_EVENTS_BY_ID = @"SELECT re.[uniqueid], 
       re.[uniquehash], 
       re.[date], 
       re.[area_uniqueid] 
FROM   musicbrainzrelease AS r 
       JOIN musicbrainzreleaseeventmusicbrainzrelease AS rre 
         ON r.uniqueid = rre.musicbrainzrelease_uniqueid 
       JOIN musicbrainzreleaseevent AS re 
         ON re.uniqueid = rre.musicbrainzreleaseevent_uniqueid 
WHERE  r.uniqueid = {0}";
    }
}