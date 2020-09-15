using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Console = Colorful.Console;

namespace VsLightning
{
    class MainMenu
    {

        public MainMenu()
        {

        }

        
        public void Intro()
        {
            string[] titleText = new string[7];
            string[] controls = new string[7];
            
            //tiles font
            titleText[0] = "[..         [..  [.. ..          [..      [..   [....   [..     [..[... [......[...     [..[..[...     [..   [....";
            titleText[1] = " [..       [.. [..    [..        [..      [.. [.    [.. [..     [..     [..    [. [..   [..[..[. [..   [.. [.    [..";
            titleText[2] = "  [..     [..   [..              [..      [..[..        [..     [..     [..    [.. [..  [..[..[.. [..  [..[..";
            titleText[3] = "   [..   [..      [..            [..      [..[..        [...... [..     [..    [..  [.. [..[..[..  [.. [..[..";
            titleText[4] = "    [.. [..          [..         [..      [..[..   [....[..     [..     [..    [..   [. [..[..[..   [. [..[..   [....";
            titleText[5] = "     [....     [..    [..        [..      [.. [..    [. [..     [..     [..    [..    [. ..[..[..    [. .. [..    [.";
            titleText[6] = "      [..        [.. ..  [..     [........[..  [.....   [..     [..     [..    [..      [..[..[..      [..  [.....";

            controls[0] = "CONTROLS:";
            controls[1] = "You can move left and right with arrow keys.";
            controls[2] = "The down arrow key plants a magical seed that grows into a tree.";
            controls[3] = "The up arrow key raises the lightning shield, that absorbs lightning strikes and grants you one thunderguard, that can protect you from future strikes.";
            controls[4] = "The s key initiates a sprint that will last a certain duration. If already sprinting, it allows you to stop quickly";
            controls[5] = "If the orb skill is acquired, it can be launched with the a key in the left direction, or with the d key to the right direction";

            string[] menuBGMCreditStrs = { "Menu BGM: Whitesand - Oblivion", "Licensed under a Creative Commons license" };
            string[] endBGMCreditStrs = { "Victory screen BGM: Ross Bugden - Run", "Licensed under a Creative Commons license" };
            int windowH = Console.BufferHeight;
            int windowW = Console.BufferWidth;

            void DisplayCredits()
            {
                Console.Clear();
                int x = (windowW - menuBGMCreditStrs[1].Length) / 2;
                int y = windowH / 3;

                Program.WriteXY(x,y, menuBGMCreditStrs[0]);
                Program.WriteXY(x, y + 1, menuBGMCreditStrs[1]);

                Program.WriteXY(x, y + 5, endBGMCreditStrs[0]);
                Program.WriteXY(x, y + 6, endBGMCreditStrs[1]);

                while (true)
                {
                    var key = Console.ReadKey(true);
                    if(key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
                Console.Clear();
            }

            while (true)
            {
                int writeX = (windowW - titleText[0].Length) / 2;
                if (writeX < 0)
                    writeX = 0;
                int writeY = windowH / 6;
                for (int i = 0; i < 7; i++)
                {
                    titleText[i] = titleText[i].Truncate(windowW - 1);
                    Program.WriteXY(writeX, writeY + i, titleText[i]);
                }
                writeY = windowH / 2 - (controls.Length / 2);
                writeX = (windowW - controls[5].Length) / 2;
                if (writeX < 0)
                    writeX = 0;

                for (int i = 0; i < 6; i++)
                {

                    controls[i] = controls[i].Truncate(windowW - 1);
                    Program.WriteXY(writeX, writeY + i, controls[i]);
                }




                string normalModeStr = "NORMAL MODE - Lightning gets progressively more dangerous. You can upgrade your abilities to survive until the skies clear up";
                string neverendingModeStr = "NEVERENDING MODE - Test your might against the fury of the Thunder God, it never ends";
                string creditsStr = "CREDITS";
                string quitStr = "QUIT GAME";

                normalModeStr = normalModeStr.Truncate(windowW - 1);
                neverendingModeStr = neverendingModeStr.Truncate(windowW - 1);
                //writeX = (windowW - normalModeStr.Length) / 2;
                writeY = 2 * windowH / 3;

                Color selectedColor = Color.CadetBlue;
                Console.ForegroundColor = selectedColor;
                Program.WriteXY(writeX, writeY, normalModeStr);
                Console.ForegroundColor = Color.AntiqueWhite;
                Program.WriteXY(writeX, writeY + 4, neverendingModeStr);
                Program.WriteXY(writeX, writeY + 8, creditsStr);
                Program.WriteXY(writeX, writeY + 12, quitStr);

                int menuItem = 0;
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (menuItem == 0)
                        {
                            menuItem = 3;
                        }
                        else
                        {
                            menuItem--;
                        }
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (menuItem == 3)
                        {
                            menuItem = 0;
                        }

                        else
                        {
                            menuItem++;
                        }
                    }

                    if (menuItem == 0)
                    {
                        Console.ForegroundColor = selectedColor;
                        Program.WriteXY(writeX, writeY, normalModeStr);
                        Console.ForegroundColor = Color.AntiqueWhite;
                    }
                    else
                    {
                        Program.WriteXY(writeX, writeY, normalModeStr);
                    }
                    if (menuItem == 1)
                    {
                        Console.ForegroundColor = selectedColor;
                        Program.WriteXY(writeX, writeY + 4, neverendingModeStr);
                        Console.ForegroundColor = Color.AntiqueWhite;
                    }
                    else
                    {
                        Program.WriteXY(writeX, writeY + 4, neverendingModeStr);
                    }
                    if (menuItem == 2)
                    {
                        Console.ForegroundColor = selectedColor;
                        Program.WriteXY(writeX, writeY + 8, creditsStr);
                        Console.ForegroundColor = Color.AntiqueWhite;
                    }
                    else
                    {
                        Program.WriteXY(writeX, writeY + 8, creditsStr);
                    }
                    if (menuItem == 3)
                    {
                        Console.ForegroundColor = selectedColor;
                        Program.WriteXY(writeX, writeY + 12, quitStr);
                        Console.ForegroundColor = Color.AntiqueWhite;
                    }
                    else
                    {
                        Program.WriteXY(writeX, writeY + 12, quitStr);
                    }

                }
                if (menuItem == 3)
                {
                    System.Environment.Exit(1);
                }
                else if (menuItem == 2)
                {
                    DisplayCredits();
                }
                else
                {
                    Program.gameMode = (Program.GameMode)menuItem;
                    Console.Clear();
                    return;
                }
            }
        }
    }
}
