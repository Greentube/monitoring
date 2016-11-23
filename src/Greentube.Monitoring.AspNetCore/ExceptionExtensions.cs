using System;

namespace Greentube.Monitoring.AspNetCore
{
    internal static class ExceptionExtensions
    {
        internal static string ToDisplayableString(this Exception exception)
        {
            var baseException = exception.GetBaseException();
            return $"{baseException.GetType()}: {baseException.Message}";
        }
    }
}