using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSimulation.SerializableEntities
{
    [Serializable]
    public class XmlSerializableTuple<T1, T2>
    {
        XmlSerializableTuple() { }

        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public static implicit operator XmlSerializableTuple<T1, T2>(Tuple<T1, T2> t)
        {
            return new XmlSerializableTuple<T1, T2>()
            {
                Item1 = t.Item1,
                Item2 = t.Item2
            };
        }

        public static implicit operator Tuple<T1, T2>(XmlSerializableTuple<T1, T2> t)
        {
            return Tuple.Create(t.Item1, t.Item2);
        }
    }
}
