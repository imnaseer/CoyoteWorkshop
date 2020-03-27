
# Coyote Workshop - Exercise 2

The second exercise builds on top of the first one - you should complete the first exercise before starting the second exercise, and copy the code you wrote for UserController in this exercise.

The first exercise asked you to implement a User controlled through which you can create, update and delete users. The second exercise asks you to implement a gallery controller through which the user can create and delete albums and upload, retrieve and delete pictures from albums. The gallery controller does not have to create any record of albums and pictures in the database, though it should check that a request is coming from a known user registered in the database. The gallery controller should create the albums and pictures in Azure storage, using the following scheme:

* Upon a request to create an album for a user, the controller should lazily create a dedicated storage account, if it doesn't exist, for the user (it's fine to use the username as the unique name of the storage account for this exercise). It should then create a container within the storage account whose name is the album name.

* Upon a request to upload a picture to an album, the gallery control should create a blob in the container corresponding to the album, in the user's storage account

You should use the AzureStorageProvider mock class for simulating interactions with the Azure Storage service.

Your code should satisfy the following constraints:

* You cannot catch any exceptions in the Gallery controller, or User controller

* Your code should pass all the Coyote tests

* You should only create the storage account for a user upon the first CreateAlbum request

* You should delete the storage account (which transitively deletes all the albums and pictures therein), if it exists, when deleting a User

## Running Coyote Tests

You can run Coyote tests as follows, from the bin/Debug folder:

  dotnet coyote.dll test TinyService.dll --method GalleryServiceConcurrencyFuzzing -i 10 --max-steps 100 --verbose

The above command runs the GalleryServiceConcurrencyFuzzing test for 10 iterations, prints out log lines on the console and bounds the execution of each iteration to 100 steps. You can run the test for a higher number of iterations, though you might want to turn off verbosity in that mode.

  dotnet coyote.dll test TinyService.dll --method GalleryServiceConcurrencyFuzzing -i 1000 --max-steps 100

## Replaying Tests

Typically, you'll find enough log lines in the output to figure out any bugs. If you can't figure out from the console output and want to replay the buggy schedule in Visual Studio, you can do so using the following command:

  dotnet coyote.dll replay TinyService.dll Output\TinyService.exe\CoyoteOutput\<file>.schedule --method GalleryServiceConcurrencyFuzzing -b
