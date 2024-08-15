using System.Globalization;

public class PubDo
{
    static public (string, string) persianDate(DateTime? date)
    {
        PersianCalendar pc = new PersianCalendar();
        var LocalData = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(date), TimeZoneInfo.Local);
        var year = pc.GetYear(LocalData).ToString().Length != 1 ? pc.GetYear(LocalData).ToString() : "0" + pc.GetYear(LocalData).ToString();
        var Month = pc.GetMonth(LocalData).ToString().Length != 1 ? pc.GetMonth(LocalData).ToString() : "0" + pc.GetMonth(LocalData).ToString();
        var day = pc.GetDayOfMonth(LocalData).ToString().Length != 1 ? pc.GetDayOfMonth(LocalData).ToString() : "0" + pc.GetDayOfMonth(LocalData).ToString();
        var hour = pc.GetHour(LocalData).ToString().Length != 1 ? pc.GetHour(LocalData).ToString() : "0" + pc.GetHour(LocalData).ToString();
        var min = pc.GetMinute(LocalData).ToString().Length != 1 ? pc.GetMinute(LocalData).ToString() : "0" + pc.GetMinute(LocalData).ToString();
        var second = pc.GetSecond(LocalData).ToString().Length != 1 ? pc.GetSecond(LocalData).ToString() : "0" + pc.GetSecond(LocalData).ToString();
        return new($"{year}/{Month}/{day}", $"{hour}:{min}:{second}");
    }
}