using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UniShedule.Model
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}
