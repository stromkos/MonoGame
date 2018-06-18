using System;
using System.Collections.Generic;


namespace Microsoft.Xna.Framework.Utilities
{
    /// <summary>
    /// Storage struct for multibyte characters
    /// </summary>
    public struct CharEx:IConvertible,IComparable<CharEx>,IComparable<Char>, IEquatable<CharEx>, IEquatable<Char>
    {
        internal int _value;


        #region Constructors
        public CharEx(int UTF32)
        {
            _value = UTF32;
        }
        public CharEx(char High,char Low)
        {
            _value = char.ConvertToUtf32(High, Low);
        }
        public CharEx(char UTF16)
        {
            if (char.IsSurrogate(UTF16))
                throw new ArgumentOutOfRangeException();
            _value = (int)UTF16;
        }
        
        public CharEx(string UTF32)
        {
            
            if ((new System.Globalization.StringInfo(UTF32)).LengthInTextElements != 1)
            {
                // check for xml encoding
                // use signed 64bit values to handle overflow cases and signed values
                if (UTF32.StartsWith("&#x"))
                {
                    _value = (int)long.Parse(UTF32.Substring(3), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                    if (UTF32.StartsWith("&#"))
                {
                    _value = (int)long.Parse(UTF32.Substring(2),System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                }
                throw new ArgumentOutOfRangeException();
            }
            _value = char.ConvertToUtf32(UTF32,0);
        }
        #endregion

        #region Instance Methods
        public bool isASCII()
        {
            return _value <= 0x7F;
        }
        public bool isExtendedASCII()
        {
            return _value <= 0xFF;
        }
        public bool isUTF16()
        {
            return _value <= 0xFFFF;
        }
        public bool isUTF32()
        {
            return _value > 0xFFFF;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Convert a string to CharEx array
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <returns></returns>
        public static CharEx[] ToArray(String s)
        {
            // Allocate list to simplify output
            List<CharEx> tmpList = new List<CharEx>(s.Length * 2);
            for (int i = 0; i < s.Length; i++)
            {
                if (!Char.IsSurrogate(s[i]))
                    tmpList.Add(new CharEx(s[i]));
                else
                {
                    if (i == s.Length - 1)
                        throw new ArgumentOutOfRangeException("Unmatched surrogate at end of string");
                    tmpList.Add(new CharEx(s[i++], s[i]));
                }
            }

            return tmpList.ToArray();
        }

        public static explicit operator char(CharEx c)
        {
            if (c.isUTF32())
                throw new InvalidCastException();
            return (char)c._value;
        }
        public static implicit operator CharEx(char c)
        {
            return new CharEx(c);
        }
        public static implicit operator int(CharEx c)
        {
            return c._value;
        }
        public static implicit operator uint(CharEx c)
        {
            return (uint)c._value;
        }
        public static bool operator >(CharEx a, CharEx b)
        {
            return a._value > b._value;
        }
        public static bool operator <(CharEx a, CharEx b)
        {
            return a._value < b._value;
        }
        public static CharEx operator +(CharEx a,int i)
        {
            return new CharEx(a._value + i);
        }
        public static CharEx operator ++(CharEx a)
        {
            return new CharEx(a._value + 1);
        }
        public static CharEx operator -(CharEx a, int i)
        {
            return new CharEx(a._value - i);
        }
        public static CharEx operator --(CharEx a)
        {
            return new CharEx(a._value - 1);
        }
        public static bool operator ==(CharEx a, CharEx b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(CharEx a, Char b)
        {
            return !a.Equals(b);
        }
        public static bool operator ==(CharEx a, Char b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(CharEx a, CharEx b)
        {
            return !a.Equals(b);
        }
        public static String ToString(CharEx[] Array)
        {
            string o = "";
            foreach (var item in Array)
            {
                o += item.ToString();
            }
            return o;
        }
        #endregion

        #region Conversion Types
        public TypeCode GetTypeCode()
        {
            return _value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            if (isUTF32())
                throw new ArgumentOutOfRangeException();
            return ((IConvertible)_value).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToSingle(provider);
        }

        public override string ToString()
        {   
            return Char.ConvertFromUtf32(_value);
        }

        public string ToString(IFormatProvider provider)
        {
            return _value.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)_value).ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToUInt64(provider);
        }
        #endregion
        public int CompareTo(CharEx obj)
        {
            return _value.CompareTo(obj._value);
        }
        public int CompareTo(Char obj)
        {
            return _value.CompareTo((int)obj);
        }

        public bool Equals(CharEx other)
        {
           return this._value == other._value;
        }

        public bool Equals(char other)
        {
            return this._value == (int)other;
        }
    }
    
    //// <summary>
    //// Read the next char from the reader. 
    //// Will read Second value of UTF-32
    //// </summary>
    //public static class CharExExtensions
    //{
    //    public static CharEx? ReadCharEx(this System.IO.BinaryReader input)
    //    {
    //        CharEx? retVal = null;
    //        char? first = input.ReadChar();
    //        if (first != null)
    //        {
    //            if (Char.IsHighSurrogate((char)first))
    //            {
    //                char? second = input.ReadChar();
    //                if (second == null || !Char.IsLowSurrogate((char)first))
    //                    throw new InvalidCastException("Expected UTF-32 Pair for Char");
    //                retVal = new CharEx((char)first, (char)second);
    //            }
    //            else
    //                retVal = new CharEx((char)first);
    //            }

    //        return retVal;
    //    }
    //    // Placeholder for Writer

    //    ////public static CharEx? WriteCharEx(this System.IO.BinaryWriter input)
    //    ////{
    //    ////    CharEx? retVal = null;
    //    ////    char? first = input.BaseStream.();
    //    ////    if (first != null)
    //    ////    {
    //    ////        if (Char.IsHighSurrogate((char)first))
    //    ////        {
    //    ////            char? second = input.ReadChar();
    //    ////            if (second == null || !Char.IsLowSurrogate((char)first))
    //    ////                throw new InvalidCastException("Expected UTF-32 Pair for Char");
    //    ////            retVal = new CharEx((char)first, (char)second);
    //    ////        }
    //    ////        else
    //    ////            retVal = new CharEx((char)first);
    //    ////    }

    //    ////    return retVal;
    //    ////}
    //}
}
