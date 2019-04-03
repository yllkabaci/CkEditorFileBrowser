using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CkEditorFileBrowser.Controllers
{

	public class ImageBrowserController : Controller
	{
		static IHostingEnvironment _hostingEnvironment;
		static IConfiguration Configuration;
		public ImageBrowserController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
		{
			_hostingEnvironment = hostingEnvironment;
			Configuration = configuration;
		}

		//I will use only images
		//If you need other type of files you can add them in imgext array
		protected String[] imgext = { ".bmp", ".gif", ".jpg", ".jpe", ".jpeg", ".png" };    // allowed image extensions

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public BrowserResponse GetImages([FromFormAttribute] string imgdr)
		{
			string webRoot = _hostingEnvironment.WebRootPath;
			BrowserResponse br = new BrowserResponse();
			br.imgs = "";
			br.menu = "";
			if (imgdr == null)
				//my files are at: wwwroot/uploads
				//you can configure your own path here on in appsettings
				imgdr = "uploads";
			string fullImagePath = Path.Combine(webRoot, imgdr);
			string[] allfiles = { };
			string[] allimages = { };
			string[] allfolders = { };
			try
			{
				allfiles = Directory.GetFiles(fullImagePath, "*.*", SearchOption.TopDirectoryOnly);
				allfolders = Directory.GetDirectories(fullImagePath);
			}
			catch (Exception e)
			{
				br.imgs = "<h2>ERROR</h2>";
			}

			foreach (string file in allfiles)
			{
				FileInfo fInfo = new FileInfo(file);
				//if the file is in the array imgext, add it to the br.imgs
				if (imgext.Contains(fInfo.Extension))
					br.imgs = br.imgs + "<span><img src='" + imgdr + "/" + fInfo.Name + "' alt='" + fInfo.Name + "' height='50' />" + FormatFileName(fInfo.Name, fInfo.Extension) + "</span>";
			}

			foreach (string folder in allfolders)
			{
				FileInfo fInfo = new FileInfo(folder);
				br.menu = br.menu + "<li><span title='" + imgdr + "/" + fInfo.Name + "'>" + fInfo.Name + "</span></li>";
			}

			if (allfiles.Count() == 0)
			{
				br.imgs = "<h1>No image</h1>";
			}

			if (allfolders.Count() != 0)
			{
				br.menu = "<ul>" + br.menu + "</ul>";
			}
			return br;
		}

		public string FormatFileName(string fileName, string extension)
		{
			fileName = fileName.Replace(extension, "");

			if (fileName.Count() > 20)
			{
				fileName = fileName.Substring(0, 20) + "..";
			}

			fileName = fileName + extension;
			return fileName;
		}

	}

	public class BrowserResponse
	{
		public string menu { get; set; }
		public string imgs { get; set; }
	}
}