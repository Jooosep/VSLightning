using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using Console = Colorful.Console;
using System.Reflection;
using Cursor = System.Windows.Input.Cursor;

namespace VsLightning
{

    public static class Extensions
    {
        public static int LimitToRange(
            this int value, int inclusiveMinimum, int inclusiveMaximum)
        {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }
    }

    class Program
    {

        //special strings
        private static string shieldChar = ((char)7442).ToString();
        private static string runLeftStr = "/" + ((char)9492).ToString();
        private static string runRightStr = ((char)9496).ToString() + "\\";
        private static string sl = ((char)9472).ToString();
        private static string slingingArms = ((char)9552).ToString();
        private static string orb = ((char)9679).ToString();
        private static string hollowOrb = ((char)7439).ToString();
        private static string halfTail1 = ((char)9481).ToString();
        private static string halfTail2 = ((char)9481).ToString();
        private static string shockedLeg = ((char)7576).ToString();


        public enum GameMode
        {
            NORMAL,
            NEVERENDING
        }
        public static GameMode gameMode;
        public static bool endGameTrigger;
        public static int windowWidth;
        public static int windowHeight;
        public static int minY = 2;
        public static int maxY;
        public static int minX = 0;
        public static int maxX;

        //hero
        private static System.Threading.Timer runDelayTimer;
        private static int step = 0;
        private static int moveAmount = 0;
        private static int moveSpeed = 1000;
        private static int baseMoveSpeed = 1000;
        private static bool shield = false;
        private static bool shieldEnabled = true;
        private static bool multipleAbsorbtion;
        private static bool absorbed;
        private static bool direction = false;
        private static bool move = false;
        private static bool dead = false;
        public static int currentPosition;

        private static bool heroDrawn;
        private static int shieldCooldown;
        private static int shieldCooldownBV = 8000;
        public static int thunderGuard = 1;

        //orbthrow

        private static int aegisPosition;
        private static bool aegisInitialDirection;
        private static bool aegisLaunched;
        private static bool aegisDirection;
        private static int aegisMoveAmount;
        private static int aegisSpeed = 3000;
        private static System.Threading.Timer aegisTimer;
        private static int orbStep;
        private static int orbEnergyGathered;
        private static List<int> orbAbsorbedIndexes = new List<int>();
        private static bool orbUnlocked;
        private static int orbCooldown = 8000;//normal 8000


        //thunder
        private static int[] travelPoints = new int[12];
        private static int[] thunderLaunchPoints = new int[12];
        private static int[] thunderLaunches = new int[12];
        private static int[] thunderLaunchExtraDelays = new int[12];
        private static int[] thunderProgresses = new int[12];
        private static int[,] thunderPath = new int[12, 59];
        private static List<int> absorbedIndexes = new List<int>();
        private static List<int> hitIndexes = new List<int>();
        private static int thunderRemainCounter = 0;
        private static int thunderProgress;
        private static int thunderMaxY;
        private static int thunderInitialY;
        private static int thunderSpeed = 25;//normal 25
        private static int thunderFrequency = 1700;//normal 1700
        private static int thunderVolume = 2;
        private static readonly int thunderFrequencyBV = 1700;
        private static readonly int thunderVolumeBV = 2;
        private static readonly int thunderSpeedBV = 25;
        private static bool thunderLaunched;
        private static readonly int lightningRange = 100;
        private static Random lightningRandom = new Random();

        //trees
        public static int treeMaxHealth;
        public static int treeMaxHealthBV = 3;
        private static List<Tree> trees = new List<Tree>();
        private static bool seedEnabled = true;
        private static int seedCooldown;
        private static int seedCooldownBV = 20000;

        //audio
        public static Audio audio = new Audio();

        //sprint
        private static int sprintDuration = 5000;
        private static int sprintCooldown = 20000;
        private static int sprintSpeed;
        private static int sprintSpeedBV = 3000;

        //stopwatches;
        static readonly Stopwatch shieldCooldownSw = new Stopwatch();
        static readonly Stopwatch lightningAdvanceSw = new Stopwatch();
        static readonly Stopwatch lightningPausedSw = new Stopwatch();
        static readonly Stopwatch seedCooldownSw = new Stopwatch();
        static readonly Stopwatch sprintCooldownSw = new Stopwatch();
        static readonly Stopwatch lightningRemainSw = new Stopwatch();
        static readonly Stopwatch orbCooldownSw = new Stopwatch();

        //timers
        private static System.Timers.Timer cooldownUpdateTimer = new System.Timers.Timer(300);

        private static System.Timers.Timer moveTimer = new System.Timers.Timer(50);

        private static System.Timers.Timer sprintTimer = new System.Timers.Timer(sprintDuration);

        private static System.Timers.Timer lowerShieldTimer = new System.Timers.Timer(800);


        //perks
        private static bool orbPerk = false;
        private static bool lightningRunnerPerk = false;
        public static int harvesterPerk = 1;

        //gameprogress
        private static int points;

        //CooldownsUI

        private static UpdatedDisplay shieldCdDisplay;
        private static UpdatedDisplay sprintCdDisplay;
        private static UpdatedDisplay seedCdDisplay;
        private static UpdatedDisplay scoreDisplay;
        public static UpdatedDisplay thunderGuardDisplay;
        public static UpdatedDisplay orbCdDisplay;

        //progress screens
        private static string[] choice0 = new string[4];
        private static string[] choice1 = new string[4];
        private static string[] choice2 = new string[4];
        private static string[] choice3 = new string[4];
        private static string[] choice4 = new string[4];
        private static string[] choice5 = new string[4];
        private static string[] choice6 = new string[4];
        private static string[] endText = new string[7];
        private static int choiceMenuXPos;
        private static int choiceMenuYPos;



        public static void WriteXY(int x, int y, string str)
        {
            x = x.LimitToRange(0, Console.WindowWidth - str.Length - 1);
            y = y.LimitToRange(0, Console.WindowHeight - 1);
            Console.SetCursorPosition(x, y);
            Console.Write(str);
        }


        static void ClearTree(int pos)
        {
            for (int t = 1; t < 6; t++)
            {
                for (int i = 0; i < 7; i++)
                {
                    WriteXY(pos + 5 - i, maxY - t, "\b \b");
                }
            }
        }

