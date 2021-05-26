using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using XwaExePatcherWindow.Models;

namespace XwaExePatcherWindow
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        public PatcherModel Patcher { get; private set; }

        private void UpdateModel()
        {
            this.DataContext = null;

            this.Patcher = new PatcherModel(this.Patcher.FileName, this.Patcher.ExeFileName);

            this.DataContext = this;
        }

        private string GetOpenFileName(string title, string extension, string filter, string fileName)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = string.Concat("Open ", title);
            dialog.CheckFileExists = true;
            dialog.AddExtension = true;
            dialog.DefaultExt = extension;
            dialog.Filter = filter;
            dialog.FileName = fileName;

            if (dialog.ShowDialog(this) == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = "patcher.xml";
                string exeFileName = "XWingAlliance.exe";

                if (!System.IO.File.Exists(fileName))
                {
                    fileName = this.GetOpenFileName(fileName, ".xml", "Patcher (*.xml)|*.xml", fileName);
                }

                if (System.IO.File.Exists(fileName))
                {
                    if (!System.IO.File.Exists(exeFileName))
                    {
                        exeFileName = this.GetOpenFileName(exeFileName, ".exe", "XWingAlliance.exe (*.exe)|*.exe", exeFileName);
                    }
                }

                if (!System.IO.File.Exists(fileName) ||
                    !System.IO.File.Exists(exeFileName))
                {
                    this.Close();
                }
                else
                {
                    this.Patcher = new PatcherModel(fileName, exeFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.DataContext = this;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var source = (FrameworkElement)sender;
            var patch = (PatchModel)source.Tag;

            try
            {
                this.Patcher.Apply(patch);
                this.UpdateModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            var source = (FrameworkElement)sender;
            var patch = (PatchModel)source.Tag;

            try
            {
                this.Patcher.Restore(patch);
                this.UpdateModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private string Get202SaveFileName(string name)
        {
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            name = string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "202",
                FileName = name
            };

            if (dialog.ShowDialog(this) == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private void Create202Button_Click(object sender, RoutedEventArgs e)
        {
            var source = (FrameworkElement)sender;
            var patch = (PatchModel)source.Tag;

            string filename = Get202SaveFileName(patch.Name);

            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            try
            {
                var zt = new ZtFile();

                foreach (PatchItemModel item in patch.Items)
                {
                    zt.Patches.Add(item.Offset, item.NewValues);
                }

                zt.Save(filename);

                MessageBox.Show(".202 created.", patch.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }
    }
}
