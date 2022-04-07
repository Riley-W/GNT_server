using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Models
{
    public class MemberInfoTransfer
    {
        public static List<object> TransfertoDateForAdmin(IQueryable<MemberInfo> origin)
        {
            string newBDate;
            string newRDate;
            List<object> result = new List<object>();

            foreach (var o in origin)
            {
                if (o.BirthDate != null&&o.RegisterDate!=null)
                {
                    newBDate = o.BirthDate.ToString();
                    string[] newBdatelist = newBDate.Split(' ');
                    newRDate = o.RegisterDate.ToString();
                    string[] newRdatelist = newRDate.Split(' ');
                    result.Add(new
                    {
                        MemberID=o.MemberID,
                        Name=o.Name,
                        Phone=o.Phone,
                        Address=o.Address,
                        Gender=o.Gender,
                        BirthDate=newBdatelist[0],
                        Email=o.Email,
                        RegisterDate= newRdatelist[0],
                        BlackList=o.BlackList,
                        Account=o.Account,
                        Password=o.Password

                    });
                }
                else
                {
                    result.Add(o);
                }
            }

            return result;

        }

        public static List<object> TransfertoDateForUser(IQueryable<MemberInfo> origin)
        {
            string newBDate;
            string newRDate;
            List<object> result = new List<object>();

            foreach (var o in origin)
            {
                if (o.BirthDate != null && o.RegisterDate != null)
                {
                    newBDate = o.BirthDate.ToString();
                    string[] newBdatelist = newBDate.Split(' ');
                    newRDate = o.RegisterDate.ToString();
                    string[] newRdatelist = newRDate.Split(' ');
                    result.Add(new
                    {
                        MemberID = o.MemberID,
                        Name = o.Name,
                        Phone = o.Phone,
                        Address = o.Address,
                        Gender = o.Gender,
                        BirthDate = newBdatelist[0],
                        Email = o.Email,
                        RegisterDate = newRdatelist[0],
                        BlackList = o.BlackList,
                        Account=o.Account,
                        Password=o.Password
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