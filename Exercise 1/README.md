
# Coyote Workshop - Exercise 1

This project contains a simple User controller which supports method to create new users, get user information, delete users and update a given user's mailing and billing address. You can find the skeleton of the controller in `Controllers\UserController.cs`. Your job is to implement the methods of the controller, with two constraints:

* You cannot catch any exceptions in the User controller methods; your code should not include try/catch blocks

* Your code should pass all the Coyote tests

You're provided with an implementation of a `DatabaseProvider` class which is a mock implementation of a CosmosDB like database. It allows you to create rows with a key and a bag of key/value pairs (aka document) as the value. The mock is already aware of a "Users" collection. You should use the mock to read/write rows in the Users collection when implementing the User controller.

## Running Coyote Tests

You can run Coyote tests as follows, from the bin/Debug folder:

  dotnet coyote.dll test TinyService.dll --method UserControllerConcurrencyFuzzing -i 10 --max-steps 100 --verbose

The above command runs the UserControllerConcurrencyFuzzing test for 10 iterations, prints out log lines on the console and bounds the execution of each iteration to 100 steps. You can run the test for a higher number of iterations, though you might want to turn off verbosity in that mode.

  dotnet coyote.dll test TinyService.dll --method UserControllerConcurrencyFuzzing -i 1000 --max-steps 100

## Replaying Tests

Typically, you'll find enough log lines in the output to figure out any bugs. If you can't figure out from the console output and want to replay the buggy schedule in Visual Studio, you can do so using the following command:

  dotnet coyote.dll replay TinyService.dll Output\TinyService.dll\CoyoteOutput\<file>.schedule --method UserControllerConcurrencyFuzzing -b
