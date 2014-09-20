using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class AttachmentService
    {
        public static AttachmentInfo Update(AttachmentInfo model) {
            if (model.Id > 0)
            {
                AttachmentManage.Update(model);
            }
            else {
                int id = AttachmentManage.Add(model);
                model.Id = id;
            }
            return model;
        }
        public static AttachmentInfo Get(int id) {
            return AttachmentManage.Get(id);
        }
        public static IPageOfList<AttachmentInfo> List(SearchSetting settings) {
            return AttachmentManage.List(settings);
        }
        public static void UpdateDownloadCount(int id) {
            AttachmentManage.UpdateDownloadCount(id);
        }
    }
}
