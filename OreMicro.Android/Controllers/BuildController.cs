using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OreMicro.Android.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OreMicro.Android.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildController : ControllerBase
    {
        readonly Build build = new Build();
        readonly ReturnData returnData = new ReturnData();

        [HttpGet]
        [Route("CheckService")]
        public object CheckService()
        {
            try
            {
                return "TRUE";
            }
            catch (Exception ex)
            {
                string psError = ex.ToString();
                return psError;
            }
        }

        [HttpGet]
        [Route("SysInfo")]
        public object SysInfo()
        {
            try
            {
                return build.SysInfo();
            }
            catch (Exception ex)
            {
                string psError = ex.ToString();
                return psError;
            }
        }

        [HttpGet]
        [Route("GetPath")]
        public object GetPath(string psPath = "")
        {
            try
            {
                return build.GetPath(psPath);
            }
            catch (Exception ex)
            {
                string psError = ex.ToString();
                return psError;
            }
        }

        [HttpGet]
        [Route("FirstRun")]
        public object FirstRun()
        {
            try
            {
                return build.FirstRun();
            }
            catch (Exception ex)
            {
                string psError = ex.ToString();
                return psError;
            }
        }

        [HttpGet]
        [Route("CreateFolder")]
        public object CreateFolder(string psPath, string psFolder)
        {
            try
            {
                return build.CreateFolder(psPath, psFolder);
            }
            catch (Exception ex)
            {
                string psError = ex.ToString();
                return psError;
            }
        }

        [HttpGet]
        [Route("GetConfig")]
        public object GetConfig()
        {
            try
            {
                return build.GetConfig();
            }
            catch (Exception ex)
            {
                string psError = ex.ToString();
                return psError;
            }
        }

        [HttpPost("ExecuteCommand")]
        public object ExecuteCommand(ExecuteCommand_IN executeCommand_IN)
        {
            try
            {
                //ExecuteCommand_IN executeCommand_IN = new ExecuteCommand_IN();
                return build.ExecuteCommmand(executeCommand_IN);
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "CONTROLLER EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }

        [HttpPost("DownloadApp")]
        public object DownloadApp(DownloadApp_IN downloadApp_IN)
        {
            try
            {
                return build.DownloadApp(downloadApp_IN);
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "CONTROLLER EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }

        [HttpPost("ZipExtract")]
        public object ZipExtract(ZipExtract_IN zipExtract_IN)
        {
            try
            {
                return build.ZipExtract(zipExtract_IN);
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "CONTROLLER EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }

        [HttpPost]
        [Route("GetAPKHostURL")]
        public object GetAPKHostURL(GetAPKHostURL_IN GetAPKHostURL_IN)
        {
            GetAPKHostURL_OUT GetAPKHostURL_OUT = new GetAPKHostURL_OUT();
            try
            {
                return build.GetAPKHostURL(GetAPKHostURL_IN);
            }
            catch (Exception ex)
            {
                GetAPKHostURL_OUT.Status = false;
                GetAPKHostURL_OUT.Message = "CONTROLLER EXCEPTION ERROR";
                GetAPKHostURL_OUT.Data = ex.Message;
                GetAPKHostURL_OUT.Error = ex.ToString();
                return GetAPKHostURL_OUT;
            }
        }

        [HttpPost("DeleteApp")]
        public object DeleteApp(DeleteApp_IN deleteApp_IN)
        {
            try
            {
                return build.DeleteApp(deleteApp_IN);
            }
            catch (Exception ex)
            {
                returnData.Status = false;
                returnData.Message = "CONTROLLER EXCEPTION ERROR";
                returnData.Data = ex.Message;
                returnData.Error = ex.ToString();
                return returnData;
            }
        }
    }
}
