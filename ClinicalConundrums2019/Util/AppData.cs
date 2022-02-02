using ClinicalConundrums2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Util
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
                      new SelectListItem {Text = "Newfoundland" , Value = "NL"},
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


        public static List<ProvinceModel> GetProvinces()
        {
            var list = new List<ProvinceModel>
            {
                 new ProvinceModel{Id = "1", Name = "AB", Checked = false},
                 new ProvinceModel{Id = "2", Name = "BC", Checked = false},
                 new ProvinceModel{Id = "3", Name = "MB", Checked = false},
                 new ProvinceModel{Id = "4", Name = "NS", Checked = false},
                 new ProvinceModel{Id = "5", Name = "NB", Checked = false},
                 new ProvinceModel{Id = "6", Name = "NL", Checked = false},
                 new ProvinceModel{Id = "7", Name = "ON", Checked = false},
                 new ProvinceModel{Id = "8", Name = "PEI", Checked = false},
                 new ProvinceModel{Id = "9", Name = "QC", Checked = false},
                 new ProvinceModel{Id = "10", Name = "SK", Checked = false},

            };

            return list;


        }


       

        public static IEnumerable<SelectListItem> GetLocationType()
        {
            List<SelectListItem> LocationTypeList = new List<SelectListItem>
            {

                    new SelectListItem {Text = "Restaurant" , Value = "Restaurant"},
                    new SelectListItem {Text = "Hospital Boardroom/Auditorium" , Value = "HospitalBoardroom"},
                    new SelectListItem {Text = "Hotel" , Value = "Hotel"},
                    new SelectListItem {Text = "Long Term Care Facility" , Value = "LongTermCareFacility"},
                    new SelectListItem {Text = "MedicalClinic/Office" , Value = "MedicalClinic"},
                    new SelectListItem {Text = "Other" , Value = "Other"},


            };
            return LocationTypeList;

        }






        public static IEnumerable<SelectListItem> GetSpeakerRoleList()
        {
            List<SelectListItem> SpeakerRoleList = new List<SelectListItem>
            {
                    new SelectListItem {Text = "Speaker", Value = "1"},
                    new SelectListItem {Text = "Moderator" , Value = "2"},
                    new SelectListItem {Text = "Both" , Value = "3"},



            };
            return SpeakerRoleList;

        }

        public static IEnumerable<SelectListItem> GetTherapeuticArea()
        {
            List<SelectListItem> TherapeuticAreaList = new List<SelectListItem>
            {

                    new SelectListItem {Text = "CV" , Value = "1"},
                    new SelectListItem {Text = "Bone" , Value = "2"},
                    new SelectListItem {Text = "Both" , Value = "3"}


            };
            return TherapeuticAreaList;

        }

    }
}