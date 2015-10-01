using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class AddPage : Page
    {
        Catagory cat;
      
        public AddPage()
        {
            this.InitializeComponent();

              }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            cat = e.Parameter as Catagory;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            DateTime day = new DateTime();
            day = pageDate.Date.Date;

            if (userInput.Text == "") { //if textbox is empty
                messageBox("Empty Name", "Name can not be empty");

            }
            else
            {
                Activity newAct = new Activity() { actName = userInput.Text, actDue = day, actFinished = (bool)completeCheckBox.IsChecked, isDeleted = false };
                cat.activityItems.Add(newAct);
                cat.updateIndividaulCatText(cat,"");
                this.Frame.GoBack();

            }
                        
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void messageBox(String title, String message)
        {
            MessageDialog dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

       

    }
}
