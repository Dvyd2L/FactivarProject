namespace Helpers.Enums;

public enum EnumRoles
{
    Admin = 1,
    User = 2
}

public enum Test : int
{
    [StringValue("a")]
    Foo = 1,

    [StringValue("b")]
    Something = 2
}

[AttributeUsage(AttributeTargets.Field)]
public class StringValueAttribute(string value) : Attribute
{
    public string Value => value;
}

public static class EnumExtensions
{
    public static string? GetStringValue(this Enum value)
    {
        Type type = value.GetType();
        System.Reflection.FieldInfo? fieldInfo = type.GetField(value.ToString());
        StringValueAttribute[]? attribute = fieldInfo?.GetCustomAttributes(
            typeof(StringValueAttribute), false
        ) as StringValueAttribute[];
        return attribute?.Length > 0 ? attribute[0].Value : null;
    }
}

public class Prueba
{
    public static void Main()
    {
        string? roleString = Test.Foo.GetStringValue();
        Console.WriteLine(roleString);
    }
}