using System;
using System.Diagnostics.CodeAnalysis;

namespace Greentube.Monitoring.Threading
{
    /// <summary>
    /// An abstraction for <see cref="System.Threading.Timer"/>
    /// </summary>
    /// <remarks>
    /// To enable Unit testing on code depending on Timer
    /// More members of Timer can be added as needed
    /// </remarks>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="System.Threading.Timer" />
    internal interface ITimer : IDisposable
    {
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Part of the Timer signature")]
        bool Change(TimeSpan dueTime, TimeSpan period);
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Part of the Timer signature")]
        bool Change(int dueTime, int period);
    }
}
