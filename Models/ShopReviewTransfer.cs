using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Models
{
    public class ShopReviewTransfer
    {
        public static List<object> changetime(IQueryable<ShopReview> shopreview)
        {
            string date;
            List<object> datelist = new List<object>();
            foreach (var ss in shopreview)
            {
                if (ss.ReviewDate != null)
                {
                    date = ss.ReviewDate.ToString();
                    string[] dd = date.Split(' ');
                    datelist.Add(new
                    {
                        MemberID = ss.MemberID,
                        MemberInfo=ss.MemberInfo,
                        ShopID = ss.ShopID,
                        ReviewDate = dd[0],
                        RContent=ss.RContent,
                        Score = ss.Score
                    });

                }
                else
                {
                    datelist.Add(ss);
                }
            }
            return datelist;
        }
    }
}