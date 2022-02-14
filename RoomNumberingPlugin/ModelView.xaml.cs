using Autodesk.Revit.UI;
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

namespace RoomNumberingPlugin
{
    /// <summary>
    /// Логика взаимодействия для ModelView.xaml
    /// </summary>
    public partial class ModelView : Window
    {
        public ModelView(ExternalCommandData commandData)
        {
            InitializeComponent();

            ViewModel vm = new ViewModel(commandData);

            //vm.CloseRequest += (s, e) => this.Close();


            DataContext = vm;
        }
    }
}
