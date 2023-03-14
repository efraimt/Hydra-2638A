using System.Data;
using System.Diagnostics;
using System.Net.Sockets;

public class MyTcpConnectTest
{
    public void Connect()
    {
        TcpClient client = new TcpClient(AddressFamily.InterNetwork);
        client.Connect(@"10.3.3.47", 3490);
        Debug.WriteLine(client.Connected.ToString() + " " + client.Client.RemoteEndPoint);
        Console.WriteLine(client.Connected.ToString() + " " + client.Client.RemoteEndPoint);
    }
}
