namespace QouToPOWebApp.Models.MiscModels
{
    public class EncryptionSetting
    {
        public string SecretKey { get; set; } = "";
        public string InitVector { get; set; } = "";
    }
}
