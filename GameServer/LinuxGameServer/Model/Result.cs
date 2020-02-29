namespace GameServer.Model
{
    class Result
    {
        public Result(int id, int userId,int totalCount,int winCount)
        {
            Id = id;
            UserId = userId;
            TotalCount = totalCount;
            WinCount = winCount;
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TotalCount { get; set; }
        public int WinCount { get; set; }
    }
}
