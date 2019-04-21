using System;
using System.Collections.Generic;
using System.Text;

namespace OverseerBot.Caching.Images
{
    public class CachedFile
    {
        private string fileName;
        private byte[] file;

        public string FileName { get => fileName; set => fileName = value; }
        public byte[] File { get => file; set => file = value; }
    }
}
