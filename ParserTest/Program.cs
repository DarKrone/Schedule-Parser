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
            var url = "http://techn.sstu.ru/new/RaspSPO/www-2024/cg75.htm";
            GetScheduleForWeekConsole(url);
            //GetScheduleForWeekInFile(url);
        }

        static void GetScheduleForWeekInFile(string url, string path = "ScheduleForWeek.txt")
        {
            try
            {
                GetRequest request = new GetRequest(url);
                request.Run();
                string response = request.Response;
                DateTime dateToday = DateTime.Today;
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
                LogCreater.CreateLog(ex, "log.txt");
            }

        }

        
        static void GetScheduleForWeekConsole(string url)
        {
            try
            {
                GetRequest request = new GetRequest(url);
                request.Run();
                string response = request.Response;
                //var dateToday = DateTime.Today;
                DateTime dateToday = new DateTime(2024, 06, 26); // такая дата т.к. расписание не обновляли и нынешней даты нету
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
                LogCreater.CreateLog(ex, "log.txt");
            }
            
        }

        static string GetScheduleForDay(string response, DateTime dateToday)
        {
            DateTime dateTomorrow = dateToday.AddDays(1);
            int strStart = response.IndexOf(dateToday.ToString("dd.MM.yyyy"));
            int strEndIndex = response.IndexOf(dateTomorrow.ToString("dd.MM.yyyy"));
            string dayString = response.Substring(strStart + 23, strEndIndex - strStart);
            string dayOfWeekString = response.Substring(strStart, 23);
            int countPair = Regex.Matches(dayString, "z1").Count;
            string scheduleForDay = "";
            scheduleForDay += GetDayOfWeek(dayOfWeekString) + "\n";

            for (int i = 0;i < 7; i++)
            {
                string onePair = ""; // тут хранятся данные только одной пары
                string tdString = GetNextDaySubstring(dayString);
                dayString = dayString.Substring(dayString.IndexOf(tdString) + tdString.Length);
                onePair += GetPairTime(tdString) + " | ";

                tdString = GetNextDaySubstring(dayString);
                dayString = dayString.Substring(dayString.IndexOf(tdString) + tdString.Length + 9);
                if (tdString.Contains("nul")) // проверка на отсутствие пары
                {
                    continue;
                }
                onePair += GetPair(tdString);
                onePair += " | " + GetCab(tdString);
                onePair += " | " + GetTeacher(tdString);

                scheduleForDay += onePair + "\n";
            }
            
            if (countPair == 0) scheduleForDay += "Отдых\n";
            return scheduleForDay;
        }

        static string GetNextDaySubstring(string str) // возвращает разбитый на блок учебный день, например первую пару
        {
            int indexLeftTd = str.IndexOf("<TD");
            int indexRightTd = str.IndexOf("/TD>") + 4;
            string daySubstring = str.Substring(indexLeftTd, indexRightTd);
            return daySubstring;
        }

        static string GetDayOfWeek(string str) // ищет и возвращает день недели, например "Сб-2"
        {
            int indexLeftTd = str.IndexOf(">");
            int indexRightTd = str.IndexOf("</");
            string dayOfWeek = str.Substring(indexLeftTd + 1, indexRightTd - indexLeftTd - 1);
            return dayOfWeek;
        }

        static string GetPairTime(string response) // возвращает время пары, если пары нету, то дальше это учтётся
        {
            int strStart = response.IndexOf("<br>") + 4;
            int strEnd = response.IndexOf('<', strStart);
            string pairTime = response.Substring(strStart, strEnd - strStart);
            return pairTime;
        }
        static string GetPair(string response) // возвращает название пары 
        {
            int strStart = response.IndexOf("Журнал занятий\">") + 16;
            int strEnd = response.IndexOf('<', strStart);
            string pair = response.Substring(strStart, strEnd - strStart);
            return pair;
        }

        static string GetCab(string response) // возвращает кабинет пары
        {
            int strStart = response.IndexOf("Расписание аудитории\">") + 22;
            int strEnd = response.IndexOf('<', strStart);
            string cab = response.Substring(strStart, strEnd - strStart);
            return cab;
        }

        static string GetTeacher(string response) // возвращает преподавателя
        {
            int strStart = response.IndexOf("Расписание преподавателя\">") + 26;
            int strEnd = response.IndexOf('<', strStart);
            string teacher = response.Substring(strStart, strEnd - strStart);
            return teacher;
        }
    }
}
