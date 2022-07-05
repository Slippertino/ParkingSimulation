using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ParkingSimulation
{
    public enum VehicleType
    {
        NONE,
        PASSENGER,
        TRUCK
    }

    public class Vehicle
    {
        struct TypeParams
        {
            public string ImagesDirectoryPath;
            public int    PriceForSecond;
            public int    MinSpeed;
            public int    MaxSpeed;
        }

        private static Random random = new Random((int)DateTime.Now.ToBinary());

        private static Dictionary<VehicleType, TypeParams> typesParams = new Dictionary<VehicleType, TypeParams> 
        { { VehicleType.PASSENGER, new TypeParams { ImagesDirectoryPath = @"../../../VehicleImages/Passengers/", 
                                                    PriceForSecond      = 5,  
                                                    MinSpeed            = 30, 
                                                    MaxSpeed            = 90 }                               },
          { VehicleType.TRUCK,     new TypeParams { ImagesDirectoryPath = @"../../../VehicleImages/Trucks/",     
                                                    PriceForSecond      = 10, 
                                                    MinSpeed            = 15, 
                                                    MaxSpeed            = 50 }                               } };


        private int  id;
        private int  imageId; 

        private DateTime entranceTime = DateTime.Now;
        private DateTime departureTime;

        public int Speed { get; }

        public VehicleType Type { get; }

        public Vehicle(VehicleType _type)
        {
            Type = _type;

            string path = typesParams[Type].ImagesDirectoryPath;

            id      = random.Next(100000, 999999);
            imageId = random.Next(0, Directory.GetFiles(path).Length - 1);
            Speed   = random.Next(typesParams[Type].MinSpeed, typesParams[Type].MaxSpeed);
        }

        public Vehicle(Vehicle vehicle)
        {
            Type         = vehicle.Type;
            id           = vehicle.id;
            imageId      = vehicle.imageId;
            Speed        = vehicle.Speed;
            entranceTime = vehicle.entranceTime;
        }

        public static Vehicle GenerateVehicle()
        {
            VehicleType type = (VehicleType)random.Next((int)VehicleType.PASSENGER, (int)VehicleType.TRUCK + 1);

            return new Vehicle(type);
        }

        public string GetPath()
        {
            return typesParams[Type].ImagesDirectoryPath + Convert.ToString(imageId) + ".png";
        }

        private long GetPayment()
        {
            departureTime = DateTime.Now;

            double durationSec = (departureTime.Subtract(entranceTime)).TotalSeconds;

            return (long)(durationSec * typesParams[Type].PriceForSecond);
        }

        public string GetGenericInfo()
        {
            string res = "";

            res += "Id : #" + id.ToString() + "\n";
            res += "Type : ";

            switch (Type)
            {
                case VehicleType.PASSENGER:
                    res += "Passenger\n";
                    break;
                case VehicleType.TRUCK:
                    res += "Truck\n";
                    break;
            }

            res += "Entrance time : " + entranceTime.ToString() + "\n";
            res += "Speed : " + Speed.ToString() + " km/h";

            return res;
        }

        public string GetDepartureInfo(ref long payment)
        {
            payment = GetPayment();

            string res = "";

            res += "Entrance time  : " + entranceTime.ToString() + "\n";
            res += "Departure time : " + departureTime.ToString() + "\n";
            res += "Seconds at parking : " + ((long)(departureTime.Subtract(entranceTime)).TotalSeconds).ToString() + "\n";
            res += "Price for second : " + typesParams[Type].PriceForSecond.ToString() + "\n";
            res += "Payment : " + payment.ToString();

            return res;
        }
    }
}
