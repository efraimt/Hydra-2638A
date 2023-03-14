using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

internal class FDL2638A1
{


    private TcpClient client;

    public void openport()
    {
        if (client == null)
        {
            client = new TcpClient();

            //init_port();
        }

        if (!client.Connected)
        {
            try
            {
                client.Connect(@"10.3.3.47", 3490);
                client.Client.ReceiveTimeout = 500;

                var s = client.GetStream();
                var sr = new StreamReader(s);
                var sw = new StreamWriter(s);
                sw.AutoFlush = true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}