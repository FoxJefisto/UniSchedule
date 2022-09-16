﻿using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using UniShedule.Telegram;
using UniShedule.Database;
using System.IO.Pipes;

namespace Telegram.Bot.Examples.Echo
{
    public class Handlers
    {
        enum UserCommands
        {
            SetGroupName,
            SetDate,
            None
        };

        private static TelegramControlsImp impControls = new TelegramControlsImp();
        private static TelegramImpFuncs impFuncs = new TelegramImpFuncs();
        private static DataBaseManager dbManager = DataBaseManager.GetInstance();
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Тип получаемого сообщения: {message.Type}\nСодержание получаемого сообщения: {message.Text}");
            if (message.Type != MessageType.Text)
                return;
            Task<Message> action;

            if (message.Text == "/start")
            {
                action = RegisterUser(botClient, message);
            }
            else
            {
                var lastUserCommand = (UserCommands)Enum.Parse(typeof(UserCommands),
               (await dbManager.GetUserInfoAsync(message.From.Id)).UserCommand);

                if (lastUserCommand == UserCommands.None)
                {
                    action = (message.Text) switch
                    {
                        "/openmenu" => OpenMainMenu(botClient, message),
                        "/closemenu" => CloseMainMenu(botClient, message),
                        "Показать расписание на сегодня" => GetTodaySchedule(botClient, message),
                        "Задать название группы" => SetGroupName(botClient, message),
                        "Задать дату" => SetDate(botClient, message),
                        "Показать расписание в определенный день" => GetSchedule(botClient, message),
                        _ => Usage(botClient, message)
                    };
                }
                else
                {
                    action = lastUserCommand switch
                    {
                        UserCommands.SetGroupName => SaveGroupName(botClient, message),
                        UserCommands.SetDate => SaveDate(botClient, message),
                        _ => Usage(botClient, message)
                    };
                }
            }
            var sentMessage = await action;
            Console.WriteLine($"Сообщение было отправлено с идентификатором: {sentMessage.MessageId}");
        }
        static async Task<Message> RegisterUser(ITelegramBotClient botClient, Message message)
        {
            await dbManager.SaveNewUserAsync(message);
            return await Usage(botClient, message);
        }
        static async Task<Message> OpenMainMenu(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.From.Id);
            if (user == null)
            {
                user = await dbManager.GetUserInfoAsync(message.Chat.Id);
            }
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: $"Текущая группа: {user.GroupName}\nТекущая дата: {DateTime.Today:M}\nВыбранная дата: {user.CurrentDate:M}",
                                                        replyMarkup: impControls.rkmMainMenu);
        }

        static async Task<Message> CloseMainMenu(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Удаление клавиатуры",
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> SetGroupName(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.From.Id);
            user.UserCommand = UserCommands.SetGroupName.ToString();
            await dbManager.ChangeUserInfo(user);
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Введите название группы",
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> SetDate(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.From.Id);
            user.UserCommand = UserCommands.SetDate.ToString();
            await dbManager.ChangeUserInfo(user);
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Введите дату",
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> SaveGroupName(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.From.Id);
            user.UserCommand = UserCommands.None.ToString();
            user.GroupName = message.Text;
            await dbManager.ChangeUserInfo(user);
            var isCorrectGroupName = await dbManager.CheckGroupNameAsync(user.GroupName);
            if (isCorrectGroupName)
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                            text: $"Название группы сохранено\nТекущее название группы: {user.GroupName}",
                                            replyMarkup: impControls.rkmMainMenu);
            }
            else
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                            text: $"Группа с таким названием не найдена. Возможно она просто отсутствует в базе. Хотите загрузить?",
                            replyMarkup: impControls.ikmYesNoLoad);
            }

        }

        static async Task<Message> SaveDate(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.From.Id);
            user.UserCommand = UserCommands.None.ToString();
            if (!DateTime.TryParse(message.Text, out var date))
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: $"Дата введена в неверном формате!",
                                                        replyMarkup: impControls.rkmMainMenu);
            }
            user.CurrentDate = date;
            await dbManager.ChangeUserInfo(user);
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: $"Дата сохранена\nТекущая дата: {user.CurrentDate:M}",
                                                        replyMarkup: impControls.rkmMainMenu);
        }

        static async Task<Message> GetTodaySchedule(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.From.Id);
            user.CurrentDate = DateTime.Today;
            await dbManager.ChangeUserInfo(user);
            bool isCorrectGroupName = await dbManager.CheckGroupNameAsync(user.GroupName);
            if (!isCorrectGroupName)
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                            text: $"Нет данных о группе {user.GroupName}",
                            replyMarkup: impControls.rkmMainMenu);
            }
            var lessons = await dbManager.GetTodayScheduleAsync(user.GroupName);
            if (lessons.Count == 0)
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                        text: $"Сегодня занятий нет)",
                                        replyMarkup: impControls.ikmDaySwitcher);

            }
            var bmp = impFuncs.DrawOneDaySchedule(lessons);
            const string path = @"C:/Windows/Temp/table.jpg";
            bmp.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                        photo: new InputOnlineFile(fileStream),
                                        replyMarkup: impControls.ikmDaySwitcher);
            }
        }

        static async Task<Message> GetSchedule(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.From.Id);
            if(user == null)
            {
                user = await dbManager.GetUserInfoAsync(message.Chat.Id);
            }
            var lessons = await dbManager.GetCustomDateScheduleAsync(user.GroupName, user.CurrentDate);
            if (lessons.Count == 0)
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                        text: $"{user.CurrentDate:M} занятий нет)",
                                        replyMarkup: impControls.ikmDaySwitcher);

            }
            var bmp = impFuncs.DrawOneDaySchedule(lessons);
            const string path = @"C:/Windows/Temp/table.jpg";
            bmp.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                        photo: new InputOnlineFile(fileStream),
                                        replyMarkup: impControls.ikmDaySwitcher);
            }
        }

        static async Task<Message> FetchNewGroup(ITelegramBotClient botClient, Message message)
        {
            var user = await dbManager.GetUserInfoAsync(message.Chat.Id);
            var loadingGroupName = user.GroupName;
            var fetcher = ScheduleFetcher.GetInstance();
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: $"Началась загрузка группы {loadingGroupName}. Я оповещу вас, когда она завершится",
                                                        replyMarkup: impControls.rkmMainMenu);
            await Task.Run(() => fetcher.SaveLessons(loadingGroupName));
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                            text: $"Загрузка группы {loadingGroupName} завершена!",
                                            replyMarkup: impControls.rkmMainMenu);
        }

        static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: impControls.usage,
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"");

            var user = await dbManager.GetUserInfoAsync(callbackQuery.Message.Chat.Id);
            Task<Message> action;
            switch (callbackQuery.Data)
            {
                case "PrevDay":
                    user.CurrentDate = user.CurrentDate.AddDays(-1);
                    await dbManager.ChangeUserInfo(user);
                    action = GetSchedule(botClient, callbackQuery.Message);
                    break;
                case "NextDay":
                    user.CurrentDate = user.CurrentDate.AddDays(1);
                    await dbManager.ChangeUserInfo(user);
                    action = GetSchedule(botClient, callbackQuery.Message);
                    break;
                case "YesLoad":
                    action = FetchNewGroup(botClient, callbackQuery.Message);
                    break;
                case "NoLoad":
                    action = OpenMainMenu(botClient, callbackQuery.Message);
                    break;
                default:
                    action = Usage(botClient, callbackQuery.Message);
                    break;
            }
            //await action;
        }

        private static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Получен линейный запрос от: {inlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };

            await botClient.AnswerInlineQueryAsync(
                inlineQueryId: inlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}
