// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Windows.Forms;

namespace CefSharp.MinimalExample.WinForms
{
    public class Program
    {
        public const string google_api_key = "AIzaSyCX5BqFRCI46zhH-PHMGo5L7ELahotKfNo";
        public const string google_default_client_id = "97384854465-i3533n9tq5895gn7ctkid5p042qv7mt0.apps.googleusercontent.com";
        public const string google_default_client_secret = "p-cQAqvBzoY7-Ur3oY9NoUHo";

        [STAThread]
        public static void Main()
        {
            // Set Google API keys, used for Geolocation requests sans GPS.  See http://www.chromium.org/developers/how-tos/api-keys
            // This will enable microphone and webcam to work. See https://console.cloud.google.com/iam-admin/serviceaccounts/project?project=flash-crawler-138718
            Environment.SetEnvironmentVariable("GOOGLE_API_KEY", google_api_key);
            Environment.SetEnvironmentVariable("GOOGLE_DEFAULT_CLIENT_ID", google_default_client_id);
            Environment.SetEnvironmentVariable("GOOGLE_DEFAULT_CLIENT_SECRET", google_default_client_secret);

            var settings = new CefSettings();
            settings.EnableInternalPdfViewerOffScreen();
            settings.WindowlessRenderingEnabled = true;
            var osVersion = Environment.OSVersion;

            settings.CachePath = "cache";
            // ENABLE Microphone, video, flash
            //settings.CefCommandLineArgs.Add("enable-system-flash", "1");
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("enable-speech-input", "1");
            // settings.CefCommandLineArgs.Add("ppapi-flash-path", "1");
            //settings.CefCommandLineArgs.Add("enable-npapi", "1");
            //settings.CefCommandLineArgs.Add("disable-bundled-ppapi-flash", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, shutdownOnProcessExit: false, performDependencyCheck: true);

            var browser = new BrowserForm();
            Application.Run(browser);
        }
    }
}
