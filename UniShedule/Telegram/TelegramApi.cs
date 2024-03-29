﻿using System;
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

        private static Printer imageCreator;

        private static TelegramControlsImp impControls = new TelegramControlsImp();

        protected TelegramApi()
        {
            dbManager = DataBaseManager.GetInstance();
            imageCreator = new Printer();
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
            Console.WriteLine($"[{DateTime.Now:T}]Сработало событие таймера");
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
                                            text: $"{DateTime.Today:M} занятий нет)");
                    continue;

                }
                var result = imageCreator.PrintOneDaySchedule(lessons);
                Bot.SendTextMessageAsync(chatId: user.Id,
                                            text: result,
                                            replyMarkup: impControls.ikmDaySwitcher);
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
