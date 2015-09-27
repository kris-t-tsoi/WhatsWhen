using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatWhen;
using Windows.Storage;

namespace WhatWhen
{
    class Catagory
    {
        internal string catName;
        internal bool isDeleted;

        internal Catagory(bool isNew)
        {

            if (isNew == true)
            {
                //wait until catagory name is set
                while (catName == null) { }
                //if catagory is not ALL then create individual file
                if (!catName.Equals("ALL"))
                {
                    createFile(catName);
                }

                //add to catagory file
                addCatagoryTextFile();

            }

        }

        async void addCatagoryTextFile()
        {
            //get catagory.txt path
            string catFile = MainPage.path + @"\catagory.txt";
            StorageFile catagoryFile = await StorageFile.GetFileFromPathAsync(catFile+".txt");

            //start a file stream
            Stream fileStream = await catagoryFile.OpenStreamForWriteAsync();

            //catagory file add in a All           
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.WriteLine(catName + "\t" + isDeleted);
            }
        }
        async void createFile(string newFileName)
        {
            
            StorageFolder storeFolder = await StorageFolder.GetFolderFromPathAsync(MainPage.path);

            //create own file
            if (await storeFolder.TryGetItemAsync("catagory.txt") == null)
            {
                StorageFile catagoryFile = await storeFolder.CreateFileAsync(newFileName);

            }

        }



    }
}
