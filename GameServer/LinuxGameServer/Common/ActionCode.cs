namespace GameServer.Common
{
    /// <summary>
    /// 指定调用的方法
    /// </summary>
    public enum ActionCode
    {
        None,
        Login,
        Logout,
        Register,
        ListRoom,
        CreateRoom,
        JoinRoom,
        UpdateRoom,
        QuitRoom,
        StartGame,
        ShowTimer,
        CreateRoadAndItem,
        TakeDamage,
        GetCoin,
        GetItem,
        StartPlay,
        LocalMove,
        RemoteMove,
        GameOver,
        UpdateResult,
        QuitBattle,
        BuyItem,
        UpdateCoin,
        UpdateShop,
        SelectRoleItem,
        BuyRoleItem,
        UpdateRoleShop,
        UpdatePlayerState
    }
}