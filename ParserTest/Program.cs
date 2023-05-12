using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var url = "http://techn.sstu.ru/new/RaspSPO/www_2semestr/cg135.htm";
            GetScheduleForWeek(url);
            Console.ReadKey();
        }

        static void GetScheduleForWeek(string url)
        {
            var request = new GetRequest(url);
            request.Run();
            var response = request.Response;
            var dateToday = DateTime.Today;
            DateTime dateIterate = dateToday;
            for (int i = 1; i <= 7; i++)
            {
                Console.WriteLine(dateIterate.ToString("dd.MM.yyyy"));
                GetScheduleForDay(response, dateIterate);
                Console.WriteLine();
                dateIterate = dateToday.AddDays(i);
            }
        }

        static void GetScheduleForToday(string url)
        {
            var request = new GetRequest(url);
            request.Run();
            var response = request.Response;
            var dateToday = DateTime.Today;
            Console.WriteLine(dateToday.ToString("dd.MM.yyyy"));
            GetScheduleForDay(response, dateToday);
        }
        static void GetScheduleForDay(string response, DateTime dateToday)
        {
            var dateTomorrow = dateToday.AddDays(1);
            var strStart = response.IndexOf(dateToday.ToString("dd.MM.yyyy"));
            var strEnd = response.IndexOf(dateTomorrow.ToString("dd.MM.yyyy"));
            var DayString = response.Substring(strStart + 23, strEnd - strStart);
            var dayOfWeekString = response.Substring(strStart, 23);
            var countPair = Regex.Matches(DayString, "z1").Count;
            Console.WriteLine(GetDayOfWeek(dayOfWeekString));

            for (int i = 0;i < 9; i++)
            {
                var schedule = "";
                var tdString = GetNextSubstring(DayString);
                DayString = DayString.Substring(DayString.IndexOf(tdString) + tdString.Length);
                schedule += GetTime(tdString) + " | ";

                tdString = GetNextSubstring(DayString);
                DayString = DayString.Substring(DayString.IndexOf(tdString) + tdString.Length + 9);
                if (tdString.Contains("nul"))
                {
                    continue;
                }
                schedule += GetPair(tdString);
                schedule += " | " + GetCab(tdString);
                schedule += " | " + GetTeacher(tdString);

                Console.WriteLine(schedule);
            }
            if (countPair ==0) Console.WriteLine("Отдых");
        }

        static string GetNextSubstring(string str)
        {
            var indexLeftTd = str.IndexOf("<TD");
            var indexRightTd = str.IndexOf("/TD>") + 4;
            var subString = str.Substring(indexLeftTd, indexRightTd);
            return subString;
        }

        static string GetDayOfWeek(string str)
        {
            var indexLeftTd = str.IndexOf(">");
            var indexRightTd = str.IndexOf("</");
            var subString = str.Substring(indexLeftTd + 1, indexRightTd - indexLeftTd - 1);
            return subString;
        }

        static string GetTime(string response)
        {
            var strStart = response.IndexOf("<br>") + 4;

            var strEnd = response.IndexOf('<', strStart);

            var pair = response.Substring(strStart, strEnd - strStart);
            return pair;
        }
        static string GetPair(string response)
        {
            var strStart = response.IndexOf("Журнал занятий\">") + 16;

            var strEnd = response.IndexOf('<', strStart);

            var pair = response.Substring(strStart, strEnd - strStart);
            return pair;
        }

        static string GetCab(string response)
        {
            var strStart = response.IndexOf("Расписание аудитории\">") + 22;

            var strEnd = response.IndexOf('<', strStart);

            var cab = response.Substring(strStart, strEnd - strStart);

            return cab;
        }

        static string GetTeacher(string response)
        {
            var strStart = response.IndexOf("Расписание преподавателя\">") + 26;

            var strEnd = response.IndexOf('<', strStart);

            var teacher = response.Substring(strStart, strEnd - strStart);

            return teacher;
        }
    }
}
