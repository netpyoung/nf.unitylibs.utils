using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticDescriptors;
using System.Collections.Immutable;


namespace NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeNowAnalyzer : DiagnosticAnalyzer
    {

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(DiagnosticDescriptorCollection.NF5003); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            MemberAccessExpressionSyntax memberAccess = (MemberAccessExpressionSyntax)context.Node;

            if (memberAccess.Name.Identifier.Text == "Now" &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.Text == "DateTime")
            {
                Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptorCollection.NF5003, memberAccess.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }


}
