using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Models
{
    public class Query
    {
        public  projectDBEntities db = new projectDBEntities();
        public static string QueryFromList(string tag)
        {

            string[] Qtags = tag.Split(',');
            int tagmax = Qtags.Count();
            List<ShopTag> Queryshoplist = new List<ShopTag>();
            for (int i = 0; i < tagmax; i++)
            {
                var shopid = from s in db.ShopTag
                              where s.TagID == i
                              select s;
                Queryshoplist.Add(shopid);
            }
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