using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Examples.Echo;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using UniShedule.Database;

namespace UniShedule.Telegram
{
    class TelegramApi
    {
        private static TelegramApi instance;

        private static TelegramBotClient? Bot;

        private static CancellationTokenSource cts;

        private static System.Timers.Timer timer;

        private static DataBaseManager dbManager;

        private static SKImageCreator imageCreator;

        protected TelegramApi()
        {
            dbManager = DataBaseManager.GetInstance();
            imageCreator = new SKImageCreator();
            SetTimer();
        }

        public static TelegramApi GetInstance()
        {
            if (instance == null)
                instance = new TelegramApi();
            return instance;
        }

        private static void SetTimer()
        {
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var users = dbManager.GetUsersOnTimed();
            foreach(var user in users)
            {
                bool isCorrectGroupName = dbManager.CheckGroupNameAsync(user.GroupName).GetAwaiter().GetResult();
                if (!isCorrectGroupName)
                {
                    Bot.SendTextMessageAsync(chatId: user.Id,
                                text: $"Нет данных о группе {user.GroupName}");
                    continue;
                }
                var lessons = dbManager.GetTodayScheduleAsync(user.GroupName).GetAwaiter().GetResult();
                if (lessons.Count == 0)
{
                    Bot.SendTextMessageAsync(chatId: user.Id,
                                            text: $"{user.CurrentDate:M} занятий нет)");
                    continue;

                }
                var result = imageCreator.PrintOneDaySchedule(lessons);
                Bot.SendTextMessageAsync(chatId: user.Id,
                                            text: result);
            }
        }



        public void Start()
        {
            Bot = new TelegramBotClient(Configuration.BotToken);

            var me = Bot.GetMeAsync().GetAwaiter().GetResult();
            Console.Title = me.Username;

            cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            Bot.StartReceiving(new DefaultUpdateHandler(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync),
                               cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
        }

        public void Stop()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
