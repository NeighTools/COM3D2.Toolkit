// --------------------------------------------------
// CM3D2.Toolkit - Extensions.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;

namespace COM3D2.Toolkit.Native
{
    internal static class Extensions
    {
        public static IEnumerable<U> Flatten<T, U>(
            T element,
            Func<T, IEnumerable<T>> elementSelector,
            Func<T, IEnumerable<U>> valueSelector)
        {
            foreach (U node in valueSelector(element))
                yield return node;
            foreach (T subElem in elementSelector(element))
                foreach (U node in Flatten(subElem, elementSelector, valueSelector))
                    yield return node;
        }

        public static IEnumerable<U> Flatten<T, U>(T element, Func<T, T> elementSelector, Func<T, IEnumerable<U>> valueSelector)
        {
            foreach (U node in valueSelector(element))
                yield return node;
            foreach (U node in Flatten(elementSelector(element), elementSelector, valueSelector))
                yield return node;
        }
    }
}
