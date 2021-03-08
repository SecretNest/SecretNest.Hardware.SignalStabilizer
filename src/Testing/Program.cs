using System;
using System.Threading.Tasks;
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
            Task.Delay(200).Wait();
            //output 100

            stabilizer.SetValue(101);
            Task.Delay(50).Wait();
            stabilizer.SetValue(102);
            Task.Delay(10).Wait();
            stabilizer.SetValue(100);
            Task.Delay(200).Wait();
            //output nothing. last value 100 was announced previously.

            stabilizer.SetValue(200);
            stabilizer.SetValue(201);
            Task.Delay(200).Wait();
            //output 201

            stabilizer.SetValue(100);
            Task.Delay(200).Wait();
            //output 100

            stabilizer.Dispose();
        }

        private static void Stabilizer_ValueChanged(object sender, ValueChangedEventArgs<int> e)
        {
            Console.WriteLine(e.Value);
        }
    }
}
