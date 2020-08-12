using Microsoft.Coyote.Tasks;

namespace TinyService
{
    // TODO: Implement the methods of the User Controller
    // Remember that you are not supposed to have any try/catch blocks in your code in the controller.
    public class GalleryController
    {
        // This method should create an album for a user, if the user exists, and if the album doesn't already exist.
        // The album is created as a container inside a dedicated Azure storage account for the user. If the storage account
        // doesn't exist, this method should create it lazily before creating the containe for the album.
        //
        // Note that the album information does not have to be persisted in the database.
        public async Task<ActionResult<Album>> CreateAlbum(string userName, string albumName)
        {
            var logger = new Logger(nameof(GalleryController));

            logger.Write("Creating album " + userName + ", " + albumName);

            var db = new DatabaseProvider("CreateAlbum", logger);
            var storage = new AzureStorageProvider("CreateAlbum", logger);

            // TODO: Implement the logic

            // Remember to lazily create the storage account for the user if it doesn't exist
            // Also, don't forget to delete that storage account when the user is deleted in the UserController

            return null;
        }

        // This method should delete the container corresponding to the given album, given the user exists
        // and the container corresponding to the album exists.
        //
        // Note that no album information has to be persisted or removed from the database.
        public async Task<ActionResult<Album>> DeleteAlbum(string userName, string albumName)
        {
            var logger = new Logger(nameof(GalleryController));

            logger.Write("Deleting album " + userName + ", " + albumName);

            var db = new DatabaseProvider("DeleteAlbum", logger);
            var storage = new AzureStorageProvider("DeleteAlbum", logger);

            // TODO: Implement the logic

            return null;
        }

        // This method should upload a picture with a given name in the given album, if the album exists.
        // It should not replace an existing picture.
        // 
        // Note that the picture information does not have to be persisted in the database.
        public async Task<ActionResult<Picture>> UploadPicture(string userName, string albumName, string pictureName, string pictureContents)
        {
            var logger = new Logger(nameof(GalleryController));

            logger.Write("Uploading picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("UploadPicture", logger);
            var storage = new AzureStorageProvider("UploadPicture", logger);

            // TODO: Implement the logic

            return null;
        }

        // This method should retrive the requested picture from the album, if it exists.
        // It should look up the album and picture information through Azure Storage Provider APIs.
        
        public async Task<ActionResult<Picture>> RetrievePicture(string userName, string albumName, string pictureName)
        {
            // This method has been implemented for you as a reference.
            // It teaches you how to use the various APIs and helper functions but is not
            // guaranteed to be correct.

            var logger = new Logger(nameof(GalleryController));

            logger.Write("Retrieving picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("RetrievePicture", logger);
            var storage = new AzureStorageProvider("RetrievePicture", logger);

            if (!await db.DoesDocumentExist(Constants.UserCollection, userName))
            {
                return new ActionResult<Picture>()
                {
                    Success = false
                };
            }

            var userDoc = await db.GetDocument(Constants.UserCollection, userName);

            if (!await storage.DoesContainerExist(userName, albumName))
            {
                return new ActionResult<Picture>()
                {
                    Success = false
                };
            }

            if (!await storage.DoesBlobExist(userName, albumName, pictureName))
            {
                return new ActionResult<Picture>()
                {
                    Success = false
                };
            }

            var pictureContents = await storage.GetBlobContents(userName, albumName, pictureName);

            return new ActionResult<Picture>()
            {
                Success = true,
                Response = new Picture(new Album(new User(userName, userDoc), albumName), pictureName, pictureContents)
            };
        }

        // This method should delete the given picture from the given album, if it exists.
        // 
        // Note that no picture information has to be persisted or removed from the database.
        public async Task<ActionResult<Picture>> DeletePicture(string userName, string albumName, string pictureName)
        {
            var logger = new Logger(nameof(GalleryController));

            logger.Write("Deleting picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("DeletePicture", logger);
            var storage = new AzureStorageProvider("DeletePicture", logger);

            // TODO: Implement the logic

            return null;
        }
    }

    public class Album
    {
        public User User;
        public string AlbumName;

        public Album()
        {
        }

        public Album(User user, string albumName)
        {
            this.User = user;
            this.AlbumName = albumName;
        }
    }

    public class Picture
    {
        public Album Album;
        public string PictureName;
        public string PictureContents;

        public Picture()
        {
        }

        public Picture(Album album, string pictureName, string pictureContents)
        {
            this.Album = album;
            this.PictureName = pictureName;
            this.PictureContents = pictureContents;
        }
    }
}
