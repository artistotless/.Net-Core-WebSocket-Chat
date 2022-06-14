
using System;

namespace AdminPanel.App.Models.Abstract
{
    public class Response<T> : IEquatable<T>
    {
        public T Result { get; set; }
        public bool HasError { get; set; } = false;
        public virtual string Message { get; set; }

        public bool Equals(T? other)
        {
            return Result.Equals(other);
        }
    }
}
