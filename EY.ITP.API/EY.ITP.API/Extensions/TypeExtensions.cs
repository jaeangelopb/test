using EY.ITP.API.Models.Entities;
using System.Data;
using System.Reflection;

namespace EY.ITP.API.Extensions
{
    public static class TypeExtensions
    {
        public static SqlDbType ToSqlDbType(this StoredProcParameter parameter)
        {
            if (SqlDbTypeMap.ContainsKey(parameter.Data_Type))
            {
                return SqlDbTypeMap[parameter.Data_Type];
            }
            throw new ArgumentException($"{parameter.Data_Type} is not a supported");
        }

        public static ParameterDirection ToParameterDirection(this StoredProcParameter parameter)
        {
            if (ParameterDirectionMap.ContainsKey(parameter.Parameter_Mode))
            {
                return ParameterDirectionMap[parameter.Parameter_Mode];
            }
            else
            {
                return ParameterDirection.Input;
            }
        }

        public static Type ToType(this TableTypeColumn type)
        {
            if (CustomTableTypeMap.ContainsKey(type.Column_Type))
            {
                return CustomTableTypeMap[type.Column_Type];
            }

            throw new ArgumentException($"{type.Column_Type} is not a supported custom table type column");
        }

        public static object ToDefaultValue(this PropertyInfo propInfo, object obj)
        {
            var val = propInfo.GetValue(obj);
            if (val != null)
            {
                if (PropertyInfoDefaulValueMap.ContainsKey(val.GetType()))
                {
                    return PropertyInfoDefaulValueMap[val.GetType()];
                }
            }
            return DBNull.Value;
        }

        private static readonly IDictionary<Type, object> PropertyInfoDefaulValueMap = new Dictionary<Type, object>()
        {
            {typeof(bool), false}
        };

        private static readonly IDictionary<string, SqlDbType> SqlDbTypeMap = new Dictionary<string, SqlDbType>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"Int",     SqlDbType.Int   },
            {"Date",    SqlDbType.DateTime},
            {"DateTime",    SqlDbType.DateTime  },
            {"NVarChar",    SqlDbType.NVarChar  },
            {"Float",   SqlDbType.Float }
        };

        private static readonly IDictionary<string, ParameterDirection> ParameterDirectionMap = new Dictionary<string, ParameterDirection>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"out", ParameterDirection.Output },
            {"inout", ParameterDirection.InputOutput },
            {"in", ParameterDirection.Input}
        };

        private static readonly IDictionary<string, Type> CustomTableTypeMap = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"Int", typeof(int)},
            {"Bit", typeof(bool)},
            {"Date", typeof(DateTime) },
            {"DateTime", typeof(DateTime)},
            {"NVarChar", typeof(string) },
            {"Float", typeof(double)},
            {"Numeric", typeof(decimal)}
        };
    }
}
