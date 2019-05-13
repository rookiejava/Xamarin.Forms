using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms.Controls;
using ElmSharp;
using Tizen.Applications;
using Tizen.NET.MaterialComponents;
#if UITEST
using Tizen.Appium;
#endif

namespace Xamarin.Forms.ControlGallery.Tizen
{
	class MainApplication : FormsApplication
	{
		internal static EvasObject NativeParent { get; private set; }

		protected override void OnCreate()
		{
			base.OnCreate();
			ThemeLoader.Initialize(DirectoryInfo.Resource);
			NativeParent = MainWindow;
			LoadApplication(new App());
		}

		static void Main(string[] args)
		{
			var app = new MainApplication();
#if !UITEST
			FormsMaps.Init("HERE", "write-your-API-key-here");
#endif
			global::Xamarin.Forms.Platform.Tizen.Forms.Init(app);
			FormsMaterial.Init();
#if UITEST
			TizenAppium.StartService(new FormsAdapter());
#endif
			app.Run(args);
		}
	}
}
