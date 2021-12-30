using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CIS341_project_cebula
{
    /// <summary>
    /// User Helper class
    /// </summary>
    public static class UserHelper
    {
        /// <summary>
        /// Check if a user is authenticated
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>boolean</returns>
        public static bool IsAuthenticated(this User user)
        {
            if (user == null)
                return false;
            return true;
        }
        /// <summary>
        /// Check if user is an author of generic content
        /// </summary>
        /// <typeparam name="T">generic content</typeparam>
        /// <param name="user">user object</param>
        /// <param name="content">generic content object</param>
        /// <returns>boolean</returns>
        public static bool IsAuthorized<T>(this User user, T content)
        {
            //Use reflection to get the generic contents UserAccountId property and compare it to the user objects property
            if ((int)content.GetType().GetProperty(Constants.Reflection.ContentProperty.UserAccountId).GetValue(content) == user.UserAccountId)
                return true;
            return false;
        }
    }
}
