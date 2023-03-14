using Hydra_2638A.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydra_2638A
{
    public partial class frmMain : Form
    {

        FDL2638A log = null;
        //private LoggerDefinition _data;

        public string LoggerMabaId;

        private ChannelDefinitionLinks _channel;
        private SortedList<string, bool> _ports;
        private string _IpAddress = null;
        private string _serialNumber = null;
        private int _scanInterval;

        #region LogerLanManager Fields
        private int _loggerid;
        private SortedDictionary<int, string> _loggers;
        #endregion

        #region DataAcqusitionDevice Fields

        private int _interval;
        private bool _isactive;
        private bool _islocked;
        private bool _isremote = false;//Edna true;
        private bool _isselftest;
        private DataHolder _dhMeasurments;
        private DataTable _dtConfig;
        private DateTime _startTime;

        private DeviceStartUpModes _startupmode;
        public event EventHandler ChannelConfigurated;
        public event EventHandler DADAqcusitionError;
        private Guid _daguid;
        private MeasurementRateEnum _rate;
        
        public int logid;

        #endregion

        public frmMain()
        {
            InitializeComponent();

            _dhMeasurments = new DataHolder();
            _channel = new ChannelDefinitionLinks();
            _loggerid = -1;
            ScanInterval = 5;
            GetLoggersList(); 
            init();

        }

        private void GetLoggersList()
        {
            Dictionary<int, string> instruments = new Dictionary<int, string>();

            DataView MyDataView = new DataView();

            instruments.Add(24, "21-54");
            instruments.Add(44, "21-80");
            instruments.Add(72, "21-109");
            instruments.Add(90, "21-130");
            instruments.Add(159, "21-194");
            instruments.Add(213, "21-242");
            instruments.Add(214, "21-243");
            instruments.Add(303, "21-260");
            instruments.Add(304, "21-261");
            instruments.Add(339, "21-142");
            instruments.Add(371, "21-296");
            instruments.Add(377, "21-297");
            instruments.Add(434, "21-21-260-2");
            instruments.Add(440, "21-330");
            instruments.Add(447, "21-336");
            instruments.Add(457, "21-357");
            instruments.Add(458, "21-337");
            instruments.Add(459, "21-358");
            instruments.Add(616, "21-378");
            instruments.Add(636, "21-80-1");
            instruments.Add(792, "21-431"); 
            instruments.Add(2184, "21-473");
            instruments.Add(2196, "21-474");
            instruments.Add(2222, "21-527");
            instruments.Add(2258, "21-534");
            instruments.Add(2306, "21-549");
            instruments.Add(3623, "21-596");
            instruments.Add(3624, "21-597");

            _loggers = new SortedDictionary<int, string>(instruments);
        }


        private void btnFindDevices_Click(object sender, EventArgs e)
        {
            //FDL2638A1 fDL2638A = new FDL2638A1();
            //fDL2638A.openport();
            //_data = new DadLanFlukeData();
            init_ports();
            foreach (KeyValuePair<string, bool> item in _ports)
            {
                KeyValuePair<string, bool> FlukeData = (KeyValuePair<string, bool>)item;
                string[] port = FlukeData.Key.Split(',');

                lstDevices.Items.Add(port[0]+","+port[1]);
            }
        }


        private void init_ports()
        {
            _ports = new SortedList<string, bool>();
            string BaseIp = "10.3.3.";
            string BasePort = ":3490";
            int BegIp = 31;
            int EndIp = 50;
            List<string> LRes = TCheckConnect.CheckConnectAsync(BaseIp, BegIp, EndIp, 100);
            //LoadZeroDAD();

            if (LRes.Count > 0)
            {
                TcpIpExec _exec = new TcpIpExec(BaseIp + BegIp.ToString() + BasePort, 300);                
                _exec.command = @"*IDN?";

                foreach (string Port in LRes)
                {
                    _exec.PortName = Port + BasePort;
                    int Cnt = 2;
                    do
                    {
                        _exec.exec(300);
                    } while (!_exec._ReceivedString.Contains("FLUKE") && (--Cnt > 0));

                    if (_exec._ReceivedString.Contains("FLUKE"))
                    {
                        string Serial = string.Empty;
                        string[] buf = _exec._ReceivedString.Split(',');

                        if (buf.Length >= 3)
                        {
                            Serial = buf[2];
                        }

                        string dadZero = CheckZeroDAD(_exec.PortName);
                        if (dadZero != "")
                            Serial = dadZero;

                        _ports.Add(_exec.PortName + ',' + Serial, true);
                    }

                }
            }
        }        

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (log.IsConnected)
            {

                setChannels();

                btnStart.Enabled = true;
            }

        }

        Dictionary<string, string> zeroList = new Dictionary<string, string>();
        private void LoadZeroDAD()
        {
            var list = File.ReadAllLines(@"C:\Program Files (x86)\Maba Hazorea\Kyulan Data Acquisitor\ZeroDAD.dat");
            foreach (var item in list)
            {
                zeroList.Add(item.Split(',')[0], item.Split(',')[1]);
            }
        }

        private string CheckZeroDAD(string key)
        {
            if (zeroList.ContainsKey(key))
                return zeroList[key];
            return "";
        }

        private void setChannels()
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            log.SetConfiguration(_channel);
            LinkChannels(_channel);
            ResetChannelsGrid();
            this.Cursor = Cursors.Default;
            Application.DoEvents();

        }
        
        private void ResetChannelsGrid()
        {
            dgvChannels.Rows.Clear();
            List<int> channels = log.ActiveChannels();
            dgvChannels.Rows.Add(channels.Count-1);
            dgvChannels.Refresh();

            foreach (var item in channels.Select((value, i) => new { i, value }))
            {
                dgvChannels.Rows[item.i].Cells[0].Value = item.value;
                dgvChannels.Refresh();
            }

        }

        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDevices.SelectedItem == null)
                return;

            string selected = lstDevices.SelectedItem.ToString();
            //string[] port = FlukeData.Key.Split(',');

            string[] Com_MabaId = selected.Split(new char[] { ',' });

            _IpAddress = Com_MabaId[0];

            if (Com_MabaId.Length > 0)
            {
                LoggerMabaId = Com_MabaId[1];
            }
            

            _daguid = Guid.NewGuid();
            //logid = rnd(1);
            log = new FDL2638A(_daguid, _IpAddress);
            //log.ChannelConfigurated += new EventHandler(log_ChannelConfigurated);

            SetLoggerCombo(LoggerMabaId);
            btnConnect.Enabled = true;

        }

        void cbLogger_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex > -1)
            {
                _loggerid = ((KeyValuePair<int, string>)cb.SelectedItem).Key;
                _serialNumber = ((KeyValuePair<int, string>)cb.SelectedItem).Value;
                //createLogger();
                //UpdateView();
                log.SerialNumber = _serialNumber;
                log.Id = _loggerid;
            }
            else
            {
                _loggerid = -1;
            }
            //UpdateLoggerData();            
            //lockTheButtons();
            Cursor = Cursors.Default;
            Application.DoEvents();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopAqcusition();
        }

         private void btnStart_Click(object sender, EventArgs e)
        {
            if (log.IsConnected)
            {
                //setChannels();
                StartAcqusition();
            }

        }       
        
       

        public void StartAcqusition()
        {
            bool _iconnected = log.IsConnected;

            if (!_iconnected)
            {
                string msg = "Device is not ready";
                MessageBox.Show(msg);
                return;
            }


            //int lastcorrected = _dhMeasurments.FilterByTool(log.ID).Count;
            //log.LastCorrected = lastcorrected;
            //log.SelfTest = _isselftest;
            log.DAGUID = _daguid;
            createdirectory();
            log.SelfTest = _isselftest;
            log.DAGUID = _daguid;
            log.LoggerLocked = _isremote;
            log.Rate = _rate;
            log.StartAcqusition();
            if (!log.Active)
            {
                throw new Exception("StartAcqusition Error");
            }
            else
            {
                btnStop.Enabled = true;
            }

        }
        public void StopAqcusition()
        {
            try
            {

                if (log != null)
                {
                    if (DialogResult.Yes == MessageBox.Show("Do you really want to stop the Acquisition Device?", "Data Acquisitor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                    {
                        log.StopAcqusition();

                    }
                }
                else
                    throw new Exception("Device is not initializated!");
            }
            catch (Exception e)
            {
                DadLog.Instanse.Write(e.Message);
            }

        }


        private void LinkChannels(ChannelDefinitionLinks links)
        {
            bool ExistsLinks = false;
            for (int i = 0; i < links.Links.Length; i++)
            {
                ExistsLinks = ExistsLinks || links.Links[i];
                //if (links.Links[i])
                //{
                //    if (IsValidLink(i))
                //        setChannelDefinition(i, links.ChannelDefinition);
                //    else
                //    {
                //        IInstrument sensor = _dbo.GetInstrument(links.ChannelDefinition.SensorID);
                //        string msg = "Selected sensor(" + sensor.MabaID + ") can not be linked to channel " + i.ToString();
                //        MessageBox.Show(msg, "Data Acquisitor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        MessageBox.Show("Selected sensor can not be linked to channel", "Data Acquisitor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    }
                //}
            }
            if (ExistsLinks)
            {
                createLogger(logid);
                //UpdateView();
            }
        }

        private bool IsValidLink(int ch)
        {
            return !(((ch == 0) && (!_channel.ChannelDefinition.Is2W)) || ((ch >= 11) && (!_channel.ChannelDefinition.Is2W/* || (_channel.ChannelDefinition.Is2W && !_data.Channels[ch - 10].Is2W)*/)));
        }

        //private void setChannelDefinition(int channel, ChannelDefinition def)
        //{
        //    _data.Channels[channel] = (ChannelDefinition)def.Clone();
        //    if (!def.Is2W)
        //    {
        //        _data.Channels[channel + 10] = new ChannelDefinition();
        //    }
        //}

        /// <summary>
        /// Add DataLogger to DataAcqusition Device
        /// </summary>
        /// <param name="Logger">ID of DataLogger</param>
        private void createLogger(int Logger)
        {
            //DataTable dtConfig = GetConfigTable(Logger);

            _channel.ChannelDefinition = new ChannelDefinition();
            log.ChannelConfigurated += new EventHandler(log_ChannelConfigurated);

            if (log.IsConnected)
            {
                //log.SetConfiguration(_channel); //log.SetConfiguration(dtConfig);
                log.LoggerLog += new LoggerLogHandler(OnLoggerLog);
                log.DataAcquired += new LoggerScanHandler(log_DataAcquired);
                log.DataSaved += new LoggerDataSavedHandler(log_DataSaved);
                log.LoggerAqcuisitionError += new EventHandler(log_LoggerAqcuisitionError);
                //_loggersList.Add(Logger.ToString(), log);
            }
            else
            {
                string msg = "Device is not enabled";
                //WriteToLog(msg);
                throw new Exception(msg);
            }


            //getStartUpModeStatus();

        }


        /// <summary>
        /// Return current configuration table for DataAcqusition Device
        /// </summary>
        /// <returns></returns>
        public DataTable GetConfigTable()
        {
            return _dtConfig;
        }

        /// <summary>
        /// Return configuration table for specific DataLogger
        /// </summary>
        /// <param name="Logger">ID of DataLogger</param>
        /// <returns></returns>
        public DataTable GetConfigTable(int Logger)
        {
            DataView dv = new DataView(_dtConfig);
            dv.RowFilter = DECLARATIONS.CDT_COL_LGR_NAME + "=" + Logger.ToString();
            dv.Sort = DECLARATIONS.CDT_COL_LGR_NAME + "," + DECLARATIONS.CDT_COL_CHNL_NAME + " ASC";
            //dv.RowFilter = DECLARATIONS.CDT_COL_LGR_NAME + "," + DECLARATIONS.CDT_COL_CHNL_NAME + " ASC";

            DataTable dt = dv.ToTable();
            return dt;
        }


        private void savedata()
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + @"\DADFiles\";
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\DADFiles\" + _daguid.ToString() + @"\";
            string file = path + "DAD.mdh";
            object synclock = new object();
            lock (synclock)
            {
                if (_dhMeasurments.Count > 0)
                {
                    //_dhMeasurments.ToFile(file);
                }
            }

        }

        public void SetLoggerCombo(string MyMabaId)
        {
            ComboBox cb = (ComboBox)cbLoggers;
            if (cb.SelectedIndex != -1) return;

            cb.SelectedIndexChanged -= new EventHandler(cbLogger_SelectedIndexChanged);
            cb.SelectedIndex = -1;
            cb.SelectedIndexChanged += new EventHandler(cbLogger_SelectedIndexChanged);

            int i = cb.FindString(MyMabaId);
            cb.SelectedIndex = i;

        }


        private void init()
        {
            init_channelview();
        }

        public void init_channelview()
        {
            foreach (Control c in grpChannels.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox chkChannel = (CheckBox)c;
                    chkChannel.CheckedChanged += new EventHandler(chkChannel_CheckedChanged);
                }
            }        

        }

        void chkChannel_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            int n = int.Parse(c.Text);
            _channel.SetLink(n, c.Checked);
            //lockTheButtons();
        }


        void log_ChannelConfigurated(object sender, EventArgs ea)
        {
            OnDadChannelConfigurated(ea);
        }

        private void OnDadChannelConfigurated(EventArgs ea)
        {
            if (ChannelConfigurated != null)
            {
                ChannelConfigurated(this, ea);
            }
        }

        public void OnLoggerLog(object sender, LoggerLogArg ea)
        {
            //FDL2638A log = (FDL2638A)sender;
            //string msg = log.SerialNumber + ",command: " + ea.Command + ",status: " + ea.CommandResult + ",thread: " + ea.ThreadName;
            //WriteToLog(msg);
        }

        void log_DataAcquired(object sender, ScanResaltArg NewData)
        {
            object synclock = new object();
            lock (synclock)
            {
                //_dhMeasurments.AddRange(NewData);
                _dhMeasurments.Merge(NewData.ScanResalt);
                ShowData(NewData);
            }
        }

        void log_DataSaved(object sender, EventArgs ea)
        {
            savedata();
        }

        void log_LoggerAqcuisitionError(object sender, EventArgs e)
        {
            OnAqcusitionError(sender);
        }

        private void OnAqcusitionError(object sender)
        {
            if (DADAqcusitionError != null)
            {
                DADAqcusitionError(sender, new EventArgs());
            }
        }

        private void createdirectory()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"DADFiles\" + _daguid.ToString() + @"\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        //public void SaveToFile(string FileName)
        //{
        //    //string path = AppDomain.CurrentDomain.BaseDirectory + @"\DADFiles\";
        //    //string time = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        //    if (FileName != string.Empty)
        //    {
        //        DaConverter.DaConverter dac = new DaConverter.DaConverter();
        //        dac.Save(FileName, _dtConfig, _dhMeasurments);
        //        DadLog.Instanse.Write("data saved to file " + FileName);
        //    }
        //}


        private void ShowData(ScanResaltArg data)
        {
            Thread listeningThread;
            listeningThread = new Thread(() => {
                for (int row = 0; row < data.ScanResalt.Count; row++)
                {
                    Invoke((MethodInvoker)delegate {

                        if (dataGridView1.Rows.Count < data.ScanResalt.Count)
                            dataGridView1.Rows.Add();
                        dataGridView1.Rows[row].Cells[0].Value = data.ScanResalt[row].Channel.ToString();
                        dataGridView1.Rows[row].Cells[1].Value = data.ScanResalt[row].Value.ToString();
                        dataGridView1.Rows[row].Cells[2].Value = DateTime.Now.ToString();
                    });
                    Thread.Sleep(30);
                }
            });
            listeningThread.Start();



        }

        private void btnStopReading_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            BoundLoggerData();
            //UpdateLoggerCombo();
            BoundMeasurementData();
            btnStart.Enabled = false;
            btnStop.Enabled = false;
        }

        private DataTable create_m2m()
        {
            DataTable dt = new DataTable();
            DataColumn c = new DataColumn("MeasurementID", System.Type.GetType("System.Int32"));
            c.AllowDBNull = false;
            dt.Columns.Add(c);
            c = new DataColumn("ID", System.Type.GetType("System.Int32"));
            c.AllowDBNull = false;
            dt.Columns.Add(c);

            c = new DataColumn("Name", System.Type.GetType("System.String"));
            c.MaxLength = 100;
            c.AllowDBNull = true;
            dt.Columns.Add(c);


            dt.PrimaryKey = new DataColumn[] { dt.Columns["MeasurementID"], dt.Columns["ID"] };

            //Temperature->Temperature
            DataRow r = dt.NewRow();
            r["MeasurementID"] = 1;
            r["ID"] = 1;
            r["Name"] = "Temperature";
            dt.Rows.Add(r);

            //Pressure->Pressure
            r = dt.NewRow();
            r["MeasurementID"] = 2;
            r["ID"] = 2;
            r["Name"] = "Pressure";
            dt.Rows.Add(r);

            //Humidity->Humidity
            r = dt.NewRow();
            r["MeasurementID"] = 3;
            r["ID"] = 3;
            r["Name"] = "Humidity";
            dt.Rows.Add(r);

            //CO2 Concentration->CO2 Concentration
            r = dt.NewRow();
            r["MeasurementID"] = 4;
            r["ID"] = 4;
            r["Name"] = "CO2 Concentration";
            dt.Rows.Add(r);

            r = dt.NewRow();
            r["MeasurementID"] = 5;
            r["ID"] = 5;
            r["Name"] = "Pressure absolute";
            dt.Rows.Add(r);

            //Voltage DC->Voltage DC
            r = dt.NewRow();
            r["MeasurementID"] = 6;
            r["ID"] = 6;
            r["Name"] = "Voltage DC";
            dt.Rows.Add(r);

            //Resistance->Resistance
            r = dt.NewRow();
            r["MeasurementID"] = 10;
            r["ID"] = 10;
            r["Name"] = "Resistance";
            dt.Rows.Add(r);

            //Frequency->Frequency
            r = dt.NewRow();
            r["MeasurementID"] = 11;
            r["ID"] = 11;
            r["Name"] = "Frequency";
            dt.Rows.Add(r);

            //Voltage AC->Voltage AC
            r = dt.NewRow();
            r["MeasurementID"] = 12;
            r["ID"] = 12;
            r["Name"] = "Voltage AC";
            dt.Rows.Add(r);

            r = dt.NewRow();
            r["MeasurementID"] = 13;
            r["ID"] = 13;
            r["Name"] = "RPM";
            dt.Rows.Add(r);

            return dt;
        }

        private void BoundMeasurementData()
        {
            DataTable _dtm2m = create_m2m();

            cbMeasurement.SelectedIndex = -1;
            cbMeasurement.SelectedIndexChanged += new EventHandler(cbMeasurement_SelectedIndexChanged);
            cbMeasurement.DataSource = _dtm2m;
            cbMeasurement.ValueMember = "ID";
            cbMeasurement.DisplayMember = "Name";
            cbMeasurement.SelectedIndex = (cbMeasurement.Items.Count > 0) ? 0 : -1;           
        }

        void cbMeasurement_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex != -1)
            {
                int id = (int)((DataRowView)cb.SelectedItem)["ID"];
                string name = (string)((DataRowView)cb.SelectedItem)["NAME"];
                _channel.ChannelDefinition.MeasurementID = id;
                _channel.ChannelDefinition.MeasurementName = name;
                if (!name.ToLower().StartsWith("pressure"))
                    _channel.ChannelDefinition.IsBP_Measured = false;
                //int dau = _adapter.GetDaUnit(id);
                //_channel.ChannelDefinition.DaUnitID = dau;
                _channel.ChannelDefinition.DaUnitID = name == "Temperature" ? 14 : 51;

                //setWorkRangeUnit();
                BoundUnitData(id);
                //setUnit(m.GetDefaultMeasurementUnit().ID);
            }
            else
            {
                _channel.ChannelDefinition.MeasurementID = -1;
                //setWorkRangeUnit();

            }

        }

        void cbUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex != -1)
            {
                Unit unit = (Unit)cb.SelectedItem;

                _channel.ChannelDefinition.UnitID = unit.ID;
                _channel.ChannelDefinition.UnitName = unit.Name;
            }
        }

        private void BoundUnitData(int mesurementid)
        {
            cbUnit.SelectedIndexChanged -= new EventHandler(cbUnit_SelectedIndexChanged);
            cbUnit.DisplayMember = "Name";
            cbUnit.ValueMember = "ID";
            cbUnit.SelectedIndex = -1;

            cbUnit.Items.Clear();
            cbUnit.Text = "";
            if (mesurementid > -1)
            {

                Unit[] units = GetUnitsAddMSRLimits(mesurementid);
                if (units != null)
                {
                    for (int i = 0; i < units.Length; i++)
                    {
                        cbUnit.Items.Add(units[i]);
                    }
                }

            }
            cbUnit.SelectedIndexChanged += new EventHandler(cbUnit_SelectedIndexChanged);
            //cbUnit.SelectedIndex = (cbUnit.Items.Count > 0) ? 0 : -1;
            if (cbUnit.Items.Count > 0) {
                string m = (string)((DataRowView)cbMeasurement.SelectedItem)["NAME"];
                if (m == "Temperature")
                    cbUnit.SelectedIndex = 1;
                else
                    cbUnit.SelectedIndex = 0;
            }
            else
                cbUnit.SelectedIndex = -1;

        }

        private Unit[] GetUnitsAddMSRLimits(int MeasurementID )
        {
            List<Unit> Units = new List<Unit>();
            switch (MeasurementID)
            {
                case 1:
                    Units.Add(new Unit(13, "K", "Kelvin"));
                    Units.Add(new Unit(14, "°C", "Celsius"));
                    Units.Add(new Unit(17, "°F", "Fahrenheit"));
                    Units.Add(new Unit(18, "°Ra", "Rankine"));
                    break;
                case 3:
                    Units.Add(new Unit(37,"%RH", "Relative humidity"));
                    Units.Add(new Unit(60, "°C D.P", "Dew Point"));
                    break;
                case 5:
                    Units.Add(new Unit(38, "Pa a", "Pascal absolute"));
                    Units.Add(new Unit(39, "kPa a", "kilopascal absolute"));
                    Units.Add(new Unit(47, "mBar a", "millibar absolute"));
                    Units.Add(new Unit(48, "Bar a", "Bar absolute"));
                    break;
                default:
                    return null;




            }
           
            return Units.ToArray();
        }


        private void BoundLoggerData()
        {
            cbLoggers.SelectedIndexChanged -= new EventHandler(cbLogger_SelectedIndexChanged);
            foreach (KeyValuePair<int, string> logger in _loggers)
            {
                if (!cbLoggers.Items.Contains(logger))
                    cbLoggers.Items.Add(logger);
            }
            cbLoggers.DisplayMember = "Value";
            cbLoggers.ValueMember = "Key";
            cbLoggers.SelectedIndexChanged += new EventHandler(cbLogger_SelectedIndexChanged);

        }
        private void UpdateLoggerCombo()
        {
            cbLoggers.SelectedIndexChanged -= new EventHandler(cbLogger_SelectedIndexChanged);
            cbLoggers.SelectedIndex = -1;
            cbLoggers.SelectedIndexChanged += new EventHandler(cbLogger_SelectedIndexChanged);

            if (_loggerid > -1)
            {
                cbLoggers.SelectedIndex = -1; 
            }
            else
            {
                cbLoggers.SelectedIndex = -1;
                cbLoggers.Text = "";
                //UpdateLoggerData();
                //lockTheButtons();
            }
        }


        #region Properties
        public int ScanInterval
        {
            get { return _scanInterval; }
            set { _scanInterval = value; }
        }
        public SortedList<string, bool> Ports
        {
            get { return _ports; }
        }
        #endregion


    }
}
