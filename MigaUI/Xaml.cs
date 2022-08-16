using System.Globalization;
using System.Windows.Media.Imaging;

namespace Acorisoft.Miga.UI
{
    public static class Xaml
    {
        
        #region DoubleBoxing


        internal static object[] DoubleBoxes = new object[]
        {
            0d,
            1d,
            2d,
        };

        public static object Box(double value)
        {
            return value switch
            {
                0 => DoubleBoxes[0],
                1 => DoubleBoxes[1],
                2 => DoubleBoxes[2],
                _ => value,
            };
        }

        #endregion
        
        #region IntBoxing


        internal static object[] IntBoxes = new object[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
        };

        public static object Box(int value)
        {
            return value switch
            {
                0 => IntBoxes[0],
                1 => IntBoxes[1],
                2 => IntBoxes[2],
                3 => IntBoxes[3],
                4 => IntBoxes[4],
                5 => IntBoxes[5],
                6 => IntBoxes[6],
                7 => IntBoxes[7],
                8 => IntBoxes[8],
                9 => IntBoxes[9],
                10 => IntBoxes[10],
                _ => value,
            };
        }

        #endregion
        
        #region Boolean Boxing

        public static readonly object True  = true;
        public static readonly object False = false;

        /// <summary>
        /// 获得指定的布尔类型的装箱值。
        /// </summary>
        public static object Box(bool condition) => condition ? True : False;

        #endregion

        #region MyRegion

        /// <summary>
        /// 寻找指定元素的父级。
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindAncestor<T>(DependencyObject source) where T : DependencyObject
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as T;
        }

        public static T GetResource<T>(this ResourceDictionary rex, string key)
        {
            return (T)rex[key];
        }

        /// <summary>
        /// 寻找资源
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetResource<T>(string key)
        {
            return (T)Application.Current.Resources[key];
        }

        /// <summary>
        /// 创建命令
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static RoutedUICommand CreateCommand(string name, Type type)
        {
            return new RoutedUICommand(name, name, type);
        }

        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dip"></param>
        /// <returns></returns>
        public static RenderTargetBitmap Capture(FrameworkElement element, int dip = 96)
        {
            if (element is null)
            {
                return null;
            }

            var dpi = VisualTreeHelper.GetDpi(element);
            var bitmap = new RenderTargetBitmap(
                (int)element.ActualWidth,
                (int)element.ActualHeight,
                dpi.PixelsPerInchX,
                dpi.PixelsPerInchY,
                PixelFormats.Pbgra32);
            bitmap.Render(element);

            return bitmap;
        }

        #endregion

        #region Color

        /// <summary>
        /// Convert a html colour code to an OpenTK colour object.
        /// </summary>
        /// <param name="hexCode">The html style hex colour code.</param>
        /// <returns>A colour from the rainbow.</returns>
        public static Color FromHex(string hexCode)
        {
            if (string.IsNullOrEmpty(hexCode))
            {
                return Colors.White;
            }
            
            // Remove the # if it exists.
            var hex = hexCode.TrimStart('#');

            // Create the colour that we will work on.
            var colour = new Color();

            // If we are working with the shorter hex colour codes, duplicate each character as per the
            // spec https://www.w3.org/TR/2001/WD-css3-color-20010305#colorunits
            // (From E3F to EE33FF)
            if (hex.Length is 3 or 4)
            {
                var longHex = "";

                // For each character in the short hex code add two to the long hex code.
                for (var i = 0; i < hex.Length; i++)
                {
                    longHex += hex[i];
                    longHex += hex[i];
                }

                // the short hex is now the long hex.
                hex = longHex;
            }

            try
            {
                const NumberStyles hexStyle = NumberStyles.HexNumber;
                // We should be working with hex codes that are 6 or 8 characters long.
                if (hex.Length is 6)
                {
                    // Create a constant of the style we want (I don't want to type NumberStyles.HexNumber 4
                    // more times.)
                    colour.R = byte.Parse(hex.Substring(0, 2), hexStyle);
                    colour.G = byte.Parse(hex.Substring(2, 2), hexStyle);
                    colour.B = byte.Parse(hex.Substring(4, 2), hexStyle);

                    // We are done, return the parsed colour.
                    colour.A = 255;
                    return colour;
                }
                if (hex.Length is 8)
                {
                    // Create a constant of the style we want (I don't want to type NumberStyles.HexNumber 4
                    // more times.)

                    // Parse Red, Green and Blue from each pair of characters.
                    colour.A = byte.Parse(hex[..2], hexStyle);
                    colour.R = byte.Parse(hex.Substring(2, 2), hexStyle);
                    colour.G = byte.Parse(hex.Substring(4, 2), hexStyle);
                    colour.B = byte.Parse(hex.Substring(6, 2), hexStyle);

                    // We are done, return the parsed colour.
                    return colour;
                }
            }
            catch
            {
                return Colors.White;
            }

            return Colors.White;
        }

        #endregion
    }
}