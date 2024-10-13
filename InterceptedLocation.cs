using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal readonly struct InterceptedLocation : IEquatable<InterceptedLocation>
    {
        private readonly int hashCode;

        public string AttributeSourceText { get; }
        public int Character { get; }
        public string FilePath { get; }
        public int Line { get; }

        public static bool operator ==(InterceptedLocation left, InterceptedLocation right) => left.Equals(right);
        public static bool operator !=(InterceptedLocation left, InterceptedLocation right) => !(left == right);

        public InterceptedLocation(InvocationExpressionSyntax invocationExpression)
        {
            var methodNode = ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name;
            var linePosition = methodNode.GetLocation().GetLineSpan().Span.Start;
            Character = linePosition.Character + 1;
            FilePath = invocationExpression.SyntaxTree.FilePath;
            Line = linePosition.Line + 1;
            AttributeSourceText = $"[InterceptsLocation(@\"{FilePath}\", {Line}, {Character})]";
            hashCode = Hash.Combine(Character, FilePath, Line);
        }

        public override bool Equals(object? obj) => obj is InterceptedLocation other && Equals(other);
        public bool Equals(InterceptedLocation other) => (Character == other.Character) &&
            (FilePath == other.FilePath) && (Line == other.Line);
        public override int GetHashCode() => hashCode;
    }
}
