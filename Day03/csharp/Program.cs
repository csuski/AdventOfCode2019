using System;
using System.Collections.Generic;
using System.IO;

namespace Day03
{
    class Program
    {
        private static List<(string line1, string line2)> TestStrings = new List<(string line1, string line2)>
        {
            (line1: "R8,U5,L5,D3", line2: "U7,R6,D4,L4"),
            (line1: "R75,D30,R83,U83,L12,D49,R71,U7,L72", line2: "U62,R66,U55,R34,D71,R55,D58,R83"),
            (line1: "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51", line2: "U98,R91,D20,R16,D67,R40,U7,R15,U6,R7")
        };

        static void Main(string[] args)
        {
            //RunTest();
            //RunTest2();
            //Part1();
            Part2();
        }

        public static void Part1()
        {
            var lines = File.ReadAllLines("Input.txt");
            if (lines.Length != 2) throw new Exception($"Expected 2 lines, received {lines.Length}");
            var result = CompareLines(lines[0], lines[1]);
            Console.WriteLine($"Part 1: Point {result.point} is the closest location at {result.Distance}");
        }

        public static void Part2()
        {
            var lines = File.ReadAllLines("Input.txt");
            if (lines.Length != 2) throw new Exception($"Expected 2 lines, received {lines.Length}");
            var result = CompareLinesWithSteps(lines[0], lines[1]);
            Console.WriteLine($"Part 1: Point {result.point} is the closest location at {result.Distance}");
        }

        public static void RunTest()
        {
            for (int i = 0; i < TestStrings.Count; i++)
            {
                var result = CompareLines(TestStrings[i].line1, TestStrings[i].line2);
                Console.WriteLine($"Test {i + 1}: Point {result.point} is the closest location at {result.Distance}");
            }
        }

        public static void RunTest2()
        {
            for (int i = 0; i < TestStrings.Count; i++)
            {
                var result = CompareLinesWithSteps(TestStrings[i].line1, TestStrings[i].line2);
                Console.WriteLine($"Test {i + 1}: Point {result.point} is the closest location at {result.Distance}");
            }
        }

        public static (Point point, int Distance) CompareLines(string line1, string line2)
        {
            var l1 = CreateLine(line1);
            var l2 = CreateLine(line2);

            (Point point, int Distance) closestPoint = (new Point(0, 0), int.MaxValue);
            var origin = new Point(0, 0);
            foreach (var p in l1.Points)
            {
                if (p.X == 0 && p.Y == 0)
                    continue;
                if (l2.ContainsPoint(p) && origin.Distance(p) < closestPoint.Distance)
                {
                    closestPoint = (p, origin.Distance(p));
                }
            }
            return closestPoint;
        }

        public static (Point point, int Distance) CompareLinesWithSteps(string line1, string line2)
        {
            var l1 = CreateLine(line1);
            var l2 = CreateLine(line2);

            (Point point, int Distance) closestPoint = (new Point(0, 0), int.MaxValue);
            var origin = new Point(0, 0);

            for (int i = 1; i < l1.Points.Count; i++)
            {
                var loc = l2.Points.IndexOf(l1.Points[i]);
                if (loc > 0)
                {
                    if (i + loc < closestPoint.Distance)
                    {
                        closestPoint = (l1.Points[i], loc + i);
                        // It is almost certainly one of the first couple of intersections
                        // so output them so we can try them out and not search the entire
                        // list, which is time consuming.
                        Console.WriteLine($"Found potential Point {l1.Points[i]} with distance {loc + i}");
                    }
                }
            }
            return closestPoint;
        }

        public static Line CreateLine(string line)
        {
            var parts = line.Split(',');
            return new Line(parts);
        }
    }

    public class Line
    {
        private List<Point> _allPoints = new List<Point>();
        private Point _currentPoint = new Point(0, 0);

        public IList<Point> Points => _allPoints;

        public Line(string[] directions)
        {
            _allPoints.Add(_currentPoint);
            foreach (var direction in directions)
            {
                var parsed = ParseString(direction);
                MoveAndAdd(parsed.Direction, parsed.Amount);
            }
        }

        public bool ContainsPoint(Point p)
        {
            return _allPoints.Exists(point => point.X == p.X && point.Y == p.Y);
        }

        public void MoveAndAdd(Direction direction, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var nextPoint = _currentPoint.CreateNextPoint(direction);
                _allPoints.Add(nextPoint);
                _currentPoint = nextPoint;
            }
        }

        public (Direction Direction, int Amount) ParseString(string str)
        {
            Direction direction;
            char dir = str[0];
            switch (dir)
            {
                case 'D':
                    direction = Direction.Down;
                    break;
                case 'U':
                    direction = Direction.Up;
                    break;
                case 'L':
                    direction = Direction.Left;
                    break;
                case 'R':
                    direction = Direction.Right;
                    break;
                default:
                    throw new Exception($"Unknown direction '{dir}'");
            }
            int val = Int32.Parse(str.Substring(1));
            return (Direction: direction, Amount: val);
        }
    }

    public enum Direction { Right, Left, Up, Down }

    public class Point : IComparable
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(object obj)
        {
            if (obj is Point p)
            {
                if (p.X == X && p.Y == Y) return 0;
                if (X < p.X) return -1;
                if (X == p.X && Y < p.Y) return -1;
                return 1;
            }
            throw new ArgumentException("Argument is not a point.");
        }

        public int Distance(Point p)
        {
            return Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
        }

        public Point CreateNextPoint(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Point(X, Y + 1);
                case Direction.Down:
                    return new Point(X, Y - 1);
                case Direction.Right:
                    return new Point(X + 1, Y);
                case Direction.Left:
                    return new Point(X - 1, Y);
            }
            return null;
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Point p)
            {
                return p.X == X && p.Y == Y;
            }
            return false;
        }
    }
}