using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Util
{
    public class BusinessDaysCalc
    {
        public static bool IsCanadaWorkday(DateTime date)
        {
            return (!IsCanadaHoliday(date) && date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);
        }
        public static bool IsSameDay(DateTime date1, DateTime date2) { return date1.Date == date2.Date; }
        public static bool IsCanadaHoliday(DateTime date)
        {
            return (
             IsSameDay(GetVictoriaDay(date.Year), date) ||
             IsSameDay(GetCivicHoliday(date.Year), date) ||

          IsSameDay(GetThanksgiving(date.Year), date) ||
           IsSameDay(GetLabourDay(date.Year), date) ||
                 IsSameDay(GetCanadaDay(date.Year), date) ||
              IsSameDay(GetFamilyDay(date.Year), date) ||

             IsSameDay(GetChristmasDay(date.Year), date) ||
             IsSameDay(GetBoxingDay(date.Year), date) ||
             IsSameDay(GetGoodFriday(date.Year), date) ||

             IsSameDay(GetNewYearsDay(date.Year), date)
             );
        }

        public static int GetWorkingdays(DateTime from, DateTime to)
        {
            int limit = 9999;
            int counter = 0;
            DateTime current = from;
            int result = 0;

            if (from > to)
            {
                DateTime temp = from;
                from = to;
                to = temp;
            }

            if (from >= to)
            {
                return 0;
            }


            while (current <= to && counter < limit)
            {
                if (IsCanadaWorkday(current))
                {
                    result++;
                }
                current = current.AddDays(1);
                counter++;

            }
            return result;
        }
        // Juldagen
        public static DateTime GetNewYearsDay(int year)
        {
            return new DateTime(year, 1, 1);
        }

        // Juldagen
        public static DateTime GetChristmasDay(int year)
        {
            return new DateTime(year, 12, 25);
        }

        // Annandag jul
        public static DateTime GetBoxingDay(int year)
        {
            return new DateTime(year, 12, 26);
        }
        public static DateTime GetCanadaDay(int year)
        {
            return new DateTime(year, 7, 2);
        }

        public static DateTime GetCivicHoliday(int year)
        {
            return new DateTime(year, 8, 6);
        }
        public static DateTime GetThanksgiving(int year)
        {
            return new DateTime(year, 10, 8);
        }
        public static DateTime GetLabourDay(int year)
        {
            return new DateTime(year, 9, 3);
        }
        public static DateTime GetVictoriaDay(int year)
        {
            return new DateTime(year, 5, 21);
        }
        // Långfredagen
        public static DateTime GetGoodFriday(int year)
        {
            return GetEasterDay(year).AddDays(-3);
        }
        public static DateTime GetFamilyDay(int year)
        {
            return new DateTime(year, 2, 19);
        }
        public static DateTime GetEasterDay(int y)
        {
            double c;
            double n;
            double k;
            double i;
            double j;
            double l;
            double m;
            double d;
            c = System.Math.Floor(y / 100.0);
            n = y - 19 * System.Math.Floor(y / 19.0);
            k = System.Math.Floor((c - 17) / 25.0);
            i = c - System.Math.Floor(c / 4) - System.Math.Floor((c - k) / 3) + 19 * n + 15;
            i = i - 30 * System.Math.Floor(i / 30);
            i = i - System.Math.Floor(i / 28) * (1 - System.Math.Floor(i / 28) * System.Math.Floor(29 / (i + 1)) * System.Math.Floor((21 - n) / 11));
            j = y + System.Math.Floor(y / 4.0) + i + 2 - c + System.Math.Floor(c / 4);
            j = j - 7 * System.Math.Floor(j / 7);
            l = i - j;
            m = 3 + System.Math.Floor((l + 40) / 44);// month
            d = l + 28 - 31 * System.Math.Floor(m / 4);// day

            double days = ((m == 3) ? d : d + 31);

            DateTime result = new DateTime(y, 3, 1).AddDays(days - 1);

            return result;
        }
    }
}