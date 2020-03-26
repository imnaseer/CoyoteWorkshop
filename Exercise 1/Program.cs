using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Coyote.Specifications;
using Microsoft.Coyote.Runtime;
using Microsoft.Coyote.Tasks;

namespace TinyService
{
    public class Program
    {
        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task UserControllerConcurrencyFuzzing(ICoyoteRuntime runtime)
        {
            const int transacriptLength = 4;

            var controller = new UserController();

            var userName = "bob";
            var emailAddress = "bob@abc.com";
            var phoneNumber = "425-123-1234";
            var mailingAddress = "101 100th Ave NE Redmond, WA";
            var billingAddress = "101 100th Ave NE Redmond, WA";

            var userTasks = new List<Task<ActionResult<User>>>();
            var addressTasks = new List<Task<ActionResult<Address>>>();

            for (int i = 0; i < transacriptLength; i++)
            {
                var choice = runtime.RandomInteger(transacriptLength);

                switch (choice)
                {
                    case 0:
                        userTasks.Add(controller.CreateUser(userName, emailAddress, phoneNumber, mailingAddress, billingAddress));
                        break;
                    case 1:
                        addressTasks.Add(controller.UpdateUserAddress(userName, mailingAddress, billingAddress));
                        break;
                    case 2:
                        userTasks.Add(controller.DeleteUser(userName));
                        break;
                    case 3:
                        userTasks.Add(controller.GetUser(userName));
                        break;
                }
            }

            var tasks = new List<Task>();
            tasks.AddRange(userTasks);
            tasks.AddRange(addressTasks);
            await Task.WhenAll(tasks);

            userTasks.ForEach(t => CheckUserTaskResult(t));
            addressTasks.ForEach(t => CheckAddressTaskResult(t));
        }

        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task ConcurrentUserCreates(ICoyoteRuntime runtime)
        {
            var userController = new UserController();

            var info = new Dictionary<string, User>();
            info["1"] = new User("user1", "bob@abc.com", "425-123-1234", "Address 1", "Address 1");
            info["2"] = new User("user1", "bob@xyz.com", "425-987-9876", "Address 2", "Address 2");

            var result = new Dictionary<string, Task<ActionResult<User>>>();
            result["1"] = userController.CreateUser(
                info["1"].UserName,
                info["1"].EmailAddress,
                info["1"].PhoneNumber,
                info["1"].Address.MailingAddress,
                info["1"].Address.BillingAddress);
            result["2"] = userController.CreateUser(
                info["2"].UserName,
                info["2"].EmailAddress,
                info["2"].PhoneNumber,
                info["2"].Address.MailingAddress,
                info["2"].Address.BillingAddress);

            Task.WaitAll(result.Values.ToArray());

            Assert(result["1"].Result.Success ^ result["2"].Result.Success);
            var successfulIndex = result["1"].Result.Success ? "1" : "2";

            var user = (await userController.GetUser(result[successfulIndex].Result.Response.UserName)).Response;
            Assert(user.EmailAddress == info[successfulIndex].EmailAddress);
            Assert(user.PhoneNumber == info[successfulIndex].PhoneNumber);
            Assert(user.Address.MailingAddress == info[successfulIndex].Address.MailingAddress);
            Assert(user.Address.BillingAddress == info[successfulIndex].Address.BillingAddress);
        }

        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task ConcurrentUserUpdates(ICoyoteRuntime runtime)
        {
            var userName = "user1";
            var emailAddress = "bob@abc.com";
            var phoneNumber = "425-123-1234";
            var mailingAddress = "101 100th Ave NE Redmond WA";
            var billingAddress = "101 100th Ave NE Redmond WA";

            var userController = new UserController();
            var createResult = await userController.CreateUser(userName, emailAddress, phoneNumber, mailingAddress, billingAddress);
            Assert(createResult.Success);

            var mailingAddressUpdate1 = "132 102nd Ave SE Seattle, WA";
            var billingAddressUpdate1 = "132 102nd Ave SE Seattle, WA";

            var mailingAddressUpdate2 = "126 105th Ave NE Los Angeles, CA";
            var billingAddressUpdate2 = "126 105th Ave NE Los Angeles, CA";

            var updateTask1 = userController.UpdateUserAddress(userName, mailingAddressUpdate1, billingAddressUpdate1);
            var updateTask2 = userController.UpdateUserAddress(userName, mailingAddressUpdate2, billingAddressUpdate2);

            Task.WaitAll(updateTask1, updateTask2);

            Assert(updateTask1.Result.Success && updateTask2.Result.Success);

            var user = await userController.GetUser(userName);
            Assert((user.Response.Address.MailingAddress == mailingAddressUpdate1 &&
                    user.Response.Address.BillingAddress == billingAddressUpdate1) ||
                   (user.Response.Address.MailingAddress == mailingAddressUpdate2 &&
                    user.Response.Address.BillingAddress == billingAddressUpdate2));
        }

        [Microsoft.Coyote.SystematicTesting.TestIterationDispose]
        public static void IterationCleanup()
        {
            DatabaseProvider.Cleanup();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Run the tests using coyote.exe");
        }

        private static void CheckUser(User user)
        {
            Assert(user != null);
            Assert(user.UserName != null);
            Assert(user.EmailAddress != null);
            Assert(user.PhoneNumber != null);
            CheckAddress(user.Address);
        }

        private static void CheckAddress(Address address)
        {
            Assert(address != null);
            Assert(address.MailingAddress != null);
            Assert(address.BillingAddress != null);
        }

        private static void CheckUserTaskResult(Task<ActionResult<User>> userTask)
        {
            if (userTask.Result.Success)
            {
                CheckUser(userTask.Result.Response);
            }
            else
            {
                Assert(userTask.Result.Response == null);
            }
        }

        private static void CheckAddressTaskResult(Task<ActionResult<Address>> addressTask)
        {
            if (addressTask.Result.Success)
            {
                CheckAddress(addressTask.Result.Response);
            }
            else
            {
                Assert(addressTask.Result.Response == null);
            }
        }

        private static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("Assert failed");
            }
        }
    }
}
