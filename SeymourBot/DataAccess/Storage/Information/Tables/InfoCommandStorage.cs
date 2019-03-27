using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SeymourBot.Storage.Information.Tables
{
    class InfoCommandTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommandID { get; set; }
        public string CommandName { get; set; }
        public string CommandContent { get; set; }
    }
}
