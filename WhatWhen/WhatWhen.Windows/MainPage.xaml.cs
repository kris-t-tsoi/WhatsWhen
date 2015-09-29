using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public partial class MainPage : Page
    {
        public NavigationCacheMode NavigationCacheMode { get; set; }

        //get Root folder
        public static string root =  Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        public static string path = root + @"\WhatData";
        internal static List<Catagory> catList = new List<Catagory>();
        Catagory useCatMethods = new Catagory();
        Boolean firstTime = true;
        String currentCat;

        
        static MainPage _instance;
        public static MainPage Current
        {
            get
            {
                return _instance;
            }
        }



        public MainPage()
        { 
            _instance = this;
            this.InitializeComponent();
            checkFilesExist();
            if (firstTime)
            {
                intialCatagories();
                firstTime = false;
            }
            refreshCategoryBar();

        }


        async void intialCatagories()
        {
            catListView.Items.Clear();
            bool done = false;
            done = await useCatMethods.getCatagories(_instance);

            //while not
            while (!done) { }
            refreshCategoryBar();
        }
    
        void refreshCategoryBar()
        {
            //clear list view not working
            catListView.Items.Clear();
            foreach (Catagory name in catList)
            {
                catListView.Items.Add(name.catName);
            }
            //catListView.Items.OrderBy(StringComparison);
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

        private void addCat_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewCatagory));
        }

        private void deleteCat_Click(object sender, RoutedEventArgs e)
        {
            delCat();
        }

        async void delCat()
        {
            int delIndex = catListView.SelectedIndex;
            Catagory deleteCat = catList.ElementAt(delIndex);
            deleteCat.isDeleted = true;
            bool finishDelete = await deleteCat.updateCatagoryText(catList);
            if (finishDelete == true)
            {
                catList.Remove(deleteCat);
                refreshCategoryBar();
            }            
        }

        private void addAct_Click(object sender, RoutedEventArgs e)
        {
            //currentCat = currently selected catagory
            this.Frame.Navigate(typeof(AddPage),currentCat);
        }


        private void editAct_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditPage));
        }

        private void deleteAct_Click(object sender, RoutedEventArgs e)
        {

        }

        public void addtoList (String adding)
        {
            Catagory add = new Catagory() { catName = adding, isDeleted = false };
            catList.Add(add);
        }

    }
}
