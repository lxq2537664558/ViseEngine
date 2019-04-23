using CodeGenerateSystem.Base;
using System.Windows;
using System.Windows.Controls;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// Interaction logic for ShaderAutoData.xaml
    /// </summary>
    public partial class ShaderAutoData : BaseNodeControl_ShaderVar
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

        public ShaderAutoData(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);

            var splits = strParam.Split(',');

            mValueType = splits[0];
            Title = splits[1];
            Description = splits[2] + "(" + mValueType + ")";
            OutLink.BackBrush = Program.GetBrushFromValueType(mValueType, this);
            AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(mValueType), OutLink, enBezierType.Right, enLinkOpType.Start, OutLink.BackBrush, true);
        }

        public override string GetValueDefine()
        {
            return mValueType + " " + GCode_GetValueName(null) + " : " + Title + ";\r\n";
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            return Title + "_" + Program.GetValuedGUIDString(this.Id);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return mValueType;
        }
    }
}
