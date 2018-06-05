using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{

    #region Attribute attempt
    ////////[System.SerializableAttribute()]
    ////////[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    ////////public class CharacterRegionStart
    ////////{
    ////////    public CharacterRegionStart()
    ////////    { }
    ////////    public CharacterRegionStart(string s, string d)
    ////////    {
    ////////        Value = s;
    ////////        DisplayAs = d;
    ////////    }
    ////////    private string displayAsField;

    ////////    private string valueField;


    ////////    /// <remarks/>
    ////////    //[XmlText()]
    ////////    [ContentSerializerAttribute(HasText =true)]
    ////////    public string Value
    ////////    {
    ////////        get
    ////////        {
    ////////            return this.valueField;
    ////////        }
    ////////        set
    ////////        {
    ////////            this.valueField = value;
    ////////           // if (!string.IsNullOrWhiteSpace(value) && value.Trim()!="0") throw new Exception(" in Value setter value: " + value + "  :");
    ////////        }
    ////////    }


    ////////    /// <remarks/>
    ////////    [ContentSerializerAttribute(ElementName = "Start",isAttribute = true)]
    ////////    public string DisplayAs
    ////////    {
    ////////        get
    ////////        {
    ////////            if (string.IsNullOrWhiteSpace(valueField))
    ////////                return Value;
    ////////            return this.displayAsField;
    ////////        }
    ////////        set
    ////////        {
    ////////            this.displayAsField = value;
    ////////            if (!string.IsNullOrWhiteSpace(value)) throw new Exception(" in displayas setter value: " + value + "  :");
    ////////        }
    ////////    }

    ////////}
#endregion
    // Describes a range of consecutive characters that should be included in the font.
    //[TypeConverter(typeof(CharacterRegionTypeConverter))]
    public struct CharacterRegion
    {
        //////[ContentSerializerAttribute(ElementName = "Start", Optional = false)]
        //////public CharacterRegionStart StartClass
        //////{
        //////    get
        //////    {

        //////        return characterRegionStart;
        //////    }
        //////    set
        //////    {
        //////        characterRegionStart = value;
        //////        //Start = characterRegionStart.Value[0];
        //////        if (!uint.TryParse(characterRegionStart.DisplayAs, out display))
        //////            display = (uint)Start + 32;
        //////    }

        //////}

        //////private CharacterRegionStart characterRegionStart;




        //[ContentSerializerIgnore]

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

        private char start;
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
        public CharacterRegion(char start, char end, uint displayAs =0)
        {
            if (start > end)
                throw new ArgumentException();

            //characterRegionStart = new CharacterRegionStart(start.ToString(), displayAs);
            this.start = start;

            // Store start in display --redundant but required
            display = start;

            End = end;
            Start = start;
            DisplayAs = displayAs;

            //override replace display with start if unassigned
            if (displayAs==0)
            {
                display = Start;
                displayAs = Start;
            }





        }

        // Default to just the base ASCII character set.
        public static CharacterRegion Default = new CharacterRegion(' ', '~');


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