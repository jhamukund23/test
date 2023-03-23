namespace Dss.Domain.UserRegistration
{
    /// <summary>
    ///  Represents the message object in UserRegistered Topic
    /// </summary>
    public class UserRegistered
    {
        /// <summary>
        /// Indicates UserId of the registered User
        /// </summary>
        public int UserId { get; set; }
    }
}
