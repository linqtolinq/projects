using System.Text.RegularExpressions;

namespace MQTT_Api_Server_Lifesaver.Extention
{
    public static class ExtensionFunc
    {
        /// <summary>
        /// 移除特殊字符[空格、换行、制表符]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWhitespaces(this string input)
        {
            return Regex.Replace(input, @"\s+", string.Empty);
        }
    }
}
