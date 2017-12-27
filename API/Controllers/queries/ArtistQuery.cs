using System;

namespace ch.wuerth.tobias.mux.API.Controllers.queries
{
    public static class ArtistQuery
    {
        public const String GET_RECORDS_BY_ID = @"SELECT r.[uniqueid], 
       r.[musicbrainzid], 
       r.[title], 
       r.[disambiguation], 
       r.[lastmusicbrainzapicall], 
       r.[length], 
       r.[video], 
       r.[musicbrainzapicallerror] 
FROM   musicbrainzartist AS a 
       JOIN musicbrainzartistcredit AS ac 
         ON ac.artist_uniqueid = a.uniqueid 
       JOIN musicbrainzartistcreditmusicbrainzrecord AS acr 
         ON acr.musicbrainzartistcredit_uniqueid = ac.uniqueid 
       JOIN musicbrainzrecord AS r 
         ON r.uniqueid = acr.musicbrainzrecord_uniqueid 
WHERE  a.uniqueid = {0}";

        public const String GET_RELEASES_BY_ID = @"SELECT r.[uniqueid], 
       r.[uniquehash], 
       r.[title], 
       r.[status], 
       r.[quality], 
       r.[country], 
       r.[date], 
       r.[disambiguation], 
       r.[statusid], 
       r.[id], 
       r.[packagingid], 
       r.[barcode], 
       r.[textrepresentation_uniqueid] 
FROM   musicbrainzartist AS a 
       JOIN musicbrainzartistcredit AS ac 
         ON ac.artist_uniqueid = a.uniqueid 
       JOIN musicbrainzreleasemusicbrainzartistcredit AS acr 
         ON acr.musicbrainzartistcredit_uniqueid = ac.uniqueid 
       JOIN musicbrainzrelease AS r 
         ON r.uniqueid = acr.musicbrainzrelease_uniqueid 
WHERE  a.uniqueid = {0}";
    }
}