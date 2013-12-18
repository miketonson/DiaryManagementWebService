using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiaryManagementWebService
{
    public class ManagementItem
    {
        public DateTime addTime;
        public int itemType;
    }

    public class BillManagement : ManagementItem
    {
        public int amount;
        public string reason;
        public Boolean type;
       
    }
    
    public class DietManagement : ManagementItem
    {
        public string amount;
        public string food;
        public DateTime time;
    }
    public class ExerciseManagement : ManagementItem
    {
        public string amount;
        public string exercise;
        public DateTime time;
    }

    public class Management
    {
        public List<ManagementItem> items;
        //public int index;

        public Management()
        {
            items = new List<ManagementItem>();
            //index = 0;
        }
    }

    public class Diary
    {
        public string content;
        public DateTime modified,created;
        public string title;
        public Management management;
        /*public void edit(int startindex, int count, string str)
        {
            content.Remove(startindex, count);
            content.Insert(startindex, str);
        }*/
    }
}