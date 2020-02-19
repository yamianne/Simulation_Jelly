using Microsoft.Win32;
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

namespace JellySimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point lastFramePos = new Point(0, 0);
        Manager manager => DataContext as Manager;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pos = e.GetPosition(this);
                manager.MoveFrame(pos - lastFramePos);
                lastFramePos = pos;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                lastFramePos = e.GetPosition(this);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            manager.StartSimultaion();
        }

        private void LoadButton_Click (object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "obj files (*.obj)|*.obj|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                TextBox_CustomMeshPath.Text = dialog.SafeFileName;
                manager.GraphicsContainer.ReadObj(dialog.FileName);
            }
        }

    }
}
