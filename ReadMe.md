Web Request Reflector
---------------------

A simple tool to test systems that make requests to other servers without expecting a specific response (e.g. Web Hooks).
WebRequestReflector is a
simple Web API project, which can be hosted using OWIN.

## Self Hosting

To self-host the project (for use with unit tests and such),
WebRequestReflector.SelfHost provides OWIN self hosting (remember to reference
Microsoft.OWIN.Hosting).