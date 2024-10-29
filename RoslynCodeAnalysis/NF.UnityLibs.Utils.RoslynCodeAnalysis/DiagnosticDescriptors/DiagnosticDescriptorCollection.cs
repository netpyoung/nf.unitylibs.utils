using Microsoft.CodeAnalysis;

namespace NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticDescriptors
{
    internal class DiagnosticDescriptorCollection
    {
        internal static readonly DiagnosticDescriptor NF5001 = new(
            id: "NF5001", //
            title: "Use MathUtil.Round_Normal instead of Math.Round",
            messageFormat: "Use MathUtil.Round_Normal instead of Math.Round for normal rounding. {0}", //
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Math.Round uses banker's rounding by default. Use MathUtil.Round_Normal for normal rounding."
        );
        internal static readonly DiagnosticDescriptor NF5002 = new(
            id: "NF5002", //
            title: "Incorrect TODO style",
            messageFormat: "TODO comment does not follow the required format: TODO(username): content", //
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "TODO comments should follow the format: TODO(username): content"
        );
        internal static readonly DiagnosticDescriptor NF5003 = new(
            id: "NF5003", //
            title: "Use DateTime.UtcNow instead of DateTime.Now",
            messageFormat: "Use DateTime.UtcNow instead of DateTime.Now for better performance and consistency", //
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "DateTime.Now can be slower and less consistent across time zones. Use DateTime.UtcNow instead"
        );
    }
}
