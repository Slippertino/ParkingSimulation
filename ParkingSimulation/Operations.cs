using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ParkingSimulation
{
    public static class Operations
    {
        public enum BaseVectors
        {
            TOP,
            DOWN,
            LEFT,
            RIGHT
        }

        public static Dictionary<Size, BaseVectors> Base = new Dictionary<Size, BaseVectors> 
        { { new Size(1, 0),  BaseVectors.RIGHT },
          { new Size(-1, 0), BaseVectors.LEFT  },
          { new Size(0, 1),  BaseVectors.DOWN  },
          { new Size(0, -1), BaseVectors.TOP   } };

        public static bool IsContained (Size s1, Size s2)
        {
            return (s1.Width > s2.Width || s1.Height > s2.Height);
        }

        public static int ConvertVehicleSpeed(int speed)
        {
            return (int)Math.Sqrt(speed);
        }
    }
}
