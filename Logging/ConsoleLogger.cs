using System;
using Castle.Windsor;
using UnityEngine;

namespace ImperialStudio.Core.Logging
{
    public class ConsoleLogger : FormattedLogger
    {
        public override string ServiceName => "Console";

        protected override void WriteColored(string format, Color? color = null, params object[] bindings)
        {
            if (color == null)
            {
                Console.ResetColor();
                Console.Write(format, bindings);
                return;
            }

            Color orgCol = GetForegroundColor();
            SetForegroundColor(color.Value);
            Console.Write(format, bindings);
            SetForegroundColor(orgCol);
        }

        protected override void WriteLineColored(string format, Color? color = null, params object[] bindings)
        {
            if (color == null)
            {
                Console.ResetColor();
                Console.WriteLine(format, bindings);
                return;
            }

            Color orgCol = GetForegroundColor();
            SetForegroundColor(color ?? orgCol);
            Console.WriteLine(format, bindings);
            SetForegroundColor(orgCol);
        }


        public static Color GetForegroundColor()
        {
            int[] cColors =
            {
                0x000000, //Black = 0
                0x000080, //DarkBlue = 1
                0x008000, //DarkGreen = 2
                0x008080, //DarkCyan = 3
                0x800000, //DarkRed = 4
                0x800080, //DarkMagenta = 5
                0x808000, //DarkYellow = 6
                0xC0C0C0, //Gray = 7
                0x808080, //DarkGray = 8
                0x0000FF, //Blue = 9
                0x00FF00, //Green = 10
                0x00FFFF, //Cyan = 11
                0xFF0000, //Red = 12
                0xFF00FF, //Magenta = 13
                0xFFFF00, //Yellow = 14
                0xFFFFFF  //White = 15
            };

            byte[] colBytes = BitConverter.GetBytes(cColors[(int) Console.ForegroundColor]);
            return new Color(colBytes[1], colBytes[2], colBytes[3], colBytes[0]);
        }

        public void SetForegroundColor(Color color)
        {
            lock (OutputLock)
            {
                ConsoleColor ret = 0;
                double rr = color.r, gg = color.g, bb = color.b, delta = double.MaxValue;

                foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
                {
                    string n = Enum.GetName(typeof(ConsoleColor), cc);
                    ColorUtility.TryParseHtmlString(n == "DarkYellow" ? "Orange" : n, out var c); // bug fix
                    double t = Math.Pow(c.r - rr, 2.0) + Math.Pow(c.g - gg, 2.0) + Math.Pow(c.b - bb, 2.0);
                    if (t == 0.0)
                    {
                        Console.ForegroundColor = cc;
                        return;
                    }

                    if (t < delta)
                    {
                        delta = t;
                        ret = cc;
                    }
                }

                Console.ForegroundColor = ret;
            }
        }

        public ConsoleLogger(IWindsorContainer container) : base(container) { }
    }
}