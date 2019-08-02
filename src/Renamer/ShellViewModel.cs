using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using KsWare.CaliburnMicro.DragDrop;

namespace KsWare.Renamer {

	[Export(typeof(IShell))]
	public class ShellViewModel : PropertyChangedBase, IShell, ICustomDropTarget, IHaveDisplayName
	{
		[Import(typeof(MainWindowViewModel))]
		private Screen _mainScreen;

		private string _displayName = "KsWare Renamer";

		public Screen MainScreen { get => _mainScreen; set => Set(ref _mainScreen, value);}

		void ICustomDropTarget.OnDrop(object sender, DragEventArgs e)
		{
			if (MainScreen is ICustomDropTarget dropTarget)
			{
				dropTarget.OnDrop(sender, e);
				return;
			}
		}

		void ICustomDropTarget.OnDragEnter(object sender, DragEventArgs e)
		{
			if (MainScreen is ICustomDropTarget dropTarget)
			{
				dropTarget.OnDragEnter(sender, e);
				return;
			}
		}

		void ICustomDropTarget.OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			if (MainScreen is ICustomDropTarget dropTarget)
			{
				dropTarget.OnGiveFeedback(sender, e);
				return;
			}
		}

		void ICustomDropTarget.OnDragOver(object sender, DragEventArgs e)
		{
			if (MainScreen is ICustomDropTarget dropTarget)
			{
				dropTarget.OnDragOver(sender, e);
				return;
			}
		}

		void ICustomDropTarget.OnDragLeave(object sender, DragEventArgs e)
		{
			if (MainScreen is ICustomDropTarget dropTarget)
			{
				dropTarget.OnDragLeave(sender, e);
				return;
			}
		}

		public string DisplayName { get => _displayName; set => Set(ref _displayName, value);}
	}
}