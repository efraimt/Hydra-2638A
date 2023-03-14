using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydra_2638A
{

    class FDLChannel
    {
        #region Fields
        private int _n;
        private string[] _par;
        string _mode;
        private double _min;
        private double _max;
        private int _precision;
        //private Correction _correct;
        private bool _isBP_Measured;
        private double _bpValue;
        private List<double> _bpValues;

        private bool _isAllowMinOOR;
        private bool _isAllowMaxOOR;        

        #endregion
        #region Constructors
        public FDLChannel()
        {
            //_bpcounter = 0;
            _bpValues = new List<double>();
        }
        public FDLChannel(int n)
        {
            _n = n;
            _bpValues = new List<double>();

        }
        #endregion
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public int Number
        {
            get { return _n; }
            set { _n = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string[] Params
        {
            get { return _par; }
            set { _par = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }
        public bool IsBpMeasured
        {
            get { return _isBP_Measured; }
            set
            {
                _isBP_Measured = value;
                //if (_isBP_Measured)
                //    createBpArray();
            }
        }
        public double BarometricPressure
        {
            get { return _bpValue; }
            set { _bpValue = value; }
        }
        public bool IsAllowMinOOR
        {
            get { return _isAllowMinOOR; }
            set { _isAllowMinOOR = value; }
        }
        public bool IsAllowMaxOOR
        {
            get { return _isAllowMaxOOR; }
            set { _isAllowMaxOOR = value; }
        }
        #endregion
        #region Public Members       

        // convert  Mesurement unit --> User unit

        public decimal ConvertToUU(decimal Value)
        {
            //עבור לחןת צריך לכפול ב-100
            if (_mode == "VDC")
                return 100 * Value;
            else
                return Value;
        }
        //correction for Mesurement unit
        public decimal GetCorrectedValue(decimal Value)
        {
            

            //TODO: return correct calculated value
            return Value;
        }
        public void ResetBP()
        {
            _bpValues = new List<double>();
        }
        #endregion
        #region Private Members
        
     
        #endregion
    }


    public class ChannelDefinition : ICloneable
    {
        public const int A385 = 0;
        public const int A392 = 1;

        private int _sensorid;
        private double _min;
        private double _max;
        private bool _is2w;
        private int _unitid;
        private string _unitname;
        private int _daunitid;
        private int _precision;

        private int _measurementid;
        private string _measurementname;
        private int _workrangeunit;
        private double _workmin;
        private double _workmax;
        //private double _workmincurrent;
        //private double _workmaxcurrent;
        private double _damin;
        private double _damax;
        private bool _isBpMeasured;
        private double _bpValue;
        private int _specific_type;
        public ChannelDefinition()
        {
            _sensorid = -1;
            _unitid = -1;
            _daunitid = -1;
            _precision = 1;
            _bpValue = 1013.25;
            reset();
        }
        public int SensorID
        {
            get { return _sensorid; }
            set
            {
                _sensorid = value;
                if (_sensorid == -1)
                    reset();
            }
        }
        public double MinValue
        {
            get { return _min; }
            set { _min = value; }

        }
        public double WorkMinValue
        {
            get { return _workmin; }
            set { _workmin = value; }
        }
        public int SpecificType
        {
            get { return _specific_type; }
            set { _specific_type = value; }
        }

        public string SpecificTypeStr
        {
            get { return ((_specific_type == A385) ? "A385" : "A392"); }
        }

        //public double WorkMinCurrentValue
        //{
        //    get { return _workmincurrent; }
        //    set { _workmincurrent = value; }

        //}
        public double WorkMaxValue
        {
            get { return _workmax; }
            set { _workmax = value; }

        }
        //public double WorkMaxCurrentValue
        //{
        //    get { return _workmaxcurrent; }
        //    set { _workmaxcurrent = value; }

        //}
        public double MaxValue
        {
            get { return _max; }
            set { _max = value; }

        }
        public double DAMaxValue
        {
            get { return _damax; }
            set { _damax = value; }

        }
        public double DAMinValue
        {
            get { return _damin; }
            set { _damin = value; }

        }
        public bool Is2W
        {
            get { return _is2w; }
            set { _is2w = value; }
        }
        public int MeasurementID
        {
            get { return _measurementid; }
            set { _measurementid = value; }
        }
        public string MeasurementName
        {
            get { return _measurementname; }
            set { _measurementname = value; }
        }
        public string UnitName
        {
            get { return _unitname; }
            set { _unitname = value; }
        }        
        public int UnitID
        {
            get { return _unitid; }
            set { _unitid = value; }
        }
        public int DaUnitID
        {
            get { return _daunitid; }
            set { _daunitid = value; }
        }
        public int WorkRangeUnitID
        {
            get { return _workrangeunit; }
            set { _workrangeunit = value; }
        }
        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }
        public bool IsBP_Measured
        {
            get { return _isBpMeasured; }
            set { _isBpMeasured = value; }
        }
        public double BP_Value
        {
            get { return _bpValue; }
            set { _bpValue = value; }
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        private void reset()
        {
            //_min =_workmin= _workmincurrent=-10000;
            //_max = _workmax= _workmaxcurrent= 10000;
            _min = _workmin = _damin = -10000;
            _max = _workmax = _damax = 10000;
            _is2w = true;
            _measurementid = -1;
            _workrangeunit = -1;
            _specific_type = A385;
        }
    }
    public class ChannelDefinitionLinks
    {
        private const int linkMaxCnt = 61;//41;
        private ChannelDefinition _ch;
        private bool[] _links;
        public ChannelDefinitionLinks()
        {
            _links = new bool[linkMaxCnt];
            _ch = new ChannelDefinition();
        }
        public ChannelDefinitionLinks(ChannelDefinition Ch)
        {
            _links = new bool[linkMaxCnt];
            _ch = (ChannelDefinition)Ch.Clone();
        }
        public ChannelDefinitionLinks(ChannelDefinitionLinks Cdl)
        {
            this._links = Cdl.Links;
            this._ch = (ChannelDefinition)Cdl.ChannelDefinition.Clone();
        }
        public bool[] Links
        {
            get { return _links; }
        }

        public void SetLink(int Channel, bool Value)
        {
            _links[Channel] = Value;
        }
        public ChannelDefinition ChannelDefinition
        {
            get { return _ch; }
            set { _ch = value; }
        }
        private void clear_links()
        {
            for (int i = 0; i < _links.Length; i++)
            {
                _links[i] = false;
            }
        }
        public void ClearLinks()
        {
            clear_links();
        }
        public bool IsReady()
        {
            bool link = false;
            for (int i = 0; i < _links.Length; i++)
            {
                if (_links[i])
                {
                    link = true;
                    break;
                }
            }
            return link;
        }
    }
}
