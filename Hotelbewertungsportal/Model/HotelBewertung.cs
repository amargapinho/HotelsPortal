using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotelbewertungsportal.Model
{
    public class HotelBewertung
    {
        public string Username { get; set; }
        public string Hotelname { get; set; }
        public DateTime Datum { get; set; }
        public string Dauer { get; set; }
        public int Bewertung { get; set; }
        public bool Empfehlung { get; set; }
    }
}
