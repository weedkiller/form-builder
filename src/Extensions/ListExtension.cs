﻿using System.Collections.Generic;
using System.Linq;

namespace form_builder.Extensions
{
    public static class ListExtension
    {
        public static string ToReadableFileType(this List<string> value)
        {
            value = value.Select(_ => _.Replace(".", string.Empty)).ToList();
            return value.Count > 1
                  ? string.Join(", ", value.Take(value.Count - 1)).ToUpper() + $" or {value.Last().ToUpper()}"
                  : value.First().ToUpper();
        }
    }
}
