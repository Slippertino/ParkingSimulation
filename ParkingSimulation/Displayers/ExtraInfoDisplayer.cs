using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;

namespace ParkingSimulation.Displayers
{
    public class ExtraInfoDisplayer
    {
        private Label displayerExtraInfo;

        private const string prefixString = "Текущая заработанная сумма: ";
        private const Single maxFontSize = 24F;
        private readonly Size maxSize;

        private double revenue = 0.0;

        public ExtraInfoDisplayer(Label extraInfoLabel, Size mainWindowSize, Size parkingSize)
        {
            displayerExtraInfo = extraInfoLabel;

            displayerExtraInfo.Location = new Point(0, 0);

            displayerExtraInfo.Visible = false;

            maxSize = new Size(parkingSize.Width - 1, mainWindowSize.Height - parkingSize.Height - 1);
        }

        public void Create()
        {
            displayerExtraInfo.Visible = true;

            UpdateInfo();
        }

        public void Reset()
        {
            displayerExtraInfo.Visible = false;

            revenue = 0.0;
        }

        private void SelectSize()
        {
            while (Operations.IsContained(displayerExtraInfo.Size, maxSize) || displayerExtraInfo.Font.Size > maxFontSize)
            {
                displayerExtraInfo.Font = new System.Drawing.Font("Microsoft Sans Serif",
                                                                  displayerExtraInfo.Font.Size - 1,
                                                                  System.Drawing.FontStyle.Bold,
                                                                  System.Drawing.GraphicsUnit.Point,
                                                                  ((byte)(204)));
            }
        }

        private void UpdateInfo()
        {
            displayerExtraInfo.Visible = false;

            displayerExtraInfo.Text = prefixString + Convert.ToString(revenue);

            SelectSize();

            displayerExtraInfo.Visible = true;
        }

        public void UpdateRevenue(double profit)
        {
            try
            {
                revenue += profit;
            }
            catch(Exception)
            {
                Reset();
            }

            UpdateInfo();
        }
    }
}
