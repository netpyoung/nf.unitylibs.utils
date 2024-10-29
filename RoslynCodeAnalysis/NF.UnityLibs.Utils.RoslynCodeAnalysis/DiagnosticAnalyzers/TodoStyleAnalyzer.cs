using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticDescriptors;

namespace NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticAnalyzers
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TodoStyleAnalyzer : DiagnosticAnalyzer
    {

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(DiagnosticDescriptorCollection.NF5002); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetRoot(context.CancellationToken);
            IEnumerable<SyntaxTrivia> todoComments = root.DescendantTrivia()
                .Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) || t.IsKind(SyntaxKind.MultiLineCommentTrivia))
                .Where(t => t.ToString().Contains("TODO"));

            foreach (SyntaxTrivia comment in todoComments)
            {
                if (!IsValidTodoFormat(comment.ToString()))
                {
                    Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptorCollection.NF5002, comment.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool IsValidTodoFormat(string comment)
        {
            Regex regex = new Regex(@"\/\/\s*TODO\([\w]+\):\s*.+");
            return regex.IsMatch(comment);
        }
    }
}
