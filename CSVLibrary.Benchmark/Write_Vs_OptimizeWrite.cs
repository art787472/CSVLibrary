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
    public class Write_Vs_OptimizeWrite
    {
        static PropertyInfo[] infos = typeof(DataModel).GetProperties();

        static StringBuilder sb = new StringBuilder(90);
        static char[] result = new char[90];

        static int PropCounts = infos.Length;
        delegate object GetterDelegate(object target);
        private static readonly GetterDelegate[] _getterDelegates =
infos.Select(p => CreateGetter(p)).ToArray();
        private static GetterDelegate CreateGetter(PropertyInfo property)
        {

            var targetType = typeof(object);

            var targetParam = Expression.Parameter(targetType, "target");


            var castTarget = Expression.Convert(targetParam, property.DeclaringType);


            var propertySetter = Expression.Call(castTarget, property.GetGetMethod());

            var lambda = Expression.Lambda<GetterDelegate>(propertySetter, targetParam);
            return lambda.Compile();
        }
        [Benchmark]
        public void Write()
        {

            var dataModel = new DataModel() { id = "1", first_name = "Andrey", last_name = "Wyborn", email = "awyborn0@eepurl.com", gender = "Male", ip_address = "230.108.222.114" };
            var fields = typeof(DataModel).GetProperties();

            string dataStr = String.Empty;
            foreach (var field in fields)
            {
                //Func<object, object>
                dataStr += $"{field.GetValue(dataModel)},";
            }

            dataStr = dataStr.TrimEnd(',');





        }
        [Benchmark]
        public void OptimizeWrite()
        {

            var dataModel = new DataModel() { id = "1", first_name = "Andrey", last_name = "Wyborn", email = "awyborn0@eepurl.com", gender = "Male", ip_address = "230.108.222.114" };
            
            //var sb = new StringBuilder();       
            for (int j = 0; j < PropCounts; j++)
            {
                sb.Append(_getterDelegates[j](dataModel));
                
                sb.Append(',');
            }

            //string result = sb.ToString(0, sb.Length - 1);
            sb.CopyTo(0, result, 0, sb.Length - 1);
            sb.Clear();

            // 1. 測量 StringBuilder 放在全域所占用的記憶體跟時間
            // 2. 測量 StringBuilder設定Size=90 放在全域所占用的記憶體跟時間
            // 3. 測量 sb.Append(","); 改成 sb.Append(','); 所占用的記憶體跟時間
        }
    }
}
