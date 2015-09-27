using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhatWhen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        

        //get Root folder
        public static string root =  Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        public static string path = root + @"\WhatData";
        static List<Catagory> catList = new List<Catagory>();
        Catagory useCatMethods = new Catagory();

        public MainPage()
        {
            this.InitializeComponent();
            checkFilesExist();
            useCatMethods.getCatagories(catList);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddPage));
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditPage));
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            
        }

        async void checkFilesExist()
        {
            //check if Data Folder exists("WhatData"), else create
            StorageFolder localFolder =
            await StorageFolder.GetFolderFromPathAsync(root);
            if (await localFolder.TryGetItemAsync("WhatData") == null)
            {
                await localFolder.CreateFolderAsync("WhatData");
            }

            //check if there is a catagory file,else create
            StorageFolder storeFolder = await StorageFolder.GetFolderFromPathAsync(path);
            if (await storeFolder.TryGetItemAsync("catagory.txt") == null)
            {
                StorageFile catagoryFile = await storeFolder.CreateFileAsync("catagory.txt");
                Stream fileStream = await catagoryFile.OpenStreamForWriteAsync();

                //catagory file add in a All
                Catagory all = new Catagory() { catName = "ALL", isDeleted = false };
                
            }
        }
    }
}

