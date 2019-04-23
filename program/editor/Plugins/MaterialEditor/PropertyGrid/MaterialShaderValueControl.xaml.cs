using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MaterialEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for MaterialShaderValueControl.xaml
    /// </summary>
    public partial class MaterialShaderValueControl : UserControl
    {
        public List<CCore.Material.MaterialShaderVarInfo> ShaderValueList
        {
            get { return (List<CCore.Material.MaterialShaderVarInfo>)GetValue(ShaderValueListProperty); }
            set
            {
                SetValue(ShaderValueListProperty, value);
            }
        }

        public static readonly DependencyProperty ShaderValueListProperty =
            DependencyProperty.Register("ShaderValueList", typeof(List<CCore.Material.MaterialShaderVarInfo>), typeof(MaterialShaderValueControl), new PropertyMetadata(new PropertyChangedCallback(ShaderValueListChangedCallback)));

        static void ShaderValueListChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MaterialShaderValueControl msvCtrl = sender as MaterialShaderValueControl;
            msvCtrl.StackPanel_ShaderVar.Children.Clear();
            if (e.NewValue != null)
            {
                int i = 0;
                foreach (var item in ((List<CCore.Material.MaterialShaderVarInfo>)(e.NewValue)))
                {
                    switch(item.EditorType)
                    {
                        case "Texture":
                            TextureValueSetter tValSet = new TextureValueSetter(item, msvCtrl);
                            tValSet.nIdx = i++;
                            tValSet.Margin = new Thickness(2);
                            msvCtrl.StackPanel_ShaderVar.Children.Add(tValSet);
                            break;
                        case "Vector":
                            VectorValueSetter vvSet = new VectorValueSetter(item, msvCtrl);
                            vvSet.nIdx = i++;
                            vvSet.Margin = new Thickness(2);
                            msvCtrl.StackPanel_ShaderVar.Children.Add(vvSet);
                            break;
                        case "Color":
                            SystemColorPicker scpSet = new SystemColorPicker(item, msvCtrl);
                            scpSet.nIdx = i++;
                            scpSet.Margin = new Thickness(2);
                            msvCtrl.StackPanel_ShaderVar.Children.Add(scpSet);
                            break;
                    }
                }
            }
        }

        public MaterialShaderValueControl()
        {
            InitializeComponent();
        }
    }
}
