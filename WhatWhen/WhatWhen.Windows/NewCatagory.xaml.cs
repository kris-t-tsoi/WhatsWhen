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
        public NewCatagory()
        {
            this.InitializeComponent();
        }

        private void newCatName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            //go back to previous page
            this.Frame.GoBack();

        }

        private void okbutton_Click(object sender, RoutedEventArgs e)
        {
            Catagory newCat = new Catagory() { catName = newCatName.Text, isDeleted = false };
            //get textbox data, check name is unique
            if (checkUnique(newCat) == true)
            {
                MainPage.catList.Add(newCat);
                newCat.catagoryCreate(MainPage.catList);

                //return to previous page
                //this.Frame.Navigate;
            }
            else
            {
                messageBox("", "");
            }


        }

        Boolean checkUnique(Catagory check)
        {

            if (MainPage.catList.Contains(check))
            {
                return false;
            }
            //return true if name is unique
            return true;
        }

        private async void messageBox (String title, String message)
        {
            MessageDialog dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }


    }

    

}