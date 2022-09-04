using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UniShedule.Model
{
    public class DateInfo
{
        public int Id { get; set; }
        public string DateWeekName { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}
