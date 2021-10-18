namespace Hotelbewertungsportal.Model
{
    public class Hotel
    {
        public int HotelId { get; set; }
        public string Hotelname { get; set; }
        public string Adresse { get; set; }
        public string Email { get; set; }
        public int Preis { get; set; }
        public int GesamtBewertungen { get; set; }
        public int AnzahlBewertungen { get; set; }
        public double Durchschnitt { get; set; }
    }
}
