using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParkingSimulation.SerializableEntities;
using ParkingSimulation.Displayers;

namespace ParkingSimulation
{
    public partial class Displayer : Form
    {
        private Size mainWindowSize = new Size(1920, 1010);

        private UserConsoleDisplayer userConsoleHandler;
        private ExtraInfoDisplayer   extraInfoHandler;
        private ParkingDisplayer     parkingHandler;

        public Displayer(ManagmentCore mCore, ParkingMap map)
        {
            InitializeComponent();

            userConsoleHandler = new UserConsoleDisplayer(mCore, console, mainWindowSize, map.GetParkingSize());
            extraInfoHandler   = new ExtraInfoDisplayer  (extraInfoLabel, mainWindowSize, map.GetParkingSize());
            parkingHandler     = new ParkingDisplayer    (mCore, parking, mainWindowSize, map);
        }

        public void Create()
        {
            parkingHandler  .Create();
            extraInfoHandler.Create();
        }

        public void Reset()
        {
            parkingHandler  .Reset();
            extraInfoHandler.Reset();
        }

        public void ClearConsole()
        {
            userConsoleHandler.Clear();
        }

        public void InstructUser(Dictionary<string, string> instructionsDescription)
        {
            userConsoleHandler.DisplayUserInstruction(instructionsDescription);
        }

        public void PostResponseStatus(string message, ResponseStatus status)
        {
            userConsoleHandler.DisplayResponse(message, status);
        }

        public void BlockConsole()
        {
            userConsoleHandler.GlobalBlock = true;
            userConsoleHandler.Block(true);
        }

        public void UnblockConsole()
        {
            userConsoleHandler.GlobalBlock = false;
            userConsoleHandler.Block(false);
        }

        public void UpdateExtraInfo(double revenue)
        {
            extraInfoHandler.UpdateRevenue(revenue);
        }

        public void PrepareUserChoose(Dictionary<Point, ParkingCell> employedSpaces,
                                      ParkingCell entrance,
                                      Dictionary<Point, ParkingCell> freeSpaces)
        {
            parkingHandler.HighLightSpaces(freeSpaces);
            parkingHandler.DrawEmployedSpaces(employedSpaces);

            if (entrance.Vehicle != null)
                parkingHandler.DrawEntrance(entrance);
        }

        public void HandleMoveProcess(Dictionary<Point, ParkingCell> employedSpaces,
                                      ParkingCell start,
                                      ParkingCell finish)
        {
            parkingHandler.DisplayVehicleMove(employedSpaces, start, finish);
        }

        public void ReDrawParking(Dictionary<Point, ParkingCell> employedSpaces)
        {
            parkingHandler.Invalidate(parkingHandler.DefaultBitmap);
            parkingHandler.FlushSpaces();
            parkingHandler.DrawEmployedSpaces(employedSpaces);
        }
    }
}
