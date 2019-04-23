using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaterialEditor.Controls.Value
{
    /// <summary>
    /// PixelMaterialData.xaml 的交互逻辑
    /// </summary>
    public partial class PixelMaterialData : CodeGenerateSystem.Base.BaseNodeControl
    {
        string mTitle = "";
        public string Title
        {
            get { return mTitle; }
            set
            {
                mTitle = value;
                OnPropertyChanged("Title");
            }
        }

        string mValueType = "";

        string mDescription = "";
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;
                OnPropertyChanged("Describe");
            }
        }

        public PixelMaterialData(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);

            var splits = strParam.Split(',');

            mValueType = splits[0];
            Title = splits[1];
            Description = splits[2] + "(" + mValueType + ")";
            OutLink.BackBrush = Program.GetBrushFromValueType(mValueType, this);
            AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromTypeString(mValueType), OutLink, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, OutLink.BackBrush, true);
        }

        public string GetRequireString()
        {
            return Title.Substring(1);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            return "pssem." + Title;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return mValueType;
        }
    }
}
