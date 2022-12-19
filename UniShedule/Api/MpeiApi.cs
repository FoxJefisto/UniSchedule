using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using UniShedule.Model;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace UniShedule
{
    class MpeiApi
    {
        private string mainUrl;
        private string nodeBtnPath;
        private RemoteWebDriver driver;
        private ChromeOptions options;
        private static MpeiApi instance;
        private static RestClient restClient;

        public static MpeiApi GetInstance()
        {
            if (instance == null)
                instance = new MpeiApi();
            return instance;
        }

        private MpeiApi()
        {
            mainUrl = @"https://mpei.ru/Education/timetable/Pages/default.aspx";
            nodeBtnPath = @"//div[@class='mpei-galaktika-group-form-control']";
            restClient = new RestClient();
            options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddArgument("disable-dev-shm-usage");
            options.AddArgument("window-size=1920x1080");
            Console.WriteLine("Драйвер получил данные о сервисе");
            driver = new RemoteWebDriver(new Uri("http://chrome:4444/wd/hub/"), options);
            driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);
        }

        private string GetUrlShedule(string groupName)
        {

            try
            {
                Console.WriteLine("Попытка браузера зайти на сайт");
                driver.Navigate().GoToUrl(mainUrl);
                Console.WriteLine("Браузер зашел на сайт");
                IWebElement element = driver.FindElement(By.XPath($"{nodeBtnPath}/input[@placeholder='Введите название группы']"));
                element.SendKeys(groupName);
                element = driver.FindElement(By.XPath($"{nodeBtnPath}/input[@type='submit']"));
                element.Click();
                Console.WriteLine("Браузер успешно получил ссылку группы");
                return driver.Url;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Thread.Sleep(1000);
                return GetUrlShedule(groupName);
            }

        }

        public List<Lesson> GetAllLessons(string groupName)
        {
            var url = GetUrlShedule(groupName);
            var lessons = new List<Lesson>();
            var matchGroupId = Regex.Match(url, @"groupoid=(\d+)");
            if (!matchGroupId.Groups[1].Success)
            {
                Console.WriteLine("Группы с таким названием не существует");
                throw new ArgumentException("Группы с таким названием не существует");
            }
            int groupId = Convert.ToInt32(matchGroupId.Groups[1].Value);
            Model.Group group = new Model.Group() { Id = groupId, Name = groupName };
            GetStartDates(out var startDate, out var limitDate);
            var currentDate = startDate;
            while (currentDate < limitDate)
            {
                lessons.AddRange(GetWeekLessons(url, group, currentDate));
                currentDate = currentDate.AddDays(7);
                Console.WriteLine($"Занятий схвачено: {lessons.Count}");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Данные о группе успешно считаны");
            return lessons;
        }

        private void GetStartDates(out DateTime startDate, out DateTime limitDate)
        {
            int startYear = DateTime.Now.Year;
            int startMonth, startDay;
            if (DateTime.Today.Month >= 8 || DateTime.Today.Month <= 1)
            {
                startMonth = 9;
                startDay = 1;
                if (DateTime.Today.Month <= 1)
                    startYear--;
                limitDate = new DateTime(startYear + 1, 1, 31);
            }
            else
            {
                startMonth = 2;
                startDay = 1;
                limitDate = new DateTime(startYear, 07, 15);
            }
            startDate = new DateTime(startYear, startMonth, startDay);
        }

        private List<Lesson> GetWeekLessons(string url, Model.Group group, DateTime currentDate)
        {
            string html;
            try
            {
                html = GetHTMLString(url + $"&start={currentDate.ToString("yyyy.MM.dd")}");
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var table = doc.DocumentNode.SelectNodes(".//table[@class='mpei-galaktika-lessons-grid-tbl']//tr");
                var lessons = new List<Lesson>();
                var lesson = new Lesson();
                DateTime date = default;
                string lessonTime = null;
                foreach (var row in table)
                {
                    var rowDate = row.SelectSingleNode("./td[@class='mpei-galaktika-lessons-grid-date']");
                    if (rowDate != null)
                    {
                        date = ParseDate(rowDate.InnerText, currentDate);
                        continue;
                    }


                    var rowTime = row.SelectSingleNode("./td[@class='mpei-galaktika-lessons-grid-time']");
                    if (rowTime != null)
                    {
                        lessonTime = rowTime.InnerText;
                    }
                    var rowLesson = row.SelectSingleNode("./td[@class='mpei-galaktika-lessons-grid-day']");
                    if (rowLesson != null)
                    {
                        lesson = new Lesson();
                        lesson.Time = lessonTime;
                        lesson.Group = group;
                        lesson.Date = date;
                        var rowLessonName = row.SelectSingleNode(".//span[@class='mpei-galaktika-lessons-grid-name']");
                        if (rowLessonName != null)
                        {
                            lesson.Name = rowLessonName.InnerText;
                        }
                        var rowLessonType = row.SelectSingleNode(".//span[@class='mpei-galaktika-lessons-grid-type']");
                        if (rowLessonType != null)
                        {
                            lesson.Type = rowLessonType.InnerText;
                        }
                        var rowLessonPlace = row.SelectSingleNode(".//span[@class='mpei-galaktika-lessons-grid-room']");
                        if (rowLessonPlace != null)
                        {
                            lesson.Place = rowLessonPlace.InnerText;
                        }
                        var rowLessonMembers = row.SelectSingleNode(".//span[@class='mpei-galaktika-lessons-grid-grp']");
                        if (rowLessonMembers != null)
                        {
                            lesson.Members = rowLessonMembers.InnerText;
                        }
                        var rowLessonTeacher = row.SelectSingleNode(".//span[@class='mpei-galaktika-lessons-grid-pers']");
                        if (rowLessonTeacher != null)
                        {
                            lesson.Teacher = rowLessonTeacher.InnerText;
                        }
                        lessons.Add(lesson);
                    }
                }
                return lessons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return GetWeekLessons(url, group, currentDate);
            }
        }

        private DateTime ParseDate(string rowInfo, DateTime currentDate)
        {
            var matchDate = Regex.Match(rowInfo, @"([А-Яа-я]+), (\d+) ([А-Яа-я]+)");
            int month = matchDate.Groups[3].Value switch
            {
                "января" => 1,
                "февраля" => 2,
                "марта" => 3,
                "апреля" => 4,
                "мая" => 5,
                "июня" => 6,
                "июля" => 7,
                "августа" => 8,
                "сентября" => 9,
                "октября" => 10,
                "ноября" => 11,
                "декабря" => 12,
                _ => 0
            };
            int year = currentDate.Year, day = Convert.ToInt32(matchDate.Groups[2].Value);
            if (currentDate.Month == 12 && month == 1)
                year++;
            var date = new DateTime(year, month, day);
            return date;
        }

        private string GetHTMLString(string url)
        {
            return restClient.GetStringAsync(url).GetAwaiter().GetResult();
        }

    }
}
