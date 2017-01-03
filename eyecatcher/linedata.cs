using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace eyecatcher
{
    //the linedata object is the container for everything associated with a single line
    // this object holds the conversion functions to and from binary strings
    public class linedata
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public double Distance { get; set; }
        public double Angle { get; set; }

        //convert line properties (as doubles) into a list of doubles
        //  order is assumed, similar to the fillvalues constructor
        public List<double> datalist
        {
            get { return new List<double>() {
                        Start.X,
                        Start.Y,
                        End.X,
                        End.Y,
                        Distance,
                        Angle
                    };
             }
        }
        
        //convert the line into a string of 1s and 0s
        // i made this in anticipation of this being part of a genetic algorithm
        //  i've since decided to use something else for the genes, but i'm leaving this in here
        //  because it was really hard to make and i might change my mind again
        public string ToBinaryString()
        {
            var linedatalist = datalist;
            var bytemultiarray = new byte[6][];
            for(var i = 0; i < 6; i++)
            {
                bytemultiarray[i] = BitConverter.GetBytes(linedatalist[i]);
            }
            var combinedbytes = Combine(bytemultiarray);
            return string.Join("",combinedbytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
        }

        public linedata(linedata copyline)
        {
            Start = copyline.Start;
            End = copyline.End;
            Distance = copyline.Distance;
            Angle = copyline.Angle;
        }

        public linedata()
        {

        }

        //given a list of doubles, assume the order and fill the properties
        public linedata(List<double> fillvalues)
        {
            Start = new Point(fillvalues[0], fillvalues[1]);
            End = new Point(fillvalues[2], fillvalues[3]);
            Distance = fillvalues[4];
            Angle = fillvalues[5];

        }

        //allow a bytestring to construct a line by sending it to the conversion function
        //  before handing it off to the constructor that takes the set of properties
        public linedata(string bytestring) : this(binaryStringToFillValues(bytestring))
        {

            
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() != GetType())
            {
                return false;
            }
            var castedobj = (linedata)obj;
            return Start == castedobj.Start &&
                   End == castedobj.End &&
                   Distance == castedobj.Distance &&
                   Angle == castedobj.Angle;
        }

        public void Flip()
        {
            var savepoint = Start;
            Start = End;
            End = savepoint;
            if (double.IsInfinity(Angle))
            {
                Angle = -1 * Angle;
            }
            else
            {
                Angle = (Angle + 180) % 360;
            }
        }

        //for debugging
        public override string ToString()
        {
            return string.Format("({0:0.##},{1:0.##}) -> ({2:0.##},{3:0.##}) | {4} | @ {5}°", Start.X, Start.Y, End.X, End.Y, Distance, Angle);
        }

        //convert the binary string into the associated Double values to fill into the properties
        static private List<double> binaryStringToFillValues(string bytestring)
        {
            int k = 0;
            //split the string up into 8-character chunks
            var bytesAsStringArray = bytestring
                .ToLookup(c => Math.Floor(k++ / 8d))
                .Select(e => new string(e.ToArray()));

            var stringtobytelist = new List<byte>();
            foreach (string bytesstring in bytesAsStringArray)
            {
                // for each byte-sized string, convert it to the byte object it represents
                // and add it to the list of byte objects
                stringtobytelist.Add(Convert.ToByte(bytesstring, 2));
            }
            var linedatalist = new List<double>(6);
            for (var i = 0; i < 6; i++)
            {
                linedatalist.Add(BitConverter.ToDouble(stringtobytelist.ToArray(), i * 8));
            }
            return linedatalist;
        }

        public string ToValuesCSV()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", Start.X, Start.Y, End.X, End.Y, Distance, Angle);
        }

        // from this stack overflow question:
        //  http://stackoverflow.com/questions/415291/best-way-to-combine-two-or-more-byte-arrays-in-c-sharp
        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public void truncate()
        {
            Start = new Point(Math.Truncate(Start.X), Math.Truncate(Start.Y));
            End = new Point(Math.Truncate(End.X), Math.Truncate(End.Y));
        }
    }

    //slice extension from https://www.dotnetperls.com/array-slice
    public static class Extensions
    {
        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

        /// <summary>
        /// From http://stackoverflow.com/questions/273313/randomize-a-listt
        /// </summary>
        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
