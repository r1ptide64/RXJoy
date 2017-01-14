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
using System.Collections.ObjectModel;
using System.Threading;

namespace FJoystick
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private USBHelper _usbHelper;
        private ObservableCollection<Axis> _collection;
        private vJoyHelper _vjoyHelper;
        //private Timer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _usbHelper = new USBHelper(this);
            _usbHelper.AxesSetup += new EventHandler(OnAxesSetup);

        }
        private void SetupAxes()
        {
            _collection = new ObservableCollection<Axis>(_usbHelper.Axes);
            dataGrid.ItemsSource = _collection;
            _vjoyHelper = new vJoyHelper(_usbHelper);
        }
        private void OnAxesSetup(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(SetupAxes));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _usbHelper.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _usbHelper.Dispose();
        }
    }
}
