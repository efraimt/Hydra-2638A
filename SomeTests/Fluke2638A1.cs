using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fluke
{
    public class Fluke2638A1 : IDisposable
    {
        private TcpClient client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private readonly string _ipAddress;
        private readonly int _port;

        public Fluke2638A1(string ipAddress, int port = 3490)
        {
            _ipAddress = ipAddress;
            _port = port;

        }

        public void Connect()
        {
            client = new TcpClient(AddressFamily.InterNetwork);
            client.Connect(_ipAddress, _port);
            //client.Connect(@"10.3.3.47", 3490);
            client.Client.ReceiveTimeout = 500;
            _stream = client.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
            _writer.AutoFlush = true;
            //client.Close();
            //client.Dispose();
        }

        string _ReceivedString;
        public string Send(string message)
        {
            try
            {
                //// Convert the message string to a byte array and send it over the network
                //byte[] buffer = Encoding.ASCII.GetBytes(message);
                //_writer.WriteLine(message);

                //System.Threading.Thread.Sleep(300);
                //// Receive the response from the server
                //buffer = new byte[client.ReceiveBufferSize];
                ////int bytesRead = await _stream.ReadAsync(buffer, 0, _client.ReceiveBufferSize);
                //_stream.ReadTimeout= 2000;
                //string response = _reader.ReadLine();

                //// Convert the response byte array to a string and return it
                ////string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                //return response;

                object locker = new object();
                lock (locker)
                {
                    try
                    {
                        openport();

                        _writer.WriteLine(message);

                        System.Threading.Thread.Sleep(300);

                        _ReceivedString = string.Empty;
                        try
                        {
                            _ReceivedString = _reader.ReadLine();
                        }
                        catch (IOException) { }


                    }
                    catch (Exception e)
                    {
                    }
                    finally
                    {
                        closeport();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
            return _ReceivedString;
        }

        //private async Task<string> SendAsync(string command)
        //{
        //    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(command);
        //    await SendAsync(bytes);
        //    var response = await ReadAsync();
        //    return response;
        //}

        //public async Task SendAsync(byte[] data)
        //{
        //    if (_client == null)
        //        throw new InvalidOperationException("Not connected to device.");

        //    try
        //    {

        //        await _stream.WriteAsync(data, 0, data.Length);
        //        await _stream.FlushAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new IOException("Unable to write data to the transport connection.", ex);
        //    }
        //}

        //public async Task<string> ReadAsync()
        //{
        //    if (_client == null)
        //        throw new InvalidOperationException("Not connected to device.");

        //    try
        //    {
        //        var buffer = new byte[1024];
        //        var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
        //        if (bytesRead == 0)
        //            throw new EndOfStreamException("The connection was closed by the device.");
        //        var response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        //        return response.TrimEnd('\n', '\r');
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }


        //}

        public async Task<string> IdentifyAsync()
        {
            var response =  Send(@"*IDN?");
            return response;
        }

        private void openport()
        {
            if (client == null)
            {
                client = new TcpClient();
            }

            if (!client.Connected)
            {
                client.Connect(_ipAddress, _port);

                client.ReceiveTimeout = 100;

                _stream = client.GetStream();
                _reader = new StreamReader(_stream);

                _writer = new StreamWriter(_stream);
                _writer.AutoFlush = true;
            }
        }


        private void closeport()
        {
            if (client != null)
            {
                //if (client.Connected)
                //{
                //    client.Client.Shutdown(SocketShutdown.Both);// sh Connect("0.0.0.0", 0);
                //    client.Client.Disconnect(true);
                //}
                if (_stream != null)
                {
                    _stream.Close();
                }

                //client.Client.Disconnect(true);
                client.Close();
            }
            //client.Dispose(true);
            client = null;

            _stream = null;
            _reader = null;
            _writer = null;

        }

        public async Task ConfigureTemperatureAsync(int[] channels, bool celsius = true, bool slowRate = true, bool highResolution = true)
        {
            try
            {
                var units = celsius ? "C" : "F";
                var rate = slowRate ? "SLOW" : "FAST";
                var resolution = highResolution ? "HIGH" : "LOW";

                 Send(":ROUTe:CHANnel:DELete ALL");
                 Send(":UNIT:TEMP " + units);
                //the :ROUTe:TEMP:RTD command sets the rate at which the Hydra takes measurements during each sample
                Send(":ROUTe:TEMP:RTD " + rate);
                 Send(":SENS:TEMP:RES " + resolution);

                foreach (var channel in channels)
                {
                     Send($":ROUTe:CHANnel:ADD {channel}");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here, for example, log the error or re-throw the exception.
                Console.WriteLine($"An error occurred while configuring temperature: {ex.Message}");
                throw;
            }
        }


        public async Task<DataPoint[]> ReadTemperaturesAsync(int[] channels)
        {
            Send(":INITiate:IMMediate");
            Send(":TRACe:CLEar");
            Send(":TRACe:FEED:CONTrol NEVer");

            var dataPoints = new DataPoint[channels.Length];

            Send(":TRACe:POINts " + dataPoints.Length);
            Send(":TRACe:FEED:SENSor:TRANsition:TIMe OFF");

            var response =  Send(":TRACe:FEED:SENSor?");
            var sensorData = response.Split(',').Select(int.Parse).ToArray();

            for (int i = 0; i < dataPoints.Length; i++)
            {
                int index = sensorData[i * 2];
                float temperature = sensorData[i * 2 + 1] / 1000.0f;
                DateTime timestamp = DateTime.Now;

                dataPoints[i] = new DataPoint(index, temperature, timestamp);
            }

            return dataPoints;
        }


        public void Dispose()
        {
            _stream?.Dispose();
            client?.Dispose();
        }

    }

    public class DataPoint
    {
        public int ChannelIndex { get; }
        public float Temperature { get; }
        public DateTime Timestamp { get; }

        public DataPoint(int channelIndex, float temperature, DateTime timestamp)
        {
            ChannelIndex = channelIndex;
            Temperature = temperature;
            Timestamp = timestamp;
        }
    }
}
