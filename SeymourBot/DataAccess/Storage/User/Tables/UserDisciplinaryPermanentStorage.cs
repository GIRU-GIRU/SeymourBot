using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SeymourBot.Storage.User
{
    class UserDisciplinaryPermanentStorage
    {
        [Key]
        public ulong DisciplineEventID { get; set; }

        public ulong UserID { get; set; }
        public DisciplinaryEventEnum DiscipinaryEventType { get; set; }
        public ulong ModeratorID { get; set; }
        public DateTime DateInserted { get; set; }
        public string Reason { get; set; }
    }
}
