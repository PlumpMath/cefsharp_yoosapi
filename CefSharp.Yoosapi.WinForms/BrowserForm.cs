// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using CefSharp;
using CefSharp.MinimalExample.WinForms.Controls;
using CefSharp.WinForms;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace CefSharp.MinimalExample.WinForms
{
    public partial class BrowserForm : Form
    {
        private readonly ChromiumWebBrowser browser;
        public const string homeUrl = "https://yoosapi.net/introduction";
        string[] historyArray = new string[] { };
        private string showHistory;
        string[] bookmarksArray = new string[] { };
        private string showBookmarks;


        public BrowserForm()
        {
            InitializeComponent();

            Text = "Yoosapi Browser";
            WindowState = FormWindowState.Maximized;

            browser = new ChromiumWebBrowser(homeUrl)
            {
                Dock = DockStyle.Fill,
            };
            toolStripContainer.ContentPanel.Controls.Add(browser);

            //Add events to browser view
            browser.LoadingStateChanged += OnLoadingStateChanged;
            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;
            browser.DownloadHandler = new DownloaderHandler();
            
            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = String.Format("CEF: {0}, CefSharp: {1}, Environment: {2}",  Cef.CefVersion, Cef.CefSharpVersion, bitness);
            DisplayOutput(version);
        }

        //Check if localstorage exists
        private void CheckBrowserLocalStorage()
        {

            // Step 03: Execute some ad-hoc JS that returns an object back to C#
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("function tempFunction() {");
            //sb.AppendLine("     // create a JS object");
            //sb.AppendLine("     var person = {firstName:'Sima', lastName:'Simic', age:23};");
            //sb.AppendLine("");
            //sb.AppendLine("     // Important: convert object to string before returning to C#");
            //sb.AppendLine("     return JSON.stringify(person);");
            //sb.AppendLine("}");
            //sb.AppendLine("tempFunction();");


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function appendLocalStorage() {");
            sb.AppendLine("if (localStorage.getItem('history')) {");
            sb.AppendLine("console.log('CheckBrowserLocalStorage:'+localStorage.getItem('history'));");
            sb.AppendLine("return localStorage.getItem('history');");
            sb.AppendLine("}");
            sb.AppendLine("return false;");
            sb.AppendLine("}");
            sb.AppendLine("appendLocalStorage();");

            var task = browser.EvaluateScriptAsync(sb.ToString());

            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    // Step 04: Recieve value from JS
                    var response = t.Result;

                    if (response.Success == true)
                    {
                        // Use JSON.net to convert to object;
                        MessageBox.Show(response.Result.ToString());
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            AddToHistory(args.Address);

            //showHistory = null;

            //foreach (string item in historyArray)
            //{
            //    showHistory += "<div><a href=" + item + ">" + item + "</a></div>";
            //}

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("function tempFunction() {");
            //sb.AppendLine("localStorage.setItem('history', '" + showHistory + "')");
            //sb.AppendLine("console.log('log:'+localStorage.getItem('history'));");
            //sb.AppendLine("}");
            //sb.AppendLine("tempFunction();");

            //browser.EvaluateScriptAsync(sb.ToString());

            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";

            //goButton.Image = isLoading ?
            //    Properties.Resources.nav_plain_red :
            //    Properties.Resources.nav_plain_green;

            HandleToolStripLayout();
        }

        public void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            //var width = toolStrip1.Width;
            //foreach (ToolStripItem item in toolStrip1.Items)
            //{
            //    if (item != urlTextBox)
            //    {
            //        width -= item.Width - item.Margin.Horizontal;
            //    }
            //}
            //urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {

            //showHistory = null;

            //foreach (string item in historyArray)
            //{
            //    showHistory += "<div><a href=" + item + ">" + item + "</a></div>";
            //}

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function tempFunction() {");
            sb.AppendLine("localStorage.setItem('history', '" + historyArray + "')");
            sb.AppendLine("localStorage.setItem('bookmarks', '" + bookmarksArray+ "')");
            sb.AppendLine("}");
            sb.AppendLine("tempFunction();");

            browser.EvaluateScriptAsync(sb.ToString());

            browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
            //DateTime thisDay = DateTime.Today;
            //string indexDate = thisDay.ToString();
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);

        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {

                browser.Load(url);

            }
        }

        private void AddToHistory(string urlAddress)
        {
            Array.Resize(ref historyArray, historyArray.Length + 1);
            historyArray[historyArray.Length - 1] = urlAddress;
        }

        private void AddToBookmarks(string urlAddress)
        {
            Array.Resize(ref bookmarksArray, bookmarksArray.Length + 1);
            bookmarksArray[bookmarksArray.Length - 1] = urlAddress;
        }

        private void urlTextBox_Click(object sender, EventArgs e)
        {

        }


        private void devConsoleToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadUrl(homeUrl);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHistory = null;

            foreach (string item in historyArray)
            {
                showHistory += "<div><a href='"+item+"'>" + item + "</a></div>";
            }
            
            browser.LoadHtml("<html><body><h1>History</h1><p>"+showHistory+"</p></body></html>", "http://customrenderinhistory");
        }

        private void bookmarkThisPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToBookmarks(urlTextBox.Text);
            MessageBox.Show("Bookmark saved!");
        }

        private void bookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBookmarks = null;

            foreach (string item in bookmarksArray)
            {
                showBookmarks += "<div><a href='" + item + "'>" + item + "</a></div>";
            }

            browser.LoadHtml("<html><body><h1>Bookmarks</h1><p>" + showBookmarks + "</p></body></html>", "http://customrenderinbookmarks");
        }

        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //Check for localstorage(history and bookmarks) and append to local array
            CheckBrowserLocalStorage();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
