using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CapitalGainsCalculator.ViewModel
{
	public class DelegateCommand : ICommand
	{
		private readonly Action<object> _execute;
		private readonly Predicate<object> _canExecute;

		public DelegateCommand(Action<object> execute)
			: this(execute, null) { }

		public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public event EventHandler CommandExecuted;

		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
			RaiseCommandExecuted();
		}

		protected void RaiseCommandExecuted()
		{
			CommandExecuted?.Invoke(this, EventArgs.Empty);
		}

		public static bool TryExecuteCommand(ICommand command, object param)
		{
			if (command.CanExecute(param))
			{
				command.Execute(param);
				return true;
			}
			else { return false; }
		}
	}
}
