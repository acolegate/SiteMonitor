namespace HttpAgentManager.Interfaces
{
    public interface IAgent
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void Dispose();

        event Agent.WriteLineEventHandler WriteLineEvent;
        event Agent.ShowDialogEventHandler ShowDialogEvent;
        event Agent.RunProgramEventHandler RunProgramEvent;

        void Stop();

        void Start();
    }
}