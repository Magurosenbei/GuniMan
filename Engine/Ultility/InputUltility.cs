using System;
using System.Collections.Generic;
using System.Reflection;


namespace Engine
{
    public static class InputUltility
    {
        public static List<T> GetEnumValues<T>()
        {
            Type currentEnum = typeof(T);
            List<T> result = new List<T>();
            if (currentEnum.IsEnum)
            {
                FieldInfo[] field = currentEnum.GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo FI in field)
                    result.Add((T)FI.GetValue(null));
            }
            else
                throw new Exception("Paramemeter must be a enum, The parameter is not enum  type");
            return result;
        }
    }
}
