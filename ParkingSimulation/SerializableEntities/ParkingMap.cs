using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;
using System.Reflection;

namespace ParkingSimulation.SerializableEntities
{
    [Serializable]
    public struct XmlColor
    {
        public int alpha;
        public int red;
        public int green;
        public int blue;

        public XmlColor(Color col)
        {
            alpha = col.A;
            red   = col.R;
            green = col.G;
            blue  = col.B;
        }

        public Color ConvertToColor()
        {
            return Color.FromArgb(alpha, red, green, blue);
        }
    }

    [Serializable]
    public struct LineParams
    {
        public Point Begin;
        public Point End;
        public XmlColor Color;
        public int Width;

        public LineParams(Point begin, Point end, XmlColor color, int width)
        {
            Begin = begin;
            End   = end;
            Color = color;
            Width = width;
        }
    }
    [Serializable]
    public struct CellParams
    {
        public Size Size;
        public Point ControlPoint;
        public RotateFlipType ImageFlipType;

        public CellParams(Size size, Point controlPoint, RotateFlipType inImageFlipType)
        {
            Size = size;
            ControlPoint = controlPoint;
            ImageFlipType = inImageFlipType;
        }
    }
    [Serializable]
    public struct ParkingParams
    {
        public Size Size;
        public XmlColor Color;
        public int RoadWidth;

        public ParkingParams(Size size, XmlColor color, int roadWidth)
        {
            Size = size;
            Color = color;
            RoadWidth = roadWidth;
        }
    }
    [Serializable]
    public struct Path
    {
        public Point Begin;
        public Point End;
        public RotateFlipType ImageFlipType;
        public Size Vector;

        public Path(Point inBegin, Point inEnd, RotateFlipType inImageFlipType, Size inVector)
        {
            Begin         = inBegin;
            End           = inEnd;
            ImageFlipType = inImageFlipType;
            Vector        = inVector;
        }
    }

    [Serializable]
    public class ParkingMap
    {
        public XmlSerializableDictionary<Point, CellParams> ParkingSpaces;
        public CellParams Entrance;

        public List<LineParams> Markings;

        public ParkingParams GenericSettings;

        public XmlSerializableDictionary<XmlSerializableTuple<CellParams, CellParams>, List<Path>> Pathes;

        public ParkingMap()
        {
            ParkingSpaces = new XmlSerializableDictionary<Point, CellParams>();
            Markings      = new List<LineParams>();
            Pathes        = new XmlSerializableDictionary<XmlSerializableTuple<CellParams, CellParams>, List<Path>>();
        }

        public XmlSerializableDictionary<XmlSerializableTuple<CellParams, CellParams>, List<Path>> GetPaths()
        {
            return Pathes;
        }

        public Color GetParkingBackColor()
        {
            return GenericSettings.Color.ConvertToColor();
        }

        public Size GetParkingSize()
        {
            return GenericSettings.Size;
        }

        public int GetRoadWidth()
        {
            return GenericSettings.RoadWidth;
        }

        public LineParams GetLine(int id)
        {
            return Markings[id];
        }

        public int GetLinesCount()
        {
            return Markings.Count;
        }

        public int GetCellsCount()
        {
            return ParkingSpaces.Count;
        }

        public Dictionary<Point, CellParams> GetParkingSpaces()
        {
            return ParkingSpaces;
        }

        public List<LineParams> GetMarkings()
        {
            return Markings;
        }

        public CellParams GetEntrance()
        {
            return Entrance;
        }
    }
}
