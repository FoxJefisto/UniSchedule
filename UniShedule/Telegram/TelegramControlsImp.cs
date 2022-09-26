using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace UniShedule.Telegram
{
    class TelegramControlsImp
    {
        public ReplyKeyboardMarkup rkmMainMenu = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Показать расписание на сегодня" },
                        new KeyboardButton[] { "Задать название группы"},
                        new KeyboardButton[] { "Показать расписание в определенный день", "Создать авторассылку" },
                        new KeyboardButton[] { "Список загруженных групп", "Скрыть меню" },
                    }
                    )
        {
            ResizeKeyboard = true
        };

        public ReplyKeyboardMarkup rkmReminderMenu = new ReplyKeyboardMarkup(
    new KeyboardButton[][]
    {
                        new KeyboardButton[] { "Включить/отключить авторассылку" },
                        new KeyboardButton[] { "Добавить новый сценарий"},
                        new KeyboardButton[] { "Удалить сценарий", "В главное меню" },
                        new KeyboardButton[] { "Список сценариев", "Скрыть меню"},
    }
    )
        {
            ResizeKeyboard = true
        };


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

        public string usage = "Использование:\nОткрыть главное меню:\n/openmenu\nОткрыть меню авторассылки:\n/switchreminder\n/Закрыть меню:\n/closemenu";
    }
}
