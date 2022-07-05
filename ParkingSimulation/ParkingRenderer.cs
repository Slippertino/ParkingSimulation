using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ParkingSimulation.SerializableEntities;
using ParkingSimulation.Displayers;
using System.Windows.Forms;
using System.Threading;

namespace ParkingSimulation
{
    public class ParkingRenderer
    {
        public delegate void SaveChanges(Bitmap bmp);
        private SaveChanges  Update;

        private readonly Bitmap parkingBitmap;

        private readonly Dictionary<Tuple<CellParams, CellParams>, List<Path>> paths;

        private readonly int roadWidth;

        public ParkingRenderer(ParkingDisplayer displayer, ParkingMap map)
        {
            Update       = displayer.Update;

            parkingBitmap = new Bitmap(displayer.DefaultBitmap);
            roadWidth     = map.GetRoadWidth();

            paths = new Dictionary<Tuple<CellParams, CellParams>, List<Path>>();

            foreach (var curPath in map.GetPaths())
            {
                paths.Add(curPath.Key, curPath.Value);
            }
        }

        public void DrawVehicle(Point controlPoint, ParkingCell cell, ref Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Image img = Image.FromFile(cell.Vehicle.GetPath());
                img.RotateFlip(cell.CellSettings.ImageFlipType);

                Point offset = new Point((cell.CellSettings.Size.Width - img.Width) / 2,
                                         (cell.CellSettings.Size.Height - img.Height) / 2);

                Point drawPoint = new Point(controlPoint.X + offset.X,
                                            controlPoint.Y + offset.Y);

                g.DrawImage(img, drawPoint);
            }
        }

        private void DrawParkedVehicles(Dictionary<Point, ParkingCell> employedSpaces, ref Bitmap bmp)
        {
            foreach (var cell in employedSpaces)
            {
                DrawVehicle(cell.Value.CellSettings.ControlPoint, cell.Value, ref bmp);
            }
        }

        public void RenderVehicleMove(Dictionary<Point, ParkingCell> employedSpaces, ParkingCell start, ParkingCell finish)
        {
            Bitmap bmp = new Bitmap(parkingBitmap);

            DrawParkedVehicles(employedSpaces, ref bmp);

            List<Path> vehiclePaths = paths[Tuple.Create(start.CellSettings, finish.CellSettings)];

            for (int i = 0; i < vehiclePaths.Count; i++)
            {
                var vehiclePath = vehiclePaths[i];

                Vehicle vehicle = start.Vehicle;

                Image img = Image.FromFile(vehicle.GetPath());
                img.RotateFlip(vehiclePath.ImageFlipType);

                Point begin = vehiclePath.Begin;
                Point end   = vehiclePath.End;

                Point offset = new Point(0, 0);

                Predicate<Point> movePredicate = null;

                switch (Operations.Base[vehiclePath.Vector])
                {
                    case Operations.BaseVectors.RIGHT:
                        offset.Y += (roadWidth - img.Height) / 2;

                        if (i == 0)
                            begin.X += (start.CellSettings.Size.Width - img.Width) / 2;
                        else if (i + 1 == vehiclePaths.Count)
                            end.X -= (finish.CellSettings.Size.Width - img.Width) / 2;

                        movePredicate = delegate (Point p) { return p.X + img.Width <= end.X; };
                        break;

                    case Operations.BaseVectors.LEFT:
                        begin.X -= img.Width;
                        offset.Y += (roadWidth - img.Height) / 2;

                        if (i == 0)
                            begin.X -= (start.CellSettings.Size.Width - img.Width) / 2;
                        else if (i + 1 == vehiclePaths.Count)
                            end.X += (finish.CellSettings.Size.Width - img.Width) / 2;

                        movePredicate = delegate (Point p) { return p.X >= end.X; };
                        break;

                    case Operations.BaseVectors.TOP:
                        begin.Y -= img.Height;
                        offset.X += (roadWidth - img.Width) / 2;

                        if (i == 0)
                            begin.Y -= (start.CellSettings.Size.Height - img.Height) / 2;
                        else if (i + 1 == vehiclePaths.Count)
                            end.Y += (finish.CellSettings.Size.Height - img.Height) / 2;

                        movePredicate = delegate (Point p) { return p.Y >= end.Y; };
                        break;

                    case Operations.BaseVectors.DOWN:
                        offset.X += (roadWidth - img.Width) / 2;

                        if (i == 0)
                            begin.Y += (start.CellSettings.Size.Height - img.Height) / 2;
                        else if (i + 1 == vehiclePaths.Count)
                            end.Y -= (finish.CellSettings.Size.Height - img.Height) / 2;

                        movePredicate = delegate (Point p) { return p.Y + img.Height <= end.Y; };
                        break;

                    default:
                        throw new Exception("Некорректный входной вектор направления -> RenderVehicleMove");
                }

                RenderDirectionMove(bmp, vehicle, img, vehiclePath, begin, offset, movePredicate);
            }
        }

        private void RenderDirectionMove(Bitmap bmp, Vehicle vehicle, Image img, Path path, Point start, Point offset, Predicate<Point> predicate)
        {
            Point cur = start;

            int count = 0;
            int speed = Operations.ConvertVehicleSpeed(vehicle.Speed);

            while (predicate(cur))
            {
                if (count % speed == 0)
                {
                    using (Bitmap curBmp = new Bitmap(bmp))
                    {
                        Graphics g = Graphics.FromImage(curBmp);
                        g.DrawImage(img, new Point(cur.X + offset.X, cur.Y + offset.Y));

                        Update(curBmp);

                        g.Dispose();
                    }
                }

                cur += path.Vector;

                count++;
            }
        }
    }
}
