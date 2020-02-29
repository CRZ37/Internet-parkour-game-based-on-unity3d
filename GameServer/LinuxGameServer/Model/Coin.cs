
namespace GameServer.Model
{
    class Coin
    {
        
        public Coin(int id, int userId, int coinNum)
        {
            Id = id;
            UserId = userId;
            CoinNum = coinNum;
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CoinNum { get; set; }
    }
}
