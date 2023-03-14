using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class TcpCommunication
{
    private TcpClient client;
    private NetworkStream stream;

    public async Task ConnectAsync(string ipAddress, int port)
    {
        try
        {
            // Create a new TcpClient object and connect to the server
            client = new TcpClient();
            await client.ConnectAsync(ipAddress, port);

            // Get a stream object for reading and writing data over the network
            stream = client.GetStream();
        }
        catch (Exception ex)
        {
            // Handle any errors that occur
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    public async Task<string> SendAndReceiveStringAsync(string message)
    {
        try
        {
            // Convert the message string to a byte array and send it over the network
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(buffer, 0, buffer.Length);

            // Receive the response from the server
            buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = await stream.ReadAsync(buffer, 0, client.ReceiveBufferSize);

            // Convert the response byte array to a string and return it
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            return response;
        }
        catch (Exception ex)
        {
            // Handle any errors that occur
            Console.WriteLine("An error occurred: " + ex.Message);
            return null;
        }
    }

    public void Disconnect()
    {
        // Clean up resources (note that we're closing the NetworkStream and TcpClient here)
        stream.Close();
        client.Close();
    }
}
