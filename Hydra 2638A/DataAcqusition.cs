using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hydra_2638A.DecomposedData;

namespace Hydra_2638A
{


    /// <summary>
    /// Contains constans and methods that describes the data structure and returns blank tables
    /// </summary>
    /// <remarks></remarks>
    public static class DECLARATIONS
    {
        // Main constat declarations
        // DATA SET NAME
        public const string DATASET_NAME = "dl_data";

        // RAW DATA TABLE NAME
        public const string RAW_DATA_TABLE_NAME = "raw_data"; // (RDT)

        // RAW DATA TABLE STRUCTURE
        // |  ID  |   Logger   |   Channel     |    TimeElapsed    |   Value   |  Units   |  InRange  |
        // +------+------------+---------------+-------------------+-----------+----------+-----------+-
        // | Auto |    Int     |      Int      |      Int          |   Double  |   Int    |   Boolean(Byte) |

        public const string RDT_COL_ID_NAME = "ID";
        public const string RDT_COL_LGR_NAME = "Logger";
        public const string RDT_COL_CHNL_NAME = "Channel";
        public const string RDT_COL_TIME_NAME = "Time Elapsed";
        public const string RDT_COL_VAL_NAME = "Value";
        public const string RDT_COL_INRANGE_NAME = "In Range";
        // 'Added by Vladimir Geshovich at 11/05/2009
        // 'Public Const RDT_COL_ALLOWOOR_NAME As String = "OOR"

        // CONFIGURATION DATA TABLE NAME
        public const string CFG_DATA_TABLE_NAME = "cfg_data"; // (CDT)

        // CONFIGURATION DATA TABLE STRUCTURE
        // |  ID  |   Logger   |   Channel     | Note |   Sensor    |   Unit   |  Precision   | Active  | S Interval | H Interval |  DA Unit   | From Range | To Range   | Started at | 
        // +------+------------+---------------+--------------------+----------+--------------+---------+------------+------------+------------+------------+------------+------------+
        // | Auto |    Int     |      Int      | Str  |     Int     |   Int    |      Int     | Boolean |     Int    |   Int      |  Str       |   Double   |   Double   |    Date    |

        public const string CDT_COL_ID_NAME = "ID";
        public const string CDT_COL_LGR_NAME = "Logger";
        public const string CDT_COL_CHNL_NAME = "Channel";
        public const string CDT_COL_NOTE_NAME = "Note";
        public const string CDT_COL_SENSOR_NAME = "Sensor";
        public const string CDT_COL_UNIT_NAME = "Unit";
        public const string CDT_COL_MEASUREMENT_NAME = "Measurement";
        public const string CDT_COL_S_INTERVAL_NAME = "S Interval";
        public const string CDT_COL_H_INTERVAL_NAME = "H Interval";
        public const string CDT_COL_ACTIVE_NAME = "Active";
        public const string CDT_COL_DA_UNIT = "DA Unit";
        public const string CDT_COL_FROM_RANGE = "From Range";
        public const string CDT_COL_TO_RANGE = "To Range";
        public const string CDT_COL_FROM_WORKRANGE = "From WorkRange";
        public const string CDT_COL_TO_WORKRANGE = "To WorkRange";

        public const string CDT_COL_STARTED_DATE = "Started at";
        public const string CDT_COL_PRECISION_NAME = "Precision";
        public const string CDT_COL_COM_PORT_NAME = "COM Port";
        public const string CDT_COL_SERVER = "Server";
        // Public Const CDT_COL_COM_IS_CORRECTED As String = "IsCorrected"
        public const string CDT_COL_BP_IS_MEASURED = "BP_IsMeasured";
        public const string CDT_COL_BP_VALUE = "BP_Value";
        public const string CDT_COL_SPECIFIC_TYPE = "Specification Type";
        public const byte Status_OOR = 0;
        public const byte Status_INRANGE = 1;
        public const byte Status_ALLOWOOR = 2;

