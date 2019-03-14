using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KPIASPNETCore.DTO
{
    public class Imagedto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Author { get; set; }
        public int Rate { get; set; }        
    }

    public static class Jsonuse<T> where T : class
    {
        public static IList<T> Read(string filePath)
        {
            return JsonConvert.DeserializeObject<IList<T>>(File.ReadAllText(filePath));
        }
        public static void Write(string filePath, IList<T> models)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(models));
        }
    }
}
