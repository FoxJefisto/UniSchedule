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
                    .OrderBy(l => l.Date).ToListAsync();
                return lessons;
            }
        }

        public async Task<List<Lesson>> GetCustomDateScheduleAsync(string groupName, DateTime date)
        {
            using (Context db = new Context())
            {
                var lessons = await db.Lessons.Include(l => l.Group).Include(l => l.Date)
                    .Where(l => l.Group.Name == groupName && l.Date.Date == date)
                    .OrderBy(l => l.Date).ThenBy(l => l.Time).ToListAsync();
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
                    Model.User user = new Model.User() { Id = message.From.Id, FirstName = message.From.FirstName, LastName = message.From.LastName, CurrentDate = message.Date.Date, GroupName = "", UserCommand = "None" };
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

        public async Task ChangeUserInfo(Model.User user)
        {
            using (Context db = new Context())
            {
                var result = await db.Users.FindAsync(user.Id);
                if(result != null)
                {
                    result.UserCommand = user.UserCommand;
                    result.GroupName = user.GroupName;
                    result.CurrentDate = user.CurrentDate;
                    db.Entry(result).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
