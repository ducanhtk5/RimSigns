using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;
using System.Web.Mvc;
using CMS.Service.Repository;

namespace CMSApplication.Models
{
    public class OrderModels
    {
        public int Id { get; set; }
        public string OrderByName { get; set; }
        public int OrderById { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public double OrderPercent { get; set; }
        public List<string> StaffID { get; set; }
        public string Parties { get; set; }

        public string ServiceType { get; set; }
        public string PartiesInformation { get; set; }
        public string BannerSize { get; set; }
        public string LatLng { get; set; }
        public Nullable<int> Quantity { get; set; }
        /////////// File Upload ///////////////
        public int FileId { get; set; }
        public string FileName { get; set; }

        public OrderModels()
        {
            Quantity = 0;
            OrderPercent = 0;
            OrderById = 0;
            Status = "Recieved";
            Size = _size.First();
            Type = _type.First();
            ServiceType = _service_type.First();
            BannerSize = "";
            Description = "";
            ServiceType = "";
            Link = "";
            PartiesInformation = "";
        }

        public static void ToModel(Order entity, ref OrderModels model)
        {
            model.Id = entity.Id;
            model.OrderById = entity.OrderById;
            model.OrderByName = entity.OrderByName;
            model.OrderDate = entity.OrderDate;
            model.Address = entity.Address;
            model.City = entity.City;
            model.Size = entity.Size;
            model.Type = entity.Type;
            model.Status = entity.Status;
            model.Link = entity.Link;
            model.Description = entity.Description;
            model.Title = entity.Title;
            model.OrderPercent = (double)entity.OrderPercent;
            model.Parties = entity.Parties;
            model.Quantity = entity.Quantity;
            model.ServiceType = entity.ServiceType;
            model.PartiesInformation = entity.PartiesInformation;
            model.LatLng = entity.LatLng;
            model.BannerSize = entity.BannerSize;
            model.StaffID = (List<string>)CMS.Common.Helpers.StringHelpers.ConvertStringToList(entity.StaffID);
        }
        public static void ToEntity(OrderModels model, ref Order entity)
        {
            entity.Id = model.Id;
            entity.OrderById = model.OrderById;
            entity.OrderByName = model.OrderByName;
            entity.OrderDate = model.OrderDate;
            entity.Address = model.Address;
            entity.City = model.City;
            entity.Size = model.Size;
            entity.Type = model.Type;
            entity.Status = model.Status;
            entity.Link = model.Link;
            entity.Description = model.Description;
            entity.Title = model.Title;
            entity.OrderPercent = (int)model.OrderPercent;
            entity.Parties = model.Parties;
            entity.Quantity = model.Quantity;
            entity.ServiceType = model.ServiceType;
            entity.PartiesInformation = model.PartiesInformation;
            entity.LatLng = model.LatLng;
            entity.BannerSize = model.BannerSize;
            entity.StaffID = CMS.Common.Helpers.StringHelpers.ConvertListToString(model.StaffID);
        }

        public SelectList SizeSelectList { get; set; }
        public SelectList TypeSelectList { get; set; }
        SelectList StatusSelectList { get; set; }
        public SelectList StaffSelectList { get; set; }
        public SelectList ServiceTypeSelectlist { get; set; }
        public List<SelectListItem> PartiesSelectList { get; set; }
        string[] _size = { "Not specified", "2x3", "4x4", "4x6", "4x8", "6x9", "8x8", "8x12", "8x10" };
        string[] _status = {"Not specified", "Recieved", "Progress", "Finished Design", "Installed Design", "Complete" };
        string[] _type = { "Not specified", "Single face", "Double face", "V-Shape" };
        string[] _service_type = { "Not specified", "New Install", "Stock Install", "Text Edit", "Remove", "Repair" };
        public void GetDataForDropdownList()
        {
            SizeSelectList = new SelectList(_size);
            TypeSelectList = new SelectList(_type);
            StatusSelectList = new SelectList(_status);
            ServiceTypeSelectlist = new SelectList(_service_type, ServiceType);

            //if (Parties != null)
            //{
            //    PartiesSelectList = new List<SelectListItem>();
            //    foreach (var item in Parties)
            //    {
            //        SelectListItem s = new SelectListItem();
            //        s.Text = item;
            //        s.Value = item;
            //        s.Selected = true;
            //        PartiesSelectList.Add(s);
            //    }
            //}
            
        }
    }
}
