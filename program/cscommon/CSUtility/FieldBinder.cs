using System;
using System.Collections.Generic;


namespace CSUtility
{
    public class DBBindField : Attribute
    {
        public string Field;
        public DBBindField(string fld)
        {
            Field = fld;
        }
    }

    public class DBBindTable : Attribute
    {
        public string Table;
        public DBBindTable(string tab)
        {
            Table = tab;
        }
    }
}

namespace CSUtility.Helper
{
    /// <summary>
    /// 服务器客户端类型
    /// </summary>
    public enum enCSType
    {
        /// <summary>
        /// 服务器客户端共用
        /// </summary>
        Common,
        /// <summary>
        /// 服务器
        /// </summary>
        Server,
        /// <summary>
        /// 客户端
        /// </summary>
        Client,
        /// <summary>
        /// 所有
        /// </summary>
        All,
    }
}

namespace CSUtility.Editor
{
    public sealed class Editor_ValueWithRange : Attribute
    {
        public double maxValue = 100;
        public double minValue = 0;
        public Editor_ValueWithRange(double min, double max)
        {
            minValue = min;
            maxValue = max;
        }
    }

    public sealed class Editor_PropertyGridDataTemplateAttribute : Attribute
    {
        string mDataTemplateType = "";
        public string DataTemplateType
        {
            get { return mDataTemplateType; }
        }

        public object[] Args { get; set; }

        public Editor_PropertyGridDataTemplateAttribute(string dataTemplateType,object[] args = null)
        {
            mDataTemplateType = dataTemplateType;
            Args = args;
        }
    }
    
    public sealed class Editor_MultipleOfTwoAttribute : Attribute { }

    public sealed class Editor_ColorPicker : Attribute { }

    public sealed class Editor_HexAttribute : Attribute { }

    public sealed class Editor_VectorEditor : Attribute { }

    public sealed class Editor_ActorLayerSetter : Attribute { }

    public sealed class Editor_Angle360Setter : Attribute { }
    public sealed class Editor_Angle180Setter : Attribute { }
    
    public sealed class Editor_ScalarVariableSetter : Attribute { }
    public sealed class Editor_HotKeySetter : Attribute { }

    #region UIEditor

    public sealed class UIEditor_ControlAttribute : Attribute
    {
        System.String mName;
        public System.String Name
        {
            get { return mName; }
        }

        public UIEditor_ControlAttribute(System.String name)
        {
            mName = name;
        }
    }

    // 可以模板化的控件
    public sealed class UIEditor_ControlTemplateAbleAttribute : Attribute
    {
        System.String mName;
        public System.String Name
        {
            get { return mName; }
        }

        public UIEditor_ControlTemplateAbleAttribute(System.String name)
        {
            mName = name;
        }
    }
    

    // Binding
    public sealed class UIEditor_BindingEventAttribute : Attribute { }
    public sealed class UIEditor_BindingMethodAttribute : Attribute { }
    public sealed class UIEditor_BindingPropertyAttribute : Attribute
    {
        Type[] mAvailableTypes;
        public Type[] AvailableTypes
        {
            get { return mAvailableTypes; }
        }

        public UIEditor_BindingPropertyAttribute(Type[] availableTypes = null)
        {
            mAvailableTypes = availableTypes;
        }
    }

    public sealed class UIEditor_WhenWinBaseParentIsTypeShow : Attribute
    {
        Type[] mParentTypes;
        public Type[] ParentTypes
        {
            get { return mParentTypes; }
        }

        public UIEditor_WhenWinBaseParentIsTypeShow(Type[] parentTypes)
        {
            mParentTypes = parentTypes;
        }
    }

    public sealed class UIEditor_PropertysWithAutoSet : Attribute { }

    public sealed class UIEditor_CommandEventAttribute : Attribute { }
    public sealed class UIEditor_CommandMethodAttribute : Attribute { }
    
    public sealed class UIEditor_Scale9InfoEditor : Attribute { }

    public sealed class UIEditor_FontParamCollectionAttribute : Attribute { }

    public sealed class UIEditor_OpenFileEditorAttribute : Attribute
    {
        public List<string> ExtNames = new List<string>();
        public UIEditor_OpenFileEditorAttribute(string ext)
        {
            var splits = ext.Split(',');
            foreach (var split in splits)
            {
                ExtNames.Add(split);
            }
        }
    }
    public sealed class UIEditor_DefaultFontPathAttribute : Attribute { }

    public sealed class UIEditor_DocumentEditorAttribute : Attribute { }

    public sealed class UIEditor_DocumentTextEditorAttribute : Attribute { }

    #endregion

    #region AIEditor

    public sealed class AIEditor_AIUseAblePropertyAttribute : Attribute { }
    public sealed class AIEditor_AIUseAbleMethodAttribute : Attribute { }

    #endregion

    #region DelegateMethodEditor

    public sealed class DelegateMethodEditor_AllowedDelegate : Attribute
    {
        // delegate类型
        public string TypeStr;

        public DelegateMethodEditor_AllowedDelegate(string typeStr)
        {
            TypeStr = typeStr;
        }
    }

    #endregion

    public sealed class CDataEditorAttribute : Attribute
    {
        public string m_strFileExt = "";

        public CDataEditorAttribute(string strFileExt)
        {
            m_strFileExt = strFileExt;
        }
    }

}
