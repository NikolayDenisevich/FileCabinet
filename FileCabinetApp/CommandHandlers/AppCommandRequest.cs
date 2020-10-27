namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides command request.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">Command name.</param>
        /// <param name="parameters">Commad parameters.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets command value.
        /// </summary>
        /// <value>
        /// Command value.
        /// </value>
        public string Command { get; }

        /// <summary>
        /// Gets command parameters value.
        /// </summary>
        /// <value>
        /// Command parameters value.
        /// </value>
        public string Parameters { get; }
    }
}