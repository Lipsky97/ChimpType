# ChimpType
![Home site screenshot](https://github.com/user-attachments/assets/512d5f4c-b07f-4c58-a4de-98c5f3ee5f43)
## Link
You can visit this website [here](https://chimptype.runasp.net/)
## Description
The idea for this website was inspired by building my first custom mechanical keyboard. Around that time, I spent a lot of hours using Monkeytype and really admired how smooth and well-designed it was. Since I didn’t have many public projects to showcase my skills, I decided to create this web app as both a fun side project and a portfolio piece. It’s not intended to compete with Monkeytype or any other typing website—just something I built for the joy of creating and to demonstrate what I can do.

Now, with that out of the way, let’s dive into the details…

### Functionality
* Measure words per minute and users accuracy
* Registration and login
* Persistent login state with session token - token with unique ID and expiry date
* Leaderboard
* Tracking stats like total characters typed, and best WPM score

### The Stack
* SQL Database: Supabase with its PostgreSQL DBMS. It's free and extremally easy to work with. Also comes with some ready to go schemas like auth for user data colletion and authentication, but in this case I decided to create a simple, custom user table.
* Blazor server: For this project I am using blazor server with pure razor components. This is a simple and elegant solution for compact webapps. There is a small bit of custom javascript to scroll text as user is typing it.
* Communication with DB: EntityFramework Core - This is an overkill for a database this size, but it is a demonstration of my ability to work with EF.
* Frontend: MudBlazor - Free to use, very pretty and versitile frontend framework. Working with it reminded me of working with devexpress.

### Potential fixes/improvements
* Optimise the way the letters are displayed - right now full text is loaded into the website instead of just the visible letters, which is very taxing on the browser
* Infinite Zen mode instead of loading in fixed number of words (1000 at the moment) - this could be done together with the previous point, dynamically loading/unloading letters unclocks potential for infinite scrolling
* More detailed stats to view
* Graphs and data grids could be prettier
* Light mode / Dark mode selection
