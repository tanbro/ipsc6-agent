using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class MyObject : IEquatable<MyObject>
    {
        public readonly string Id;
        public MyObject(string id)
        {
            Id = id;
        }

        public bool Equals(MyObject other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            var that = obj as MyObject;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(MyObject lhs, MyObject rhs)
        {
            //return ReferenceEquals(lhs, rhs) || (lhs is object && lhs.Equals(rhs)) || (rhs is object && rhs.Equals(lhs));
            return (lhs is object) && (rhs is object) && lhs.Equals(rhs);
        }

        public static bool operator !=(MyObject lhs, MyObject rhs)
        {
            return !(lhs == rhs);
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            var myObj = new MyObject("123");
            
            Console.WriteLine(string.Format("{0}", null == myObj));
            Console.WriteLine(string.Format("{0}", null != myObj));
            Console.WriteLine(string.Format("{0}", myObj == null));
            Console.WriteLine(string.Format("{0}", myObj != null));

            Console.WriteLine(string.Format("{0}", myObj == new MyObject("123")));
            Console.WriteLine(string.Format("{0}", myObj != new MyObject("123")));

            Console.WriteLine(string.Format("{0}", myObj == new MyObject("456")));
            Console.WriteLine(string.Format("{0}", myObj != new MyObject("456")));
        }
    }
}
