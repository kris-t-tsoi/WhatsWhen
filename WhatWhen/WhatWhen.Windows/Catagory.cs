using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace WhatWhen
{
    class Catagory
    {
        internal string catName;
        internal Boolean isDeleted;
        internal List<Activity> activityItems = new List<Activity>();

              
         public void catagoryCreate (List<Catagory> list)
        {
            
                //wait until catagory name is set
                while (catName == null) { }

                //create individual file
                  createFile(catName);

            //add to catagory list then update catagory file
            list.Add(this);
            updateCatagoryText(list);               

            }          
        

         async void updateCatagoryText(List<Catagory> list)
        {            
            //get catagory.txt path
            StorageFile catagoryFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\catagory.txt");
            
            //start a file stream for writing
            Stream fileStream = await catagoryFile.OpenStreamForWriteAsync();

            //rewrite all and add catagory into catagory.txt          
            using (StreamWriter writer = new StreamWriter(fileStream))
            {   
            foreach(Catagory line in list)
                {   //if the user has deleted the catagory, do not save in file
                    if(line.isDeleted == false)
                    {
                        writer.WriteLine(line.catName);
                    }
                    else
                    {
                        //remove the deleted catagory's file
                        deleteFile(line.catName+".txt");
                    }
                }
                }
        }
         async void createFile(string newFileName)
        {
            
            StorageFolder storeFolder = await StorageFolder.GetFolderFromPathAsync(MainPage.path);

            //create own file
            if (await storeFolder.TryGetItemAsync(newFileName) == null)
            {
                StorageFile catagoryFile = await storeFolder.CreateFileAsync(newFileName+".txt");

            }
            }


        public async Task<bool> getCatagories( MainPage mainP)
        {


            //get catagory.txt path
            var catagoryFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\catagory.txt");
            if (catagoryFile != null)
            {
                Stream fileStream = await catagoryFile.OpenStreamForReadAsync();

                //rewrite all and add catagory into catagory.txt          
                using (StreamReader read = new StreamReader(fileStream))

                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        //read line, and store data in list
                        // Catagory filedata = new Catagory() { catName = line, isDeleted =false };
                        mainP.addtoList(line);
                        //read line
                        read.ReadLine();
                    }

                }
            }

            return true;
            
        }


        Stream gernerateStreamFromString(String address)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter wr = new StreamWriter(stream);
            wr.Write(address);
            wr.Flush();
            stream.Position = 0;
            return stream;
        }

       
        async void deleteFile(String filename)
        {
            StorageFile deleteFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\" + filename);
            await deleteFile.DeleteAsync();
        }

        //get activites per category
        public async void getActivitiesInCatagory()
        {
          
            //get text path
            StorageFile actFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\"+this.catName+".txt");

            //start a file stream for reading
            Stream fileStream = await actFile.OpenStreamForReadAsync();

            //rewrite all and add catagory into catagory.txt          
            using (StreamReader read = new StreamReader(fileStream))
            {
                string line;
                while ((line = read.ReadLine()) != null)
                {
                    //split line, and store data in list
                    String[] info = line.Split('\t');
                    Activity filedata = new Activity() { };
                    activityItems.Add(filedata);
                    //read line
                    read.ReadLine();
                }

            }
        }


        async void updateIndividaulCatText(List<Catagory> list)
        {
            //get catagory.txt path
            StorageFile catFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\"+this.catName+".txt");

            //start a file stream for writing
            Stream fileStream = await catFile.OpenStreamForWriteAsync();

            //rewrite all and add catagory into catagory.txt          
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                foreach (Activity line in this.activityItems)
                {   //if the user has deleted the catagory, do not save in file
                    if (line.isDeleted == false)
                    {
                        writer.WriteLine(line.actName);
                    }
                    else
                    {   //if activity is deleted remove from activity list
                        activityItems.Remove(line);
                    }
                   
                }
            }
        }

    }
}
