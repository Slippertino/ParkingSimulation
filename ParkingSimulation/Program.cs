using System;

namespace ParkingSimulation
{
    static class Program
    {
        private const string parkingMapPath = @"../../../parking_map.xml";

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new ManagmentCore(parkingMapPath).Run();
        }
    }
}
