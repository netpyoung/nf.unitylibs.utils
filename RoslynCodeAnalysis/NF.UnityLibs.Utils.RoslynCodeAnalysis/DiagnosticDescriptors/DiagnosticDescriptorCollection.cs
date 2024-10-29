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
            isEnabledByDefault: true
            //description: "Math.Round uses banker's rounding by default. Use MathUtil.Round_Normal for normal rounding."
        );
    }
}
