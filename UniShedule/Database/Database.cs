using System;
using System.Collections.Generic;
using System.Text;

namespace UniShedule.Database
{
    public class Database
    {
        private MpeiApi api;
        private static Database instance;
        public static Database GetInstance()
        {
            if (instance == null)
                instance = new Database();
            return instance;
        }

        public void SaveLessons(string groupName)
        {
            var lessons = api.GetAllLessons(groupName);
            using (Context db = new Context())
            {
                db.Lessons.AddRange(lessons);
                db.SaveChanges();
            }
        }

        public void TruncateAllTables()
        {
            //TODO: Доделать удаление таблицы
        }

        private Database()
        {
            api = MpeiApi.GetInstance();
        }
       
    }
}
