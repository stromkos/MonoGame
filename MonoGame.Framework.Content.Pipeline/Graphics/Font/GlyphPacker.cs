// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
	// Helper for arranging many small bitmaps onto a single larger surface.
	internal static class GlyphPacker
	{
		/// <summary>
		/// Arrange Glyphs and create output bitmap.
		/// </summary>
		/// <param name="sourceGlyphs"> The filled <see cref="Glyph"/> array to be placed.</param>
		/// <param name="requirePOT"> Does the output need to be sized to a power of 2.</param>
		/// <param name="requireSquare"> Does the output need to be sized as a square.</param>
		/// <param name="logger"> Logger used to display informational and warning messages to user.</param>
		/// <exception cref="Exception">Thrown when the generated Bitmap is too large.</exception>
		public static BitmapContent ArrangeGlyphs(Glyph[] sourceGlyphs, bool requirePOT, bool requireSquare, ContentBuildLogger logger)
		{
			var glyphs = new ArrangedGlyph[sourceGlyphs.Length];

			int totalArea = 0;

			// Build up an array of all the glyphs needing to be arranged and count the total area
			for (int i = 0; i < sourceGlyphs.Length; i++)
			{
				var glyph = new ArrangedGlyph();

				glyph.Source = sourceGlyphs[i];

				// Leave a one pixel border around every glyph in the output bitmap.
				glyph.Width = sourceGlyphs[i].Subrect.Width + 2;
				glyph.Height = sourceGlyphs[i].Subrect.Height + 2;
				totalArea += glyph.Width * glyph.Height;
				glyphs[i] = glyph;
			}

			// Sort so the tallest then widest glyphs get arranged first.
			Array.Sort(glyphs);			

			// Work out how big the output bitmap should be.
			int outputWidth = 512; // use 512 to restrict error.  Max height is < 512k
			int outputHeight = 0;

			// First Pass to get the dimensions
			PlaceGlyphs(glyphs, outputWidth, out outputWidth, out outputHeight, requirePOT);
			outputWidth = MakeValidTextureSize(Math.Min((requirePOT)? 8192 : 16380,(int)Math.Sqrt(outputWidth * outputHeight)), requirePOT);
			// Second pass with square width
			PlaceGlyphs(glyphs, outputWidth, out outputWidth, out outputHeight, requirePOT);
			
			int max = Math.Max(outputWidth, outputHeight);
			if (requireSquare)
            {
				outputHeight = max;
				outputWidth = max;
			}

			// Warn about large texture sizes;  11585 overflows the Array index capacity.
			if ((((requirePOT || requireSquare) && max > 8192) || outputHeight * outputWidth >= 16380 * 16384))
				throw new Exception(String.Format("Font texture size is too large {0}x{1}, try using a smaller size.",
				outputWidth, outputHeight));
			if (outputHeight *  outputWidth >= 16380 * 8192)
				logger.LogWarning("", null, 
				"The font texture size is very large, {0}x{1}, and may fail to build.", outputWidth, outputHeight);
			else
			if (max > 8192)
				logger.LogWarning("", null, 
				"The font texture size is larger than 8192. Check to see if this size, {0}x{1}, is supported on the target platform", 
				outputWidth, outputHeight);
			else
			if (max > 4096)
				logger.LogWarning("", null, 
				"The font texture size is larger than 4096. Check to see if this size, {0}x{1}, is supported on the target platform", 
				outputWidth, outputHeight);
			else
			if (max > 2048 && requirePOT)  // Most modern systems support 4k textures. Only warn for POT profiles
				logger.LogWarning("", null, 
				"The font texture size is larger than 2048. Check to see if this size, {0}x{1}, is supported on the target platform", 
				outputWidth, outputHeight);

			// Create the merged output bitmap.
			return CopyGlyphsToOutput(glyphs, outputWidth, outputHeight, logger);
		}

		// Choose positions for each glyph. Filling in between the lines and line ends from the smallest
		// This code is completely linearly scaling. Each of the while loops removes an iteration of the outer for loop
		static void PlaceGlyphs(ArrangedGlyph[] glyphs, int guess, out int outputWidth, out int outputHeight, bool requirePOT)
		{
			int currentLineHeight = 0;
			int x = 0;
			int y = 0;
			int subX = 0;
			int smallestGlyphIndex = glyphs.Length - 1;
			Dictionary<uint,Point> duplicates = new Dictionary<uint,Point>(glyphs.Length);

			outputWidth = guess;

			Point output = new Point();

			for (int i = 0; i <= smallestGlyphIndex; i++)
			{
				var glyph = glyphs[i];
				var smallestGlyph = glyphs[smallestGlyphIndex];

				if (duplicates.TryGetValue(glyph.Source.GlyphIndex, out output))
				{
					glyph.X = output.X;
					glyph.Y = output.Y;
					continue;
				}
				
				currentLineHeight = Math.Max(glyph.Height, currentLineHeight);

				if (x + glyph.Width > outputWidth)
				{
					//end of line, see if the smallest will fit here
					while (smallestGlyphIndex > i && x + smallestGlyph.Width < outputWidth)
					{
						if (duplicates.TryGetValue(smallestGlyph.Source.GlyphIndex, out output))
						{
							smallestGlyph.X = output.X;
							smallestGlyph.Y = output.Y;
							smallestGlyph = glyphs[--smallestGlyphIndex];
							continue;
						}
						
						duplicates.Add(smallestGlyph.Source.GlyphIndex, new Point(x, y));	

						smallestGlyph.X = x;
						smallestGlyph.Y = y;
						int subY = y + smallestGlyph.Height;
						int xMax = smallestGlyph.Width;
						smallestGlyph = glyphs[--smallestGlyphIndex];

						// See if another one fits below, restricted to the height of previous glyph
						while (smallestGlyphIndex > i && i > 0 &&
							y + glyphs[i-1].Height - subY > smallestGlyph.Height && 
							x + smallestGlyph.Width < outputWidth)
						{
							if (duplicates.TryGetValue(smallestGlyph.Source.GlyphIndex, out output))
							{
								smallestGlyph.X = output.X;
								smallestGlyph.Y = output.Y;							
								smallestGlyph = glyphs[--smallestGlyphIndex];
								continue;
							}

							duplicates.Add(smallestGlyph.Source.GlyphIndex, new Point(subX, subY));	
							smallestGlyph.X = x;
							smallestGlyph.Y = subY;
							xMax = Math.Max(xMax,smallestGlyph.Width);
							subY += smallestGlyph.Height; 
							smallestGlyph = glyphs[--smallestGlyphIndex];
						}
						
						x += xMax;
					}

					y += currentLineHeight;
					currentLineHeight = 0;
					subX = 0;
					x = 0;
				}

				duplicates.Add(glyph.Source.GlyphIndex, new Point(x, y));				
				glyph.X = x;
				glyph.Y = y;

				subX = Math.Max(x, subX);
				while (smallestGlyphIndex > i && subX <= x + glyph.Width &&
						currentLineHeight - glyph.Height >= smallestGlyph.Height && 
						subX + smallestGlyph.Width < outputWidth)
				{
					// Fill the space between lines 
					// Moving down then to the right until EOL or it does not fit vertically
					// where it checks on each new main glyph's height
					int subY = y + glyph.Height;
					
					if (duplicates.TryGetValue(smallestGlyph.Source.GlyphIndex, out output))
					{
						smallestGlyph.X = output.X;
						smallestGlyph.Y = output.Y;							
						smallestGlyph = glyphs[--smallestGlyphIndex];
						continue;
					}

					duplicates.Add(smallestGlyph.Source.GlyphIndex, new Point(subX, subY));	
					smallestGlyph.X = subX;
					smallestGlyph.Y = subY;
				
					int subSubY = subY + smallestGlyph.Height;
					int xMax = smallestGlyph.Width;
					smallestGlyph = glyphs[--smallestGlyphIndex];

					// See if another one fits below
					while (smallestGlyphIndex > i && 
						y + currentLineHeight - subSubY > smallestGlyph.Height && 
						subX + smallestGlyph.Width < outputWidth)
					{
						if (duplicates.TryGetValue(smallestGlyph.Source.GlyphIndex, out output))
						{
							smallestGlyph.X = output.X;
							smallestGlyph.Y = output.Y;							
							smallestGlyph = glyphs[--smallestGlyphIndex];
							continue;
						}

						duplicates.Add(smallestGlyph.Source.GlyphIndex, new Point(subX, subSubY));	
						smallestGlyph.X = subX;
						smallestGlyph.Y = subSubY;
						xMax = Math.Max(xMax,smallestGlyph.Width);
						subSubY += smallestGlyph.Height; 
						smallestGlyph = glyphs[--smallestGlyphIndex];
					}
					subX += xMax;
				}
				x += glyph.Width;
			}

			// If the output is a single line shrink width 
			if (y == 0) outputWidth = MakeValidTextureSize(x, requirePOT);

			outputHeight = MakeValidTextureSize(y + currentLineHeight, requirePOT);
		}

		// Once arranging is complete, copies each glyph to its chosen position in the single larger output bitmap.
		static BitmapContent CopyGlyphsToOutput(ArrangedGlyph[] glyphs, int width, int height, ContentBuildLogger logger)
		{
            var output = new PixelBitmapContent<Color>(width, height);

#if (NET472 || NETSTANDARD2_1 || NETCOREAPP3_1 || NET5_0)
			var duplicates = new HashSet<uint>(glyphs.Length);
#else
			var duplicates = new HashSet<uint>();  //to allow console 4.5 builds
#endif			

			int storedGlyphs = 0;
			int zeroGlyphs = 0;

			foreach (var glyph in glyphs)
			{
				var sourceGlyph = glyph.Source;
				var sourceRegion = sourceGlyph.Subrect;
				var destinationRegion = new Rectangle(glyph.X, glyph.Y, sourceRegion.Width, sourceRegion.Height);
				if (sourceGlyph.GlyphIndex == 0) zeroGlyphs++;
				if (!duplicates.Contains(sourceGlyph.GlyphIndex))
				{
					BitmapContent.Copy(sourceGlyph.Bitmap, sourceRegion, output, destinationRegion);
					duplicates.Add(sourceGlyph.GlyphIndex);
					storedGlyphs++;
				}

				sourceGlyph.Bitmap = output;
				sourceGlyph.Subrect = destinationRegion;
			}

			logger.LogMessage("Created {0} unique Glyphs from {1} characters. {2} were reported as not found in the font file.", storedGlyphs, glyphs.Length, zeroGlyphs);

			return output;
		}


		// Internal helper class keeps track of a glyph while it is being arranged.
		class ArrangedGlyph : IComparable<ArrangedGlyph>
		{
			public Glyph Source;

			public int X;
			public int Y;

			public int Width;
			public int Height;

			public int CompareTo(ArrangedGlyph b)
			{				
				int aSize = (Height << 10) | Width;
				int bSize = (b.Height << 10) | b.Width;

			if (aSize != bSize)
				return bSize.CompareTo(aSize);
			else
				return Source.Character.CompareTo(b.Source.Character);
			}
		}


		// Rounds a value up to the next larger valid texture size.
		static int MakeValidTextureSize(int value, bool requirePowerOfTwo)
		{
			// In case we want to compress the texture, make sure the size is a multiple of 4.
			const int blockSize = 4;

			if (requirePowerOfTwo)
			{
				// Round up to a power of two.
				int powerOfTwo = blockSize;

				while (powerOfTwo < value)
					powerOfTwo <<= 1;

				return powerOfTwo;
			}
			else
			{
				// Round up to the specified block size.
				return (value + blockSize - 1) & ~(blockSize - 1);
			}
		}
	}

}

