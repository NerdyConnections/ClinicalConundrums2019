using ClinicalConundrumsFrenchSpeaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrumsFrenchSpeaker.Util
{
   
        public class AppData
        {
            public static IEnumerable<SelectListItem> GetPaymentMethodList()
            {
                List<SelectListItem> PaymentMethodList = new List<SelectListItem>
            {

                    new SelectListItem {Text = "Cash" , Value = "Cash"},
                    new SelectListItem {Text = "Cheque" , Value = "Cheque"},

            };
                return PaymentMethodList;

            }
            public static IEnumerable<SelectListItem> GetProvinceList()
            {
                List<SelectListItem> ProvinceList = new List<SelectListItem>
            {

                    new SelectListItem {Text = "Alberta" , Value = "AB"},
                    new SelectListItem {Text = "British Columbia" , Value = "BC"},
                    new SelectListItem {Text = "Manitoba" , Value = "MB"},
                    new SelectListItem {Text = "New Brunswick" , Value = "NB"},
                      new SelectListItem {Text = "Newfoundland and Labrador" , Value = "NL"},
                        new SelectListItem {Text = "Nova Scotia" , Value = "NS"},
                          new SelectListItem {Text = "Ontario" , Value = "ON"},
                            new SelectListItem {Text = "Prince Edward Island" , Value = "PEI"},
                              new SelectListItem {Text = "Quebec" , Value = "QC"},
                                new SelectListItem {Text = "Saskatchewan" , Value = "SK"},

            };
                return ProvinceList;

            }

            public static IEnumerable<SelectListItem> GetUserTypeList()
            {
                List<SelectListItem> UserTypeList = new List<SelectListItem>
            {
                    new SelectListItem {Text = "Sales Representative", Value = "1"},
                    new SelectListItem {Text = "Regional Manager" , Value = "4"},
                    new SelectListItem {Text = "Head Office" , Value = "5"},
                    new SelectListItem {Text = "Sales Director" , Value = "6"},


            };
                return UserTypeList;

            }


            public static IEnumerable<SelectListItem> GetMealTypeList()
            {
                List<SelectListItem> MealTypeList = new List<SelectListItem>
            {
                    new SelectListItem {Text = "Breakfast", Value = "Breakfast"},
                    new SelectListItem {Text = "Lunch" , Value = "Lunch"},
                    new SelectListItem {Text = "Dinner" , Value = "Dinner"},



            };
                return MealTypeList;

            }

            public static IEnumerable<SelectListItem> GetAVEquipmentList()
            {
                List<SelectListItem> AVEquipmentList = new List<SelectListItem>
            {
                    new SelectListItem {Text = "Available at the venue ", Value = "AvailableAtVenue"},
                    new SelectListItem {Text = "Will be provided/arranged by the Session contact (in-kind support)" , Value = "ProvidedBySessionContact"},

            };
                return AVEquipmentList;

            }

            public static IEnumerable<SelectListItem> GetSponsorList()
            {
                List<SelectListItem> UserTypeList = new List<SelectListItem>
            {
                    new SelectListItem {Text = "Amgen", Value = "1"}



            };
                return UserTypeList;

            }

        
            //public static List<TerritoryModel> GetTerritories()
            //{
            //    var list = new List<TerritoryModel>
            //{
            //     new TerritoryModel{Id = "1", TerritoryID = "41", Checked = false},
            //     new TerritoryModel{Id = "2", TerritoryID = "43", Checked = false},
            //     new TerritoryModel{Id = "3", TerritoryID = "44", Checked = false},
            //     new TerritoryModel{Id = "4", TerritoryID = "47", Checked = false},
            //     new TerritoryModel{Id = "5", TerritoryID = "48", Checked = false},
            //     new TerritoryModel{Id = "6", TerritoryID = "LTC Bone", Checked = false},


            //};

            //    return list;


            //}



        }
    }
