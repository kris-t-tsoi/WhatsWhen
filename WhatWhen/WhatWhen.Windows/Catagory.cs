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
        

         internal async Task<bool> updateCatagoryText(List<Catagory> list)
        {            
            //get catagory.txt path
            StorageFile catagoryFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\catagory.txt");
            
            //start a file stream for writing
            Stream fileStream = await catagoryFile.OpenStreamForWriteAsync();

            List<Catagory> toDelete = new List<Catagory>();

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
                        //remove the deleted catagory's file and from list
                        deleteFile(line.catName + ".txt");
                        toDelete.Add(line);
                    }
                }
                }
            foreach(Catagory c in toDelete)
            {
                list.Remove(c);
            }

            return true;
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
                        Catagory filedata = new Catagory() { catName = line, isDeleted =false };
                        mainP.addtoList(line);
                    }
                }
            }
            return true;
        }


        async void deleteFile(String filename)
        {
            StorageFile deleteFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\" + filename);
            await deleteFile.DeleteAsync();
        }




        //get activites per category
        public async Task<bool> getActivitiesInCatagory()
        {
            //get text path
            StorageFile actFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\"+this.catName+".txt");

            //start a file stream for reading
            Stream fileStream = await actFile.OpenStreamForReadAsync();

            this.activityItems = new List<Activity>();

            //rewrite all and add catagory into catagory.txt          
            using (StreamReader read = new StreamReader(fileStream))
            {
                string line;
                while ((line = read.ReadLine()) != null)
                {
                    //split line, and store data in list
                    String[] info = line.Split('\t');
                    if (checkNewActivity(info[0])==true)
                    {
                        Activity filedata = new Activity() { actName = info[0], actDue = Convert.ToDateTime(info[1]), actFinished = Convert.ToBoolean(info[2]) };
                        activityItems.Add(filedata);
                    }
                   
                }
            }
            return true;
        }

        bool checkNewActivity(String act)
        {
            foreach (Activity a in activityItems)
            {
                if (a.actName.Equals(act))
                {
                    return false;
                }
            }
            return true;

        }


        public  async Task<bool> updateIndividaulCatText(Catagory list, String delActName)
        {
            //get catagory.txt path
            StorageFile catFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\"+list.catName+".txt");

            //start a file stream for writing
            Stream fileStream = await catFile.OpenStreamForWriteAsync();

            List<Activity> toDelete = new List<Activity>();

            //rewrite all and add catagory into catagory.txt          
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                foreach (Activity line in list.activityItems)
                {   //if the user has deleted the catagory, do not save in file
                    if (line.isDeleted == false && !line.actName.Equals(delActName))
                    {
                        writer.WriteLine(line.actName+"\t"+line.actDue + "\t" +line.actFinished);
                    }
                    else
                    {
                        toDelete.Add(line);
                    }
                }
            }

            foreach(Activity act in toDelete)
            {
                //if activity is deleted remove from activity list
                activityItems.Remove(act);
            }
            return true;
        }

    }
}
