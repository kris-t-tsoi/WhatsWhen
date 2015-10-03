using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class EditPage : Page
    {
        PassCatAndAct pam;

        public EditPage()
        {
            this.InitializeComponent();
        }

        //get parameter from navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            pam = e.Parameter as PassCatAndAct;
            userInput.Text = pam.act.actName;

            //cant set datepicker or checkbox
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }       

        private void addCategory_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewCatagory));
        }

        //update category's unique text file with change
        private async void ok_Click(object sender, RoutedEventArgs e)
        {
            //get date
            DateTime day = new DateTime();
            day = pageDate.Date.Date;

            if (userInput.Text == "")
            { //if textbox is empty
                messageBox("Empty Name", "Name can not be empty");
            }
            else
            {
                Activity newAct = new Activity() { actName = userInput.Text, actDue = day, actFinished = (bool)completeCheckBox.IsChecked, isDeleted = false };
                pam.cat.activityItems.Remove(pam.act);
                pam.cat.activityItems.Add(newAct);
                pam.cat.updateIndividaulCatText(pam.cat, "");
                this.Frame.GoBack();
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        //popup message box
        private async void messageBox(String title, String message)
        {
            MessageDialog dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

    }
}
