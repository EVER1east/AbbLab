using System;
using System.Collections.Generic;
using System.Text;
using AbbLab.Parsing;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public partial struct PartialComponent
    {
        /// <summary>
        ///   <para>Gets the partial version component that represents the specified wildcard <paramref name="character"/>.</para>
        /// </summary>
        /// <param name="character">The partial version component wildcard: <c>'x'</c>, <c>'X'</c> or <c>'*'</c>.</param>
        /// <returns>The partial version component that represents the specified wildcard <paramref name="character"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="character"/> is not a wildcard character.</exception>
        [Pure] public static PartialComponent Parse(char character) => character switch
        {
            'x' => X, 'X' => CapitalX, '*' => Star,
            _ => throw new ArgumentException("The specified character is not a wildcard character.", nameof(character)),
        };
        [Pure] public static bool TryParse(char character, out PartialComponent component)
        {
            switch (character)
            {
                case 'x': component = X;
                    return true;
                case 'X': component = CapitalX;
                    return true;
                case '*': component = Star;
                    return true;
                default: component = default;
                    return false;
            }
        }







    }
}