        private static void PauseStopWatches()
        {
            sprintCooldownSw.Stop();
            seedCooldownSw.Stop();
            shieldCooldownSw.Stop();
        }
        private static void ResumeStopWatches()
        {
            sprintCooldownSw.Start();
            seedCooldownSw.Start();
            shieldCooldownSw.Start();
        }
        private static void InitTexts()
        {
            choice0[0] = "Improve running speed";
            choice0[1] = "Enhance seed quality so they grow more durable and fruitbearing trees, but increase cooldown by 5s";
            choice0[2] = "Shield cooldown -1 seconds and enable absorbtion of multiple lightning rays";
            choice0[3] = "";

            choice1[0] = "Shield cooldown -1 seconds";
            choice1[1] = "Seed cooldown -10 seconds enhance seed quality so they grow more durable trees";
            choice1[2] = "Improve sprinting speed";
            choice1[3] = "";

            choice2[0] = "Shield cooldown -2 second";
            choice2[1] = "Improve seed quality to grow more durable and fruitbearing trees";
            choice2[2] = "Improve sprinting speed";
            choice2[3] = "";

            choice3[0] = "Learn the orb skill, that allows you to launch an orb that collects thunderguards from lightningrays it passes (a key - left, d key - right)";
            choice3[1] = "Give up your shield to permanently sprint, lightning will no longer home into you";
            choice3[2] = "";
            choice3[3] = "";


            endText[0] = "[..      [........   [....   [........[...     [..[.....          [.       [.......    [..      [..";
            endText[1] = "[..      [..       [.    [.. [..      [. [..   [..[..   [..      [. ..     [..    [..   [..    [..";
            endText[2] = "[..      [..       [.    [.. [..      [. [..   [..[..   [..      [. ..     [..    [..   [..    [..";
            endText[3] = "[..      [......  [..        [......  [..  [.. [..[..    [..   [..   [..   [. [..          [..";
            endText[4] = "[..      [..      [..   [....[..      [..   [. [..[..    [..  [...... [..  [..  [..        [..";
            endText[5] = "[..      [..       [..    [. [..      [..    [. ..[..   [..  [..       [.. [..    [..      [..";
            endText[6] = "[........[........  [.....   [........[..      [..[.....    [..         [..[..      [..    [..";
        }

