using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace Hydra_2638A.DaConverter
{
    public class DaConverter
    {
        private DataTable _dtcfg;
        private DataHolder _dh;
        public DaConverter()
        {
        }
        public void Load(string FileName)
        {
            //FileInfo fi = new FileInfo(FileName);
            byte[] cfgbuf;
            byte[] dhbuf;
            using (FileStream fs = new FileStream(FileName,FileMode.Open,FileAccess.Read))
            {
                int size = sizeof(int);
                byte[] header = new byte[size];
                fs.Read(header, 0, size);
                int cfgsize = BitConverter.ToInt32(header,0);
                fs.Read(header, 0, size);
                int dhsize = BitConverter.ToInt32(header, 0);
                cfgbuf = new byte[cfgsize];
                dhbuf = new byte[dhsize];
                fs.Read(cfgbuf, 0, cfgsize);
                fs.Read(dhbuf, 0, dhsize);

            }
            //string cfg = Encoding.Unicode.GetString(cfgbuf);
            string cfg = Encoding.Default.GetString(cfgbuf);

            DataSet ds = new DataSet();
            StringReader xml = new StringReader(cfg);
            ds.ReadXml(xml, XmlReadMode.ReadSchema);
            _dtcfg = ds.Tables[0];
            _dh = new DataHolder(dhbuf);
        }
        public void Save(string FileName,DataTable dtConfig,DataHolder MeasurmentData)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dtConfig);
            byte [] cfgbuf;
            byte [] dhbuf;
            using (MemoryStream ms = new MemoryStream())
            {
                ds.WriteXml(ms, XmlWriteMode.WriteSchema);
                ms.Position = 0;
                cfgbuf=ms.ToArray();
                dhbuf=MeasurmentData.ToBytesArray();
                int cfgsize = cfgbuf.Length;
                int dhsize = dhbuf.Length;
                using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                {
                    
                    
                    byte[] header = BitConverter.GetBytes(cfgsize);
                    fs.Write(header, 0, header.Length);
                    header = BitConverter.GetBytes(dhsize);
                    fs.Write(header, 0, header.Length);
                    fs.Write(cfgbuf, 0, cfgbuf.Length);
                    fs.Write(dhbuf, 0, dhbuf.Length);
                }
                ds.Tables.Clear();
            }
        }
        public DataTable Config
        {
            get { return _dtcfg; }
        }
        public DataHolder MeasurementData
        {
            get { return _dh; }
        }
    }
}
