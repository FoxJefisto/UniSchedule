using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using UniShedule.Model;

namespace UniShedule.Telegram
{
    class TelegramControlsImp
    {
        public ReplyKeyboardMarkup rkmMainMenu = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Показать расписание на сегодня" },
                        new KeyboardButton[] { "Задать название группы"},
                        new KeyboardButton[] { "Показать расписание в определенный день", "Меню авторассылок" },
                        new KeyboardButton[] { "Список загруженных групп", "Скрыть меню" },
                    }
                    )
        {
            ResizeKeyboard = true
        };

        public ReplyKeyboardMarkup getRKMReminderMenu(User user)
        {
            var state = user.ReminderState switch
            {
                false => "Включить",
                true => "Отключить"
            };
            ReplyKeyboardMarkup rkmReminderMenu = new ReplyKeyboardMarkup(
    new KeyboardButton[][]
    {
                        new KeyboardButton[] { $"{state} авторассылки" },
                        new KeyboardButton[] { "Добавить новый сценарий"},
                        new KeyboardButton[] { "Удалить сценарий", "В главное меню" },
                        new KeyboardButton[] { "Список сценариев", "Скрыть меню"},
    }
    )
            {
                ResizeKeyboard = true
            };
            return rkmReminderMenu;
        }


        public InlineKeyboardMarkup ikmDaySwitcher = new InlineKeyboardMarkup(new[]
                                {
                                    new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("<<", "PrevDay"),
                                        InlineKeyboardButton.WithCallbackData(">>", "NextDay")
                                    }
        });

        public InlineKeyboardMarkup ikmYesNoLoad = new InlineKeyboardMarkup(new[]
                        {
                                    new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("Да", "YesLoad"),
                                        InlineKeyboardButton.WithCallbackData("Нет", "NoLoad")
                                    }
        });

        public InlineKeyboardMarkup ikmShowGroups = new InlineKeyboardMarkup(new[]
                        {
                                    new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("Группы", "ShowGroups"),
                                    }
        });

        public InlineKeyboardMarkup ikmShowReminders = new InlineKeyboardMarkup(new[]
                {
                                    new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("Активные сценарии", "ShowReminders"),
                                    }
        });

        public string usage = "Использование:\nОткрыть главное меню:\n/openmenu\nОткрыть меню авторассылок:\n/switchreminder\n/Закрыть меню:\n/closemenu";

        public string welcome = $"Привет! Я умею узнавать расписание занятий у всех групп НИУ МЭИ. Тебе всего лишь нужно указать номер группы в соответствующем меню, а всё остальное я сделаю сам) " +
                                                    $"Пока мои знания довольно скудные. Я знаю лишь некоторые группы, о которых рассказали мне студенты. С ними ты сможешь ознакомиться в разделе 'Список загруженных групп'. " +
                                                    $"Если твоей группы там нет - это здорово, значит у тебя появилась возможность внести свой вклад в моей развитие. " +
                                                    $"Укажи её название в меню выбора группы и следуй дальнейшим инструкциям. После этого остается немного подождать, пока я буду обрабатывать новые данные.";
    }
}
