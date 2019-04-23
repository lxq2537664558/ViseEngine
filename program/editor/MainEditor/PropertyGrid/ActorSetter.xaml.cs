using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MainEditor.PropertyGrid
{
    /// <summary>
    /// LightSetter.xaml 的交互逻辑
    /// </summary>
    public partial class ActorSetter : UserControl
    {
        public PropertyDescriptor BindProperty
        {
            get { return (PropertyDescriptor)GetValue(BindPropertyProperty); }
            set { SetValue(BindPropertyProperty, value); }
        }
        public static readonly DependencyProperty BindPropertyProperty =
                            DependencyProperty.Register("BindProperty", typeof(PropertyDescriptor), typeof(ActorSetter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBindPropertyChanged)));

        public static void OnBindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ActorSetter control = d as ActorSetter;
            var newPro = e.NewValue as PropertyDescriptor;
            foreach(var att in newPro.Attributes)
            {
                if(att is CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute)
                {
                    var pgAtt = att as CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute;
                    if (pgAtt.Args == null || pgAtt.Args.Length == 0)
                        return;

                    control.mActorGameType = (UInt16)pgAtt.Args[0];
                    break;
                }
            }
        }

        UInt16 mActorGameType = 0;

        public string ActorName
        {
            get { return (string)GetValue(ActorNameProperty); }
            set { SetValue(ActorNameProperty, value); }
        }
        public static readonly DependencyProperty ActorNameProperty =
            DependencyProperty.Register("ActorName", typeof(string), typeof(ActorSetter), new UIPropertyMetadata());

        public Guid ActorId
        {
            get { return (Guid)GetValue(ActorIdProperty); }
            set { SetValue(ActorIdProperty, value); }
        }
        public static readonly DependencyProperty ActorIdProperty =
            DependencyProperty.Register("ActorId", typeof(Guid), typeof(ActorSetter),
            new FrameworkPropertyMetadata(Guid.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnActorIdChanged)));

        public static void OnActorIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ActorSetter control = d as ActorSetter;

            Guid oldValue = (Guid)e.OldValue;
            Guid newValue = (Guid)e.NewValue;

            if (newValue == oldValue)
                return;

            BindingOperations.ClearBinding(control, ActorNameProperty);
            if (newValue == Guid.Empty)
            {
                control.ActorName = "";
            }
            else
            {
                var actor = CCore.Client.MainWorldInstance.FindActor(newValue);
                if (actor != null)
                {
                    BindingOperations.SetBinding(control, ActorNameProperty, new Binding("ActorName") { Source = actor, UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged });
                }
                else
                    control.ActorName = "";
            }
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;

        public ActorSetter()
        {
            InitializeComponent();
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);
        }

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("WorldActor");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                var actor = (CCore.World.Actor)data[0];
                if (actor != null && actor.GameType == mActorGameType)
                    ActorId = actor.Id;
            }
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ActorId == Guid.Empty)
                return;

            var actor = CCore.Client.MainWorldInstance.FindActor(ActorId);
            if(actor == null)
            {
                EditorCommon.MessageBox.Show($"地图中找不到对象{ActorName}!");
                return;
            }
            EditorCommon.GameActorOperation.SelectActors(new List<CCore.World.Actor>() { actor });
            EditorCommon.GameActorOperation.FocusActors(new List<CCore.World.Actor>() { actor });
        }

        private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActorId = Guid.Empty;
        }

        private void Rectangle_AddUVAnim_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void Rectangle_AddUVAnim_DragLeave(object sender, DragEventArgs e)
        {
        }

        private void Rectangle_AddUVAnim_DragOver(object sender, DragEventArgs e)
        {
        }
        private void Rectangle_AddUVAnim_Drop(object sender, DragEventArgs e)
        {
        }
    }
}
