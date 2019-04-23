using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MaterialEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for VectorValueSetter.xaml
    /// </summary>
    public partial class VectorValueSetter : UserControl
    {
        CCore.Material.MaterialShaderVarInfo mMatShaderVarInfo;
        MaterialShaderValueControl mParentControl;

        List<TextBox> m_ValueShowElementList = new List<TextBox>();

        public int nIdx = 0;
        public bool bInitFinish = false;
        protected int mTabIndex = 0;

        public VectorValueSetter(CCore.Material.MaterialShaderVarInfo info, MaterialShaderValueControl parentControl)
        {
            InitializeComponent();

            mMatShaderVarInfo = info;
            mParentControl = parentControl;
            mTabIndex = 0;

            InitValueSetPanel(info.VarType);
            SetValue(info.VarValue);

            Binding bind = new Binding();
            bind.Source = info;
            bind.Path = new System.Windows.PropertyPath("NickName");
            Label_ValueName.SetBinding(Label.ContentProperty, bind);

            bInitFinish = true;
        }

        public void SetValue(string value)
        {
            var splits = value.Split(',');

            if (m_ValueShowElementList.Count != splits.Length)
            {
                EditorCommon.MessageBox.Show("参数" + mMatShaderVarInfo.VarName + "类型与值数量不匹配，请检查材质文件!");
                return;
            }

            for (int i = 0; i < m_ValueShowElementList.Count; ++i )
            {
                m_ValueShowElementList[i].Text = splits[i];
            }
        }

        protected void InitValueSetPanel(string valueType)
        {
            switch (valueType)
            {
                case "int":
                    AddValueEditor("int");
                    break;
                case "float":
                case "float1":
                    AddValueEditor("float");
                    break;
                case "float2":
                    AddValueEditor("x");
                    AddValueEditor("y");
                    break;
                case "float3":
                    AddValueEditor("x");
                    AddValueEditor("y");
                    AddValueEditor("z");
                    break;
                case "float4":
                    AddValueEditor("x");
                    AddValueEditor("y");
                    AddValueEditor("z");
                    AddValueEditor("w");
                    break;
            }
        }

        protected void AddValueEditor(string strName)
        {
            Grid grid = new Grid()
            {
                Margin = new Thickness(1)
            };
            ColumnDefinition colDef = new ColumnDefinition();
            colDef.Width = new System.Windows.GridLength(25);
            grid.ColumnDefinitions.Add(colDef);
            colDef = new ColumnDefinition();
            colDef.Width = new System.Windows.GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(colDef);

            Label lName = new Label()
            {
                Content = strName,
                Foreground = Brushes.White
            };
            Grid.SetColumn(lName, 0);
            grid.Children.Add(lName);

            TextBox tBox = new TextBox()
            {
                Text = "0",
                TabIndex = mTabIndex++,
                Margin = new Thickness(0, 0, 2, 2)
            };
            //tBox.TextChanged += new TextChangedEventHandler(ValueTextChanged);
            //tBox.TextInput += new TextCompositionEventHandler(ValueTextInput);
            tBox.LostFocus += new RoutedEventHandler(ValueTextChanged);
            tBox.KeyDown += new KeyEventHandler(ValueTextKeyDown);
            tBox.Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TextBoxStyle_Default")) as System.Windows.Style;
            //tBox.SetResourceReference(TextBox.StyleProperty, "TextBoxStyle");
            Grid.SetColumn(tBox, 1);
            grid.Children.Add(tBox);
            m_ValueShowElementList.Add(tBox);
            StackPanel_Values.Children.Add(grid);
        }

        protected void ValueTextKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Enter:
                    {
                        ValueTextChanged(sender, null);
                    }
                    break;
            }
        }

        //protected void ValueTextInput(object sender, TextCompositionEventArgs e)
        protected void ValueTextChanged(object sender, RoutedEventArgs e)
        //protected void ValueTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!bInitFinish)
                return;

            string strValue = "";
            foreach (var tBox in m_ValueShowElementList)
            {
                strValue += tBox.Text + ",";
            }

            strValue = strValue.Remove(strValue.Length - 1);        // 去掉最后一个","

            if (mMatShaderVarInfo.VarValue != strValue)
            {
                mMatShaderVarInfo.VarValue = strValue;

                // 重新刷新list（临时代码，日后寻找更合适的值回传方案）
                var list = new List<CCore.Material.MaterialShaderVarInfo>();
                list.AddRange(mParentControl.ShaderValueList);
                list[nIdx] = mMatShaderVarInfo;
                mParentControl.ShaderValueList = list;
            }
        }
    }
}
