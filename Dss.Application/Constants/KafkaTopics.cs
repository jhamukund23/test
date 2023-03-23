namespace Dss.Application.Constants
{
    /// <summary>
    /// Represents the list of topics in Kafka
    /// </summary>
    public static class KafkaTopics
    {
        public static string RegisterUser => "RegisterUser";
        public static string UserRegistered => "UserRegistered";

        public static string SendNewFileToIDM => "SendNewFileToIDM";
        public static string SASUrlResponse => "SASUrlResponse";
        public static string BlobUploadCompletedEvent => "BlobUploadCompletedEvent";
        public static string AddDocument => "AddDocument";
        public static string AddDocumentResponse => "AddDocumentResponse";
    }
}
