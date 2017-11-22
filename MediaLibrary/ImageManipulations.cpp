#include "stdafx.h"
#include "ImageManipulations.h"

void MediaLibrary::Imaging::Manipulation::Resize(int* pd, int* pixels, int widthSource, int heightSource, int width, int height, byte* baseImage)
{
	float xs = (float)widthSource / width;
	float ys = (float)heightSource / height;

	float fracx, fracy, ifracx, ifracy, sx, sy, l0, l1, rf, gf, bf;
	int c, x0, x1, y0, y1;
	byte c1a, c1r, c1g, c1b, c2a, c2r, c2g, c2b, c3a, c3r, c3g, c3b, c4a, c4r, c4g, c4b;
	byte a, r, g, b;

	// Bilinear
	int srcIdx = 0;

	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			sx = x * xs;
			sy = y * ys;
			x0 = (int)sx;
			y0 = (int)sy;

			// Calculate coordinates of the 4 interpolation points
			fracx = sx - x0;
			fracy = sy - y0;
			ifracx = 1.0f - fracx;
			ifracy = 1.0f - fracy;
			x1 = x0 + 1;
			if (x1 >= widthSource)
			{
				x1 = x0;
			}
			y1 = y0 + 1;
			if (y1 >= heightSource)
			{
				y1 = y0;
			}

			// Read source color
			c = pixels[y0 * widthSource + x0];
			c1a = (byte)(c >> 24);
			c1r = (byte)(c >> 16);
			c1g = (byte)(c >> 8);
			c1b = (byte)(c);

			c = pixels[y0 * widthSource + x1];
			c2a = (byte)(c >> 24);
			c2r = (byte)(c >> 16);
			c2g = (byte)(c >> 8);
			c2b = (byte)(c);

			c = pixels[y1 * widthSource + x0];
			c3a = (byte)(c >> 24);
			c3r = (byte)(c >> 16);
			c3g = (byte)(c >> 8);
			c3b = (byte)(c);

			c = pixels[y1 * widthSource + x1];
			c4a = (byte)(c >> 24);
			c4r = (byte)(c >> 16);
			c4g = (byte)(c >> 8);
			c4b = (byte)(c);

			// Calculate colors
			// Alpha
			l0 = ifracx * c1a + fracx * c2a;
			l1 = ifracx * c3a + fracx * c4a;
			a = (byte)(ifracy * l0 + fracy * l1);

			// Write destination
			if (a > 0)
			{
				// Red
				l0 = ifracx * c1r + fracx * c2r;
				l1 = ifracx * c3r + fracx * c4r;
				rf = ifracy * l0 + fracy * l1;

				// Green
				l0 = ifracx * c1g + fracx * c2g;
				l1 = ifracx * c3g + fracx * c4g;
				gf = ifracy * l0 + fracy * l1;

				// Blue
				l0 = ifracx * c1b + fracx * c2b;
				l1 = ifracx * c3b + fracx * c4b;
				bf = ifracy * l0 + fracy * l1;

				// Cast to byte
				float alpha = a / 255.0f;
				r = (byte)((rf * alpha) + (baseImage[srcIdx] * (1.0f - alpha)));
				g = (byte)((gf * alpha) + (baseImage[srcIdx] * (1.0f - alpha)));
				b = (byte)((bf * alpha) + (baseImage[srcIdx] * (1.0f - alpha)));

				pd[srcIdx++] = (255 << 24) | (r << 16) | (g << 8) | b;
			}
			else
			{
				// Alpha, Red, Green, Blue							
				pd[srcIdx++] = (255 << 24) | (baseImage[srcIdx] << 16) | (baseImage[srcIdx] << 8) | baseImage[srcIdx];
			}
		}
	}
}