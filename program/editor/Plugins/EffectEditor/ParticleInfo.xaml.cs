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

namespace EffectEditor
{
    /// <summary>
    /// ParticleInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ParticleInfo : UserControl
    {
        public ParticleInfo()
        {
            InitializeComponent();
        }

        public void UpdateEffectInfo(CCore.Effect.EffectTemplate effectTemplate)
        {
            TextBlock_Particles.Text = effectTemplate.ParticlesCount.ToString();
        }
    }
}
