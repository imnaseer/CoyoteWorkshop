using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Coyote.Tasks;

namespace TinyService
{
    // TODO: Implement the methods of the User Controller
    public class GalleryController
    {
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

        public async Task<ActionResult<Album>> DeleteAlbum(string userName, string albumName)
        {
            var logger = new Logger(nameof(GalleryController));

            logger.Write("Deleting album " + userName + ", " + albumName);

            var db = new DatabaseProvider("DeleteAlbum", logger);
            var storage = new AzureStorageProvider("DeleteAlbum", logger);

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<Picture>> UploadPicture(string userName, string albumName, string pictureName, string pictureContents)
        {
            var logger = new Logger(nameof(GalleryController));

            logger.Write("Uploading picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("UploadPicture", logger);
            var storage = new AzureStorageProvider("UploadPicture", logger);

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<Picture>> RetrievePicture(string userName, string albumName, string pictureName)
        {
            var logger = new Logger(nameof(GalleryController));

            logger.Write("Retrieving picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("RetrievePicture", logger);
            var storage = new AzureStorageProvider("RetrievePicture", logger);

            // TODO: Implement the logic

            return null;
        }

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
