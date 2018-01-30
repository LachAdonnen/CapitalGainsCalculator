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
using System.Windows.Shapes;
using CapitalGainsCalculator.ViewModel;

namespace CapitalGainsCalculator.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class AllOrdersView : Window
	{
		public AllOrdersView()
		{
			InitializeComponent();
		}

		private void OrderDataColumnHeader_Click(object sender, RoutedEventArgs e)
		{ }

		protected override void OnClosing(CancelEventArgs e)
		{
			DelegateCommand.TryExecuteCommand((DataContext as AllOrdersViewModel)?.SaveToStorageCommand, null);
		}
	}
}
