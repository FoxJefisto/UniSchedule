using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UniShedule.Model
{
    class Reminder
    {
        public long Id { get; set; }
        public DateTime RemindTime { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
