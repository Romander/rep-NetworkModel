using System.Linq;
using System.Windows;

namespace Microsoft_Automatic_Graph_Layout
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public InputTable item = new InputTable("", "", -1);

        public AddWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (OperationBox.Text != "" || BeforeOperationBox.Text != "" || TimeBox.Text != "")
            {
                item = new InputTable(OperationBox.Text, BeforeOperationBox.Text, int.Parse(TimeBox.Text));
                Close();
            }
            else
            {
                MessageBox.Show("Error!");
                Close();
            }
        }
    }
}
