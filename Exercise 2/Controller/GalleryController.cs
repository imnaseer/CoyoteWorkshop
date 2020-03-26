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
        private Logger logger = new Logger(nameof(GalleryController));

        public async Task<ActionResult<Album>> CreateAlbum(string userName, string albumName)
        {
            logger.Write("Creating album " + userName + ", " + albumName);

            var db = new DatabaseProvider("CreateAlbum");
            var storage = new AzureStorageProvider("CreateAlbum");

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<Album>> DeleteAlbum(string userName, string albumName)
        {
            logger.Write("Deleting album " + userName + ", " + albumName);

            var db = new DatabaseProvider("DeleteAlbum");
            var storage = new AzureStorageProvider("DeleteAlbum");

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<Picture>> UploadPicture(string userName, string albumName, string pictureName, string pictureContents)
        {
            logger.Write("Uploading picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("UploadPicture");
            var storage = new AzureStorageProvider("UploadPicture");

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<Picture>> RetrievePicture(string userName, string albumName, string pictureName)
        {
            logger.Write("Retrieving picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("RetrievePicture");
            var storage = new AzureStorageProvider("RetrievePicture");

            // TODO: Implement the logic

            return null;
        }

        public async Task<ActionResult<Picture>> DeletePicture(string userName, string albumName, string pictureName)
        {
            logger.Write("Deleting picture " + userName + ", " + albumName + ", " + pictureName);

            var db = new DatabaseProvider("DeletePicture");
            var storage = new AzureStorageProvider("DeletePicture");

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
