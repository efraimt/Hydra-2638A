using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace Hydra_2638A
{
    public class TcpIpExec
    {
        private TcpClient client;

        private Stream s;
        private StreamReader sr;
        private StreamWriter sw;

        private string _Ip;
        private Int32 _PortNum;
        private string _command;
        private string _response;
        public string _ReceivedString;
        private int _delay;
        private ManualResetEvent connectDone;

        public TcpIpExec(string portName, int delay)
        {
            PortName = portName;
            _delay = delay;
            connectDone = new ManualResetEvent(false);
            _ReceivedString = string.Empty;
        }
        public string PortName
        {
            get { return IpAddress(); }
            set { SetIpAddress(value); }
        }
        public string command
        {
            get { return _command; }
            set { _command = value; }
        }
        private void openport()
        {
            if (client == null)
            {
                client = new TcpClient();
            }

            if (!client.Connected)
            {
                client.Connect(_Ip, _PortNum);

                client.ReceiveTimeout = 100;

                s = client.GetStream();
                sr = new StreamReader(s);

                sw = new StreamWriter(s);
                sw.AutoFlush = true;
            }
        }

        private void closeport()
        {
            //return;
            if (client != null)
            {
                if (client.Connected)
                {
                    client.Client.Shutdown(SocketShutdown.Both);// sh Connect("0.0.0.0", 0);
                    client.Client.Disconnect(true);
                }
                if (s != null)
                {
                    s.Close();
                }
                client.Close();
            }
            //client.Dispose(true);
            client = null;

            s = null;
            sr = null;
            sw = null;

        }
        private void exec()
        {
            exec(_delay);
        }

        public void exec(int delay)
        {
            object locker = new object();
            lock (locker)
            {
                try
                {
                    openport();
                    DadLog.Instanse.Write("Command:" + _command);
                    sw.WriteLine(_command);

                    System.Threading.Thread.Sleep(delay);

                    _ReceivedString = string.Empty;
                    try
                    {
                        _ReceivedString = sr.ReadLine();
                    }
                    catch (IOException) { }

                    DadLog.Instanse.Write("DA  - s - " + _ReceivedString);
                    _response = _ReceivedString;
                }
                catch (Exception e)
                {
                    DadLog.Instanse.Write("DA - exeption:" + e.Message);
                }
                finally
                {
                    closeport();
                }
            }

        }
        private void SetIpAddress(string _aPortName)
        {
            string[] buf = _aPortName.Split(':');
            if (buf.Length >= 1)
                _Ip = buf[0];

            _PortNum = 0;
            Int32 TmpPortNum;
            if ((buf.Length >= 2) && (Int32.TryParse(buf[1], out TmpPortNum)))
                _PortNum = TmpPortNum;
        }
        private string IpAddress()
        {
            return _Ip + ':' + _PortNum.ToString();
        }
    }

    public class TCheckConnect
    {
        public class WaitCntClass
        {
            public WaitCntClass(int _WaitCnt)
            {
                WaitCnt = _WaitCnt;
            }
            public int WaitCnt;
        }

        public class StateObject
        {
            public string Ip = string.Empty;
            public int Del;
            public List<string> ResList;
            public AutoResetEvent Ev;
            public object LockObj;
            public WaitCntClass WaitCnt;
            public StateObject(string _Ip, int _Del, List<string> _ResList, AutoResetEvent _Ev, object _LockObj, WaitCntClass _WaitCnt)
            {
                Ip = _Ip;
                Del = _Del;
                ResList = _ResList;
                Ev = _Ev;
                LockObj = _LockObj;
                WaitCnt = _WaitCnt;
            }
        }

        public static bool CheckConnectOne(string Ip, int del)
        {
            return (new Ping().Send(Ip, del).Status == IPStatus.Success);
        }

        static void ThreadProc(Object StateInfo)
        {
            StateObject St = (StateObject)StateInfo;
            if (CheckConnectOne(St.Ip, St.Del))
            {
                lock (St.LockObj)
                {
                    St.ResList.Add(St.Ip);
                }
            }
            if (Interlocked.Decrement(ref St.WaitCnt.WaitCnt) == 0)
            {
                lock (St.LockObj)
                {
                    St.Ev.Set();
                }
            }
        }
        public static List<string> CheckConnectAsync(string IpBase, int IpStart, int IpStop, int delOne)
        {
            int Len = IpStop - IpStart + 1;
            List<string> MyResult = new List<string>(Len);
            AutoResetEvent MyEv = new AutoResetEvent(false);
            object MyLockObj = new Object();
            WaitCntClass WaitCnt = new WaitCntClass(Len);
            for (int i = IpStart; i <= IpStop; i++)
            {
                StateObject st = new StateObject(IpBase + i.ToString(), delOne, MyResult, MyEv, MyLockObj, WaitCnt);
                ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ThreadProc), st);
            }
            Thread.Sleep(1);
            MyEv.WaitOne(Len * 500);
            return new List<string>(MyResult);
        }
    }
}