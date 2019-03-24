using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LoggerLib
{
    public class MyTxtLogger : IDisposable
    {
        private List<string> _logsNames;
        private StreamWriter _writer;
        private bool _useC;
        private bool _useD;
        private bool _useF;
        private bool _useE;
        private bool _append;
        private object _locker;

        #region Constructor
        private MyTxtLogger(bool append)
        {
            _logsNames = new List<string>();
            _logsNames.Add(@"c:\mylog.txt");
            _logsNames.Add(@"d:\mylog.txt");
            _logsNames.Add(@"e:\mylog.txt");
            _logsNames.Add(@"f:\mylog.txt");
            Append = append;
            _locker = new object();
        }
        #endregion

        #region Create logger
        public static MyTxtLogger CreateTxtLogger(bool append = true)
        {
            return new MyTxtLogger(append);
        }
        #endregion

        #region Public properties
        public bool Use_C
        {
            get
            {
                return _useC;
            }

            set
            {
                _useC = value;
                if (value)
                {
                    _useD = !value;
                    _useE = !value;
                    _useF = !value;
                    //trancating file if needed
                    if (!Append && File.Exists(this["C"]))
                        using (FileStream file = new FileStream(this["C"], FileMode.Truncate)) { }
                }
            }
        }

        public bool Use_D
        {
            get
            {
                return _useD;
            }

            set
            {
                _useD = value;
                if (value)
                {
                    _useC = !value;
                    _useE = !value;
                    _useF = !value;
                    //trancating file if needed
                    if (!Append && File.Exists(this["D"]))
                        using (FileStream file = new FileStream(this["D"], FileMode.Truncate)) { }
                }
            }
        }

        public bool Use_F
        {
            get
            {
                return _useF;
            }

            set
            {
                _useF = value;
                if (value)
                {
                    _useC = !value;
                    _useD = !value;
                    _useE = !value;
                    //trancating file if needed
                    if (!Append && File.Exists(this["F"]))
                        using (FileStream file = new FileStream(this["F"], FileMode.Truncate)) { }
                }
            }
        }

        public bool Use_E
        {
            get
            {
                return _useE;
            }

            set
            {
                _useE = value;
                if (value)
                {
                    _useC = !value;
                    _useD = !value;
                    _useF = !value;
                    //trancating file if needed
                    if (!Append && File.Exists(this["E"]))
                        using (FileStream file = new FileStream(this["E"], FileMode.Truncate)) { }
                }
            }
        }

        public bool Append
        {
            get
            {
                return _append;
            }

            set
            {
                _append = value;
            }
        }
        #endregion

        #region Indexators
        public string this[int index]
        {
            get { return _logsNames[index]; }
        }

        public string this[string value]
        {
            get
            {
                if (value.Equals("C"))
                    return _logsNames[0];
                if (value.Equals("D"))
                    return _logsNames[1];
                if (value.Equals("E"))
                    return _logsNames[2];
                if (value.Equals("F"))
                    return _logsNames[3];
                else
                    throw new ArgumentOutOfRangeException("Such value was not found");
            }
        }
        #endregion

        #region WriteProtocol overloads
        //uses current log
        public void WriteProtocol(string action, string whoCalled, string description)
        {
            string currentLogName = _useC ? this["C"] :
                _useD ? this["D"] :
                _useE ? this["E"] : this["F"];
            try
            {
                lock (_locker)
                {
                    using (_writer = new StreamWriter(currentLogName, true, System.Text.Encoding.Default))
                    {
                        _writer.Write(DateTime.Now.ToString());
                        _writer.Write("\t");
                        _writer.Write(action);
                        _writer.Write("\t");
                        _writer.Write(whoCalled);
                        _writer.Write("\t");
                        _writer.Write(description);
                        _writer.Write("\r\n");
                        _writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        //uses directly pointed log
        public void WriteProtocol(string logName, string action, string whoCalled, string description)
        {
            try
            {
                lock (_locker)
                {
                    using (StreamWriter sw = new StreamWriter(logName, true, System.Text.Encoding.Default))
                    {
                        sw.Write(DateTime.Now.ToString());
                        sw.Write("\t");
                        sw.Write(action);
                        sw.Write("\t");
                        sw.Write(whoCalled);
                        sw.Write("\t");
                        sw.Write(description);
                        sw.Write("\r\n");
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()//for manual call
        {
            _writer.Dispose();
        }
        void IDisposable.Dispose() //auto-called
        {
            _writer.Dispose();
        }

        #endregion

    }
}
