using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticAnalyzers;
using NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticDescriptors;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NF.UnityLibs.Utils.RoslynCodeAnalysis.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TodoStyleCodeFixProvider)), Shared]
    public class TodoStyleCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get => ImmutableArray.Create(DiagnosticDescriptorCollection.NF5002.Id);
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            SyntaxTrivia trivia = root.FindTrivia(diagnosticSpan.Start);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Fix TODO format",
                    createChangedDocument: c => FixTodoFormat(context.Document, trivia, c),
                    equivalenceKey: nameof(CodeFixProvider)),
                diagnostic);
        }

        private async Task<Document> FixTodoFormat(Document document, SyntaxTrivia trivia, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);
            string newTriviaText = "// TODO(username): " + trivia.ToString().Replace("TODO", "").Trim();
            SyntaxTrivia newTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, newTriviaText);
            SyntaxNode newRoot = root.ReplaceTrivia(trivia, newTrivia);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
