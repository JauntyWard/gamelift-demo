# gamelift-demo

A skeleton for building gamelift demos consisting of a client and a server which use the gamelift service. The server 
implements all of the Gamelift server callbacks and provides a restful API for the client to termiante the game session.
The client is a simple python client that simulates players joining and then leaving. The server is written in C# and has 
been tested on Windows with .Net and on OS X and Amazon Linux with Mono. To get this going, build the server and upload 
it to the gamelift service.
