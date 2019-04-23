using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// TextureControl.xaml 的交互逻辑
    /// </summary>
    [CodeGenerateSystem.ShowInMaterialEditorMenu("参数.贴图")]
    public partial class TextureControl : BaseNodeControl
    {
        string m_strTexturePath;

        public TextureControl(Canvas paraentCanvas, string strParam)
            : base(paraentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);
        }

        public override void Save(MidLayer.Support.IXmlNode xmlNode, bool newGuid)
        {
            base.Save(xmlNode, newGuid);
        }

        public override void Load(MidLayer.Support.IXmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);
        }

        private void MenuItem_LoadTexture_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                m_strTexturePath = ofd.FileName;
                image_Texture.Source = new BitmapImage(new Uri(m_strTexturePath));
            }
        }
    }
}
