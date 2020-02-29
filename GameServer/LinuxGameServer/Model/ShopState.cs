namespace GameServer.Model
{
    class ShopState
    {
        public ShopState(int id, int userId,int healthTime, int bigHealthTime,int skillTimeTime,int bigSkillTimeTime)
        {
            Id = id;
            UserId = userId;
            HealthTime = healthTime;
            BigHealthTime = bigHealthTime;
            SkillTimeTime = skillTimeTime;
            BigSkillTimeTime = bigSkillTimeTime;
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HealthTime { get; set; }
        public int BigHealthTime { get; set; }
        public int SkillTimeTime { get; set; }
        public int BigSkillTimeTime { get; set; }
    }
}
