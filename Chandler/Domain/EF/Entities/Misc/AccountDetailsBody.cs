namespace Domain.EF.Entities.Misc
{
    /// <summary>
    /// Account details body object
    /// </summary>
    public class AccountDetailsBody
    {
        /// <summary>
        /// Account username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Account email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Account password
        /// </summary>
        public string Password { get; set; }
    }
}
