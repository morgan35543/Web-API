using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DistSysACW.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 

        public User() { }

        [Key]
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }

        #endregion
    }
      
    /// <summary>
    /// Change role JSON string to User object
    /// </summary>
    public class UserJSON 
    {
        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("role")]
        public string role { get; set; }
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    public class UserDatabaseAccess
    {
        // TODO: Make methods which allow us to read from/write to the database 
        public static User createUser(string username) // Write
        {
            
                using (var context = new UserContext())
                {
                    User newUser = new User();
                    if (context.Users.Count() == 0)
                        newUser.Role = "Admin";
                    else
                        newUser.Role = "User";

                    Guid guid = Guid.NewGuid();
                    string apikey = guid.ToString();

                    while (keyExists(apikey)) // Must be unique
                    {
                        guid = Guid.NewGuid();
                        apikey = guid.ToString();
                    }

                    newUser.ApiKey = apikey;
                    newUser.UserName = char.ToUpper(username[0]) + username.Substring(1).ToLower();

                    context.Users.Add(newUser);
                    context.SaveChangesAsync();
                    return newUser;
                }
            
        }

        public static bool deleteByKey(string apikey) // Write
        {
            try
            {
                using (var context = new UserContext())
                {
                    User user = new User() { ApiKey = apikey };
                    context.Attach(user);
                    context.Remove(user);
                    context.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }

        public static bool keyExists(string apikey) // Read
        {
            try
            {
                using (var context = new UserContext())
                {
                    if (context.Users.Any(a => a.ApiKey == apikey))
                        return true;
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static User userByKey(string apikey) // Read
        {
            try
            {
                using (var context = new UserContext())
                {
                    var user = context.Users.First(a => a.ApiKey == apikey);
                    context.Attach(user);
                    return user;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool userExists(string username, string apikey) // Read
        {
            try
            {
                using (var context = new UserContext())
                {
                    if (context.Users.Any(o => o.UserName.ToLower() == username.ToLower() && o.ApiKey == apikey))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool usernameExists(string username) // Read
        {
            try
            {
                using (var context = new UserContext())
                {
                    if (context.Users.Any(o => o.UserName.ToLower() == username.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static User userByName(string username) // Read
        {
            try
            {
                using (var context = new UserContext())
                {
                    var user = new User { UserName = username };
                    context.Attach(user);
                    return user;
                }
            }
            catch
            {
                return null;
            }
        }
        
        // Changerole
        public static bool changeRole(string newRole, string username)
        {
            try
            {
                using (var context = new UserContext())
                {
                    newRole = char.ToUpper(newRole[0]) + newRole.Substring(1).ToLower();
                    User user = context.Users.SingleOrDefault(o => o.UserName.ToLower() == username.ToLower());
                    user.Role = newRole;
                    context.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        
    }


}