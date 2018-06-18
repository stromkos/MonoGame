// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Utilities;


namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Writes the character value to the output.
    /// </summary>
    [ContentTypeWriter]
    class CharExWriter : BuiltInContentWriter<CharEx>
    {
        /// <summary>
        /// Writes the value to the output.
        /// </summary>
        /// <param name="output">The output writer object.</param>
        /// <param name="value">The value to write to the output.</param>
        protected internal override void Write(ContentWriter output, CharEx value)
        {
            output.Write(value.ToString().ToCharArray(),0, value.ToString().Length);
        }
    }
}
