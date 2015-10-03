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
        //get paths of folders
        public static string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        public static string path = root + @"\WhatData";

        //list of categories
        internal static List<Catagory> catList = new List<Catagory>();
        Catagory useCatMethods = new Catagory();
        Catagory currentlyViewing;

        //List of activites in each section on main page
        List<Activity> toDoIndex;
        List<Activity> overIndex;
        List<Activity> doneIndex;

        //get instance of page
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

            //check WhatData folder and catagory.txt file exist
            //Then update view on page
            checkFilesExist();

           
        }


        //check WhatData folder exist
        async void checkFilesExist()
        {
            StorageFolder localFolder = await StorageFolder.GetFolderFromPathAsync(root);
            //check if Data Folder exists("WhatData"), else create
            if (await localFolder.TryGetItemAsync("WhatData") == null)
            {
                await localFolder.CreateFolderAsync("WhatData");
            }

            StorageFolder storeFolder = await StorageFolder.GetFolderFromPathAsync(root + @"\WhatData");

            //check catagory.txt file
            bool done = await createCatagoryFile();
            while (!done) { }

            //refresh page view
            intialCatagories();
        }

        //check catagory.txt file
        async Task<bool> createCatagoryFile()
        {
            StorageFolder storeFolder = await StorageFolder.GetFolderFromPathAsync(root + @"\WhatData");

            //check if there is a catagory file,else create
            if (await storeFolder.TryGetItemAsync("catagory.txt") == null)
            {
                StorageFile catagoryFile = await storeFolder.CreateFileAsync("catagory.txt");
            }
            return true;

        }

         //get categories from file
        async void intialCatagories()
        {
            catListView.Items.Clear();
            bool done = false;
            done = await useCatMethods.getCatagories(_instance);

            while (!done) { }

            //refesh category bar
            refreshCategoryBar();
        }

        //refesh category bar
        void refreshCategoryBar()
        {
            //clear current items in listview, then add in categories
            catListView.Items.Clear();
            foreach (Catagory name in catList)
            {
                catListView.Items.Add(name.catName);
            }
        }

       //User want to create a new category, goes to newCategory page
        private void addCat_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewCatagory), catList);
        }

        //deletes currently selected category
        private void deleteCat_Click(object sender, RoutedEventArgs e)
        {
            if (catListView.SelectedItems.Count == 0)
            {
                messageBox("Category Not Selected", "Please Select A Category to Delete");
            }
            else
            {
                delCat();
            }
        }

        //delete category and its unique text file before refreshing category bar
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
        

        //add created category into list
        public void addtoList(String adding)
        {
            Catagory add = new Catagory() { catName = adding, isDeleted = false };
            foreach (Catagory i in catList)
            {
                if (i.catName.Equals(add.catName))
                {
                    return;
                }
            }
            catList.Add(add);
        }

        //create popup message box
        private async void messageBox(String title, String message)
        {
            MessageDialog dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        //refresh actitivties
        private void catListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int selIndex = catListView.SelectedIndex;
            Catagory selectCat = catList.ElementAt(selIndex);
            refreshActivityView(selectCat);
        }

        //refresh activities of currently selected category
        void refreshActivityView(Catagory name)
        {
            toDoIndex = new List<Activity>();
            overIndex = new List<Activity>();
            doneIndex = new List<Activity>();

            doneListView.Items.Clear();
            doListView.Items.Clear();
            overListView.Items.Clear();

            foreach (Activity act in name.activityItems)
            {

                String print = act.actDue.ToString("dd / MM / yyyy") + "\t\t\t" + act.actName;

                if (act.actFinished == true)
                {

                    doneListView.FontSize = 22;
                    doneListView.Items.Add(print);
                    doneIndex.Add(act);

                }
                else if (act.actDue < DateTime.Now)
                {
                    overListView.Items.Add(print);
                    overIndex.Add(act);
                }
                else
                {
                    doListView.Items.Add(print);
                    toDoIndex.Add(act);
                }
            }
        }


        //add activity to currently selected category
        private void addAct_Click(object sender, RoutedEventArgs e)
        {
            //currentCat = currently selected catagory
            if (catListView.SelectedItems.Count == 0)
            {
                messageBox("No Category was Selected", "Please select the category you wish to add a To Do in");
            }
            else
            {
                this.Frame.Navigate(typeof(AddPage), catList.ElementAt(catListView.SelectedIndex));
            }
        }

        //edit currently selected activity
        private void editAct_Click(object sender, RoutedEventArgs e)
        {
            //change catListView
            if (doListView.SelectedItems.Count == 0 && doneListView.SelectedItems.Count == 0 && overListView.SelectedItems.Count == 0)
            {
                messageBox("To Do Not Selected", "Please Select a To Do Activity to Edit");
            }
            else if ((doListView.SelectedItems.Count + doneListView.SelectedItems.Count + overListView.SelectedItems.Count) > 1)
            {
                messageBox("Too Many Items Selected", "Please Only Select One Item You Wish to Edit");
            }
            else
            {   //check which  list the activiy is in
                PassCatAndAct param = new PassCatAndAct();
                param.cat = currentlyViewing;

                if (doListView.SelectedItems.Count == 1)
                {
                    param.act = toDoIndex.ElementAt(doListView.SelectedIndex);
                    this.Frame.Navigate(typeof(EditPage), param);
                }
                else if (doneListView.SelectedItems.Count == 1)
                {
                    param.act = doneIndex.ElementAt(doneListView.SelectedIndex);
                    this.Frame.Navigate(typeof(EditPage), param);
                }
                else
                {
                    param.act = overIndex.ElementAt(overListView.SelectedIndex);
                    this.Frame.Navigate(typeof(EditPage), param);
                }
            }
        }

        //delete currently selected activity
        private void deleteAct_Click(object sender, RoutedEventArgs e)
        {
            //change catListView
            if (doListView.SelectedItems.Count == 0 && doneListView.SelectedItems.Count == 0 && overListView.SelectedItems.Count == 0)
            {
                messageBox("To Do Not Selected", "Please Select Which To Do Activity You Wish to Delete");
            }
            else if ((doListView.SelectedItems.Count + doneListView.SelectedItems.Count + overListView.SelectedItems.Count) > 1)
            {
                messageBox("Too Many Items Selected", "Please Only Select One Item You Wish to Delete");
            }
            else
            {   //check which list selected activity is in
                if (doListView.SelectedItems.Count == 1)
                {
                    delAct(doListView, toDoIndex);
                }
                else if (doneListView.SelectedItems.Count == 1)
                {
                    delAct(doneListView, doneIndex);
                }
                else
                {
                    delAct(overListView, overIndex);
                }
            }
        }

        //vs wont let me delete without giving a error
        private void Page_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
        }

        //gets activities for the selected category
        private async void catListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int selIndex = catListView.SelectedIndex;
            Catagory selectCat = catList.ElementAt(selIndex);
            currentlyViewing = selectCat;
            bool done = await selectCat.getActivitiesInCatagory();
            while (!done) { }
            refreshActivityView(selectCat);
        }

        //delete selected activity from file and list
        async void delAct(ListView lv, List<Activity> index)
        {
            int delIndex = lv.SelectedIndex;
            Activity deleteAct = index.ElementAt(delIndex);
            currentlyViewing.activityItems.Remove(deleteAct);
            bool finishDelete = await useCatMethods.updateIndividaulCatText(currentlyViewing, deleteAct.actName);
            if (finishDelete == true)
            {
                refreshActivityView(currentlyViewing);
            }
        }

        //refesh and deselects selected activities
        private void doListView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            refreshActivityView(currentlyViewing);
        }

        //refesh and deselects selected activities
        private void overListView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            refreshActivityView(currentlyViewing);
        }

        //refesh and deselects selected activities
        private void doneListView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            refreshActivityView(currentlyViewing);
        }
    }
}
