namespace CepgpParser.Core
{
    public class CepgpParserLog
    {
        public CepgpParserLogLevel Level;
        public string Message;
    }

    public enum CepgpParserLogLevel
    {
        Info,
        Warning,
        Warning_ParseIgnore,
        Error
    }
}