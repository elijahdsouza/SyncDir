using System.Collections.Generic;

namespace Sync.Models
{
    public class SyncFileData
    {

        public string Name { get; set; }

        public IEnumerable<Log> Logs { get; set; }

    }
}