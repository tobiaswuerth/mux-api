using System;

namespace ch.wuerth.tobias.mux.API.Controllers.queries
{
    public static class TrackQuery
    {
        public const String GET_TRACKS_LIKE_PATH = @"SELECT [UniqueId]
      ,[Path]
      ,[LastFingerprintCalculation]
      ,[FingerprintError]
      ,[Duration]
      ,[Fingerprint]
      ,[FingerprintHash]
      ,[LastAcoustIdApiCall]
      ,[AcoustIdApiError]
      ,[AcoustIdApiErrorCode]
  FROM [dbo].[Track]
  WHERE Path LIKE {0}";
    }
}