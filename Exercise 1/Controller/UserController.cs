using System.Collections.Concurrent;
using Microsoft.Coyote.Tasks;

namespace TinyService
{
    using Document = ConcurrentDictionary<string, string>;

    // TODO: Implement the methods of the User Controller
    public class UserController
    {
        public async Task<ActionResult<User>> CreateUser(
            string userName,
            string email,
            string phoneNumber,
            string mailingAddress,
            string billingAddress)
        {
            var logger = new Logger(nameof(UserController));

            logger.Write($"Creating user {userName}, {email}, {phoneNumber}, {mailingAddress}, {billingAddress}");

            var db = new DatabaseProvider("CreateUser", logger);

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<User>> GetUser(string userName)
        {
            var logger = new Logger(nameof(UserController));

            logger.Write("Get user " + userName);

            var db = new DatabaseProvider("GetUser", logger);

            if (!await db.DoesDocumentExist(Constants.UserCollection, userName))
            {
                return new ActionResult<User>() { Success = false };
            }

            var userDoc = await db.GetDocument(Constants.UserCollection, userName);

            return new ActionResult<User>()
            {
                Success = true,
                Response = new User(userName, userDoc)
            };
        }

        public async Task<ActionResult<Address>> UpdateUserAddress(string userName, string mailingAddress, string billingAddress)
        {
            var logger = new Logger(nameof(UserController));

            logger.Write($"Updating user address {userName} {mailingAddress} {billingAddress}");

            var db = new DatabaseProvider("UpdateUserAddress", logger);

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<User>> DeleteUser(string userName)
        {
            var logger = new Logger(nameof(UserController));

            logger.Write("Deleting user " + userName);

            var db = new DatabaseProvider("DeleteUser", logger);

            // TODO: Implement the logic

            return null;
        }
    }

    public class User
    {
        public string UserName;
        public string EmailAddress;
        public string PhoneNumber;
        public Address Address;

        public User()
        {
        }

        public User(string userName, Document doc)
        {
            this.UserName = userName;
            this.EmailAddress = doc[Constants.EmailAddress];
            this.PhoneNumber = doc[Constants.PhoneNumber];
            this.Address = new Address(doc[Constants.MailingAddress], doc[Constants.BillingAddress]);
        }

        public User(
            string userName,
            string emailAddress,
            string phoneNumber,
            string mailingAddress,
            string billingAddress)
        {
            this.UserName = userName;
            this.EmailAddress = emailAddress;
            this.PhoneNumber = phoneNumber;
            this.Address = new Address(mailingAddress, billingAddress);
        }
    }

    public class Address
    {
        public string MailingAddress;
        public string BillingAddress;

        public Address(string mailingAddress, string billingAddress)
        {
            this.MailingAddress = mailingAddress;
            this.BillingAddress = billingAddress;
        }
    }
}
