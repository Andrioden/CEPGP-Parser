using System;
using System.ComponentModel;
using System.Reflection;

namespace CepgpParser.Parser.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        /// Source: https://stackoverflow.com/a/4249887/686131
        /// </summary>
        public static T GetEnumValueFromDescription<T>(string description)
        {
            MemberInfo[] fis = typeof(T).GetFields();

            foreach (var fi in fis)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == description)
                    return (T)Enum.Parse(typeof(T), fi.Name);
            }

            throw new Exception($"Enum description '{description}' for enum {typeof(T)} not found.");
        }
    }
}
