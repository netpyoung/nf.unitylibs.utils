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
    public class NodiscardCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get => ImmutableArray.Create(DiagnosticDescriptorCollection.NF5004.Id);
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
            InvocationExpressionSyntax invocation = root.FindNode(diagnosticSpan) as InvocationExpressionSyntax;
            if (invocation == null)
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Discard return value explicitly (_ = ...)",
                    createChangedDocument: c => AddDiscardAssignmentAsync(context.Document, invocation, c),
                    equivalenceKey: nameof(CodeFixProvider)),
                diagnostic);
        }

        private async Task<Document> AddDiscardAssignmentAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null)
            {
                return document;
            }

            if (invocation.Parent is not ExpressionStatementSyntax exprStmt)
            {
                return document;
            }

            InvocationExpressionSyntax cleanInvocation = invocation.WithoutLeadingTrivia().WithoutTrailingTrivia();
            ExpressionStatementSyntax discardAssignment =
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName("_"),
                        cleanInvocation));

            discardAssignment = discardAssignment
                .WithLeadingTrivia(exprStmt.GetLeadingTrivia())
                .WithTrailingTrivia(exprStmt.GetTrailingTrivia());

            SyntaxNode newRoot = root.ReplaceNode(exprStmt, discardAssignment);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
