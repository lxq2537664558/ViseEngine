using System;

namespace EffectEditor
{
    class Program
    {
        public static string GetDescription(object obj)
        {
            if (obj == null)
                return "";

            var _type = obj.GetType();
            if (_type.IsEnum)
            {
                var fi = _type.GetField(System.Enum.GetName(_type, obj));
                if (fi != null)
                {
                    var dna = Attribute.GetCustomAttribute(fi, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
                    if (dna != null && !string.IsNullOrEmpty(dna.Description))
                        return dna.Description;
                }
            }

            return _type.Name;
        }

        public static void SaveEffectTemplate(EffectResourceInfo res)
        {
            if (res == null || res.EffectTemplate == null)
                return;

            //res.GetSnapshotImage(true);

            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult updateResult) =>
                {
                    if (updateResult.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{res.ResourceTypeName}{res.Name} {res.AbsResourceFileName}使用版本控制更新失败!");
                    }
                    else
                    {
                        CCore.Effect.EffectManager.Instance.SaveEffectTemplate(res.EffectTemplate.Id);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{res.ResourceTypeName}{res.Name} {res.AbsResourceFileName}使用版本控制提交失败!");
                            }
                        }, res.AbsResourceFileName, $"AutoCommit 修改{res.ResourceTypeName}{res.Name}");

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult snapUpdateResult) =>
                        {
                            if (snapUpdateResult.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{res.ResourceTypeName}{res.Name}缩略图 {res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制更新失败!");
                            }
                            else
                            {
                                res.ParentBrowser.ReCreateSnapshot(res);
                                EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                                {
                                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{res.ResourceTypeName}{res.Name}缩略图 {res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制提交失败!");
                                    }
                                }, res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 修改{res.ResourceTypeName}{res.Name}缩略图");
                            }
                        }, res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                    }
                }, res.AbsResourceFileName);
            }
            else
            {
                CCore.Effect.EffectManager.Instance.SaveEffectTemplate(res.EffectTemplate.Id);
                res.ParentBrowser.ReCreateSnapshot(res);
            }
        }
    }
}
