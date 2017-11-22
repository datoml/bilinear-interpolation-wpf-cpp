using MediaLibraryProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BilinearInterpolationMVP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WIDTH = 320;
        const int HEIGHT = 240;

        private Timer _tImageStreamEmulator;
        private BitmapSource _source;
        private WriteableBitmap _destination;

        private int[] foregroundBuffer;
        GCHandle foregroundBufferHandle;

        // Debug
        System.Diagnostics.Stopwatch _sw = new System.Diagnostics.Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            // The buffer we use
            BackgroundImage = new byte[320 * 240];

            // We need to pin the foreground buffer to make sure
            // the int buffer is aligned in  memory.
            foregroundBuffer = new int[64 * 48];
            foregroundBufferHandle = GCHandle.Alloc(foregroundBuffer, GCHandleType.Pinned);

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            BitmapPalette myPalette = new BitmapPalette(colors);

            // Calculate the stride
            int stride = WIDTH * (32 + 7) / 8;
            byte[] pixels = new byte[stride * HEIGHT];

            // Create bitmap source
            _source = WriteableBitmap.Create(
                WIDTH,
                HEIGHT,
                96,
                96,
                System.Windows.Media.PixelFormats.Pbgra32,
                myPalette,
                pixels,
                stride);

            // Create destination writeable bitmap
            _destination = new WriteableBitmap(_source);

            // Do stuff when window loaded
            Loaded += OnLoaded;

            // Create fake timer
            _tImageStreamEmulator = new Timer(16);
            _tImageStreamEmulator.Elapsed += OnNewImageAvailable;

            // Start fake stream
            _tImageStreamEmulator.Start();
        }        

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // First load the background and foreground images to emulate the data stream of images.
            // In this case its always the same image
            var background = new BitmapImage(new Uri(@"../../media/background.bmp", UriKind.Relative));
            background.CopyPixels(BackgroundImage, 320, 0);

            var foreground = new BitmapImage(new Uri(@"../../media/foreground.png", UriKind.Relative));
            foreground.CopyPixels(foregroundBuffer, 64 * 4, 0);
        }

        private void OnNewImageAvailable(object sender, ElapsedEventArgs e)
        {
            _sw.Start();

            // Pointer for the destination BackBuffer
            IntPtr ptrDest = IntPtr.Zero;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // Get the dest BackBuffer from the writeable bitmap.
                // This has to be done in the UI thread.
                ptrDest = _destination.BackBuffer;
            });

            // Start resizing the foreground image and merge it with the background image.
            ImageHelper.Resize(ptrDest, 64, 48, WIDTH, HEIGHT, BackgroundImage, foregroundBufferHandle);

            _sw.Stop();
            System.Diagnostics.Debug.WriteLine("Resize process took {0} ms.", _sw.ElapsedMilliseconds);
            _sw.Reset();

            // Set the new source for the image on the GUI.
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                this.ImageStream.Source = _destination;
            });

            // Output a debug image
            ////var resultImage = new System.Drawing.Bitmap(320, 240, 4 * 320, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptrDest);
            ////resultImage.Save(@"C:\Temp\combined_thingy.bmp");
        }

        // Properties
        public byte[] BackgroundImage { get; set; }
    }
}
