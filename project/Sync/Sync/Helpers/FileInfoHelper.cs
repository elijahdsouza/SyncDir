using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using Newtonsoft.Json;

using Sync.Core;
using Sync.Models;

namespace Sync.Helpers
{
    internal class FileInfoHelper
    {

        public const string SynFile = "log.json";

        internal FileInfoHelper()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        internal IEnumerable<FileInfo> GetDirectoryInfo(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentException("No diretories provided.");
            }

            if (args.Length < 2)
            {
                throw new ArgumentException("Please enter 2 directories");
            }
            var path1 = args[0];
            var path2 = args[1];

            if (!Directory.Exists(path1))
            {
                throw new ArgumentException(string.Format("[{0}] is not a valid directory path", path1));
            }
            if (!Directory.Exists(path2))
            {
                throw new ArgumentException(string.Format("[{0}] is not a valid directory path", path2));
            }

            if (!Directory.Exists(path1) && Directory.Exists(path2))
            {
                Directory.CreateDirectory(path1);
            }
            if (!Directory.Exists(path2) && Directory.Exists(path1))
            {
                Directory.CreateDirectory(path2);
            }

            return new List<FileInfo>
            {
                new FileInfo(path1),
                new FileInfo(path2)
            };
        }

        internal void Copy(SyncFileData fileToCopy, SyncDirectory fromDirectory, SyncDirectory toDirectory)
        {
            var src = Format(fromDirectory, fileToCopy.Name);
            var dest = Format(toDirectory, fileToCopy.Name);

            File.Copy(src, dest, true);
            var syncFileData
                = SyncFileManager.CreateSyncFileData(dest);

            var foo = toDirectory.SyncFileData.ToList();
            foo.Add(syncFileData);
            toDirectory.SyncFileData = foo;

            WriteToSyncFile(toDirectory);
        }

        private static string Format(SyncDirectory fullName, string fileName)
        {
            return string.Format(@"{0}\{1}", fullName.FileInfo.FullName, fileName);
        }

        internal string GetFileHash(FileInfo fileInfo)
        {
            var fileStream = fileInfo.Open(FileMode.Open);
            var byteString = SHA256.Create().ComputeHash(fileStream).Select(b => b.ToString("x2"));
            var hash = string.Concat(byteString);
            return hash;
        }

        internal void WriteToSyncFile(SyncDirectory syncDirB)
        {
            var syncPath = string.Format(@"{0}\{1}", syncDirB.FileInfo.FullName, SynFile);

            var json = JsonConvert.SerializeObject(syncDirB.SyncFileData);

            File.WriteAllText(syncPath, json);
        }


        public static void WriteLogToSyncFile(string syncPath, IList<SyncFileData> updatedLogs)
        {
            File.WriteAllText(syncPath, JsonConvert.SerializeObject(updatedLogs));
        }


    }
}