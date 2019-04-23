using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using Microsoft.Xna.Framework.Graphics;

namespace MaterialEditor
{
    public class DDSConverter : IValueConverter
    {
        private static readonly DDSConverter defaultInstace = new DDSConverter();

        public static DDSConverter Default
        {
            get
            {
                return DDSConverter.defaultInstace;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            else if (value is Stream)
                return DDSConverter.Convert((Stream)value);
            else if (value is string)
                return DDSConverter.Convert((string)value);
            else if (value is byte[])
                return DDSConverter.Convert((byte[])value);
            else
                throw new NotSupportedException(string.Format("{0} cannot convert from {1}.", this.GetType().FullName, value.GetType().FullName));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(string.Format("{0} does not support converting back.", this.GetType().FullName));
        }

        public static ImageSource Convert(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                return DDSConverter.Convert(fileStream);
            }
        }

        public static ImageSource Convert(byte[] imageData)
        {
            using (var memoryStream = new MemoryStream(imageData))
            {
                return DDSConverter.Convert(memoryStream);
            }
        }

        public static ImageSource Convert(Stream stream)
        {
            return null;
            // Create the graphics device
            //using (var graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.NullReference, IntPtr.Zero, new PresentationParameters()))
            //{
            //    // Setup the texture creation parameters
            //    var textureCreationParameters = new TextureCreationParameters()
            //    {
            //        Width = -1,
            //        Height = -1,
            //        Depth = 1,
            //        TextureUsage = TextureUsage.None,
            //        Format = SurfaceFormat.Color,
            //        Filter = FilterOptions.None,
            //        MipFilter = FilterOptions.None,
            //        MipLevels = 1
            //    };

            //    // Load the texture
            //    using (var texture = Texture2D.FromFile(graphicsDevice, stream, textureCreationParameters))
            //    {
            //        // Get the pixel data
            //        var pixelColors = new Microsoft.Xna.Framework.Graphics.Color[texture.Width * texture.Height];
            //        texture.GetData(pixelColors);

            //        // Copy the pixel colors into a byte array
            //        var bytesPerPixel = 3;
            //        var stride = texture.Width * bytesPerPixel;

            //        var pixelData = new byte[pixelColors.Length * bytesPerPixel];
            //        for (var i = 0; i < pixelColors.Length; i++)
            //        {
            //            pixelData[i * bytesPerPixel + 0] = pixelColors[i].R;
            //            pixelData[i * bytesPerPixel + 1] = pixelColors[i].G;
            //            pixelData[i * bytesPerPixel + 2] = pixelColors[i].B;
            //        }

            //        // Create a bitmap source
            //        return System.Windows.Media.Imaging.BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Rgb24, null, pixelData, stride);
            //    }
            //}
        }
    }
}
