using System;
using System.Threading;
using SecretNest.Hardware;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            SignalStabilizer<int> stabilizer = new SignalStabilizer<int>(100);
            stabilizer.ValueChanged += Stabilizer_ValueChanged;

            stabilizer.SetValue(100);
            Thread.Sleep(100);

            stabilizer.SetValue(101);
            stabilizer.SetValue(102);
            stabilizer.SetValue(100);
            Thread.Sleep(100);

            stabilizer.SetValue(200);
            stabilizer.SetValue(201);
            Thread.Sleep(100);

            stabilizer.SetValue(100);
            Thread.Sleep(100);

            stabilizer.Dispose();
        }

        private static void Stabilizer_ValueChanged(object sender, ValueChangedEventArgs<int> e)
        {
            Console.WriteLine(e.Value);
        }
    }
}
