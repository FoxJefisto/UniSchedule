using System;
using System.Collections.Generic;
using System.Linq;
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
            using (Context db = new Context())
            {
                if(!db.Groups.Any(x => x.Name == groupName))
                {
                    var lessons = api.GetAllLessons(groupName);
                    db.Lessons.AddRange(lessons);
                    db.SaveChanges();
                }
            }
        }

        private ScheduleFetcher()
        {
            api = MpeiApi.GetInstance();
        }
       
    }
}
