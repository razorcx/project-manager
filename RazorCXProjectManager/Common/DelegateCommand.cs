using System;
using System.Windows.Input;

namespace RazorCXProjectManager.Common
{
	internal class DelegateCommand : ICommand
	{
		private readonly Action _action;

		public DelegateCommand(Action action)
		{
			_action = action;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_action();
		}
	}
}