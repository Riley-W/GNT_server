using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Models
{
    public class RouteTransfer
    {
        public static List<object> changetime(IQueryable<Route> route)
        {
            string reDate;
            List<object> aa = new List<object>();
            foreach (var ss in route)
            {
                if (ss.AddDate != null)
                {
                    
                    reDate = ss.AddDate.ToString();
                    string[] dd = reDate.Split(' ');
                    aa.Add(new
                    {
                        RouteID=ss.RouteID,
                        MemberID = ss.MemberID,
                        Title = ss.Title,
                        AddDate = dd[0],
                        Dest1 = ss.Dest1,ss.ShopInfo,
                        Dest2 = ss.Dest2,ss.ShopInfo1,
                        Dest3 = ss.Dest3,ss.ShopInfo2,
                        Dest4 = ss.Dest4,ss.ShopInfo3,
                        Dest5 = ss.Dest5,ss.ShopInfo4,
                        Dest6 = ss.Dest6,ss.ShopInfo5,
                        Dest7 = ss.Dest7,ss.ShopInfo6,
                        Dest8 = ss.Dest8,ss.ShopInfo7
                    });
                    //aa.Add(new
                    //{
                    //    Dest1 = ss.ShopInfo,
                    //    Dest2 = ss.ShopInfo1,
                    //    Dest3 = ss.ShopInfo2,
                    //    Dest4 = ss.ShopInfo3,
                    //    Dest5 = ss.ShopInfo4,
                    //    Dest6 = ss.ShopInfo5,
                    //    Dest7 = ss.ShopInfo6,
                    //    Dest8 = ss.ShopInfo7,
                    // });    
                        

                }
                else
                {
                    aa.Add(ss);
                }

            }
            return aa;
        }
    }
}