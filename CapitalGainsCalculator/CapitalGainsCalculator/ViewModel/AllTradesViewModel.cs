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
	public class AllTradesViewModel : TradeHistoryViewModel
	{
		private const string IsoStoreFileName = "Trade Data - Current.txt";

		private IsolatedStorageFile _isoStore;
		
		public AllTradesViewModel()
			: base()
		{
			SelectedTrade = null;
			VisibleTrades = this;
			InSelectionMode = true;
		}

		protected override void OnInitialize()
		{
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
							_tradesVM = (ObservableCollection<TradeOrderViewModel>)(new BinaryFormatter().Deserialize(isoStream));
						}
						catch (Exception) { } // Just use a blank trade history if read fails
					}
				}
			}
		}

		private void LoadTestingDefaults()
		{
			if (_tradesVM.Count < 1)
			{
				_tradesVM.Add(
					new TradeOrderViewModel(
						new BuyOrder()
						{
							OrderInstant = new DateTime(2018, 1, 1),
							BaseCurrency = CoinType.USD,
							BaseAmount = new decimal(100),
							BaseFee = new decimal(0),
							TradeCurrency = CoinType.ETH,
							TradeAmount = new decimal(1),
							Location = TradingLocation.GDAX
					}));
				_tradesVM.Add(
					new TradeOrderViewModel(
						new WithdrawOrder()
						{
							OrderInstant = new DateTime(2018, 1, 10),
							TradeCurrency = CoinType.ETH,
							TradeAmount = new decimal(1),
							Location = TradingLocation.GDAX
					}));
				_tradesVM.Add(
					new TradeOrderViewModel(
						new DepositOrder()
						{
							OrderInstant = new DateTime(2018, 1, 10),
							TradeCurrency = CoinType.ETH,
							TradeAmount = new decimal(1),
							Location = TradingLocation.Coinbase
					}));
				_tradesVM.Add(
					new TradeOrderViewModel(
						new SellOrder()
						{
							OrderInstant = new DateTime(2018, 1, 1),
							BaseCurrency = CoinType.USD,
							BaseAmount = new decimal(150),
							BaseFee = new decimal(0),
							TradeCurrency = CoinType.ETH,
							TradeAmount = new decimal(1),
							Location = TradingLocation.Coinbase
					}));
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
				RaisePropertyChangedEvent("InCreationMode");
			}
		}

		private TradeHistoryViewModel _visibleTrades;
		public TradeHistoryViewModel VisibleTrades
		{
			get { return _visibleTrades; }
			private set
			{
				_visibleTrades = value;
				RaisePropertyChangedEvent("VisibleTrades");
			}
		}

		private int _selectedIndex;
		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set
			{
				_selectedIndex = value;
				SelectedTrade = new TradeOrderViewModel(_tradesVM[_selectedIndex]);
				RaisePropertyChangedEvent("SelectedIndex");
			}
		}

		private TradeOrderViewModel _selectedTrade;
		public TradeOrderViewModel SelectedTrade
		{
			get { return _selectedTrade; }
			set
			{
				_selectedTrade = value;
				RaisePropertyChangedEvent("SelectedTrade");
			}
		}
		#endregion

		#region Commands
		public DelegateCommand SortTradesCommand;
		public DelegateCommand FilterTradesCommand;

		private DelegateCommand _createTradeCommand = null;
		public DelegateCommand CreateTradeCommand
		{
			get { return InitializeCommand(_createTradeCommand, param => this.ExecuteCreateTrade(param), null); }
		}

		private DelegateCommand _promptFileCommand = null;
		public DelegateCommand PromptFileCommand
		{
			get { return InitializeCommand(_promptFileCommand, param => this.ExecutePromptFile(), null); }
		}

		private DelegateCommand _importFileCommand = null;
		public DelegateCommand ImportTradesCommand
		{
			get { return InitializeCommand(_importFileCommand, param => this.ExecuteImportTrades(param), null); }
		}

		private DelegateCommand _deleteTradeCommand = null;
		public DelegateCommand DeleteTradeCommand
		{
			get { return InitializeCommand(_deleteTradeCommand, param => this.ExecuteDeleteTrade(), param => this.CanExecuteDeleteTrade()); }
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

		private DelegateCommand InitializeCommand(DelegateCommand command, Action<object> execute, Predicate<object> canExecute)
		{
			if (command == null)
			{
				command = new DelegateCommand(execute, canExecute);
			}
			return command;
		}
		#endregion

		#region Command Handlers
		private void ExecuteCreateTrade(object param)
		{
			TradeType typeParse;
			if (Enum.TryParse<TradeType>(param as string, out typeParse))
			{
				InSelectionMode = false;
				SelectedTrade = new TradeOrderViewModel(typeParse);
			}
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

		private void ExecuteImportTrades(object param)
		{
			string fileName = param as string;
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				ImportTradeData(ProcessTradeImportFile(fileName));
			}
		}

		private void ExecuteSaveToStorage()
		{
			using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStoreFileName, FileMode.Create, _isoStore))
			{
				try
				{
					new BinaryFormatter().Serialize(isoStream, _tradesVM);
				}
				catch (Exception) { } // Fail silently
			}
		}

		private bool CanExecuteDeleteTrade()
		{
			return InSelectionMode && SelectedTrade != null;
		}

		private void ExecuteDeleteTrade()
		{
			_tradesVM.Remove(SelectedTrade);
			SelectedTrade = null;
		}

		private void ExecuteAcceptChanges()
		{
			if (InSelectionMode)
			{
				_tradesVM[SelectedIndex] = SelectedTrade;
			}
			else
			{
				_tradesVM.Add(SelectedTrade);
			}
			CleanupChanges();
		}

		private void ExecuteCancelChanges()
		{
			CleanupChanges();
		}

		private void CleanupChanges()
		{
			SelectedTrade = null;
			InSelectionMode = true;
		}
		#endregion

		#region Import Helper Methods
		private List<string> ProcessTradeImportFile(string fileName)
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

		private void ImportTradeData(List<string> importData)
		{
			foreach (string importLine in importData)
			{
				_tradesVM.Add(new TradeOrderViewModel(importLine));
			}
		}
		#endregion
	}
}
