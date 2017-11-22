using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibraryProxy
{
    public static class ImageHelper
    {
        public unsafe static void Resize(IntPtr dest, int srcWidth, int srcHeight, int width, int height, byte[] baseImage, GCHandle acHandle)
        {
            var handle = GCHandle.Alloc(baseImage, GCHandleType.Pinned);
            var ptrBaseImage = handle.AddrOfPinnedObject();

            MediaLibrary.Imaging.Manipulation.Resize((int*)dest, (int*)acHandle.AddrOfPinnedObject(), srcWidth, srcHeight, width, height, (byte*)ptrBaseImage.ToPointer());
        }
    }
}
