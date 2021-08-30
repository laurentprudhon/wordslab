using System;

namespace wordslab.installer.Windows
{
    class Docker
    {
        // Executes : wsl -- ls -l $(which docker)
        // Returns  : 
        // true if a previous docker version must be uninstalled from WSL
        // false otherwise
        public static bool CheckPreviousDockerVersionInWsl()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("wsl.exe", "-- ls -l $(which docker)", 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    if(!output.Contains("docker-desktop"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            { }
            return false;
        }

        // Executes : docker --version
        // Returns  : 
        // Version object if Windows Docker version was correctly parsed
        // null otherwise
        public static Version CheckWindowsDockerVersion(bool fromWSL = false)
        {
            string command = "docker.exe";
            string args = "--version";
            if(fromWSL)
            {
                command = "wsl.exe";
                args = "-- docker --version";
            }
            try
            {
                string output;
                string error;
                int exitcode = Process.Run(command, args, 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    int versionIndex = output.IndexOf("version");
                    if (versionIndex > 0 && (versionIndex + 8) < output.Length)
                    {
                        versionIndex += 8;
                        int firstDot = output.IndexOf('.', versionIndex);
                        int secondDot = output.IndexOf('.', firstDot + 1);
                        int comma = output.IndexOf(',', secondDot + 1);
                        if (firstDot > versionIndex && secondDot > firstDot && comma > secondDot)
                        {
                            var major = Int32.Parse(output.Substring(versionIndex, firstDot - versionIndex));
                            var minor = Int32.Parse(output.Substring(firstDot + 1, secondDot - firstDot - 1));
                            var build = Int32.Parse(output.Substring(secondDot + 1, comma - secondDot - 1));
                            return new Version(major, minor, build);
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return null;
        }
    }
}
