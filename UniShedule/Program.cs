using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UniShedule.Model;

namespace UniShedule
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = Database.Database.GetInstance();
            db.SaveLessons("А-05-19");
        }
    }
}
