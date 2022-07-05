using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ParkingSimulation.SerializableEntities;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace ParkingSimulation
{
    public enum ResponseStatus
    {
        FAIL,
        SUCCESS
    }

    public class ManagmentCore
    {
        private static Dictionary<string, string> userInstructionsList = new Dictionary<string, string>
        { {"help",              "Предоставляет список команд с описанием"         },
          {"create",            "Создает макет парковки, команда обязательна"     },
          {"destroy",           "Удаляет текущую парковку"                        },           
          {"add",               "Запрос на добавление новой машины на парковку"   },
          {"remove",            "Запрос на отъезд любой машины из парковки"       },
          {"add truck",         "Запрос на добавление грузовика на парковку"      },          
          {"remove truck",      "Запрос на отъезд грузовика из парковки"          },          
          {"add passenger",     "Запрос на добавление легковой машины на парковку"},           
          {"remove passenger",  "Запрос на отъезд легковой машины из парковки"    },
          {"clear",             "Очищает консоль"                                 } };

        private Displayer displayer;
        private Parking   parking;

        private string executingCommand;

        public ManagmentCore(string mapPath)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (FileStream fs = new FileStream(mapPath, FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(ParkingMap));

                ParkingMap pMap;

                pMap = (ParkingMap)formatter.Deserialize(fs);

                parking   = new Parking(pMap);
                displayer = new Displayer(this, pMap);
            }
        }

        public void Run()
        {
            Application.Run(displayer);
        }

        public string GetSpaceInfo(Point controlPoint)
        {
            try
            {
                return parking.EmployedSpaces[controlPoint].GetSpaceInfo();
            }
            catch(Exception)
            {
                return parking.FreeSpaces[controlPoint].GetSpaceInfo();
            }
        }

        private void AddPrepareVehicle(Vehicle vehicle)
        {
            try
            {
                parking.ApplyNewVehicle(vehicle);

                var employedSpaces = new Dictionary<Point, ParkingCell>(parking.EmployedSpaces);
                employedSpaces.Add(parking.Entrance.CellSettings.ControlPoint, parking.Entrance);

                displayer.PrepareUserChoose(employedSpaces,
                                            parking.Entrance,
                                            parking.FreeSpaces);

                if (MessageBox.Show(vehicle.GetGenericInfo(), 
                    "Vehicle information", 
                    MessageBoxButtons.OKCancel, 
                    MessageBoxIcon.Warning) == DialogResult.Cancel)
                {
                    parking.Entrance.Vehicle = null;
                    displayer.ReDrawParking(parking.EmployedSpaces);
                    throw new Exception("The request was rejected.");
                }

                displayer.PostResponseStatus("Success! Choose the space.\n", ResponseStatus.SUCCESS);
                displayer.BlockConsole();
            }
            catch (Exception ex)
            {
                displayer.PostResponseStatus("Fail! " + ex.Message + "\n", ResponseStatus.FAIL);
            }
        }

        private void RemovePrepareVehicle(VehicleType type)
        {
            try
            {
                displayer.PrepareUserChoose(parking.EmployedSpaces,
                                            parking.Entrance,
                                            parking.GetSameVehicles(type));

                displayer.PostResponseStatus("Success! Choose the vehicle.\n", ResponseStatus.SUCCESS);
                displayer.BlockConsole();
            }
            catch (Exception ex)
            {
                displayer.PostResponseStatus("Fail! " + ex.Message + "\n", ResponseStatus.FAIL);
            }

        }

        private void AddExecuteVehicle(Point controlPoint)
        {
            displayer.HandleMoveProcess(parking.EmployedSpaces,
                                        parking.Entrance,
                                        parking.FreeSpaces[controlPoint]);

            parking.RegisterNewVehicle(controlPoint);

            displayer.ReDrawParking(parking.EmployedSpaces);

            displayer.UnblockConsole();
        }

        private void RemoveExecuteVehicle(Point controlPoint)
        {
            ParkingCell vacatedSpace = new ParkingCell(parking.EmployedSpaces[controlPoint]);

            parking.RemoveVehicle(controlPoint);

            displayer.ReDrawParking(parking.EmployedSpaces);

            displayer.HandleMoveProcess(parking.EmployedSpaces,
                                        vacatedSpace,
                                        parking.Entrance);

            long payment = 0;

            MessageBox.Show(vacatedSpace.Vehicle.GetDepartureInfo(ref payment), 
                            "Departure information", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);

            displayer.ReDrawParking(parking.EmployedSpaces);

            displayer.UpdateExtraInfo(payment);

            displayer.UnblockConsole();
        }

        public void HandleUserSpaceChoice(Point controlPoint)
        {
            switch (executingCommand.Contains("add"))
            {
                case true:
                    AddExecuteVehicle(controlPoint);
                    break;

                case false:
                    RemoveExecuteVehicle(controlPoint);
                    break;
            }
        }

        public void HandleUserInstruction(string instruction)
        {
            executingCommand = instruction;

            switch (executingCommand)
            {
                case "clear":
                    displayer.ClearConsole();
                    break;

                case "help":
                    displayer.InstructUser(userInstructionsList);
                    break;

                case "create":
                    if (!parking.IsCreated)
                    {
                        parking  .Create();
                        displayer.Create();
                        displayer.PostResponseStatus("Success! Parking field is created.\n", ResponseStatus.SUCCESS);
                    }
                    else
                        displayer.PostResponseStatus("Fail! Parking field has been already created.\n", ResponseStatus.FAIL);
                    break;

                case "destroy":
                    if (parking.IsCreated)
                    {
                        parking  .Reset();
                        displayer.Reset();
                        displayer.PostResponseStatus("Success! Parking field is destroyed.\n", ResponseStatus.SUCCESS);
                    }
                    else
                        displayer.PostResponseStatus("Fail! Parking field has been already destroyed.\n", ResponseStatus.FAIL);
                    break;

                case "add":
                    AddPrepareVehicle(Vehicle.GenerateVehicle());
                    break;

                case "add truck":
                    AddPrepareVehicle(new Vehicle(VehicleType.TRUCK));
                    break;

                case "add passenger":
                    AddPrepareVehicle(new Vehicle(VehicleType.PASSENGER));
                    break;

                case "remove":
                    RemovePrepareVehicle(VehicleType.NONE);
                    break;

                case "remove truck":
                    RemovePrepareVehicle(VehicleType.TRUCK);
                    break;

                case "remove passenger":
                    RemovePrepareVehicle(VehicleType.PASSENGER);
                    break;

                default:
                    displayer.PostResponseStatus("Fail! Incorrect command.\nEnter < help > for instructions.\n", ResponseStatus.FAIL);
                    break;
            }
        }
    }
}
