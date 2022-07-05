using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ParkingSimulation.SerializableEntities;
using System.Xml.Serialization;

namespace ParkingSimulation
{
    public class Parking
    {
        public Dictionary<Point, ParkingCell> FreeSpaces     { get; } = new Dictionary<Point, ParkingCell>();
        public Dictionary<Point, ParkingCell> EmployedSpaces { get; } = new Dictionary<Point, ParkingCell>();

        public ParkingCell Entrance { get; }

        public bool IsCreated { get; set; } = false;

        public Parking(ParkingMap pMap)
        {
            var spaces = pMap.GetParkingSpaces();

            Entrance = new ParkingCell(pMap.GetEntrance());

            foreach (var cur in spaces)
            {
                if (cur.Value.ControlPoint != Entrance.CellSettings.ControlPoint)
                    FreeSpaces.Add(cur.Key, new ParkingCell(cur.Value));
            }
        }

        public void IsParkingCreated()
        {
            if (!IsCreated)
            {
                throw new Exception("Parking field has not been created yet.");
            }
        }

        public void ApplyNewVehicle(Vehicle vehicle)
        {
            IsParkingCreated();

            if (FreeSpaces.Count != 0)
            {
                Entrance.Vehicle = vehicle;
            }
            else
            {
                throw new Exception("There is no free parking space.");
            }
        }

        public Dictionary<Point, ParkingCell> GetSameVehicles(VehicleType type)
        {
            IsParkingCreated();

            Dictionary<Point, ParkingCell> res = new Dictionary<Point, ParkingCell>();

            foreach (var space in EmployedSpaces)
            {
                if (space.Value.Vehicle?.Type == type || type == VehicleType.NONE)
                {
                    res.Add(space.Key, space.Value);
                }
            }

            if (res.Count == 0)
            {
                throw new Exception("There is not any vehicle of this type at the parking.");
            }

            return res;
        }

        public void RegisterNewVehicle(Point controlPoint)
        {
            var vehicleCell = FreeSpaces[controlPoint];
            FreeSpaces.Remove(controlPoint);
 
            vehicleCell.Vehicle = Entrance.Vehicle;
            Entrance.Vehicle = null;

            EmployedSpaces.Add(controlPoint, vehicleCell);
        }

        public void RemoveVehicle(Point controlPoint)
        {
            var vehicleCell = EmployedSpaces[controlPoint];

            vehicleCell.Vehicle = null;

            EmployedSpaces.Remove(controlPoint);

            FreeSpaces.Add(controlPoint, vehicleCell);
        }

        public void Create()
        {
            IsCreated = true;
        }

        public void Reset()
        {
            IsCreated = false;

            foreach (var space in EmployedSpaces)
            {
                FreeSpaces.Add(space.Key, space.Value);
            }

            EmployedSpaces.Clear();
        }
    }
}
