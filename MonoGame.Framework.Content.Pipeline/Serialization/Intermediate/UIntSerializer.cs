// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate
{
    [ContentTypeSerializer]
    class UIntSerializer : ElementSerializer<uint>
    {
        public UIntSerializer() :
            base("int", 1)
        {
        }

        protected internal override uint Deserialize(string[] inputs, ref int index)
        {
            string i = inputs[index++].Trim().ToUpperInvariant();
            // Check for hex input
            if (i.StartsWith("0X"))
            {
                // Trim "0x" from beginning of number
                i = i.Substring(2);
                return uint.Parse(i, System.Globalization.NumberStyles.AllowHexSpecifier,System.Globalization.CultureInfo.InvariantCulture);
            }
            else
                return XmlConvert.ToUInt32(i);

        }

        protected internal override void Serialize(uint value, List<string> results)
        {
            results.Add(XmlConvert.ToString(value));
        }
    }
}