using ServerSync.Model.Configuration;
using ServerSync.Model.State;

namespace ServerSync.Model.Actions
{
    /// <summary>
    /// Interface for all actions that can be executed during as sync job
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// The name of the action
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Specifies whether the action is enabled. Is not, they action will be skipped during execution
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// The name of the filter to apply to the sync state before executing the action
        /// </summary>
        string InputFilterName { get; }

        /// <summary>
        /// The <see cref="ISyncConfiguration"/> the action is part of
        /// </summary>
        ISyncConfiguration Configuration { get; }

        /// <summary>
        /// The sync state the action operates on
        /// </summary>
        ISyncState State { get; set; }

        /// <summary>
        /// Executes the action
        /// </summary>
        void Run();
    }
}
