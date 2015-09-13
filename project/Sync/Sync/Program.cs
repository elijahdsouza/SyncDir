using System;
using System.Linq;

using Sync.Core;
using Sync.Helpers;
using Sync.Models;

namespace Sync
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Start(args);

            Console.Write("Done");
            Console.Read();
        }

        private static void Start(string[] args)
        {
            var fileHelper = new FileInfoHelper();

            var dirs = fileHelper.GetDirectoryInfo(args).ToList();

            var syncDirA = new SyncDirectory(dirs.ElementAt(0));
            var syncDirB = new SyncDirectory(dirs.ElementAt(1));

            var syncMan = new SyncManager();
            syncMan.Sync(syncDirA, syncDirB);
        }

    }
}