namespace TournamentSoftware
{
    internal class Participant
    {
        public Participant()
        {
        }

        public string Nominations { get; internal set; }
        public int ClubRating { get; internal set; }
        public int CommonRating { get; internal set; }
        public int Weight { get; internal set; }
        public int Height { get; internal set; }
        public string City { get; internal set; }
        public string Club { get; internal set; }
        public bool Posevnoy { get; internal set; }
        public string Sex { get; internal set; }
        public int DateOfBirth { get; internal set; }
        public string Psevdonim { get; internal set; }
        public string Otcestvo { get; internal set; }
        public string Surname { get; internal set; }
        public string Name { get; internal set; }
        public int Id { get; internal set; }
    }
}