namespace AegisFlow.Data
{
    /// <summary>
    /// 账号数据中心。只保存账号状态，不处理登录流程。
    /// </summary>
    public sealed class AccountDC : DataCenterBase
    {
        public string AccountId { get; private set; }
        public string Token { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public void AttachLoginResult(string accountId, string token)
        {
            AccountId = accountId;
            Token = token;
            Save();
        }

        public void Clear()
        {
            AccountId = null;
            Token = null;
            Save();
        }
    }
}
