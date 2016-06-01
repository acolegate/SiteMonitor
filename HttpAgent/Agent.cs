using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Timers;

using HttpAgentManager.Interfaces;

namespace HttpAgentManager
{
    public class Agent : IDisposable, IAgent
    {
        private const string SiteValuesFileExtension = ".sitevalue";
        private static readonly string CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private readonly int _agentId;
        private readonly HttpClient _client;
        private readonly Config.Site _site;
        private readonly Timer _timer;
        private Regex _regex;

        public Agent(int agentId, Config.Site site, HttpClient httpClient, Timer timer)
        {
            _agentId = agentId;
            _site = site;

            _client = httpClient;
            _timer = timer;

            _regex = new Regex(site.RegexPattern, RegexOptions.Compiled);
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
        }

        public void Start()
        {
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _timer.Stop();
            _client.Dispose();
            _regex = null;
        }

        internal void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            ProcessSite();

            // DEBUG - uncomment this to allow repeated hits
            //_timer.Start();
        }

        internal void ProcessSite()
        {
            using (HttpResponseMessage response = _client.GetAsync(_site.Url).Result)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string html = response.Content.ReadAsStringAsync().Result;
                    ProcessHtml(html);
                }
                else
                {
                    RaiseWriteLineEvent(string.Format("Received code {0}", (int)response.StatusCode));
                }
            }
        }

        internal void ProcessHtml(string html)
        {
            foreach (Match match in _regex.Matches(html))
            {
                if (HelperFunctions.EvaluateTest(_site.Test, match.Groups[_site.Test.CaptureGroup].Value))
                {
                    // test passed
                    if (EvaluateValueChanged(match.Groups[_site.CaptureGroupToRecord].Value))
                    {
                        ProcessAction(_site.ActionOnTestValueChange, match);
                    }
                }
            }
        }

        internal bool EvaluateValueChanged(string capturedValue)
        {
            string pathAndFilename = CurrentDirectory + @"\" + _site.UniqueMd5 + SiteValuesFileExtension;

            string previousValue = null;

            if (File.Exists(pathAndFilename))
            {
                using (StreamReader streamReader = new StreamReader(pathAndFilename))
                {
                    previousValue = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }

            // Write the new capturedvalue

            using (TextWriter textWriter = new StreamWriter(pathAndFilename, false))
            {
                textWriter.Write(capturedValue);
                textWriter.Flush();
                textWriter.Close();
            }

            return capturedValue != previousValue;
        }

        internal void ProcessAction(Config.Action action, Match match)
        {
            switch (action.Type)
            {
                case Config.ActionType.RunProgram:
                    {
                        RaiseRunProgramEvent("Run Program");
                        break;
                    }
                case Config.ActionType.ShowDialog:
                    {
                        RaiseShowDialogEvent("Show dialog");
                        break;
                    }
                case Config.ActionType.WriteLine:
                    {
                        RaiseWriteLineEvent("Write line");
                        break;
                    }
                default:

                    throw new ArgumentOutOfRangeException();
            }
        }

        #region events

        public delegate void RunProgramEventHandler(object sender, RunProgramEventArgs e);

        public delegate void ShowDialogEventHandler(object sender, ShowDialogEventArgs e);

        public delegate void WriteLineEventHandler(object sender, WriteLineEventArgs e);

        internal void RaiseWriteLineEvent(string message)
        {
            WriteLineEvent?.Invoke(this, new WriteLineEventArgs(_agentId, _site, message));
        }

        internal void RaiseRunProgramEvent(string message)
        {
            RunProgramEvent?.Invoke(this, new RunProgramEventArgs(_agentId, _site, message));
        }

        internal void RaiseShowDialogEvent(string message)
        {
            ShowDialogEvent?.Invoke(this, new ShowDialogEventArgs(_agentId, _site, message));
        }

        public event WriteLineEventHandler WriteLineEvent;
        public event RunProgramEventHandler RunProgramEvent;
        public event ShowDialogEventHandler ShowDialogEvent;

        public class WriteLineEventArgs : EventArgs
        {
            public WriteLineEventArgs(int agentId, Config.Site site, string message)
            {
                AgentId = agentId;
                Site = site;
                Message = message;
            }

            public int AgentId { get; set; }
            public Config.Site Site { get; set; }
            public string Message { get; }
        }

        public class RunProgramEventArgs : EventArgs
        {
            public RunProgramEventArgs(int agentId, Config.Site site, string message)
            {
                AgentId = agentId;
                Site = site;
                Message = message;
            }

            public int AgentId { get; set; }
            public Config.Site Site { get; set; }
            public string Message { get; }
        }

        public class ShowDialogEventArgs : EventArgs
        {
            public ShowDialogEventArgs(int agentId, Config.Site site, string message)
            {
                AgentId = agentId;
                Site = site;
                Message = message;
            }

            public int AgentId { get; set; }
            public Config.Site Site { get; set; }
            public string Message { get; }
        }

        #endregion
    }
}