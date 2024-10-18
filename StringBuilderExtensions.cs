using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder TrimEnd(this StringBuilder sb)
        {
            int i = sb.Length - 1;
            for ( ; (i >= 0) && char.IsWhiteSpace(sb[i]); --i) { }
            sb.Length = i + 1;
            return sb;
        }
    }
}
