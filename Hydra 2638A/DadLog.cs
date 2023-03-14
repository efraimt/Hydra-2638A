using System;
using System.IO;

namespace Hydra_2638A
{

    public sealed class DadLog
    {
        public static readonly DadLog Instanse = new DadLog();
        private string _file;
        private DadLog()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            _file = path + "DadLog.txt";
            if (File.Exists(_file))
                File.Delete(_file);
        }
        public void Write(string msg)
        {
            writeToLog(_file, msg);
        }
        public void Write(string File, string msg)
        {
            writeToLog(File, msg);
        }
        private void writeToLog(string File, string msg)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File, true))  //append mode
                {
                    object synclock = new object();
                    lock (synclock)
                    {
                        sw.WriteLine(string.Format("{0:dd/MM/yyyy HH:mm:ss}.{1:000} : ", DateTime.Now, DateTime.Now.Millisecond) + msg);
                        sw.Close();
                    }
                }
            }
            catch
            {
            }
        }
    }

}