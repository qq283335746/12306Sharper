using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TygaSoft.Model
{
    public static class StringExtensions
    {
        public static Encoding Encoding => Encoding.UTF8;
        public static readonly char[] Separator1 = new char[] { '|' };
        public static readonly char[] Separator2 = new char[] { ';' };

        public static string[] ToArray1(this string input)
        {
            return input.Split(Separator1, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] ToArray2(this string input)
        {
            return input.Split(Separator2, StringSplitOptions.RemoveEmptyEntries);
        }

        public static byte[] AsBytes(this string input)
        {
            return Encoding.GetBytes(input);
        }

        public static async Task<string> AsString(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        public static async Task<byte[]> AsBytes(this Stream inputStream)
        {
            using (var outStream = new MemoryStream())
            {
                await inputStream.CopyToAsync(outStream);
                return outStream.ToArray();
            }
        }

        public static string ToCst(this DateTime shortTime)
        {
            var s = shortTime.ToString("D", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            var arr = s.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Format("{0} {1} {2} {3} 00:00:00 GMT+0800 (CST)", arr[0].Substring(0, 3), arr[2].Substring(0, 3), arr[1], arr[3]);
        }

        public static string ToParameterString(this IEnumerable<NetParameter> datas)
        {
            return string.Join("&", datas.Select(m => m.ToString()));
        }

        public static IEnumerable<KeyValuePair<string, string>> ToNvcs(this IEnumerable<NetParameter> datas)
        {
            var dics = new Dictionary<string, string>();
            foreach (var item in datas)
            {
                dics.Add(item.Name, item.Value);
            }
            return dics;
        }

        public static T ToModel<T>(this string content) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                return null;
            }
        }
    }
}