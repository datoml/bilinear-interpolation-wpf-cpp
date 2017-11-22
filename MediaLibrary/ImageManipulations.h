#pragma once

namespace MediaLibrary
{
	namespace Imaging
	{
		using namespace System;

		public ref class Manipulation
		{
		public:
			static void Resize(int* dest, int* pixels, int widthSource, int heightSource, int width, int height, byte* baseImage);
		};
	}
}