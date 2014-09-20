using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace Hite.Common
{
    public static class ConvertHelper
    {
        #region = ChangeType =
        public static object ChangeType(object value, Type conversionType)
        {
            
            return ChangeType(value, conversionType, Thread.CurrentThread.CurrentCulture);
        }

        public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }
            if (value == null)
            {
                if (conversionType.IsValueType)
                {
                    throw new InvalidCastException("InvalidCast_CannotCastNullToValueType");
                }
                return null;
            }
            IConvertible convertible1 = value as IConvertible;
            if (convertible1 == null)
            {
                if (value.GetType() != conversionType)
                {
                    throw new InvalidCastException("InvalidCast_IConvertible");
                }
                return value;
            }
            
            if (typeof(System.Enum).IsAssignableFrom(conversionType) == true)
            {
                return convertible1.ToInt32(provider);
            }

            #region - bool -
            if (conversionType == typeof(bool))
            {
                return convertible1.ToBoolean(provider);
            }
            #endregion

            #region - char -
            if (conversionType == typeof(char))
            {
                return convertible1.ToChar(provider);
            }
            #endregion

            #region - sbyte -
            if (conversionType == typeof(sbyte))
            {
                return convertible1.ToSByte(provider);
            }
            #endregion

            #region - byte -
            if (conversionType == typeof(byte))
            {
                return convertible1.ToByte(provider);
            }
            #endregion

            #region - short -
            if (conversionType == typeof(short))
            {
                return convertible1.ToInt16(provider);
            }
            #endregion

            #region - ushort -
            if (conversionType == typeof(ushort))
            {
                return convertible1.ToUInt16(provider);
            }
            #endregion

            #region - int -
            if (conversionType == typeof(int))
            {
                return convertible1.ToInt32(provider);
            }
            #endregion

            #region - uint -
            if (conversionType == typeof(uint))
            {
                return convertible1.ToUInt32(provider);
            }
            #endregion

            #region - long -
            if (conversionType == typeof(long))
            {
                return convertible1.ToInt64(provider);
            }
            #endregion

            #region - ulong -
            if (conversionType == typeof(ulong))
            {
                return convertible1.ToUInt64(provider);
            }
            #endregion

            #region - float -
            if (conversionType == typeof(float))
            {
                return convertible1.ToSingle(provider);
            }
            #endregion

            #region - double -
            if (conversionType == typeof(double))
            {
                return convertible1.ToDouble(provider);
            }
            #endregion

            #region - decimal -
            if (conversionType == typeof(decimal))
            {
                return convertible1.ToDecimal(provider);
            }
            #endregion

            #region - DateTime -
            if (conversionType == typeof(DateTime))
            {
                try
                {
                    return convertible1.ToDateTime(provider);
                }
                catch (Exception ex)
                {
                   throw new System.FormatException(ex.Message + "\n" + value.ToString());
                }
            }
            #endregion

            if(conversionType == typeof(Nullable<DateTime>)){
                var _value = value as Nullable<DateTime>;
                if(_value.HasValue){
                    return _value.Value;
                }
                return _value;
            }

            #region - string -
            if (conversionType == typeof(string))
            {
                return convertible1.ToString(provider);
            }
            #endregion

            #region - object -
            if (conversionType == typeof(object))
            {
                return value;
            }
            #endregion

            #region = Uri =
            if (conversionType == typeof(Uri))
            {
                string s = value.ToString();
                return new Uri(s);
            }
            #endregion

            



            return convertible1.ToType(conversionType, provider);
        } 
        #endregion

        #region - GetObjectFromString -
        /// <summary>
        /// Gets the object from string.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializeAs">The serialize as.</param>
        /// <param name="attValue">The att value.</param>
        /// <returns></returns>
        public static object GetObjectFromString(Type type, SettingsSerializeAs serializeAs, string attValue)
        {
            // Deal with string types
            if (type == typeof(string) && (string.IsNullOrEmpty(attValue) == true || attValue.Length < 1 || serializeAs == SettingsSerializeAs.String))
                return attValue;

            // Return null if there is nothing to convert
            if (string.IsNullOrEmpty(attValue) == true)
                return null;

            // Convert based on the serialized type
            switch (serializeAs)
            {
                case SettingsSerializeAs.Binary:
                    byte[] buf = Convert.FromBase64String(attValue);
                    MemoryStream ms = null;
                    try
                    {
                        ms = new System.IO.MemoryStream(buf);
                        return (new BinaryFormatter()).Deserialize(ms);
                    }
                    finally
                    {
                        if (ms != null)
                            ms.Close();
                    }

                case SettingsSerializeAs.Xml:
                    StringReader sr = new StringReader(attValue);
                    XmlSerializer xs = new XmlSerializer(type);
                    return xs.Deserialize(sr);

                case SettingsSerializeAs.String:
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if (converter != null && converter.CanConvertTo(typeof(String)) && converter.CanConvertFrom(typeof(String)))
                        return converter.ConvertFromString(attValue);
                    throw new ArgumentException("Unable to convert type: " + type.ToString() + " from string", "type");

                default:
                    return null;
            }
        }
        #endregion

        #region - ConvertObjectToString -
        /// <summary>
        /// Converts the object to string.
        /// </summary>
        /// <param name="propValue">The prop value.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializeAs">The serialize as.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns></returns>
        public static string ConvertObjectToString(object propValue, Type type, SettingsSerializeAs serializeAs, bool throwOnError)
        {
            if (serializeAs == SettingsSerializeAs.ProviderSpecific)
            {
                if (type == typeof(string) || type.IsPrimitive)
                    serializeAs = SettingsSerializeAs.String;
                else
                    serializeAs = SettingsSerializeAs.Xml;
            }

            try
            {
                switch (serializeAs)
                {
                    case SettingsSerializeAs.String:
                        TypeConverter converter = TypeDescriptor.GetConverter(type);
                        if (converter != null && converter.CanConvertTo(typeof(String)) && converter.CanConvertFrom(typeof(String)))
                            return converter.ConvertToString(propValue);
                        throw new ArgumentException("Unable to convert type " + type.ToString() + " to string", "type");
                    case SettingsSerializeAs.Binary:
                        MemoryStream ms = new System.IO.MemoryStream();
                        try
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            bf.Serialize(ms, propValue);
                            byte[] buffer = ms.ToArray();
                            return Convert.ToBase64String(buffer);
                        }
                        finally
                        {
                            ms.Close();
                        }

                    case SettingsSerializeAs.Xml:
                        XmlSerializer xs = new XmlSerializer(type);
                        StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

                        xs.Serialize(sw, propValue);
                        return sw.ToString();
                }
            }
            catch (Exception)
            {
                if (throwOnError)
                    throw;
            }
            return null;
        }
        #endregion
    }
}
