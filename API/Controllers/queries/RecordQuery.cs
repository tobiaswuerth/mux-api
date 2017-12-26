using System;

namespace ch.wuerth.tobias.mux.API.Controllers.queries
{
    public static class RecordQuery
    {
        public const String GET_TRACKS_BY_ID = @"SELECT -1                                           AS 'UniqueId', 
       ( Sum(ar.score) / Count(ar.track_uniqueid) ) AS 'Score', 
       -1                                           AS 'AcoustId_UniqueId', 
       ar.track_uniqueid 
FROM   musicbrainzrecord AS r 
       JOIN musicbrainzrecordacoustid AS ra 
         ON r.uniqueid = ra.musicbrainzrecord_uniqueid 
       JOIN acoustidresult AS ar 
         ON ar.acoustid_uniqueid = ra.acoustid_uniqueid 
       JOIN track AS t 
         ON t.uniqueid = ar.track_uniqueid 
WHERE  r.uniqueid = {0} 
GROUP  BY ar.track_uniqueid";

        public const String GET_RELEASES_BY_ID = @"SELECT re.[uniqueid], 
       re.[uniquehash], 
       re.[title], 
       re.[status], 
       re.[quality], 
       re.[country], 
       re.[date], 
       re.[disambiguation], 
       re.[statusid], 
       re.[id], 
       re.[packagingid], 
       re.[barcode], 
       re.[textrepresentation_uniqueid] 
FROM   musicbrainzrecord AS r 
       JOIN musicbrainzreleasemusicbrainzrecord AS rr 
         ON r.uniqueid = rr.musicbrainzrecord_uniqueid 
       JOIN musicbrainzrelease AS re 
         ON rr.musicbrainzrelease_uniqueid = re.uniqueid 
WHERE  r.uniqueid = {0}";

        public const String GET_ARTISTS_BY_ID = @"SELECT ac.[uniqueid], 
       ac.[uniquehash], 
       ac.[name], 
       ac.[joinphrase], 
       ac.[artist_uniqueid] 
FROM   musicbrainzrecord AS r 
       JOIN musicbrainzartistcreditmusicbrainzrecord AS rac 
         ON r.uniqueid = rac.musicbrainzrecord_uniqueid 
       JOIN musicbrainzartistcredit AS ac 
         ON ac.uniqueid = rac.musicbrainzartistcredit_uniqueid 
WHERE  r.uniqueid = {0} ";

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
FROM   musicbrainzrecord AS re 
       JOIN musicbrainzaliasmusicbrainzrecord AS ra 
         ON ra.musicbrainzrecord_uniqueid = re.uniqueid 
       JOIN musicbrainzalias AS a 
         ON ra.musicbrainzalias_uniqueid = a.uniqueid 
WHERE  re.uniqueid = {0}";
    }
}