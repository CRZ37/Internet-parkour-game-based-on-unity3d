
namespace GameServer.Model
{
    class PlayerState
    {
        public PlayerState(int id, int userId, int health, float skillTime)
        {
            Id = id;
            UserId = userId;
            Health = health;
            SkillTime = skillTime;
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Health { get; set; }
        public float SkillTime { get; set; }
    }
}
