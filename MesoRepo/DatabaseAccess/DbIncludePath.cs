using System;
using System.Linq.Expressions;

namespace DatabaseAccess {
    public class DbIncludePath<T> {
        public Expression<Func<T, object>> Expression { get; set; }
    }
}