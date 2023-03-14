using System;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Principal;

class Program
{
    static void Main(string[] args)
    {
        // Set up IP address and port number for the Fluke 2638A
        string ipAddress = "10.3.3.32";
        int port = 3490;

        TcpClient tcpClient = new TcpClient(ipAddress, port);
        NetworkStream networkStream = tcpClient.GetStream();

        try
        {
            // Create a TCP client and connect to the instrument

            string response=string.Empty;
            // Send the* IDN? command to identify the instrument
            string command = "*IDN?\n";
            byte[] commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
            networkStream.Write(commandBytes, 0, commandBytes.Length);

            // Read the instrument identification string
            byte[] responseBytes = new byte[1024];
            while (!networkStream.DataAvailable)
            {
                // Wait for data to be available
            }
            int bytesRead = networkStream.Read(responseBytes, 0, responseBytes.Length);
            response = System.Text.Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

            Console.WriteLine("Instrument identification: {0}", response.Trim());

            // Set the data transfer mode to ASCII
            command = "FORM:DATA ASCII\n";
            commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
            networkStream.Write(commandBytes, 0, commandBytes.Length);

            // Set the data transfer mode to include timestamps
            command = "FORM:TIME:TYPE ABS\n";
            commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
            networkStream.Write(commandBytes, 0, commandBytes.Length);

            // Set the channels to measure
            command = "ROUT:CHAN 101,102,103,104\n";
            commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
            networkStream.Write(commandBytes, 0, commandBytes.Length);

            // Initiate a single measurement cycle
            command = "INIT\n";
            commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
            networkStream.Write(commandBytes, 0, commandBytes.Length);

            // Wait for the measurement cycle to complete
            command = "STAT:OPER:COND?\n";
            commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
            networkStream.Write(commandBytes, 0, commandBytes.Length);
            
            int count = 0;
            while (!networkStream.DataAvailable && count < 10)
            {
                count++;
                Thread.Sleep(300);
                // Wait for data to be available
            }
            bytesRead = networkStream.Read(responseBytes, 0, responseBytes.Length);
            response += System.Text.Encoding.ASCII.GetString(responseBytes, 0, bytesRead);


            // Request the measurement data and timestamps from the instrument
            command = "TRAC:DATA? 1, 1000, \"defbuffer1\", READ, REL, TSTamp\n";
            commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
            networkStream.Write(commandBytes, 0, commandBytes.Length);

            // Read the measurement data and timestamps from the instrument
            while (!networkStream.DataAvailable)
            {
                // Wait for data to be available
            }
            responseBytes = new byte[4096];
            bytesRead = networkStream.Read(responseBytes, 0, responseBytes.Length);
            response = System.Text.Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

            // Parse the measurement data and timestamps
            string[] dataRows = response.Split(',');
            foreach (string dataRow in dataRows)
            {
                string[] dataValues = dataRow.Split(';');
                double measurement = Double.Parse(dataValues[0]);
                DateTime timestamp = DateTime.ParseExact(dataValues[1], "yyyy/MM/dd HH:mm:ss.ffffff", null);
                Console.WriteLine("Measurement: {0}, Timestamp: {1}", measurement, timestamp);
            }

            // Close the network stream and TCP client
            networkStream.Close();
            tcpClient.Close();
        }

        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            networkStream.Close();
            tcpClient.Close();
        }

        Console.ReadKey();
    }
}
