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
                        new KeyboardButton[] { "Задать название группы" },
                        new KeyboardButton[] { "Задать дату" },
                        new KeyboardButton[] { "Показать расписание в определенный день" }
                    })
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

        public string usage = "Использование:\n" +
                                     "/openmenu   - открыть главное меню\n" +
                                     "/closemenu   - закрыть главной меню\n";
    }
}
