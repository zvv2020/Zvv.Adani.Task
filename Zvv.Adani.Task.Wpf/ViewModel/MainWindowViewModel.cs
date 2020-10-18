using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Zvv.Adani.Task.Base;
using Zvv.Adani.Task.Wpf.Model;

namespace Zvv.Adani.Task.Wpf.ViewModel
{
    /// <summary>
    /// The view model for the Main Window.
    /// </summary>
    class MainWindowViewModel:INotifyPropertyChanged
    {
        private string _directoryName;
        private ObservableCollection<SumFileInfoGridModel> _currentGridDataSource;
        private long _filesCount;

        private bool _isProcessing;
        private bool _isStopping;

        private readonly object _dataSourceLocker;
        private readonly object _filesCountLocker;

        /// <summary>
        /// The data source for the Data Grid that displays the results.
        /// </summary>
        public ObservableCollection<SumFileInfoGridModel> CurrentGridDataSource
        {
            get => _currentGridDataSource;
            set
            {
                _currentGridDataSource = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// A current directory name.
        /// </summary>
        public string DirectoryName
        {
            get => _directoryName;
            set
            {
                _directoryName = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// A current count of processed files.
        /// </summary>
        public long FilesCount
        {
            get => _filesCount;
            set
            {
                _filesCount = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Displays whether operation is executing this moment.
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Displays whether a stop command has been sent and not finished yet.
        /// </summary>
        public bool IsStopping
        {
            get => _isStopping;
            set
            {
                _isStopping = value;
                OnPropertyChanged();
            }
        }

        private Command _browseCommand;
        private Command _applyCommand;
        private Command _stopCommand;

        private DirectoryProcessor _directoryProcessor;

        /// <summary>
        /// The command sent with the button "Apply".
        /// </summary>
        public Command ApplyCommand => _applyCommand ??= new Command(arg => 
        {
            ProcessCurrentDirectory();
            IsProcessing = false;
        });

        /// <summary>
        /// The command sent with the button "Browse".
        /// </summary>
        public Command BrowseCommand => _browseCommand ??= new Command(arg =>
        {
            if (arg is Window window)
                SelectDirectory(window);
            else
                SelectDirectory();
        });

        /// <summary>
        /// The command sent with the button "Stop".
        /// </summary>
        public Command StopCommand => _stopCommand ??= new Command(arg => CancelProcessing());

        /// <summary>
        /// Creates an object of the class.
        /// </summary>
        public MainWindowViewModel()
        {
            CurrentGridDataSource = new ObservableCollection<SumFileInfoGridModel>();
            _dataSourceLocker = new object();
            _filesCountLocker = new object();
        }

        /// <summary>
        /// Occurs if a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        /// <summary>
        /// Shows a message box with specified parameters.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="title">The message title.</param>
        /// <param name="button">The button set.</param>
        /// <param name="icon">The message box icon.</param>
        /// <param name="owner">The owner of the message box.</param>
        private void ShowMessageBox(string message, string title,MessageBoxButton button, MessageBoxImage icon, Window owner=null)
        {
            if (owner!=null)
                MessageBox.Show(owner, message, title, button, icon);
            else
                MessageBox.Show(message, title, button, icon);
        }

        /// <summary>
        /// Processed the selected directory.
        /// </summary>
        /// <param name="owner">The owner of the operation. It uses for inner message boxes.</param>
        private void ProcessCurrentDirectory(Window owner=null)
        {
            if (!string.IsNullOrWhiteSpace(DirectoryName))
            {
                CurrentGridDataSource.Clear();
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    IsStopping = false;
                    IsProcessing = true;
                    List<SumFileInfo> resultList=null;
                    try
                    {
                        _directoryProcessor = new DirectoryProcessor(DirectoryName);
                        _directoryProcessor.FileProcessed += DirectoryProcessor_FileProcessed;
                        _directoryProcessor.FilesCountReceived += DirectoryProcessor_FilesCountReceived;
                        resultList = _directoryProcessor.GetSums();
                    }
                    catch (Exception e)
                    {
                        ShowMessageBox($"Error while processing the directory: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, owner);
                    }
                    finally
                    {
                        if (_directoryProcessor != null)
                        {
                            _directoryProcessor.FileProcessed -= DirectoryProcessor_FileProcessed;
                            _directoryProcessor.FilesCountReceived -= DirectoryProcessor_FilesCountReceived;
                        }
                    }

                    if (!IsStopping && resultList != null)
                        try
                        {
                            Tools.SaveAsXmlReport(DirectoryName, resultList);
                        }
                        catch(Exception e)
                        {
                            ShowMessageBox($"Error while saving results to an XML file: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, owner);
                        }

                    IsProcessing = false;
                    IsStopping = false;
                });
            }
            else
                ShowMessageBox($"Please type a directory name.", "A directory name is needed", MessageBoxButton.OK, MessageBoxImage.Information, owner);
        }

        private void DirectoryProcessor_FilesCountReceived(DirectoryProcessor sender, long result)
        {
            lock (_filesCountLocker)
                Application.Current.Dispatcher.Invoke(() => FilesCount = result);
        }

        private void DirectoryProcessor_FileProcessed(DirectoryProcessor sender, DirectoryProcessor.FileProcessedEventArgs e)
        {
            if (e.Succeed)
                lock (_dataSourceLocker)
                    Application.Current.Dispatcher.Invoke(() => CurrentGridDataSource.Add(new SumFileInfoGridModel { Data = e.Result, Number = e.FilesProcessed }));
        }

        /// <summary>
        /// Cancels the current operation.
        /// </summary>
        private void CancelProcessing()
        {
            IsStopping = true;
            _directoryProcessor?.Cancel();
        }

        /// <summary>
        /// Shows a dialog window for selecting a directory.
        /// </summary>
        /// <param name="window">The owner of the operation.</param>
        private void SelectDirectory(Window window = null)
        {
            var dialog = new VistaFolderBrowserDialog()
            {
                SelectedPath = DirectoryName
            };
            if ((window != null && dialog.ShowDialog(window) == true) ||
                (window == null && dialog.ShowDialog() == true))
                DirectoryName = dialog.SelectedPath;
        }
    }
}
