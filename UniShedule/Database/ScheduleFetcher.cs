using System;
using System.Collections.Generic;
using System.Text;

namespace UniShedule.Database
{
    public class ScheduleFetcher
    {
        private MpeiApi api;
        private static ScheduleFetcher instance;
        public static ScheduleFetcher GetInstance()
        {
            if (instance == null)
                instance = new ScheduleFetcher();
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

        private ScheduleFetcher()
        {
            api = MpeiApi.GetInstance();
        }
       
    }
}
