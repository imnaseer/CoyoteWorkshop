
# Coyote Workshop - Exercise 1

This project contains a simple User controller which supports method to create new users, get user
information, delete users and update a given user's mailing and billing address. You can find the
skeleton of the controller in `Controllers\UserController.cs`. Your job is to implement the methods
of the controller, with two constraints:

* You cannot catch any exceptions in the User controller methods; your code should not include try/catch blocks

* Your code should pass all the Coyote tests

You're provided with an implementation of a `DatabaseProvider` class which is a mock implementation
of a CosmosDB like database. It allows you to create rows with a key and a bag of key/value pairs
(aka document) as the value. The mock is already aware of a "Users" collection. You should use the
mock to read/write rows in the Users collection when implementing the User controller.

## Getting familiar with the code

You've already been provided with an implementation of the UserController's `GetUser` method. Your
task is to complete the implementation of the rest of the methods and make sure all the Coyote
tests pass. The basic implementation of the methods itself isn't as interesting and shouldn't take
you a long time. The learning will happen when (or if) your tests fail due to interference between
concurrent calls and you modify your implementation to fix bugs found by Coyote.

Here are a few pointers to get you started on your first implementation of the methods:

- `DatabaseProvider` is a mock CosmosDB-like database which can have multiple collections. A
`Users` collection has already been provisioned for your convenience. You should write code
assuming it exists.

- Each row in the database collection contains a key and an associated `Document`.

- In the exercise, the `Document` is just a type alias for `Dictionary<string, string>` which
models a collection of key/value pairs.

- The controller methods return `ActionResult<T>`. ActionResult objects contain a true/false
`Success` value indicating whether the call succeeded and a `Value` object (of type `T`) which
should be populated only if the call successfully completed and should be null otherwise.
`ActionResult` models controllers returning various error codes (200, 400) etc. We kept things
simple in the exercise by just returning a true/false response.

- Controller methods often return `ActionResult<User>` where `User` is a model object returned to
the caller. It's important to note that `User` objects are not directly stored in the database -
the database stores `Document` objects instead. You'll often need to convert `Document` objects to
`User` objects and the `User` object has a convenient constructor which can use for that. You
should not need to convert `User` objects to `Document` objects in the exercise, only the reverse
mapping.

- Your controller code should _not_ catch any exceptions - Coyote will not be able to find
interference bugs in your code if you catch them in the controller. You should write your code in a
way which leads to zero exceptions in the presence of concurrency. You can argue that real
production code doesn't work that way. This is a simplified setting however in which we are using
uncaught exceptions as a proxy for safety bugs. Production code catches and classifies exceptions
but the workshop code keeps things simple by forbidding catching of any exceptions.

## Running Coyote Tests

You can run Coyote tests as follows, from the bin/Debug/netcoreapp2.2 folder:

  dotnet coyote.dll test TinyService.dll --method UserControllerConcurrencyFuzzing -i 10 --max-steps 100 --verbose

The above command runs the UserControllerConcurrencyFuzzing test for 10 iterations, prints out log
lines on the console and bounds the execution of each iteration to 100 steps.You can run the test
for a higher number of iterations, though you might want to turn off verbosity in that mode.

  dotnet coyote.dll test TinyService.dll --method UserControllerConcurrencyFuzzing -i 1000 --max-steps 100

## Replaying Tests

Typically, you'll find enough log lines in the output to figure out any bugs. If you can't figure
out from the console output and want to replay the buggy schedule in Visual Studio, you can do so
using the following command:

  dotnet coyote.dll replay TinyService.dll Output\TinyService.dll\CoyoteOutput\<file>.schedule --method UserControllerConcurrencyFuzzing -b
