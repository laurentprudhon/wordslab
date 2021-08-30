using System;

namespace wordslab.installer.Linux
{
    class Docker
    {
        // Executes : docker --version
        // Returns  : 
        // Version object if Docker engine version was correctly parsed
        // null otherwise
        public static Version CheckDockerVersion()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("docker", "--version", 5, out output, out error);
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

        public static bool CheckWindowsSubsystemForLinux()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("cat", "/proc/sys/kernel/osrelease", 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    if (output.Contains("microsoft", StringComparison.InvariantCultureIgnoreCase) || output.Contains("WSL",StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            { }
            return false;            
        }
    }
}
