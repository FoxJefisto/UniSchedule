using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniShedule.Model;

namespace UniShedule.Telegram
{
    class Printer
    {

        public string PrintOneDaySchedule(List<Lesson> lessons)
        {
            StringBuilder str = new StringBuilder();
            string dateOfWeek = lessons.First().Date.DayOfWeek switch
            {
                DayOfWeek.Monday => "Понедельник",
                DayOfWeek.Tuesday => "Вторник",
                DayOfWeek.Wednesday => "Среда",
                DayOfWeek.Thursday => "Четверг",
                DayOfWeek.Friday => "Пятница",
                DayOfWeek.Saturday => "Суббота",
                DayOfWeek.Sunday => "Воскресенье",
                _ => ""
            };
            str.Append($"{dateOfWeek}, {lessons.First().Date:M}\n");
            foreach (var lesson in lessons)
            {
                str.Append($"{new string('-', 20)}\n");
                str.Append($"{lesson.Time}\n");
                str.Append($"{lesson.Name}\n");
                str.Append($"{lesson.Type}\n");
                str.Append($"{lesson.Place}\n");
                str.Append($"{lesson.Members}\n");
                str.Append($"{lesson.Teacher}\n");
            }
            return str.ToString();
        }
    }
}
