using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Hite.Common.Reflection;

namespace Hite.Model
{
    public class SiteInfo
    {
        public int Id { get; set; }
        [DisplayName("名称")]
        [Required(ErrorMessage="名称不能为空")]
        [DbField(Size=50)]
        public string Name { get; set; }
        [DbField(Size = 100)]
        public string Url { get; set; }
        [DbField(Size=200)]
        public string Logo { get; set; }
        public IndexFileNameOfSite IndexFileName { get; set; }
        [DbField(Size = 100)]
        public string WebTitle { get; set; }
        [DbField(Size = 200)]
        public string WebKeywords { get; set; }
        [DbField(Size = 200)]
        public string WebDesc { get; set; }
        [DbField(Name = "WebLanguage")]
        public WebLanguage Language { get; set; }
        public string Introduction { get; set; }
        public string ContactUs { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ThirdCode { get; set; }

        public SiteInfo() {
            CreateDateTime = DateTime.Now;
        }
    }
}
