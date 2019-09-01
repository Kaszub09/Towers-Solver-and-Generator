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

namespace Pyramids
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PagePyramidSolver pagePyramidSolver;
        PagePyramidGenerator pagePyramidGenerator;
        ResourceDictionary ResDict1;
        public MainWindow()
        {
            InitializeComponent();
            pagePyramidSolver = new PagePyramidSolver(this);
            pagePyramidGenerator = new PagePyramidGenerator(this);         
            Content = pagePyramidSolver;
            ResDict1 = new ResourceDictionary
            {
                Source = new Uri("/DS3Style.xaml", UriKind.RelativeOrAbsolute)
            };
        }

        public void Change()
        {
            if(Content == pagePyramidSolver)
            {
                Content = pagePyramidGenerator;
            }
            else
            {
                Content = pagePyramidSolver;
            }
        }

        public void ChangeStyle()
        {

            if (Application.Current.Resources.MergedDictionaries.Any()){
                Application.Current.Resources.MergedDictionaries.Clear();
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(ResDict1);
            }

        }
    }
}