        public static DataTable GetNewDataTable()
        {
            DataTable MyTable = new DataTable(RAW_DATA_TABLE_NAME);

            // create columns

            // |  ID  |   Logger   |   Channel     |    TimeElapsed    |   Value   |  Units   | Percision |  InRange   |  OOR      | 
            // +------+------------+---------------+-------------------+-----------+----------+-----------+------------+-----------+
            // | Auto |    Int     |      Int      |      Int          |   Double  |   Int    |   Int     |  Boolean(Byte)   |  Boolean  |

            DataColumn MyColumn;

            MyColumn = new DataColumn(RDT_COL_ID_NAME, System.Type.GetType("System.Int32"));
            MyColumn.AutoIncrement = true;
            MyColumn.AutoIncrementSeed = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Entry ID";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(RDT_COL_LGR_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Data logger's ID";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(RDT_COL_CHNL_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Data logger channel's ID";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(RDT_COL_TIME_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Time elapsed";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(RDT_COL_VAL_NAME, System.Type.GetType("System.Double"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement value";
            MyTable.Columns.Add(MyColumn);

            // 'MyColumn = New DataColumn(RDT_COL_INRANGE_NAME, System.Type.GetType("System.Boolean"))
            MyColumn = new DataColumn(RDT_COL_INRANGE_NAME, System.Type.GetType("System.Byte"));
            // 'MyColumn.DefaultValue = False
            MyColumn.DefaultValue = 0;

            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "In range";
            MyTable.Columns.Add(MyColumn);

            // ' added by Vladimir Gershovich at 11/05/2009
            // 'MyColumn = New DataColumn(RDT_COL_ALLOWOOR_NAME, System.Type.GetType("System.Boolean"))
            // 'MyColumn.DefaultValue = False
            // 'MyColumn.AllowDBNull = False
            // 'MyColumn.Caption = "OOR"
            // 'MyTable.Columns.Add(MyColumn)

            return MyTable;
        }
        public static System.Data.DataTable GetNewConfigTable()
        {
            DataTable MyTable = new DataTable(DECLARATIONS.CFG_DATA_TABLE_NAME);

            // create columns

            // |  ID  |   Logger   | COM_PORT     |  Channel     | Note |   Sensor    |   Unit   |  Precision   | Active  | S Interval | H Interval |  DA Unit   | From Range | To Range   | Started at | Server Name | BP_IsMeasured | BP_Value  | From WorkRange | To WorkRange   |
            // +------+------------+--------------+--------------+------+-------------+----------+--------------+---------+------------+------------+------------+------------+------------+------------+-------------+---------------+-----------+----------------+----------------+
            // | Auto |    Int     |      Str     |     Int      | Str  |     Int     |   Int    |      Int     | Boolean |     Int    |   Int      |  Str       |   Double   |   Double   |    Date    |   String    | Boolean       |  Double   |   Double       |   Double       |


            DataColumn MyColumn;

            MyColumn = new DataColumn(CDT_COL_ID_NAME, System.Type.GetType("System.Int32"));
            MyColumn.AutoIncrement = true;
            MyColumn.AutoIncrementSeed = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Entry ID";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_LGR_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Data logger's ID";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_CHNL_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Data logger channel's ID";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_NOTE_NAME, System.Type.GetType("System.String"));
            MyColumn.Caption = "Note";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_SENSOR_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Instrument ID";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_UNIT_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement unit";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_MEASUREMENT_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement object";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_ACTIVE_NAME, System.Type.GetType("System.Boolean"));
            MyColumn.DefaultValue = false;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Status: is active";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_S_INTERVAL_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Software interval";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_H_INTERVAL_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Hardware interval";
            MyTable.Columns.Add(MyColumn);

            // 'MyColumn = New DataColumn(CDT_COL_DA_UNIT, System.Type.GetType("System.String"))
            MyColumn = new DataColumn(CDT_COL_DA_UNIT, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = string.Empty;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "DA device internal unit name";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_FROM_RANGE, System.Type.GetType("System.Double"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement range interval start";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_TO_RANGE, System.Type.GetType("System.Double"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement range interval stop";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_STARTED_DATE, System.Type.GetType("System.DateTime"));
            MyColumn.DefaultValue = null;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "DA start date & time";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_PRECISION_NAME, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement percision";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_COM_PORT_NAME, System.Type.GetType("System.String"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Communication Port";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_SERVER, System.Type.GetType("System.String"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Server Name";
            MyTable.Columns.Add(MyColumn);

            // MyColumn = New DataColumn(CDT_COL_COM_IS_CORRECTED, System.Type.GetType("System.Boolean"))
            // MyColumn.DefaultValue = 0
            // MyColumn.AllowDBNull = False
            // MyColumn.Caption = "Is Corrected"
            // MyTable.Columns.Add(MyColumn)

            MyColumn = new DataColumn(CDT_COL_BP_IS_MEASURED, System.Type.GetType("System.Boolean"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Is BP Measured";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_BP_VALUE, System.Type.GetType("System.Double"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Is BP Measured";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_FROM_WORKRANGE, System.Type.GetType("System.Double"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement range interval start";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_TO_WORKRANGE, System.Type.GetType("System.Double"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = false;
            MyColumn.Caption = "Measurement range interval stop";
            MyTable.Columns.Add(MyColumn);

            MyColumn = new DataColumn(CDT_COL_SPECIFIC_TYPE, System.Type.GetType("System.Int32"));
            MyColumn.DefaultValue = 0;
            MyColumn.AllowDBNull = true;
            MyColumn.Caption = "Specification Type";
            MyTable.Columns.Add(MyColumn);

            return MyTable;
        }
        /// <summary>
        ///         ''' Adds rows from SourceTable to TargetTable 
        ///         ''' </summary>
        ///         ''' <param name="TargetTable">The table to add rows in</param>
        ///         ''' <param name="SourceTable">The table to get rows from. ID column values are ignored</param>
        ///         ''' <remarks>Tables must have same schema</remarks>
        public static void AddDataToDataTable(ref DataTable TargetTable, DataTable SourceTable)
        {
            // 'If Not TargetTable.DataSet.GetXmlSchema = SourceTable.DataSet.GetXmlSchema Then Throw New Exception("Tables must be same!")
            foreach (DataRow r in SourceTable.Rows)
            {
                DataRow nr = TargetTable.NewRow();
                foreach (DataColumn c in SourceTable.Columns)
                {
                    if (c.ColumnName != RDT_COL_ID_NAME)
                        nr[c.ColumnName] = r[c];
                }
                TargetTable.Rows.Add(nr);
            }
        }
    }


    /// <summary>
    /// Represents set of DataQuants
    /// </summary>
    /// <remarks></remarks>
    public partial class DataHolder : IDisposable, ICloneable, IList<DataQuant>
    {
        // Inherits List(Of DataQuant)

        private const int INIT_CAPACITY = 100000;
        private const int INIT_CHUNK_CAPACITY = 1000;


        public partial struct FileHeader
        {
            public Guid DAGUID;
            public int RangeStart;
            public int RangeStop;
            public string Description;
        }
        public partial struct DataTimedSpan
        {
            public int TimeStart;
            public int TimeEnd;
        }
        public partial struct Logger_Channel
        {
            public int Logger;
            public int Channel;
        }
        private FileHeader _Header = new FileHeader();
        private List<DataQuant> _List;


        public partial struct DataHolderWithFileHeader
        {
            public DataHolderWithFileHeader(DataHolder.FileHeader header, DateTime startTime, List<DataQuant> Measures)
            {
                this.header = header;
                this.Measures = Measures;
                this.startTime = startTime;
            }
            public FileHeader header { get; }
            public DateTime startTime { get; }
            public List<DataQuant> Measures { get; }
        }

        #region Properties
        /// <summary>
        /// Returns or sets custom data description
        /// </summary>
        /// <value>Custom data description</value>
        /// <returns>Custom data description</returns>
        /// <remarks></remarks>
        public string Description
        {
            get
            {
                return _Header.Description;
            }
            set
            {
                _Header.Description = value;
            }
        }
        /// <summary>
        /// Returns lower limit of the data range that described by the collection
        /// </summary>
        /// <returns>The upper lower of the data range that described by the collection.
        /// (The index of the first element in whole data)</returns>
        /// <remarks></remarks>
        public int RangeStart
        {
            get
            {
                return _Header.RangeStart;
            }
        }
        /// <summary>
        /// Returns upper limit of the data range that described by the collection
        /// </summary>
        /// <returns>The upper lower of the data range that described by the collection.
        /// (The index of the last element in whole data)</returns>
        /// <remarks></remarks>
        public int RangeStop
        {
            get
            {
                return _Header.RangeStop;
            }
        }
        /// <summary>
        /// Returns data acquisition identificator.
        /// </summary>
        /// <returns> Returns data acquisition identificator.</returns>
        /// <remarks>When a new data acquisition is started unique identificator is assigned to it.</remarks>
        public Guid DAIndentificator
        {
            get
            {
                return _Header.DAGUID;
            }
        }
        /// <summary>
        /// Returns information header
        /// </summary>
        /// <returns>Information header</returns>
        /// <remarks></remarks>
        public FileHeader Header
        {
            get
            {
                return _Header;
            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes new instance of the DataHoder class random default guid
        /// </summary>
        /// <remarks></remarks>
        public DataHolder()
        {
            _List = new List<DataQuant>();
            _Header.DAGUID = Guid.NewGuid();
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="Data">Bytes array that holdes serialized DataQuant chunks</param>
        /// <remarks>Unlike the file data stream should not include the header</remarks>
        public DataHolder(byte[] Data) : this(new MemoryStream(Data))
        {
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class by loading the data from a file.
        /// </summary>
        /// <param name="Filename">Path to the file that holdes serialized DataQuant chunks.</param>
        /// <remarks></remarks>
        public DataHolder(string Filename)
        {
            _List = new List<DataQuant>();
            ReadStream(File.OpenRead(Filename), false);
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class by loading the data from a file.
        /// </summary>
        /// <param name="Filename">Path to the file that holdes serialized DataQuant chunks.</param>
        /// <param name="FromRange">Index to start read the data from.</param>
        /// <param name="ToRange">Index to stop read the data at.</param> 
        /// <remarks></remarks>
        public DataHolder(string Filename, int FromRange, int ToRange)
        {
            _List = new List<DataQuant>();
            ReadStream(File.OpenRead(Filename), FromRange, ToRange);
        }

        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="DataTable">DataTable that holdes DataQuant data</param>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <remarks></remarks>
        //public DataHolder(Guid DAGUID, System.Data.DataTable DataTable)
        //{
        //    _List = new List<DataQuant>();
        //    foreach (DataRow row in DataTable.Rows)
        //    {
        //        var Quant = new DataQuant();
        //        // Quant.ID = CInt(row[RDT_COL_ID_NAME))
        //        Quant.ElapsedTime = int.Parse(row[DECLARATIONS.RDT_COL_TIME_NAME].ToString());
        //        Quant.Logger = int.Parse(row[DECLARATIONS.RDT_COL_LGR_NAME].ToString());
        //        Quant.Channel = int.Parse(row[DECLARATIONS.RDT_COL_CHNL_NAME].ToString());
        //        Quant.Value = int.Parse(row[DECLARATIONS.RDT_COL_VAL_NAME].ToString());
        //        Quant.InRange = byte.Parse(row[DECLARATIONS.RDT_COL_INRANGE_NAME].ToString());
        //        Add(Quant);
        //    }
        //    _Header.DAGUID = DAGUID;
        //    DataTable.Dispose();
        //}
        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="DataStream">Binary stream that holdes serialized DataQuant chunks</param>
        /// <remarks></remarks>
        public DataHolder(Stream DataStream)
        {
            _List = new List<DataQuant>();
            ReadStream(DataStream, false);
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class with default capapcity
        /// </summary>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <remarks></remarks>
        public DataHolder(Guid DAGUID)
        {
            _List = new List<DataQuant>(INIT_CAPACITY);
            _Header.DAGUID = DAGUID;
        }

        /// <summary>
        /// Initializes new instance of the DataHoder class with given capapcity
        /// </summary>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <param name="Capacity">Initial capacity</param>
        /// <remarks></remarks>
        public DataHolder(Guid DAGUID, int Capacity)
        {
            _List = new List<DataQuant>(Capacity);
            _Header.DAGUID = DAGUID;
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class with given capapcity
        /// </summary>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <param name="StartIndex">Start global index for given data</param> 
        /// <param name="StopIndex">Stop global index for given data</param> 
        /// <remarks></remarks>
        public DataHolder(Guid DAGUID, int StartIndex, int StopIndex)
        {
            _List = new List<DataQuant>();
            _Header.RangeStart = StartIndex;
            _Header.RangeStop = StopIndex;
            _Header.DAGUID = DAGUID;
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="Data">Initial array of DataQuant objects to be added</param>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <remarks></remarks>
        public DataHolder(Guid DAGUID, DataQuant[] Data)
        {
            _List = new List<DataQuant>();
            AddRange(Data);
            _Header.DAGUID = DAGUID;
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="Data">Initial array of DataQuant objects to be added</param>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <param name="StartIndex">Start global index for given data</param> 
        /// <param name="StopIndex">Stop global index for given data</param> 
        /// <remarks></remarks>
        public DataHolder(Guid DAGUID, DataQuant[] Data, int StartIndex, int StopIndex)
        {
            _List = new List<DataQuant>();
            if (StopIndex - StartIndex + 1 != Data.Length)
                throw new InvalidIndexException();
            AddRange(Data);
            _Header.RangeStart = StartIndex;
            _Header.RangeStop = StopIndex;
            _Header.DAGUID = DAGUID;
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="Data">Initial Dictionary(Of Integer, DataQuant) contains objects to be added</param>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <remarks>Note that produced object indexes are flatted</remarks>
        public DataHolder(Guid DAGUID, Dictionary<int, DataQuant> Data)
        {
            _List = new List<DataQuant>();
            var MyDataQuants = new DataQuant[Data.Count];
            Data.Values.CopyTo(MyDataQuants, 0);
            _List.AddRange(MyDataQuants);
            _Header.RangeStart = 0;
            _Header.RangeStop = 0;
            _Header.DAGUID = DAGUID;
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="Data">Initial Dictionary(Of Integer, DataQuant) contains objects to be added</param>
        /// <param name="DAGUID">Data acquisition identificator</param>
        /// <param name="StartIndex">Start global index for given data</param> 
        /// <param name="StopIndex">Stop global index for given data</param> 
        /// <remarks>Note that produced object indexes are flatted</remarks>
        public DataHolder(Guid DAGUID, Dictionary<int, DataQuant> Data, int StartIndex, int StopIndex)
        {
            _List = new List<DataQuant>();
            if (StopIndex - StartIndex + 1 != Data.Count)
                throw new InvalidIndexException();
            var MyDataQuants = new DataQuant[Data.Count];
            Data.Values.CopyTo(MyDataQuants, 0);
            _List.AddRange(MyDataQuants);
            _Header.RangeStart = StartIndex;
            _Header.RangeStop = StopIndex;
            _Header.DAGUID = DAGUID;
        }
        /// <summary>
        /// Initializes new instance of the DataHoder class
        /// </summary>
        /// <param name="DataHolder">Initial data</param>
        /// <remarks>Index will be set according data size</remarks>
        public DataHolder(DataHolder DataHolder)
        {
            _List = new List<DataQuant>();
            _List.AddRange(DataHolder);
            _Header.RangeStart = DataHolder.RangeStart;
            _Header.RangeStop = DataHolder.RangeStop;
            _Header.DAGUID = DataHolder.DAIndentificator;
        }
        #endregion
        #region Methods
        #region Filtering

        /// <summary>
        /// Returns filtered DataHolder object
        /// </summary>
        /// <param name="FromIndex">The data index in the main data collection within filtering should be started</param>
        /// <returns>Filtered new instance of data holder object</returns>
        /// <remarks>When cross parameters filtering is performed, index filters are recommended to be applied first</remarks>
        public DataHolder FilterByIndex(int FromIndex)
        {
            // If _NeedResort Then Me.SortByIndex()
            int LocalIndex = FromIndex - _Header.RangeStart;
            if (LocalIndex > _List.Count | LocalIndex < 0)
                throw new InvalidIndexException();
            var MyDataArray = new DataQuant[(_List.Count - LocalIndex)];
            _List.CopyTo(LocalIndex, MyDataArray, 0, MyDataArray.Length);
            return new DataHolder(_Header.DAGUID, MyDataArray, FromIndex, _List.Count - 1);
        }
        /// <summary>
        /// Returns filtered DataHolder object
        /// </summary>
        /// <param name="FromIndex">The data index in the main data collection within filtering should be started</param>
        /// <param name="ToIndex">The data index in the main data collection within filtering should be finished (this point is excluded)</param>
        /// <returns>Filtered instance of data holder object</returns>
        /// <remarks>This filer sorts data by index. When cross parameters filtering is performed, index filters are recommended to be applied first</remarks>
        public DataHolder FilterByIndex(int FromIndex, int ToIndex)
        {
            // If _NeedResort Then Me.SortByIndex()
            int LocalFromIndex = FromIndex - _Header.RangeStart;
            int LocalToIndex = ToIndex - _Header.RangeStart;
            if (LocalFromIndex > _List.Count | LocalFromIndex < 0)
                throw new InvalidIndexException();
            if (LocalToIndex > _List.Count | LocalToIndex < 0)
                throw new InvalidIndexException();
            var MyDataArray = new DataQuant[LocalToIndex - LocalFromIndex + 1];
            _List.CopyTo(LocalFromIndex, MyDataArray, 0, MyDataArray.Length);
            return new DataHolder(_Header.DAGUID, MyDataArray, FromIndex, ToIndex);
        }
        /// <summary>
        /// Returns filtered DataHolder object
        /// </summary>
        /// <param name="Logger">The logger id within filtering should be finished</param>
        /// <returns>Sorted dictionary of data quants where the key is the data index</returns>
        /// <remarks>When cross parameters filtering is performed, index filters are recommended to be applied first</remarks>
        public Dictionary<int, DataQuant> FilterByTool(int Logger)
        {
            var FilteredData = new Dictionary<int, DataQuant>();

            // For Each data As DataQuant In Me
            // If data.Logger = Logger Then FilteredData.Add(_List.IndexOf(data), data)
            // Next
            for (int i = 0, loopTo = Count - 1; i <= loopTo; i++)
            {
                var data = this[i];
                if (data.Logger == Logger)
                {
                    FilteredData.Add(i, data);
                }
            }
            return FilteredData;
        }
        /// <summary>
        /// Returns filtered DataHolder object
        /// </summary>
        /// <param name="Logger">The logger id within filtering should be finished</param>
        /// <param name="Channel">The logger's channel id within filtering should be finished</param>
        /// <returns>Sorted dictionary of data quants where the key is the data index</returns>
        /// <remarks>When cross parameters filtering is performed, index filters are recommended to be applied first</remarks>
        public Dictionary<int, DataQuant> FilterByTool(int Logger, int Channel)
        {
            var FilteredData = new Dictionary<int, DataQuant>();
            // For Each data As DataQuant In Me
            // If data.Logger = Logger AndAlso data.Channel = Channel Then FilteredData.Add(_List.IndexOf(data), data)
            // Next
            for (int i = 0, loopTo = Count - 1; i <= loopTo; i++)
            {
                var data = this[i];
                if (data.Logger == Logger && data.Channel == Channel)
                {
                    FilteredData.Add(i, data);
                }
            }
            return FilteredData;
        }
        /// <summary>
        /// Returns filtered DataHolder object
        /// </summary>
        /// <param name="Loggers">The array that contains loggers id for each entry in Channels array.</param>
        /// <param name="Channels">The array that contains channels id for each entry in Logger array.</param>
        /// <returns>Filtered DataHolder object</returns>
        /// <remarks>Loggers and Channels arrays must have same length!</remarks>
        public DataHolder FilterByTool(int[] Loggers, int[] Channels)
        {
            var NewDataHolder = new DataHolder(_Header.DAGUID);
            var MyList = new List<string>();
            for (int i = 0, loopTo = Loggers.Length - 1; i <= loopTo; i++)
                MyList.Add(Loggers[i] + "." + Channels[i]);
            foreach (DataQuant data in this)
            {
                if (MyList.Contains(data.Logger + "." + data.Channel))
                    NewDataHolder.Add(data);
            }
            return NewDataHolder;
        }
        /// <summary>
        /// Returns filtered DataHolder object
        /// </summary>
        /// <param name="FromTime">The elapsed seconds within filtering should be started</param>
        /// <returns>Sorted dictionary of data quants where the key is the data index</returns>
        /// <remarks>When cross parameters filtering is performed, index filters are recommended to be applied first</remarks>
        public Dictionary<int, DataQuant> FilterByTime(int FromTime)
        {
            var FilteredData = new Dictionary<int, DataQuant>();
            // For Each data As DataQuant In Me
            // If data.ElapsedTime >= FromTime Then FilteredData.Add(_List.IndexOf(data), data)
            // Next
            for (int i = 0, loopTo = Count - 1; i <= loopTo; i++)
            {
                var data = this[i];
                if (data.ElapsedTime >= FromTime)
                {
                    FilteredData.Add(i, data);
                }
            }
            return FilteredData;
        }
        /// <summary>
        /// Returns filtered DataHolder object
        /// </summary>
        /// <param name="FromTime">The elapsed seconds within filtering should be started</param>
        /// <param name="ToTime">The elapsed seconds within filtering should be stopped (this point is excluded)</param>
        /// <returns>Sorted dictionary of data quants where the key is the data index</returns>
        /// <remarks>When cross parameters filtering is performed, index filters are recommended to be applied first</remarks>
        public Dictionary<int, DataQuant> FilterByTime(int FromTime, int ToTime)
        {
            var FilteredData = new Dictionary<int, DataQuant>();
            // For Each data As DataQuant In Me
            // If data.ElapsedTime >= FromTime AndAlso data.ElapsedTime < ToTime Then FilteredData.Add(_List.IndexOf(data), data)
            // Next
            for (int i = 0, loopTo = Count - 1; i <= loopTo; i++)
            {
                var data = this[i];
                if (data.ElapsedTime >= FromTime && data.ElapsedTime < ToTime)
                {
                    FilteredData.Add(i, data);
                }
            }
            return FilteredData;
        }

        #endregion
        #region Conversions

        /// <summary>
        /// Returns data as binary stream
        /// </summary>
        /// <returns>Binary stream that holds serialized data</returns>
        /// <remarks></remarks>
        public Stream ToDataStream()
        {
            // This function writes data to compressed binary stream
            Stream MyStream = new MemoryStream();
            WriteStream(ref MyStream, false);
            MyStream.Seek(0L, SeekOrigin.Begin);
            return MyStream;
        }
        /// <summary>
        /// Returns data as bytes array
        /// </summary>
        /// <returns>The bytes array that holdes serialized data</returns>
        /// <remarks></remarks>
        public byte[] ToBytesArray()
        {
            return ((MemoryStream)ToDataStream()).ToArray();
        }
        /// <summary>
        /// Returns data as DataTable
        /// </summary>
        /// <returns>DataTable that holds the data</returns>
        /// <remarks>Only data (QuantData array) is written to stream</remarks>
        //public DataTable ToDataTable()
        //{
        //    DataTable MyDataTable = DECLARATIONS.GetNewDataTable();
        //    int i = _Header.RangeStart;
        //    foreach (DataQuant quant in _List)
        //    {
        //        i += 1;
        //        DataRow MyRow = MyDataTable.NewRow();
        //        MyRow[DECLARATIONS.RDT_COL_ID_NAME] = i;
        //        MyRow[DECLARATIONS.RDT_COL_INRANGE_NAME] = quant.InRange;

        //        MyRow[DECLARATIONS.RDT_COL_LGR_NAME] = quant.Logger;
        //        MyRow[DECLARATIONS.RDT_COL_VAL_NAME] = quant.Value;
        //        MyRow[DECLARATIONS.RDT_COL_CHNL_NAME] = quant.Channel;
        //        MyRow[DECLARATIONS.RDT_COL_TIME_NAME] = quant.ElapsedTime;
        //        MyDataTable.Rows.Add(MyRow);
        //    }
        //    return MyDataTable;
        //}

        #endregion
        #region Files operations


        /// <summary>Writes binary file that holds the data.</summary>
        /// <param name="Filename">Path to file to be created.</param>
        /// <remarks>Note: existing file will be replaced.</remarks>
        public void ToFile(string Filename)
        {
            // This function writes data to binary file.
            // Existing file will be replaced
            string MyLock = "FileWritingOperation";
            lock (MyLock)
            {
                using (var MyFileStream = new FileStream(Filename, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    Stream argStream = MyFileStream;
                    WriteStream(ref argStream, false);
                }
            }
        }
        /// <summary>
        /// Reads information header from the file.
        /// </summary>
        /// <param name="Filename">Path to existing file.</param>
        /// <returns>Information header from the file.</returns>
        /// <remarks></remarks>
        public static FileHeader GetFileHeader(string Filename)
        {
            using (var MyReader = new BinaryReader(File.OpenRead(Filename)))
            {
                return ReadHeader(MyReader);
            }
        }
        /// <summary>Appends the data to existing binary data file.</summary>
        /// <param name="Filename">Path to existing file</param>
        /// <remarks>RangeStop (in file) index will be overwrited. If file doesn't exists new file will be created.</remarks>
        /// <exception cref="InconsistentIndexException ">will be thrown if added data index is inconsistent with previously written data.</exception>
        public void AppendToFile(string Filename)
        {
            // This function appends data to existing binary file.
            // If file doesn't exist yet lets call ToFile method instead
            if (!File.Exists(Filename))
            {
                ToFile(Filename);
                return;
            }
            string MyLock = "FileAppendOperation";
            lock (MyLock)
            {
                FileHeader SavedHeader;
                FileHeader CombinedHeader;
                using (var MyReader = new BinaryReader(File.OpenRead(Filename)))
                {
                    SavedHeader = ReadHeader(MyReader);
                    /* TODO ERROR: Skipped IfDirectiveTrivia
                    #If DEBUG Then
                    *//* TODO ERROR: Skipped DisabledTextTrivia
                                        Debug.WriteLine("Saved Header: Start=" & SavedHeader.RangeStart & ", Stop=" & SavedHeader.RangeStop)
                                        Debug.WriteLine("Current Header: Start=" & _Header.RangeStart & ", Stop=" & _Header.RangeStop)
                    *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                    #End If
                    */
                    if (SavedHeader.RangeStop + 1 != _Header.RangeStart)
                        throw new InconsistentIndexException(SavedHeader.RangeStop + 1, _Header.RangeStart);
                    if (!SavedHeader.DAGUID.Equals(_Header.DAGUID))
                        throw new InconsistentDAGUIDException();
                    CombinedHeader = SavedHeader;
                    CombinedHeader.RangeStart = SavedHeader.RangeStart;
                    CombinedHeader.RangeStop = _Header.RangeStop;
                }
                using (var MyWriter = new BinaryWriter(File.OpenWrite(Filename)))
                {
                    WriteHeader(MyWriter, CombinedHeader);
                    MyWriter.Seek(0, SeekOrigin.End);
                    var argStream = MyWriter.BaseStream;
                    WriteStream(ref argStream, true);
                }
            }
        }


        public void ToJson(string Filename, DateTime startTime)
        {
            DataHolderWithFileHeader FileData = new DataHolderWithFileHeader(this._Header, startTime, this._List);


            FileInfo fi = new FileInfo(Filename);
            if (!fi.Directory.Exists)
            {
                System.IO.Directory.CreateDirectory(fi.DirectoryName);
            }

            using (StreamWriter file = File.CreateText(Filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, FileData);
            }
        }

        #endregion

        /// <summary>
        /// Sorts data by index
        /// </summary>
        /// <returns>Dictionary(Of DataQuant, Integer) sorted by given comparer</returns>
        /// <remarks></remarks>
        public new Dictionary<int, DataQuant> Sort(IComparer<DataQuant> Comparer)
        {
            var MyDictionary = new Dictionary<int, DataQuant>();
            var MyData = new DataHolder(this);
            MyData.Sort(Comparer);
            foreach (DataQuant data in MyData)
                MyDictionary.Add(_List.IndexOf(data) + _Header.RangeStart, data);
            return MyDictionary;
        }

        /// <summary>
        /// Adds new data chunk to data collection
        /// </summary>
        /// <param name="MyData">Collection of DataQuants</param>
        /// <exception cref=" InconsistentDAGUIDException "> will be thrown if provided data is DataHolder type and has different DAGUID</exception>
        /// <remarks>Each data index will be set to last collection index. To merge 2 different data holder objects please use Merge method instead.</remarks>
        public new void AddRange(IEnumerable<DataQuant> MyData)
        {
            if (MyData is DataHolder)
            {
                if (!((DataHolder)MyData).DAIndentificator.Equals(_Header.DAGUID))
                    throw new InconsistentDAGUIDException();
            }
            _List.AddRange(MyData);
            _Header.RangeStop = _List.Count - 1;
        }

        /// <summary>
        /// Adds new data chunk to data collection
        /// </summary>
        /// <param name="MyData">Collection of DataQuants</param>
        /// <remarks>Each data index will be set to last collection index. This method doesn't check provided data DAGUID.</remarks>
        public new void Merge(IEnumerable<DataQuant> MyData)
        {
            _List.AddRange(MyData);
            _Header.RangeStop = _Header.RangeStart + _List.Count - 1;
        }

        /// <summary>
        /// Creates and returns clones of the collection
        /// </summary>
        /// <returns>Returns clone of the collection</returns>
        /// <remarks></remarks>
        public object Clone()
        {
            return new DataHolder(this);
        }

        /// <summary>
        /// Returns TRUE when given index is covered by the collection, otherwise returns FALSE
        /// </summary>
        /// <returns>TRUE when given index is covered by the collection; otherwise FALSE</returns>
        /// <remarks></remarks>
        public bool InRange(int index)
        {
            return index >= _Header.RangeStart & index <= _Header.RangeStop;
        }
        #region Data decomposition to simple types arrays
        /// <summary>
        /// Returns raw data values array
        /// </summary>
        /// <returns>Values array (doubles)</returns>
        public double[] GetValues()
        {
            var MyValues = new List<double>(Count);
            foreach (DataQuant DataQuant in _List)
                MyValues.Add((double)DataQuant.Value);
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns values array filtered by Channel and Logger 
        /// </summary>
        /// <param name="Channel">ID of the channel to filter by</param>
        /// <param name="Logger">ID of the logger to filter by</param>
        /// <returns>Returns values array of doubles filtered by Channel and Logger  </returns>
        public double[] GetValues(int Logger, int Channel)
        {
            var MyValues = new List<double>(Count);
            foreach (DataQuant DataQuant in this)
            {
                if (DataQuant.Logger == Logger & DataQuant.Channel == Channel)
                    MyValues.Add((double)DataQuant.Value);
            }
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns raw data values array
        /// </summary>
        /// <returns>Values array (decimals)</returns>
        public decimal[] GetValuesDEC()
        {
            var MyValues = new List<decimal>(Count);
            foreach (DataQuant DataQuant in this)
                MyValues.Add(DataQuant.Value);
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns values array filtered by Channel and Logger 
        /// </summary>
        /// <param name="Channel">ID of the channel to filter by</param>
        /// <param name="Logger">ID of the logger to filter by</param>
        /// <returns>Returns values array of decimals filtered by Channel and Logger  </returns>
        public decimal[] GetValuesDEC(int Logger, int Channel)
        {
            var MyValues = new List<decimal>(Count);
            foreach (DataQuant DataQuant in this)
            {
                if (DataQuant.Logger == Logger & DataQuant.Channel == Channel)
                    MyValues.Add(DataQuant.Value);
            }
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns array of raw data elapsed seconds 
        /// </summary>
        /// <returns>Elapsed seconds array</returns>
        public double[] GetTimes()
        {
            var MyTimes = new List<double>(Count);
            foreach (DataQuant DataQuant in this)
                MyTimes.Add(DataQuant.ElapsedTime);
            return MyTimes.ToArray();
        }
        /// <summary>
        /// Returns elapsed seconds array for given logger and channel
        /// </summary>
        /// <param name="Channel">ID of the channel to filter by</param>
        /// <param name="Logger">ID of the logger to filter by</param>
        /// <returns>Elapsed seconds array</returns>
        public double[] GetTimes(int Logger, int Channel)
        {
            var MyTimes = new List<double>(Count);
            foreach (DataQuant DataQuant in this)
            {
                if (DataQuant.Logger == Logger & DataQuant.Channel == Channel)
                    MyTimes.Add(DataQuant.ElapsedTime);
            }
            return MyTimes.ToArray();
        }
        /// <summary>
        /// Returns raw channels array
        /// </summary>
        /// <returns>Channels array</returns>
        public int[] GetChanels()
        {
            var MyValues = new List<int>(Count);
            foreach (DataQuant DataQuant in _List)
                MyValues.Add(DataQuant.Channel);
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns channels array
        /// </summary>
        /// <returns>Channels array</returns>
        public int[] GetChanelsList()
        {
            var MyValues = new List<int>(Count);
            foreach (DataQuant DataQuant in _List)
            {
                if (!MyValues.Contains(DataQuant.Channel))
                {
                    MyValues.Add(DataQuant.Channel);
                }
            }
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns raw data loggers array
        /// </summary>
        /// <returns>Loggers array</returns>
        public int[] GetLoggers()
        {
            var MyValues = new List<int>(Count);
            foreach (DataQuant DataQuant in _List)
                MyValues.Add(DataQuant.Logger);
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns data loggers array
        /// </summary>
        /// <returns>Loggers array</returns>
        public int[] GetLoggersList()
        {
            var MyValues = new List<int>(Count);
            foreach (DataQuant DataQuant in _List)
            {
                if (!MyValues.Contains(DataQuant.Logger))
                {
                    MyValues.Add(DataQuant.Logger);
                }
            }
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns raw data loggers array  filtered by Channel and Logger 
        /// </summary>
        /// <param name="Channel">ID of the channel to filter by</param>
        /// <param name="Logger">ID of the logger to filter by</param>
        /// <returns>Loggers array</returns>
        public int[] GetLoggers(int Logger, int Channel)
        {
            var MyValues = new List<int>(Count);
            foreach (DataQuant DataQuant in _List)
                MyValues.Add(DataQuant.Logger);
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns raw data InRange status array
        /// </summary>
        /// <returns>InRange status data array</returns>
        public byte[] GetInRangeStatus()
        {
            // 'Public Function GetInRangeStatus() As Boolean()
            // 'Dim MyValues As New List(Of Boolean)(Me.Count)
            var MyValues = new List<byte>(Count);

            foreach (DataQuant DataQuant in _List)
                MyValues.Add(DataQuant.InRange);
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns raw data InRange status array filtered by Channel and Logger 
        /// </summary>
        /// <param name="Channel">ID of the channel to filter by</param>
        /// <param name="Logger">ID of the logger to filter by</param>
        /// <returns>InRange status data array</returns>
        public byte[] GetInRangeStatus(int Logger, int Channel)
        {
            // 'Public Function GetInRangeStatus(ByVal Logger As Integer, ByVal Channel As Integer) As Boolean()

            // 'Dim MyValues As New List(Of Boolean)(Me.Count)
            var MyValues = new List<byte>(Count);

            foreach (DataQuant DataQuant in _List)
            {
                if (DataQuant.Logger == Logger & DataQuant.Channel == Channel)
                    MyValues.Add(DataQuant.InRange);
            }
            return MyValues.ToArray();
        }
        /// <summary>
        /// Returns decomposed to simple arrays data
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public DecomposedData Decompose()
        {
            return new DecomposedData(this);
        }
        /// <summary>
        /// Returns filtered by logger, channel and decomposed to simple arrays data.
        /// </summary>
        /// <param name="Logger">Logger to filter by.</param>
        /// <param name="Channel">Channel to filter by.</param>
        /// <returns>Filtered by logger, channel and decomposed to simple arrays data.</returns>
        /// <remarks></remarks>
        public DecomposedData Decompose(int Logger, int Channel)
        {
            return new DecomposedData(this, Logger, Channel);
        }
        /// <summary>
        /// Returns filtered by logger, channel,lower range,upper range and decomposed to simple arrays data.
        /// </summary>
        /// <param name="Logger">Logger to filter by.</param>
        /// <param name="Channel">Channel to filter by.</param>
        /// <param name="RangeStart">Lower range (included) to filter by.</param>
        /// <param name="RangeStop">Upper range (excluded)to filter by.</param>
        /// <returns>Filtered by logger, channel,lower range,upper range and decomposed to simple arrays data.</returns>
        /// <remarks></remarks>
        public DecomposedData Decompose(int Logger, int Channel, int RangeStart, int RangeStop)
        {
            return new DecomposedData(this, Logger, Channel);
        }
        #endregion

        #region IList(Of DataQuant) implementation
        /// <summary>
        /// Adds new data point to data collection
        /// </summary>
        /// <param name="DataQuant">DataQuant to be added</param>
        /// <remarks>The data index will be set to last collection index</remarks>
        public new void Add(DataQuant DataQuant)
        {
            _List.Add(DataQuant);
            _Header.RangeStop = _List.Count - 1;
        }
        /// <summary>
        /// Determines whether an element is in the Collections.
        /// </summary>
        /// <param name="item"> The object to locate in the Collections. The value can be null for reference types.</param>
        /// <returns>true if item is found in the Collections; otherwise, false.</returns>
        /// <remarks></remarks>
        public bool Contains(DataQuant item)
        {
            return _List.Contains(item);
        }
        /// <summary>
        /// Copies the entire Collections to a compatible one-dimensional array of DataQuants, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array of DataQuants that is the destination of the elements copied from the Collections.The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <remarks></remarks>
        public void CopyTo(DataQuant[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Gets the number of elements actually contained in the Collections.
        /// </summary>
        /// <returns>The number of elements actually contained in the Collections.</returns>
        /// <remarks></remarks>
        public int Count
        {
            get
            {
                return _List.Count;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the System.Collections.Generic.ICollection(Of T) is read-only. For this class always returns false.
        /// </summary>
        /// <returns>Alway false</returns>
        /// <remarks></remarks>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Searches for the specified object and returns the index of the first occurrence within the entire Collections.
        /// </summary>
        /// <param name="item">The item to locate in the Collection. The value can be null for reference types.</param>
        /// <returns>The index of the first occurrence of item within the entire Collections, if found; otherwise, –1.</returns>
        /// <remarks></remarks>
        public int IndexOf(DataQuant item)
        {
            return _List.IndexOf(item) + _Header.RangeStart;
        }
        /// <summary>
        /// Returns specific item according given index
        /// </summary>
        /// <param name="index">Index of the data in the main data collection</param>
        /// <returns>Data specific to provided index</returns>
        /// <remarks>Please note, the index should be given as index in main, the total data collection</remarks>
        public DataQuant this[int index]
        {
            get
            {
                int LocalIndex = index - _Header.RangeStart;
                if (LocalIndex > _List.Count | LocalIndex < 0)
                    throw new InvalidIndexException();
                return _List[LocalIndex];
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the System.Collections.Generic.List(Of DataQuant).
        /// </summary>
        /// <returns>A System.Collections.Generic.List(Of T).Enumerator for the System.Collections.Generic.List(Of DataQuant)</returns>
        /// <remarks></remarks>
        public IEnumerator GetEnumerator()
        {
            return _List.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through the System.Collections.Generic.List(Of DataQuant).
        /// </summary>
        /// <returns>A System.Collections.Generic.List(Of T).Enumerator for the System.Collections.Generic.List(Of DataQuant)</returns>
        /// <remarks></remarks>
        public IEnumerator<DataQuant> GetEnumerator1()
        {
            return _List.GetEnumerator();
        }

        IEnumerator<DataQuant> IEnumerable<DataQuant>.GetEnumerator() => GetEnumerator1();
        #region Not implemented
        /// <summary>
        /// This method is not implemented to prevent possible data loss or corruption
        /// </summary>
        /// <exception cref="NotImplementedException"> will be thrown</exception>
        /// <remarks>To prevent NotImplementedException exception apply ObsoleteAttribute with error parameter false</remarks>
        [Obsolete("This method is not implemented")]
        public void RemoveAt(int index)
        {
            foreach (Attribute attribute in GetType().GetCustomAttributes(false))
            {
                if (attribute is ObsoleteAttribute)
                {
                    if (((ObsoleteAttribute)attribute).IsError)
                        throw new NotImplementedException();
                }
                else
                {
                    // do nothing
                }
            }
        }
        /// <summary>
        /// This method is not implemented to prevent possible data loss or corruption
        /// </summary>
        /// <exception cref="NotImplementedException"> will be thrown</exception>
        /// <remarks>To prevent NotImplementedException exception apply ObsoleteAttribute with error parameter false</remarks>
        [Obsolete("This method is not implemented")]
        public void Clear()
        {
            foreach (Attribute attribute in GetType().GetCustomAttributes(false))
            {
                if (attribute is ObsoleteAttribute)
                {
                    if (((ObsoleteAttribute)attribute).IsError)
                        throw new NotImplementedException();
                }
                else
                {
                    // do nothing
                }
            }
        }
        /// <summary>
        /// This method is not implemented to prevent possible data loss or corruption
        /// </summary>
        /// <exception cref="NotImplementedException"> will be thrown</exception>
        /// <remarks>To prevent NotImplementedException exception apply ObsoleteAttribute with error parameter false</remarks>
        [Obsolete("This method is not implemented")]
        public bool Remove(DataQuant item)
        {
            foreach (Attribute attribute in GetType().GetCustomAttributes(false))
            {
                if (attribute is ObsoleteAttribute)
                {
                    if (((ObsoleteAttribute)attribute).IsError)
                        throw new NotImplementedException();
                }
                else
                {
                    return false;
                }
            }

            return default;
        }
        /// <summary>
        /// This method is not implemented to prevent possible data loss or corruption
        /// </summary>
        /// <exception cref="NotImplementedException"> will be thrown</exception>
        /// <remarks>To prevent NotImplementedException exception apply ObsoleteAttribute with error parameter false</remarks>
        [Obsolete("This method is not implemented")]
        public void Insert(int index, DataQuant item)
        {
            foreach (Attribute attribute in GetType().GetCustomAttributes(false))
            {
                if (attribute is ObsoleteAttribute)
                {
                    if (((ObsoleteAttribute)attribute).IsError)
                        throw new NotImplementedException();
                }
                else
                {
                    // do nothing
                }
            }
        }
        #endregion
        #endregion

        #endregion
        #region Private Procedures
        private void ReadStream(Stream Stream, bool SkipHeader)
        {
            using (var MyReader = new BinaryReader(Stream))
            {
                FileHeader MyHeader = default;
                if (!SkipHeader)
                    MyHeader = ReadHeader(MyReader);
                while (MyReader.BaseStream.Position < MyReader.BaseStream.Length)
                    Add(ReadQuant(MyReader));
                _List.TrimExcess();
                if (!SkipHeader)
                    _Header = MyHeader;
            }
        }
        private void ReadStream(Stream Stream, int FromIndex, int ToIndex)
        {
            using (var MyReader = new BinaryReader(Stream))
            {
                _Header = ReadHeader(MyReader);
                int i = _Header.RangeStart;
                if (FromIndex < _Header.RangeStart | ToIndex > _Header.RangeStop)
                    throw new InvalidIndexException();
                while (MyReader.PeekChar() != -1)
                {
                    var MyQuant = ReadQuant(MyReader);
                    if (i >= FromIndex & i <= ToIndex)
                        Add(MyQuant);
                }
                _List.TrimExcess();
            }
        }
        private void WriteStream(ref Stream Stream, bool SkipHeader)
        {
            var MyWriter = new BinaryWriter(Stream);
            if (!SkipHeader)
                WriteHeader(MyWriter, _Header);
            foreach (DataQuant quant in this)
                WriteQuant(MyWriter, quant);
            MyWriter.Flush();
        }
        private DataQuant ReadQuant(BinaryReader MyReader)
        {
            var Quant = new DataQuant();
            try
            {
                Quant.ElapsedTime = MyReader.ReadInt32();
                Quant.Logger = MyReader.ReadInt32();
                Quant.Channel = MyReader.ReadInt32();
                Quant.Value = MyReader.ReadDecimal();
                // 'Quant.InRange = MyReader.ReadBoolean
                Quant.InRange = MyReader.ReadByte();
            }

            // ' added by Vladimir Gershovich at 11/05/2009
            // 'Quant.AllowOOR = MyReader.ReadBoolean
            catch (Exception ex)
            {
                /* TODO ERROR: Skipped IfDirectiveTrivia
                #If DEBUG Then
                *//* TODO ERROR: Skipped DisabledTextTrivia
                                Debug.WriteLine(ex.Message)
                                Debug.Assert(False)
                *//* TODO ERROR: Skipped ElseDirectiveTrivia
                #Else
                */
                throw ex;
                /* TODO ERROR: Skipped EndIfDirectiveTrivia
                #End If
                */
            }
            return Quant;
        }
        private void WriteQuant(BinaryWriter Writer, DataQuant Quant)
        {
            Writer.Write(Quant.ElapsedTime);
            Writer.Write(Quant.Logger);
            Writer.Write(Quant.Channel);
            Writer.Write(Quant.Value);
            Writer.Write(Quant.InRange);
        }
        private static FileHeader ReadHeader(BinaryReader MyReader)
        {
            var MyHeader = new FileHeader();
            MyHeader.DAGUID = new Guid(MyReader.ReadBytes(16));
            MyHeader.RangeStart = MyReader.ReadInt32();
            MyHeader.RangeStop = MyReader.ReadInt32();
            int DescLen = MyReader.ReadInt32();
            if (DescLen > 0)
                MyHeader.Description = MyReader.ReadChars(DescLen).ToString();
            return MyHeader;
        }
        private void WriteHeader(BinaryWriter Writer, FileHeader Header)
        {
            Writer.Write(Header.DAGUID.ToByteArray());
            Writer.Write(Header.RangeStart);
            Writer.Write(Header.RangeStop);
            if (Header.Description != null)
            {
                Writer.Write(Header.Description.Length); // Description length
                Writer.Write(Header.Description.ToCharArray());
            }
            else
            {
                Writer.Write(0);

            } // Zero length
        }

        #endregion
        #region IDisposable Support 
        private bool disposedValue = false;        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: free managed resources when explicitly called
                    _List.Clear();
                }
                // TODO: free shared unmanaged resources
            }
            disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Sub classes - Comparison
        // Public Class ByIndexComparer
        // Implements System.Collections.Generic.IComparer(Of DataQuant)
        // Public Function Compare(ByVal x As DataQuant, ByVal y As DataQuant) As Integer Implements System.Collections.Generic.IComparer(Of DataQuant).Compare
        // 'Less than zero: x is less than y.
        // 'Zero: x equals y.
        // 'Greater than zero: x is greater than y.
        // If x.ID < y.ID Then Return -1
        // If x.ID = y.ID Then Return 0
        // If x.ID > y.ID Then Return 1
        // End Function
        // End Class
        public partial class ByTimeComparer : IComparer<DataQuant>
        {
            public int Compare(DataQuant x, DataQuant y)
            {
                // Less than zero: x is less than y.
                // Zero: x equals y.
                // Greater than zero: x is greater than y.
                if (x.ElapsedTime < y.ElapsedTime)
                    return -1;
                if (x.ElapsedTime == y.ElapsedTime)
                    return 0;
                if (x.ElapsedTime > y.ElapsedTime)
                    return 1;
                return default;
            }
        }
        public partial class ByLoggerComparer : IComparer<DataQuant>
        {
            public int Compare(DataQuant x, DataQuant y)
            {
                // Less than zero: x is less than y.
                // Zero: x equals y.
                // Greater than zero: x is greater than y.
                if (x.Logger < y.Logger)
                    return -1;
                if (x.Logger == y.Logger)
                    return 0;
                if (x.Logger > y.Logger)
                    return 1;
                return default;
            }
        }
        public partial class ByChannelComparer : IComparer<DataQuant>
        {
            public int Compare(DataQuant x, DataQuant y)
            {
                // Less than zero: x is less than y.
                // Zero: x equals y.
                // Greater than zero: x is greater than y.
                if (x.Channel < y.Channel)
                    return -1;
                if (x.Channel == y.Channel)
                    return 0;
                if (x.Channel > y.Channel)
                    return 1;
                return default;
            }
        }
        public partial class ByInRangeComparer : IComparer<DataQuant>
        {
            public int Compare(DataQuant x, DataQuant y)
            {
                // Less than zero: x is less than y.
                // Zero: x equals y.
                // Greater than zero: x is greater than y.
                if (x.InRange != y.InRange)
                    return -1;
                if (x.Channel == y.Channel)
                    return 0;
                return default;
            }
        }

        public partial class ByValueComparer : IComparer<DataQuant>
        {
            public int Compare(DataQuant x, DataQuant y)
            {
                // Less than zero: x is less than y.
                // Zero: x equals y.
                // Greater than zero: x is greater than y.
                if (x.Value < y.Value)
                    return -1;
                if (x.Value == y.Value)
                    return 0;
                if (x.Value > y.Value)
                    return 1;
                return default;
            }
        }
        #endregion

    }

    // ---------------------------------------------------------------------------------------


    /// <summary>
    /// Represents simples data unit
    /// </summary>
    /// <remarks></remarks>
    public struct DataQuant
    {
        /// <summary>
        ///         ''' Id of the logger in kyulan DB
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public int Logger;
        /// <summary>
        ///         ''' Channel ID
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public int Channel;
        /// <summary>
        ///         ''' Elapsed time in seconds
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public int ElapsedTime;
        /// <summary>
        ///         ''' The measured value in decimal format
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public decimal Value;
        /// <summary>
        ///         ''' In range flag
        ///         ''' </summary>
        ///         ''' <remarks></remarks>

        public byte InRange;
    }


    // ---------------------------------------------------------------------------------------

    /// <summary>
    /// Decomposes DataHolder to simple arrays
    /// </summary>
    /// <remarks></remarks>
    public partial class DecomposedData
    {
        private List<double> _Values = new List<double>();
        private List<int> _Times = new List<int>();
        private List<int> _Channels = new List<int>();
        private List<int> _Loggers = new List<int>();
        // 'Private _InRangeStatus As New List(Of Boolean)
        private List<byte> _InRangeStatus = new List<byte>();

        // ' added by Vladimir Gershocich at 11/05/2009
        private List<bool> _AllowOORStatus = new List<bool>();

        private int _LoggerFilter = -1;
        private int _ChannelFilter = -1;
        private int _RangeStartFilter = -1;
        private int _RangeStopFilter = -1;
        public DecomposedData(DataHolder DataHolder)
        {
            Decompose(DataHolder);
        }
        public DecomposedData(DataHolder DataHolder, int Logger, int Channel)
        {
            _LoggerFilter = Logger;
            _ChannelFilter = Channel;
            Decompose(DataHolder);
        }
        public DecomposedData(DataHolder DataHolder, int Logger, int Channel, int RangeStart, int RangeStop)
        {
            _LoggerFilter = Logger;
            _ChannelFilter = Channel;
            _RangeStartFilter = RangeStart;
            _RangeStopFilter = RangeStop;
            Decompose(DataHolder);
        }
        public double[] Values
        {
            get
            {
                return _Values.ToArray();
            }
        }
        public int[] Times
        {
            get
            {
                return _Times.ToArray();
            }
        }
        public int[] Loggers
        {
            get
            {
                return _Loggers.ToArray();
            }
        }
        public int[] Channles
        {
            get
            {
                return _Channels.ToArray();
            }
        }
        public byte[] InRangeStatus
        {
            // 'Public ReadOnly Property InRangeStatus() As Boolean()

            get
            {
                return _InRangeStatus.ToArray();
            }
        }

        private void Decompose(DataHolder DataHolder)
        {
            foreach (DataQuant DataQuant in DataHolder)
            {
                bool MyFlag = true;
                MyFlag = _LoggerFilter < 0 || DataQuant.Logger == _LoggerFilter;
                MyFlag = MyFlag & (_ChannelFilter < 0 || DataQuant.Channel == _ChannelFilter);
                MyFlag = MyFlag & (_RangeStartFilter < 0 || _RangeStopFilter < 0 || _RangeStartFilter <= DataHolder.IndexOf(DataQuant) && _RangeStopFilter > DataHolder.IndexOf(DataQuant));
                if (MyFlag)
                {
                    _Values.Add((double)DataQuant.Value);
                    _Times.Add(DataQuant.ElapsedTime);
                    _Channels.Add(DataQuant.Channel);
                    _Loggers.Add(DataQuant.Logger);
                    _InRangeStatus.Add(DataQuant.InRange);
                }
            }

        }


        #region Custom exception declarations

        /// <summary>
        /// Represents an error that was caused by an operation request during incorrect operation state
        /// </summary>
        /// <remarks></remarks>
        public partial class InvalidOperationStateException : Exception
        {
            public override string Message
            {
                get
                {
                    return "Required operation cannot be performed during current device operation state";
                }
            }
        }
        // ---------------------------------------------------------------------------------------

        /// <summary>
        /// Represents an error that occures when provided index is out of actual data range
        /// </summary>
        /// <remarks></remarks>
        public partial class InvalidIndexException : Exception
        {
            public override string Message
            {
                get
                {
                    return "Unable to perform the requested operation because provided dat index is out of actual data range bounds.";
                }
            }
        }
        // ---------------------------------------------------------------------------------------

        /// <summary>
        /// Represents an error that occures on append or merge attempt when provided index doesn't continue the existsing index
        /// </summary>
        /// <remarks></remarks>
        public partial class InconsistentIndexException : Exception
        {
            private int _ExpectedIndex = -1;
            private int _GivenIndex = -1;
            public InconsistentIndexException(int ExpectedIndex, int GivenIndex)
            {
                _ExpectedIndex = ExpectedIndex;
                _GivenIndex = GivenIndex;
            }
            public int ExpectedIndex
            {
                get
                {
                    return _ExpectedIndex;
                }
            }
            public int GivenIndex
            {
                get
                {
                    return _GivenIndex;
                }
            }
            public override string Message
            {
                get
                {
                    return "Unable to perform the requested operation because provided data index is inconsistent to existing data.";
                }
            }
        }
        // ---------------------------------------------------------------------------------------

        /// <summary>
        /// Represents an error that occures on append or merge attempt when data acquisition identifiers (DAGUIDs) of data collections aren't equal.
        /// </summary>
        /// <remarks></remarks>
        public partial class InconsistentDAGUIDException : Exception
        {
            public override string Message
            {
                get
                {
                    return "Unable to perform the requested operation because provided DA GUID is inconsistent to existing data.";
                }
            }
        }
        // ---------------------------------------------------------------------------------------
        #endregion



    }



    public class LoggerDefinition
    {
        protected SortedList<int, ChannelDefinition> _channels;
        protected int MaxChannels = 61;//41;
        public LoggerDefinition()
        {
            _channels = new SortedList<int, ChannelDefinition>();
            for (int i = 0; i < MaxChannels; i++)
            {
                _channels.Add(i, new ChannelDefinition());
            }
        }
        public ChannelDefinition GetChannel(int i)
        {
            return _channels[i];
        }
        public void SetChannel(int num, ChannelDefinition Channel)
        {
            _channels[num] = Channel;
        }
        public SortedList<int, ChannelDefinition> Channels
        {
            get { return _channels; }
        }
    }


    public delegate void LoggerScanHandler(object sender, ScanResaltArg arg);
    public class ScanResaltArg : EventArgs
    {
        private List<DataQuant> _list;
        public ScanResaltArg(List<DataQuant> Resalts)
        {
            _list = Resalts;
        }
        public List<DataQuant> ScanResalt
        {
            get { return _list; }
        }
    }

    public delegate void LoggerLogHandler(object sender, LoggerLogArg ea);
    public delegate void LoggerDataSavedHandler(object sender, EventArgs ea);
    public delegate void BarometricPressureChangedHandler(object sender, BPValueChangedEventsArg ea);

    public class BPValueChangedEventsArg : EventArgs
    {
        private int _loggerID;
        private int _channel;
        private double _value;
        public BPValueChangedEventsArg(int LoggerID, int Channel, double Value)
        {
            _loggerID = LoggerID;
            _channel = Channel;
            _value = Value;
        }
        public BPValueChangedEventsArg(int Channel, double Value)
        {
            _channel = Channel;
            _value = Value;
        }
        public int LoggerID
        {
            get { return _loggerID; }
            set { _loggerID = value; }
        }
        public int Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }


   
    
}
