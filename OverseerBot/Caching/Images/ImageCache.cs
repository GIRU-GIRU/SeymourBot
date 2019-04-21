using System;
using System.Collections.Generic;
using System.Text;

namespace OverseerBot.Caching.Images
{
    class ImageCache
    {
        private CachedElement[] cachedElements;
        private int currentIndex;

        public ImageCache(int size)
        {
            cachedElements = new CachedElement[size];
            for (int i = 0; i < cachedElements.Length; i++)
            {
                cachedElements[i] = new CachedElement();
            }
            currentIndex = 0;
        }

        public void AddImageInCache(CachedElement element)
        {
            if (currentIndex >= cachedElements.Length)
            {
                currentIndex = 0;
            }
            cachedElements[currentIndex++] = element;
        }

        public CachedElement FindImage(ulong id)
        {
            CachedElement result = null;
            for (int i = 0; i < cachedElements.Length && result == null; i++)
            {
                if (cachedElements[i].MessageId == id)
                {
                    result = cachedElements[i];
                }
            }
            return result;
        }
    }
}
