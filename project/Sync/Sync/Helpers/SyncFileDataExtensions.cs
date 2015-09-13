using System.Collections.Generic;
using System.IO;

using Sync.Models;

namespace Sync.Helpers
{
    public static class SyncFileDataExtensions
    {

        public static SyncFileData ToSyncFileData(this FileInfo fileInfo)
        {
            var fm = new FileInfoHelper();
            return new SyncFileData
            {
                Name = fileInfo.Name,
                Logs = new List<Log>
                {
                    new Log
                    {
                        Date = fileInfo.LastWriteTimeUtc,
                        Hash = fm.GetFileHash(fileInfo)
                    }
                }
            };
        }

    }
}