using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Sync.Helpers;
using Sync.Models;

namespace Sync.Core
{
    public static class SyncFileManager
    {

        public const string SynFile = "log.json";

        public static IList<SyncFileData> GetSyncFileContents(FileInfo directory)
        {
            var syncPath = string.Format(@"{0}\{1}", directory.FullName, SynFile);
            List<SyncFileData> updatedLogs;

            if (File.Exists(syncPath))
            {
                var listofLogs = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(syncPath))
                    .Select(JTokenToSyncFileData);

                updatedLogs = UpdateSyncFile(directory, listofLogs).ToList();
            }
            else
            {
                updatedLogs = (List<SyncFileData>) GetSyncFileData(directory);
            }

            FileInfoHelper.WriteLogToSyncFile(syncPath, updatedLogs);

            return updatedLogs;
        }

        public static SyncFileData ToSyncFileData(FileSystemInfo directory, string fileName)
        {
            return new FileInfo(fileName).ToSyncFileData();
        }

        private static IEnumerable<SyncFileData> UpdateSyncFile(FileSystemInfo directory,
            IEnumerable<SyncFileData> syncLogs)
        {
            var syncFileDatas = syncLogs as IList<SyncFileData> ?? syncLogs.ToList();

            var filesInDir = Directory
                .GetFiles(directory.FullName)
                .Where(f => f != SynFile)
                .Select(fileName => ToSyncFileData(directory, fileName))
                .ToList();

            filesInDir.ForEach(fileInfo =>
            {
                // if the file is int the logs
                var file = syncFileDatas.SingleOrDefault(syncFileData => syncFileData.Name == fileInfo.Name);

                // if the file does not exist in the logs 
                if (file == null)
                {
                    // add to sync file
                    var x = syncFileDatas.ToList();
                    x.Add(fileInfo);
                    syncFileDatas = x;
                }
                else
                {
                    UpdateSyncFile(file, fileInfo);
                }
            });

            return syncFileDatas;
        }

        private static void UpdateSyncFile(SyncFileData file, SyncFileData fileInfo)
        {
            var last = file.Logs.Last(); // last log in sync file
            var hash = last.Hash; // hash of the file in sync log 

            var dirlog = fileInfo.Logs.Last(); // last log
            var updateHash = dirlog.Hash; // hash of the update file in directory

            if (hash == updateHash)
            {
                return;
            }
            if (last.Date >= dirlog.Date)
            {
                throw new InvalidOperationException(
                    string.Format("Sync file date is new that directory file [{0}]", fileInfo.Name));
            }
            var syncFileLogs = file.Logs.ToList();

            syncFileLogs.Add(new Log
            {
                Hash = updateHash,
                Date = dirlog.Date
            });
        }

        private static IList<SyncFileData> GetSyncFileData(FileSystemInfo directory)
        {
            return Directory.GetFiles(directory.FullName)
                .Select(f => new FileInfo(f))
                .Select(p => p.ToSyncFileData())
                .ToList();
        }

        private static SyncFileData JTokenToSyncFileData(JToken jToken)
        {
            var serializer = new JsonSerializer();

            var syncFileData = (SyncFileData) serializer.Deserialize(new JTokenReader(jToken), typeof (SyncFileData));

            return syncFileData;
        }

        public static SyncFileData CreateSyncFileData(string path)
        {
            return new FileInfo(path).ToSyncFileData();
        }

    }
}