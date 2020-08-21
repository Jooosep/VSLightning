using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Console = Colorful.Console;

namespace VsLightning
{
    class Tree
    {
        private static string tb = ((char)9617).ToString();
        private static string tf = ((char)1423).ToString();
        private string[] treeStrings = { ((char)9553).ToString(), ((char)9553).ToString(), ((char)9553).ToString(), tb + tb + tb + tb + tb, tb + tb + tb};
        private System.Timers.Timer growTimer;
        private System.Timers.Timer fruitTimer;
        public bool readyToGrow;
        public bool fruit;
        public int buildPhase;
        public int position;
        public int health;
        public bool lightningHasHit = false;
        
        public Tree(int pos)
        {
            position = pos;
            buildPhase = -1;
            fruit = false;
            readyToGrow = false;
            growTimer = new System.Timers.Timer(2000);
            growTimer.Elapsed += AllowGrow;
            growTimer.Enabled = true;
            growTimer.AutoReset = false;
            if (Program.harvesterPerk == 1)
            {
                fruitTimer = new System.Timers.Timer(10000);
            }
            else if(Program.harvesterPerk == 2)
            {
                fruitTimer = new System.Timers.Timer(5000);
            }
            else
            {
                fruitTimer = new System.Timers.Timer(2500);
            }
            fruitTimer.Elapsed += GrowFruit;
            fruitTimer.AutoReset = false;
            health = Program.treeMaxHealth;
        }

        private void AllowGrow(Object source, EventArgs e)
        {
            readyToGrow = true;
        }
        private void GrowFruit(Object source, EventArgs e)
        {
            fruit = true;
        }

        public void WriteTree()
        {

            if(readyToGrow)
            {
                readyToGrow = false;
                buildPhase++;
                if(buildPhase < 4)
                {
                    growTimer.Enabled = true;
                }
                else
                {
                    fruitTimer.Enabled = true;
                }
                
            }
            
            if (buildPhase >= 0)
            {
                int[] treeY = new int[5];
                for (int i = 0; i < 5; i++)
                {
                    treeY[i] = Program.maxY - (i + 2);
                }
                int[] treeX = { position, position, position, position - 2, position - 1 };

                if (buildPhase < 5)
                {
                    for (int i = 0; i < buildPhase + 1; i++)
                    {
                        Program.WriteXY(treeX[i], treeY[i], treeStrings[i]);
                    }

                    
                    if (fruit)
                    {
                        Program.WriteXY(position + 1, Program.maxY - 4, tf);
                        if(Program.currentPosition - 2 < position + 1 && Program.currentPosition + 2 > position + 1)
                        {
                            Program.thunderGuard++;
                            Program.thunderGuardDisplay.Update(Program.thunderGuard.ToString());
                            Program.audio.PlayNom();
                            fruit = false;
                            fruitTimer.Enabled = true;
                        }
                    }

                }

            }
            
        }
  
    }

}
