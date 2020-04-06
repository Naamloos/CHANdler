namespace Chandler.Data.Entities
{
    /// <summary>
    /// ApiActionError
    /// </summary>
    public class ApiActionStatus
    {
        /// <summary>
        /// Title of the message
        /// </summary>
        public string Title { get; set; } = "No Error";

        /// <summary>
        /// Http response code
        /// </summary>
        public int ResponseCode { get; set; } = 200;

        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; set; } = "No Error";
    }
}
