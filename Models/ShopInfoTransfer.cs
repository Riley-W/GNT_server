using GNT_server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Models
{
    public class ShopInfoTransfer
    {
        public static string TypeTransfer(string type)
        {
            string realtype;
            if (type == "bar")
                realtype = "酒吧";
            else if (type == "snack")
                realtype = "小吃宵夜";
            else if (type == "dessert")
                realtype = "咖啡甜點";
            else if (type == "viewpoint")
                realtype = "夜間景點";
            else
                realtype = "";
            return realtype;
        }
        public static List<string> CheckTagList(IQueryable<Tag> tagname)
        {
            string a = "";
            List<string> taglist = new List<string>();
            if (Global.TagList.Count == 0)
            {

                foreach (var tname in tagname)
                {
                    a = tname.Tag1 + "," + tname.TagName;
                    taglist.Add(a);
                }
                Global.TagList = taglist;
            }
            else
            {
                taglist = Global.TagList;
            }
            return taglist;
        }
        public static IQueryable<ShopInfo> ChangeTagtoChinese(IQueryable<ShopInfo> result, IQueryable<Tag> t)
        {
            List<string> taglist = new List<string>();
            taglist = CheckTagList(t);
            string tags = "";
            foreach (var r in result)
            {
                tags = "";
                if (!string.IsNullOrEmpty(r.TagIds))
                {
                    string[] ids = r.TagIds.Split(',');
                    foreach (string i in ids)
                    {
                        for (int j = 0; j < taglist.Count(); j++)
                        {
                            if (taglist[j].Contains(i))
                            {
                                string[] tag = taglist[j].Split(',');
                                tags += tag[1] + ",";
                                break;
                            }
                        }

                    }
                    if (tags.Trim().Substring(tags.Trim().Length - 1, 1) == ",")
                    {
                        tags = tags.Trim().Substring(0, tags.Trim().Length - 1);
                    }
                    r.TagIds = tags;

                }
            }
            return result;
        }

        public static string ChangeChinesetoTag(ShopInfo shopinfo, IQueryable<Tag> t)
        {
            List<string> taglist = new List<string>();
            taglist = CheckTagList(t);
            string tags = "";
            if (!string.IsNullOrEmpty(shopinfo.TagIds))
            {
                string[] tagc = shopinfo.TagIds.Split(',');
                foreach (string tag in tagc)
                {
                    for (int i = 0; i < taglist.Count(); i++)
                    {
                        if (taglist[i].Contains(tag))
                        {
                            if (i < 9)
                            {
                                tags += "T" + "0" + $"{i+1}"+",";
                            }
                            else
                            {
                                tags += "T" + $"{i+1}" + ",";
                            }

                            break;
                        }
                    }
                    if (tags == "")
                    {
                        tags = shopinfo.TagIds;
                    }

                }
                if (tags.Trim().Substring(tags.Trim().Length - 1, 1) == ",")
                {
                    tags = tags.Trim().Substring(0, tags.Trim().Length - 1);
                }
                shopinfo.TagIds = tags;

            }
            
            return tags;
        }
    }
}