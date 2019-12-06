using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace csharp
{
    class Program
    {

        // This doesn't work and has off by one error for some reason.
        const uint StartRange = 402328;
        const uint EndRange = 864247;

        static void Main(string[] args)
        {
            BruteForcePart1();
        }

        static void BruteForcePart1() {
            // TODO: Off by one error?
            Console.WriteLine("Brute forcing...");
            var firstDigit = new DigitEnumerator(StartRange).Last();
            // get the first valid number, which is the first number repeating
            uint firstValidNumber = uint.Parse(new string(new DigitEnumerator(StartRange).Last().ToString()[0], 6));

            var validNumbers = 0;
            var num = firstValidNumber;
            while(num <= EndRange) {
                if(IsValidPart2(num)) {     // Todo add switch to part 2
                    Console.WriteLine(num);
                    validNumbers++;
                }
                num++;
            }

            Console.WriteLine($"Total Valid numbers = {validNumbers}");
        }

        static uint GetDigit(uint digit, uint number)
        {
            if (digit >= 6) throw new ArgumentOutOfRangeException(nameof(digit),
                $"Must be less than 6 digits, got {digit}");

            if (number > 999999) throw new ArgumentOutOfRangeException(nameof(number),
                 $"Number must be less than 6 digits, got {number}");


            if (digit == 0) return number % 10;
            //return number / ((uint)(Math.Pow(10, digit))) % 10;
            return GetDigit(digit - 1, number / 10);  // recursion!
        }



        static bool IsValid(uint number)
        {
            if (number > 999999) return false;

            var enumer = new DigitEnumerator(number);
            var prevNumber =0;
            foreach(var digit in enumer.Reverse()) {
                if(digit < prevNumber){
                    return false;
                }
                prevNumber = digit;
            }
            return true;
        }

        static bool IsValidPart2(uint number)
        {
            if (number > 999999) return false;
            
            uint repeatCount = 0;
            bool hasTwoReating = false;
            bool hasRepeating = false;
            var enumer = new DigitEnumerator(number);
            var prevNumber =0;
            foreach(var digit in enumer.Reverse()) {
                if(digit < prevNumber){
                    return false;
                }

                if(digit == prevNumber) {
                    hasRepeating = true;
                    repeatCount++;
                }
                else {
                    if(repeatCount == 1) {
                        hasTwoReating = true;
                    }
                    repeatCount = 0;
                }
                prevNumber = digit;
            }
            return !hasRepeating || (hasTwoReating || repeatCount == 1);
        }


        public class DigitEnumerator : IEnumerable<byte>
        {
            private uint _number;
            public DigitEnumerator(uint number)
            {
                _number = number;
            }

            public IEnumerator<byte> GetEnumerator()
            {
                var tempNum = _number;
                var digit = tempNum % 10;
                while (tempNum > 0)
                {
                    yield return (byte)digit;
                    tempNum /= 10;
                    digit = tempNum % 10;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
