using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wordslab.installer
{
    class Process
    {
        public static int Run(string program, string arguments, int timeoutsec, out string output, out string error, bool unicodeEncoding = false)
        {
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = program;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                if (unicodeEncoding) proc.StartInfo.StandardOutputEncoding = Encoding.Unicode;
                proc.StartInfo.RedirectStandardOutput = true;
                if (unicodeEncoding) proc.StartInfo.StandardErrorEncoding = Encoding.Unicode;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();

                string localerror = null;
                proc.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler((sender, e) =>
                { localerror += e.Data; });
                
                proc.Start();

                // To avoid deadlocks, use an asynchronous read operation on at least one of the streams.  
                proc.BeginErrorReadLine();
                output = proc.StandardOutput.ReadToEnd();
                error = localerror;              

                if(proc.WaitForExit(timeoutsec * 1000))
                {
                    return proc.ExitCode;
                }
                else
                {
                    throw new Exception($"Program {program} did not exit before the timeout of {timeoutsec} sec");
                }
            }
        }
    }
}
