using System;
using System.IO;
using System.Linq;
using ch.wuerth.tobias.mux.Data;
using ch.wuerth.tobias.mux.Data.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    public class FilesController : DataController
    {
        [ HttpGet("auth/files/{id}") ]
        public IActionResult Get(Int32? id)
        {
            try
            {
                if (!IsAuthorized(out IActionResult result))
                {
                    return result;
                }

                // validate
                if (id == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                // get track
                Track track;
                using (DataContext dataContext = NewDataContext())
                {
                    track = dataContext.SetTracks.FirstOrDefault(x => x.UniqueId.Equals(id));
                }

                // validate
                if (null == track || !System.IO.File.Exists(track.Path))
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                FileSystemInfo file = new FileInfo(track.Path);
                if (!file.Exists)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                // get mime type
                if (!new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out String contentType))
                {
                    // default fallback
                    contentType = $"audio/{file.Extension.Substring(1)}";
                }

                return PhysicalFile(file.FullName, contentType);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}