using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PopulateTreeView();
        }

        private void PopulateTreeView()
        {
            TreeViewItem root = new TreeViewItem
            {
                Header = "This PC"
            };
            foreach (var drive in DriveInfo.GetDrives())
            {
                TreeViewItem driveItem = new TreeViewItem { Header = drive.Name };
                root.Items.Add(driveItem);

                FillTreeView(drive.Name, driveItem);
            }
            tvFilesAndFolders.Items.Add(root);
        }

        private void FillTreeView(string directoryPath, TreeViewItem parentItem)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(directoryPath))
                {
                    TreeViewItem directoryItem = new TreeViewItem { Header = Path.GetFileName(directory), Tag = directory };
                    directoryItem.Items.Add(null); // Add a dummy item for lazy loading
                    directoryItem.Expanded += DirectoryItem_Expanded;
                    parentItem.Items.Add(directoryItem);
                }
                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    TreeViewItem fileItem = new TreeViewItem { Header = Path.GetFileName(file), Tag = file };
                    parentItem.Items.Add(fileItem);
                }
            }
            catch (UnauthorizedAccessException)
            {
                parentItem.Items.Add(new TreeViewItem { Header = "Access Denied" });
            }
            catch (Exception ex)
            {
                parentItem.Items.Add(new TreeViewItem { Header = $"Error: {ex.Message}" });
            }
        }

        private void DirectoryItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == null)
            {
                item.Items.Clear();

                FillTreeView(item.Tag.ToString(), item);
            }
        }

        private void tvFilesAndFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = tvFilesAndFolders.SelectedItem as TreeViewItem;
            if (item != null)
            {
                lblPath.Content = "Selected Path: " + item.Tag?.ToString();
            }
        }

        private void tvFilesAndFolders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tvFilesAndFolders.SelectedItem is TreeViewItem selectedItem && File.Exists(selectedItem.Tag.ToString()))
            {
                Process.Start(new ProcessStartInfo(selectedItem.Tag.ToString()) { UseShellExecute = true });
            }
        }
    }
}
