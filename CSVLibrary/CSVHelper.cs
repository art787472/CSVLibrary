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


            bool validatePath = ValidatePath(path);
            if(!validatePath)
            {
                throw new Exception("檔案夾不存在!");
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

            

            if(dict.Count == 0)
            {
                for(int i = 0; i < fieldTyps.Length; i++)
                {
                    dict[fieldTyps[i]] = i;
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
                    if(fields.Length <= idx)
                    {
                        break;
                    }
                    info.SetValue(item, Convert.ChangeType(fields[idx], info.PropertyType));
                }


                list.Add(item);

                line = sr.ReadLine();
            }
            sr.Close();
            return list;
        }

        private static (bool,HeaderCategory) CheckHasHeader<T>(StreamReader sr, out string header)
        {
            // 看第一筆資料是不是標題
            
            var firstLine = sr.ReadLine();
            var fields = typeof(T).GetProperties();
            header = String.Empty;
            bool hasHeader = false;
            HeaderCategory type = HeaderCategory.NoHeader;

            // 製作標題傳給 header
            foreach (var field in fields)
            {
                string displayName = field.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                if (displayName == null)
                {
                    header += $"{field.Name},";
                }
                else
                {
                    header += $"{displayName},";
                }
            }
            header = header.TrimEnd(',');
            if (firstLine == null)
            {
                type = HeaderCategory.EmptyFile;
            }
            else
            {
                var names = firstLine.Split(',');
                if (names[0] == fields[0].Name || names[0] == fields[0].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName)
                {
                    hasHeader = true;
                    type = HeaderCategory.HasHeader;
                }

            }



            sr.Close();
            return (hasHeader, type);


        }

        public static void Write<T>(List<T> list, string path, bool addHeader = true) where T : new()
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
            var (hasHeader, type) = CheckHasHeader<T>(sr, out string headerStr);

            //StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            
            
            if(type == HeaderCategory.EmptyFile)
            {
                StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
                sw.WriteLine(headerStr);
                WriteData<T>(sw, list);
                sw.Close();
            }
            if(type == HeaderCategory.HasHeader)
            {
                StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
                WriteData<T>(sw, list);
                sw.Close();
            }
            if(type == HeaderCategory.NoHeader)
            {
                string originalData = ReadData(path);
                StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
                sw.WriteLine(headerStr);
                sw.Write(originalData);
                WriteData<T>(sw, list);
                sw.Close() ;
            }


            
        }

        public static void Write<T>(T item, string path, bool hasHeader = true) where T:new()
        {
            Write<T>(new List<T> { item }, path, hasHeader);
        }

        private static bool ValidatePath(string path)
        {
            var fileNames = path.Split('\\').ToList();
            string fileName = fileNames.Last();
            fileNames.Remove(fileNames.Last());
            string directoryPath = string.Join("\\", fileNames);
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }
            var temp = Path.GetExtension(fileName);
            if (Path.GetExtension(fileName) != ".csv")
            {
                throw new Exception("必須是csv檔案");
            }
            return true;
        }

        private static string ReadData(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string res = sr.ReadToEnd();
            sr.Close();
            return res;
        }

        private static void WriteData<T>(StreamWriter sw, List<T> list)
        {
            var fields = typeof(T).GetProperties();
            foreach (T i in list)
            {
                string dataStr = String.Empty;
                foreach (var field in fields)
                {
                    dataStr += $"{field.GetValue(i)},";
                }

                dataStr = dataStr.TrimEnd(',');

                sw.WriteLine(dataStr);
            }
            sw.Close();
        }
    }
}
