using System.Globalization;

namespace Acorisoft.Miga.Utils.Infrastructures
{
    public static class StringConverter
    {
        internal delegate object Expression(string value);

        #region HelperMethods

        private static readonly object True  = true;
        private static readonly object False = false;

        private static readonly NumberFormatInfo nfi = new NumberFormatInfo();

        internal static object ToInt8(string value)
        {
            return byte.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToSInt8(string value)
        {
            return sbyte.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToInt16(string value)
        {
            return short.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToUInt16(string value)
        {
            return ushort.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToInt32(string value)
        {
            return int.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToUInt32(string value)
        {
            return uint.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToBoolean(string value)
        {
            return bool.TryParse(value, out var val) && val ? True : False;
        }

        internal static object ToInt64(string value)
        {
            return long.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0L;
        }

        internal static object ToUInt64(string value)
        {
            return ulong.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0L;
        }

        internal static object ToFP32(string value)
        {
            return float.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0f;
        }

        internal static object ToFP64(string value)
        {
            return double.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0d;
        }

        internal static object ToDecimal(string value)
        {
            return decimal.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToString(string value)
        {
            return value;
        }

        internal static object ToGuid(string value)
        {
            return Guid.TryParse(value, out var val) ? val : Guid.NewGuid();
        }

        internal static object ToTimeSpan(string value)
        {
            return TimeSpan.TryParse(value, out var val) ? val : TimeSpan.Zero;
        }

        internal static object ToDateTime(string value)
        {
            return DateTime.TryParse(value, out var val) ? val : new DateTime();
        }

        internal static object ToIntPtr(string value)
        {
            return IntPtr.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToUIntPtr(string value)
        {
            return UIntPtr.TryParse(value, NumberStyles.None, nfi, out var val) ? val : 0;
        }

        internal static object ToByteArray(string value)
        {
            try
            {
                return string.IsNullOrEmpty(value) ? null : System.Convert.FromBase64String(value);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        #endregion

        private static readonly Dictionary<Type, Expression> _dictionary =
            new Dictionary<Type, Expression>
            {
                { typeof(byte), ToInt8 },
                { typeof(sbyte), ToSInt8 },
                { typeof(short), ToInt16 },
                { typeof(ushort), ToUInt16 },
                { typeof(bool), ToBoolean },
                { typeof(int), ToInt32 },
                { typeof(uint), ToUInt32 },
                { typeof(long), ToInt64 },
                { typeof(ulong), ToInt64 },
                { typeof(float), ToFP32 },
                { typeof(double), ToFP64 },
                { typeof(decimal), ToDecimal },
                { typeof(Guid), ToGuid },
                { typeof(byte[]), ToByteArray },
                { typeof(string), ToString },
                { typeof(TimeSpan), ToTimeSpan },
                { typeof(DateTime), ToDateTime },
                { typeof(IntPtr), ToIntPtr },
                { typeof(UIntPtr), ToUIntPtr },
            };

        public static T Convert<T>(string value)
        {
            if (!_dictionary.TryGetValue(typeof(T), out var expression))
            {
                return default(T);
            }

            return (T)expression(value);
        }
    }
}