using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OreMicro.Android.General
{
    public class OreLog
    {
        public static void AddUserLog(string psError, string psPath = "")
        {
            try
            {
                if (String.IsNullOrEmpty(psPath)) psPath = Config.BaseSrcPath;
                StreamWriter sw = null;
                if (psPath != "")
                {
                    DateTime dtTime = DateTime.Now;

                    string lsFile = psPath + "buildlog.txt";
                    if (!File.Exists(lsFile)) File.Create(lsFile).Close();
                    using (sw = new StreamWriter(lsFile, true))
                    {
                        sw.WriteLine(Environment.NewLine);
                        sw.WriteLine(dtTime.ToString() + ":" + psError);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                string message = ex.Message;
            }
        }
    }
}
