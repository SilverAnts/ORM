using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq.Expressions;
using SilverAnts.Element;
using System.Globalization;

namespace SilverAnts.Core.Mapper
{

    public class EMapper
    {

        #region 字段
        private static readonly MethodInfo fnGetItem =
                typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo fnIsDBNull =
                typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        #endregion

        #region 方法
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reader"></param>
        /// <param name="startBound"></param>
        /// <param name="length"></param>
        /// <param name="returnNullIfFirstMissing"></param>
        /// <returns></returns>
        public static Func<IDataReader, object> GetDeserializer(Type type, IDataReader reader)
        {
            //dymanic
            if (type == typeof(object))
            {
                return GetDynamicDeserializer(reader);
            }
            //基本类型
            if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
            {
                return GetStructDeserializer(reader, type);
            }
            //类
            else
            {
                return GetClassDeserializer(reader, type);
            }
        }
        /// <summary>
        /// 实例化--动态类
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static Func<IDataReader, object> GetDynamicDeserializer(IDataReader r)
        {
            return (dr) =>
            {
                //列名-属性名
                var names = Enumerable.Range(0, dr.FieldCount).Select(i => dr.GetName(i)).ToArray();
                //列/属性值
                var values = Enumerable.Range(0, dr.FieldCount).Select(i => dr.GetValue(i) is DBNull ? null : dr.GetValue(i)).ToArray();
                return new DymaicRow(new DymaicFields(names), values);
            };
        }
        /// <summary>
        /// 实体类
        /// </summary>
        /// <param name="r"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Func<IDataReader, object> GetStructDeserializer(IDataReader r, Type type)
        {
            if (type.IsEnum)
            {    
                return (dr) =>
                {
                    var val = dr.GetValue(0);
                    if (val is float || val is double || val is decimal)
                    {
                        val = Convert.ChangeType(val, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture);
                    }
                    return val is DBNull ? null : Enum.ToObject(type, val);
                };
            }
            else
            {
                return (dr) =>
                {
                    var val = dr.GetValue(0);
                    return val is DBNull ? null :val;
                };
            }
        }
         /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<IDataReader, object> GetClassDeserializerA(IDataReader r, Type type)
        {
            return (dr) =>
            {
                var entity = Activator.CreateInstance(type);

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    PropertyInfo item = type.GetProperty(dr.GetName(i));
                    if (item == null)
                        continue;
                    //给属性赋值
                    item.SetValue(entity, Convert.ChangeType(dr[i], item.PropertyType), null);
                }
                return entity;
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<IDataReader, object> GetClassDeserializer(IDataReader dr, Type type)
        {
            //查询结果-实体类成员
            var columns = CacheMapper.GetColumnsMapper(type);
            //创建动态方法
            DynamicMethod dm = new DynamicMethod(string.Format("To{0}Entity", type.Name), type, new[] { typeof(IDataReader) }, typeof(EMapper));
            //IL
            ILGenerator il = dm.GetILGenerator();
            //-try
            //il.BeginExceptionBlock();
            //- new T()  实例化
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            //异常
            for (int i = 0; i < dr.FieldCount; i++)
            {
                var column = columns.Single(o => o.ColumnName == dr.GetName(i));
                if (column == null)
                {
                    Logger.WriteLog(string.Format("当前DataReader->列{0}未与实体类{1}对应", dr.GetName(i), type.Name));
                    continue;
                }
                //类型
                Type fieldType = dr.GetFieldType(i);
                Type itemType = column.ColumnType;
                //标记
                Label endIfLabel = il.DefineLabel();
                //if(idr[i]==DBNull.Value)
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Callvirt, fnIsDBNull);
                //跳出
                il.Emit(OpCodes.Brtrue, endIfLabel);
                //获得当前值
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Callvirt, fnGetItem);
                //是否使用Nullable  如：DateTime？
                Type nullUnderlyingType = Nullable.GetUnderlyingType(itemType);
                Type unboxType = nullUnderlyingType != null ? nullUnderlyingType : itemType;
                //--
                if (unboxType == fieldType)
                {
                    il.Emit(OpCodes.Unbox_Any, unboxType);
                    if (nullUnderlyingType != null)
                    {
                        il.Emit(OpCodes.Newobj, itemType.GetConstructor(new[] { nullUnderlyingType }));
                    }
                }
                else
                {
                    if (unboxType.IsEnum)
                    {

                    }
                    //il.Emit(OpCodes.Castclass, unboxType);
                    //其他
                    Logger.WriteLog(string.Format("当前DataReader->列{0}未与实体类{2}->{1}类型不匹配", dr.GetName(i), column.ColumnName, type.Name));
                }
                //赋值
                il.Emit(OpCodes.Callvirt, column.PropertyInfo.GetSetMethod(true));
                il.MarkLabel(endIfLabel);
            }
            il.Emit(OpCodes.Ret);
            return (Func<IDataReader, object>)dm.CreateDelegate(typeof(Func<IDataReader, object>));
        }
        #endregion
    }
}