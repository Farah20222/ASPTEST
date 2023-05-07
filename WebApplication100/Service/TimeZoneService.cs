using ExpressTimezone;

namespace WebApplication100.Service
{
    public class TimeZoneService: ITimeZoneService
    {
        public DateTime ChangeTimeZoneToRegional(DateTime dateTime)
        {
            return dateTime.UTCToRegionalTime("Asia/Bahrain");
        }
    }
}
