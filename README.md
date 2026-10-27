# Jira Story Scraper

A Windows desktop tool, titled Jira Story Fetcher, that pulls the Jira
stories assigned to a list of people.

## What it does

You sign in with your Jira username and password and enter groups,
user IDs (IDSIDs) and names. The tool then calls the Jira REST API
search (`rest/api/2/search`) for the stories assigned to each person.

## Tech

- C# and WPF
- Jira REST API v2

## Building

Open `JiraStoryReport.sln` in Visual Studio and build it.

## License

Licensed under the Apache License 2.0. See [LICENSE](LICENSE).
