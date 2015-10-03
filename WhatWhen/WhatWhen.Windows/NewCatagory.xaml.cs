using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class NewCatagory : Page
    {

        List<Catagory> cat;

        public NewCatagory()
        {
            this.InitializeComponent();


        }

        //get input parameter
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            cat = e.Parameter as List<Catagory>;
        }

        //refresh category bar that shows current categories made
        void refreshCategoryBar()
        {
            
            CatItems.Items.Clear();
            foreach (Catagory name in cat)
            {
                CatItems.Items.Add(name.catName);
            }

        }


        private void newCatName_TextChanged(object sender, TextChangedEventArgs e) { }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            //go back to previous page
            this.Frame.GoBack();

        }

        //check if category is unique and create
        private void okbutton_Click(object sender, RoutedEventArgs e)
        {
            Catagory newCat = new Catagory() { catName = newCatName.Text, isDeleted = false };
            //get textbox data, check name is unique
            if (checkUnique(newCat) == true)
            {

                newCat.catagoryCreate(MainPage.catList);

                //return to previous page
                this.Frame.GoBack();
            }
            else
            {
                messageBox("Please Pick Another Name", "There is already another category with the same name");
            }


        }

        //check is category is unique
        Boolean checkUnique(Catagory check)
        {

            if (MainPage.catList.Contains(check))
            {
                return false;
            }
            //return true if name is unique
            return true;
        }

        //message pop up box
        private async void messageBox(String title, String message)
        {
            MessageDialog dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        //refresh category bar
        private void CatItems_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            refreshCategoryBar();
        }
    }



}