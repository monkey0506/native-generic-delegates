using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static class IDictionaryExtensions
    {
        [return: MaybeNull]
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> _this, TKey key)
            where TValue : new()
        {
            return GetOrCreate(_this, key, () => new())!;
        }

        [return: MaybeNull]
        public static TValue GetOrCreate<TKey, TValue>
        (
            this IDictionary<TKey, TValue> _this,
            TKey key,
            Func<TValue> valueCreator
        )
        {
            ArgumentNullExceptionHelper.ThrowIfNull(_this);
            ArgumentNullExceptionHelper.ThrowIfNull(key);
            ArgumentNullExceptionHelper.ThrowIfNull(valueCreator);
            if (_this.TryGetValue(key, out var value))
            {
                return value;
            }
            value = valueCreator();
            _this[key] = value!;
            return value;
        }
    }
}
