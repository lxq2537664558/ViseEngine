using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SubversionPlugin
{
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "SVNCommand")]
    [Guid("802D959F-9FFE-463F-9903-029F2308E707")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    //[EditorCommon.VersionControl.VersionControl(VersionControlType = "SVN")]
    internal class SVNCommand : EditorCommon.VersionControl.IVersionControl, EditorCommon.PluginAssist.IEditorPlugin
    {
        public string PluginName
        {
            get { return "PrefabBrowser"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "PrefabBrowser",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {
        }

        bool mValid = false;
        public bool Valid
        {
            get
            {
                if (!mSvnAvailableChecked)
                    mValid = CheckSVNAvaliable();

                return mValid;
            }
        }

        bool mSvnAvailableChecked = false;
        private bool CheckSVNAvaliable()
        {
            mSvnAvailableChecked = true;
            var result = StartProcess("help");
            switch(result)
            {
                case EditorCommon.VersionControl.EProcessResult.Exception:
                    return false;
            }

            return true;
        }

        public string VersionControlType
        {
            get { return "SVN"; }
        }

        string mOutputString = "";

        // 获得文件的SVN状态(只处理文件，不处理目录)
        public EditorCommon.VersionControl.EStatus GetSVNStatus(string svnInfo)
        {
            if (string.IsNullOrEmpty(svnInfo))
                return EditorCommon.VersionControl.EStatus.Normal;

            var splits = svnInfo.Split(' ');
            if (splits.Length < 2)
                return EditorCommon.VersionControl.EStatus.Unknow;

            switch (splits[0])
            {
                case "L":
                    return EditorCommon.VersionControl.EStatus.Lock;
                case "M":
                    return EditorCommon.VersionControl.EStatus.Modify;
                case "?":
                    return EditorCommon.VersionControl.EStatus.NotControl;
                case "!":
                    return EditorCommon.VersionControl.EStatus.Lost;
                case "~":
                    return EditorCommon.VersionControl.EStatus.TypeChanged;
                case "I":
                    return EditorCommon.VersionControl.EStatus.Ignore;
                case "A":
                    return EditorCommon.VersionControl.EStatus.Add;
                case "D":
                    return EditorCommon.VersionControl.EStatus.Delete;
                case "C":
                    return EditorCommon.VersionControl.EStatus.Conflict;
                case "R":
                    return EditorCommon.VersionControl.EStatus.Replace;
                case "S":
                    return EditorCommon.VersionControl.EStatus.Branch;
            }

            if (svnInfo.Contains("was not found"))
                return EditorCommon.VersionControl.EStatus.NotControl;

            return EditorCommon.VersionControl.EStatus.Unknow;
        }

        private EditorCommon.VersionControl.EProcessResult StartProcess(string startArgs)
        {
            try
            {
                mOutputString = "";
                mProcessOutputStr = "";
                mProcessErrorStr = "";

                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.Arguments = startArgs;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                //startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                startInfo.FileName = "svn";

                var currentProcess = new System.Diagnostics.Process();
                currentProcess.StartInfo = startInfo;
                //currentProcess.EnableRaisingEvents = true;
                currentProcess.OutputDataReceived += _OnOutputDataReceived;
                currentProcess.ErrorDataReceived += _OnErrorDataReceived;
                //currentProcess.ErrorDataReceived += OnInternalErrorDataReceived;
                //currentProcess.Exited += OnInternalCommandExited;
                currentProcess.Start();
                currentProcess.BeginOutputReadLine();
                currentProcess.BeginErrorReadLine();
                currentProcess.WaitForExit();

                var retCode = EditorCommon.VersionControl.EProcessResult.Unknow;

                //var errorStr = currentProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(mProcessErrorStr))
                {
                    retCode = EditorCommon.VersionControl.EProcessResult.HasError;
                    mOutputString = mProcessErrorStr;
                }
                else
                {
                    retCode = EditorCommon.VersionControl.EProcessResult.Success;
                    mOutputString = mProcessOutputStr;// currentProcess.StandardOutput.ReadToEnd();
                }

                return retCode;

            }
            catch (System.Exception ex)
            {
                mOutputString = ex.ToString();
                return EditorCommon.VersionControl.EProcessResult.Exception;
            }
        }

        string mProcessOutputStr = "";
        private void _OnOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            mProcessOutputStr += string.IsNullOrEmpty(e.Data) ? "" : (e.Data + "\r\n");
        }
        string mProcessErrorStr = "";
        private void _OnErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            mProcessErrorStr += string.IsNullOrEmpty(e.Data) ? "" : (e.Data + "\r\n");
        }

        private string GetUrlsParameter(string[] urls)
        {
            string url = "";
            foreach (string u in urls)
            {
                var tempStr = u.Replace("\\", "/");
                url += " " + "\"" + tempStr.TrimEnd('\\', '/') + "\"";
            }
            return url.TrimStart();
        }

        #region Commands

        EditorCommon.VersionControl.EStatus CheckStatus(string url)
        {
            url = url.Replace("\\", "/");
            var result = StartProcess("status \"" + url + "\"");
            return GetSVNStatus(mOutputString);
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Info(string url, int specVersion = -1)
        {
            url = url.Replace("\\", "/");
            var cmd = "info \"" + url + "\"";
            if (specVersion > 0)
                cmd += " -r " + specVersion;
            var result = StartProcess(cmd);

            var state = CheckStatus(url);
            return new EditorCommon.VersionControl.VersionControlCommandResult(result, state);
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Update(string url, int specVersion = -1)
        {
            var cmd = "update \"" + url + "\" --non-interactive";
            if (specVersion > 0)
                cmd += " -r " + specVersion;

            var result = StartProcess(cmd);
            var state = CheckStatus(url);
            return new EditorCommon.VersionControl.VersionControlCommandResult(result, state);
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Commit(string url, string logs)
        {
            url = url.Replace("\\", "/");
            var result = Update(url);
            switch (result.State)
            {
                case EditorCommon.VersionControl.EStatus.Conflict:
                case EditorCommon.VersionControl.EStatus.Ignore:
                case EditorCommon.VersionControl.EStatus.Unknow:
                    return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, result.State);

                case EditorCommon.VersionControl.EStatus.NotControl:
                    {
                        // 不在版本控制则先进性添加然后上传
                        return Add(url, logs, true);
                    }

                default:
                    {
                        var cmd = "commit -m \"" + logs + "\" \"" + url + "\"";
                        var processResult = StartProcess(cmd);
                        var state = CheckStatus(url);
                        return new EditorCommon.VersionControl.VersionControlCommandResult(processResult, state);
                    }
            }
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Add(string url, string logs, bool withSub)
        {
            var state = CheckStatus(url);
            switch (state)
            {
                case EditorCommon.VersionControl.EStatus.Conflict:
                    return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.Conflict);

                case EditorCommon.VersionControl.EStatus.NotControl:
                    {
                        url = url.Replace("\\", "/");
                        var cmd = "add " + (withSub ? "" : "--non-recursive ") + "\"" + url + "\"";
                        var result = StartProcess(cmd);
                        return Commit(url, logs);
                    }

                default:
                    return Commit(url, logs);
            }
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Delete(string url, string logs)
        {
            var state = CheckStatus(url);
            switch (state)
            {
                case EditorCommon.VersionControl.EStatus.Conflict:
                    return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.Conflict);

                case EditorCommon.VersionControl.EStatus.Unknow:
                case EditorCommon.VersionControl.EStatus.NotControl:
                    {
                        if (System.IO.Directory.Exists(url))
                            System.IO.Directory.Delete(url, true);
                        else if (System.IO.File.Exists(url))
                            System.IO.File.Delete(url);

                        return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.Success, EditorCommon.VersionControl.EStatus.Unknow);
                    }

                default:
                    {
                        url = url.Replace("\\", "/");
                        var cmd = $"delete \"{url}\" --force";
                        var result = StartProcess(cmd);
                        return Commit(url, logs);
                    }
            }
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Move(string oldUrl, string newUrl, string logs)
        {
            oldUrl = oldUrl.Replace("\\", "/");
            newUrl = newUrl.Replace("\\", "/");

            if (string.IsNullOrWhiteSpace(oldUrl) || string.IsNullOrWhiteSpace(newUrl))
                return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.Invalid);

            if (string.Equals(oldUrl.ToLower(), newUrl.ToLower()))
                return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.Invalid);

            if (!Directory.Exists(oldUrl) && !File.Exists(oldUrl))
                return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.Lost);

            if (Directory.Exists(newUrl) || File.Exists(newUrl))
                return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.Exist);

            var state = CheckStatus(oldUrl);
            switch (state)
            {
                case EditorCommon.VersionControl.EStatus.Conflict:
                    return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.Conflict);

                case EditorCommon.VersionControl.EStatus.Unknow:
                case EditorCommon.VersionControl.EStatus.NotControl:
                    {
                        try
                        {
                            Directory.Move(oldUrl, newUrl);
                            return Add(newUrl, logs, true);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.Exception, EditorCommon.VersionControl.EStatus.Unauthorized);
                        }
                        catch (PathTooLongException)
                        {
                            return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.Exception, EditorCommon.VersionControl.EStatus.PathTooLong);
                        }
                        catch (Exception)
                        {
                            return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.Exception, EditorCommon.VersionControl.EStatus.Unknow);
                        }
                    }

                default:
                    {
                        var cmd = $"rename \"{oldUrl}\" \"{newUrl}\"";
                        var result = StartProcess(cmd);
                        var urlState = CheckStatus(newUrl);
                        return new EditorCommon.VersionControl.VersionControlCommandResult(result, urlState);
                    }
            }
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Revert(string url, bool inherit)
        {
            url = url.Replace("\\", "/");

            var state = CheckStatus(url);
            switch (state)
            {
                case EditorCommon.VersionControl.EStatus.NotControl:
                    return new EditorCommon.VersionControl.VersionControlCommandResult(EditorCommon.VersionControl.EProcessResult.HasError, EditorCommon.VersionControl.EStatus.NotControl);

                default:
                    {
                        string argR = inherit ? "-R" : "";
                        var cmd = $"revert {argR} \"{url}\"";
                        var result = StartProcess(cmd);
                        var urlState = CheckStatus(url);
                        return new EditorCommon.VersionControl.VersionControlCommandResult(result, urlState);
                    }
            }
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Cleanup(string url)
        {
            url = url.Replace("\\", "/");

            var cmd = $"cleanup \"{url}\"";
            var result = StartProcess(cmd);
            var state = CheckStatus(url);
            return new EditorCommon.VersionControl.VersionControlCommandResult(result, state);
        }

        public EditorCommon.VersionControl.VersionControlCommandResult Checkout(string remoteUrl, string localUrl)
        {
            remoteUrl = remoteUrl.Replace("\\", "/");
            localUrl = localUrl.Replace("\\", "/");

            var cmd = $"checkout \"{remoteUrl}\" \"{localUrl}\"";
            var result = StartProcess(cmd);
            var state = CheckStatus(localUrl);
            return new EditorCommon.VersionControl.VersionControlCommandResult(result, state);
        }

        #endregion

    }
}