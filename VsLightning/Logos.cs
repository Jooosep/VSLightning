using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Console = Colorful.Console;
using System.Diagnostics;

namespace VsLightning
{
    class Logos
    {



        static readonly Stopwatch lightningAdvanceSw = new Stopwatch();

        //thunder
        private static int[] travelPoints = new int[12];
        private static int[] thunderLaunchPoints = new int[12];
        private static int[,] thunderPath = new int[12, 59];
        private static List<int> absorbedIndexes = new List<int>();
        private static List<int> hitIndexes = new List<int>();
        private static int thunderProgress;
        private static int thunderSpeed = 10;//normal 25
        private static int thunderVolume = 7;
        private static readonly int lightningRange = 100;
        private static Random lightningRandom = new Random();

        public static string[] JtLogo = {
"     [..[... [......     [.......                        [..                 [..                               ",
"     [..     [..         [..    [..                      [..                 [..   [.                          ",
"     [..     [..         [..    [..[. [...   [..         [..[..  [..   [...[.[. [.      [..    [.. [..   [....",
"     [..     [..         [.......   [..    [..  [..  [.. [..[..  [.. [..     [..  [.. [..  [..  [..  [..[..",
"     [..     [..         [..        [..   [..    [..[.   [..[..  [..[..      [..  [..[..    [.. [..  [..  [... ",
"[.   [..     [..         [..        [..    [..  [.. [.   [..[..  [.. [..     [..  [.. [..  [..  [..  [..    [..",
" [....       [..         [..       [...      [..     [.. [..  [..[..   [...   [.. [..   [..    [...  [..[.. [.."};
        public Logos()
        {

        }

        static void RecordPath(int i, int direction)
        {
            if (!absorbedIndexes.Contains(i) && !hitIndexes.Contains(i))
            {

                    thunderPath[i, thunderProgress] = travelPoints[i];
                    if (direction == 0)
                        Program.WriteXY(travelPoints[i], thunderProgress, "\\\n");
                    else if (direction == 1)
                        Program.WriteXY(travelPoints[i], thunderProgress, "/\n");
                    else
                        Program.WriteXY(travelPoints[i], thunderProgress, "|\n");

            }
        }

        static void ClearThunder(int maxY)
        {
            for (int i = 0; i < maxY; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            }
        }

        static void Thunder(int minY, int maxY, int minX, int maxX)
        {

            Console.ForegroundColor = Color.AntiqueWhite;
            int[] random = new int[12];
            if (thunderProgress == minY)
            {
                for (int i = 0; i < thunderVolume; i++)
                {
                    int span = maxX - minX;
                    thunderLaunchPoints[i] = minX + lightningRandom.Next(span + 1);
                    travelPoints[i] = thunderLaunchPoints[i];

                }
            }

            for (int i = 0; i < thunderVolume; i++)
            {
                random[i] = lightningRandom.Next(lightningRange);
                if (random[i] < 34 && travelPoints[i] < maxX - 2)
                {
                    travelPoints[i] += 1;
                    RecordPath(i, 0);
                }
                else if (random[i] < 67 && travelPoints[i] > minX + 2)
                {
                    travelPoints[i] -= 1;
                    RecordPath(i, 1);
                }
                else
                {
                    RecordPath(i, 2);
                }
                
            }
        }

        static bool AdvanceLightning(int maxY,int minX, int maxX)
        {

            if (thunderProgress < maxY && lightningAdvanceSw.ElapsedMilliseconds > thunderSpeed)
            {
                lightningAdvanceSw.Restart();
                int minY = 0;
                Thunder(minY, maxY, minX, maxX );

                thunderProgress++;

            }
            
            if (thunderProgress == maxY)
            {
                thunderProgress = 0;
                ClearThunder(maxY);
                return true;
            }
            
            return false;
        }

