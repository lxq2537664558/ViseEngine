
/* 项目“Client.Android”的未合并的更改
在此之前:
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore
在此之后:
namespace CCore
*/
using System;

namespace CCore
{
#if WIN
    /// <summary>
    /// 编辑器基类
    /// </summary>
    public interface EditorClassBase
    {
        /// <summary>
        /// 是否是编辑器模式
        /// </summary>
        bool IsEditorMode { get; set; }
        /// <summary>
        /// 退出游戏时调用此方法，清除编辑器中的数据
        /// </summary>
        void FinalInstance();
        /// <summary>
        /// 初始化编辑器数据
        /// </summary>
        /// <param name="env">渲染环境</param>
        /// <param name="gameForm">游戏窗口</param>
        void Initialize(CCore.Graphics.REnviroment env, System.Windows.Forms.Form gameForm);
        /// <summary>
        /// 游戏窗口尺寸或位置改变时调用此方法
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="top">上</param>
        /// <param name="height">高</param>
        /// <param name="width">宽</param>
        void OnGameWindowChanged(int left, int top, int height, int width);
        /// <summary>
        /// 每帧执行
        /// </summary>
        void Tick();

        /// <summary>
        /// 对象拖入游戏窗口时调用此方法
        /// </summary>
        /// <param name="sender">控件信息</param>
        /// <param name="e">点击事件的参数</param>
        void OnGameWindowDragEnter(object sender, System.Windows.Forms.DragEventArgs e);
        /// <summary>
        /// 对象拖出游戏窗口时调用此方法
        /// </summary>
        /// <param name="sender">控件信息</param>
        /// <param name="e">点击事件的参数</param>
        void OnGameWindowDragLeave(object sender, EventArgs e);
        /// <summary>
        /// 对象放置入游戏窗口时调用此方法
        /// </summary>
        /// <param name="sender">控件信息</param>
        /// <param name="e">点击事件的参数</param>
        void OnGameWindowDragDrop(object sender, System.Windows.Forms.DragEventArgs e);
        /// <summary>
        /// 对象拖过游戏窗口时调用此方法
        /// </summary>
        /// <param name="sender">控件信息</param>
        /// <param name="e">点击事件的参数</param>
        void OnGameWindowDragOver(object sender, System.Windows.Forms.DragEventArgs e);
    }
#endif
}
