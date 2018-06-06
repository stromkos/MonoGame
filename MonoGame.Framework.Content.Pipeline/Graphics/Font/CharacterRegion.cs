using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    ///  Describes a range of consecutive characters that should be included in the font.
    /// </summary>
    //[TypeConverter(typeof(CharacterRegionTypeConverter))]
    public struct CharacterRegion
    {
        [ContentSerializerAttribute(Optional = true)]
        public uint DisplayAs
        {
            get { return display; }
            set
            {
                if (value == 0)
                    display = Start;
                else
                    display = value;
            }
        }
        public char Start
        {
            get { return start; }
            set
            {
                start = value;
                if (display == 0)
                    display = value;
            }
        }
        public char End;
        private char start;
        private uint display;

        // Enumerates all characters within the region. 
        public Dictionary<char, uint> Characters()
        {
            Dictionary<char, uint> o = new Dictionary<char, uint>();
            // run concurrent for loop, c = char d = uint
            uint d = display - 1;
            for (var c = Start; c <= End; c++)
            {
                d++;
                o.Add(c, d);
            }
            return o;
        }

        // Constructor.
        public CharacterRegion(char start, char end, uint displayAs = 0)
        {
            if (start > end)
                throw new ArgumentException();
            this.start = start;
            display = start;

            End = end;
            Start = start;
            DisplayAs = displayAs;

            //override replace display with start if unassigned
            if (displayAs == 0)
            {
                display = Start;
                displayAs = Start;
            }
        }

        // Default to just the base ASCII character set.
        public static CharacterRegion Default = new CharacterRegion(' ', '~', 32);

        /// <summary>
        /// Test if there is an element in this enumeration.
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="source">The enumerable source.</param>
        /// <returns><c>true</c> if there is an element in this enumeration, <c>false</c> otherwise</returns>
        public static bool Any<T>(IEnumerable<T> source)
        {
            return source.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Select elements from an enumeration.
        /// </summary>
        /// <typeparam name="TSource">The type of the T source.</typeparam>
        /// <typeparam name="TResult">The type of the T result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>A enumeration of selected values</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (TSource sourceItem in source)
            {
                foreach (TResult result in selector(sourceItem))
                    yield return result;
            }
        }

        /// <summary>
        /// Selects distinct elements from an enumeration.
        /// </summary>
        /// <typeparam name="TSource">The type of the T source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>A enumeration of selected values</returns>
        public static IEnumerable<TSource> Distinct<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            // using Dictionary is not really efficient but easy to implement
            var values = new Dictionary<TSource, object>(comparer);
            foreach (TSource sourceItem in source)
            {
                if (!values.ContainsKey(sourceItem))
                {
                    values.Add(sourceItem, null);
                    yield return sourceItem;
                }
            }
        }
    }
}
