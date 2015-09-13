using System.Linq;

using Sync.Helpers;
using Sync.Models;

namespace Sync.Core
{
    internal class SyncManager
    {

        internal void Sync(SyncDirectory syncDirA, SyncDirectory syncDirB)
        {
            UpdateDirectory(syncDirA, syncDirB);

            //DoStuff(syncDirB, syncDirA);
        }

        private static void UpdateDirectory(SyncDirectory syncDirA, SyncDirectory syncDirB)
        {
            var fm = new FileInfoHelper();
            syncDirA.SyncFileData.ToList().ForEach(syncFileData => { Compare(syncFileData, syncDirA, syncDirB); });

            fm.WriteToSyncFile(syncDirB);
        }

        private static void Compare(SyncFileData syncFileData, SyncDirectory syncDirA, SyncDirectory syncDirB)
        {
            var fileInfoHelper = new FileInfoHelper();
            var syncFile = syncDirB.SyncFileData.SingleOrDefault(f => f.Name == syncFileData.Name);
            if (syncFile == null)
            {
                fileInfoHelper.Copy(syncFileData, syncDirA, syncDirB);
            }
            else
            {
                if (syncFileData.Logs.Last().Hash != syncFile.Logs.Last().Hash)
                {
                    if (syncFileData.Logs.Last().Date > syncFile.Logs.Last().Date)
                    {
                        //fm.Copy(syncDirA, syncDirB, syncFileData);
                    }
                }
                else
                {
                    if (syncFileData.Logs.Last().Date > syncFile.Logs.Last().Date)
                    {
                        // fm.Copy(syncDirB, syncDirA, syncFileData);
                    }
                }
            }
        }

    }
}