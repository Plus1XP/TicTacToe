using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //readonly LogicController logicController;
        public MainWindow()
        {
            this.InitializeComponent();
            //this.logicController = new LogicController();
            //this.DataContext = this.logicController;
        }
    }
}
