using System;
using System.Threading.Tasks;
using Fluke;

namespace FlukeTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var fluke = new Fluke2638A("10.3.3.32", 3490))
            {
                
                var result = await fluke.IdentifyAsync();
                var status = await fluke.GetStatusAsync();
                //await fluke.TurnDisplayOffAsync();
                //await fluke.TurnDisplayOnAsync();
                await fluke.SetCurrentDateTimeAsync();
                await fluke.SetScanResolutionAsync(); 
                
                var channels = new[] { 101,102, 103 };
                await fluke.ConfigureScanAsync(channels);

                await fluke.StartScanAsync();
                var point = await fluke.ReadDataFromLastIndexAsync();
                await fluke.StopScanAsync();
         
                //await fluke.ConfigureTemperatureAsync(channels, 2, "SLOW", 1, "K");
                //await fluke.StartScanAsync();
                //// Start reading data
                //var data = await fluke.ReadTemperaturesAsync();
            }
            // Configure channels 101, 103, 105, and 107 for K-type thermocouples with slow rate
            // and 1.8 second interval
           
            
            ////MyTcpConnectTest tcpTest = new MyTcpConnectTest();
            ////tcpTest.Connect();

            //// Create a new Fluke2638A object with default IP address and port 3490
            //var fluke = new Fluke2638A(@"10.3.3.47",3490);
            //fluke.
            //try
            //{

            //    var identification = await fluke.IdentifyAsync();
            //    Console.WriteLine($"Device identification: {identification}");

            //    // Configure the Fluke device to measure temperatures on channels 0-5 in Celsius units, slow rate, and high resolution
            //    int[] channels = { 0, 1, 2, 3, 4, 5 };
            //    await fluke.ConfigureTemperatureAsync(channels, celsius: true, slowRate: true, highResolution: true);

            //    // Read temperature data once per second
            //    while (true)
            //    {
            //        var dataPoints = await fluke.ReadTemperaturesAsync(channels);

            //        foreach (var dataPoint in dataPoints)
            //        {
            //            Console.WriteLine($"Channel {dataPoint.ChannelIndex}: {dataPoint.Temperature} Celsius at {dataPoint.Timestamp}");
            //        }

            //        await Task.Delay(1000);
            //    }
            //}
            //finally
            //{
            //    // Dispose of the Fluke2638A object when done
            //    fluke.Dispose();
            //}
        }
    }
}
