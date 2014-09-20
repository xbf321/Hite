using System;

namespace Hite.Common.Reflection
{
    /// <summary>
    /// 反射属性时可用此标签名称代替属性名称映射数据库字段
    /// 并且可设置字段长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DbFieldAttribute : Attribute
    {
        public string Name { get; set; }
        public int Size { get; set; }
    }
}
