

    /// <summary>
    /// 用来保存协议中定义的常量
    /// </summary>
    public class ProConstant
    {

        /// <summary>
        ///  呢称、用户名、真实姓名 实际保存的数组长度
        /// </summary>
        public const int NAME_LENGTH = 22;
        /// <summary>
        ///  限制名称的长度为 [6-20]
        /// </summary>
        public const int NAME_LENGTH_MAX = 20;
        public const int NAME_LENGTH_MIN = 6;
        /// <summary>
        /// MD5 加密过后的用户密码长度
        /// </summary>
        public const int PASSWORD_MD5_LENGTH = 64;
        /// <summary>
        /// 原始密码数组保存长度
        /// </summary>
        public const int PASSWORD_RAW_LENGTH = 22;
        /// <summary>
        /// 原始密码长度[6-20]
        /// </summary>
        public const int PASSWORD_RAW_LENGTH_MAX = 20;
        public const int PASSWORD_RAW_LENGTH_MIN = 6;

        /// <summary>
        /// 手机号码长度
        /// </summary>
        public const int PHONE_NUMBER_LENGTH = 15;

        /// <summary>
        /// 手机验证码长度
        /// </summary>
        public const int PHONE_VCODE_LENGTH = 36;
        /// <summary>
        /// 身份证号码长度
        /// </summary>
        public const int ID_CARD_NUMBER_LENGTH = 19;
        /// <summary>
        /// MD5后的身份证号码长度
        /// </summary>
        public const int ID_CARD_NUMBER_MD5_LENGTH = 64;
        /// <summary>
        /// 军衔名称
        /// </summary>
        public const int RANKNAME_LENGHT = 22;
        /// <summary>
        /// 添加好友时请求内容的最大长度
        /// </summary>
        public const int ADDFRIEND_CONTENT_LEN = 100;
        /// <summary>
        /// 聊天内容大小
        /// </summary>
        public const int TALK_CONTENT_LEN = 200;
        /// <summary>
        /// 商会公告内容最大长度
        /// </summary>
        public const int SOCIAL_NOTICE_LEN = 120;
        /// <summary>
        /// 游戏名称长度
        /// </summary>
        public const int GAMENAME_LEN = 20;
        /// <summary>
        /// 游戏房间长度
        /// </summary>
        public const int GAMEROOM_LEN = 30;
        /// <summary>
        ///机器设备ID字符长度
        /// </summary>
        public const int MAC_LEN = 64;



        //手机短信验证码获取
        public const int MDM_GP_SMS = 117;
        public const int ASS_GP_SMS_VCODE = 1;

        public const int MAX_PEOPLE = 180;

        /// <summary>
        /// 最大聊天数据长度
        /// </summary>
        public const int MAX_TALK_LEN = 500;
        /// <summary>
        /// 普通聊天数据长度
        /// </summary>
        public const int NORMAL_TALK_LEN = 200;
}
