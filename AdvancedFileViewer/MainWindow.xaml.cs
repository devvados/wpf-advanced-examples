using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic.FileIO;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace AdvancedFileViewer
{
    public class FollowedFile
    {
        public object FileOrDirectory { get; set; }
        public object Path { get; set; }
    }
    public partial class MainWindow : Window
    {
        public static string SelectedMethod;
        private DirectoryInfo _dirInfo;
        private string _fullPath = "";
        private readonly object _movePath = "";
        public List<FollowedFile> Following = new List<FollowedFile>(); 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var fldEx = new FolderBrowserDialog();

            var dialRes = fldEx.ShowDialog();
            if (dialRes == System.Windows.Forms.DialogResult.OK)
            {
                var dirPath = fldEx.SelectedPath;
                _fullPath = dirPath;
                _dirInfo = new DirectoryInfo(dirPath);
                TreeViewBuild();
            }
        }

        #region Работа с деревом
        private void TreeViewBuild()
        {
            TreeDirView.Items.Clear();
            foreach (var item in _dirInfo.GetDirectories().Select(dir => new TreeViewItem
            {
                Tag = dir,
                Header = dir.ToString()
            }))
            {
                item.Items.Add("*");
                item.Expanded += Tree_Expanded;
                TreeDirView.Items.Add(item);
            }

            foreach (var newItemFiles in _dirInfo.GetFiles())
            {
                var newItem = new TreeViewItem
                {
                    Tag = newItemFiles,
                    Header = newItemFiles.ToString(),
                };
                TreeDirView.Items.Add(newItem);
            }
        }

        private void TreeDirView_OnLoaded(object sender, RoutedEventArgs e)
        {
            TreeDirView.ContextMenu = TreeViewContextMenu();
        }
        private void Tree_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem) e.OriginalSource;
            item.Items.Clear();
            DirectoryInfo dir;
            var tag = item.Tag as DriveInfo;

            if (tag != null)
            {
                var drive = tag;
                dir = drive.RootDirectory;
            }
            else
            {
                var info = item.Tag as DirectoryInfo;
                if (info != null)
                {
                    dir = info;
                    _fullPath = dir.FullName;
                }
                else
                {
                    dir = null;
                }
            }
            if (dir != null)
            {
                try
                {
                    var files = ((DirectoryInfo) item.Tag).GetFiles();
                    foreach (var subDir in dir.GetDirectories())
                    {
                        var newItem = new TreeViewItem
                        {
                            Tag = subDir,
                            Header = subDir.ToString()
                        };
                        newItem.Items.Add("*");
                        item.Items.Add(newItem);
                    }
                    foreach (var newItemFiles in files)
                    {
                        var newItem = new TreeViewItem
                        {
                            Tag = newItemFiles,
                            Header = newItemFiles.ToString(),
                        };
                        item.Items.Add(newItem);
                    }
                }
                catch (Exception ex)
                {
                    LabelStatus.Content = ex.Message;
                }
            }
        }  

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private ContextMenu TreeViewContextMenu()
        {
            var cMenu = new ContextMenu();
            var mItem = new MenuItem() {Header = "Открыть"};
            var mItem0 = new MenuItem() {Header = "Создать"};
            var mItem1 = new MenuItem() {Header = "Наблюдать"};
            var mItem2 = new MenuItem() {Header = "Копировать"};
            var mItem3 = new MenuItem() {Header = "Переместить"};
            var mItem4 = new MenuItem() {Header = "Удалить"};
            var mItem5 = new MenuItem() { Header = "Переименовать" };
            var mItem6 = new MenuItem() {Header = "Шифровать"};
            var mItem7 = new MenuItem() { Header = "Посмотреть атрибуты" };
            var mItem8 = new MenuItem() {Header = "Только чтение" };
            mItem.Click += OpenClick;
            mItem0.Click += CreateClick;
            mItem1.Click += FollowClick;
            mItem2.Click += CopyClick;
            mItem3.Click += MoveClick;
            mItem4.Click += DeleteClick;
            mItem5.Click += RenameClick;
            mItem6.Click += CodeClick;
            mItem7.Click += CheckAttr;
            mItem8.Click += ModSet;
            cMenu.Items.Add(mItem);
            cMenu.Items.Add(mItem0);
            cMenu.Items.Add(mItem1);
            cMenu.Items.Add(mItem2);
            cMenu.Items.Add(mItem3);
            cMenu.Items.Add(mItem4);
            cMenu.Items.Add(mItem5);
            cMenu.Items.Add(mItem6);
            cMenu.Items.Add(mItem7);
            cMenu.Items.Add(mItem8);

            return cMenu;
        }
        #endregion 

        #region Открыть, создать, наблюдать +
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            var temp = (TreeDirView.SelectedItem as TreeViewItem);
            if (temp != null)
            {
                var info = temp.Tag as FileInfo;
                if (info != null)
                {
                    var curFile = info;
                    System.Diagnostics.Process.Start(curFile.FullName);
                }
                var dir = temp.Tag as DirectoryInfo;
                if (dir != null)
                {
                    var curDir = dir;
                    System.Diagnostics.Process.Start(curDir.FullName);
                }
            }
        }
        private void CreateClick(object sender, RoutedEventArgs e)
        {
            var temp = (TreeDirView.SelectedItem as TreeViewItem);
            if (temp != null)
            {
                var tempTag = temp.Tag as DirectoryInfo;
                if (tempTag != null)
                {
                    var currentDir = tempTag;
                    var dlg = new CreateFolderOrFile();
                    dlg.ShowDialog();

                    var resultObject = CreateFolderOrFile.FolderOrFile;
                    var fileName = CreateFolderOrFile.FileName;
                    if (resultObject != null && fileName != null)
                    {
                        if (resultObject == "DirectoryInfo")
                        {
                            var newDirPath = Path.Combine(currentDir.FullName, fileName);
                            if (!Directory.Exists(newDirPath))
                            {
                                Directory.CreateDirectory(newDirPath);
                            }
                            //TreeViewBuild();
                            TreeDirView.Items.Refresh();
                        }
                        else if (resultObject == "FileInfo")
                        {
                            var newFilePath = Path.Combine(currentDir.FullName, fileName + ".txt");
                            if (!File.Exists(newFilePath))
                            {
                                File.Create(newFilePath);
                            }
                            //TreeViewBuild();
                            TreeDirView.Items.Refresh();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите папку!");
                }
            }
        }
        private void FollowClick(object sender, RoutedEventArgs e)
        {
            var temp = (TreeDirView.SelectedItem as TreeViewItem);
   
            if (temp != null)
            {
                var objType = temp.Tag;
                if (objType is FileInfo)
                {
                    var objFile = objType as FileInfo;
                    //для записи в лог
                    var watch = new FileSystemWatcher
                    {
                        Path = objFile.DirectoryName,
                        NotifyFilter =
                            NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess |
                            NotifyFilters.CreationTime,
                    };
                    watch.Changed += FileChanged;
                    watch.Created += FileChanged;
                    watch.Deleted += FileChanged;
                    watch.Renamed += FileRenamed;
                    watch.EnableRaisingEvents = true;
                    var folFile = new FollowedFile() { FileOrDirectory = objFile, Path = objFile.FullName };
                    Following.Add(folFile);
                    ListViewFollow.Items.Add(folFile);
                }

                if (objType is DirectoryInfo)
                {
                    var objDir = objType as DirectoryInfo;
                    //для записи в лог
                    var watch = new FileSystemWatcher
                    {
                        Path = objDir.FullName,
                        NotifyFilter =
                            NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess |
                            NotifyFilters.CreationTime,
                        IncludeSubdirectories = true,
                    };
                    watch.Changed += FileChanged;
                    watch.Created += FileChanged;
                    watch.Deleted += FileChanged;
                    watch.Renamed += FileRenamed;
                    watch.EnableRaisingEvents = true;

                    var folFile = new FollowedFile() { FileOrDirectory = objDir, Path = objDir.FullName };
                    Following.Add(folFile);
                    ListViewFollow.Items.Add(folFile);
                }
            }
        }

        private void FileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            TbLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                TbLog.Text += "Объект: " + fileSystemEventArgs.Name + " " + fileSystemEventArgs.FullPath + " " + fileSystemEventArgs.ChangeType + "\n";
            }));  
        }
        private void FileRenamed(object sender, RenamedEventArgs fileSystemEventArgs)
        {
            TbLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                TbLog.Text += "Объект: " + fileSystemEventArgs.Name + " переименован. Из " + fileSystemEventArgs.OldFullPath + " в " + fileSystemEventArgs.FullPath + "\n";
            }));
        }
        #endregion 

        #region Копировать объект +
        private void Copy(string source, string dest)
        {
            var oldDirectory = source;
            var newDirectory = dest;
            var treeItem = (TreeDirView.SelectedItem as TreeViewItem);
            if (treeItem != null)
            {
                try
                {
                    var dirOrFile = treeItem.Tag;
                    var fdName = dirOrFile.ToString();
                    if (dirOrFile is FileInfo)
                    {
                        //копируем файл
                        File.Copy(Path.Combine(oldDirectory, fdName), Path.Combine(newDirectory, fdName), true);
                    }
                    else if (dirOrFile is DirectoryInfo)
                    {
                        //копируем папку
                        var destinationDir = Path.Combine(newDirectory, dirOrFile.ToString());
                        if (!Directory.Exists(destinationDir))
                        {
                            Directory.CreateDirectory(destinationDir);
                        }
                        FileSystem.CopyDirectory(Path.Combine(oldDirectory, _movePath.ToString()), destinationDir,
                            UIOption.AllDialogs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void CopyClick(object sender, RoutedEventArgs e)
        {
            var dialog = new MoveOrCopyDestination();
            dialog.ShowDialog();
            var oldDirectory = new DirectoryInfo(_fullPath).ToString();
            if (MoveOrCopyDestination.Dir != null)
            {
                var newDirectory = MoveOrCopyDestination.Dir.ToString();
                Copy(oldDirectory, newDirectory);
                TreeDirView.Items.Refresh();
                //TreeViewBuild();
            }
            else
            {
                LabelStatus.Content = "Копирование не осуществлено, так как не указан путь копирования...";
            }
        }
        #endregion
        
        #region Переместить объект +
        private void SyncListView(object foldDir, string newDir, string name)
        {
            //синхронизируем ListView с наблюдаемыми объектами
            var dirOrFile = foldDir;
            var newDirectory = newDir;
            var fdName = name;
            foreach (var fol in Following)
            {
                if (fol.FileOrDirectory == dirOrFile)
                {
                    if (fol.Path.ToString() != Path.Combine(newDirectory,fdName))
                        fol.Path = Path.Combine(newDirectory, fdName);
                }
            }
        }
        private void RefreshListView()
        {
            ListViewFollow.Items.Clear();
            foreach (var fol in Following)
            {
                ListViewFollow.Items.Add(fol);
            }
        }
        private void MoveClick(object sender, RoutedEventArgs e)
        {
            //получение ответа от диалога задания пути
            var dialog = new MoveOrCopyDestination();
            dialog.ShowDialog();
            if (MoveOrCopyDestination.Dir != null)
            {
                var newDirectory = MoveOrCopyDestination.Dir.ToString();

                var treeItem = (TreeDirView.SelectedItem as TreeViewItem);
                if (treeItem != null)
                {
                    var dirOrFile = treeItem.Tag;
                    var fdName = dirOrFile.ToString();
                    if (dirOrFile is FileInfo)
                    {
                        var curFile = dirOrFile as FileInfo;
                        if (!File.Exists(Path.Combine(newDirectory, fdName)))
                        {
                            //перемещаем файл
                            File.Move(curFile.FullName, Path.Combine(newDirectory, fdName));
                            SyncListView(dirOrFile, newDirectory, fdName);
                        }
                    }
                    else if (dirOrFile is DirectoryInfo)
                    {
                        //перемещаем папку
                        var curDir = dirOrFile as DirectoryInfo;
                        var destinationDir = Path.Combine(newDirectory, dirOrFile.ToString());
                        if (!Directory.Exists(destinationDir))
                        {
                            Directory.CreateDirectory(destinationDir);
                            FileSystem.MoveDirectory(curDir.FullName, destinationDir, UIOption.AllDialogs);
                            SyncListView(dirOrFile, newDirectory, fdName);
                        }
                    }
                }
                //обновляем TreeView и ListView
                TreeDirView.Items.Remove(TreeDirView.SelectedItem);
                TreeDirView.Items.Refresh();
                //TreeViewBuild();
                RefreshListView();
            }
            else
            {
                LabelStatus.Content = "Перемещение не осуществлено, так как не указан путь перемещения объекта...";
            }
        }
        #endregion
        
        #region Удалить, Переименовать, Закодировать, Посмотреть/задать аттрибуты
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var treeItem = (TreeDirView.SelectedItem as TreeViewItem);
            if (treeItem != null)
            {
                var objectType = treeItem.Tag;
                if (objectType is FileInfo)
                {
                    var curPath = objectType as FileInfo;
                    File.Delete(curPath.FullName);
                    for (var i = 0; i < Following.Count; i++)
                    {
                        if(Following[i].FileOrDirectory.ToString() == curPath.Name && Following[i].Path.ToString() == curPath.FullName)
                            Following.RemoveAt(i);
                    }
                }
                if (objectType is DirectoryInfo)
                {
                    var curDir = objectType as DirectoryInfo;
                    Directory.Delete(curDir.FullName, true);
                    for (var i = 0; i < Following.Count; i++)
                    {
                        if (Following[i].FileOrDirectory.ToString() == curDir.FullName && Following[i].Path.ToString() == curDir.FullName)
                            Following.RemoveAt(i);
                    }
                }
                TreeDirView.Items.Remove(treeItem);
                TreeDirView.Items.Refresh();
                RefreshListView();
            }
        }
        private void RenameClick(object sender, RoutedEventArgs e)
        {
            var treeItem = (TreeDirView.SelectedItem as TreeViewItem);
            if (treeItem != null)
            {
                var dlg = new RenameFileOfFolder();
                dlg.ShowDialog();
                var newName = RenameFileOfFolder.NewFileName;
                try
                {
                    if (newName != null)
                    {
                        var objectType = treeItem.Tag;
                        if (objectType != null)
                        {
                            if (objectType is FileInfo)
                            {
                                var curPath = objectType as FileInfo;
                                if (curPath.Directory != null)
                                {
                                    var newPath = Path.Combine(curPath.Directory.ToString(), newName);
                                    File.Move(curPath.FullName, newPath + Path.GetExtension(curPath.FullName));
                                    for (var i = 0; i < Following.Count; i++)
                                    {
                                        if (Following[i].FileOrDirectory.ToString() == curPath.Name &&
                                            Following[i].Path.ToString() == curPath.FullName)
                                        {
                                            var renamedFol = new FollowedFile
                                            {
                                                FileOrDirectory = new FileInfo(newName + ".txt"),
                                                Path = newPath + ".txt"
                                            };
                                            Following[i] = renamedFol;
                                        }
                                    }
                                }
                            }
                            if (objectType is DirectoryInfo)
                            {
                                var curDir = objectType as DirectoryInfo;
                                string renamedDir = null;
                                if (curDir.Parent != null)
                                {
                                    renamedDir = curDir.Parent.FullName;
                                    Directory.Move(curDir.FullName, Path.Combine(renamedDir, newName));
                                }
                                for (var i = 0; i < Following.Count; i++)
                                {
                                    if (Following[i].FileOrDirectory.ToString() == curDir.Name && Following[i].Path.ToString() == curDir.FullName)
                                    {
                                        if (renamedDir != null)
                                        {
                                            var renamedFol = new FollowedFile
                                            {
                                                FileOrDirectory = new DirectoryInfo(newName),
                                                Path = Path.Combine(renamedDir, newName)
                                            };
                                            Following[i] = renamedFol;
                                        }
                                    }
                                }
                            }
                        }
                        RefreshListView();
                        TreeDirView.Items.Refresh();
                    }
                    else
                    {
                        LabelStatus.Content = "Переименование не осуществлено, так как не было задано новое имя...";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void CodeClick(object sender, RoutedEventArgs e)  
        {
            var wnd = new Code();
            wnd.ShowDialog();
            
            if(SelectedMethod == "TripleDes") { /*EncryptTextToFile();*/ }
            if (SelectedMethod == "AES") { /*EncryptStringToBytes_Aes;*/ }
            if (SelectedMethod == "Rijndael") { /*EncryptStringToBytes();*/ }
        }
        private void CheckAttr(object sender, RoutedEventArgs e)
        {
            TbAttr.Clear();
            var treeItem = (TreeDirView.SelectedItem as TreeViewItem);
            if (treeItem != null)
            {
                var objectType = treeItem.Tag;
                if (objectType is FileInfo)
                {
                    var curPath = objectType as FileInfo;
                    TbAttr.Text+= curPath.Attributes.ToString();
                }
                if (objectType is DirectoryInfo)
                {
                    var curDir = objectType as DirectoryInfo;
                    TbAttr.Text += curDir.Attributes.ToString();
                }
                TreeDirView.Items.Remove(treeItem);
                TreeDirView.Items.Refresh();
                RefreshListView();
            }
        }
        private void ModSet(object sender, RoutedEventArgs e)
        {
            var treeItem = (TreeDirView.SelectedItem as TreeViewItem);
            var fileSystemInfo = treeItem?.Tag as FileSystemInfo;
            if (fileSystemInfo != null)
            {
                var selectedObjectPath = fileSystemInfo.FullName;
                var dlg = new AttributeReadOnly();
                dlg.ShowDialog();

                var setOrOff = AttributeReadOnly.SetReadOnly;
                try
                {
                    if (!string.IsNullOrEmpty(setOrOff))
                    {
                        if (setOrOff == "Set")
                        {
                            File.SetAttributes(selectedObjectPath, fileAttributes: FileAttributes.ReadOnly);
                        }
                        else if (setOrOff == "Off")
                        {
                            var isReadOnly = ((File.GetAttributes(selectedObjectPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);

                            if (isReadOnly)
                                fileSystemInfo.Attributes &= ~FileAttributes.ReadOnly;
                            else
                            {
                                throw new Exception("Атрибут <Только для чтения> не задан для данного файла");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion
    }
}