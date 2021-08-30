using System;

namespace wordslab.installer.Windows
{
    class Wsl
    {
        // Executes : wsl -l -v
        // Returns  : 
        // -1 if WSL2 is not installed
        //  0 if WSL2 is ready but no distribution was installed
        //  1 if WSL2 is ready but the default distribution is set to run in WSL version 1
        //  2 if WSL2 is ready and the default distribution is set to run in WSL version 2
        public static int CheckWSLVersion()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("wsl.exe", "-l -v", 5, out output, out error, true);
                if (exitcode == 0 && String.IsNullOrEmpty(error))
                {
                    if (String.IsNullOrEmpty(output))
                    {
                        return 0;
                    }
                    var lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length < 2)
                    {
                        return 0;
                    }
                    for (var i = 1; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        var cols = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (cols.Length == 4)
                        {
                            return Int32.Parse(cols[3]);
                        }
                    }
                    return 0;
                }
            }
            catch (Exception)
            { }
            return -1;
        }

        // Executes : wsl -- uname -r
        // Returns  :  
        // Version object if kernel version was correctly parsed
        // null otherwise
        public static Version CheckKernelVersion()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("wsl.exe", "-- uname -r", 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    int firstDot = output.IndexOf('.');
                    int secondDot = output.IndexOf('.', firstDot + 1);
                    if(firstDot > 0 && secondDot > firstDot)
                    {
                        var major = Int32.Parse(output.Substring(0, firstDot));
                        var minor = Int32.Parse(output.Substring(firstDot+1, secondDot-firstDot-1));
                        return new Version(major, minor);
                    }
                }
            }
            catch (Exception)
            { }
            return null;
        }

        // Executes : wsl -- cat /etc/*-release
        // Returns  :  
        // true if the default distribution launched by the wsl command is Ubuntu
        // false otherwise
        public static bool CheckUbuntuDistribution(out string distrib, out string version)
        {
            distrib = "unknown";
            version = "?";
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("wsl.exe", "-- cat /etc/*-release", 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    var lines = output.Split('\n');
                    foreach(var line in lines)
                    {
                        if (line.StartsWith("DISTRIB_ID="))
                        {
                            distrib = line.Substring(11);
                        }
                        else if (line.StartsWith("DISTRIB_RELEASE="))
                        {
                            version = line.Substring(16);
                        }
                    }
                    var major = Int32.Parse(version.Substring(0, 2));
                    if( String.Compare(distrib, "Ubuntu", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                        major >= 18)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            { }
            return false;
        }
    }
}
