﻿using System;
using System.Diagnostics;
using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Tizen;
using Xamarin.UITest;

namespace Xamarin.Forms.Core.UITests
{
	public class TizenTestBase
	{
		protected const string TizenApplicationDriverUrl = "http://192.168.0.49:4723/wd/hub";
		protected static TizenDriver<TizenElement> Session;

		public static IApp ConfigureApp()
		{
			if (Session == null)
			{
				AppiumOptions appiumOptions = new AppiumOptions();

				appiumOptions.AddAdditionalCapability("platformName", "Tizen");
				//TM1
				appiumOptions.AddAdditionalCapability("deviceName", "0000d84200006200");
				//For Emul
				//appiumOptions.AddAdditionalCapability("deviceName", "emulator-26101");

				appiumOptions.AddAdditionalCapability("appPackage", "ControlGallery.Tizen");
				appiumOptions.AddAdditionalCapability("app", "ControlGallery.Tizen-1.0.0.tpk");

				Session = new TizenDriver<TizenElement>(new Uri(TizenApplicationDriverUrl), appiumOptions);
				Assert.IsNotNull(Session);
				//Reset();
			}

			return new TizenDriverApp(Session);
		}

		internal static void HandleAppClosed(Exception ex)
		{
			if (ex is InvalidOperationException && ex.Message == "Currently selected window has been closed")
			{
				Session = null;
			}
		}

		public static void Reset()
		{
			try
			{
				Debug.WriteLine($">>>>> TizenTestBase Reset");
				Session?.ResetApp();
			}
			catch (Exception ex)
			{
				HandleAppClosed(ex);
				Debug.WriteLine($">>>>> TizenTestBase ConfigureApp 49: {ex}");
				throw;
			}
		}
	}
}
