using System.Collections.Concurrent;
using Microsoft.Coyote.Tasks;

namespace TinyService
{
    using Document = ConcurrentDictionary<string, string>;

    // TODO: Implement the methods of the User Controller
    // Remember that you are not supposed to have any try/catch blocks in your code in the controller.
    public class UserController
    {
        // This method should create a user in the database if one doesn't already exist.
        // If one doesn't exist, it should return an ActionResult<User> { Success = true, Response = user-object }
        // If one already exists, it should return an ActionResult<User> { Success = false, Response = null }
        // Note that you'll persist a Document object (which is just a dictionary of key/value pairs) for storing the user information 
        // in the database. You can utilize a convenient User object constructor to convert the Document to the User object which has
        // to be returned to the caller.
        // 
        // Remember that you are not supposed to have any try/catch blocks in your code.
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

        
        // This method returns a user, if the user exists.
        // If the user exists, it should return ActionResult<User> { Success = true, Response = user-object }
        // If the user doesn't exist, it should return ActionResult<User> { Success = false, Response = null }
        // 
        // Remember that you are not supposed to have any try/catch blocks in your code.
        public async Task<ActionResult<User>> GetUser(string userName)
        {
            // This method has been implemented for you as a reference.
            // It teaches you how to use the various APIs and helper functions but is not
            // guaranteed to be correct.

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

        // This method should update the mailing and billing address of the given user, if the user exists.
        // If the user exists, the addresses should be updated and ActionResult<User> { Success = true, Response = update-user-object} should
        // be returned.
        // If the user doesn't exist, this method should return ActionResult<User> { Success = false, Response = null }
        //
        // Remember that you are not supposed to have any try/catch blocks in your code.
        public async Task<ActionResult<Address>> UpdateUserAddress(string userName, string mailingAddress, string billingAddress)
        {
            var logger = new Logger(nameof(UserController));

            logger.Write($"Updating user address {userName} {mailingAddress} {billingAddress}");

            var db = new DatabaseProvider("UpdateUserAddress", logger);

            // TODO: Implement the logic

            return null;
        }

        // This method should delete the user, if the user exists.
        // If the user exists, it should delete the user and return ActionResult<User> { Success = true, Response = user-object }
        // If teh user doesn't exist, it should return ActionResult<User> { Success = false, Response = null }
        //
        // Remember that you are not supposed to have any try/catch blocks in your code.
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
