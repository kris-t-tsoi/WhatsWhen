using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
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
        Catagory currentlyViewing;

        
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
            //goes in here everytime page opens up
            if (firstTime ==true)
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
            if (catListView.SelectedItems.Count==0) {
                messageBox("No Category was Selected", "Please select the category you wish to add a To Do in");
            } else
            {
                this.Frame.Navigate(typeof(AddPage), catList.ElementAt(catListView.SelectedIndex));
            }

            
        }


        private void editAct_Click(object sender, RoutedEventArgs e)
        {
            //change catListView
            if (catListView.SelectedItems.Count == 0)
            {
                messageBox("To Do Not Selected", "Please Select a To Do Activity to Edit");
            }
            else
            {
                this.Frame.Navigate(typeof(EditPage));
            }
        }

        private void deleteAct_Click(object sender, RoutedEventArgs e)
        {
            //change catListView
            if (catListView.SelectedItems.Count == 0)
            {
                messageBox("To Do Not Selected", "Please Select What You Want to Delete");
            }
            else
            {
                // message box ask if really want to delete


            }
            }

        public void addtoList (String adding)
        {
            Catagory add = new Catagory() { catName = adding, isDeleted = false };
            foreach(Catagory i in catList)
            {
                if (i.catName.Equals(add.catName))
                {
                    return;
                }
            }
            catList.Add(add);
            
            
        }

        private async void messageBox(String title, String message)
        {
            MessageDialog dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        private void catListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int selIndex = catListView.SelectedIndex;
            Catagory selectCat = catList.ElementAt(selIndex);
            refreshActivityView(selectCat);

        }


        void refreshActivityView(Catagory name)
        {
            //clear list view not working
            doneListView.Items.Clear();
            overListView.Items.Clear();
            doListView.Items.Clear();

            foreach (Activity act in name.activityItems) {

                String print = act.actDue.ToString("dd MMMM yyyy") + "\t\t\t\t" + act.actName;

                if (act.actFinished == true ){

                    doneListView.Items.Add(print);

                }else if (act.actDue<DateTime.Now)
                {
                    overListView.Items.Add(print);
                }
                else
                {
                    doListView.Items.Add(print);
                }
                
            }
            //catListView.Items.OrderBy(StringComparison);
        }

        private void Page_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
         

        }

        private async void catListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int selIndex = catListView.SelectedIndex;
            Catagory selectCat = catList.ElementAt(selIndex);
            currentlyViewing = selectCat;
           bool done = await selectCat.getActivitiesInCatagory();
            while (!done) { }
            refreshActivityView(selectCat);
        }

        async void delAct(ListView lv)
        {
            int delIndex = lv.SelectedIndex;
            Activity deleteAct = currentlyViewing.activityItems.ElementAt(0); //TODO Fix
            deleteAct.isDeleted = true;
            bool finishDelete = await currentlyViewing.updateCatagoryText(catList);
            if (finishDelete == true)
            {
                currentlyViewing.activityItems.Remove(deleteAct);
                refreshCategoryBar();
            }
        }

    }
}
