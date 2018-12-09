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

        public static T ToModel<T>(this string content) where T:class
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