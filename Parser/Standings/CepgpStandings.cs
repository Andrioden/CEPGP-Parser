using System.Collections.Generic;

namespace CepgpParser.Parser.Standings
{
    internal class CepgpStandingsRaw
    {
        /* List of 
            * [
            *      string Nick,
            *      string Class,
            *      string Rank,
            *      int EP,
            *      int GP,
            *      decimal Ratio
            * ]
            * 
            * Example: ["Aksje","Rogue","Alt",6,70,0.08]
        */
        public List<List<object>> Roster { get; set; }

        public int Timestamp { get; set; }
    }

    public class CepgpStandings
    {
        public List<CepgpStandingsPlayer> Roster { get; set; }
        public int Timestamp { get; set; }

    }

    public class CepgpStandingsPlayer
    {
        public string Nick { get; set; }
        public string Class { get; set; }
        public string Rank { get; set; }
        public int EP { get; set; }
        public int GP { get; set; }
        public decimal Ratio { get; set; }
    }
}
