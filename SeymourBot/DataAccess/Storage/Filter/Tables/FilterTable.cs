using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SeymourBot.DataAccess.Storage.Filter.Tables
{
    class FilterTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FilterId { get; set; }
        public string FilterPattern { get; set; }
        public string FilterName { get; set; }
        public FilterTypeEnum FilterType { get; set; }
    }
}
