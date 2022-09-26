using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniShedule.Model;

namespace UniShedule.Telegram
{
    class SKImageCreator
    {
        //public SKData DrawOneDaySchedule(List<Lesson> lessons)
        //{
        //    try
        //    {
        //        Console.WriteLine("Начало рисования картинки");
        //        SKImageInfo originalImageInfo = new SKImageInfo(700, 1000);
        //        SKImage originalImage;
        //        int h;
        //        int indent = 20;
        //        using (SKSurface surface = SKSurface.Create(originalImageInfo))
        //        {
        //            SKCanvas g = surface.Canvas;
        //            g.Clear(new SKColor(14, 22, 33));
        //            int i = 1;
        //            using (SKPaint paint = new SKPaint())
        //            using (SKTypeface tf = SKTypeface.FromFamilyName("Verdana"))
        //            {
        //                paint.Color = new SKColor(108, 67, 175);
        //                paint.IsAntialias = true;
        //                paint.TextSize = 18;
        //                paint.Typeface = tf;
        //                g.DrawText($"{lessons.First().Date.DateWeekName}, {lessons.First().Date.Date:M}",
        //                    new SKPoint(50, 20), paint);
        //                i++;
        //                paint.Color = SKColors.White;
        //                g.DrawLine(0, indent * i, originalImageInfo.Width, indent * i, paint);
        //                i++;
        //                foreach (var lesson in lessons)
        //                {
        //                    g.DrawText($"{lesson.Time}", new SKPoint(10, indent * (i + 2)), paint);
        //                    g.DrawText($"{lesson.Name}", new SKPoint(160, indent * i++), paint);
        //                    g.DrawText($"{lesson.Type}", new SKPoint(160, indent * i++), paint);
        //                    g.DrawText($"{lesson.Place}", new SKPoint(160, indent * i++), paint);
        //                    g.DrawText($"{lesson.Members}", new SKPoint(160, indent * i++), paint);
        //                    g.DrawText($"{lesson.Teacher}", new SKPoint(160, indent * i++), paint);
        //                    g.DrawLine(0, indent * i, originalImageInfo.Width, indent * i, paint);
        //                    i++;
        //                }
        //                h = indent * i;
        //            }
        //            originalImage = surface.Snapshot();
        //        }
        //        SKImageInfo destImageInfo = new SKImageInfo(450, h);
        //        using (SKSurface surface = SKSurface.Create(destImageInfo))
        //        {
        //            SKCanvas g = surface.Canvas;
        //            g.DrawImage(originalImage, new SKRect(0, 0, originalImageInfo.Width, h), new SKRect(0, 0, destImageInfo.Width, h));
        //            SKBitmap sKBitmap = SKBitmap.FromImage(surface.Snapshot());
        //            SKData sKData = sKBitmap.Encode(SKEncodedImageFormat.Png, 100);
        //            Console.WriteLine("Картинка создана");
        //            return sKData;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine($"Ошибка: {ex.Message}");
        //        return DrawOneDaySchedule(lessons);
        //    }
        //}

        public string PrintOneDaySchedule(List<Lesson> lessons)
        {
            StringBuilder str = new StringBuilder();
            str.Append($"{lessons.First().Date.DateWeekName}, {lessons.First().Date.Date:M}\n");
            foreach (var lesson in lessons)
            {
                str.Append($"{new string('-', 20)}\n");
                str.Append($"{lesson.Time}\n");
                str.Append($"{lesson.Name}\n");
                str.Append($"{lesson.Type}\n");
                str.Append($"{lesson.Place}\n");
                str.Append($"{lesson.Members}\n");
                str.Append($"{lesson.Teacher}\n");
            }
            return str.ToString();
        }

        //public void SaveImage(SKData sKData, string fileName)
        //{
        //    using (FileStream fileStream = File.Create(fileName))
        //    {
        //        sKData.AsStream().Seek(0, System.IO.SeekOrigin.Begin);
        //        sKData.AsStream().CopyTo(fileStream);
        //        fileStream.Flush();
        //        fileStream.Close();
        //        Console.WriteLine("Картинка отправлена в файл");
        //    }
        //}
    }
}
