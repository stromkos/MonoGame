using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Xna.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
	public class CharacterRegionTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}


		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			// Input must be a string.
			string source = value as string;

			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentException();
			}

			// Supported input formats:
			//  A
			//  A-Z
			//  32-127
			//  0x20-0x7F

			var splitStr = source.Split('-');
			

			switch (splitStr.Length)
			{
				case 1:
				// Only a single character (eg. "a").
				return new CharacterRegion(new CharEx(splitStr[0]), new CharEx(splitStr[0]));

				case 2:
				// Range of characters (eg. "a-z").
				return new CharacterRegion(new CharEx(splitStr[0]), new CharEx(splitStr[1]));

				default:
				throw new ArgumentException();
			}
		}


		static CharEx ConvertCharacter(string value)
		{
			if (value.Length == 1 || (value.Length == 2 && Char.IsSurrogate(value[0])))
			{
				// Single character directly specifies a codepoint.
				return new CharEx(value);
			}
			else
			{
				// Otherwise it must be an integer (eg. "32" or "0x20").
				return new CharEx((int)intConverter.ConvertFromInvariantString(value));
			}
		}


		static TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
	}
}