        public void PlayLogoAnimations()
        {
            Color altColor = Color.CadetBlue;
            Color mainColor = Color.AntiqueWhite;
            int windowH = Console.BufferHeight;
            int windowW = Console.BufferWidth;
            int endY = windowH / 2 - (JtLogo.Length / 2);
            int writeX = (windowW - JtLogo[0].Length) / 2;
            int writeY = 0;
            int counter = 0;
            int thunderMaxY = endY - 1;
            int thunderMinX = writeX;
            int thunderMaxX = writeX + JtLogo[0].Length;
            int logoLayerCount = JtLogo.Length;
            List<int> writeLayers = new List<int>();

            lightningAdvanceSw.Start();
            thunderProgress = 0;
            Program.audio.PlayChargeUp();
            Thread.Sleep(2000);
            //Console.BackgroundColor = Color.Black;
            //Console.Clear();
            while (true)
            { 
                Console.Clear();
                Console.SetCursorPosition(writeX, writeY);
                if(writeY < endY)
                {
                    writeY++;
                    Console.ForegroundColor = altColor;
                    for (int j = 0; j < logoLayerCount; j++)
                    {
                        if (!writeLayers.Contains(j))
                        {
                            Console.SetCursorPosition(writeX, writeY + j);
                            Console.Write(JtLogo[j]);
                        }
                    }
                    Thread.Sleep(10);
                }
                else
                {
                    Program.audio.StopChargeUp();
                    Console.ForegroundColor = mainColor;
                    if (writeLayers.Count < logoLayerCount)
                        writeLayers.Add(writeLayers.Count);
                    for (int j = 0; j < writeLayers.Count; j++)
                    {
                        Console.SetCursorPosition(writeX, writeY + j);
                        Console.Write(JtLogo[writeLayers[j]]);
                    }
                    Console.ForegroundColor = altColor;
                    for (int j = 0; j < logoLayerCount; j++)
                    {
                        if (!writeLayers.Contains(j))
                        {
                            Console.SetCursorPosition(writeX, writeY + j);
                            Console.Write(JtLogo[j]);
                        }
                    }
                    while (true)
                    {
                        if (AdvanceLightning(thunderMaxY, thunderMinX, thunderMaxX))
                        {
                            Program.audio.PlayShocker();
                            Console.ForegroundColor = mainColor;
                            if (writeLayers.Count < logoLayerCount)
                                writeLayers.Add(writeLayers.Count);
                            for (int j = 0; j < writeLayers.Count; j++)
                            {
                                Console.SetCursorPosition(writeX, writeY + j);
                                Console.Write(JtLogo[writeLayers[j]]);
                            }
                            Console.ForegroundColor = altColor;
                            for (int j = 0; j < logoLayerCount; j++)
                            {
                                if (!writeLayers.Contains(j))
                                {
                                    Console.SetCursorPosition(writeX, writeY + j);
                                    Console.Write(JtLogo[j]);
                                }
                            }
                            if (writeLayers.Count == logoLayerCount)
                            {
                                break;
                            }
                        }
                    }
                    Program.audio.PlayHighVoltage();
                    Thread.Sleep(100);
                    if (writeLayers.Count == logoLayerCount)
                    {
                        break;
                    }
                }
                
            }
            Console.ForegroundColor = mainColor;
            Thread.Sleep(3000);
            /*
            while (true)
            {
                Console.Clear();

                if (writeY < windowH)
                {
                    writeY++;
                    Console.ForegroundColor = mainColor;
                    if (writeLayers.Count < logoLayerCount)
                        writeLayers.Add(writeLayers.Count);
                    for (int j = 0; j < writeLayers.Count; j++)
                    {
                        if (writeY + j < windowH)
                        {
                            Console.SetCursorPosition(writeX, writeY + j);
                            Console.Write(JtLogo[writeLayers[j]]);
                        }
                    }
                    Thread.Sleep(10);
                }
                else
                {
                    break;
                }


            }
            Thread.Sleep(2000);
            */
        }
    }
}
