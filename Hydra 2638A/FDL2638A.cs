using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Hydra_2638A
{
    internal class FDL2638A
    {

        #region Fields

        ChannelDefinitionLinks channels;

        const string measurement_NameEn = "Temperature";    //TODO: get real definition
        const string SubGroupData_ValueEN = "TC-K";         //TODO: get real definition


        private TcpClient client;

        private Stream s;
        private StreamReader sr;
        private StreamWriter sw;

        private string _Ip;
        private Int32 _PortNum;

        private const int SELFTEST_TIMEOUT = 50000;
        private const decimal OPEN_TEMPERATURE_SENSOR = 9000000000;
        private Dictionary<int, FDLChannel> _channels;

        private string _command;
        private string _response;
        private string _model;
        private string _name;
        private string _revision;
        private string _serialNumber;
        private int _id;
        private string _error;

        private bool _isconnected;
        private bool _isrun;
        private bool _selftest;
        private int _scanInterval;
        private int _softInterval;
        private int _saveCounter;
        private DateTime _lastDisconnect;
        private DateTime _lastConnect;

        private MeasurementRateEnum _rate;

        private DataHolder _dhMeasurments;
        private DataTable _dtcfg;
        private int _lastIndex;
        private int _lastCorrected;
        private DateTime _startTime;
        private DateTime _lastScanTime;
        private ExecutionStatusEnum _commandStatus;
        private bool _islocked;
        private int _delay;
        private bool _pon;

        Thread thcorr;
        Thread thac;

        private string _ReceivedString;
        private Guid _daguid;
        private DeviceStartUpModes _startupmode;

        #endregion

        #region Constructors
        public FDL2638A(Guid DAGUID)
        {
            _daguid = DAGUID;
            init();
        }
        public FDL2638A(Guid DAGUID, string portName)
        {
            _daguid = DAGUID;
            PortName = portName;
            init();
        }

        #endregion

        #region Properties
        public bool Active
        {
            get { return _isconnected && _isrun; }
        }
        public bool SelfTest
        {
            set { _selftest = value; }
        }
        public int Interval
        {
            get { return _softInterval; }
            set { _softInterval = value; }
        }
        public int EntriesCount
        {
            //get { return _dtMeasurments==null ? 0: _dtMeasurments.Rows.Count; }
            get { return _dhMeasurments == null ? 0 : _dhMeasurments.Count; }

        }

        public bool LoggerLocked
        {
            get { return _islocked; }
            set { _islocked = value; }
        }
        public bool IsConnected
        {
            get
            {
                try
                {
                    identify();
                }
                catch
                {
                    _isconnected = false;
                }
                return _isconnected;
            }
        }

        public int Port
        {
            get { return _PortNum; }
            set { _PortNum = value; }
        }

        public string PortName
        {
            get { return IpAddress(); }
            set { SetIpAddress(value); }
        }

        public Guid DAGUID
        {
            set
            {
                _daguid = value;
            }
        }
        public MeasurementRateEnum Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public List<int> ActiveChannels()
        {
            List<int> temp = new List<int>();
            foreach (KeyValuePair<int, FDLChannel> ch in _channels)
            {
                temp.Add(ch.Value.Number);
            }

            return temp;
        }
        #endregion
        
        #region Public Members

        public void Clear()
        {
            _dhMeasurments = GetNewDataHolder();
        }

        public void StartAcqusition()
        {
            try
            {
                identify();

                if (_isconnected)
                {
                    DadLog.Instanse.Write("DA -StartAcqusition");
                    getPrintStatus();
                    //if (_startupmode == DeviceStartUpModes.Normal)
                    //{
                    //  createdirectory();
                        open();
                        start();
                    //}
                    //else
                    //{
                    //    //recoverdata();
                    //    //if (_dtcfg == null)
                    //    //{
                    //        _startTime = DateTime.Now;
                    //    //}
                    //    //else
                    //    //    _startTime = (DateTime)_dtcfg.Rows[0][DECLARATIONS.CDT_COL_STARTED_DATE];

                    //    if (_dhMeasurments!=null && _dhMeasurments.Count > 0)
                    //        _lastScanTime = _startTime.AddSeconds(_dhMeasurments[_dhMeasurments.Count - 1].ElapsedTime);
                    //    else
                    //        _lastScanTime = _startTime;

                    //    _lastIndex = 1;
                    //    startTimer();
                    //}

                    thcorr = new Thread(new ThreadStart(correctResult));
                    thcorr.Name = _id.ToString() + "_Correction";
                    thcorr.IsBackground = true;
                    thcorr.Priority = ThreadPriority.AboveNormal;
                    thcorr.Start();

                }
                else
                {
                    throw new Exception("Device is not connected to COM PORT");
                }
            }
            catch (Exception e)
            {
                _error = e.Message;
                throw e;
            }

        }

        public DataHolder GetNewDataHolder()
        {
            return new DataHolder();
        }

        public void StopAcqusition()
        {
            try
            {
                //_tm.Stop();
                //string path = AppDomain.CurrentDomain.BaseDirectory+@"DADFiles\";
                object synclock = new object();
                lock (synclock)
                {
                    _isrun = false;
                }
                thcorr.Join();
                thac.Join();
                try
                {
                    savedata();
                    //savedatafiles();
                    //string time = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                    //FileInfo fi;
                    //if (File.Exists(path+_serialNumber + ".dat"))
                    //{
                    //    fi = new FileInfo(path+_serialNumber + ".dat");
                    //    fi.MoveTo(path + _serialNumber + "_" + time + ".bak");

                    //}
                    //if (File.Exists(path + _serialNumber + ".xml"))
                    //{
                    //    fi = new FileInfo(path + _serialNumber + ".xml");
                    //    fi.MoveTo(path + _serialNumber + "_" + time + ".xml");
                    //}
                }
                catch (Exception e)
                {
                    DadLog.Instanse.Write(e.Message);
                }
                foreach (FDLChannel ch in _channels.Values)
                {
                    ch.ResetBP();
                }
                lock_off();
                reset();
                closeport();
                Clear();
            }
            catch (Exception ex)
            {
                DadLog.Instanse.Write(ex.Message);
            }
        }

        public void SetConfiguration(ChannelDefinitionLinks channel)
        {
            this.channels = channel;
            try
            {
                //_channels.Clear();
                //readsettings();
                readchannels();
            }
            catch (Exception e)
            {
                _error = e.Message;
                throw e;
            }
        }

        #endregion

        #region Events
        public event LoggerLogHandler LoggerLog;
        public event LoggerScanHandler DataAcquired;
        public event LoggerDataSavedHandler DataSaved;
        public event EventHandler ChannelConfigurated;
        public event EventHandler LoggerAqcuisitionError;
        public event BarometricPressureChangedHandler BPValueChanged;
        #endregion


        #region Private Members
        #region Driver Commands

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
        private void open()
        {
            if (_isconnected)
            {
                try
                {
                    openport();
                    reset();
                    if (_islocked)
                        remote_on();
                    setdate();
                    if (_selftest)
                    {
                        selftest();
                        if (_error != string.Empty)
                            throw new Exception(_error);
                    }
                    setScanResume();
                    //print_on();
                    rate();
                    //format();
                    init_channels();
                    //_dtMeasurments = createDataTable();
                    Clear();
                }
                catch (Exception e)
                {
                    _error = e.Message;
                    throw new Exception(_error);
                }
            }
            else
            {
                throw new Exception("Unrecognized Logger");
            }
        }
        private void savedata()
        {

            //string path = AppDomain.CurrentDomain.BaseDirectory + @"DADFiles\" + _daguid.ToString();
            //string file = path + @"\" + _serialNumber + @"\" + _serialNumber + ".mdh";

            //if (_lastIndex > _saveCounter)
            //{
            //    try
            //    {
            //        _dhMeasurments.ToFile(file);

            //        _lastIndex = 1;
            //        OnDataSaved(new EventArgs());
            //    }
            //    catch (Exception e)
            //    {
            //        throw e;
            //    }

            //}

            string path = AppDomain.CurrentDomain.BaseDirectory + @"DADFiles\" + _daguid.ToString();
            string file = path + @"\" + _serialNumber + @"\" + _serialNumber + ".json";
            if (_lastIndex > _saveCounter)
            {
                _dhMeasurments.ToJson(file, _startTime); 

            }
        }

        private void saveconfig()
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + @"DADFiles\";
            string path = AppDomain.CurrentDomain.BaseDirectory + @"DADFiles\" + _daguid.ToString();
            string file = path + @"\" + _serialNumber + @"\" + _serialNumber + ".xml";
            DataSet ds = new DataSet(DECLARATIONS.DATASET_NAME);
            try
            {
                ds.Tables.Add(_dtcfg);
                //ds.WriteXml(path+_serialNumber + ".xml", XmlWriteMode.WriteSchema);
                ds.WriteXml(file, XmlWriteMode.WriteSchema);
                ds.Tables.Remove(_dtcfg);
            }
            catch (Exception e)
            {
                DadLog.Instanse.Write(e.Message);
            }

        }

        private void setdate()
        {
            DateTime dt = DateTime.Now;
            set_date(dt.Year, dt.Month, dt.Day);
            set_time(dt.Hour, dt.Minute);
        }
        private void start()
        {
            _startTime = DateTime.Now;
            clear_scan();
            _lastIndex = 1;
            startscan();
            startTimer();
        }
        private void startscan()
        {
            TimeSpan tm = new TimeSpan(0, 0, _scanInterval);
            set_scan_interval(tm.Hours, tm.Minutes, tm.Seconds);
            set_trig_cnt(0);
            set_scan_start();
            _lastScanTime = DateTime.Now;
        }
        
        private int getCount()
        {
            int count = -1;
            getScanCount();

            try
            {
                count = int.Parse(_response);
            }
            catch (Exception e)
            {
                _error = e.Message;
                DadLog.Instanse.Write("FlukeDataLogger:getCount() " + e.Message);
                //throw e;
            }
            return count;
        }
        private void getResults()
        {


            int k;
            while (((k = getCount()) > 0) && _isrun)
            {

                Debug.WriteLine("FDL2638A:getResults() _lastIndex=" + _lastIndex.ToString());
                Debug.WriteLine("FDL2638A:getResults() BufferCounter=" + k.ToString());

                get_scanResult(_lastIndex);
                //System.Diagnostics.Debug.WriteLine(_response);
                string[] buf = _response.Split(new char[] { ',' });

                if (_dhMeasurments.Count == 0)
                {
                    _lastScanTime = _startTime = DateTime.Now;
                    saveconfig();
                }
                if (_pon)
                {
                    //System.Diagnostics.Debug.WriteLine(_logger.MabaID + " power on" + " Last connect:" + _lastConnect.ToShortTimeString() + " Last scan:" + _lastScanTime.ToShortTimeString());
                    TimeSpan ts = _lastConnect - _startTime;
                    int p = (int)(ts.TotalSeconds / _scanInterval);
                    int tm = _dhMeasurments.Count / _channels.Count;

                    System.Diagnostics.Debug.WriteLine("Holes detected:" + p.ToString());

                    for (int i = tm; i < p; i++)
                    {
                        System.Diagnostics.Debug.WriteLine("t_before=" + i.ToString());
                        get_Dummy(_scanInterval * i);
                        System.Diagnostics.Debug.WriteLine("t_after=" + (_scanInterval * _dhMeasurments.Count / _channels.Count).ToString());
                        Thread.Sleep(1);
                    }
                    _pon = false;
                }
                try
                {
                    readscan(buf, _scanInterval * _dhMeasurments.Count / _channels.Count);
                    _lastIndex++;
                    Thread.Sleep(1);
                }
                catch (Exception e)
                {
                    string s = e.Message;
                    DadLog.Instanse.Write(s);
                }
                if (_lastIndex % 10 == 0) savedata();
            }
            //if ((k > 0) && (_lastIndex == k + 1) && _isrun)
            if ((k == 0) /* && (_lastIndex == k + 1)*/ && _isrun)
                savedata();
        }

        private void correctResult()
        {
            while (_isrun)
            {
                try
                {
                    DataHolder dh = _dhMeasurments.FilterByIndex(_lastCorrected);
                    List<DataQuant> dc = new List<DataQuant>();
                    for (int i = 0; i < dh.Count; i++)
                    {
                        DataQuant dq = dh[i + _lastCorrected];
                        FDLChannel c = _channels[dq.Channel];
                        if (dq.InRange != 0)
                        {
                            decimal val = dq.Value;
                            try
                            {
                                //  MessageBox.Show("c.GetCorrectedValue(val) - " + val + ",dq.logger - " + dq.Logger.ToString() + " - c.Instrument" + c.Instrument + "c.Measurement - " + c.Measurement + " c.DA_Unit.LongNameHe - " + c.DA_Unit.LongNameHe);
                                decimal corrval = c.GetCorrectedValue(val);
                                decimal uuval = c.ConvertToUU(corrval);
                                dq.Value = uuval;
                                //if (c.Number != 0)  //Edna Task 225 - Don't add correction to channel 0 
                                //    dq.Value = GetCorrectedLoggerValue(uuval, c.Measurement, c.DA_Unit);//Edna 13/1/14
                                //else
                                //    dq.Value = uuval;
                            }
                            catch (Exception e)
                            {
                                //dq.Value = Convert.ToDecimal(c.Min);
                                //dq.InRange = false;
                                dq.InRange = 0;

                                DadLog.Instanse.Write(e.Message);
                            }
                        }
                        dc.Add(dq);

                        Thread.Sleep(1);
                        if (dc.Count % (10 * _channels.Count) == 0)
                            break;

                    }
                    if (dc.Count > 0)
                    {
                        OnDataAcquired(new ScanResaltArg(dc));
                        object synclock = new object();
                        lock (synclock)
                        {
                            _lastCorrected += dc.Count;
                        }
                    }
                }
                catch (Exception e)
                {
                    DadLog.Instanse.Write(e.Message);
                }
                finally
                {
                    Thread.Sleep(500);
                }
            }

        }
        private void readscan(string[] results, int TimeElapsed)
        {
            int i = 0;
            if (results.Length != _channels.Count) return;
            foreach (KeyValuePair<int, FDLChannel> c in _channels)
            {
                try
                {
                    //const double rd0 = -9.900000e+37;
                    //double rd;
                    //Double.TryParse(results[i++], out rd)
                    double rd = double.Parse(results[i++], System.Globalization.NumberStyles.Float);
                    decimal r = 999;
                    //if (Math.Abs(Math.Abs(rd) - Math.Abs(rd0)) < 100000.1)
                    if ((Math.Abs(rd) < 1.0e+15) && (Math.Abs(rd) > 1.0e-15))
                    {
                        r = Convert.ToDecimal(rd);
                    }
                    DataQuant dq = new DataQuant();
                    dq.Logger = _id;
                    dq.Channel = c.Key;
                    dq.ElapsedTime = TimeElapsed;
                    dq.Value = r;
                    dq.InRange = getInRangeValue(r, c.Value);
                    object synclock = new object();
                    lock (synclock)
                    {
                        _dhMeasurments.Add(dq);
                    }
                }
                catch (Exception e)
                {
                    string s = e.Message;
                    DadLog.Instanse.Write(s);
                    throw e;
                }
            }
        }
        private byte getInRangeValue(decimal value, FDLChannel c)
        {
            decimal min = Convert.ToDecimal(c.Min);
            decimal max = Convert.ToDecimal(c.Max);
            //decimal validmax = c.Measurement.NameEn == Temperature ? 100000d : max;
            byte ret = DECLARATIONS.Status_OOR;
            if (value >= min && value <= max)
            {
                ret = DECLARATIONS.Status_INRANGE;
            }
            else if ((c.IsAllowMinOOR && (value < min)) || (c.IsAllowMaxOOR && (value > max)))
            {
                ret = DECLARATIONS.Status_ALLOWOOR;
            }
            else
                ret = DECLARATIONS.Status_OOR;
            //if (c.Measurement.NameEn == "Temperature" && value == OPEN_TEMPERATURE_SENSOR)
            if (c.Mode == "TEMP" && value == OPEN_TEMPERATURE_SENSOR)
                ret = DECLARATIONS.Status_OOR;

            return ret;
        }

        private void startTimer()
        {
            object synclock = new object();
            lock (synclock)
            {
                _isrun = true;
            }
            thac = new Thread(new ThreadStart(startreading));
            thac.IsBackground = true;
            thac.Name = "thAc_" + _daguid;
            thac.Start();

        }
        private void startreading()
        {
            do
            {
                //Thread.Sleep(4 * _scanInterval * 1000 / 10);

                try
                {
                    if (checkConnection())
                    {
                        getResults();
                    }

                }
                catch (Exception e)
                {
                    DadLog.Instanse.Write(e.Message);
                    //OnLoggerLog(new LoggerLogArg(_command,
                    //throw new Exception(e.Message);
                }
                finally
                {
                    Thread.Sleep(100);

                }

            }
            while (_isrun);
        }
        private bool checkConnection()
        {
            bool connnected = _isconnected;
            _isconnected = TCheckConnect.CheckConnectOne(_Ip, _delay);
            if (_isconnected)
            {
                identify();
            }
            if (connnected && _isconnected) //connect OK
            {
                return true;
            }
            else if (connnected && !_isconnected) //disconnect detected
            {
                _lastDisconnect = DateTime.Now;
                string msg = "Logger is disconnected from " + PortName;
                DadLog.Instanse.Write("FDL2638A:checkConnection() " + msg);
                OnAqcusitionError(msg);
                return false;
            }
            else if (!connnected && _isconnected) //connect detected
            {
                //string msg = "Logger " + _serialNumber + " was connected to " + PortName;
                //OnAqcusitionError(msg);
                //get_esr();
                //_lastConnect = DateTime.Now;
                //int resp;
                //if (int.TryParse(_response, out resp))
                //{
                //    _pon = ((resp & 128) == 128);
                //    msg += ".Reason - " + (_pon ? "Power ON " : "RS232");
                //}
                //if (_islocked)
                //    remote_on();
                //DadLog.Instanse.Write("FDL2638A:checkConnection() " + msg);
                return true;

            }
            else //disconnect continue
            {
                //TimeSpan ts = DateTime.Now.Subtract(_lastDisconnect);
                //if (ts.TotalSeconds > _scanInterval)
                //{
                //    OnAqcusitionError("Logger " + _serialNumber + " is disconnected from " + PortName);
                //    _lastDisconnect = DateTime.Now;

                //}
                return false;
            }
        }

        private string getFunc(int ch, string mode, params string[] Plist)
        {
            string result = string.Empty;
            string range = string.Empty;
            string terminals = string.Empty;
            string type = string.Empty;
            string specific_type;
            int ChannelLogerId = ChannelIndToLoggerChannelId(ch);

            switch (mode.ToUpper())
            {
                case "TEMP":
                    if (Plist.Length > 0)
                    {
                        type = Plist[0];
                    }
                    switch (type.ToUpper())
                    {
                        case "J":
                        case "K":
                        case "E":
                        case "T":
                        case "N":
                        case "R":
                        case "S":
                        case "B":
                        case "C":
                            return @"TEMP:TC:TYPE " + type.ToUpper() + ",(@" + ChannelLogerId.ToString() + ")";
                        //break;
                        case "PT":
                            if (Plist.Length > 1)
                                terminals = Plist[1];
                            specific_type = "A385";
                            if (Plist.Length > 2)
                                specific_type = Plist[2];

                            switch (terminals)
                            {
                                case "2":
                                    return @"TEMP:RTD:TYPE " + specific_type + ",(@" + ChannelLogerId.ToString() + ")";
                                case "4":
                                    if ((ch == 0) || (ch > 10 && ch < 21) || (ch > 30 && ch < 41) || (ch > 50))
                                        throw new Exception("Invalide channel " + ch.ToString());
                                    return @"TEMP:FRTD:TYPE " + specific_type + ",(@" + ChannelLogerId.ToString() + ")";
                            }
                            break;
                        default:
                            throw new Exception("Invalide or missing temperature's sensor type");

                    }
                    result = mode + "," + type.ToUpper() + ((terminals.Length > 0) ? "," : "") + terminals;
                    break;
                case "OFF": throw new Exception(@"Invalide ""OFF"" Parameters");
                //result = mode;
                //break;
                case "VDC":
                    return @"FUNC ""VOLT:DC"" " + ",(@" + ChannelLogerId.ToString() + ")";

                case "VAC":
                    if (Plist.Length > 0)
                    {
                        range = Plist[0];
                        switch (range.ToUpper())
                        {
                            case "AUTO":
                            case "1":
                            case "2":
                            case "3":
                            case "4":
                                range = Plist[0];
                                break;
                            default:
                                throw new Exception("Invalide Range parameter:" + Plist[0]);
                        }
                    }
                    else
                    {
                        range = "AUTO";
                    }
                    result = mode + "," + range;
                    return @"FUNC ""VOLT:AC"" " + ",(@" + ChannelLogerId.ToString() + ")";
                //break;
                case "OHMS":
                    if (Plist.Length > 0)
                    {
                        range = Plist[0];
                        switch (range.ToUpper())
                        {
                            case "AUTO":
                            case "1":
                            case "2":
                            case "3":
                            case "4":
                            case "5":
                            case "6":
                                range = Plist[0];
                                break;
                            default:
                                throw new Exception("Invalide Range parameter:" + Plist[0]);

                        }
                    }
                    else
                    {
                        range = "AUTO";
                    }
                    if (Plist.Length >= 1)
                    {
                        terminals = Plist[1];
                    }
                    switch (terminals)
                    {
                        case "2": return @"FUNC ""VOLT:RES"" " + ",(@" + ChannelLogerId.ToString() + ")";
                        case "4": return @"FUNC ""VOLT:FRES"" " + ",(@" + ChannelLogerId.ToString() + ")";
                        //break;
                        default:
                            throw new Exception("Invalide or missing terminal Parameters");
                    }
                //result = mode + "," + range + "," + terminals;
                //break;
                case "FREQ":
                    if (Plist.Length > 0)
                    {
                        range = Plist[0];
                        switch (range.ToUpper())
                        {
                            case "AUTO":
                            case "1":
                            case "2":
                            case "3":
                            case "4":
                            case "5":
                                range = Plist[0];
                                break;
                            default:
                                throw new Exception("Invalide Range parameter:" + Plist[0]);

                        }
                    }
                    else
                    {
                        range = "AUTO";
                    }
                    result = mode + "," + range;
                    return @"FUNC ""FREQ"" " + ",(@" + ChannelLogerId.ToString() + ")";
                //break;
                default:
                    break;
            }
            return "," + result;
        }

        private void openport()
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
                    client.Connect(_Ip, _PortNum);
                    client.Client.ReceiveTimeout = 500;

                    s = client.GetStream();
                    sr = new StreamReader(s);
                    sw = new StreamWriter(s);
                    sw.AutoFlush = true;
                }
                catch (Exception)
                {
                    throw;
                }

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
                if (s != null)
                {
                    s.Close();
                }

                //client.Client.Disconnect(true);
                client.Close();
            }
            //client.Dispose(true);
            client = null;

            s = null;
            sr = null;
            sw = null;

        }


        private void init_logger()
        {
            try
            {
                //Settings.Settings setts = MySettings.Instanse.Settings;
                _delay = 300;
                identify();
                if (_isconnected)
                {
                    DadLog.Instanse.Write("DA -init_logger");
                    getPrintStatus();
                }
                _channels = new Dictionary<int, FDLChannel>();
                _scanInterval = 5;

                _saveCounter = 5;

#if DEBUG
                _softInterval = 10;
#else
                _softInterval = 30;
#endif
                _id = 99999;//(int)_dtcfg.Rows[0][DECLARATIONS.CDT_COL_LGR_NAME];
                _rate = MeasurementRateEnum.SLOW;
                //_rate = MeasurementRateEnum.FAST;

                _selftest = false;//TODO: get from GUI
                
            }
            catch (Exception e)
            {
                DadLog.Instanse.Write(e.Message);
            }

        }
        private void init()
        {
            //init_port();
            init_logger();
        }

        private void readchannels()
        {
            foreach (var item in channels.Links.Select((selected, ch_no) => new { ch_no, selected }))
            {

                bool isConnected = item.selected;

                if (!isConnected)
                    continue;
                int Ch = item.ch_no; //(int)r[DECLARATIONS.CDT_COL_CHNL_NAME];
                int precision = 1;  //(int)r[DECLARATIONS.CDT_COL_PRECISION_NAME];
                double min = 0; //Convert.ToDouble(r[DECLARATIONS.CDT_COL_FROM_RANGE]);
                double max = 1308;   //Convert.ToDouble(r[DECLARATIONS.CDT_COL_TO_RANGE]);
                bool isBpMeasured = false;  //Convert.ToBoolean(r[DECLARATIONS.CDT_COL_BP_IS_MEASURED]);
                double bpValue = 1013.25;   //Convert.ToDouble(r[DECLARATIONS.CDT_COL_BP_VALUE]);
                string mode;
                int SpecType = 0;   //(int)r[DECLARATIONS.CDT_COL_SPECIFIC_TYPE];
                string SpecTypeStr = (SpecType == 0) ? "A385" : "A392";

                List<string> lprm = new List<string>();

                switch (channels.ChannelDefinition.MeasurementName)
                {
                    case "Temperature":

                        mode = "TEMP";
                        //switch (channels.ChannelDefinition.UnitName)
                        //{
                        //    case "PT-100":
                        //        lprm.AddRange(new string[] { "PT", "4", SpecTypeStr });
                        //        break;
                        //    case "TC-J":
                        //        lprm.AddRange(new string[] { "J" });
                        //        break;
                        //    case "TC-K":
                        //        lprm.AddRange(new string[] { "K" });
                        //        break;
                        //    case "TC-E":
                        //        lprm.AddRange(new string[] { "E" });
                        //        break;
                        //    case "TC-T":
                        //        lprm.AddRange(new string[] { "T" });
                        //        break;
                        //    case "TC-N":
                        //        lprm.AddRange(new string[] { "N" });
                        //        break;
                        //    case "TC-R":
                        //        lprm.AddRange(new string[] { "R" });
                        //        break;
                        //    case "TC-S":
                        //    case "TC-R+S":
                        //        lprm.AddRange(new string[] { "S" });
                        //        break;
                        //    case "TC-B":
                        //        lprm.AddRange(new string[] { "B" });
                        //        break;
                        //    case "TC-C":
                        //        lprm.AddRange(new string[] { "C" });
                        //        break;
                        //    case "טמפרטורה-וולט":
                        //        mode = "VDC";
                        //        lprm.AddRange(new string[] { "AUTO" });
                        //        break;
                        //}
                        lprm.AddRange(new string[] { "K" });
                        break;
                    case "Resistance":
                        mode = "OHMS";
                        lprm.AddRange(new string[] { "AUTO", "2" });
                        break;
                    case "Frequency":
                        mode = "FREQ";
                        //prm=new string[]{"AUTO","2"};
                        lprm.AddRange(new string[] { "AUTO" });
                        break;
                    case "Voltage AC":
                        mode = "VAC";
                        lprm.AddRange(new string[] { "AUTO" });
                        break;
                    case "Voltage DC":
                        mode = "VDC";
                        lprm.AddRange(new string[] { "AUTO" });
                        break;
                    default:
                        mode = "VDC";
                        lprm.AddRange(new string[] { "AUTO" });
                        break;
                }
                string[] prm = new string[lprm.Count];
                lprm.CopyTo(prm);
                createChannel(Ch, mode, prm, min, max, precision, isBpMeasured, bpValue);
                //    //ChannelConfigurated(this,new EventArgs());
                //    OnChannelConfigurated(new EventArgs());
            }

        }

        private int ChannelIndToLoggerChannelId(int ch)
        {
            if (ch == 0) return 1;
            if (ch <= 20) return ch + 100;
            if (ch <= 40) return ch - 20 + 200;
            if (ch <= 60) return ch - 40 + 300;
            return -1;
        }
        private void createChannel(int Ch, string mode, string[] Params, double min, double max, int precision, bool IsBpMeasured, double BpValue)
        {
            FDLChannel channel = new FDLChannel();

            channel.Number = Ch;
            channel.Mode = mode;
            channel.Params = Params;
            channel.Min = min;
            channel.Max = max;
            channel.Precision = precision;
            channel.IsBpMeasured = IsBpMeasured;
            channel.BarometricPressure = BpValue;

            if (!_channels.ContainsKey(Ch))
            {
                _channels.Add(Ch, channel);
            }
        }
        private void exec()
        {
            exec(_delay);
        }
        private void exec(int delay)
        {
            object locker = new object();
            lock (locker)
            {
                try
                {
                    _response = string.Empty;
                    _ReceivedString = string.Empty;

                    openport();
                    DadLog.Instanse.Write("Command:" + _command);
                    sw.WriteLine(_command);

                    System.Threading.Thread.Sleep(delay);

                    _ReceivedString = string.Empty;
                    try
                    {
                        _ReceivedString = sr.ReadLine();
                    }
                    catch (IOException e)
                    {
                        _error = e.Message;
                        DadLog.Instanse.Write("DA - io exeption:" + _error);
                    }

                    DadLog.Instanse.Write("DA  - s - " + _ReceivedString);
                    //DadLog.Instanse.Write("DA  - r - " + _ReceivedString);
                    _response = _ReceivedString;
                    _commandStatus = _response.Trim() == string.Empty ? ExecutionStatusEnum.DeviceNotReady : ExecutionStatusEnum.OK;
                }
                catch (Exception e)
                {
                    _error = e.Message;
                    DadLog.Instanse.Write("DA - exeption:" + _error);
                    _commandStatus = ExecutionStatusEnum.Error;
                    //OnLoggerLog(new LoggerLogArg(_command, _commandStatus.ToString(), System.Threading.Thread.CurrentThread.Name));

                }
                finally
                {
                    //OnLoggerLog(new LoggerLogArg(_command, _commandStatus.ToString(), System.Threading.Thread.CurrentThread.Name));
                    //DadLog.Instanse.Write(s + " " + _command + " " + _commandStatus.ToString());
                    closeport();
                }
            }

        }

        private void init_channels()
        {

            foreach (KeyValuePair<int, FDLChannel> ch in _channels)
            {
                initChannel(ch.Value.Number, ch.Value.Mode, ch.Value.Params);
            }
            //TEMP:TC:TYPE K,(@101, 103:105)
            //_command = Commands2638A.CONFIG_SENS + Gen_channels_Str();// + ch.ToString() + getFunc(ch, mode, par);
            //exec();

            _command = Commands2638A.SCAN + Gen_channels_Str();
            exec();

            //_command = Commands2638A.MON_CHANNEL + "(@102)";
            //exec();

            //_command = Commands2638A.MONITORING;
            //exec();

            //_command = Commands2638A.RWLS;
            //exec();

            //_command = @"SYST:KLOC OFF";
            //exec();
        }

        public void setScanResume()
        {
            _command = Commands2638A.SCANRESUME;
            exec();
            _islocked = false;
        }
        private string Gen_channels_Str()
        {
            string res = @"(@";
            foreach (KeyValuePair<int, FDLChannel> ch in _channels)
            {
                res = string.Format("{0}{1}, ", res, ChannelIndToLoggerChannelId(ch.Value.Number));
            }
            int LastInd = res.LastIndexOf(",");
            if (LastInd != -1)
            {
                res = res.Remove(LastInd);
                res = res + ")";
            }
            else
                res = string.Empty;

            return res;
        }
        private void get_Dummy(int TimeElapsed)
        {
            foreach (KeyValuePair<int, FDLChannel> c in _channels)
            {
                try
                {
                    decimal r = decimal.MinValue;
                    //DataRow dr = _dtMeasurments.NewRow();
                    //dr[DECLARATIONS.RDT_COL_LGR_NAME] = _id;
                    //dr[DECLARATIONS.RDT_COL_CHNL_NAME] = c.Key;
                    //dr[DECLARATIONS.RDT_COL_TIME_NAME] = TimeElapsed;
                    //dr[DECLARATIONS.RDT_COL_VAL_NAME] = r;
                    //dr[DECLARATIONS.RDT_COL_UNIT_NAME] = c.Value.Unit.ID;
                    //dr[DECLARATIONS.RDT_COL_PRECISION_NAME] = c.Value.Precision;
                    //dr[DECLARATIONS.RDT_COL_INRANGE_NAME] = r > Convert.ToDecimal(c.Value.Min) && r < Convert.ToDecimal(c.Value.Max);
                    //_dtMeasurments.Rows.Add(dr);
                    DataQuant dq = new DataQuant();
                    dq.Logger = _id;
                    dq.Channel = c.Key;
                    dq.ElapsedTime = TimeElapsed;
                    dq.Value = r;
                    //dq.Unit = c.Value.Unit.ID;
                    //dq.Precision = c.Value.Precision;
                    //dq.InRange = r > Convert.ToDecimal(c.Value.Min) && r < Convert.ToDecimal(c.Value.Max);
                    dq.InRange = getInRangeValue(r, c.Value);
                    object synclock = new object();
                    lock (synclock)
                    {
                        _dhMeasurments.Add(dq);
                    }
                }
                catch (Exception e)
                {
                    string s = e.Message;
                    DadLog.Instanse.Write(s);
                    throw e;
                }
            }
        }

        #endregion

        #region Basic Logger Commands
        private void set_scan_interval(int Hours, int Minutes, int Seconds)
        {
            //_command = Commands2638A.INTVL + Hours.ToString() + "," + Minutes.ToString() + "," + Seconds.ToString();
            //_command = Commands2638A.INTVL + (Hours + Minutes + Seconds).ToString();
            //Changed by Alex bug #073 v 1.3.0.4 29.11.2016
            _command = Commands2638A.INTVL + (Hours * 360 + Minutes * 60 + Seconds).ToString();
            //_command = string.Format(Commands2638A.INTVL + "{0},{1},{2}", Hours, Minutes, Seconds);

            exec(_delay);
        }
        private void set_scan_start()
        {
            _command = Commands2638A.SCAN_START;
            exec(_delay);
        }
        private void set_trig_cnt(int aCnt)
        {
            _command = Commands2638A.TRIG_CNT + aCnt.ToString();
            exec(_delay);
        }
        //private void set_print_type()
        //{
        //    _command = Commands2638A.PRINT_TYPE + string.Format("{0:0},{1:0}", (int)_printDest, (int)_printType);
        //    exec(_delay);
        //}
        private void set_date(int year, int month, int day)
        {
            _command = Commands2638A.SETDATE + year.ToString() + "," + month.ToString() + "," + day.ToString();
            exec(_delay);
        }
        private void set_time(int hours, int minutes)
        {
            _command = Commands2638A.SETTIME + hours.ToString() + "," + minutes.ToString() + ", 00";
            exec(_delay);
        }
        //private void set_print_on()
        //{
        //    _command = Commands2638A.PRINT + ((int)_printState).ToString();
        //    exec(_delay);
        //}
        private void getScanCount()
        {
            _command = Commands2638A.LOG_COUNT;
            exec(_delay);
            //exec(150);
            //if (_commandStatus != ExecutionStatusEnum.OK)
            //    throw new Exception(_commandStatus.ToString());

        }
        private void clear_scan()
        {
            _command = Commands2638A.LOG_CLR;
            exec(50);
        }
        private void remote_on()
        {
            _command = Commands2638A.RWLS;
            exec();
            _islocked = true;
        }
        private void reset()
        {
            _command = Commands2638A.RST;
            exec();

        }
        private void get_scanResult(int i)
        {
            _command = Commands2638A.LOGGED; //+ i.ToString();
            exec(2 * _delay);

        }
        private void get_stb()
        {
            _command = Commands2638A.STB;
            exec();
        }
        private void get_esr()
        {
            _command = Commands2638A.ESR;
            exec();
        }
        private void clear_logger()
        {
            _command = Commands2638A.CLR;
            exec();
        }
        private void identify()
        {
            DadLog.Instanse.Write("identify");
            try
            {
                _command = Commands2638A.IDN;

                int Cnt = 30;
                do
                {
                    //Changed by Alex bug #301 v 1.3.0.6 25.07.2017
                    //exec(500);
                    exec(1000);
                    //Changed by Alex bug #301 v 1.3.0.6 25.07.2017
                    //Thread.Sleep(100);
                    Thread.Sleep(200);
                } while ((_commandStatus != ExecutionStatusEnum.OK) && (--Cnt > 0));

                //MessageBox.Show((_commandStatus == ExecutionStatusEnum.OK ? "OK, " : "Fail") + Cnt.ToString());

                if (_commandStatus == ExecutionStatusEnum.OK)
                {
                    string[] buff = _response.Split(new char[] { ',' }, StringSplitOptions.None);
                    if (buff.Length >= 4)
                    {
                        _name = buff[0];
                        _model = buff[1];
                        _revision = buff[3];
                        _isconnected = true;
                    }
                    else
                    {
                        DadLog.Instanse.Write("identify - 3");
                        _isconnected = false;
                    }

                }
                else
                {
                    DadLog.Instanse.Write("identify - 4");
                    _isconnected = false;
                    throw new Exception(_commandStatus.ToString());
                }

            }
            catch (Exception e)
            {
                DadLog.Instanse.Write("identify - 5");
                closeport();
                _error = e.Message;
                DadLog.Instanse.Write(_error);
                _isconnected = false;
                //throw e;
            }

        }
        private void selftest()
        {
            closeport();
            _command = Commands2638A.SELFTEST;
            //int timeout = _port.ReadTimeout;
            //_port.ReadTimeout = SELFTEST_TIMEOUT;
            try
            {
                //exec(SELFTEST_TIMEOUT);
                exec_selftest(SELFTEST_TIMEOUT);

                if (_commandStatus == ExecutionStatusEnum.OK)
                {
                    if (!_response.EndsWith("0"))
                    {
                        _error = "Self Test fail with result:" + _response;
                    }
                    else
                        _error = string.Empty;
                }
            }
            catch (Exception e)
            {
                DadLog.Instanse.Write("selftest");
                _error = e.Message;
                DadLog.Instanse.Write(e.Message);
                _isconnected = false;
            }
            finally
            {
                closeport();
                //_port.ReadTimeout = timeout;
                openport();
            }

        }
        private void exec_selftest(int timeout)
        {
            string s = string.Empty;

            try
            {
                openport();
                //_port.DiscardInBuffer();
                sw.WriteLine(_command);
                _ReceivedString = string.Empty;

                //System.Threading.Thread.Sleep(timeout);
                DateTime start = DateTime.Now;
                do
                {
                    s += sr.ReadLine();
                    System.Threading.Thread.Sleep(1);

                } while (!s.Contains("\r\n") && DateTime.Now < start.AddMilliseconds(timeout));
                string[] buf = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                //s = (buf.Length > 1) ? buf[1] : buf[0];
                if (buf.Length > 0)
                {
                    switch (buf[buf.Length - 1])
                    {
                        case "=>":
                            _commandStatus = ExecutionStatusEnum.OK;
                            break;
                        case "!>":
                            //_commandStatus = ExecutionStatusEnum.Unexecuteble;
                            _commandStatus = ExecutionStatusEnum.OK;
                            break;
                        case "?>":
                        default:
                            _commandStatus = ExecutionStatusEnum.Error;
                            break;
                    }
                    _response = buf[0];
                }

                else
                {
                    _commandStatus = ExecutionStatusEnum.DeviceNotReady;
                }

            }
            catch (Exception e)
            {
                _error = e.Message;
                _commandStatus = ExecutionStatusEnum.Error;
                //OnLoggerLog(new LoggerLogArg(_command, _commandStatus.ToString(), System.Threading.Thread.CurrentThread.Name));

            }

        }
        private void lock_off()
        {
            _command = Commands2638A.LOCKOFF;
            exec();
            _islocked = false;
        }

        private void initChannel(int ch, string mode, string[] par)
        {
            _command = getFunc(ch, mode, par);
            //DadLog.Instanse.Write("OldCommand:" + "FUNC " + ch.ToString() + getFunc2625(ch, mode, par));

            ////MessageBox.Show(_command);
            exec();
        }
        private void rate()
        {
            _command = Commands2638A.RATE + _rate.ToString();// ((int)_rate).ToString();
            exec();
        }
        private void getPrintStatus()
        {
            DadLog.Instanse.Write("DA -getPrintStatus");
            _command = Commands2638A.PRINTSTATUS;
            short IntResponce;
            int Cnt = 1;
            do
            {
                exec();
                DadLog.Instanse.Write("DA - CNt - " + Convert.ToString(Cnt) + " _response" + Convert.ToString(_response));
                if (!(Int16.TryParse(_response, out IntResponce)))
                {
                    IntResponce = -1;
                }
            }
            while (!(IntResponce < 256) && (--Cnt > 0));

            _startupmode = IntResponce < 256 ? DeviceStartUpModes.Normal : DeviceStartUpModes.Recovery;

        }

        //#region Driver Commands

        //#endregion


        #endregion
        #endregion


        private void OnAqcusitionError(object sender)
        {
            if (LoggerAqcuisitionError != null)
            {
                LoggerAqcuisitionError(sender, new EventArgs());
            }
        }

        private void OnDataAcquired(ScanResaltArg dh)
        {
            if (DataAcquired != null)
            {
                DataAcquired(this, dh);
            }
        }


    }



    class Commands2638A
    {
        public const string IDN = @"*IDN?";
        public const string SELFTEST = @"*TST?";
        public const string FUNCQUERY = @"FUNC? ";
        public const string FUNC = @"FUNC ";
        //public const string RWLS = @"RWLS";
        public const string RWLS = @"DISP:STAT ON";
        //public const string RWLS = @"LOCK 2";
        public const string CONFIG_SENS = @"TEMP:TC:TYPE K,";
        public const string RST = @"*RST";
        public const string CLR = @"*CLS";
        //public const string INTVL = @"INTVL ";
        public const string INTVL = @"TRIG:TIM ";
        public const string TRIG_CNT = @"TRIG:COUN ";
        //public const string LOCKOFF = @"LOCS";
        public const string LOCKOFF = @"DISP:STAT OFF";
        //public const string UNLOCK = @"LOCK 0";
        public const string SCAN = @"ROUT:SCAN ";
        //Aded by Alex bug #030 v 1.3.0.3 11.08.2016
        public const string SCANRESUME = @"ROUT:SCAN:RES ON";
        public const string MONITORING = @"ROUT:MON:STAT ON";
        public const string MON_CHANNEL = @"ROUT:MON ";
        public const string SCAN_ALL = @"LAST? ";
        //public const string SCAN_START = @"SCAN 1";
        public const string SCAN_START = @"INIT";
        public const string SCAN_STOP = @"SCAN 0";
        //public const string TRG = @"*TRG";
        /// <summary>
        /// the RATE command sets the time interval between samples
        /// </summary>
        public const string RATE = @"RATE ";
        public const string LOG = @"LOG?";
        //public const string LOGGED = @"LOGGED? ";
        public const string LOGGED = @"DATA:READ?";
        //public const string LOG_CLR = @"LOG_CLR";
        public const string LOG_CLR = @"DATA:CLE";

        //public const string LOG_COUNT = @"LOG_COUNT?";
        public const string LOG_COUNT = @"DATA:POIN?";
        public const string PRINT = @"PRINT ";

        public const string PRINTSTATUS = @"STAT:OPER:COND?";
        //public const string PRINTSTATUS = @"PRINT?";

        public const string PRINT_TYPE = @"PRINT_TYPE ";
        //public const string FORMAT = @"FORMAT ";
        public const string SETDATE = @"SYST:DATE ";
        public const string SETTIME = @"SYST:TIME ";
        public const string OK = @"=>";
        public const string STB = @"*STB?";
        public const string ESR = @"*ESR?";

    }

    public class LoggerLogArg : EventArgs
    {
        private string _command;
        private string _result;
        string _thread;
        public LoggerLogArg(string Command, string CommandResult, string ThreadName)
        {
            _command = Command;
            _result = CommandResult;
            _thread = ThreadName;
        }
        public string Command
        {
            get { return _command; }
        }
        public string CommandResult
        {
            get { return _result; }

        }
        public string ThreadName
        {
            get
            {
                return _thread;
            }
        }

    }

    public enum MeasurementRateEnum
    {
        SLOW = 0, FAST = 1
    }

    public enum ExecutionStatusEnum
    {
        OK = 0, Error = 1, Unexecuteble = 2, DeviceNotReady = 3, TimeOut = 4
    }

    // Summary:
    //     Represents device startup modes
    public enum DeviceStartUpModes
    {
        Normal = 0,
        Recovery = 1,
    }

}