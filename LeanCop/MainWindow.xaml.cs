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

namespace LeanCop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void drawButton_Click(object sender, RoutedEventArgs e)
        {
            String[,] matrix;
            List<Connection> connections;

            try
            {
                matrix = ParameterParser.parseMatrix(matrixTextbox.Text);
                connections = ParameterParser.parseConnections(connectionTextbox.Text);

                var dm = new DrawingManager(canvas);
                dm.drawProof(matrix, connections);

                matrixTextbox.Foreground = Brushes.Black;
                connectionTextbox.Foreground = Brushes.Black;
            } catch (Exception ex) {
                var s = ex.TargetSite;
                matrixTextbox.Foreground = Brushes.Red;
                connectionTextbox.Foreground = Brushes.Red;
            }
        }

    }
}
