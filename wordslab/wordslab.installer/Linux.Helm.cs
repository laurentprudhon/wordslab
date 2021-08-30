using System;

namespace wordslab.installer.Linux
{
    class Helm
    {
        // Executes : kubectl version
        // Returns  : 
        // Version object if kubectl version was correctly parsed
        // null otherwise
        public static Version CheckHelmVersion()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("helm", "version", 5, out output, out error);
                if (!String.IsNullOrEmpty(output))
                {
                    // Version:"v3.2.1"
                    int versionIndex = output.IndexOf("Version");
                    if (versionIndex > 0 && (versionIndex + 10) < output.Length)
                    {
                        versionIndex += 10;
                        int firstDot = output.IndexOf('.', versionIndex);
                        int secondDot = output.IndexOf('.', firstDot + 1);
                        int eol = output.IndexOf('"', secondDot + 1);
                        if (firstDot > versionIndex && secondDot > firstDot && eol > secondDot)
                        {
                            var major = Int32.Parse(output.Substring(versionIndex, firstDot - versionIndex));
                            var minor = Int32.Parse(output.Substring(firstDot + 1, secondDot - firstDot - 1));
                            var build = Int32.Parse(output.Substring(secondDot + 1, eol - secondDot - 1));
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
