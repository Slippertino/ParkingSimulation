using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParkingSimulation.SerializableEntities;
using System.Drawing;

namespace ParkingSimulation
{
    public class ParkingCell
    { 
        public CellParams CellSettings { get; }
        public Vehicle Vehicle { get; set; } = null;

        public ParkingCell(CellParams cellSettings)
        {
            CellSettings = cellSettings;
        }

        public ParkingCell(ParkingCell cell)
        {
            CellSettings = cell.CellSettings;
            Vehicle      = new Vehicle(cell.Vehicle);
        }

        public string GetSpaceInfo()
        {
            return (Vehicle == null)
                     ? "Free parking space "
                     : Vehicle.GetGenericInfo();
        }
    }
}
