using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using KsWare.CaliburnMicro.DragDrop;
using KsWare.CaliburnMicro.Extensions;
using KsWare.CaliburnMicro.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace KsWare.Renamer
{
	/// <summary>
	/// Interaction logic for MainWindowView.xaml
	/// </summary>
	[Export]
	public partial class MainWindowViewModel : Screen, ICustomDropTarget
	{
		private FileSystemInfo[] _entries;
		private string[] _names;
		private Stack<string> _textboxUndoStack = new Stack<string>();
		private string _text;
		private string _searchText;
		private string _replaceText;
		private string _letterCase = "Start case";

		public MainWindowViewModel()
		{
		}

		public string Text { get => _text; set => Set(ref _text, value); }
		public string SearchText { get => _searchText; set => Set(ref _searchText, value); }
		public string ReplaceText { get => _replaceText; set => Set(ref _replaceText, value); }

		public async Task MenuFileOpen()
		{
			string AskForFolder()
			{
				//TODO maybe use another FolderDialog, but for the first, this one does the job
				using (var dlg = new CommonOpenFileDialog { IsFolderPicker = true, EnsurePathExists = true })
				{
					var result = dlg.ShowDialog();
					if (result != CommonFileDialogResult.Ok) return null;
					return dlg.FileNames.FirstOrDefault();
				}
			}

			string folder = await ApplicationWrapper.Dispatcher.InvokeAsync(AskForFolder);
			if (folder == null) return;

			var directory = new DirectoryInfo(folder);
			_entries = directory.GetFileSystemInfos();
			_names = _entries.Select(e => e.Name).ToArray();
			Text = string.Join("\r\n", _names);
		}

		public void Replace()
		{
			var s = Text;
			_textboxUndoStack.Push(s);
			var pattern = CreateRegexPattern(SearchText);
			var s2 = Regex.Replace(s, pattern, ReplaceText);
			Text = s2;
		}

		public void Undo()
		{
			if(_textboxUndoStack.Count==0) return;
			var s = _textboxUndoStack.Pop();
			Text = s;
		}

		public void Apply()
		{
			var lines = Text.TrimEnd().Split(new[] {"\r\n"}, StringSplitOptions.None);
			if (lines.Length != _names.Length)
			{
				MessageBox.Show("The number of lines does not match with the number of files!\nOperation canceled.");
			}

			for (int i = 0; i < lines.Length; i++)
			{
				var newName = lines[i];
				var entry = _entries[i];
				var newPath = Path.Combine(Path.GetDirectoryName(entry.FullName), newName);

				if (entry is DirectoryInfo directory)
				{
					directory.MoveTo(newPath);
				} else 
				if (entry is FileInfo file)
				{
					file.MoveTo(newPath);
				}
			}
		}

		public string LetterCase { get => _letterCase; set => Set(ref _letterCase, value); } 
		public void FormatLetterCase()
		{
			_textboxUndoStack.Push(Text);
			// https://en.wikipedia.org/wiki/Letter_case
			// A comparison of various case styles (from most to least capitals used)
			var lines = Text.TrimEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
			for (int i = 0; i < lines.Length; i++) { lines[i] = ApplyLetterCase(lines[i]); }

			Text = string.Join("\r\n", lines);
		}

		private string ApplyLetterCase(string line)
		{
			switch (LetterCase)
			{
				case "Start case":
					return Regex.Replace(line, @"\p{L}+", ApplyStartCase);
					default: return line;

			}
		}

		private string ApplyStartCase(Match match)
		{
			return match.Value[0].ToString().ToUpper() + match.Value.Substring(1);
		}

		private string CreateRegexPattern(string text)
		{
			// Escapes a minimal set of characters (\, *, +, ?, |, {, [, (,), ^, $,., #, and white space) by replacing them with their escape codes. 
			var pattern = Regex.Escape(text);
			return pattern;
		}

		void ICustomDropTarget.OnDrop(object sender, DragEventArgs e)
		{
			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files.Length == 1)
			{
				if (Directory.Exists(files[0]))
				{
					var directory = new DirectoryInfo(files[0]);
					_entries = directory.GetFileSystemInfos();
				}
				else
				{
					_entries=new FileSystemInfo[] { new FileInfo(files[0]) };
				}
			}
			else
			{
				var entries=new List<FileSystemInfo>();
				foreach (var file in files)
				{
					if (Directory.Exists(file))
						entries.Add(new DirectoryInfo(file));
					else
						entries.Add(new FileInfo(file));
					
				}
				_entries = entries.ToArray();
			}
			
			_names = _entries.Select(en => en.Name).ToArray();
			Text = string.Join("\r\n", _names);
		}

		void ICustomDropTarget.OnDragEnter(object sender, DragEventArgs e)
		{
			
		}

		void ICustomDropTarget.OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			
		}

		void ICustomDropTarget.OnDragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
		}

		void ICustomDropTarget.OnDragLeave(object sender, DragEventArgs e)
		{
			
		}
	}
}