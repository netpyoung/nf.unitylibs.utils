using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticDescriptors;
using System.Collections.Immutable;
using System.Linq;

namespace NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NodiscardAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(DiagnosticDescriptorCollection.NF5004);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation.Parent is not ExpressionStatementSyntax)
            {
                return;
            }

            IMethodSymbol symbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (symbol == null)
            {
                return;
            }

            if (symbol.ReturnsVoid)
            {
                return;
            }

            INamedTypeSymbol nodiscardAttrSymbol = context.Compilation.GetTypeByMetadataName("NF.UnityLibs.Utils.NodiscardAttribute");
            if (nodiscardAttrSymbol == null)
            {
                return;
            }

            bool isNodiscard = symbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, nodiscardAttrSymbol));
            if (!isNodiscard)
            {
                return;
            }

            Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptorCollection.NF5004, invocation.GetLocation(), symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
