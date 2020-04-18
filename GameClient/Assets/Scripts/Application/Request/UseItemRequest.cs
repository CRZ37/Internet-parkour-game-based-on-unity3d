using Common;
public class UseItemRequest : BaseRequest
{
    private RemotePlayerMove remotePlayerMove;
    public void SetRemotePlayerMove(RemotePlayerMove remotePlayerMove)
    {
        this.remotePlayerMove = remotePlayerMove;
    }
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.UseItem;
        base.Awake();
    }
    public void SendUseItemRequest(ItemType itemType)
    {
        SendRequest(((int)itemType).ToString());
    }
    public override void OnResponse(string data)
    {
        ItemType itemType = (ItemType)int.Parse(data);
        remotePlayerMove.UseItemSync(itemType);
    }
}
