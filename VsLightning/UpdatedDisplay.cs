using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsLightning
{
    class UpdatedDisplay
    {
        private string displayTitle;
        public string currentValue;
        private int xPos;
        private int yPos;
        public UpdatedDisplay(string title, string value, int x, int y)
        {
            displayTitle = title;
            currentValue = value;
            xPos = x;
            yPos = y;
        }

        public void Write()
        {
            Program.WriteXY(xPos, yPos, displayTitle + currentValue);
        }
        public void Update(string newValue)
        {
            currentValue = newValue;
            Program.WriteXY(xPos + displayTitle.Length, yPos, "      ");
            Program.WriteXY(xPos + displayTitle.Length, yPos, currentValue);
        }
        public void Hide()
        {
            Program.WriteXY(xPos, yPos, "                   ");

        }
    }
}
