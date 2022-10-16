using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UniShedule.Model;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace UniShedule.Database
{
    class DataBaseManager
    {
        private static DataBaseManager instance;
        protected DataBaseManager()
        {

        }

        public static DataBaseManager GetInstance()
        {
            if (instance == null)
                instance = new DataBaseManager();
            return instance;
        }
        public async Task<List<Lesson>> GetAllScheduleAsync(string groupName)
        {
            using (Context db = new Context())
            {
                var lessons = await db.Lessons.Include(l => l.Group).Include(l => l.Date).Where(l => l.Group.Name == groupName).OrderBy(l => l.Date).ToListAsync();
                return lessons;
            }
        }

        public async Task<List<Lesson>> GetTodayScheduleAsync(string groupName)
        {
            using (Context db = new Context())
            {
                var lessons = await db.Lessons.Include(l => l.Group).Include(l => l.Date)
                    .Where(l => l.Group.Name == groupName && l.Date.Date == DateTime.Today)
                    .OrderBy(l => l.Time).ToListAsync();
                return lessons;
            }
        }

        public async Task<List<Lesson>> GetCustomDateScheduleAsync(string groupName, DateTime date)
        {
            using (Context db = new Context())
            {
                var lessons = await db.Lessons.Include(l => l.Group).Include(l => l.Date)
                    .Where(l => l.Group.Name == groupName && l.Date.Date == date)
                    .OrderBy(l => l.Time).ToListAsync();
                return lessons;
            }
        }

        public async Task<bool> CheckGroupNameAsync(string groupName)
        {
            using (Context db = new Context())
            {
                return await db.Groups.AnyAsync(x => x.Name == groupName);
            }
        }

        public async Task SaveNewUserAsync(Message message)
        {
            using (Context db = new Context())
            {
                bool isExist = await db.Users.AnyAsync(x => x.Id == message.From.Id);
                if (!isExist)
                {
                    Model.User user = new Model.User()
                    {
                        Id = message.From.Id,
                        FirstName = message.From.FirstName,
                        LastName = message.From.LastName,
                        UserName = message.From.Username,
                        CurrentDate = message.Date.Date,
                        GroupName = "",
                        UserCommand = "None",
                        ReminderState = false
                    };
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<Model.User> GetUserInfoAsync(long id)
        {
            using (Context db = new Context())
            {
                var user = await db.Users.FindAsync(id);
                return user;
            }
        }

        public async Task ChangeUserInfoAsync(Model.User user)
        {
            using (Context db = new Context())
            {
                db.Attach(user);
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task SaveReminderAsync(DateTime remindTime)
        {
            using (Context db = new Context())
            {
                bool isExist = await db.Reminders.AnyAsync(x => x.RemindTime.Hour == remindTime.Hour && x.RemindTime.Minute == remindTime.Minute);
                if (!isExist)
                {
                    db.Reminders.Add(new Reminder { RemindTime = remindTime });
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteReminderAsync(DateTime remindTime)
        {
            using (Context db = new Context())
            {
                var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.RemindTime.Hour == remindTime.Hour && x.RemindTime.Minute == remindTime.Minute);
                if (reminder != null && reminder.Users.Count == 0)
                {
                    db.Reminders.Remove(reminder);
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task SaveUserReminderAsync(Model.User user, DateTime remindTime)
        {
            using (Context db = new Context())
            {
                db.Users.Attach(user);
                var reminder = await db.Reminders.FirstAsync(x => x.RemindTime.Hour == remindTime.Hour && x.RemindTime.Minute == remindTime.Minute);
                user.Reminders.Add(reminder);
                reminder.Users.Add(user);
                await db.SaveChangesAsync();
            }
        }

        public async Task<string> DeleteUserReminderAsync(Model.User user, DateTime remindTime)
        {
            using (Context db = new Context())
            {
                db.Users.Attach(user);
                var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.RemindTime.Hour == remindTime.Hour && x.RemindTime.Minute == remindTime.Minute);
                if(reminder == null)
                {
                    return "Сценарий на данное время не обнаружен";
                }
                user.Reminders.Remove(reminder);
                reminder.Users.Remove(user);
                await db.SaveChangesAsync();
                return "Сценарий на данное время удален";
            }
        }

        public async Task<string> GetReminders(long Id)
        {
            using (Context db = new Context())
            {
                var user = await db.Users.Include(d => d.Reminders).FirstAsync(x => x.Id == Id);
                var sb = new StringBuilder();
                foreach (var reminder in user.Reminders.OrderBy(x => x.RemindTime.Hour).ThenBy(x => x.RemindTime.Minute))
                {
                    sb.Append($"{reminder.RemindTime:t}\n");
                }
                return sb.ToString();
            }
        }

        public async Task<List<string>> GetGroupsAsync()
        {
            using (Context db = new Context())
            {
                return await Task.Run(() => db.Groups.Select(x => x.Name).OrderBy(x => x).ToList());
            }
        }

        public List<Model.User> GetUsersOnTimed()
        {
            using (Context db = new Context())
            {
                var now = DateTime.Now;
                var users = db.Users.Include(d => d.Reminders).Where(x => x.ReminderState == true);
                var usersTimed = new List<Model.User>();
                foreach(var user in users)
                {
                    if (user.Reminders.Any(y => y.RemindTime.Hour == now.Hour && y.RemindTime.Minute == now.Minute))
                    {
                        usersTimed.Add(user);
                    }
                }
                return usersTimed;
            }
        }
    }
}
