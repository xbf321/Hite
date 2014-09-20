using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class JobService
    {
        public static JobInfo Update(JobInfo model) {
            if (model.Id > 0)
            {
                //UPDATE
                JobManage.Update(model);
            }
            else {
                int id = JobManage.Add(model);
                model.Id = id;
            }
            return model;
        }
        public static JobInfo Get(int id) {
            return JobManage.Get(id);
        }
        public static IPageOfList<JobInfo> List(SearchSetting settings){
            return JobManage.List(settings);
        }
    }
}
