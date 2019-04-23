namespace MeshTemplateEditor
{
    public class Program
    {
        public static string ResourceType
        {
            get { return "MeshTemplate"; }
        }

        public static string SocketDragType
        {
            get { return "MeshSocket"; }
        }

        public static void SaveMeshTemplate(MeshTemplateResourceInfo res)
        {
            if (res == null || res.MeshTemplate == null)
                return;

            res.GetSnapshotImage(true);

            CCore.Mesh.MeshTemplateMgr.Instance.SaveMeshTemplate(res.MeshTemplate.MeshID);

            EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
            {
                if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{res.ResourceTypeName}{res.Name} {res.AbsResourceFileName}使用版本控制提交失败!");
                }
                else
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                    {
                        if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{res.ResourceTypeName}{res.Name} {res.AbsResourceFileName}使用版本控制提交失败!");
                        }
                    }, res.AbsResourceFileName, $"AutoCommit 修改{res.ResourceTypeName}{res.Name}");

                    EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExt) =>
                    {
                        if (resultSnapshotExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{res.ResourceTypeName}{res.Name}缩略图 {res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制提交失败!");
                        }
                        else
                        {
                            EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtCommit) =>
                            {
                                if (resultSnapshotExtCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                {
                                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{res.ResourceTypeName}{res.Name}缩略图 {res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制提交失败!");
                                }
                            }, res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 修改{res.ResourceTypeName}{res.Name}缩略图");
                        }
                    }, res.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                }
            }, res.AbsResourceFileName);
        }
    }
}
