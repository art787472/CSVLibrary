using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace CSVLibrary.Benchmark
{
    [MemoryDiagnoser]
    public class Split_VS_Span
    {
        static PropertyInfo[] infos = typeof(DataModel).GetProperties();
        static int PropCounts = infos.Length;   
        delegate void SetterDelegate(object target, object value);
        private static readonly SetterDelegate[] _setterDelegates =
        infos.Select(p => CreateSetter(p)).ToArray();
        private static SetterDelegate CreateSetter(PropertyInfo property)
        {
            var targetType = typeof(object);
            var valueType = typeof(object);

            var targetParam = Expression.Parameter(targetType, "target");
            var valueParam = Expression.Parameter(valueType, "value");

            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var castValue = Expression.Convert(valueParam, property.PropertyType);

            var propertySetter = Expression.Call(castTarget, property.GetSetMethod(), castValue);

            var lambda = Expression.Lambda<SetterDelegate>(propertySetter, targetParam, valueParam);
            return lambda.Compile();
        }


        [Benchmark]
        public void Split() {
            string line = "1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114";
            string[] datas = line.Split(',');

            List<DataModel> list = new List<DataModel>();
            DataModel model = new DataModel();
            var props = typeof(DataModel).GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                props[i].SetValue(model, datas[i]);
            }

            list.Add(model);
        }

        [Benchmark]
        public void Span()
        {
            string line = "1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114";
            ReadOnlySpan<char> datas = line.AsSpan();
            DataModel dataModel = new DataModel();

            int start = 0;
            for (int i = 0; i < PropCounts; i++)
            {
                // 找逗號位置
                int commaIndex = datas.Slice(start).IndexOf(',');

                if (commaIndex == -1)
                {
                    // 最後一欄
                    _setterDelegates[i](dataModel, datas.Slice(start).ToString());
                    break;
                }
                else
                {
                    _setterDelegates[i](dataModel, datas.Slice(start, commaIndex).ToString());
                    start += commaIndex + 1;
                }
            }

            // 這裡就已經是完整填好的 DataModel
            List<DataModel> dataList = new List<DataModel> { dataModel };

        }
    }
}
