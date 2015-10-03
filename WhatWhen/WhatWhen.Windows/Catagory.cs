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

        //create new catgory file and add to list
        public void catagoryCreate(List<Catagory> list)
        {
            //wait until catagory name is set
            while (catName == null) { }

            //create individual file
            createFile(catName);

            //add to catagory list then update catagory file
            list.Add(this);
            updateCatagoryText(list);
        }

        //update catagory.txt file which contains names of categories
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
                foreach (Catagory line in list)
                {   //if the user has deleted the catagory, do not save in file
                    if (line.isDeleted == false)
                    {
                        writer.WriteLine(line.catName);
                    }
                    else
                    {
                        //remove the deleted catagory's file and from list
                        deleteFile(line.catName + ".txt");
                        toDelete.Add(line);
                    }
                }//write ---- to show end of activities
                writer.WriteLine("----");

            }

            foreach (Catagory c in toDelete)
            {
                list.Remove(c);
            }

            return true;
        }

        //create file
        async void createFile(string newFileName)
        {
            StorageFolder storeFolder = await StorageFolder.GetFolderFromPathAsync(MainPage.path);

            //create own file
            if (await storeFolder.TryGetItemAsync(newFileName) == null)
            {
                StorageFile catagoryFile = await storeFolder.CreateFileAsync(newFileName + ".txt");
            }
        }

        // get categories from text file
        public async Task<bool> getCatagories(MainPage mainP)
        {
            //get catagory.txt path
            var catagoryFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\catagory.txt");
            if (catagoryFile != null)
            {
                Stream fileStream = await catagoryFile.OpenStreamForReadAsync();

                //rewrite all and add catagory into catagory.txt          
                using (StreamReader read = new StreamReader(fileStream))
                {
                    string line = read.ReadLine();
                    while (line != null)
                    {
                        if (!line.Equals("----")) //reach end of ehat need to be read
                        {
                            //read line, and store data in list
                            Catagory filedata = new Catagory() { catName = line, isDeleted = false };
                            mainP.addtoList(line);

                            line = read.ReadLine();
                        }
                        else
                        {
                            return true;
                        }
                    }


                }
            }
            return true;
        }

        //delete file
        async void deleteFile(String filename)
        {
            StorageFile deleteFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\" + filename);
            await deleteFile.DeleteAsync();
        }

        //get activites per category
        public async Task<bool> getActivitiesInCatagory()
        {
            //get text path
            StorageFile actFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\" + this.catName + ".txt");

            //start a file stream for reading
            Stream fileStream = await actFile.OpenStreamForReadAsync();

            this.activityItems = new List<Activity>();

            //rewrite all and add catagory into catagory.txt          
            using (StreamReader read = new StreamReader(fileStream))
            {
                string line = read.ReadLine();
                while (line != null)
                {
                    if (!line.Equals("----"))
                    {

                        //split line, and store data in list
                        String[] info = line.Split('\t');
                        if (checkNewActivity(info[0]) == true)
                        {
                            Activity filedata = new Activity() { actName = info[0], actDue = Convert.ToDateTime(info[1]), actFinished = Convert.ToBoolean(info[2]) };
                            activityItems.Add(filedata);

                            line = read.ReadLine();
                        }
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            return true;
        }

        //check new activity for category is unique
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

        //update categorie's unique text file with its list of activities
        public async Task<bool> updateIndividaulCatText(Catagory list, String delActName)
        {
            //get catagory.txt path
            StorageFile catFile = await StorageFile.GetFileFromPathAsync(MainPage.path + @"\" + list.catName + ".txt");

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
                        writer.WriteLine(line.actName + "\t" + line.actDue + "\t" + line.actFinished);
                    }
                    else
                    {
                        toDelete.Add(line);
                    }
                } //show end of file
                writer.WriteLine("----");
            }

            foreach (Activity act in toDelete)
            {
                //if activity is deleted remove from activity list
                activityItems.Remove(act);
            }
            return true;
        }

    }
}
