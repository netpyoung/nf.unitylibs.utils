using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NF.UnityLibs.Utils.RoslynCodeAnalysis.DiagnosticDescriptors;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NF.UnityLibs.Utils.RoslynCodeAnalysis.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DateTimeNowCodeFixProvider)), Shared]
    public class DateTimeNowCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get => ImmutableArray.Create(DiagnosticDescriptorCollection.NF5003.Id);
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
            MemberAccessExpressionSyntax memberAccess = NewMethod(root, diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use DateTime.UtcNow",
                    createChangedDocument: c => ReplaceDateTimeNowWithUtcNow(context.Document, memberAccess, c),
                    equivalenceKey: nameof(CodeFixProvider)),
                diagnostic);
        }

        private static MemberAccessExpressionSyntax NewMethod(SyntaxNode root, TextSpan diagnosticSpan)
        {
            return root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
        }

        private async Task<Document> ReplaceDateTimeNowWithUtcNow(Document document, MemberAccessExpressionSyntax memberAccess, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);
            MemberAccessExpressionSyntax newMemberAccess = memberAccess.WithName(SyntaxFactory.IdentifierName("UtcNow"));
            SyntaxNode newRoot = root.ReplaceNode(memberAccess, newMemberAccess);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