        static void GameProgress()
        {
            if (gameMode == GameMode.NORMAL)
            {
                if (points < 10)
                {
                    thunderSpeed--;
                    thunderFrequency = thunderFrequency - 60;
                }
                if (points == 20)
                {
                    thunderVolume = 2;
                }
                else if (points == 30)
                {
                    PauseStopWatches();
                    WriteXY(choiceMenuXPos, choiceMenuYPos, "Select an upgrade by pressing a number key");
                    WriteXY(choiceMenuXPos, choiceMenuYPos + 1, "1) " + choice0[0]);
                    WriteXY(choiceMenuXPos, choiceMenuYPos + 2, "2) " + choice0[1]);
                    WriteXY(choiceMenuXPos, choiceMenuYPos + 3, "3) " + choice0[2]);
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    audio.PauseStormBG();
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (int.TryParse(key.KeyChar.ToString(), out int num))
                        {
                            if (num > 0 && num < 4)
                            {
                                if (num == 1)
                                {
                                    if (moveSpeed == baseMoveSpeed)
                                    {
                                        moveSpeed += 1000;
                                    }
                                    baseMoveSpeed += 1000;


                                }
                                else if (num == 2)
                                {
                                    seedCooldown += 5000;
                                    harvesterPerk++;
                                    treeMaxHealth++;

                                }
                                else if (num == 3)
                                {
                                    shieldCooldown = shieldCooldown - 1000;
                                    multipleAbsorbtion = true;
                                }
                                break;
                            }
                        }
                    }
                    ClearThunder();
                    GameBorders();
                    ResumeStopWatches();
                    audio.ContinueStormBG();

                }
                else if (points == 50)
                {
                    thunderVolume = 3;
                    thunderSpeed = 12;
                }
                else if (points == 60)
                {
                    thunderFrequency -= 250;

                    PauseStopWatches();
                    aegisLaunched = false;
                    WriteXY(choiceMenuXPos, choiceMenuYPos, "Select an upgrade by pressing a number key");
                    WriteXY(choiceMenuXPos, choiceMenuYPos + 1, "1) " + choice1[0]);
                    WriteXY(choiceMenuXPos, choiceMenuYPos + 2, "2) " + choice1[1]);
                    WriteXY(choiceMenuXPos, choiceMenuYPos + 3, "3) " + choice1[2]);
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    audio.PauseStormBG();
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (int.TryParse(key.KeyChar.ToString(), out int num))
                        {
                            if (num > 0 && num < 4)
                            {
                                if (num == 1)
                                {
                                    shieldCooldown = shieldCooldown - 1000;

                                }
                                else if (num == 2)
                                {
                                    seedCooldown = seedCooldown - 10000;
                                    treeMaxHealth++;

                                }
                                else if (num == 3)
                                {
                                    sprintSpeed += 1000;
                                }
                                break;
                            }
                        }
                    }
                    ClearThunder();
                    GameBorders();
                    ResumeStopWatches();
                    audio.ContinueStormBG();

                }
                else if (points == 90)
                {
                    thunderVolume = 4;
                    thunderFrequency -= 200;
                }
                else if (points == 125)
                {

                    thunderFrequency -= 200;

                    PauseStopWatches();
                    aegisLaunched = false;
                    WriteXY(choiceMenuXPos, choiceMenuYPos, "Select an upgrade by pressing a number key");
                    audio.PauseStormBG();
                    for (int i = 0; i < 3; i++)
                    {
                        WriteXY(choiceMenuXPos, choiceMenuYPos + 1 + i, (i + 1).ToString() + ") " + choice2[i]);
                    }

                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (int.TryParse(key.KeyChar.ToString(), out int num))
                        {
                            if (num > 0 && num < 4)
                            {
                                if (num == 1)
                                {
                                    shieldCooldown = shieldCooldown - 2000;

                                }
                                else if (num == 2)
                                {
                                    treeMaxHealth = treeMaxHealth + 2;
                                    harvesterPerk = 2;
                                }
                                else if (num == 3)
                                {
                                    sprintSpeed += 1000;
                                }
                                break;
                            }
                        }
                    }
                    ClearThunder();
                    GameBorders();
                    ResumeStopWatches();
                    audio.ContinueStormBG();
                }
                else if (points == 200)
                {
                    thunderSpeed = 8;
                    thunderVolume = 5;
                    PauseStopWatches();
                    aegisLaunched = false;
                    WriteXY(choiceMenuXPos, choiceMenuYPos, "Select an upgrade by pressing a number key");
                    for (int i = 0; i < 2; i++)
                    {
                        WriteXY(choiceMenuXPos, choiceMenuYPos + 1 + i, (i + 1).ToString() + ") " + choice3[i]);
                    }
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    audio.PauseStormBG();
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (int.TryParse(key.KeyChar.ToString(), out int num))
                        {
                            if (num > 0 && num < 3)
                            {
                                if (num == 1)
                                {
                                    orbPerk = true;
                                    orbCooldownSw.Restart();
                                    orbCdDisplay.Write();

                                }
                                else if (num == 2)
                                {
                                    lightningRunnerPerk = true;
                                    shieldEnabled = false;
                                    shieldCdDisplay.Hide();
                                    moveSpeed = sprintSpeed;
                                }
                                break;
                            }
                        }
                    }
                    ClearThunder();
                    GameBorders();
                    ResumeStopWatches();
                    audio.ContinueStormBG();
                }
                else if (points == 200)
                {
                    thunderVolume = 6;
                }
                else if (points == 250)
                {
                    thunderVolume = 7;
                    thunderFrequency = 410;
                }
                else if (points == 350)
                {
                    thunderVolume = 8;
                }
                else if (points == 400)
                {
                    thunderFrequency = 350;
                    thunderSpeed = 5;
                }
                else if (points == 450)
                {
                    thunderVolume = 9;
                }
                else if (points == 500)
                {
                    runDelayTimer.Dispose();
                    aegisTimer.Dispose();
                    Console.BackgroundColor = Color.Aquamarine;

                    Console.ForegroundColor = Color.FromArgb(35,35,35);
                    audio.PauseStormBG();
                    audio.PlayEndBGM();
                    Thread.Sleep(1000);
                    for (int i = 0; i < maxY; i++)
                    {

                        Console.SetCursorPosition(0, i);
                        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
                        ClearHeroEven();
                        MoveHero(currentPosition);
                        Thread.Sleep(200);
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        WriteXY(maxX / 3, maxY / 3 + i, endText[i]);

                        Thread.Sleep(500);
                    }
                    Thread.Sleep(2000);

                    string congratulatoryString = "YOU MAY LIVE ETERNALLY!";
                    string proceedString = "PRESS ENTER TO RETURN TO MAIN MENU";
                    for (int i = 0; i < congratulatoryString.Length; i++)
                    {
                        WriteXY(maxX / 3, maxY / 2, congratulatoryString.Substring(0,i + 1));
                        Thread.Sleep(50);
                    }
                    Thread.Sleep(300);
                    for (int i = 0; i < proceedString.Length; i++)
                    {
                        WriteXY(maxX / 3, maxY / 2 + 2, proceedString.Substring(0, i + 1));
                        Thread.Sleep(50);
                    }

                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if(key.Key == ConsoleKey.Enter)
                        {
                            endGameTrigger = true;
                            audio.StopEndBGM();
                            Console.BackgroundColor = Color.FromArgb(35, 35, 35);
                            Console.ForegroundColor = Color.AntiqueWhite;
                            return;
                        }
                    }

                }
            }
            
        }
        static int AegisThrow()
        {
            moveAmount = 0;
            sprintCooldownSw.Stop();
            seedCooldownSw.Stop();
            int thunderGuardGained = 0;

            if (direction == false)
            {
                WriteXY(currentPosition, maxY - 4, "O");
                WriteXY(currentPosition - 1, maxY - 3, slingingArms + "|");
                WriteXY(currentPosition - 1, maxY - 2, "< \\");

                for (int i = currentPosition - 2; i > 1; i--)
                {
                    WriteXY(i, maxY - 3, "(");
                    for (int p = 0; p < currentPosition - 2 - i; p++)
                    {
                        Console.Write(sl);
                    }
                    Thread.Sleep(2);
                }

            }
            else
            {
                WriteXY(currentPosition, maxY - 4, "O");
                WriteXY(currentPosition, maxY - 3, "|" + slingingArms);
                WriteXY(currentPosition - 1, maxY - 2, "/ >");

                for (int i = currentPosition + 2; i < maxX; i++)
                {
                    Console.SetCursorPosition(currentPosition + 2, maxY - 3);
                    for (int p = currentPosition + 2; p < i; p++)
                    {
                        Console.Write(sl);
                    }
                    WriteXY(i, maxY - 3, ")");
                    Thread.Sleep(2);
                }
            }

            if (thunderProgress > maxY - 15)
            {
                for (int i = 0; i < thunderVolume; i++)
                {
                    if ((direction == false) ? ((travelPoints[i] < currentPosition)) : ((travelPoints[i] > currentPosition)))
                    {
                        thunderGuardGained++;
                        for (int t = thunderProgress; t < maxY - 1; t++)
                        {
                            WriteXY(travelPoints[i], t, "|");
                            Thread.Sleep(3);
                        }
                    }
                }

                for (int i = 0; i < thunderVolume; i++)
                {
                    if ((direction == false) ? ((travelPoints[i] < currentPosition)) : ((travelPoints[i] > currentPosition)))
                    {
                        for (int t = thunderProgress; t < maxY; t++)
                        {
                            WriteXY(travelPoints[i] + 1, t, "\b \b");
                            Thread.Sleep(3);
                        }
                    }
                }
            }
            
            if (direction == false)
            {
                for (int i = 1; i < currentPosition - 2; i++)
                {
                    WriteXY(i + 1, maxY - 3, "\b \b");
                    WriteXY(i + 1, maxY - 3, "(");
                    Thread.Sleep(2);
                }
            }
            else
            {
                for (int i = maxX - 1; i > currentPosition + 2; i--)
                {
                    WriteXY(i - 2, maxY - 3, ")");
                    WriteXY(i, maxY - 3, "\b \b");
                    Thread.Sleep(2);
                }
            }

            sprintCooldownSw.Start();
            seedCooldownSw.Start();
            shield = false;
            shieldCooldownSw.Restart();
            ClearHero(currentPosition);
            MoveHero(currentPosition);
            return thunderGuardGained;
        }
        static void SpeedLine()
        {
            string[] mSpeedLine = { sl + sl, sl + sl, sl + sl + sl };
            string[] lSpeedLine = { sl + sl, sl + sl + sl, sl + sl + sl };

            int[] startY = { maxY - 4, maxY - 3, maxY - 2 };
            if (direction == true)
            {

                if (sprintSpeed < 4000)
                {
                    int[] startX = { currentPosition - 2, currentPosition - 3, currentPosition - 4 };
                    for (int i = 0; i < 3; i++)
                    {
                        WriteXY(startX[i], startY[i], mSpeedLine[i]);
                    }

                }
                else
                {
                    int[] startX = { currentPosition - 2, currentPosition - 4, currentPosition - 5 };
                    for (int i = 0; i < 3; i++)
                    {
                        WriteXY(startX[i], startY[i], lSpeedLine[i]);
                    }
                }
            }
            else
            {
                int[] startX = { currentPosition, currentPosition + 1, currentPosition + 1 };
                if (sprintSpeed < 4000)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        WriteXY(startX[i], startY[i], mSpeedLine[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        WriteXY(startX[i], startY[i], lSpeedLine[i]);
                    }
                }
            }
        }

        static void ClearAegis()
        {
            if (aegisDirection)
            {
                WriteXY(aegisPosition, maxY - 3, "    ");
            }
            else
            {
                WriteXY(aegisPosition - 3, maxY - 3, "      ");
            }
        }
        static void DrawAegis()
        {
            if (orbStep % 10 < 5)
            {
                if (aegisDirection)
                {
                    WriteXY(aegisPosition, maxY - 3, sl + sl + sl + orb);
                }
                else
                {
                    WriteXY(aegisPosition - 1, maxY - 3, orb + sl + sl + sl);
                }
            }
            else
            {
                if (aegisDirection)
                {
                    WriteXY(aegisPosition, maxY - 3, halfTail1 + halfTail1 + halfTail1 + hollowOrb);
                }
                else
                {
                    WriteXY(aegisPosition - 1, maxY - 3, hollowOrb + halfTail2 + halfTail2 + halfTail2);
                }
            }
            if (orbStep > 10)
            {
                orbStep = 0;
            }

        }
        static void ClearHeroEven()
        {
            WriteXY(currentPosition - 4, maxY - 2, "         ");
            WriteXY(currentPosition - 4, maxY - 3, "         ");
            WriteXY(currentPosition - 4, maxY - 4, "         ");
            WriteXY(currentPosition - 4, maxY - 5, "         ");
        }
        static void ClearHero(int currentPosition)
        {

            /*
            if (moveSpeed > baseMoveSpeed || sprintCooldownSw.ElapsedMilliseconds < 100)
            {
                WriteXY(currentPosition - 11, maxY - 2, "                     ");
                WriteXY(currentPosition - 10, maxY - 3, "                    ");
                WriteXY(currentPosition - 8, maxY - 4, "                 ");
                WriteXY(currentPosition - 6, maxY - 5, "              ");
            }
            */
            if (moveSpeed > baseMoveSpeed || sprintCooldownSw.ElapsedMilliseconds < 100)
            {
                if (thunderProgress > maxY - 6)
                {
                    WriteXY(currentPosition - 11, maxY - 2, "                     ");
                    WriteXY(currentPosition - 10, maxY - 3, "                    ");
                    WriteXY(currentPosition - 8, maxY - 4, "                 ");
                    WriteXY(currentPosition - 6, maxY - 5, "              ");
                }
                else
                {
                    WriteXY(currentPosition - 30, maxY - 2, "                                                                                            ");
                    WriteXY(currentPosition - 30, maxY - 3, "                                                                                            ");
                    WriteXY(currentPosition - 30, maxY - 4, "                                                                                            ");
                    WriteXY(currentPosition - 30, maxY - 5, "                                                                                            ");
                }
            }
            else
            {
                WriteXY(currentPosition - 5, maxY - 2, "          ");
                WriteXY(currentPosition - 5, maxY - 3, "          ");
                WriteXY(currentPosition - 5, maxY - 4, "          ");
                WriteXY(currentPosition - 4, maxY - 5, "        ");
            }
            /*
            if(currentPosition > maxX - 12)
            {
                WriteXY(maxX - 4, maxY - 2, "    ");
                WriteXY(maxX - 4, maxY - 3, "    ");
                WriteXY(maxX - 4, maxY - 4, "    ");
                WriteXY(maxX - 4, maxY - 5, "    ");
            }
            */
        }
        static void MoveHero(int position)
        {
            //Console.ForegroundColor = Color.FromArgb(109, 193, 225);
            heroDrawn = false;

            if (!move && !dead)
            {
                heroDrawn = true;
                if (shield)
                {
                    Console.ForegroundColor = System.Drawing.Color.Aquamarine;
                    WriteXY(position, maxY - 5, shieldChar);
                    Console.ForegroundColor = System.Drawing.Color.AntiqueWhite;
                    WriteXY(position - 1, maxY - 4, "\\O/");
                    WriteXY(position, maxY - 3, "|");
                    WriteXY(position - 1, maxY - 2, "/ \\");
                }
                else
                {

                    WriteXY(position, maxY - 4, "O");
                    WriteXY(position - 1, maxY - 3, "/|\\");
                    WriteXY(position - 1, maxY - 2, "/ \\");
                }
            }
            else
            {
                if (dead)
                {
                    if (step % 2 == 0)
                    {
                        WriteXY(position, maxY - 4, "@");

                        WriteXY(position - 1, maxY - 3, sl + " " + sl);
                        WriteXY(position - 1, maxY - 2, shockedLeg + " \\");
                        step++;
                    }
                    else
                    {
                        WriteXY(position, maxY - 4, "O");
                        WriteXY(position - 1, maxY - 3, "/|\\");
                        WriteXY(position - 1, maxY - 2, "/ \\");
                        step++;
                    }
                    if (step > 10)
                    {
                        step = 0;
                    }
                }
                else if (direction == true)
                {

                    if (moveSpeed != baseMoveSpeed)
                    {
                        SpeedLine();
                    }

                    if (shield)
                    {
                        Console.ForegroundColor = System.Drawing.Color.Aquamarine;
                        WriteXY(position + 1, maxY - 5, shieldChar);
                        Console.ForegroundColor = System.Drawing.Color.AntiqueWhite;
                        WriteXY(position + 1, maxY - 4, "|O");
                        WriteXY(position + 1, maxY - 3, "/");
                    }
                    else
                    {
                        WriteXY(position, maxY - 4, " O");
                        WriteXY(position - 1, maxY - 3, "//");
                    }
                    if (step % 10 < 5)
                    {
                        WriteXY(position - 1, maxY - 2, " />");
                        step++;
                    }
                    else
                    {

                        WriteXY(position - 1, maxY - 2, runRightStr);
                        step++;

                    }

                    if (step > 9)
                    {
                        step = 0;
                    }

                }
                else if (direction == false)
                {
                    if (moveSpeed != baseMoveSpeed)
                    {
                        SpeedLine();
                    }
                    if (shield)
                    {
                        Console.ForegroundColor = System.Drawing.Color.Aquamarine;
                        WriteXY(position - 1, maxY - 5, shieldChar);
                        Console.ForegroundColor = System.Drawing.Color.AntiqueWhite;
                        WriteXY(position - 2, maxY - 4, "O|");
                        WriteXY(position - 1, maxY - 3, "\\");
                    }
                    else
                    {
                        WriteXY(position - 2, maxY - 4, "O");
                        WriteXY(position - 1, maxY - 3, "\\\\");
                    }
                    if (step % 10 < 5)
                    {

                        WriteXY(position - 1, maxY - 2, runLeftStr);
                        step++;

                    }
                    else
                    {
                        WriteXY(position - 1, maxY - 2, "<\\ ");
                        step++;
                    }
                }
            }
            DrawTrees();
        }


        static void RecordPath(int i, int direction)
        {
            if (!absorbedIndexes.Contains(i) && !hitIndexes.Contains(i))
            {

                thunderPath[i, thunderProgress - 3] = travelPoints[i];
                if (direction == 0)
                    WriteXY(travelPoints[i], thunderProgress, "\\\n");
                else if (direction == 1)
                    WriteXY(travelPoints[i], thunderProgress, "/\n");
                else
                    WriteXY(travelPoints[i], thunderProgress, "|\n");

            }
        }

        static void ClearThunder()
        {

            for (int i = 0; i < maxY - 4; i++)
            {
                Console.SetCursorPosition(0, i + 3);
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            }
            ClearHero(currentPosition);
            MoveHero(currentPosition);
            foreach (var tree in trees)
            {
                tree.lightningHasHit = false;
            }
        }
        static void Thunder()
        {

            Console.ForegroundColor = Color.GhostWhite;

            int closestTree = 1000;
            int closestTreePos = -1000;
            int[] random = new int[12];

            for (int i = 0; i < thunderVolume; i++)
            {
                random[i] = lightningRandom.Next(lightningRange);
            }
            if (thunderProgress == thunderInitialY)
            {
                for (int i = 0; i < thunderVolume; i++)
                {
                    thunderLaunchPoints[i] = random[i] + currentPosition - (lightningRange / 2);
                    if (thunderLaunchPoints[i] < 0)
                    {
                        thunderLaunchPoints[i] = Math.Abs(thunderLaunchPoints[i]);
                    }
                    if (thunderLaunchPoints[i] < 2)
                    {
                        thunderLaunchPoints[i] = 2;
                    }
                    if (thunderLaunchPoints[i] > windowWidth - 2)
                    {
                        thunderLaunchPoints[i] = (windowWidth - 2) - thunderLaunchPoints[i] % (windowWidth - 2);
                    }

                    if (i == 0)
                        travelPoints[i] = thunderLaunchPoints[i];
                    else
                    {
                        travelPoints[i] = thunderLaunchPoints[i] + (i * (lightningRandom.Next(200)) - (i * 100));
                        if (travelPoints[i] > windowWidth)
                        {
                            travelPoints[i] = travelPoints[i] % windowWidth;
                        }
                        else if (travelPoints[i] < minX)
                        {
                            travelPoints[i] = Math.Abs(travelPoints[i]) % windowWidth;
                        }
                    }
                }
                if(thunderVolume > 6)
                {
                    travelPoints[thunderVolume - 1] = currentPosition; 
                }
            }

            for (int i = 0; i < thunderVolume; i++)
            {

                if (thunderProgress > (int)(0.76 * maxY))
                {
                    foreach (var tree in trees)
                    {
                        if (tree.buildPhase == 4 && Math.Abs(tree.position - travelPoints[i]) < closestTree)
                        {
                            closestTree = Math.Abs(tree.position - travelPoints[i]);
                            closestTreePos = tree.position;
                        }
                    }

                    if (Math.Abs(closestTreePos - travelPoints[i]) < 70)
                    {
                        if (closestTreePos > travelPoints[i])
                        {
                            travelPoints[i] += 1;
                            RecordPath(i, 0);
                        }
                        else if (closestTreePos < travelPoints[i] && travelPoints[i] > 2)
                        {
                            travelPoints[i] -= 1;
                            RecordPath(i, 1);
                        }
                        else
                        {
                            RecordPath(i, 2);
                        }
                    }
                    else if(aegisLaunched && Math.Abs(aegisPosition - travelPoints[i]) < 70)
                    {
                        if (aegisPosition > travelPoints[i])
                        {
                            travelPoints[i] += 1;
                            RecordPath(i, 0);
                        }
                        else if (aegisPosition < travelPoints[i] && travelPoints[i] > 2)
                        {
                            travelPoints[i] -= 1;
                            RecordPath(i, 1);
                        }
                        else
                        {
                            RecordPath(i, 2);
                        }
                    }
                    else if (Math.Abs(currentPosition - travelPoints[i]) < 60 && !(lightningRunnerPerk))
                    {

                        if (currentPosition > travelPoints[i])
                        {
                            travelPoints[i] += 1;
                            RecordPath(i, 0);
                        }
                        else if (currentPosition < travelPoints[i] && travelPoints[i] > 2)
                        {
                            travelPoints[i] -= 1;
                            RecordPath(i, 1);
                        }
                        else
                        {
                            RecordPath(i, 2);
                        }
                    }
                    else
                    {
                        if (random[i] < 34 && travelPoints[i] < windowWidth - 2)
                        {
                            travelPoints[i] += 1;
                            RecordPath(i, 0);
                        }
                        else if (random[i] < 67 && travelPoints[i] > 2)
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
                else
                {
                    if (random[i] < 34 && travelPoints[i] < windowWidth - 2)
                    {
                        travelPoints[i] += 1;
                        RecordPath(i, 0);
                    }
                    else if (random[i] < 67 && travelPoints[i] > 2)
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
        }
        static void AdvanceLightning()
        {
            if (!thunderLaunched && lightningPausedSw.ElapsedMilliseconds > thunderFrequency)
            {
                thunderLaunched = true;
                lightningAdvanceSw.Restart();
            }
            if (thunderLaunched)
            {
                if (thunderProgress < thunderMaxY && lightningAdvanceSw.ElapsedMilliseconds > thunderSpeed)
                {
                    lightningAdvanceSw.Restart();

                    Thunder();
                    if (thunderProgress > maxY - 6)
                    {
                        MoveHero(currentPosition);
                    }
                    thunderProgress++;

                }
                else if (lightningRemainSw.ElapsedMilliseconds > 15)
                {
                    thunderRemainCounter++;
                    lightningRemainSw.Restart();

                }

                if (thunderProgress >= maxY - 4)
                {
                    for (int i = 0; i < thunderVolume; i++)
                    {
                        List<int> removeIndexes = new List<int>();
                        int treeIt = 0;
                        foreach (var tree in trees)
                        {
                            if (tree.buildPhase > 3 && (travelPoints[i] < tree.position + 7 && travelPoints[i] > tree.position - 7) && !tree.lightningHasHit)
                            {
                                if (tree.health == 1)
                                {
                                    audio.PlayExplosion();
                                    removeIndexes.Add(treeIt);
                                    MoveHero(currentPosition);
                                }
                                else
                                {
                                    //audio.PlayShock3();
                                    tree.health -= 1;
                                    tree.lightningHasHit = true;
                                }
                            }
                            treeIt++;
                        }
                        foreach (var it in removeIndexes)
                        {
                            trees.RemoveAt(it);
                        }
                    }
                    if (aegisLaunched)
                    {
                        for (int i = 0; i < thunderVolume; i++)
                        {
                            if (travelPoints[i] < aegisPosition + 5 && travelPoints[i] > aegisPosition - 5 && !orbAbsorbedIndexes.Contains(i))
                            {
                                orbEnergyGathered++;
                                audio.PlayAbsorb();
                                orbAbsorbedIndexes.Add(i);
                            }
                        }
                    }
                    for (int i = 0; i < thunderVolume; i++)
                    {
                        bool lastPointHit = travelPoints[i] < currentPosition + 3 && travelPoints[i] > currentPosition - 3;
                        bool upperPositionHit = false;
                        if(thunderProgress >= maxY -3)
                            upperPositionHit = thunderPath[i, maxY - 7] < currentPosition + 3 && thunderPath[i, maxY - 7] > currentPosition - 3;
                        bool middlePositionHit = false;
                        if (thunderProgress >= maxY - 2)
                            middlePositionHit = thunderPath[i, maxY - 6] < currentPosition + 3 && thunderPath[i, maxY - 6] > currentPosition - 3;
                        if (lastPointHit || middlePositionHit || upperPositionHit)
                        {
                            if (shield)
                            {
                                if (!absorbedIndexes.Contains(i) && !hitIndexes.Contains(i))
                                {
                                    if (!absorbed || multipleAbsorbtion)
                                    {
                                        audio.PlayAbsorb();
                                        thunderGuard++;
                                        thunderGuardDisplay.Update(thunderGuard.ToString());
                                        absorbedIndexes.Add(i);
                                        absorbed = true;
                                    }
                                    hitIndexes.Add(i);
                                    //OnThunderEnd();
                                }
                            }
                            else if (thunderGuard > 0)
                            {
                                if (!hitIndexes.Contains(i))
                                {
                                    audio.PlayPlayerHit();
                                    thunderGuard--;
                                    thunderGuardDisplay.Update(thunderGuard.ToString());
                                    hitIndexes.Add(i);
                                    //OnThunderEnd();
                                }
                            }
                            else if (!hitIndexes.Contains(i))
                            {
                                ClearHero(currentPosition);
                                dead = true;
                                MoveHero(currentPosition);
                                if (points < 10)
                                {
                                    audio.PlayShock1();
                                }
                                else if (points < 20)
                                {
                                    audio.PlayShock2();
                                }
                                else if (points > 19)
                                {
                                    audio.PlayShock3();
                                }

                                for (int j = 0; j < 20; j++)
                                {
                                    MoveHero(currentPosition);
                                    Thread.Sleep(30);
                                    ClearHero(currentPosition);
                                    Thread.Sleep(10);

                                }

                                OnGameEnd();
                                return;
                            }
                        }

                    }

                }
                if (thunderProgress == thunderMaxY)
                {
                    if (thunderRemainCounter == 0)
                    {
                        lightningRemainSw.Restart();
                        thunderRemainCounter++;
                    }
                    if (thunderRemainCounter > 10)
                        OnThunderEnd();
                }
            }
        }
        static void GameBorders()
        {
            //Console.ForegroundColor = Color.FromArgb(130, 90, 70);
            Console.ForegroundColor = Color.FromArgb(70, 90, 60);
            /*
            Console.SetCursorPosition(minX, minY);
            for (int i = 0; i < windowWidth; i++)
            {
                Console.Write((char)9523);
            }
            */

            Console.SetCursorPosition(minX, maxY - 1);
            for (int i = 0; i < windowWidth; i++)
            {
                Console.Write((char)9600);
            }
            /*
            Console.SetCursorPosition(minX, maxY);
            for (int i = 0; i < windowWidth; i++)
            {
                Console.Write((char)9477);
            }
            */
            Console.ForegroundColor = Color.AntiqueWhite;
        }
        static void UpdateCooldowns()
        {
            if (shieldEnabled)
            {
                if (shield)
                {
                    if (shieldCdDisplay.currentValue != "ACTIVE")
                        shieldCdDisplay.Update("ACTIVE");
                }
                else if ((int)shieldCooldownSw.ElapsedMilliseconds > shieldCooldown)
                {

                    if (shieldCdDisplay.currentValue != "READY")
                        shieldCdDisplay.Update("READY");
                }
                else
                {
                    string shieldCd = ((shieldCooldown - (int)shieldCooldownSw.ElapsedMilliseconds) / 1000 + 1).ToString();
                    if (shieldCd != shieldCdDisplay.currentValue)
                    {
                        shieldCdDisplay.Update(shieldCd);
                    }
                }
            }
            if (seedEnabled)
            {
                if ((int)seedCooldownSw.ElapsedMilliseconds > seedCooldown)
                {
                    if (seedCdDisplay.currentValue != "READY")
                        seedCdDisplay.Update("READY");
                }
                else
                {
                    string seedCd = ((seedCooldown - (int)seedCooldownSw.ElapsedMilliseconds) / 1000 + 1).ToString();
                    if (seedCd != seedCdDisplay.currentValue)
                    {
                        seedCdDisplay.Update(seedCd);
                    }
                }
            }

            if (!lightningRunnerPerk)
            {
                if (moveSpeed > baseMoveSpeed)
                {
                    if (sprintCdDisplay.currentValue != "ACTIVE")
                        sprintCdDisplay.Update("ACTIVE");
                }
                else if ((int)sprintCooldownSw.ElapsedMilliseconds > sprintCooldown)
                {
                    if (sprintCdDisplay.currentValue != "READY")
                        sprintCdDisplay.Update("READY");
                }
                else
                {
                    string sprintCd = ((sprintCooldown - (int)sprintCooldownSw.ElapsedMilliseconds) / 1000 + 1).ToString();
                    if (sprintCd != sprintCdDisplay.currentValue)
                    {
                        sprintCdDisplay.Update(sprintCd);
                    }
                }
            }
            else
            {
                if(gameMode == GameMode.NORMAL)
                    if (sprintCdDisplay.currentValue != "ACTIVE")
                        sprintCdDisplay.Update("ACTIVE");
            }
            if(orbPerk)
            {
                if(aegisLaunched)
                {
                    if (orbCdDisplay.currentValue != "ACTIVE")
                        orbCdDisplay.Update("ACTIVE");
                }
                else if ((int)orbCooldownSw.ElapsedMilliseconds > orbCooldown)
                {
                    if (orbCdDisplay.currentValue != "READY")
                        orbCdDisplay.Update("READY");
                }
                else
                {
                    string orbCd = ((orbCooldown - (int)orbCooldownSw.ElapsedMilliseconds) / 1000 + 1).ToString();
                    if (orbCd != orbCdDisplay.currentValue)
                    {
                        orbCdDisplay.Update(orbCd);
                    }
                }
            }

        }
        static void DrawTrees()
        {
            foreach (var tree in trees)
            {
                tree.WriteTree();
            }
        }

        static void SetInitialValues()
        {
            seedCooldown = seedCooldownBV;
            shieldCooldown = shieldCooldownBV;
            treeMaxHealth = treeMaxHealthBV;
            orbCdDisplay.Hide();
            Console.Clear();
            trees.Clear();
            shieldEnabled = true;
            shieldCdDisplay.Write();
            baseMoveSpeed = 1000;
            moveAmount = 0;
            move = false;
            orbPerk = false;
            if (gameMode == GameMode.NORMAL)
            {
                sprintSpeed = 3000;
                seedCdDisplay.Write();
                sprintCdDisplay.Write();
                sprintCooldownSw.Restart();
                seedCooldownSw.Restart();
                seedEnabled = true;
                lightningRunnerPerk = false;
                thunderFrequency = thunderFrequencyBV;
                thunderSpeed = thunderSpeedBV;
                thunderVolume = thunderVolumeBV;
                thunderGuard = 1;
                moveSpeed = baseMoveSpeed;
            }
            else
            {
                sprintCooldownSw.Reset();
                seedCooldownSw.Reset();
                seedEnabled = false;
                sprintSpeed = 5000;
                moveSpeed = sprintSpeed;
                lightningRunnerPerk = true;
                thunderVolume = 9;
                thunderSpeed = 7;
                thunderFrequency = 330;
                thunderGuard = 10;
            }
            points = 0;
            scoreDisplay.Write();
            thunderGuardDisplay.Write();

            shieldCooldownSw.Restart();
            lightningAdvanceSw.Restart();
            thunderLaunched = false;
            lightningPausedSw.Restart();
            GameBorders();

            UpdateCooldowns();
            audio.PlayStormBG();

            dead = false;
            currentPosition = maxX / 2;
            ClearHero(currentPosition);
            MoveHero(currentPosition);
            scoreDisplay.Update(points.ToString());
            
            thunderGuardDisplay.Update(thunderGuard.ToString());

            thunderProgress = thunderInitialY;

            absorbedIndexes.Clear();
            hitIndexes.Clear();
            thunderRemainCounter = 0;
            lightningRemainSw.Reset();
            orbAbsorbedIndexes.Clear();
            orbEnergyGathered = 0;
            aegisLaunched = false;
            absorbed = false;
            multipleAbsorbtion = false;
            
            harvesterPerk = 1;
            runDelayTimer = new Timer(AllowMove, null, 17, Timeout.Infinite);
            aegisTimer = new Timer(AegisMove, null, 5, Timeout.Infinite);
        }
        static void OnGameEnd()
        {
            runDelayTimer.Dispose();
            aegisTimer.Dispose();
            
            int writeX = windowWidth / 3;
            int writeY = windowHeight / 3;
            string infoStr = "YOU SURVIVED " + points + " STRIKES BEFORE FRYING.";
            string proceedStr = "PRESS ENTER TO PLAY AGAIN OR ESC TO RETURN TO THE MAIN MENU";
            for (int i = 0; i < infoStr.Length; i++)
            {
                WriteXY(writeX, writeY, infoStr.Substring(0, i + 1));
                Thread.Sleep(20);
            }
            Thread.Sleep(200);
            for (int i = 0; i < proceedStr.Length; i++)
            {
                WriteXY(writeX, writeY + 2, proceedStr.Substring(0, i + 1));
                Thread.Sleep(20);
            }
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            while (true)
            {
                var key = Console.ReadKey(true);
                if(key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if(key.Key == ConsoleKey.Escape)
                {
                    endGameTrigger = true;
                    return;
                }
            }
            SetInitialValues();

        }
        static void OnThunderEnd()
        {
            ClearThunder();
            thunderLaunched = false;
            thunderProgress = minY + 1;
            lightningPausedSw.Restart();
            points++;
            scoreDisplay.Update(points.ToString());
            //heroDrawn = false;
            GameProgress();
            absorbedIndexes.Clear();
            hitIndexes.Clear();
            thunderRemainCounter = 0;
            lightningRemainSw.Reset();
            orbAbsorbedIndexes.Clear();
            absorbed = false;
            
        }
        static void AllowWriteCooldowns(Object stateInfo, EventArgs e)
        {
            allowCooldownsReady = true;
        }
        static void EndSprint(Object stateInfo, EventArgs e)
        {
            if (!lightningRunnerPerk)
            {
                moveSpeed = baseMoveSpeed;
                sprintCooldownSw.Restart();
            }
        }

        static void AllowMove(Object stateInfo)
        {

            if (move)
            {
                if (direction)
                {
                    moveAmount += moveSpeed;
                }
                else
                {
                    moveAmount -= moveSpeed;
                }
            }

            runDelayTimer.Change(17, Timeout.Infinite);
        }
        static void AegisMove(Object stateInfo)
        {
            orbStep++;
            if (aegisLaunched)
            {
                if (aegisDirection)
                {
                    aegisMoveAmount += aegisSpeed;
                }
                else
                {
                    aegisMoveAmount -= aegisSpeed;
                }
            }
            aegisTimer.Change(5, Timeout.Infinite);
        }

        static void RememberMove(Object stateInfo, EventArgs e)
        {
            move = false;
        }
        static void LowerShield(Object stateInfo, EventArgs e)
        {
            shield = false;
            heroDrawn = false;

            shieldCooldownSw.Restart();
        }
        static void ReleaseShieldCooldown(Object stateInfo, EventArgs e)
        {

        }
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {

            public short X;
            public short Y;
            public COORD(short x, short y)
            {
                this.X = x;
                this.Y = y;
            }

        }
        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }
        private struct RECT
        {
            #region Variables.
            /// <summary>
            /// Left position of the rectangle.
            /// </summary>
            public int Left;
            /// <summary>
            /// Top position of the rectangle.
            /// </summary>
            public int Top;
            /// <summary>
            /// Right position of the rectangle.
            /// </summary>
            public int Right;
            /// <summary>
            /// Bottom position of the rectangle.
            /// </summary>
            public int Bottom;
            #endregion

            #region Operators.
            /// <summary>
            /// Operator to convert a RECT to Drawing.Rectangle.
            /// </summary>
            /// <param name="rect">Rectangle to convert.</param>
            /// <returns>A Drawing.Rectangle</returns>
            public static implicit operator Rectangle(RECT rect)
            {
                return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            /// <summary>
            /// Operator to convert Drawing.Rectangle to a RECT.
            /// </summary>
            /// <param name="rect">Rectangle to convert.</param>
            /// <returns>RECT rectangle.</returns>
            public static implicit operator RECT(Rectangle rect)
            {
                return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
            #endregion

            #region Constructor.
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="left">Horizontal position.</param>
            /// <param name="top">Vertical position.</param>
            /// <param name="right">Right most side.</param>
            /// <param name="bottom">Bottom most side.</param>
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
            #endregion
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD Size;
            public COORD CursorPosition;
            public short Attributes;
            public SMALL_RECT Window;
            public COORD MaximumWindowSize;
        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int handle);
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleDisplayMode(
                IntPtr ConsoleOutput
                , uint Flags
                , out COORD NewScreenBufferDimensions
                );

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleScreenBufferInfo(IntPtr hConsole, out CONSOLE_SCREEN_BUFFER_INFO info);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleWindowInfo(IntPtr hConsole, bool absolute, ref SMALL_RECT rect);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        [DllImport("user32.dll")]
        static extern bool ClipCursor(ref RECT lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr handle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;


        static int rightArrowKeyCode = 39;
        static int leftArrowKeyCode = 37;
        static int upArrowKeyCode = 38;
        static int downArrowKeyCode = 40;
        static int sArrowKeyCode = 83;
        static int dArrowKeyCode = 68;
        static int aArrowKeyCode = 65;

        static bool allowCooldownsReady = false;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            /*
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine(i + ": " + (char)i);
            }
            Console.ReadLine();
            */

            InitTexts();

            IntPtr ThisConsole = GetConsoleWindow();
            ShowWindow(ThisConsole, MAXIMIZE);
            IntPtr hConsole = GetStdHandle(-11);   // get console handle
            COORD xy = new COORD(100, 100);
            SetConsoleDisplayMode(hConsole, 1, out xy); // set the console to fullscreen

            Console.BackgroundColor = Color.FromArgb(35, 35, 35);
            Console.ForegroundColor = Color.AntiqueWhite;

            Console.CursorVisible = false;

            

            int moveRenewDelay = 30;
            int moveDuration = 50;
            
            //Timer updateLightning = new Timer(UpdateMove, null, 30, 30);
            windowHeight = Console.WindowHeight;
            windowWidth = Console.WindowWidth;
            Console.SetBufferSize(windowWidth, windowHeight);
            windowHeight = Console.WindowHeight;
            windowWidth = Console.WindowWidth;
            maxX = windowWidth;
            maxY = windowHeight - 4;
            thunderMaxY = maxY - 1;
            Console.SetBufferSize(windowWidth, windowHeight);

            RECT windowRect = new RECT();
            GetWindowRect(ThisConsole, out windowRect);
            int windowMaxX = windowRect.Right;
            RECT rect = new RECT(windowMaxX, 0, windowMaxX, 0);
            ClipCursor(ref rect);

            thunderInitialY = minY + 1;
            thunderProgress = thunderInitialY;
            currentPosition = maxX / 2;

            MainMenu menu = new MainMenu();
            Logos logos = new Logos();
            logos.PlayLogoAnimations();

            //init cdUI
            shieldCdDisplay = new UpdatedDisplay("SHIELD: ", (shieldCooldown / 1000).ToString(), maxX / 2, maxY + 1);
            seedCdDisplay = new UpdatedDisplay("SEED: ", (seedCooldown / 1000).ToString(), maxX / 2, maxY + 2);
            sprintCdDisplay = new UpdatedDisplay("SPRINT: ", (sprintCooldown / 1000).ToString(), maxX / 2, maxY + 3);
            scoreDisplay = new UpdatedDisplay("SCORE: ", "0", maxX / 5, maxY + 3);
            thunderGuardDisplay = new UpdatedDisplay("THUNDERGUARDS: ", "1", maxX / 5, maxY + 1);
            orbCdDisplay = new UpdatedDisplay("ORB: ", (orbCooldown / 1000).ToString(), (maxX / 2) + (maxX / 5), maxY + 1);
            choiceMenuXPos = maxX / 5;
            choiceMenuYPos = maxY / 3;

            Stopwatch moveRememberSw = new Stopwatch();

            while (true)
            {
                Console.Clear();
                audio.PlayMenuBGM();
                menu.Intro();
                audio.StopMenuBGM();

                SetInitialValues();
                
                cooldownUpdateTimer.Close();
                cooldownUpdateTimer = new System.Timers.Timer(300);
                cooldownUpdateTimer.Elapsed += AllowWriteCooldowns;
                cooldownUpdateTimer.AutoReset = true;
                cooldownUpdateTimer.Enabled = true;
                moveTimer.Close();
                moveTimer = new System.Timers.Timer(50);
                moveTimer.Elapsed += RememberMove;
                moveTimer.AutoReset = false;

                sprintTimer.Close();
                sprintTimer = new System.Timers.Timer(sprintDuration);
                sprintTimer.Elapsed += EndSprint;
                sprintTimer.AutoReset = false;
                lowerShieldTimer.Close();
                lowerShieldTimer = new System.Timers.Timer(800);
                lowerShieldTimer.Elapsed += LowerShield;
                lowerShieldTimer.AutoReset = false;

                moveRememberSw.Restart();

                //Thread.Sleep(30000);
                endGameTrigger = false;
                while (true)
                {

                    if (moveRememberSw.ElapsedMilliseconds > moveRenewDelay)
                    {
                        if (GetAsyncKeyState(rightArrowKeyCode) == Int16.MinValue)
                        {
                            move = true;
                            if (!direction)
                            {
                                ClearHero(currentPosition);
                                MoveHero(currentPosition);
                                direction = true;
                            }
                            moveRememberSw.Restart();
                        }
                        else if (GetAsyncKeyState(leftArrowKeyCode) == Int16.MinValue)
                        {
                            move = true;
                            if (direction)
                            {
                                ClearHero(currentPosition);
                                MoveHero(currentPosition);
                                direction = false;
                            }
                            moveRememberSw.Restart();

                        }
                        else if (moveRememberSw.ElapsedMilliseconds > moveDuration)
                        {
                            move = false;
                            moveAmount = 0;
                        }
                    }
                    if (!aegisLaunched && GetAsyncKeyState(dArrowKeyCode) == Int16.MinValue && orbPerk && orbCooldownSw.ElapsedMilliseconds > orbCooldown)
                    {
                        if (currentPosition < maxX - 6)
                        {
                            audio.PlayOrbLaunch();
                            orbCooldownSw.Restart();
                            aegisLaunched = true;
                            aegisDirection = true;
                            aegisInitialDirection = true;
                            aegisPosition = currentPosition + 2;
                        }
                    }
                    else if (!aegisLaunched && GetAsyncKeyState(aArrowKeyCode) == Int16.MinValue && orbPerk && orbCooldownSw.ElapsedMilliseconds > orbCooldown)
                    {

                        if (currentPosition > 6)
                        {
                            audio.PlayOrbLaunch();
                            orbCooldownSw.Restart();
                            aegisLaunched = true;
                            aegisDirection = false;
                            aegisInitialDirection = false;
                            aegisPosition = currentPosition - 2;
                        }
                    }
                    if (GetAsyncKeyState(downArrowKeyCode) == Int16.MinValue)
                    {
                        if (seedCooldownSw.ElapsedMilliseconds > seedCooldown)
                        {
                            trees.Add(new Tree(currentPosition));
                            seedCooldownSw.Restart();
                            audio.PlayPlantingSound();
                        }
                    }
                    if (GetAsyncKeyState(sArrowKeyCode) == Int16.MinValue)
                    {
                        if (moveSpeed != sprintSpeed && sprintCooldownSw.ElapsedMilliseconds > sprintCooldown && !lightningRunnerPerk)
                        {
                            moveSpeed = sprintSpeed;
                            sprintTimer.Enabled = true;
                        }
                        else if (moveSpeed == sprintSpeed)
                        {
                            move = false;
                        }
                    }

                    if (!shield && GetAsyncKeyState(upArrowKeyCode) == Int16.MinValue && shieldCooldownSw.ElapsedMilliseconds > shieldCooldown && shieldEnabled)
                    {
                        shield = true;
                        lowerShieldTimer.Enabled = true;
                        ClearHero(currentPosition);
                        MoveHero(currentPosition);
                    }
                    if (!move && !heroDrawn)
                    {
                        ClearHero(currentPosition);
                        MoveHero(currentPosition);
                    }

                    if (aegisLaunched)
                    {
                        ClearAegis();
                        int posChange = (aegisMoveAmount / 1000) / 1;
                        if (posChange < 0)
                        {
                            if (aegisPosition > 4)
                            {
                                aegisPosition += posChange;
                                if (aegisPosition < 4)
                                {
                                    aegisDirection = !aegisDirection;
                                }
                            }
                            else
                            {
                                aegisDirection = !aegisDirection;
                            }
                        }
                        else if (posChange > 0)
                        {
                            if (aegisPosition < maxX - 5)
                            {
                                aegisPosition += posChange;
                                if (aegisPosition > maxX - 5)
                                {
                                    aegisDirection = !aegisDirection;
                                }
                            }
                            else
                            {
                                aegisDirection = !aegisDirection;
                            }
                        }
                        aegisMoveAmount = 0;

                        if (aegisInitialDirection)
                        {
                            if (!aegisDirection && (aegisPosition < currentPosition || aegisPosition < currentPosition + 4))
                            {
                                thunderGuard += orbEnergyGathered;
                                orbEnergyGathered = 0;
                                thunderGuardDisplay.Update(thunderGuard.ToString());
                                aegisLaunched = false;
                                //audio.PlayOrbLaunch();
                            }
                            else
                                DrawAegis();
                        }
                        else
                        {
                            if (aegisDirection && (aegisPosition > currentPosition || aegisPosition > currentPosition - 4))
                            {
                                thunderGuard += orbEnergyGathered;
                                orbEnergyGathered = 0;
                                thunderGuardDisplay.Update(thunderGuard.ToString());
                                aegisLaunched = false;
                                //audio.PlayOrbLaunch();
                            }
                            else
                                DrawAegis();
                        }

                    }

                    if (move && moveAmount >= 999 || moveAmount <= -999)
                    {
                        int posChange = (moveAmount / 1000) / 1;
                        if (posChange < 0)
                        {
                            if (currentPosition > 4)
                            {
                                currentPosition += posChange;
                            }
                        }
                        else if (posChange > 0)
                        {
                            if (currentPosition < maxX - 5)
                            {
                                currentPosition += posChange;
                            }
                        }
                        moveAmount = 0;
                        ClearHero(currentPosition);
                        MoveHero(currentPosition);
                    }

                    AdvanceLightning();
                    if (endGameTrigger)
                    {
                        break;
                    }
                    DrawTrees();
                    if (allowCooldownsReady)
                    {
                        UpdateCooldowns();
                        cooldownUpdateTimer.Enabled = true;
                    }
                    
                }
                audio.PauseStormBG();
            }
        }
    }
}
