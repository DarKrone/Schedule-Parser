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
            //var url = "http://techn.sstu.ru/new/RaspSPO/www_2semestr/cg135.htm";
            var url = "http://techn.sstu.ru/new/RaspSPO/www-2023/cg87.htm";
            //GetScheduleForWeekConsole(url);
            GetScheduleForWeekInFile(url);
        }

        static void GetScheduleForWeekInFile(string url, string path = "ScheduleForWeek.txt")
        {
            try
            {
                var request = new GetRequest(url);
                request.Run();
                var response = request.Response;
                var dateToday = DateTime.Today;
                //var dateToday = new DateTime(2023,06,26);
                DateTime dateIterate = dateToday;
                string scheduleForWeek = "";
                for (int i = 1; i <= 7; i++)
                {
                    scheduleForWeek += dateIterate.ToString("dd.MM.yyyy") + "\n";
                    scheduleForWeek += GetScheduleForDay(response, dateIterate) + "\n";
                    dateIterate = dateToday.AddDays(i);
                }

                using(StreamWriter writer = new StreamWriter(path,false))
                {
                    writer.Write(scheduleForWeek);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogCreater logs = new LogCreater();
                logs.CreateLog(ex, "log.txt");
            }

        }

        
        static void GetScheduleForWeekConsole(string url)
        {
            try
            {
                var request = new GetRequest(url);
                request.Run();
                var response = request.Response;
                //var dateToday = DateTime.Today;
                var dateToday = new DateTime(2023, 06, 26);
                DateTime dateIterate = dateToday;
                for (int i = 1; i <= 7; i++)
                {
                    Console.WriteLine(dateIterate.ToString("dd.MM.yyyy"));
                    string scheduleForDay = GetScheduleForDay(response, dateIterate);
                    Console.WriteLine(scheduleForDay);
                    dateIterate = dateToday.AddDays(i);
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
                LogCreater logs = new LogCreater();
                logs.CreateLog(ex, "log.txt");
            }
            
        }

        //static void GetScheduleForToday(string url)
        //{
        //    var request = new GetRequest(url);
        //    request.Run();
        //    var response = request.Response;
        //    var dateToday = DateTime.Today;
        //    Console.WriteLine(dateToday.ToString("dd.MM.yyyy"));
        //    GetScheduleForDay(response, dateToday);
        //}
        static string GetScheduleForDay(string response, DateTime dateToday)
        {
            var dateTomorrow = dateToday.AddDays(1);
            var strStart = response.IndexOf(dateToday.ToString("dd.MM.yyyy"));
            var strEnd = response.IndexOf(dateTomorrow.ToString("dd.MM.yyyy"));
            var DayString = response.Substring(strStart + 23, strEnd - strStart);
            var dayOfWeekString = response.Substring(strStart, 23);
            var countPair = Regex.Matches(DayString, "z1").Count;
            string scheduleForDay = "";
            scheduleForDay += GetDayOfWeek(dayOfWeekString) + "\n";

            for (int i = 0;i < 7; i++)
            {
                var schedule = ""; // тут хранятся данные только одной пары
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

                scheduleForDay += schedule + "\n";
            }
            
            if (countPair == 0) scheduleForDay += "Отдых\n";
            return scheduleForDay;
        }

        static string GetNextSubstring(string str) // возвращает разбитый на блок учебный день, например первую пару
        {
            var indexLeftTd = str.IndexOf("<TD");
            var indexRightTd = str.IndexOf("/TD>") + 4;
            var subString = str.Substring(indexLeftTd, indexRightTd);
            return subString;
        }

        static string GetDayOfWeek(string str) // ищет и возвращает день недели, например "Сб-2"
        {
            var indexLeftTd = str.IndexOf(">");
            var indexRightTd = str.IndexOf("</");
            var subString = str.Substring(indexLeftTd + 1, indexRightTd - indexLeftTd - 1);
            return subString;
        }

        static string GetTime(string response) // возвращает время пары, если пары нету, то дальше это учтётся
        {
            var strStart = response.IndexOf("<br>") + 4;
            var strEnd = response.IndexOf('<', strStart);
            var pair = response.Substring(strStart, strEnd - strStart);
            return pair;
        }
        static string GetPair(string response) // возвращает название пары 
        {
            var strStart = response.IndexOf("Журнал занятий\">") + 16;
            var strEnd = response.IndexOf('<', strStart);
            var pair = response.Substring(strStart, strEnd - strStart);
            return pair;
        }

        static string GetCab(string response) // возвращает кабинет пары
        {
            var strStart = response.IndexOf("Расписание аудитории\">") + 22;
            var strEnd = response.IndexOf('<', strStart);
            var cab = response.Substring(strStart, strEnd - strStart);
            return cab;
        }

        static string GetTeacher(string response) // возвращает преподавателя
        {
            var strStart = response.IndexOf("Расписание преподавателя\">") + 26;
            var strEnd = response.IndexOf('<', strStart);
            var teacher = response.Substring(strStart, strEnd - strStart);
            return teacher;
        }
    }
}
