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
        public static async Task GalleryServiceConcurrencyFuzzing(ICoyoteRuntime runtime)
        {
            const int transacriptLength = 9;

            var userController = new UserController();
            var galleryController = new GalleryController();

            var userName = "bob";
            var emailAddress = "bob@abc.com";
            var phoneNumber = "425-123-1234";
            var mailingAddress = "101 100th Ave NE Redmond, WA";
            var billingAddress = "101 100th Ave NE Redmond, WA";
            var albumName = "Holiday";
            var pictureName = "Beach.jpg";
            var pictureContent = "923bac12";

            var userTasks = new List<Task<ActionResult<User>>>();
            var addressTasks = new List<Task<ActionResult<Address>>>();
            var albumTasks = new List<Task<ActionResult<Album>>>();
            var pictureTasks = new List<Task<ActionResult<Picture>>>();

            for (int i = 0; i < transacriptLength; i++)
            {
                var choice = runtime.RandomInteger(transacriptLength);

                switch (choice)
                {
                    case 0:
                        userTasks.Add(userController.CreateUser(userName, emailAddress, phoneNumber, mailingAddress, billingAddress));
                        break;
                    case 1:
                        addressTasks.Add(userController.UpdateUserAddress(userName, mailingAddress, billingAddress));
                        break;
                    case 2:
                        userTasks.Add(userController.DeleteUser(userName));
                        break;
                    case 3:
                        userTasks.Add(userController.GetUser(userName));
                        break;
                    case 4:
                        albumTasks.Add(galleryController.CreateAlbum(userName, albumName));
                        break;
                    case 5:
                        albumTasks.Add(galleryController.DeleteAlbum(userName, albumName));
                        break;
                    case 6:
                        pictureTasks.Add(galleryController.UploadPicture(userName, albumName, pictureName, pictureContent));
                        break;
                    case 7:
                        pictureTasks.Add(galleryController.DeletePicture(userName, albumName, pictureName));
                        break;
                    case 8:
                        pictureTasks.Add(galleryController.RetrievePicture(userName, albumName, pictureName));
                        break;
                }
            }

            var tasks = new List<Task>();
            tasks.AddRange(userTasks);
            tasks.AddRange(addressTasks);
            tasks.AddRange(albumTasks);
            tasks.AddRange(pictureTasks);
            await Task.WhenAll(tasks.ToArray());

            userTasks.ForEach(t => CheckUserTaskResult(t));
            addressTasks.ForEach(t => CheckAddressTaskResult(t));
            albumTasks.ForEach(t => CheckAlbumTaskResult(t));
            pictureTasks.ForEach(t => CheckPictureTaskResult(t));
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

        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task DeleteWithConcurrentAPIs(ICoyoteRuntime runtime)
        {
            var userName = "user1";
            var emailAddress = "bob@abc.com";
            var phoneNumber = "425-123-1234";
            var mailingAddress = "101 100th Ave NE Redmond WA";
            var billingAddress = "101 100th Ave NE Redmond WA";
            var albumName = "myAlbum";
            var pictureName = "pic.jpg";
            var pictureContents = "0x2321";

            var userController = new UserController();
            var galleryController = new GalleryController();

            var logger = new Logger("DeleteWithConcurrentAPIs");
            var db = new DatabaseProvider("Test", logger);
            var storage = new AzureStorageProvider("Test", logger);

            var createResult = await userController.CreateUser(userName, emailAddress, phoneNumber, mailingAddress, billingAddress);
            Assert(createResult.Success);
            Assert(createResult.Response.UserName == userName && createResult.Response.EmailAddress == emailAddress);
            var doc = await db.GetDocument(Constants.UserCollection, userName);
            Assert(doc[Constants.EmailAddress] == emailAddress);

            var tasks = new List<Task>();

            // We start the delete operation, along with a number of concurrenct APIs in the background
            // which should not interfere with the invariant we check at the end of the delete operation
            var deleteResultTask = userController.DeleteUser(userName);
            tasks.Add(deleteResultTask);

            const int transcriptLength = 7;
            for (int i = 0; i < transcriptLength; i++)
            {
                var choice = runtime.RandomInteger(transcriptLength);

                switch (choice)
                {
                    case 0:
                        tasks.Add(userController.UpdateUserAddress(userName, mailingAddress, billingAddress));
                        break;
                    case 1:
                        tasks.Add(userController.GetUser(userName));
                        break;
                    case 2:
                        tasks.Add(galleryController.CreateAlbum(userName, albumName));
                        break;
                    case 3:
                        tasks.Add(galleryController.DeleteAlbum(userName, albumName));
                        break;
                    case 4:
                        tasks.Add(galleryController.UploadPicture(userName, albumName, pictureName, pictureContents));
                        break;
                    case 5:
                        tasks.Add(galleryController.RetrievePicture(userName, albumName, pictureName));
                        break;
                    case 6:
                        tasks.Add(galleryController.DeletePicture(userName, albumName, pictureName));
                        break;
                }
            }

            Task.WaitAll(tasks.ToArray());

            var deleteResult = deleteResultTask.Result;

            Assert(deleteResult.Success);
            Assert(!await db.DoesDocumentExist(Constants.UserCollection, userName));
            Assert(!await storage.DoesAccountExits(doc[Constants.UniqueId]));
        }

        [Microsoft.Coyote.SystematicTesting.TestIterationDispose]
        public static void IterationCleanup()
        {
            DatabaseProvider.Cleanup();
            AzureStorageProvider.Cleanup();
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

        private static void CheckAlbum(Album album)
        {
            Assert(album != null);
            Assert(album.AlbumName != null);
            CheckUser(album.User);
        }

        private static void CheckPicture(Picture picture)
        {
            Assert(picture != null);
            Assert(picture.PictureName != null);
            Assert(picture.PictureContents != null);
            CheckAlbum(picture.Album);
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

        private static void CheckAlbumTaskResult(Task<ActionResult<Album>> albumTask)
        {
            if (albumTask.Result.Success)
            {
                CheckAlbum(albumTask.Result.Response);
            }
            else
            {
                Assert(albumTask.Result.Response == null);
            }
        }

        private static void CheckPictureTaskResult(Task<ActionResult<Picture>> pictureTask)
        {
            if (pictureTask.Result.Success)
            {
                CheckPicture(pictureTask.Result.Response);
            }
            else
            {
                Assert(pictureTask.Result.Response == null);
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
