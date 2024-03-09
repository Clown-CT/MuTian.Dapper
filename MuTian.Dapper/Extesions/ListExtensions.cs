using MuTian.Dapper.Attributes;
using System.Data;
using System.Reflection;

namespace MuTian.Dapper.Extesions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Convert a List{T} to a DataTable.
        /// </summary>
        public static DataTable ToDataTable<T>(this List<T> modelList)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(c => !c.CustomAttributes.Any(x => x.AttributeType == typeof(NotToDataTableAttribute))).ToArray();
            foreach (PropertyInfo prop in props)
            {
                Type t = prop.PropertyType;
                if (t.IsEnum)
                {
                    t = t.GetEnumUnderlyingType();
                }
                else if (t != null
                    && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && t.IsValueType)
                {
                    t = Nullable.GetUnderlyingType(t) ?? throw new NullReferenceException("GetUnderlyingType method return null value!");
                }
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in modelList)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    object? val = props[i].GetValue(item);
                    if (props[i].PropertyType.IsEnum && val != null)
                    {
                        values[i] = (int)val;
                    }
                    values[i] = val ?? DBNull.Value;
                }
                tb.Rows.Add(values);
            }
            return tb;
        }
    }
}
