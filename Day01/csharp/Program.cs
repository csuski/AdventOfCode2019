using System.IO;
using System;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Part 1
            using(var reader = File.OpenText("input.dat")) {
                var line = reader.ReadLine();
                var total = 0;
                while(line != null) {
                    var val = Int32.Parse(line);
                    val /=3;
                    val -=2;
                    total += val;
                    line = reader.ReadLine();
                }
                Console.WriteLine($"Total = {total}");
            }

            // Part 2
            using(var reader = File.OpenText("input.dat")) {
                var line = reader.ReadLine();
                var total = 0;
                while(line != null) {
                    var val = Int32.Parse(line);
                    var fuel = GetFuel(val);
                    total += fuel;
                    line = reader.ReadLine();
                }
                Console.WriteLine($"Total = {total}");
            }
        }

        public static int GetFuel(int mass) {
            var fuel = mass / 3 - 2;
            if(fuel <= 0) return 0;
            return fuel + GetFuel(fuel);
        }
    }
}
