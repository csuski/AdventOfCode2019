using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day06
{
    class Program
    {

        static readonly string CenterOfMass = "COM";
        static readonly string You = "YOU";
        static readonly string Santa = "SAN";

        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Equals("Part1",
              StringComparison.CurrentCultureIgnoreCase))
            {
                Part1("input.txt");

            }
            else if (args.Length > 0 && args[0].Equals("Part2",
              StringComparison.CurrentCultureIgnoreCase))
            {
                Part2("input.txt");
            }
            else
            {
                Test("test.txt");
            }
        }

        static void Part1(string fileName)
        {
            // Key = orbiter, value = planet
            var orbits = new Dictionary<string, string>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                var planets = line.Split(')');
                orbits[planets[1]] = planets[0];
            }
            int val = CountAllPaths(orbits);
            Console.WriteLine("Total direct and indirect orbits = " + val);
        }

        static void Part2(string fileName)
        {
            // Key = orbiter, value = planet
            var orbits = new Dictionary<string, string>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                var planets = line.Split(')');
                orbits[planets[1]] = planets[0];
            }
            var myPath = new List<string>();
            PathToCenterOfMass(orbits, myPath, You);
            PrintPath(myPath);
            var santaPath = new List<string>();
            PathToCenterOfMass(orbits, santaPath, Santa);
            PrintPath(santaPath);
            var firstDiff = 0;
            for (int i = 0; i < myPath.Count && i < santaPath.Count; i++)
            {
                if (!myPath[i].Equals(santaPath[i], StringComparison.CurrentCultureIgnoreCase))
                {
                    firstDiff = i;
                    break;
                }
            }
            var totalHops = myPath.Count - firstDiff + santaPath.Count - firstDiff - 2; // remove SAN and YOU
            Console.WriteLine($"Total hops = {totalHops}");
        }

        static void PrintPath(List<string> path)
        {
            bool first = true;
            foreach (var v in path)
            {
                if (first)
                {
                    Console.Write(v);
                    first = false;
                }
                else
                {
                    Console.Write($"->{v}");
                }
            }
            Console.WriteLine();
        }

        static void PathToCenterOfMass(Dictionary<string, string> orbits, List<string> Path, string planet)
        {
            var orbiting = orbits[planet];
            if (orbiting.Equals(CenterOfMass, StringComparison.CurrentCultureIgnoreCase))
            {
                Path.Add(planet);
                Path.Add(CenterOfMass);
                return;
            }
            else
            {
                PathToCenterOfMass(orbits, Path, orbiting);
                Path.Add(planet);
            }
        }


        static void Test(string fileName)
        {
            // Key = orbiter, value = planet
            var orbits = new Dictionary<string, string>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                var planets = line.Split(')');
                orbits[planets[1]] = planets[0];
            }
            int val = CountAllPaths(orbits);
            Console.WriteLine("Total direct and indirect orbits = " + val);
        }

        static int CountAllPaths(Dictionary<string, string> orbits)
        {

            // Planet, orbit length
            var orbitLengths = new Dictionary<string, int>();
            GetLengthsForOrbitingPlanets(orbits, orbitLengths, CenterOfMass, 1);
            return orbitLengths.Values.Sum();
        }

        static public void GetLengthsForOrbitingPlanets(Dictionary<string, string> orbits,
            Dictionary<string, int> orbitLengths, string planet, int currentDistance)
        {
            var orbitingPlanets = GetOribitingPlanets(orbits, planet);
            foreach (var p in orbitingPlanets)
            {
                orbitLengths[p] = currentDistance;
                GetLengthsForOrbitingPlanets(orbits, orbitLengths, p, currentDistance + 1);
            }
        }

        static public IEnumerable<string> GetOribitingPlanets(Dictionary<string, string> orbits, string planet)
        {
            var orbiters = new List<string>();
            foreach (var pair in orbits)
            {
                if (pair.Value.Equals(planet, StringComparison.CurrentCultureIgnoreCase))
                {
                    orbiters.Add(pair.Key);
                }
            }
            return orbiters;
        }




    }
}
