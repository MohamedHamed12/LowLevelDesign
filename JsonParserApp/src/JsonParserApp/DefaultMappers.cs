using System;
using System.Linq;
using System.Reflection;
using JsonParserApp.Elements;

namespace JsonParserApp
{
    internal static class DefaultMappers
    {
        public static object? ObjectToType(JsonObject obj, Type target)
        {
            if (target == typeof(object)) return obj;
            // simple mapping for POCOs: match public writable properties by name (case-insensitive)
            var inst = Activator.CreateInstance(target) ?? throw new JsonParseException($"Cannot create instance of {target.FullName}");
            var props = target.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                          .Where(p => p.CanWrite).ToArray();
            foreach (var p in props)
            {
                if (obj.Members.TryGetValue(p.Name, out var val))
                {
                    object? converted = ConvertElementToType(val, p.PropertyType);
                    p.SetValue(inst, converted);
                }
            }
            return inst;
        }

        public static T PrimitiveToType<T>(IJsonElement el)
        {
            object? o = ConvertElementToType(el, typeof(T));
            return (T)o!;
        }

        private static object? ConvertElementToType(IJsonElement el, Type target)
        {
            if (el is JsonString s)
            {
                if (target == typeof(string)) return s.Value;
                if (target == typeof(char)) return s.Value.Length > 0 ? s.Value[0] : '\0';
                // attempt System.Convert change
                return Convert.ChangeType(s.Value, target);
            }
            if (el is JsonNumber n)
            {
                if (target == typeof(double)) return n.Value;
                return Convert.ChangeType(n.Value, target);
            }
            if (el is JsonBoolean b)
            {
                if (target == typeof(bool)) return b.Value;
                return Convert.ChangeType(b.Value, target);
            }
            if (el is JsonNull) return null;
            if (el is JsonObject o)
            {
                return ObjectToType(o, target);
            }
            // arrays and others not implemented here (could be added)
            throw new JsonParseException($"Cannot convert element to {target.Name}");
        }
    }
}
