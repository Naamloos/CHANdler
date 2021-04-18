using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Chandler.Misc
{
    public static class ToastNotificationExtensions
    {
        /// <summary>
        /// Adds an alert message to the action result
        /// </summary>
        /// <param name="result">Current ActionResult</param>
        /// <param name="body">The main text for the alert</param>
        /// <returns></returns>
        public static IActionResult WithAlert(this IActionResult result, IToastNotification toast, string body)
        {
            toast.AddAlertToastMessage(body);
            return result;
        }

        /// <summary>
        /// Adds an error message to the action result
        /// </summary>
        /// <param name="result">Current ActionResult</param>
        /// <param name="body">The main text for the alert</param>
        /// <returns></returns>
        public static IActionResult WithError(this IActionResult result, IToastNotification toast, string body)
        {
            toast.AddErrorToastMessage(body);
            return result;
        }

        /// <summary>
        /// Adds a success message to the action result
        /// </summary>
        /// <param name="result">Current ActionResult</param>
        /// <param name="body">The main text for the alert</param>
        /// <returns></returns>
        public static IActionResult WithSuccess(this IActionResult result, IToastNotification toast, string body)
        {
            toast.AddSuccessToastMessage(body);
            return result;
        }

        /// <summary>
        /// Adds a success message to the toast in the format "{0} created successfully"
        /// </summary>
        /// <param name="result"></param>
        /// <param name="toast"></param>
        /// <param name="created"></param>
        /// <returns></returns>
        public static IActionResult WithSuccessfulCreate(this IActionResult result, IToastNotification toast, string created)
        {
            toast.AddSuccessToastMessage($"{created} created successfully");
            return result;
        }

        /// <summary>
        /// Adds a success message to the toast in the format "{0} edited successfully"
        /// </summary>
        /// <param name="result"></param>
        /// <param name="toast"></param>
        /// <param name="edited"></param>
        /// <returns></returns>
        public static IActionResult WithSuccessfulEdit(this IActionResult result, IToastNotification toast, string edited)
        {
            toast.AddSuccessToastMessage($"{edited} edited successfully");
            return result;
        }

        /// <summary>
        /// Adds a success message to the toast in the format "{0} deleted successfully"
        /// </summary>
        /// <param name="result"></param>
        /// <param name="toast"></param>
        /// <param name="deleted"></param>
        /// <returns></returns>

        public static IActionResult WithSuccessfulDelete(this IActionResult result, IToastNotification toast, string deleted)
        {
            toast.AddSuccessToastMessage($"{deleted} deleted successfully");
            return result;
        }

        /// <summary>
        /// Adds an error message with the text "An internal error has occurred"
        /// </summary>
        /// <param name="result"></param>
        /// <param name="toast"></param>
        /// <returns></returns>
        public static IActionResult WithInternalError(this IActionResult result, IToastNotification toast)
        {
            toast.AddErrorToastMessage("An internal error has occurred");
            return result;
        }

        /// <summary>
        /// Adds an error message with the text "A {0} with the given Id was not found"
        /// </summary>
        /// <param name="result"></param>
        /// <param name="toast"></param>
        /// <param name="notfound"></param>
        /// <returns></returns>
        public static IActionResult WithNotFoundError(this IActionResult result, IToastNotification toast, string notfound)
        {
            toast.AddErrorToastMessage($"A {notfound} with the given Id was not found");
            return result;
        }
    }
}
