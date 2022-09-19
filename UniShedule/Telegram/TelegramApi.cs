using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Examples.Echo;
using Telegram.Bot.Extensions.Polling;

namespace UniShedule.Telegram
{
    class TelegramApi
    {
        private static TelegramApi instance;

        private static TelegramBotClient? Bot;

        private CancellationTokenSource cts;

        protected TelegramApi()
        {
        }

        public static TelegramApi GetInstance()
        {
            if (instance == null)
                instance = new TelegramApi();
            return instance;
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
