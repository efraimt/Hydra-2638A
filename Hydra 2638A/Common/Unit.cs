using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydra_2638A.Common
{
    public class Unit
    {

        private int _ID;
        private string _Name;
        private string _ShortName;

        public Unit(int ID, string Name, string ShortName)
        {
            _ID = ID;
            _Name = Name;
            _ShortName = ShortName;
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string ShortName
        {
            get { return _ShortName; }
            set { _ShortName = value; }
        }

        //public double AbsMaxVal { get; set; }
        //public double AbsMinVal { get; set; }
        //public UnitGroup Group { get; }
        //public int GroupID { get; }
        //public bool HasAbsMax { get; set; }
        //public bool HasAbsMin { get; set; }
        //public bool HasAbsZero { get; set; }
        //public string Info { get; }
        //public string LongNameEn { get; }
        //public string LongNameHe { get; }
        //public string Note { get; }
        //public int Rarit { get; }
        //public string ShortNameEn { get; }
        //public string ShortNameEn_prv { get; }
        //public string ShortNameEnAsc { get; }
        //public string ShortNameHe { get; }
        //public string ShortNameHe_prv { get; }
        //public string ShortNameHeAsc { get; }

    }
}
