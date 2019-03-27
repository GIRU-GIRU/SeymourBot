using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SeymourBot.Storage
{
    class UserDisciplinaryEventStorage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DisciplineEventID { get; set; }
        public ulong UserID { get; set; }
        public DisciplineEventEnum DiscipinaryEventType { get; set; }
        public ulong ModeratorID { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime DateToRemove { get; set; }
        public string Reason { get; set; }
    }
}
