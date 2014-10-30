//using System;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Globalization;

//namespace CachProvider.Memory.Tests
//{
//    public class NameValueCollectionTypeConverter: TypeConverter
//    {
//        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
//        {
//            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
//        }

//        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
//        {
//            if (value is string)
//            {
//                var nvc = new NameValueCollection();

//                var items = value.ToString().Split(Convert.ToChar(";"));
//                foreach (var item in items)
//                {
//                    var key = item;
//                    var val = string.Empty;
//                    var indx = item.IndexOf("=", StringComparison.Ordinal);
//                    if (indx > 0)
//                    {
//                        key = item.Substring(0, indx);
//                        val = item.Length >= indx + 1 ? item.Substring(indx + 1, item.Length - (indx + 1)) : string.Empty;
//                    }
//                    nvc.Add(key, val);
//                }
//                return nvc;
//            }
//            return base.ConvertFrom(context, culture, value);
//        }

//        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
//        {
//            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
//        }

//        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
//        {
//            return destinationType == typeof(string) ? "" : base.ConvertTo(context, culture, value, destinationType);
//        }
//    }
//}