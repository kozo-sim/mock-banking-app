
WDT Assignment 3

Kozo Simutanyi
s3750253

-----

Angular component is unfortunately missing, as I couldn't get it to post to the API in my existing project structure (or something, idk).
Sadly I don't want to lose any sleep over this as it's only 8% of the mark, and I already have a pass.

-----

The API can be tested with the URLs/methods present in the file;
MiBank_A3/Controllers/API/AdminApiController.cs

Most importantly, login is done by posting to;
https://localhost:port/api/login?username=admin&password=admin

A BillPay record will have to be created manually through the assignment 2 UI so that the relevant section can be tested.

GET /api/bills gives a mysterious 404. After debugging the endpoint step-by-step from line 76 of AdminApiController, the return value looks fine, but there appears to be an infinite loop between some navigation properties or something.
All other endpoints should work.

-----

