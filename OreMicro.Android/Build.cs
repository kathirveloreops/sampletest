using Amazon.S3;
using Amazon.S3.Transfer;
using Newtonsoft.Json;
using OreMicro.Android.General;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OreMicro.Android
{
    public class Build
    {
        readonly ReturnData returnData = new ReturnData();
        string psError = string.Empty;
        public object SysInfo()
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                var cpuCount = Environment.ProcessorCount.ToString();
                var os = Environment.OSVersion.ToString();
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                var totsize = FormatBytes(allDrives[0].TotalSize);
                dict.Add("Operating System", os);
                dict.Add("Total CPU", cpuCount);
                dict.Add("Total Diskspace", totsize);

                return JsonConvert.SerializeObject(dict);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        public object GetPath(string psPath)
        {
            try
            {
                if (String.IsNullOrEmpty(psPath)) psPath = "/";
                DirectoryInfo directoryInfo = new DirectoryInfo(psPath);
                List<string> folders = new List<string>();
                var direcotries = Directory.GetDirectories(psPath);
                foreach (var folder in direcotries)
                    folders.Add(new DirectoryInfo(folder).Name);
                var files = Directory.GetFiles(psPath, "*", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                    folders.Add(Path.GetFileName(file));
                return JsonConvert.SerializeObject(folders);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        public object GetConfig()
        {
            try
            {
                ConfigClass config = new ConfigClass();
                return JsonConvert.SerializeObject(config);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        public object FirstRun()
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string exepath = String.Concat(Config.BaseSrcPath, "1.0/oreapp/");
                string errorpath = String.Concat(Config.BaseSrcPath, "1.0/oreapp/");
                string lsCommand = "react-native bundle --platform android --dev false --entry-file index.js --bundle-output android/app/src/main/assets/index.android.bundle --assets-dest android/app/src/main/res";
                var result = Bash(exepath, lsCommand, errorpath);
                dict.Add("React", JsonConvert.SerializeObject(result));
                exepath += "android";
                lsCommand = "chmod +x gradlew && ./gradlew clean && ./gradlew assembleDebug";
                result = Bash(exepath, lsCommand, errorpath);
                dict.Add("Gradle", JsonConvert.SerializeObject(result));
                return JsonConvert.SerializeObject(dict);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }


        public object CreateFolder(string psPath, string psFolder)
        {
            try
            {
                string psMessage = string.Empty;
                if (Directory.Exists(psPath))
                {
                    Directory.CreateDirectory(psPath + psFolder);
                    psMessage = "Folder created";
                }
                else
                {
                    psMessage = "base folder not available";
                }
                return JsonConvert.SerializeObject(psMessage);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        public object ExecuteCommmand(ExecuteCommand_IN executeCommand_IN)
        {
            string errorpath = String.Concat(Config.BaseSrcPath, executeCommand_IN.FrameworkVersion, "/oreapp/");
            string exepath = String.Concat(Config.BaseSrcPath, executeCommand_IN.FrameworkVersion, "/oreapp/");

            if (!String.IsNullOrEmpty(executeCommand_IN.Path)) exepath += executeCommand_IN.Path;
            string lsCommand = Regex.Replace(executeCommand_IN.Command, "projectsource", executeCommand_IN.AppName.ToLower());

            try
            {
                if (Directory.Exists(exepath))
                {
                    OreLog.AddUserLog("------------------------------------------------COMMAND START--------------------------------------------", errorpath);
                    OreLog.AddUserLog("APP NAME: " + executeCommand_IN.AppName, errorpath);
                    return Bash(exepath, lsCommand, errorpath);
                }
                else
                {
                    returnData.Status = false;
                    returnData.Message = "Execution path not exist";
                    returnData.Data = "";
                    returnData.Error = exepath;
                }

                return returnData;
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }

        public object DownloadApp(DownloadApp_IN downloadApp_IN)
        {
            try
            {
                string zipPath = String.Concat(Config.BaseSrcPath, downloadApp_IN.FrameworkVersion, "/oreapp/");
                string Filepath = zipPath + downloadApp_IN.Filename;
                if (Directory.Exists(zipPath))
                {
                    if (File.Exists(Filepath))
                    {
                        File.Delete(Filepath);
                    }
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(downloadApp_IN.URL, Filepath);
                    }
                    if (File.Exists(Filepath))
                    {
                        returnData.Status = true;
                        returnData.Message = "File downloaded";
                        returnData.Data = Filepath;
                        returnData.Error = "";
                    }
                    else
                    {
                        returnData.Status = false;
                        returnData.Message = "File downloaded but not available";
                        returnData.Data = Filepath;
                        returnData.Error = "";
                    }
                }
                else
                {
                    returnData.Status = false;
                    returnData.Message = "Folder not Exist";
                    returnData.Data = zipPath;
                    returnData.Error = "";
                }
                return returnData;
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }

        public object ZipExtract(ZipExtract_IN zipExtract_IN)
        {
            try
            {
                string zipPath = String.Concat(Config.BaseSrcPath, zipExtract_IN.FrameworkVersion, "/oreapp/IOS_", zipExtract_IN.AppName.ToLower(), ".zip");
                string ExtractPath = String.Concat(Config.BaseSrcPath, zipExtract_IN.FrameworkVersion, "/oreapp/");
                if (File.Exists(zipPath))
                {
                    ZipFile.ExtractToDirectory(zipPath, ExtractPath, true);
                    returnData.Status = true;
                    returnData.Message = "Extracted Successfully";
                    returnData.Data = zipPath;
                    returnData.Error = "";
                }
                else
                {
                    returnData.Status = false;
                    returnData.Message = "File not exists in backup path";
                    returnData.Data = "";
                    returnData.Error = zipPath;
                }
                return returnData;
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }

        public object GetAPKHostURL(GetAPKHostURL_IN getAPKHostURL_IN)
        {
            GetAPKHostURL_OUT GetAPKHostURL_OUT = new GetAPKHostURL_OUT();
            try
            {
                string lsAppPath = String.Concat(Config.BaseSrcPath, getAPKHostURL_IN.FrameworkVersion, "/oreapp/");
                string lsAPkpath = string.Empty;
                string lsurl = string.Empty;
                string lslogurl = string.Empty;
                string psError = string.Empty;
                string logfilePath = String.Concat(lsAppPath, "buildlog.txt");
                if (getAPKHostURL_IN.Debug)
                    lsAPkpath = String.Concat(lsAppPath, "android/app/build/outputs/apk/debug/app-debug.apk");
                else
                    lsAPkpath = String.Concat(lsAppPath, "android/app/build/outputs/apk/release/app-release.apk");

                if (!File.Exists(lsAPkpath))
                {
                    GetAPKHostURL_OUT.Status = false;
                    GetAPKHostURL_OUT.Message = "APK File not exists";
                    GetAPKHostURL_OUT.Data = lsAPkpath;
                    GetAPKHostURL_OUT.APKUrl = "";
                    GetAPKHostURL_OUT.LogUrl = "";
                    GetAPKHostURL_OUT.Error = "";
                    return GetAPKHostURL_OUT;
                }
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff",
                                           CultureInfo.InvariantCulture);
                string apkname = "app-" + getAPKHostURL_IN.UserName.ToLower() + "-" + timestamp + ".apk";
                string newlogfile = "buildlog-" + getAPKHostURL_IN.UserName.ToLower() + "-" + timestamp + ".txt";
                if (File.Exists(logfilePath)) UploadFileMPUHighLevelAPITest.UploadFile(logfilePath, newlogfile, ref psError);
                bool awsapkresult = UploadFileMPUHighLevelAPITest.UploadFile(lsAPkpath, apkname, ref psError);
                if (awsapkresult)
                {
                    lsurl = UploadFileMPUHighLevelAPITest.baseUrl + apkname;
                    lslogurl = UploadFileMPUHighLevelAPITest.baseUrl + newlogfile;
                    GetAPKHostURL_OUT.Status = true;
                    GetAPKHostURL_OUT.Message = "APK Url Exist";
                    GetAPKHostURL_OUT.Data = lsurl;
                    GetAPKHostURL_OUT.APKUrl = lsurl;
                    GetAPKHostURL_OUT.LogUrl = lslogurl;
                    GetAPKHostURL_OUT.Error = "";
                }
                else
                {
                    GetAPKHostURL_OUT.Status = false;
                    GetAPKHostURL_OUT.Message = "File not uploaded to cloud server";
                    GetAPKHostURL_OUT.Data = lsurl;
                    GetAPKHostURL_OUT.APKUrl = lsurl;
                    GetAPKHostURL_OUT.LogUrl = lslogurl;
                    GetAPKHostURL_OUT.Error = psError;
                }

                return GetAPKHostURL_OUT;
            }
            catch (Exception ex)
            {
                GetAPKHostURL_OUT.Status = false;
                GetAPKHostURL_OUT.Message = "EXCEPTION ERROR";
                GetAPKHostURL_OUT.Data = ex.Message;
                GetAPKHostURL_OUT.Error = ex.ToString();
                GetAPKHostURL_OUT.APKUrl = "";
                GetAPKHostURL_OUT.LogUrl = "";
                return GetAPKHostURL_OUT;
            }
        }

        public object DeleteApp(DeleteApp_IN deleteApp_IN)
        {
            try
            {
                string srcPath = String.Concat(Config.BaseSrcPath, deleteApp_IN.FrameworkVersion, "/oreapp/");
                string tempNodePath = String.Concat(Config.BaseSrcPath, deleteApp_IN.FrameworkVersion, "/node_modules/");
                string srcNodePath = String.Concat(Config.BaseSrcPath, deleteApp_IN.FrameworkVersion, "/oreapp/node_modules/");
                if (Directory.Exists(srcPath))
                {
                    Directory.Move(srcNodePath, tempNodePath);
                    Directory.Delete(srcPath, true);
                    if (!Directory.Exists(srcPath)) Directory.CreateDirectory(srcPath);
                    Directory.Move(tempNodePath, srcNodePath);
                    returnData.Status = true;
                    returnData.Message = "All folders and files deleted successfully";
                    returnData.Data = srcPath;
                    returnData.Error = "";
                }
                else
                {
                    returnData.Status = false;
                    returnData.Message = "Folder Not Exist";
                    returnData.Data = srcPath;
                    returnData.Error = "";
                }

                return returnData;
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private ReturnData Bash(string cmdpath, string cmd, string errorpath)
        {
            try
            {
                OreLog.AddUserLog("COMMAND PATH: " + cmdpath, errorpath);
                OreLog.AddUserLog("COMMAND: " + cmd, errorpath);
                var escapedArgs = cmd.Replace("\"", "\\\"");
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.WorkingDirectory = cmdpath;
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                processStartInfo.FileName = "/bin/bash";
                processStartInfo.Arguments = $"-c \"{escapedArgs}\"";
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardError = true;

                OreLog.AddUserLog("COMMAND EXECUTION - STARTED: " + cmd, errorpath);

                Process process = Process.Start(processStartInfo);
                if (process != null && !process.HasExited)
                {
                    process.WaitForExit(1000);
                }

                string output = string.Empty, error = string.Empty;
                if (processStartInfo.RedirectStandardError == true)
                {
                    output = JsonConvert.SerializeObject(process.StandardOutput.ReadToEnd());
                    error = JsonConvert.SerializeObject(process.StandardError.ReadToEnd());
                    returnData.Data = output;
                    returnData.Error = error;
                }

                process.Close();
                OreLog.AddUserLog("COMMAND EXECUTION - COMPLETED :" + cmd, errorpath);
                OreLog.AddUserLog("COMMAND RESULT - START", errorpath);
                OreLog.AddUserLog("OUTPUT :" + output, errorpath);
                OreLog.AddUserLog("ERROR :" + error, errorpath);
                OreLog.AddUserLog("COMMAND RESULT - END", errorpath);

                returnData.Status = true;
                returnData.Message = "COMMAND COMPLETED. Please check log to confirm";
                OreLog.AddUserLog("------------------------------------------------COMMAND END--------------------------------------------", errorpath);
                return returnData;
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "EXCEPTION ERROR IN BASH";
                returnData.Error = ex.ToString();
                returnData.Data = ex.Message;
                OreLog.AddUserLog(ex.ToString(), errorpath);
                OreLog.AddUserLog("------------------------------------------------COMMAND EXCEPTION END--------------------------------------------", errorpath);
                return returnData;
            }
        }
    }

    public static class UploadFileMPUHighLevelAPITest
    {
        public static string bucketName = "oreopsbuild";
        public static string folderName = "App";
        public static string baseUrl = "https://oreopsbuild.sgp1.digitaloceanspaces.com/App/";
        //public static string filePath = "d:\\sathish.k\\upload.txt";
        public static string endpoingURL = "https://sgp1.digitaloceanspaces.com";
        public static IAmazonS3 s3Client;

        public static bool UploadFile(string filePath, string fileName, ref string psError)
        {
            var s3ClientConfig = new AmazonS3Config
            {
                ServiceURL = endpoingURL
            };
            s3Client = new AmazonS3Client(s3ClientConfig);
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName + @"/" + folderName,
                    FilePath = filePath,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6291456, // 6 MB
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead
                };
                fileTransferUtility.Upload(fileTransferUtilityRequest);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                psError = String.Format("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                return false;
            }
            catch (Exception e)
            {
                psError = String.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                return false;
            }
        }
    }
}
