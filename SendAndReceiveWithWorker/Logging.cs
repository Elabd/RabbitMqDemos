using System;
using System.Globalization;
using System.IO;


namespace SendAndReceiveWithWorker
{
    public class Logging
    {
        public Logging()
        {
        }

        private static readonly object locker = new object();

        public void WriteToLog(string message)
        {
           // string filePath = $"../../Data/Log{Thread.CurrentThread.ManagedThreadId}.txt";
            string filePath = $"../../Data/Log.txt";

           
            lock (locker)
            {
                if (!File.Exists(filePath)) { File.Create(filePath); }
                   
                StreamWriter SW;
                SW = File.AppendText(filePath);
                
                SW.WriteLine("-----------------------------------------------------------------------------");
                SW.WriteLine("Date : " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                SW.WriteLine();
                SW.WriteLine($"message : {message}");
                SW.Close();
            }
        }
    }
}
