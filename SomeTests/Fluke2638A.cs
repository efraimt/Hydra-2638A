using SomeTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fluke
{
    public class Fluke2638A : IDisposable
    {
        private string ip;
        private int port;
        private TcpClient client;
        private Stream stream;
        private StreamWriter writer;
        private StreamReader reader;

        private IEnumerable<int> channels = new List<int>();

        public Fluke2638A(string ip, int port=3490)
        {
            this.ip = ip;
            this.port = port;
            ConnectAsync();

                    }

        private void ConnectAsync()
        {
            client = new TcpClient(AddressFamily.InterNetwork);
            client.Connect(ip, port);
            if (!client.Connected)
            {

                Debugger.Break();
            }
            client.ReceiveTimeout= 300;
            stream = client.GetStream();
            stream.ReadTimeout = 500;
            writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };
            reader = new StreamReader(stream, Encoding.ASCII);


            
        }


        public async Task<string> IdentifyAsync()
        {
            return await SendAsync("*IDN?");
        }

        public async Task<HydraStatus> GetStatusAsync()
        {
            var response = await SendAsync("STAT:OPER:COND?");
            int statusInt = 0;
            int.TryParse(response?.Trim(), out statusInt);

            return (HydraStatus)statusInt;
        }

        public async Task TurnDisplayOnAsync()
        {
            await SendNoOutputCommandAsync(":DISPlay:STATe ON");
        }

        public async Task TurnDisplayOffAsync()
        {
            await SendNoOutputCommandAsync(":DISPlay:STATe OFF");
        }

        public async Task SetDateTimeAsync(DateTime dateTime)
        {
            var command = string.Format(":SYST:DATE {0:yyyy/MM/dd}", dateTime);
            await SendNoOutputCommandAsync(command);

            command = string.Format(":SYST:TIME {0:HH:mm:ss}", dateTime);
            await SendNoOutputCommandAsync(command);
        }

        public async Task SetCurrentDateTimeAsync()
        {
            await SetDateTimeAsync(DateTime.Now);
        }

        public async Task SetScanResolutionAsync(ScanResolution resolution= ScanResolution.High)
        {
            var resolutionOption = resolution == ScanResolution.High ? "HIGH" : "LOW";
            var command = $":SENS:FUNC:RES {resolutionOption}";
            await SendNoOutputCommandAsync(command);
        }

        public async Task SetScanRateAsync(ScanRate rate, double interval)
        {
            var command = $":ROUT:SCAN:RATE {rate},{interval:F3}";
            await SendNoOutputCommandAsync(command);
        }

        public async Task SetTemperatureTypeForSingleChannelAsync(int channel,TemperatureType type)
        {
            var command = $":TEMP:TC:TYPE {type},(@{channel})";
            await SendNoOutputCommandAsync(command);
        }

        private async Task SetTemperatureTypeForChannelsAsync(IEnumerable<int> channels, TemperatureType type)
        {
            foreach (var channel in channels)
            {
                await SetTemperatureTypeForSingleChannelAsync(channel, type);
            }
        }

        public async Task SetTriggerTimeAsync(int timeInSeconds)
        {
            var command = $":TRIG:TIMe {timeInSeconds}";
            await SendNoOutputCommandAsync(command);
        }



        /// <summary>
        /// 1. Resets all channels
        /// 2. Clears all data
        /// 3. Set channels from the list
        /// 4. Set the trmocouple type for each channel in the list
        /// </summary>
        /// <param name="channels"></param>
        /// <returns></returns>
        public async Task ConfigureScanAsync( IEnumerable<int> channels,   int triggerTimeInSeconds=1,TemperatureType type = TemperatureType.K)
        {
            this.channels = channels;
            await ReasetAllChannelsAsync();
            await ClearTraceDataAsync();
            // Add channels to scan list
            var command = $":ROUT:SCAN (@{string.Join(",", channels)})";
            await SendNoOutputCommandAsync(command);
            await SetTriggerTimeAsync(triggerTimeInSeconds);
            await SetTemperatureTypeForChannelsAsync(channels, type);
        }

        public async Task ReasetAllChannelsAsync()
        {
            // Reset all channels
            var command = @":ROUT:CHAN:DEL:ALL";
            await SendNoOutputCommandAsync(command);
        }

        public async Task ClearDataAsync()
        {
            var command = ":DATA:CLE";
            await SendNoOutputCommandAsync(command);
        }

        /// <summary>
        /// This will clear the buffer and start the acquisition anew, effectively deleting older records from the buffer.
        /// </summary>
        /// <returns></returns>
        public async Task ClearTraceDataAsync()
        {
            var command = "TRAC:CLE";
            await SendNoOutputCommandAsync(command);
        }

        public async Task StartMeasureAsync()
        {
            await SendNoOutputCommandAsync("INIT");
        }

        public async Task<int> GetDataPointCountAsync()
        {
            var command = "DATA:POINts?";
            var response = await SendAsync(command);
            if (int.TryParse(response, out int count))
            {
                return count;
            }
            else
            {
                throw new Exception($"Invalid response received: {response}");
            }
        }


        int _lastDataIndex = 1;
        public async Task<double[][]> ReadDataFromLastIndexAsync()
        {
            if (channels == null) throw new InvalidOperationException("Can't read data when channels are not set");
            int dataPointCount = await GetDataPointCountAsync();
            if (dataPointCount == 0)
            {
                return null;
            }

            int startIndex = _lastDataIndex + 1;
            int endIndex = startIndex + dataPointCount - 1;
            _lastDataIndex = endIndex;

            var command = $":TRAC:DATA? {startIndex}, {endIndex}, \"defbuffer1\", SOUR, READ";
            var response = await SendAsync(command);

            var dataPoints = new double[dataPointCount][];
            var lines = response.Split(',');

            for (int i = 0; i < dataPointCount; i++)
            {
                dataPoints[i] = new double[channels.Count()];
                for (int j = 0; j < channels.Count(); j++)
                {
                    int index = i * channels.Count() + j;
                    double value = double.Parse(lines[index]);
                    dataPoints[i][j] = value;
                }
            }

            return dataPoints;
        }



        public async Task ConfigureTemperatureAsync(int[] channels, int resolution, string rate, int interval, string thermocoupleType)
        {
            var resolutionStr = resolution.ToString("F1");
            var units = "C";
            var rateStr = rate == "SLOW" ? "1.8" : "0.8";

            await SendAsync(":ROUT:SCAN:INT " + interval);
            await SendAsync(":ROUT:SCAN:RES " + resolutionStr);
            await SendAsync(":SENS:TEMP:TC:TYPE " + thermocoupleType);
            await SendAsync(":SENS:TEMP:TC:UNIT " + units);
            await SendAsync(":ROUT:SCAN:COUNt 100");

            await SendAsync(":ROUT:SCAN:DELete:ALL");
            foreach (var channel in channels)
            {
                await SendAsync($":ROUT:SCAN:ADD {channel}");
            }

            await SendAsync($":ROUT:SCAN:RTD {rateStr}");

            /*
             * How many times to scan, or 
             :ROUT:SCAN:COUN INF
            to go until we send STOP command
             */
            await SendAsync(":ROUT:SCAN:COUNt 100");
            await SendAsync(":ROUT:SCAN:LOOP");
        }

        public async Task StartScanAsync()
        {
            await SendAsync(":INIT");
        }

        public async Task StopScanAsync()
        {
            await SendAsync("ABORT");
        }



        public async Task<double[]> ReadTemperaturesAsync()
        {
            await SendAsync(":FETC?");
            var response = await ReadAsync();
            var dataPoints = response.Trim().Split(',');
            var temperatures = new double[dataPoints.Length];
            for (int i = 0; i < dataPoints.Length; i++)
            {
                temperatures[i] = double.Parse(dataPoints[i]);
            }
            return temperatures;
        }

        public async Task<string> SendAsync(string command)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(command + "\r\n");
            await stream.WriteAsync(buffer, 0, buffer.Length);
            await Task.Delay(300);
            var response = await ReadAsync();
            return response;
        }

        public async Task SendNoOutputCommandAsync(string command)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(command + "\r\n");
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task<string> ReadAsync(int timeout = 500)
        {
            var buffer = new byte[1024];
            using (var cts = new CancellationTokenSource(timeout))
            {
                try
                {
                    var byteCount = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                    return Encoding.ASCII.GetString(buffer, 0, byteCount);
                }
                catch (OperationCanceledException)
                {
                    //The last command has no output
                    return string.Empty;
                }
            }
        }

        //private async Task<string> ReadAsync()
        //{
        //    try
        //    {
        //        var buffer = new byte[1024];
        //        var byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
        //        return Encoding.ASCII.GetString(buffer, 0, byteCount);
        //    }
        //    catch (IOException ex)//No message to read
        //    {
        //        // Handle the read timeout exception here
        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    { 
        //        return string.Empty; 
        //    }
        //}

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
            stream.Dispose();
            client.Dispose();
        }
    }
}
