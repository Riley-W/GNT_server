using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Models
{
    public class Query
    {
        public static string QueryFromList(string tag)
        {
            List<string>QueryList = new List<string>() 
            {
                "餐酒館",//0
                "西班牙",//1
                "調酒" ,//2
               "酒吧",//3
                "日式酒店",//4
                "老宅" ,//5
                "精釀啤酒",//6
                "居酒屋" ,//7
                "水果調酒",//8
                "創意調酒", //9
                "經典調酒"//10
            };
            int tagmax = QueryList.Count();
            string[] querytags = tag.Split(',');
            string newquerystring="";
            List<string> Searchstringlist = new List<string>();
            for (int i = 0; i <= tagmax; i++)
            {
                if (querytags.Contains($"{i}"))
                {
                   string q= QueryList[i];
                    Searchstringlist.Add(q);
                }
            }
            foreach(string st in Searchstringlist)
            {
                newquerystring +=st+",";
            }
            if (newquerystring.Trim().Substring(newquerystring.Trim().Length - 1, 1) == ",")
            {
                newquerystring = newquerystring.Trim().Substring(0, newquerystring.Trim().Length - 1);
            }
            return newquerystring;

        }
    }
}