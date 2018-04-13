# TBR_CL
This is a command line version of the TBR message browser found in the [TBR repository](https://github.com/rrkreitler/TBR).

This version will query the web API, allow the user to page through the list of results, and save the results to a local json file. 

It uses the following command line syntax:

TBR url startdate [/ST starttime] enddate [/ET endtime] [/F [drive:][path]filename] [/NL] [/P pages] [/V]

**url** - Url of the message archive.

**startdate** - Start date for the range of messages in the archive.

**/ST** - Start time for the range of messages in the archive.

**starttime** - Timestamp value in the form "4:35 PM"

**enddate** - End date for the range of messages in the archive.

**/ET** - End time for the range of messages in the archive.

**endtime** - Timestamp value in the form "4:35 PM"

**/F** - Downloads and saves messages.

**[dirve:][path]filename**
            - Specifies drive, directory, and name of file where downloaded messages will be saved.
            
**/NL** - No List. Messages will not be displayed. Only the summary will be shown. NOTE: This overrides /P

**/P** - Sets page size.

**pages** - Sets the number of messages to be displayed on each page.

**/V** - Verbose mode shows remote host query details.
