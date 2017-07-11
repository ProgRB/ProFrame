using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public class UniDbTypeHelper
    {
        /// <summary>
        /// Возвращает тип параметра по типу C#
        /// </summary>
        /// <param name="sharpType"></param>
        /// <returns></returns>
        public static UniDbType GetUniDbType(Type sharpType)
        {
            if (sharpType == typeof(string))
                return UniDbType.String;
            if (sharpType == typeof(decimal) || sharpType == typeof(float))
                return UniDbType.Decimal;
            if (sharpType == typeof(int) || sharpType == typeof(short))
                return UniDbType.Int;
            if (sharpType == typeof(DateTime))
                return UniDbType.DateTime;
            if (sharpType == typeof(byte[]))
                return UniDbType.Blob;
            if (sharpType == typeof(decimal[]))
                return UniDbType.ArrayNumber;
            return UniDbType.String;
        }

    }
}
