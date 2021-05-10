using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] l1 = { "1", "2", "3", "4" };
            string[] l2 = { "a", "b", "c",};

            var it = l1.Zip(l2, (first, second) => new { first, second });
            foreach(var pair in it)
            {
                Console.WriteLine("{0} ---> {1}", pair.first, pair.second);
            }
        }
    }
}
