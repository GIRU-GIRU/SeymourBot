using System;
using System.Collections.Generic;
using System.Text;

namespace OverseerBot.Caching.Images
{
    class CachedElement
    {
        private ulong messageId;
        private List<CachedFile> files;

        public ulong MessageId { get => messageId; set => messageId = value; }
        public List<CachedFile> Files { get => files; set => files = value; }

        public CachedElement()
        {
            files = new List<CachedFile>();
        }
    }
}
