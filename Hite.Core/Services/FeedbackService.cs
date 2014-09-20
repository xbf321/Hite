using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class FeedbackService
    {
        public static FeedbackInfo Create(FeedbackInfo model) {
            if(model.Id == 0){
                int id = FeedbackManage.Add(model);
                model.Id = id;
            }
            return model;
        }
        public static IPageOfList<FeedbackInfo> List(SearchSetting settings)
        {
            return FeedbackManage.List(settings);
        }
    }
}
