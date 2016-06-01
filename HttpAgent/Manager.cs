using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Timers;

namespace HttpAgentManager
{
    public class Manager : IDisposable
    {
        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

        private readonly List<Agent> _agents;
        private readonly Config _config;

        public Manager(Config config)
        {
            _config = config;

            _agents = new List<Agent>(config.Sites.Count);

            for (int i = 0; i < config.Sites.Count; i++)
            {
                Config.Site site = config.Sites[i];

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                httpClient.Timeout = site.Timeout;

                Timer timer = new Timer
                                  {
                                      AutoReset = false,
                                      Interval = site.CheckFrequency.TotalMilliseconds
                                  };

                _agents.Add(new Agent(i + 1, site, httpClient, timer));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (Agent agent in _agents)
            {
                agent.Stop();
                agent.WriteLineEvent -= Agent_WriteLineEvent;
                agent.RunProgramEvent -= Agent_RunProgramEvent;
                agent.ShowDialogEvent -= Agent_ShowDialogEvent;
            }
        }

        internal void Agent_WriteLineEvent(object sender, Agent.WriteLineEventArgs e)
        {
            RaiseWriteLineEvent(string.Format("Reply from agent {0} about url {1} : {2}", e.AgentId, e.Site.Url, e.Message));
        }

        internal void Agent_ShowDialogEvent(object sender, Agent.ShowDialogEventArgs e)
        {
            RaiseShowDialogEvent(e.Message);
        }

        internal void Agent_RunProgramEvent(object sender, Agent.RunProgramEventArgs e)
        {
            RaiseRunProgramEvent(e.Message);
        }

        public void Start()
        {
            foreach (Agent agent in _agents)
            {
                agent.Start();
                agent.WriteLineEvent += Agent_WriteLineEvent;
                agent.RunProgramEvent += Agent_RunProgramEvent;
                agent.ShowDialogEvent += Agent_ShowDialogEvent;
            }
        }

        #region events

        public delegate void RunProgramEventHandler(object sender, RunProgramEventArgs e);

        public delegate void ShowDialogEventHandler(object sender, ShowDialogEventArgs e);

        public delegate void WriteLineEventHandler(object sender, WriteLineEventArgs e);

        public event WriteLineEventHandler WriteLineEvent;
        public event ShowDialogEventHandler ShowDialogEvent;
        public event RunProgramEventHandler RunProgramEvent;

        internal void RaiseWriteLineEvent(string message)
        {
            WriteLineEvent?.Invoke(this, new WriteLineEventArgs(message));
        }

        internal void RaiseShowDialogEvent(string message)
        {
            ShowDialogEvent?.Invoke(this, new ShowDialogEventArgs(message));
        }

        internal void RaiseRunProgramEvent(string message)
        {
            RunProgramEvent?.Invoke(this, new RunProgramEventArgs(message));
        }

        public class WriteLineEventArgs : EventArgs
        {
            public WriteLineEventArgs(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        public class ShowDialogEventArgs : EventArgs
        {
            public ShowDialogEventArgs(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        public class RunProgramEventArgs : EventArgs
        {
            public RunProgramEventArgs(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        #endregion
    }
}