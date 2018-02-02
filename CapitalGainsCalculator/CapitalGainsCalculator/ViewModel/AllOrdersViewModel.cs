using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CapitalGainsCalculator.Model;

namespace CapitalGainsCalculator.ViewModel
{
	public class AllOrdersViewModel : OrderHistoryViewModel
	{
		private const string IsoStoreFileName = "Order Data - Current.txt";

		private IsolatedStorageFile _isoStore;
		
		public AllOrdersViewModel()
			: base()
		{
			SelectedOrder = null;
			VisibleOrders = this;
			InSelectionMode = true;

			_isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly |
				IsolatedStorageScope.Domain, null, null);
			LoadFromStorage();
		}

		private void LoadFromStorage()
		{
			if (_isoStore.FileExists(IsoStoreFileName))
			{
				using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStoreFileName, FileMode.Open, _isoStore))
				{
					if (isoStream.Length > 0)
					{
						try
						{
							isoStream.Position = 0;
							_ordersVM = (ObservableCollection<OrderViewModel>)(new BinaryFormatter().Deserialize(isoStream));
						}
						catch (Exception) { } // Just use a blank trade history if read fails
					}
				}
			}
		}

		#region Public Properties
		private bool _inSelectionMode;
		public bool InSelectionMode
		{
			get { return _inSelectionMode; }
			set
			{
				_inSelectionMode = value;
				RaisePropertyChangedEvent(nameof(InSelectionMode));
			}
		}

		private OrderHistoryViewModel _visibleOrders;
		public OrderHistoryViewModel VisibleOrders
		{
			get { return _visibleOrders; }
			private set
			{
				_visibleOrders = value;
				RaisePropertyChangedEvent(nameof(VisibleOrders));
			}
		}

		private int _selectedIndex;
		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set
			{
				_selectedIndex = value;
				SelectedOrder = new OrderViewModel(_ordersVM[_selectedIndex]);
				RaisePropertyChangedEvent(nameof(SelectedIndex));
			}
		}

		private OrderViewModel _selectedOrder;
		public OrderViewModel SelectedOrder
		{
			get { return _selectedOrder; }
			set
			{
				_selectedOrder = value;
				RaisePropertyChangedEvent(nameof(SelectedOrder));
			}
		}
		#endregion

		#region Commands
		public DelegateCommand SortOrdersCommand;
		public DelegateCommand FilterOrdersCommand;

		private DelegateCommand _createOrderCommand = null;
		public DelegateCommand CreateOrderCommand
		{
			get { return InitializeCommand(_createOrderCommand, param => this.ExecuteCreateOrder(), null); }
		}

		private DelegateCommand _promptFileCommand = null;
		public DelegateCommand PromptFileCommand
		{
			get { return InitializeCommand(_promptFileCommand, param => this.ExecutePromptFile(), null); }
		}

		private DelegateCommand _importFileCommand = null;
		public DelegateCommand ImportTradesCommand
		{
			get { return InitializeCommand(_importFileCommand, param => this.ExecuteImportFile(param), null); }
		}

		private DelegateCommand _deleteOrderCommand = null;
		public DelegateCommand DeleteOrderCommand
		{
			get { return InitializeCommand(_deleteOrderCommand, param => this.ExecuteDeleteOrder(), param => this.CanExecuteDeleteOrder()); }
		}

		private DelegateCommand _acceptChangesCommand = null;
		public DelegateCommand AcceptChangesCommand
		{
			get { return InitializeCommand(_acceptChangesCommand, param => this.ExecuteAcceptChanges(), null); }
		}

		private DelegateCommand _cancelChangesCommand = null;
		public DelegateCommand CancelChangesCommand
		{
			get { return InitializeCommand(_cancelChangesCommand, param => this.ExecuteCancelChanges(), null); }
		}

		private DelegateCommand _saveToStorageCommand = null;
		public DelegateCommand SaveToStorageCommand
		{
			get { return InitializeCommand(_saveToStorageCommand, param => this.ExecuteSaveToStorage(), null); }
		}

		private DelegateCommand _calculateTaxesCommand = null;
		public DelegateCommand CalculateTaxesCommand
		{
			get { return InitializeCommand(_calculateTaxesCommand, param => this.ExecuteCalculateTaxesCommand(), null); }
		}

		private static DelegateCommand InitializeCommand(DelegateCommand command, Action<object> execute, Predicate<object> canExecute)
		{
			if (command == null)
			{
				command = new DelegateCommand(execute, canExecute);
			}
			return command;
		}
		#endregion

		#region Command Handlers
		private void ExecuteCreateOrder()
		{
			InSelectionMode = false;
			SelectedOrder = new OrderViewModel(new ExchangeOrder());
		}

		private void ExecutePromptFile()
		{
			Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
			fileDialog.DefaultExt = ".csv";
			fileDialog.Filter = "Comma-Separated Values (*.csv)|*.csv";
			Nullable<bool> result = fileDialog.ShowDialog();

			if (result == true)
			{
				DelegateCommand.TryExecuteCommand(ImportTradesCommand, fileDialog.FileName);
			}
		}

		private void ExecuteImportFile(object param)
		{
			Initialize(); // Reset prior to import
			string fileName = param as string;
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				ImportOrderData(ProcessOrderImportFile(fileName));
			}
		}

		private void ExecuteSaveToStorage()
		{
			using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStoreFileName, FileMode.Create, _isoStore))
			{
				try
				{
					new BinaryFormatter().Serialize(isoStream, _ordersVM);
				}
				catch (Exception) { } // Fail silently
			}
		}

		private void ExecuteCalculateTaxesCommand()
		{
			TaxCalculator.GenerateTaxEvents(TaxCalculationType.LIFO, OrdersModel);
			foreach (OrderViewModel order in Orders)
			{
				order.RaiseTaxLinesChanged();
			}
		}

		private bool CanExecuteDeleteOrder()
		{
			return InSelectionMode && SelectedOrder != null;
		}

		private void ExecuteDeleteOrder()
		{
			_ordersVM.RemoveAt(SelectedIndex);
			SelectedOrder = null;
		}

		private void ExecuteAcceptChanges()
		{
			if (InSelectionMode)
			{
				_ordersVM[SelectedIndex] = SelectedOrder;
			}
			else
			{
				_ordersVM.Add(SelectedOrder);
			}
			CleanupChanges();
		}

		private void ExecuteCancelChanges()
		{
			CleanupChanges();
		}

		private void CleanupChanges()
		{
			SelectedOrder = null;
			InSelectionMode = true;
		}
		#endregion

		#region Import Helper Methods
		private List<string> ProcessOrderImportFile(string fileName)
		{
			List<string> importData = new List<string>();
			try
			{
				string[] fileContents = System.IO.File.ReadAllLines(fileName);
				importData.AddRange(fileContents);
				importData.RemoveAt(0); // Header Row
			}
			catch (Exception) { }
			return importData;
		}

		private void ImportOrderData(List<string> importData)
		{
			foreach (string importLine in importData)
			{
				_ordersVM.Add(new OrderViewModel(importLine));
			}
		}
		#endregion
	}
}
