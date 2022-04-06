# Scholarship Application
## Description: 
This program was a legacy application that required upgrading to a more recent version of ASP.NET Core. 
The application was upgraded from ASP.NET Core 2.0 to 6.0.
There was a bug that prevented the Entity Framework databases from auto generating that we fixed. 
We also added a new question to the student profile.  
## Authors: 
Tage Higley,
Dennis Chase,
Hunter Olson,
Cecilia Harvey
### Date: 
March 2022
## Tools Used:
ASP.NET Core 6.0,
SQL,
Entity Framework,
Visual Studio
### Hunter Olson's Contributions
Helped to migrate to dotnet 2.1. Fixed bug that wouldn't allow you to see applications if a judge hasen't rated it. 
### Usage:
**Logging In**
To log in, use an existing username and password, or create a new account. 
- New accounts will default to a student account
- To log in as an Admin, use these credentials: wcs@weber.edu, AdminPassword1
- To log in as a Judge, use these credentials: wcsJudge@weber.edu, JudgePassword1
After logging in, you will be redirected to the dashboard of your account type. 
**View the New Feature**
- Create a new student profile, by selecting create new account. 
- Fill out your login information and click submit
- After successfully creating an account, you will be redirected to the student profile page. 
- In the Clubs and Organizations section you should see a question about High School STEM activities you were involved in. This is the new feature.
- After you've filled out all the required fields you may press save to save your data. 
- When you adit your Student Profile in the future, you'll see that all information has persisted, including the new field.
- You can also see this information from the Judge and Admin accounts. 
- As a judge, you can see this information when reviewing a scholarship application submitted by a student. 
- As an Admin, you can see this information when you are awarding a scholarship to a student whose application has already been judged. 
