using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SeymourBot.Storage
{
    class UserDisciplinaryEventArchive
    {
        [Key]
        public ulong ArchiveID { get; set; }
        public ulong UserID { get; set; }
        public DisciplineEventEnum DisciplineType { get; set; }
        public ulong ModeratorID { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime DateToRemove { get; set; }
        public DateTime DateArchived { get; set; }
        public string Reason { get; set; }

        public ulong DisciplineEventID { get; set; }
    }
}
