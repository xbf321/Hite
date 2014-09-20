using System;
using System.Xml.Linq;


using Hite.Model;

namespace Hite.Mvc
{
    public static class LanguageResourceHelper
    {
        public static string GetString(string key,WebLanguage language) {
            string path = string.Format("Language/{0}.xml",language);
            XElement items = XElement.Load(String.Concat(System.AppDomain.CurrentDomain.BaseDirectory, path));
            var list = items.Elements("item");
            string value = string.Empty;
            foreach(var item in list){
                string _key = (string)item.Attribute("key");
                string _value = item.Value;
                if(key.ToLower() == _key.ToLower()){
                    value = _value;
                    break;
                }
            }
            return value;
        }
    }
}
