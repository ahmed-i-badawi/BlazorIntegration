using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtApp.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel() { Username = "jason_admin", EmailAddress = "jason.admin@email.com", Password = "MyPass_w0rd", GivenName = "Jason", Surname = "Bryant", Role = "POS" },
            new UserModel() { Username = "jason_admin2222", EmailAddress = "jason.admin22@email.com", Password = "MyPass_w0rd", GivenName = "Jason2", Surname = "Bryant2", Role = "POS" },
            new UserModel() { Username = "elyse_seller", EmailAddress = "elyse.seller@email.com", Password = "MyPass_w0rd", GivenName = "Elyse", Surname = "Lambert", Role = "Customer" },
        };

        public static List<string> Serials = new List<string>()
        {
            "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",
        };

    }
}
