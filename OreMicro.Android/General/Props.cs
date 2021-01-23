using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OreMicro.Android.General
{
    public class Props
    {

    }

	public class ReturnData
	{
		public bool Status { get; set; }
		public string Message { get; set; }
		public string Data { get; set; }
		public string Error { get; set; }
	}

	public class ExecuteCommand_IN
	{
		public string AppName { get; set; }
		public string Path { get; set; }
		public string Command { get; set; }
		public string FrameworkVersion { get; set; }
	}

	public class ExecuteCommand_OUT
	{
		public bool Status { get; set; }
		public string Message { get; set; }
		public string Data { get; set; }
		public string Error { get; set; }
	}
	public class DownloadApp_IN
	{
		public string UserName { get; set; }
		public string AppName { get; set; }
		public string URL { get; set; }
		public string Filename { get; set; }
		public string FrameworkVersion { get; set; }
	}

	public class ZipExtract_IN
	{
		public string UserName { get; set; }
		public string AppName { get; set; }
		public string FrameworkVersion { get; set; }
	}

	public class GetAPKHostURL_IN
	{
		public string UserName { get; set; }
		public string AppName { get; set; }
		public bool Debug { get; set; }
		public string FrameworkVersion { get; set; }
	}

	public class GetAPKHostURL_OUT
	{
		public bool Status { get; set; }
		public string Message { get; set; }
		public string Data { get; set; }
		public string Error { get; set; }
		public string APKUrl { get; set; }
		public string LogUrl { get; set; }
	}

	public class DeleteApp_IN
	{
		public string FrameworkVersion { get; set; }
	}

}
