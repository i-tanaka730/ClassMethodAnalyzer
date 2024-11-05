using System.ComponentModel;
using System.Windows.Input;
using ClassMethodAnalyzer.Commands;
using ClassMethodAnalyzer.Model;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ClassMethodAnalyzer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _analysisResult;

        public string AnalysisResult
        {
            get => _analysisResult;
            set
            {
                _analysisResult = value;
                OnPropertyChanged(nameof(AnalysisResult));
            }
        }

        public ICommand SelectFolderCommand { get; }
        public ICommand ClearCommand { get; }

        public MainViewModel()
        {
            SelectFolderCommand = new RelayCommand(SelectFolder);
            ClearCommand = new RelayCommand(Clear);
        }

        private void SelectFolder()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folderPath = dialog.FileName;
                AnalysisResult = CodeAnalyzer.Analyze(folderPath);
            }
        }

        private void Clear()
        {
            AnalysisResult = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}