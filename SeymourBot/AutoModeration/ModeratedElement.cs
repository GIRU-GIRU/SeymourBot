using SeymourBot.DataAccess.Storage.Filter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.AutoModeration
{
    public class ModeratedElement
    {
        private string pattern;
        private string dialog;

        public string Pattern { get => pattern; set => pattern = value; }
        public string Dialog { get => dialog; set => dialog = value; }
    }
}
