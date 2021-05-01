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
            Random rand = new Random();
            HashSet<int> mySet = new HashSet<int>();
            for(var i=0; i<10;i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    mySet.Add(j);
                }

                foreach (var x in mySet.OrderBy(_ => rand.Next()))
                {
                    Console.Write("{0}, ", x);
                }
                Console.WriteLine();
                mySet.Clear();
            }
        }
    }
}
