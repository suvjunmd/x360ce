﻿using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using x360ce.Engine;
using x360ce.Engine.Win32;

namespace x360ce.App
{
	public class AppHelper
	{
		#region DLL Functions

		static void Elevate()
		{
			// If this is Vista/7 and is not elevated then elevate.
			if (WinAPI.IsVista && !WinAPI.IsElevated()) WinAPI.RunElevated();
		}

		public static bool WriteFile(string resourceName, string destinationFileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var sr = assembly.GetManifestResourceStream(resourceName);
			FileStream sw = null;
			try
			{
				sw = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write);
			}
			catch (Exception)
			{
				Elevate();
				return false;
			}
			var buffer = new byte[1024];
			while (true)
			{
				var count = sr.Read(buffer, 0, buffer.Length);
				if (count == 0) break;
				sw.Write(buffer, 0, count);
			}
			sr.Close();
			sw.Close();
			return true;
		}

		public static bool CopyFile(string sourceFileName, string destFileName)
		{
			try
			{
				File.Copy(sourceFileName, destFileName, true);
			}
			catch (Exception)
			{
				Elevate();
				return false;
			}
			return true;
		}


		///// <summary></summary>
		///// <returns>True if file exists.</returns>
		//public static bool CreateDllFile(bool create, string file)
		//{
		//	if (create)
		//	{
		//		// If file don't exist exists then...
		//		var present = EngineHelper.GetDefaultDll();
		//		if (present == null)
		//		{
		//			var xFile = JocysCom.ClassLibrary.ClassTools.EnumTools.GetDescription(XInputMask.XInput13_x86);
		//			var resourceName = EngineHelper.GetXInputResoureceName();
		//                  AppHelper.WriteFile(resourceName, xFile);
		//		}
		//		else if (!File.Exists(file))
		//		{
		//			present.CopyTo(file, true);
		//		}
		//	}
		//	else
		//	{
		//		if (File.Exists(file))
		//		{
		//			try
		//			{
		//				File.Delete(file);
		//			}
		//			catch (Exception) { }
		//		}
		//	}
		//	return File.Exists(file);
		//}

		#endregion

		public static Bitmap GetDisabledImage(Bitmap image)
		{
			var effects = new JocysCom.ClassLibrary.Drawing.Effects();
			var newImage = (Bitmap)image.Clone();
			effects.GrayScale(newImage);
			effects.Transparent(newImage, 50);
			return newImage;
		}

		// Use special function or comparison fails.
		public static bool IsSameDevice(Device device, Guid instanceGuid)
		{
			return instanceGuid.Equals(device == null ? Guid.Empty : device.Information.InstanceGuid);
		}

		public static void GetFiles(DirectoryInfo di, ref List<FileInfo> fileList, string searchPattern, bool allDirectories)
		{
			try
			{
				if (allDirectories)
				{
					foreach (DirectoryInfo subDi in di.GetDirectories())
					{
						GetFiles(subDi, ref fileList, searchPattern, allDirectories);
					}
				}
			}
			catch { }
			try
			{
				fileList.AddRange(di.GetFiles(searchPattern));
			}
			catch { }
		}

	}
}
