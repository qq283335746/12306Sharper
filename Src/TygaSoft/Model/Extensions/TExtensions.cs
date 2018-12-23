using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TygaSoft.Model
{
    public static class TExtensions
    {
        public static IEnumerable<EnumAttrInfo> ToEnumAttrs(this Type enumType)
        {
            var list = new List<EnumAttrInfo>();
            var values = Enum.GetValues(enumType);
            foreach (var value in values)
            {

                var name = Enum.GetName(enumType, value);
                var attrs = enumType.GetField(name).CustomAttributes;

                var attrInfo = new EnumAttrInfo
                {
                    Name = name,
                    Value = (int)value,
                    Description = attrs.FirstOrDefault()?.ConstructorArguments[0].Value.ToString()
                };
                list.Add(attrInfo);
            }

            return list;
        }
    }
}
