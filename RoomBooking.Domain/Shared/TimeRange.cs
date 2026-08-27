namespace RoomBooking.Domain.Shared
{
    public sealed record TimeRange
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public TimeSpan Duration
        {
            get
            {
                return End - Start;
            }
        }


        public TimeRange(DateTime start, DateTime end)
        {
            if (end <= start)
            {
                throw new ArgumentException("Range end must be later than its start.", nameof(end));
            }
                
            Start = start;
            End = end;
        }


        public bool Contains(DateTime moment)
        {
            return moment >= Start && moment < End;
        }

    }
}
