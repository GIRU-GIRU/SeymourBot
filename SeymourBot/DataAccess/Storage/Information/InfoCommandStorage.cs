using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SeymourBot.Storage.Information
{
    class InfoCommandTable
    {
        [Key]
        public int CommandID { get; set; }
        public string CommandName { get; set; }
        public string CommandContent { get; set; }
    }
}
