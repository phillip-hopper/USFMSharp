using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Gecko;

namespace uw_edit.UserControls
{
	public class Browser : UserControl
	{
		public event EventHandler<Gecko.Events.GeckoDocumentCompletedEventArgs> DocumentCompleted;

		public GeckoWebBrowser WebBrowser { get; private set; }

		public Browser()
		{
			InitializeControl();
		}

		void InitializeControl()
		{
			WebBrowser = new GeckoWebBrowser();

			WebBrowser.Parent = this;
			WebBrowser.Dock = DockStyle.Fill;
			Controls.Add(WebBrowser);
#if !DEBUG
			WebBrowser.NoDefaultContextMenu = true;
#endif

			// we want clicks in iframes to propagate all the way up to C#
			WebBrowser.FrameEventsPropagateToMainWindow = true;

			WebBrowser.DocumentCompleted += (sender, e) => DocumentCompleted?.Invoke(this, e);
		}

		public static void SetUpXulRunner()
		{
			if (Xpcom.IsInitialized)
				return;

			string xulRunnerPath = Environment.GetEnvironmentVariable("XULRUNNER");
			if (string.IsNullOrEmpty(xulRunnerPath) || !Directory.Exists(xulRunnerPath))
			{
				xulRunnerPath = Path.Combine(Program.GetAppDirectory(), "Firefox");
			}
			Xpcom.Initialize(xulRunnerPath);

			var errorsToHide = new List<string>
			{
				// 21 JUL 2014, PH: This is a confirmed bug in firefox (https://bugzilla.mozilla.org/show_bug.cgi?id=1020846)
				//   and is supposed to be fixed in firefox 33.
				"is being assigned a //# sourceMappingURL, but already has one"
			};

			GeckoPreferences.User["network.proxy.http"] = string.Empty;
			GeckoPreferences.User["network.proxy.http_port"] = 80;
			GeckoPreferences.User["network.proxy.type"] = 1; // 0 = direct (uses system settings on Windows), 1 = manual configuration

			// Try some settings to reduce memory consumption by the mozilla browser engine.
			// See http://kb.mozillazine.org/About:config_entries, http://www.davidtan.org/tips-reduce-firefox-memory-cache-usage
			// and http://forums.macrumors.com/showthread.php?t=1838393.
			GeckoPreferences.User["memory.free_dirty_pages"] = true;
			GeckoPreferences.User["browser.sessionhistory.max_entries"] = 0;
			GeckoPreferences.User["browser.sessionhistory.max_total_viewers"] = 0;
			GeckoPreferences.User["browser.cache.memory.enable"] = false;

			// Some more settings that can help to reduce memory consumption.
			// See http://www.instantfundas.com/2013/03/how-to-keep-firefox-from-using-too-much.html
			// and http://kb.mozillazine.org/Memory_Leak.
			// maximum amount of memory used to cache decoded images
			GeckoPreferences.User["image.mem.max_decoded_image_kb"] = 40960;        // 40MB (default = 256000 == 250MB)
																					// maximum amount of memory used by javascript
			GeckoPreferences.User["javascript.options.mem.max"] = 40960;            // 40MB (default = -1 == automatic)
																					// memory usage at which javascript starts garbage collecting
			GeckoPreferences.User["javascript.options.mem.high_water_mark"] = 20;   // 20MB (default = 128 == 128MB)
																					// SurfaceCache is an imagelib-global service that allows caching of temporary
																					// surfaces. Surfaces normally expire from the cache automatically if they go
																					// too long without being accessed.
			GeckoPreferences.User["image.mem.surfacecache.max_size_kb"] = 40960;    // 40MB (default = 102400 == 100MB)
			GeckoPreferences.User["image.mem.surfacecache.min_expiration_ms"] = 500;    // 500ms (default = 60000 == 60sec)

			// maximum amount of memory for the browser cache (probably redundant with browser.cache.memory.enable above, but doesn't hurt)
			GeckoPreferences.User["browser.cache.memory.capacity"] = 0;             // 0 disables feature

			// do these do anything?
			//GeckoPreferences.User["javascript.options.mem.gc_frequency"] = 5;	// seconds?
			//GeckoPreferences.User["dom.caches.enabled"] = false;
			//GeckoPreferences.User["browser.sessionstore.max_tabs_undo"] = 0;	// (default = 10)
			//GeckoPreferences.User["network.http.use-cache"] = false;

			// These settings prevent a problem where the gecko instance running the add page dialog
			// would request several images at once, but we were not able to generate the image
			// because we could not make additional requests of the localhost server, since some limit
			// had been reached. I'm not sure all of them are needed, but since in this program we
			// only talk to our own local server, there is no reason to limit any requests to the server,
			// so increasing all the ones that look at all relevant seems like a good idea.
			GeckoPreferences.User["network.http.max-persistent-connections-per-server"] = 200;
			GeckoPreferences.User["network.http.pipelining.maxrequests"] = 200;
			GeckoPreferences.User["network.http.pipelining.max-optimistic-requests"] = 200;

			// This suppresses the normal zoom-whole-window behavior that Gecko normally does when using the mouse while
			// while holding crtl. Code in bloomEditing.js provides a more controlled zoom of just the body.
			GeckoPreferences.User["mousewheel.with_control.action"] = 0;
		}



		//void _browser_DocumentCompleted(object sender,  e)
		//{
		//	var doc = _browser.Document;
		//	var found = doc.GetElementById("usfm-content");

		//	Console.Out.WriteLine("here");
		//}
	}
}

