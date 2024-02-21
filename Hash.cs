using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static class Hash
    {
        private static int GetHashCode(ICollection collection)
        {
            int[] hashCodes = new int[collection.Count];
            int i = -1;
            foreach (var element in collection)
            {
                hashCodes[++i] = GetHashCode(element);
            }
            return CombineHashCodes(hashCodes);
        }

        private static int GetHashCode(IEnumerable enumerable)
        {
            var hashCodes = new List<int>();
            foreach (var element in enumerable)
            {
                hashCodes.Add(GetHashCode(element));
            }
            return CombineHashCodes([.. hashCodes]);
        }

        private static int GetHashCode<T>(T value)
        {
            return value switch
            {
                ISymbol symbol => SymbolEqualityComparer.Default.GetHashCode(symbol),
                ICollection collection => GetHashCode(collection),
                IEnumerable enumerable => GetHashCode(enumerable),
                not null => value.GetHashCode(),
                _ => 0
            };
        }

        /// <summary>
        /// Combines the provided hashCode codes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Adapted from <see href="https://stackoverflow.com/a/1646913">Quick and Simple Hash Code Combinations - Stack
        /// Overflow</see> answer by user <see href="https://stackoverflow.com/users/22656/jon-skeet">Jon Skeet</see>, licensed
        /// under <see href="https://creativecommons.org/licenses/by-sa/2.5/">CC BY-SA 2.5</see>. Changes have been made to
        /// permit a variable number of hashCode codes.
        /// </para>
        /// </remarks>
        /// <param name="hashCodes">The hashCode codes to combine.</param>
        /// <returns>The hashCode value generated from the provided hashCode codes.</returns>
        private static int CombineHashCodes(params int[] hashCodes)
        {
            unchecked
            {
                int hash = 17;
                foreach (var hashToCombine in hashCodes)
                {
                    hash = hash * 31 + hashToCombine;
                }
                return hash;
            }
        }

        public static int Combine<T>(T t) => GetHashCode(t);

        public static int Combine<T1, T2>(T1 t1, T2 t2)
        {
            return CombineHashCodes(GetHashCode(t1), GetHashCode(t2));
        }

        public static int Combine<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return CombineHashCodes(GetHashCode(t1), GetHashCode(t2), GetHashCode(t3));
        }

        public static int Combine<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return CombineHashCodes(GetHashCode(t1), GetHashCode(t2), GetHashCode(t3), GetHashCode(t4));
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return CombineHashCodes(GetHashCode(t1), GetHashCode(t2), GetHashCode(t3), GetHashCode(t4), GetHashCode(t5));
        }

        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return CombineHashCodes
            (
                GetHashCode(t1),
                GetHashCode(t2),
                GetHashCode(t3),
                GetHashCode(t4),
                GetHashCode(t5),
                GetHashCode(t6)
            );
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return CombineHashCodes
            (
                GetHashCode(t1),
                GetHashCode(t2),
                GetHashCode(t3),
                GetHashCode(t4),
                GetHashCode(t5),
                GetHashCode(t6),
                GetHashCode(t7)
            );
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return CombineHashCodes
            (
                GetHashCode(t1),
                GetHashCode(t2),
                GetHashCode(t3),
                GetHashCode(t4),
                GetHashCode(t5),
                GetHashCode(t6),
                GetHashCode(t7),
                GetHashCode(t8)
            );
        }
    }
}
