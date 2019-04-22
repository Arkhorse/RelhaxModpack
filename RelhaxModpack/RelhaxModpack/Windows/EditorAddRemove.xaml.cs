﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RelhaxModpack.UIComponents;

namespace RelhaxModpack.Windows
{
    /// <summary>
    /// Interaction logic for EditorAddRemove.xaml
    /// </summary>
    public partial class EditorAddRemove : RelhaxWindow
    {
        public List<DatabasePackage> GlobalDependencies;
        public List<Dependency> Dependencies;
        public List<Category> ParsedCategoryList;
        public DatabasePackage SelectedPackage = null;
        public bool EditOrAdd = true;
        public bool AddSaveLevel = true;
        private const string GlobalDependenciesCategoryHeader = "--Global Dependencies--";
        private const string DependenciesCategoryHeader = "--Dependencies--";

        public EditorAddRemove()
        {
            InitializeComponent();
        }

        private void RelhaxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //load the top box with selection info
            CategoryComboBox.Items.Clear();
            CategoryComboBox.Items.Add(GlobalDependenciesCategoryHeader);
            CategoryComboBox.Items.Add(DependenciesCategoryHeader);
            foreach (Category cat in ParsedCategoryList)
                CategoryComboBox.Items.Add(cat);
            PackageComboBox.Items.Clear();
            //edit or add info
            if(EditOrAdd)
            {
                //edit
                AddSameLevel.Visibility = Visibility.Hidden;
                AddSameLevel.IsEnabled = false;
                AddNewLeve1.Visibility = Visibility.Hidden;
                AddNewLeve1.IsEnabled = false;
                CategoryTextBox.Text = "Select the category to move package to";
                PackageTextBox.Text = "Select the package to move to";
            }
            else
            {
                //add
                AddSameLevel.Visibility = Visibility.Visible;
                AddSameLevel.IsEnabled = true;
                AddNewLeve1.Visibility = Visibility.Visible;
                AddNewLeve1.IsEnabled = true;
                CategoryTextBox.Text = "Select the category to add package to";
                PackageTextBox.Text = "Select the package to add to";
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CategoryComboBox.SelectedItem is string s)
            {
                if(s.Equals(GlobalDependenciesCategoryHeader))
                {
                    foreach (DatabasePackage package in GlobalDependencies)
                        PackageComboBox.Items.Add(new EditorComboBoxItem(package));
                }
                else if (s.Equals(DependenciesCategoryHeader))
                {
                    foreach (DatabasePackage package in Dependencies)
                        PackageComboBox.Items.Add(new EditorComboBoxItem(package));
                }
            }
            else if(CategoryComboBox.SelectedItem is Category cat)
            {
                PackageComboBox.Items.Clear();
                foreach (DatabasePackage package in cat.GetFlatPackageList())
                    PackageComboBox.Items.Add(new EditorComboBoxItem(package));
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PackageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PackageComboBox.SelectedItem is EditorComboBoxItem item)
                SelectedPackage = item.Package;
        }

        private void AddSameLevel_Checked(object sender, RoutedEventArgs e)
        {
            AddSaveLevel = true;
        }

        private void AddNewLeve1_Checked(object sender, RoutedEventArgs e)
        {
            AddSaveLevel = false;
        }
    }
}
