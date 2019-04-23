namespace UVAnimEditor
{
    public class Program
    {
        [System.Runtime.InteropServices.DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        public struct MousePoint
        {
            public int X;
            public int Y;
            public MousePoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public extern static bool GetCursorPos(out MousePoint pt);

        static public void SaveUVAnim(UVAnimResourceInfo uvResInfo)
        {
            if (uvResInfo == null || uvResInfo.UVAnim == null)
                return;            
            
            uvResInfo.GetSnapshotImage(true);

            if (string.IsNullOrEmpty(uvResInfo.UVAnim.UVAnimName))
            {
                EditorCommon.MessageBox.Show("UVAnimName未设置，请设置后再保存!");
                return;
            }

            UISystem.UVAnimMgr.Instance.SaveUVAnim(uvResInfo.UVAnim.Id);
            EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
            {
                if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{uvResInfo.ResourceTypeName}{uvResInfo.Name} {uvResInfo.AbsResourceFileName}使用版本控制上传失败!");
                }
                else
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                    {
                        if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{uvResInfo.ResourceTypeName}{uvResInfo.Name} {uvResInfo.AbsResourceFileName}使用版本控制上传失败!");
                        }
                    }, uvResInfo.AbsResourceFileName, $"AutoCommit 修改{uvResInfo.ResourceTypeName}{uvResInfo.Name}");

                    EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExt) =>
                    {
                        if (resultSnapshotExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{uvResInfo.ResourceTypeName}{uvResInfo.Name} {uvResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制上传失败!");
                        }
                        else
                        {
                            EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtCommit) =>
                            {
                                if (resultSnapshotExtCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                {
                                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{uvResInfo.ResourceTypeName}{uvResInfo.Name} {uvResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制上传失败!");
                                }
                            }, uvResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 修改{uvResInfo.ResourceTypeName}{uvResInfo.Name}缩略图");
                        }
                    }, uvResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                }
            }, uvResInfo.AbsResourceFileName);


        }

    }
}
