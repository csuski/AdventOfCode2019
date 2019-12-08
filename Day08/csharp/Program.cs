using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var array = Part1(File.ReadAllText("input.txt"));
            Part2(array);
        }

        const int Width = 25;
        const int Height = 6;
        const int Pixels = Width * Height;

        static List<int[]> Part1(string text)
        {
            Console.WriteLine("Part1...");
            List<int[]> layers = new List<int[]>();
            var allVals = text.Select(c => Int32.Parse(c.ToString())).ToArray();
            int layerCount = allVals.Length / Pixels;
            for (int i = 0; i < layerCount; i++)
            {
                var layer = new int[Pixels];
                Array.Copy(allVals, i * Pixels, layer, 0, Pixels);
                layers.Add(layer);
            }

            int[] fewestZeroLayer = layers[0];
            int fewestZeros = fewestZeroLayer.Count(x => x == 0);
            for(int i = 1; i < layers.Count; i++)
            {
                var zeros = layers[i].Count(x => x == 0);
                if (zeros < fewestZeros)
                {
                    fewestZeros = zeros;
                    fewestZeroLayer = layers[i];
                }
            }

            var ones = fewestZeroLayer.Count(x => x == 1);
            var twos = fewestZeroLayer.Count(x => x == 2);
            Console.WriteLine(ones * twos);
            return layers;
        }

        static void Part2(List<int[]> layers)
        {
            Console.WriteLine("Part2...");
            int[] masterLayer = new int[Pixels];
            Array.Copy(layers[0], masterLayer, Pixels);
            for(int l = 1; l < layers.Count; l++)
            {
                for (int i = 0; i < Pixels; i++)
                {
                    if (masterLayer[i] == 2)
                    {
                        masterLayer[i] = layers[l][i];
                    }
                }
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var va = masterLayer[i * Width + j];
                    if (va == 1) { Console.Write("*"); }
                    else { Console.Write(" "); }
                }
                Console.WriteLine();
            }
        }
    }
}