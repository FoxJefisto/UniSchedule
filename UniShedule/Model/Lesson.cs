using System;
using System.Collections.Generic;
using System.Text;
using UniShedule.Model;

namespace UniShedule.Model
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateInfo Date { get; set; } 
        public string Time { get; set; }
        public string Type { get; set; }
        public string Place { get; set; }
        public string Members { get; set; }
        public Group Group { get; set; }
        public string Teacher { get; set; }

    }
}
