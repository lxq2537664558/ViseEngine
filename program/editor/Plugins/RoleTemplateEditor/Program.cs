namespace RoleTemplateEditor
{
    public class Program
    {
        public static void SaveRoleTemplate(RoleTemplateResourceInfo roleResInfo)
        {
            if (roleResInfo == null || roleResInfo.RoleTemplate == null)
                return;

            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            { 
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult updateResult) =>
                {
                    if (updateResult.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{roleResInfo.ResourceTypeName}{roleResInfo.Name} {roleResInfo.AbsResourceFileName}使用版本控制更新失败!");
                    }
                    else
                    {
                        CSUtility.Data.RoleTemplateManager.Instance.SaveRoleTemplate(roleResInfo.RoleTemplate.Id);
                        //roleResInfo.HostControl.Browser.ReCreateSnapshot(roleResInfo);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{roleResInfo.ResourceTypeName}{roleResInfo.Name} {roleResInfo.AbsResourceFileName}使用版本控制提交失败!");
                            }
                        }, roleResInfo.AbsResourceFileName, $"AutoCommit 修改{roleResInfo.ResourceTypeName}{roleResInfo.Name}");

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update(null, roleResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{roleResInfo.ResourceTypeName}{roleResInfo.Name}缩略图 {roleResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制提交失败!");
                            }
                        }, roleResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 修改{roleResInfo.ResourceTypeName}{roleResInfo.Name}缩略图");

                        // 通知服务器更新模板
            #warning 通知服务器更新模板
                        //var args = new string[2];
                        //args[0] = "Role";
                        //args[1] = roleResInfo.RoleTemplate.Id.ToString();
                        //FrameSet.GMCommandManager.Instance.GMExecute("ReloadTemplate", args);

                    }
                }, roleResInfo.AbsResourceFileName);
            }
            else
            {
                CSUtility.Data.RoleTemplateManager.Instance.SaveRoleTemplate(roleResInfo.RoleTemplate.Id);
                //roleResInfo.HostControl.Browser.ReCreateSnapshot(roleResInfo);

                // 通知服务器更新模板
    #warning 通知服务器更新模板
                //var args = new string[2];
                //args[0] = "Role";
                //args[1] = roleResInfo.RoleTemplate.Id.ToString();
                //FrameSet.GMCommandManager.Instance.GMExecute("ReloadTemplate", args);
            }

        }
    }
}
