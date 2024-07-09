using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary
{
    public class CSVHelper
    {
        public static List<T> Read<T>(string path) where T : new()
        {
            var fileNames = path.Split('\\').ToList();
            string fileName = fileNames.Last();
            fileNames.Remove(fileNames.Last());
            string directoryPath = string.Join("\\", fileNames);
            if (!Directory.Exists(directoryPath))
            {
                throw new Exception("檔案夾不存在!");
            }
            var temp = Path.GetExtension(fileName);
            if (Path.GetExtension(fileName) != ".csv")
            {
                throw new Exception("必須是csv檔案");
            }

            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            List<T> list = new List<T>();
            var line = sr.ReadLine();
            var headerNames = line.Split(',');

            var fieldTyps = typeof(T).GetProperties();
            Dictionary<PropertyInfo, int> dict = new Dictionary<PropertyInfo, int>(); // 欄位跟 csv 的第幾筆資料的對應關係
            foreach (var f in fieldTyps)
            {
                string displayName = f.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                if (headerNames.Contains(displayName))
                {
                    dict.Add(f, Array.IndexOf(headerNames, displayName));
                    continue;
                }
                if (headerNames.Contains(f.Name))
                {
                    dict.Add(f, Array.IndexOf(headerNames, f.Name));
                    continue;
                }
            }
            line = sr.ReadLine();
            while (line != null)
            {
                var fields = line.Split(',');
                T item = new T();
                foreach (var info in dict.Keys)
                {
                    int idx = dict[info];
                    info.SetValue(item, Convert.ChangeType(fields[idx], info.PropertyType));
                }


                list.Add(item);

                line = sr.ReadLine();
            }
            sr.Close();
            return list;
        }

        public static void Write<T>(List<T> list, string path, bool hasHeader = true) where T : new()
        {
            var fileNames = path.Split('\\').ToList();
            string fileName = fileNames.Last();
            fileNames.Remove(fileNames.Last());
            string directoryPath = string.Join("\\", fileNames);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (Path.GetExtension(fileName) != ".csv")
            {
                throw new Exception("必須是csv檔案");
            }

            // 看第一筆資料是不是標題
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            var firstLine = sr.ReadLine();
            var names = firstLine.Split(',');
            var fields = list[0].GetType().GetProperties();
            if (names[0] == fields[0].Name || names[0] == fields[0].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName)
            {
                hasHeader = false; // 如果本來就有標題就不用把標題加入檔案
            }
            sr.Close();

            StreamWriter sw = new StreamWriter(path , true, Encoding.UTF8);

            if(hasHeader)
            {
                string headerStr = String.Empty;

                foreach(var field in fields)
                {
                    string displayName = field.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    if(displayName == null)
                    {
                        headerStr += $"{field.Name},";
                    }
                    headerStr += $"{displayName},";
                }
                headerStr = headerStr.TrimEnd(',');
                sw.WriteLine(headerStr);

            }

            foreach (T i in list)
            {
                string dataStr = String.Empty;
                foreach(var field in fields)
                {
                    dataStr += $"{field.GetValue(i)},";
                }

                dataStr = dataStr.TrimEnd(',');
                
                sw.WriteLine(dataStr);
            }
            sw.Close();
        }

        public static void Write<T>(T item, string path, bool hasHeader = true) where T:new()
        {
            Write<T>(new List<T> { item }, path, hasHeader);
        }
    }
}
