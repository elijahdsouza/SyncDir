using System.Collections.Generic;
using System.IO;

using Sync.Core;

namespace Sync.Models
{
    public class SyncDirectory
    {

        public SyncDirectory(FileInfo fileInfo)
        {
            SyncFileData = SyncFileManager.GetSyncFileContents(fileInfo);
            FileInfo = fileInfo;
        }

        public IList<SyncFileData> SyncFileData { get; set; }

        public FileInfo FileInfo { get; private set; }

    }
}