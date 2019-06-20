using ApplicationUsageAnalyser.ViewModel;
using System.Windows;

namespace ApplicationUsageAnalyser.View
{
    /// <summary>
    /// Interaction logic for ApplicationUsageAnalyserView.xaml
    /// </summary>
    public partial class ApplicationUsageAnalyserView : Window
    {
       

        public ApplicationUsageAnalyserView()
        {
            InitializeComponent();
            DataContext = new ApplicationUsageAnalyserVM();
        }
    }
}
