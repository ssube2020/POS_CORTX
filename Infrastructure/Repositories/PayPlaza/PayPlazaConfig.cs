using System;
using System.IO;

namespace YourNamespace
{
    public class PayPlazaConfig
    {
        private PayPlazaConfig() { }

        private static readonly PayPlazaConfig instance = new PayPlazaConfig();

        public static PayPlazaConfig Instance
        {
            get
            {
                return instance;
            }
        }

        public string P12Pass
        {
            get
            {
                string p12Pass = "";
                string privateFolder = Environment.GetEnvironmentVariable("PRIVATE_FOLDER");
                string[] lines = File.ReadAllLines(Path.Combine(privateFolder, "PayPlazaP12.txt"));
                foreach (string line in lines)
                {
                    p12Pass = line.Trim();
                    break;
                }
                return p12Pass;
            }
        }
    }
}
