using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Models
{
    public class WebsiteReviewTransfer
    {
        public static List<object> TransfertoDate(IQueryable<WebsiteReview> origin)
        {
            string newDate;
            List<object> result = new List<object>();

            foreach (var o in origin)
            {
                if (o.ReviewDate != null)
                {
                    newDate = o.ReviewDate.ToString();
                    string[] newdatelist = newDate.Split(' ');
                    result.Add(new
                    {
                        ReviewID = o.ReviewID,
                        MemberID = o.MemberID,
                        ReviewDate = newdatelist[0],
                        Type = o.Type,
                        RContent = o.RContent,
                        Status = o.Status,
                        Remark = o.Remark
                    });
                }
                else
                {
                    result.Add(o);
                }
            }

            return result;

        }

    }
}