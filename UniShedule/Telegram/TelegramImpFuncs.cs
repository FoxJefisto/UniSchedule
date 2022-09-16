using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UniShedule.Model;

namespace UniShedule.Telegram
{
    class TelegramImpFuncs
    {
        public Bitmap DrawOneDaySchedule(List<Lesson> lessons)
        {
            Bitmap bmp = new Bitmap(550, 1000, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int h;
            using (Graphics g = Graphics.FromImage(bmp))
            using (Font font = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point))
            using (Brush brushTitle = new SolidBrush(Color.FromArgb(108, 67, 175)))
            using (Brush brushBackground = new SolidBrush(Color.FromArgb(14, 22, 33)))
            using (Pen pen = new Pen(Brushes.White))
            {
                g.FillRectangle(brushBackground, 0, 0, 550, 1000);
                int i = 1;
                g.DrawString($"{lessons.First().Date.DateWeekName}, {lessons.First().Date.Date:M}", font, brushTitle, new PointF(50, 0));
                i++;
                g.DrawLine(pen, 0, 15 * i, 550, 15 * i);
                i++;
                foreach (var lesson in lessons)
                {
                    g.DrawString($"{lesson.Time}", font, Brushes.White, new PointF(30, 15 * (i + 2)));
                    g.DrawString($"{lesson.Name}", font, Brushes.White, new PointF(150, 15 * i++));
                    g.DrawString($"{lesson.Type}", font, Brushes.White, new PointF(150, 15 * i++));
                    g.DrawString($"{lesson.Place}", font, Brushes.White, new PointF(150, 15 * i++));
                    g.DrawString($"{lesson.Members}", font, Brushes.White, new PointF(150, 15 * i++));
                    g.DrawString($"{lesson.Teacher}", font, Brushes.White, new PointF(150, 15 * i++));
                    i++;
                    g.DrawLine(pen, 0, 15 * i, 550, 15 * i);
                    i++;
                }
                h = 15 * i;
            }
            Bitmap target = new Bitmap(420, h);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, target.Width, target.Height),
                                 0, 0, 550, h,
                                 GraphicsUnit.Pixel);
            }
            return target;
        }
    }
}
