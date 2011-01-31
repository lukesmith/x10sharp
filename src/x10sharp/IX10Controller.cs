using System;

namespace x10sharp
{
    public interface IX10Controller
    {
        event EventHandler<EventArgs> ControllerReady;

        event EventHandler<EventArgs> CommandSent;
        
        event EventHandler<DataEventArgs> DataReceived;
        
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        byte[] CreateCommand(X10Command command);

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The <see cref="X10Command"/> command.</param>
        /// <returns></returns>
        bool SendCommand(X10Command command);
        
        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="commandSet">The <see cref="X10Commandset"/> command set.</param>
        /// <returns></returns>
        bool SendCommand(X10CommandSet commandSet);

        /// <summary>
        /// Configures the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="ConnectionSettings"/> settings.</param>
        void Configure(ConnectionSettings settings);

        /// <summary>
        /// Runs the macro.
        /// </summary>
        /// <param name="macro">The <see cref="Macro"/> macro.</param>
        /// <returns></returns>
        bool RunMacro(Macro macro);
    }
}
